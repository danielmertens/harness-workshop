# T03 — Pension premium

Every employee pays a pension premium. The payslip does not deduct it yet.

## Requirements

### Scenario: base salary above the franchise

- **Given** an employee with a base salary above the franchise of 1750.00 per month
- **When** their payslip is calculated
- **Then** it has a deduction `Pension premium`, shown above `Wage tax`
- **And** the pension premium is 7.5% of the base salary above 1750.00, rounded to whole cents
- **And** the wage tax is calculated on the taxable wage: gross salary − pension premium
- **And** net salary = gross salary − pension premium − wage tax

### Scenario: base salary at or below the franchise

- **Given** an employee with a base salary of 1750.00 or less
- **When** their payslip is calculated
- **Then** the pension premium is 0.00, never negative

### Scenario: part-time employee

- **Given** a part-time employee
- **When** their payslip is calculated
- **Then** the franchise is the same 1750.00 as for a full-time employee: it is not reduced by the part-time factor

## Examples

| Given employee | Base salary | Then pension premium | Wage tax | Net salary |
|---|---|---|---|---|
| E001 (shown when the page opens) | 4200.00 | -183.75 | -1288.04 | 2728.21 |
| E002 (32 hours) | 2880.00 | -84.75 | -756.29 | 2038.96 |
| E005 (24 hours) | 2970.00 | -91.50 | -786.26 | 2092.24 |
| E007 | 2750.00 | -75.00 | -713.00 | 1962.00 |

A base salary of 1700.00 has a pension premium of 0.00.

## Technical notes

- **The rates change every year.** 7.5% and 1750.00 come from the pension fund's regulations for 2026 and change on 1 January. Keep them as named constants in one place, like the rates in `WageTax`.
- **Only the base salary is pensionable.** Earnings that come later (overtime, bonus) will not be. Calculate the premium from the base salary, not from the gross salary, even though both are the same today.
- **This is the employee's share only.** The employer pays a share too, but it does not belong on the payslip.
- **Round the premium first.** Round it to whole cents, then use the rounded premium for the taxable wage. The pension fund reconciles on rounded amounts.

## Done when

- The page shows the pension premium, and the amounts match the examples.
- The rule is described in the payslip rules of `README.md`.
- Tests cover the new behaviour and the whole test suite passes.
