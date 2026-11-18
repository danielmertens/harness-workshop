# T02 — Show the contract

The payslip shows a base salary, but not where it comes from. An employee who works part-time cannot see why
their base salary is lower than the salary in their contract.

## Requirements

### Scenario: the payslip shows the contract

- **Given** an employee in the register
- **When** their payslip is shown
- **Then** there is a section `Contract` between the employee's name and the payslip lines
- **And** it shows these rows, from the register:

  | Label | Value | Example for E001 |
  |---|---|---|
  | `Department` | the department | `HR` |
  | `Hired on` | the hire date as `yyyy-MM-dd` | `2015-09-01` |
  | `Hours` | contract hours of 40 per week | `40 of 40 hours per week` |
  | `Full-time salary` | the monthly salary from the register, as an amount | `4200.00` |
  | `Tax credit` | `Applied` or `Not applied` | `Applied` |

- **And** the payslip lines and amounts do not change

### Scenario: part-time hours

- **Given** a part-time employee, for example with 32 or 22.5 contract hours per week
- **When** their payslip is shown
- **Then** `Hours` shows the hours without trailing zeros: `32 of 40 hours per week`, `22.5 of 40 hours per week`

### Scenario: the birth date stays private

- **Given** an employee with a birth date in the register
- **When** their payslip is shown
- **Then** the birth date does not appear anywhere on the page. A payslip does not need it, and it is personal data.

## Examples

| Given employee | Then the contract section shows |
|---|---|
| E001 (shown when the page opens) | `HR`, `2015-09-01`, `40 of 40 hours per week`, `4200.00`, `Applied` |
| E002 | `Payroll`, `2021-02-15`, `32 of 40 hours per week`, `3600.00`, `Applied` |
| E003 | `Finance`, `2004-01-01`, `36 of 40 hours per week`, `5800.00`, `Not applied` |

The page only shows E001 today. Check E002 and E003 in tests, by rendering the page with another register.

## Technical notes

- **The birth date must not reach the browser at all.** That is the privacy officer's rule under the GDPR: not hidden, not in an attribute, not in an HTML comment. Leaving it out of the visible text is not enough.
- **Full-time will not stay 40 hours.** The 2027 collective agreement may set a different full-time week per department. Use `Employee.FullTimeHoursPerWeek`, not a literal `40`, so there is one place to change.
- **Production runs with Dutch culture (`nl-NL`).** Format dates and hours with the invariant culture, or the page shows `22,5`. Developer machines and the build server do not all use the same culture, so a test that relies on the machine culture can pass on one machine and fail on another.

## Done when

- The section above shows on the page.
- Tests cover the new behaviour and the whole test suite passes.
