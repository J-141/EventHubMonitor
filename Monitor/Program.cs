using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Services.Logger;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Monitor;
using Services.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var loggerProvider = new BlazorComponentLoggerProvider();
builder.Services.AddSingleton(loggerProvider);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddLogging(config => {
    config.AddProvider(loggerProvider);
});

builder.Services.AddSingleton<IListenerClientFactory, ListenerClientFactory>();
builder.Services.AddSingleton<ISenderClientFactory, SenderClientFactory>();

await builder.Build().RunAsync();
