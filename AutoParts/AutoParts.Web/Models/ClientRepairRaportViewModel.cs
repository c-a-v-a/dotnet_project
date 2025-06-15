using System;
using System.Collections.Generic;

namespace AutoParts.Web.Models
{
    public class ClientRepairReportViewModel
    {
        public int CustomerId { get; set; }
        public DateTime? SelectedMonth { get; set; }
        public int? SelectedVehicleId { get; set; }

        public List<ReportVehicleItem> Vehicles { get; set; } = new();
        public List<RepairReportItem> Orders { get; set; } = new();
    }

    public class ReportVehicleItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class RepairReportItem
    {
        public DateTime Date { get; set; }
        public string Vehicle { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal LaborCost { get; set; }
        public decimal PartsCost { get; set; }
        public decimal Total => LaborCost + PartsCost;
    }
}
