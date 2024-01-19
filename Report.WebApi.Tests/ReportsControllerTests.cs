using AppServices.Interfaces;
using Microsoft.Extensions.Logging;
using Report.WebApi.Controllers;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Text;
using Report.Models;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Report.WebApi.Tests;

public class ReportsControllerTests
{
    private readonly ReportsController _reportsController;
    private readonly Mock<IReportService> _reportServiceMock;

    public ReportsControllerTests()
    {
        _reportServiceMock = new();
        _reportsController = new(Mock.Of<ILogger<ReportsController>>(), _reportServiceMock.Object);
    }

    [Fact]
    public async Task GenerateReportTest()
    {
        //arrange
        var reports = Builder<ReportModel>.CreateListOfSize(5).Build();
        var jsonContent = JsonConvert.SerializeObject(reports);
        var jsonFileMock = new Mock<IFormFile>();
        jsonFileMock.Setup(f => f.Length).Returns(jsonContent.Length);
        jsonFileMock.Setup(f => f.ContentType).Returns("application/json");
        jsonFileMock.Setup(f => f.FileName).Returns("reports.json");
        jsonFileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(Encoding.UTF8.GetBytes(jsonContent)));

        var templateContent = "<template-row><firstname/>;<lastname /></template-row>";
        var templateFileMock = new Mock<IFormFile>();
        templateFileMock.Setup(f => f.Length).Returns(templateContent.Length);
        templateFileMock.Setup(f => f.ContentType).Returns("text/plain");
        templateFileMock.Setup(f => f.FileName).Returns("template.txt");
        templateFileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(Encoding.UTF8.GetBytes(templateContent)));

        _reportServiceMock.Setup(e => e.GenerateReport(jsonContent, templateContent))
            .ReturnsAsync(@"""FirstName1;LastName1
                    FirstName2;LastName2
                    FirstName3;LastName3
                    FirstName4;LastName4
                    FirstName5;LastName5
                    """);

        //act
        var result = await _reportsController.GenerateReport(new() { jsonFileMock.Object, templateFileMock.Object });
        var actual = Assert.IsType<FileContentResult>(result);

        //assert
        Assert.Equal("reports.txt", actual.FileDownloadName);
        Assert.Equal("text/plain", actual.ContentType);
        Assert.NotNull(actual.FileContents);
    }
}