using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Api.Domain.Entities;
using Reports.Features.Reportss.Commands.CreateReport;
using Reports.Features.Reportss.Commands.LockReport;
using Reports.Features.Reportss.Commands.UnlockReport;
using Reports.Features.Reportss.Model;
using Reports.Features.Reportss.Queries.GetAllReport;
using Reports.Features.Reportss.Queries.GetAvailableReportTypes;
using Reports.Features.Reportss.Queries.GetReportById;
using System.Text.RegularExpressions;



namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController() : BaseController
    {


        [HttpPost("create")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<ActionResult<string>> CreateReport([FromForm] CreateReportCommand command)
        {
            return Ok(await _mediator.Send(command));
        }



        [HttpPost("unlock-report/{reportId}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<ActionResult<string>> UnlockReport(int reportId)
        {
            return Ok(await _mediator.Send(new UnlockReportCommand { ReportId = reportId }));
        }



        [HttpPost("lock-report/{reportId}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<ActionResult<string>> LockReport(int reportId)
        {
            return Ok(await _mediator.Send(new LockReportCommand { ReportId = reportId }));
        }



        // endpoint to return all reports 
        [HttpPost("all-reports")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<PagedList<GetAllReportModel>> GetAllReports(GetAllReportQuery query)
        {
            return await _mediator.Send(query);

        }


        // endpoint to return report by id

        [HttpGet("{id}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<ActionResult<GetReportByIdModel>> GetReportById(int id)
        {
            return Ok(await _mediator.Send(new GetReportByIdQuery { Id = id }));
        }



        [Authorize]
        [HttpGet("my-available-reports")]
        public async Task<ActionResult<List<string>>> GetMyAvailableReports()
        {
            return Ok(await _mediator.Send(new GetAvailableReportTypesQuery()));
        }

        [HttpGet("Geha-list")]
        public ActionResult<ICollection<string>> GetGehaStatus()
        {
            var statuses = Enum.GetNames<Geha>();
            return Ok(statuses);
        }

        [HttpGet("Level-list")]
        public ActionResult<ICollection<string>> GetLevelStatus()
        {
            var statuses = Enum.GetNames<Level>();
            return Ok(statuses);
        }



        [HttpGet("placeholders")]
        public IActionResult GetPlaceholders([FromQuery] string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
                return BadRequest("Invalid file path");

            try
            {
                var placeholders = new HashSet<string>();
                var placeholderPattern = new Regex(@"\([^\(\)]+\)"); // Matches (Eshara)

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;

                    // 🟢 1. من النصوص العادية
                    var texts = body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();
                    foreach (var text in texts)
                    {
                        var matches = placeholderPattern.Matches(text.Text);
                        foreach (Match match in matches)
                            placeholders.Add(match.Value);
                    }

                    // 🟢 2. من Alt Text بتاع الصور
                    var drawings = body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Drawing>();
                    foreach (var drawing in drawings)
                    {
                        var nonVisualProps = drawing.Descendants<DocumentFormat.OpenXml.Drawing.NonVisualDrawingProperties>().FirstOrDefault();
                        if (nonVisualProps != null)
                        {
                            var altText = nonVisualProps.Description ?? nonVisualProps.Title;

                            if (!string.IsNullOrEmpty(altText))
                            {
                                var matches = placeholderPattern.Matches(altText);
                                foreach (Match match in matches)
                                    placeholders.Add(match.Value);
                            }
                        }
                    }
                }

                if (!placeholders.Any())
                    return Ok(new List<string> { "No placeholders found." });

                return Ok(placeholders.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading Word file: {ex.Message}");
            }
        }



    }

}


