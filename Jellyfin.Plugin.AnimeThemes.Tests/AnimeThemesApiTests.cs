using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Plugin.AnimeThemes.Tests;

public sealed class AnimeThemesApiTests
{
    private static string AnimeJson(int id, int? aniDbId)
    {
        var resources = aniDbId is null
            ? "[]"
            : $"[{{\"id\":{id},\"external_id\":{aniDbId},\"site\":\"aniDB\"}}]";
        return $"{{\"id\":{id},\"name\":\"n{id}\",\"slug\":\"s{id}\",\"year\":2000,\"resources\":{resources},\"animethemes\":[]}}";
    }

    private static string Page(params string[] anime) => "{\"anime\":[" + string.Join(",", anime) + "]}";

    private sealed class FakeFactory(Func<HttpRequestMessage, string> responder) : IHttpClientFactory
    {
        public List<string> Requests { get; } = [];

        public HttpClient CreateClient(string name) => new(new FakeHandler(this, responder)) { BaseAddress = new Uri("https://example.test") };

        private sealed class FakeHandler(FakeFactory owner, Func<HttpRequestMessage, string> responder) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                owner.Requests.Add(request.RequestUri!.PathAndQuery);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responder(request), Encoding.UTF8, "application/json")
                });
            }
        }
    }

    [Fact]
    public async Task EmptyIdListDoesNotCallApi()
    {
        var factory = new FakeFactory(_ => Page());
        using var api = new AnimeThemesApi(factory, NullLogger<AnimeThemesDownloader>.Instance);

        var result = await api.FindByAniDbId([]);

        Assert.Empty(result);
        Assert.Empty(factory.Requests);
    }

    [Fact]
    public async Task ReturnsEntryForEveryRequestedIdAndGroupsDuplicates()
    {
        var factory = new FakeFactory(_ => Page(AnimeJson(1, 10), AnimeJson(2, 10), AnimeJson(3, null)));
        using var api = new AnimeThemesApi(factory, NullLogger<AnimeThemesDownloader>.Instance);

        var result = await api.FindByAniDbId([10, 20]);

        Assert.Equal(2, result[10].Length);
        Assert.Empty(result[20]);
    }

    [Fact]
    public async Task FollowsPagination()
    {
        var factory = new FakeFactory(request =>
            request.RequestUri!.Query.Contains("page%5Bnumber%5D=1", StringComparison.Ordinal)
                ? Page(Enumerable.Range(1, 100).Select(i => AnimeJson(i, 10)).ToArray())
                : Page(AnimeJson(101, 10)));
        using var api = new AnimeThemesApi(factory, NullLogger<AnimeThemesDownloader>.Instance);

        var result = await api.FindByAniDbId([10]);

        Assert.Equal(101, result[10].Length);
        Assert.Equal(2, factory.Requests.Count);
    }

    [Fact]
    public async Task ToleratesMissingResources()
    {
        var factory = new FakeFactory(_ => "{\"anime\":[{\"id\":1,\"name\":\"a\",\"slug\":\"a\"}]}");
        using var api = new AnimeThemesApi(factory, NullLogger<AnimeThemesDownloader>.Instance);

        var result = await api.FindByAniDbId([10]);

        Assert.Empty(result[10]);
    }
}
