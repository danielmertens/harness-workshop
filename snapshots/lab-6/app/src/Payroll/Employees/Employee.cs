namespace Payroll;

/// <summary>An employee and their contract, as registered by HR.</summary>
/// <param name="MonthlySalary">Gross salary per month for a full-time contract (40 hours per week).</param>
/// <param name="HoursPerWeek">Contract hours per week. Less than 40 means part-time.</param>
/// <param name="TaxCredit">Whether this employer applies the general tax credit for this employee.</param>
public sealed record Employee(
    string Id,
    string Name,
    string Department,
    DateOnly BirthDate,
    DateOnly HireDate,
    decimal MonthlySalary,
    decimal HoursPerWeek,
    bool TaxCredit)
{
    public const decimal FullTimeHoursPerWeek = 40m;

    /// <summary>1 for full-time, 0.8 for 32 hours, and so on.</summary>
    public decimal PartTimeFactor => HoursPerWeek / FullTimeHoursPerWeek;
}
