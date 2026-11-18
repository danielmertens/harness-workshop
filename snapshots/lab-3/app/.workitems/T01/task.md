# T01 — Choose the employee

The payslip page always shows the first employee in the register. HR wants to look at anyone's payslip.

## Requirements

### Scenario: the page opens

- **Given** the employee register
- **When** the page opens
- **Then** above the payslip there is a dropdown with the label `Employee`
- **And** it lists every employee in the register, ordered by employee number, each option showing the number and
  the name, for example `E002 Daan Jansen`
- **And** the first employee is selected and their payslip is shown, as today

### Scenario: choosing another employee

- **Given** the page is open
- **When** HR chooses another employee in the dropdown
- **Then** that employee's payslip is shown straight away, without a button and without reloading the page
- **And** the payslip itself does not change: same lines, same amounts as that employee would get today

## Examples

| Given / When | Then the page shows |
|---|---|
| The page opens | `Sanne de Vries`, `Employee E001`, `Net salary` 2821.00, and `E001 Sanne de Vries` selected |
| You choose `E002 Daan Jansen` | `Daan Jansen`, `Employee E002`, `Base salary` 2880.00, `Wage tax` -786.80, `Net salary` 2093.20 |
| You choose `E003 Fatima El Amrani` | `Net salary` 3086.10 |

## Technical notes

- **Select by employee number, never by name.** Names are not unique in the real register (there are two
  `J. de Vries`), so the option value and the lookup use the employee number.
- **A plain dropdown is enough.** The production register has about 150 employees. No search box, no paging.
- **No deep link yet.** The chosen employee does not go into the URL and does not need to survive a reload. A later
  work item adds `/payslip/{id}`; do not add routing now.

## Done when

- The behaviour above works in the browser.
- Tests cover the new behaviour (at least: the dropdown lists all seven employees in order, and the first one is
  selected when the page opens) and the whole test suite passes.
