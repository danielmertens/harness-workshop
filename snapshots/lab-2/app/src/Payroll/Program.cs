using Payroll;
using Payroll.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(EmployeeRegister.Load(Path.Combine(AppContext.BaseDirectory, "data", "employees.csv")));
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

// Plain http on purpose (see Properties/launchSettings.json): no development certificate needed.
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
