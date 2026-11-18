namespace Payroll.Tests;

public class PayslipCalculatorTests
{
    [Fact]
    public void Full_time_payslip()
    {
        var slip = PayslipCalculator.Calculate(TestEmployees.Sanne);

        Assert.Equal([new PayLine("Base salary", 4200.00m)], slip.Earnings);
        Assert.Equal([new PayLine("Wage tax", 1379.00m)], slip.Deductions);
        Assert.Equal(4200.00m, slip.Gross);
        Assert.Equal(2821.00m, slip.Net);
    }

    [Fact]
    public void Part_time_salary_follows_the_contract_hours()
    {
        // 32 of 40 hours: 3600.00 x 0.8
        var slip = PayslipCalculator.Calculate(TestEmployees.Daan);

        Assert.Equal(2880.00m, slip.Gross);
        Assert.Equal(2093.20m, slip.Net);
    }

    [Fact]
    public void No_tax_credit_means_more_wage_tax()
    {
        var slip = PayslipCalculator.Calculate(TestEmployees.Fatima);

        Assert.Equal(5220.00m, slip.Gross);
        Assert.Equal(2133.90m, slip.Deductions.Single(l => l.Label == "Wage tax").Amount);
        Assert.Equal(3086.10m, slip.Net);
    }
}
