using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Payroll.Tests;

/// <summary>
/// Renders a component to HTML with the HtmlRenderer that is built into ASP.NET Core: no browser, no web server.
/// The HTML is what the page shows when it first loads.
/// </summary>
public static class PageRenderer
{
    public static async Task<string> RenderAsync<TComponent>(
        EmployeeRegister register, IDictionary<string, object?>? parameters = null)
        where TComponent : IComponent
    {
        var services = new ServiceCollection()
            .AddSingleton(register)
            .AddLogging()
            .BuildServiceProvider();

        await using var renderer = new HtmlRenderer(services, NullLoggerFactory.Instance);
        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var view = parameters is null ? ParameterView.Empty : ParameterView.FromDictionary(parameters);
            var output = await renderer.RenderComponentAsync<TComponent>(view);
            return output.ToHtmlString();
        });
    }
}
