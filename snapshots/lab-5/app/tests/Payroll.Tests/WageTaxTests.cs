namespace Payroll.Tests;

public class WageTaxTests
{
    [Fact]
    public void Wage_tax_in_the_first_bracket_only()
    {
        // 33,543.00 a year: 36% = 12,075.48, minus the credit of 3,000 = 9,075.48, / 12 = 756.29
        Assert.Equal(756.29m, WageTax.Monthly(2795.25m, taxCredit: true));
    }

    [Fact]
    public void Wage_tax_over_both_brackets()
    {
        // 50,400.00 a year: 40,000 x 36% + 10,400 x 49.5% = 19,548, minus 3,000 = 16,548, / 12 = 1,379.00
        Assert.Equal(1379.00m, WageTax.Monthly(4200.00m, taxCredit: true));
    }

    [Fact]
    public void Wage_tax_without_the_tax_credit()
    {
        Assert.Equal(1004.00m, WageTax.Monthly(2788.89m, taxCredit: false));
    }

    [Fact]
    public void Wage_tax_is_never_negative()
    {
        Assert.Equal(0m, WageTax.Monthly(400m, taxCredit: true));
    }

    [Theory]
    [InlineData(10.004, 10.00)]
    [InlineData(10.005, 10.01)]
    [InlineData(-10.005, -10.01)]
    public void Money_rounds_half_a_cent_away_from_zero(double amount, double expected)
    {
        Assert.Equal((decimal)expected, Money.Round((decimal)amount));
    }
}
