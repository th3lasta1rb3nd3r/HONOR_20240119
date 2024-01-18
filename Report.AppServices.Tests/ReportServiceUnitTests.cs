using AppServices;
using AppServices.Interfaces;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using Reporting.Models;
using Snapshooter.Xunit;
using System;

namespace Report.AppServices.Tests
{
    public class ReportServiceUnitTests
    {
        private readonly IReportService _reportService;

        public ReportServiceUnitTests()
        {
            _reportService = new ReportService();
        }

        [Fact]
        public void GenerateReportInvalidXmlTest()
        {
            //prepare
            const string template = "<template-row>< firstname/></template-row>";

            //act
            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);

            //assert
            Snapshot.Match(invalidTemplates);
        }

        [Fact]
        public void GenerateReportInvalidTemplateFieldTest()
        {
            //prepare
            const string template = "<template-row><firstname/><lastname1 /></template-row>";

            //act
            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);

            //assert
            Snapshot.Match(invalidTemplates);
        }

        [Fact]
        public async Task GenerateReportSuccessTest()
        {
            //prepare
            var reports = Builder<ReportModel>.CreateListOfSize(5).Build();

            string json = JsonConvert.SerializeObject(reports);
            const string template = "<template-row><firstname/><lastname /></template-row>";

            //act
            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);
            var result = await _reportService.GenerateReport(json, template);

            //assert
            Assert.Empty(invalidTemplates);
            Snapshot.Match(result);
        }

        [Fact]
        public async Task GenerateReportSuccessDuplicateFieldsTest()
        {
            //pre[are
            var reports = Builder<ReportModel>.CreateListOfSize(5).Build();

            string json = JsonConvert.SerializeObject(reports);
            const string template = "<template-row><firstname/><firstname /></template-row>";

            //act
            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);
            var result = await _reportService.GenerateReport(json, template);

            //assert
            Assert.Empty(invalidTemplates);
            Snapshot.Match(result);
        }

        [Fact]
        public async Task GenerateReportSuccessFields10000Test()
        {
            //pre[are
            var reports = Builder<ReportModel>.CreateListOfSize(1000000).Build();

            string json = JsonConvert.SerializeObject(reports);
            const string template = "<template-row><firstname/><lastName /></template-row>";

            //act
            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);
            var result = await _reportService.GenerateReport(json, template);

            //assert
            Assert.Empty(invalidTemplates);
            Snapshot.Match(result);
        }
    }
}