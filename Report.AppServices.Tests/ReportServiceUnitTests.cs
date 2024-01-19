using AppServices;
using AppServices.Interfaces;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using Report.Models;
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
            const string template = "<template-row><firstname/>;<lastname /></template-row>";

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
            //prepare
            var reports = Builder<ReportModel>.CreateListOfSize(5).Build();

            string json = JsonConvert.SerializeObject(reports);
            const string template = "<template-row><firstname/>;<firstname /></template-row>";

            //act
            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);
            var result = string.Empty;

            if (invalidTemplates.Count == 0)
            {
                result = await _reportService.GenerateReport(json, template);
            }

            //assert
            Assert.Empty(invalidTemplates);
            Snapshot.Match(result);
        }

        [Fact]
        public async Task GenerateReportSuccessFields1000Test()
        {
            //prepare
            var reports = Builder<ReportModel>.CreateListOfSize(1000).Build();

            string json = JsonConvert.SerializeObject(reports);
            const string template = "<template-row><firstname/>;<lastName />;<dob />;<Field4 />;<Field5 />;<Field6 />;<Field7 />;<Field8 />;<Field9 />;<Field10 />;<Field11 />;<Field12 />;<Field13 />;<Field14 />;<Field15 />;<Field16 />;<Field17 />;<Field18 />;<Field19 />;<Field20 />;<Field21 />;<Field22 />;<Field23 />;<Field24 />;<Field25 />;<Field26 />;<Field27 />;<Field28 />;<Field29 />;<Field30 />;<Field31 />;<Field32 />;<Field33 />;<Field34 />;<Field35 />;<Field36 />;<Field37 />;<Field38 />;<Field39 />;<Field40 />;<Field41 />;<Field42 />;<Field43 />;<Field44 />;<Field45 />;<Field46 />;<Field47 />;<Field48 />;<Field49 />;<Field50 />;<Field51 />;<Field52 />;<Field53 />;<Field54 />;<Field55 />;<Field56 />;<Field57 />;<Field58 />;<Field59 />;<Field60 />;<Field61 />;<Field62 />;<Field63 />;<Field64 />;<Field65 />;<Field66 />;<Field67 />;<Field68 />;<Field69 />;<Field70 />;<Field71 />;<Field72 />;<Field73 />;<Field74 />;<Field75 />;<Field76 />;<Field77 />;<Field78 />;<Field79 />;<Field80 />;<Field81 />;<Field82 />;<Field83 />;<Field84 />;<Field85 />;<Field86 />;<Field87 />;<Field88 />;<Field89 />;<Field90 />;<Field91 />;<Field92 />;<Field93 />;<Field94 />;<Field95 />;<Field96 />;<Field97 />;<Field98 />;<Field99 />;<Field100 /></template-row>";

            //act
            var invalidTemplates = _reportService.ValidateTemplate<ReportModel>(template);
            var result = string.Empty;

            if (invalidTemplates.Count == 0)
            {
                result = await _reportService.GenerateReport(json, template);
            }

            //assert
            Assert.Empty(invalidTemplates);
            Snapshot.Match(result);
        }
    }
}