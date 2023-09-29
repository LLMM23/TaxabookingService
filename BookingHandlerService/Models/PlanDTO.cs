﻿namespace BookingHandlerService.Models
{
    public class PlanDTO
    {
        public string CustomerName { get; set; }
        public DateTime StartTime { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
    }
}
