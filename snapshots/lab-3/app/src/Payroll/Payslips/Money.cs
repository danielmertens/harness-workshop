using System.Globalization;

namespace Payroll;

/// <summary>Amounts are euros in a decimal. Every amount on a payslip is rounded to whole cents.</summary>
public static class Money
{
    /// <summary>Rounds to whole cents. Half a cent rounds away from zero: 10.005 becomes 10.01.</summary>
    public static decimal Round(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    /// <summary>Two decimals, a '.' as decimal separator, no thousands separator: 1234.50</summary>
    public static string Format(decimal amount) => amount.ToString("0.00", CultureInfo.InvariantCulture);
}
