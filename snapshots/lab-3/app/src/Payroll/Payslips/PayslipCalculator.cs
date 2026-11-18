namespace Payroll;

/// <summary>Calculates the monthly payslip of one employee.</summary>
public static class PayslipCalculator
{
    public static Payslip Calculate(Employee employee)
    {
        var baseSalary = Money.Round(employee.MonthlySalary * employee.PartTimeFactor);

        var earnings = new List<PayLine>
        {
            new("Base salary", baseSalary),
        };

        var gross = earnings.Sum(l => l.Amount);
        var wageTax = WageTax.Monthly(gross, employee.TaxCredit);

        var deductions = new List<PayLine>
        {
            new("Wage tax", wageTax),
        };

        return new Payslip(employee, earnings, deductions);
    }
}
