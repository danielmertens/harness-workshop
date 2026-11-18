using System.Globalization;

namespace Payroll;

/// <summary>All employees, read from a semicolon-separated file with a header line.</summary>
public sealed class EmployeeRegister
{
    private static readonly string[] Columns =
        ["id", "name", "department", "birthDate", "hireDate", "monthlySalary", "hoursPerWeek", "taxCredit"];

    private readonly Dictionary<string, Employee> _byId;

    public EmployeeRegister(IEnumerable<Employee> employees)
    {
        All = employees.OrderBy(e => e.Id, StringComparer.Ordinal).ToList();
        _byId = All.ToDictionary(e => e.Id, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<Employee> All { get; }

    public Employee? Find(string id) => _byId.GetValueOrDefault(id);

    public static EmployeeRegister Load(string path) => Parse(File.ReadAllLines(path));

    public static EmployeeRegister Parse(IReadOnlyList<string> lines)
    {
        if (lines.Count == 0 || lines[0].Trim() != string.Join(';', Columns))
            throw new FormatException($"line 1: expected the header '{string.Join(';', Columns)}'");

        var employees = new List<Employee>();
        for (var i = 1; i < lines.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            employees.Add(ParseLine(lines[i], lineNumber: i + 1));
        }
        return new EmployeeRegister(employees);
    }

    private static Employee ParseLine(string line, int lineNumber)
    {
        var f = line.Split(';');
        if (f.Length != Columns.Length)
            throw new FormatException($"line {lineNumber}: expected {Columns.Length} fields, found {f.Length}");

        return new Employee(
            Id: f[0].Trim(),
            Name: f[1].Trim(),
            Department: f[2].Trim(),
            BirthDate: Date(f[3], "birthDate", lineNumber),
            HireDate: Date(f[4], "hireDate", lineNumber),
            MonthlySalary: Number(f[5], "monthlySalary", lineNumber),
            HoursPerWeek: Number(f[6], "hoursPerWeek", lineNumber),
            TaxCredit: f[7].Trim() switch
            {
                "yes" => true,
                "no" => false,
                _ => throw new FormatException($"line {lineNumber}: taxCredit must be 'yes' or 'no'"),
            });
    }

    private static DateOnly Date(string text, string column, int lineNumber) =>
        DateOnly.TryParseExact(text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
            ? d
            : throw new FormatException($"line {lineNumber}: {column} must be a date like 2026-01-31");

    private static decimal Number(string text, string column, int lineNumber) =>
        decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var n) && n > 0
            ? n
            : throw new FormatException($"line {lineNumber}: {column} must be a number greater than 0");
}
