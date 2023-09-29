using Microsoft.AspNetCore.Mvc;
using BookingHandlerService.Models;
using System.Text.Json;

namespace BookingHandlerService.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    string _filepath = string.Empty;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IConfiguration configuration, ILogger<BookingController> logger)
    {
        _filepath = configuration["filepath"] ?? String.Empty;
        _logger = logger;
    }

    [HttpPost]
    public void PostBooking(BookingDTO booking)
    {

    }

    [HttpGet]
    public IActionResult GetPlanFile()
    {
        string planFileJson = JsonSerializer.Serialize(ReadPlanCSV());

        return Ok(planFileJson);
    }

    public List<PlanDTO> ReadPlanCSV()
    {
        string[] csv = System.IO.File.ReadAllLines(_filepath);
        List<PlanDTO> plan = new List<PlanDTO>();

        foreach (var line in csv)
        {
            string[] fields = line.Split(",");

            PlanDTO planDTO = new PlanDTO();
            planDTO.CustomerName = fields[0];
            planDTO.StartTime = DateTime.Parse(fields[1]);
            planDTO.StartLocation = fields[2];
            planDTO.EndLocation = fields[3];
        }

        return plan;
    }
}
