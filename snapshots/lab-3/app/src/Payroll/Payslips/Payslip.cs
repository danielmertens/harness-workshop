namespace Payroll;

/// <summary>One line on a payslip. Amounts are positive; whether a line adds or subtracts follows from its list.</summary>
public sealed record PayLine(string Label, decimal Amount);

public sealed record Payslip(Employee Employee, IReadOnlyList<PayLine> Earnings, IReadOnlyList<PayLine> Deductions)
{
    public decimal Gross => Earnings.Sum(l => l.Amount);
    public decimal Net => Gross - Deductions.Sum(l => l.Amount);
}
