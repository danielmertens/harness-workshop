using Payroll.Components.Pages;

namespace Payroll.Tests;

public class HomePageTests
{
    [Fact]
    public async Task Shows_the_payslip_of_the_first_employee()
    {
        var html = await PageRenderer.RenderAsync<Home>(TestEmployees.Register());

        Assert.Contains("Sanne de Vries", html);
        Assert.Contains("Employee E001", html);
        Assert.Contains("November 2026", html);
    }

    [Fact]
    public async Task Shows_every_payslip_line_with_its_amount()
    {
        var html = await PageRenderer.RenderAsync<Home>(TestEmployees.Register());

        Assert.Contains("<td>Base salary</td><td class=\"amount\">4200.00</td>", html);
        Assert.Contains("<td>Gross salary</td><td class=\"amount\">4200.00</td>", html);
        Assert.Contains("<td>Wage tax</td><td class=\"amount\">-1379.00</td>", html);
        Assert.Contains("<td>Net salary</td><td class=\"amount\">2821.00</td>", html);
    }
}
