namespace Application.DTOs;

public class DashboardStatisticsDto // what JSON get returned to the frontend
{
    public int TotalUsers { get; set; }
    public int FullyImmunised { get; set; }
    public int PartiallyImmunised { get; set; }
    public int NonImmunised { get; set; }
    public int Overdue { get; set; }
    public decimal ImmunisationCompletionRate { get; set; }
}