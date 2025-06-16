namespace AutoParts.Web.Models;

public class OpenOrderItem
{
    public string Vehicle { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public string Mechanic { get; set; } = "";
}
