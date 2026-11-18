namespace Payroll;

/// <summary>
/// Wage tax with two brackets and an optional tax credit. The monthly tax is calculated on an annual basis:
/// the month's taxable wage times 12, through the brackets, minus the credit, divided by 12.
/// </summary>
public static class WageTax
{
    public const decimal FirstBracketLimit = 40_000m;
    public const decimal FirstBracketRate = 0.36m;
    public const decimal SecondBracketRate = 0.495m;
    public const decimal AnnualTaxCredit = 3_000m;

    public static decimal Annual(decimal annualTaxableWage, bool taxCredit)
    {
        if (annualTaxableWage <= 0) return 0m;

        var first = Math.Min(annualTaxableWage, FirstBracketLimit) * FirstBracketRate;
        var second = Math.Max(0m, annualTaxableWage - FirstBracketLimit) * SecondBracketRate;
        var credit = taxCredit ? AnnualTaxCredit : 0m;
        return Math.Max(0m, first + second - credit);
    }

    public static decimal Monthly(decimal monthlyTaxableWage, bool taxCredit) =>
        Money.Round(Annual(monthlyTaxableWage * 12, taxCredit) / 12);
}
