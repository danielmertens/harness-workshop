# payroll

A one-page Blazor app that shows a monthly payslip for an employee in the HR register.

All rates and rules below are simplified and made up for this repository. They are not real Dutch tax rates.

```
dotnet run --project src/Payroll
```

Then open <http://localhost:5080>. The app uses plain http on purpose, so no development certificate is needed.

## What the page shows today

The payslip for November 2026 of the **first** employee in the register: name, employee number, base salary,
gross salary, wage tax and net salary. The register holds more than the page shows.

## The employee register

`src/Payroll/data/employees.csv`, separated by semicolons, loaded once at startup (`EmployeeRegister`).

| Column | Meaning |
|---|---|
| `id` | Employee number, such as `E001`. |
| `name`, `department` | As registered by HR. |
| `birthDate`, `hireDate` | Dates as `yyyy-MM-dd`. |
| `monthlySalary` | Gross salary per month for a **full-time** contract of 40 hours per week. |
| `hoursPerWeek` | Contract hours per week. Less than 40 means part-time. |
| `taxCredit` | `yes` or `no`: whether the employer applies the tax credit. |

The same people are in `tests/Payroll.Tests/TestEmployees.cs`.

## Payslip rules

- **Amounts** are rounded to whole cents, half a cent away from zero (`Money.Round`), and shown with two decimals
  and a `.` (`Money.Format`). Deductions are shown with a minus sign.
- **Base salary** = monthly salary × part-time factor. The part-time factor is hours per week / 40.
- **Gross salary** = the sum of all earnings. Today the only earning is the base salary.
- **Wage tax** is calculated on a yearly basis: the taxable wage of the month (today: the gross salary) × 12,
  through the brackets, minus the tax credit if the employee has one, never below 0, divided by 12.

  | Yearly taxable wage | Rate |
  |---|---|
  | up to 40,000 | 36% |
  | above 40,000 | 49.5% |

  The tax credit is 3,000 per year.
- **Net salary** = gross salary − all deductions.

## Build and test

```
dotnet build
dotnet test
```
