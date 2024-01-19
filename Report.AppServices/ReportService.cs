using AppServices.Interfaces;
using Report.Models;
using Report.Models.Errors;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace AppServices;

public class ReportService : IReportService
{
    readonly string[] _exludedXmlTags = new[] { "template-row" };

    public async Task<string> GenerateReport(string json, string xmlTemplate)
    {
        //convert obj to string, and return if empty
        var reportModels = JsonSerializer.Deserialize<ReportModel[]>(json);
        if (reportModels is null)
        {
            return string.Empty;
        }

        //reformat xml template
        xmlTemplate = XDocument.Parse(xmlTemplate).ToString(SaveOptions.DisableFormatting);
        xmlTemplate = xmlTemplate.ToLowerInvariant();

        //replace not includednxml tags with empty string
        var xmlTagNames = ExtractXmlTags(xmlTemplate, _exludedXmlTags);
        foreach (var e in _exludedXmlTags)
        {
            xmlTemplate = xmlTemplate
                                .Replace($"<{e}>", string.Empty, StringComparison.OrdinalIgnoreCase)
                                .Replace($"</{e}>", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        //get properties that matches with selected xml tags only
        var properties = typeof(ReportModel).GetProperties()
                    .Where(p =>
                        Array.Exists(xmlTagNames, e => string.Equals(e, p.Name, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();

        //leverage by number of tasks to make the process faster
        var tasks = new List<Task<(int, string)>>();
        int pageSize = 5000;
        for (int pageIndex = 0; pageIndex <= (reportModels.Length / pageSize); pageIndex++)
        {
            var items = reportModels.Skip(pageIndex * pageSize).Take(pageSize).ToArray();
            if (items.Length == 0) { continue; }

            tasks.Add(ReplaceXmlTagsWithValues(pageIndex, items, xmlTemplate, properties));
        }

        //wait all tasks to be done
        var doneTasks = await Task.WhenAll(tasks);

        //combine all tasks and their results into one string
        StringBuilder sbResult = new();
        foreach (var task in doneTasks.OrderBy(e => e.Item1))
        {
            sbResult.AppendLine(task.Item2);
        }

        //free tasks
        tasks.Clear();

        return sbResult.ToString().Trim();
    }

    public List<ErrorMessage> ValidateTemplate<T>(string xmlTemplate)
    {
        var result = new List<ErrorMessage>();
        try
        {
            var xmlTagNames = ExtractXmlTags(xmlTemplate, _exludedXmlTags);

            var properties = typeof(T).GetProperties()
                        .Select(prop => prop.Name).ToArray();

            var invalidTagNames = xmlTagNames
                                .Where(e => !Array.Exists(properties, p => string.Equals(p, e, StringComparison.OrdinalIgnoreCase)))
                                .ToArray();

            foreach (var invalidTag in invalidTagNames)
            {
                result.Add(new NotFoundTemplateFieldError(invalidTag));
            }
        }
        catch (Exception ex)
        {
            result.Add(new GenericError(ex));
        }

        return result;
    }

    private static string[] ExtractXmlTags(string xmlTemplate, string[] excluded)
    {
        var tagNames = XDocument.Parse(xmlTemplate)
            .Descendants()
            .Select(e => e.Name.LocalName)
            .Distinct()
            .Where(e => !Array.Exists(excluded, ex => string.Equals(ex, e, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return tagNames;
    }

    private static Task<(int, string)> ReplaceXmlTagsWithValues(int pageIndex, ReportModel[] items, string xmlTemplate, PropertyInfo[] properties)
    {
        StringBuilder sbResult = new();
        foreach (var item in items)
        {
            string template = xmlTemplate;
            foreach (var p in properties)
            {
                var value = p.GetValue(item, null)?.ToString() ?? string.Empty;
                template = template
                            .Replace($"<{p.Name.ToLowerInvariant()} />", value, StringComparison.OrdinalIgnoreCase);
            }

            sbResult.AppendLine(template);
        }

        return Task.FromResult<(int, string)>(new(pageIndex, sbResult.ToString()));
    }
}
