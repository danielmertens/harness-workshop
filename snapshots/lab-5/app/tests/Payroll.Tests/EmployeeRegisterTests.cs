namespace Payroll.Tests;

public class EmployeeRegisterTests
{
    private const string Header = "id;name;department;birthDate;hireDate;monthlySalary;hoursPerWeek;taxCredit";

    [Fact]
    public void Parses_an_employee_line()
    {
        var register = EmployeeRegister.Parse([Header, "E002;Daan Jansen;Payroll;1992-07-30;2021-02-15;3600.00;32;yes"]);

        Assert.Equal(TestEmployees.Daan, register.Find("E002"));
    }

    [Fact]
    public void Finds_an_employee_case_insensitively()
    {
        Assert.Equal("E001", TestEmployees.Register().Find("e001")?.Id);
    }

    [Fact]
    public void Unknown_employee_is_null()
    {
        Assert.Null(TestEmployees.Register().Find("E999"));
    }

    [Fact]
    public void Wrong_header_is_rejected()
    {
        var error = Assert.Throws<FormatException>(() => EmployeeRegister.Parse(["id,name"]));
        Assert.StartsWith("line 1:", error.Message);
    }

    [Fact]
    public void Bad_field_is_reported_with_its_line_number()
    {
        var error = Assert.Throws<FormatException>(() => EmployeeRegister.Parse(
            [Header, "E002;Daan Jansen;Payroll;1992-07-30;2021-02-15;lots;32;yes"]));

        Assert.Equal("line 2: monthlySalary must be a number greater than 0", error.Message);
    }

    [Fact]
    public void The_shipped_data_file_loads()
    {
        var register = EmployeeRegister.Load(Path.Combine(AppContext.BaseDirectory, "data", "employees.csv"));

        Assert.Equal(TestEmployees.Register().All, register.All);
    }
}
