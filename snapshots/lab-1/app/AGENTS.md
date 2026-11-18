# AGENTS.md

Instructions for coding agents working in this repository. What the app does and the payslip rules it follows are
in `README.md`: read it before you change a calculation.

## Code map

| Path | What is in it |
|---|---|
| `src/Payroll/Components/Pages/Home.razor` | The page. |
| `src/Payroll/Components/App.razor` | The HTML shell. The whole app runs in the `InteractiveServer` render mode, so `@onchange` and `@bind` work. |
| `src/Payroll/wwwroot/app.css` | The styles. |
| `src/Payroll/data/employees.csv` | The employee register. Its columns are described in `README.md`. |
| `src/Payroll/Employees/` | `Employee`, `EmployeeRegister` (reads the CSV). |
| `src/Payroll/Payslips/` | `PayslipCalculator` and its rules: `WageTax`, `Money`, `Payslip`. |
| `tests/Payroll.Tests/` | xUnit tests. `PageRenderer` renders a component to an HTML string, so tests can check the page. `TestEmployees.cs` holds the same people as the register. |

## Build and test

```
dotnet build
dotnet test
```

## Rules for changes

- **No new packages.** Do not add NuGet packages to any project (no bUnit, no test or UI libraries). Everything
  this repository needs is in .NET itself.
- **Page tests check the rendered HTML.** `PageRenderer` renders a component to an HTML string with the
  `HtmlRenderer` built into ASP.NET Core. It shows what the page looks like when it opens. To test what the page
  shows for another employee, render it with a register in which that employee comes first, or test the logic
  behind the page directly.
- **Interaction is checked in the browser, by a human.** What happens after a click or a change in a dropdown is
  not covered by unit tests in this repository. Do not build test infrastructure for it.
- You do not need to start the app to check a change. Do not start it from an agent session: it keeps running.
