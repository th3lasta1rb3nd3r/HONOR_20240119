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
            const string template = "<template-row>< firstname/></template-row>";

            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);

            Snapshot.Match(invalidTemplates);
        }

        [Fact]
        public void GenerateReportInvalidTemplateFieldTest()
        {
            const string template = "<template-row><firstname/><lastname1 /></template-row>";

            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);

            Snapshot.Match(invalidTemplates);
        }

        [Fact]
        public void GenerateReportSuccessTest()
        {
            var reports = Builder<ReportModel>.CreateListOfSize(5).Build();

            string json = JsonConvert.SerializeObject(reports);
            const string template = "<template-row><firstname/><lastname /></template-row>";

            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);

            if (invalidTemplates.Count > 0)
            {
                return;
            }

            var result = _reportService.GenerateReport(json, template);

            Snapshot.Match(result);
        }

        [Fact]
        public void GenerateReportSuccessDuplicateFieldsTest()
        {
            var reports = Builder<ReportModel>.CreateListOfSize(5).Build();

            string json = JsonConvert.SerializeObject(reports);
            const string template = "<template-row><firstname/><firstname /></template-row>";

            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);

            if (invalidTemplates.Count > 0)
            {
                return;
            }

            var result = _reportService.GenerateReport(json, template);

            Snapshot.Match(result);
        }
    }
}