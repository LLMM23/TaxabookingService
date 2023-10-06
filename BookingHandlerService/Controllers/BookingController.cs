using Microsoft.AspNetCore.Mvc;
using BookingHandlerService.Models;
using System.Text.Json;
using BookingHandlerService.Messaging;

namespace BookingHandlerService.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    string _filepath = string.Empty;
    private readonly ILogger<BookingController> _logger;
    string _mqHost = string.Empty;

    public BookingController(IConfiguration configuration, ILogger<BookingController> logger)
    {
        _filepath = configuration["WorkPath"] ?? string.Empty;
        _mqHost = configuration["MqHost"] ?? string.Empty;
        _logger = logger;
        _logger.LogInformation($"env: {_filepath}, {_mqHost}");
    }

    [HttpPost]
    public void PostBooking(BookingDTO booking)
    {
        PlanDTO planDTO = new PlanDTO()
        {
            CustomerName = booking.CustomerName,
            StartTime = booking.StartTime,
            StartLocation = booking.StartLocation,
            EndLocation = booking.EndLocation
        };

        MessagingHandler.SendDTO(planDTO, _mqHost);
    }

    [HttpGet]
    public IActionResult GetPlanFile()
    {
        List<PlanDTO> planList = ReadPlanCSV();

        planList.OrderByDescending(p => p.StartTime);

        string planFileJson = JsonSerializer.Serialize(planList);

        return Ok(planFileJson);
    }

    private List<PlanDTO> ReadPlanCSV()
    {
        string[] csv = System.IO.File.ReadAllLines(Path.Combine(_filepath, "Plan.csv"));
        List<PlanDTO> plan = new List<PlanDTO>();

        foreach (var line in csv)
        {
            string[] fields = line.Split(",");

            PlanDTO planDTO = new PlanDTO();
            planDTO.CustomerName = fields[0];
            planDTO.StartTime = DateTime.Parse(fields[1]);
            planDTO.StartLocation = fields[2];
            planDTO.EndLocation = fields[3];
            plan.Add(planDTO);
        }

        return plan;
    }
}