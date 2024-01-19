using AppServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Report.Models.Errors;
using Report.WebApi.Errors;
using System.Text;

namespace Report.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IReportService _reportService;

        public ReportsController(ILogger<ReportsController> logger, IReportService reportService)
        {
            _logger = logger;
            _reportService = reportService;
        }

        /// <summary>
        /// generate reports
        /// </summary>
        /// <param name="files">a json file and tempalte (xml format) are required</param>
        /// <returns>file text</returns>
        [HttpPost(nameof(GenerateReport))]
        public async Task<IActionResult> GenerateReport(List<IFormFile> files)
        {
            try
            {
                var errorMessages = new List<ErrorMessage>();
                if (files == null || files.Count == 0)
                {
                    errorMessages.Add(new NoFilesError());
                    return BadRequest(errorMessages);
                }

                var templateContent = await ValidateAndGetTemplateContent(files, errorMessages);
                var jsonContent = await ValidateAndGetJsonContent(files, errorMessages);
                if (errorMessages.Any())
                {
                    return BadRequest(errorMessages);
                }

                var result = await _reportService.GenerateReport(jsonContent, templateContent);
                var byteArray = ConvertStringToBytes(result);

                return File(byteArray, "text/plain", "reports.txt");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GenerateReport: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// validate and get content of a json file
        /// </summary>
        /// <param name="files"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        private static async Task<string> ValidateAndGetJsonContent(List<IFormFile> files, List<ErrorMessage> errorMessages)
        {
            var jsonFile = GetFileByExtensionAndContentType(files, ".json", "application/json");
            if (jsonFile == null)
            {
                errorMessages.Add(new InvalidFileTypeError("Please provide a valid JSON file for datasource."));
                return string.Empty;
            }

            return await GetContent(jsonFile);
        }

        /// <summary>
        /// validate and get template content
        /// </summary>
        /// <param name="files"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        private async Task<string> ValidateAndGetTemplateContent(List<IFormFile> files, List<ErrorMessage> errorMessages)
        {
            var templateFile = GetFileByExtensionAndContentType(files, ".txt", "text/plain");
            if (templateFile == null)
            {
                errorMessages.Add(new InvalidFileTypeError("Please provide a valid TXT file for template."));
                return string.Empty;
            }

            var templateContent = await GetContent(templateFile);
            var errors = _reportService.ValidateReportTemplate(templateContent);
            if (errors != null && errors.Any())
            {
                errorMessages.AddRange(errors);
            }

            return templateContent;
        }

        /// <summary>
        /// get file by extension and content type
        /// </summary>
        /// <param name="files"></param>
        /// <param name="extension"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private static IFormFile? GetFileByExtensionAndContentType(List<IFormFile> files, string extension, string contentType)
        {
            return files.SingleOrDefault(e =>
                Path.GetExtension(e.FileName).Equals(extension, StringComparison.OrdinalIgnoreCase)
                && e.ContentType == contentType);
        }

        private static async Task<string> GetContent(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            using var streamReader = new StreamReader(file.OpenReadStream());
            return await streamReader.ReadToEndAsync();
        }

        private static byte[] ConvertStringToBytes(string data)
        {
            Encoding encoding = Encoding.UTF8;
            return encoding.GetBytes(data);
        }
    }
}
