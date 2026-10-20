using System;
using System.Net.Http.Headers;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.AnimeThemes;

/// <inheritdoc />
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<AnimeThemesDownloader>();

        var productHeader = new ProductInfoHeaderValue(
            "jf-plugin-animethemes",
            applicationHost.ApplicationVersionString);

        serviceCollection.AddTransient<PollyResilienceHandler>();

        serviceCollection
            .AddHttpClient("AnimeThemes", c =>
            {
                c.BaseAddress = new Uri("https://api.animethemes.moe");
                c.DefaultRequestHeaders.UserAgent.Add(productHeader);
            })
            .AddHttpMessageHandler<PollyResilienceHandler>();

        // Media files can be large, so this client gets a generous timeout.
        serviceCollection
            .AddHttpClient(AnimeThemesDownloader.DownloadClientName, c =>
            {
                c.Timeout = TimeSpan.FromMinutes(30);
                c.DefaultRequestHeaders.UserAgent.Add(productHeader);
            })
            .AddHttpMessageHandler<PollyResilienceHandler>();
    }
}
