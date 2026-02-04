namespace Application.Models;

public class DashboardStatistics // this is what stored procedure returns
{
    public int TotalUsers { get; set; }
    public int FullyImmunised { get; set; }
    public int PartiallyImmunised { get; set; }
    public int NonImmunised { get; set; }
    public int Overdue { get; set; }
}