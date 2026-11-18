namespace Payroll.Tests;

/// <summary>The same people as in src/Payroll/data/employees.csv, so tests and the command line agree.</summary>
public static class TestEmployees
{
    public static readonly Employee Sanne = new("E001", "Sanne de Vries", "HR",
        new DateOnly(1985, 3, 12), new DateOnly(2015, 9, 1), 4200.00m, 40, TaxCredit: true);

    public static readonly Employee Daan = new("E002", "Daan Jansen", "Payroll",
        new DateOnly(1992, 7, 30), new DateOnly(2021, 2, 15), 3600.00m, 32, TaxCredit: true);

    public static readonly Employee Fatima = new("E003", "Fatima El Amrani", "Finance",
        new DateOnly(1978, 1, 1), new DateOnly(2004, 1, 1), 5800.00m, 36, TaxCredit: false);

    public static readonly Employee Lucas = new("E004", "Lucas Bakker", "IT",
        new DateOnly(1999, 11, 5), new DateOnly(2026, 3, 15), 3100.00m, 40, TaxCredit: true);

    public static readonly Employee Noor = new("E005", "Noor Visser", "HR",
        new DateOnly(1968, 5, 20), new DateOnly(1996, 4, 1), 4950.00m, 24, TaxCredit: true);

    public static readonly Employee Thomas = new("E006", "Thomas Smit", "Sales",
        new DateOnly(1990, 12, 31), new DateOnly(2019, 6, 1), 2850.00m, 40, TaxCredit: false);

    public static readonly Employee Emma = new("E007", "Emma Mulder", "Payroll",
        new DateOnly(2001, 2, 28), new DateOnly(2026, 11, 16), 2750.00m, 40, TaxCredit: true);

    public static EmployeeRegister Register() => new([Sanne, Daan, Fatima, Lucas, Noor, Thomas, Emma]);
}
