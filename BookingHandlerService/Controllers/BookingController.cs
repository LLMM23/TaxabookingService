using Microsoft.AspNetCore.Mvc;
using BookingHandlerService.Models;
using System.Text.Json;
using BookingHandlerService.Messaging;
using System.Diagnostics;

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

        var hostName = System.Net.Dns.GetHostName();
        var ips = System.Net.Dns.GetHostAddresses(hostName);
        var ipaddress = ips.First().MapToIPv4().ToString();
        _logger.LogInformation(1, $"BookingHandler responding from {ipaddress}");
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

        _logger.LogInformation("Endpoint for storing booking called");

        MessagingHandler.SendDTO(planDTO, _mqHost);
    }

    [HttpGet]
    public IActionResult GetPlanFile()
    {
        _logger.LogInformation("Get plan endpoint called");

        List<PlanDTO> planList = ReadPlanCSV();

        planList.OrderByDescending(p => p.StartTime);

        string planFileJson = JsonSerializer.Serialize(planList);

        return Ok(planFileJson);
    }

    [HttpGet("version")]
    public async Task<Dictionary<string, string>> GetVersion()
    {
        _logger.LogInformation("Version endpoint called");

        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;
        properties.Add("service", "qgt-customer-service");
        var ver = FileVersionInfo.GetVersionInfo(typeof(Program)
        .Assembly.Location).ProductVersion;
        properties.Add("version", ver!);
        try
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
            var ipa = ips.First().MapToIPv4().ToString();
            properties.Add("hosted-at-address", ipa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            properties.Add("hosted-at-address", "Could not resolve IP-address");
        }
        return properties;
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