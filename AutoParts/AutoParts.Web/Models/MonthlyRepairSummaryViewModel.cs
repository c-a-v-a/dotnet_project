namespace AutoParts.Web.Models;

public class MonthlyRepairSummaryViewModel
{
    public DateTime? SelectedMonth { get; set; }

    public List<MonthlyRepairItem> Items { get; set; } = new();
}

public class MonthlyRepairItem
{
    public string CustomerName { get; set; } = "";
    public string Vehicle { get; set; } = "";
    public int ServiceCount { get; set; }
    public decimal TotalCost { get; set; }
}
