using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RibbitReels.Data.DTOs;
using Xunit;

namespace RibbitReels.IntegrationTests.Branches;

public class BranchFlowTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;

    public BranchFlowTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UserCanCompleteBranchFlow()
    {
        var login = await LoginAsAdmin("test@ribbit.com", "12345678");
        SetAuthHeader(login.Token);

        var branch = await CreateBranch("Intro to Rust", "Learn Rust fast");

        var createdLeaf1 = await CreateLeaf(branch.Id, "Basics", "Variables in Rust", "http://video1.com");
        var createdLeaf2 = await CreateLeaf(branch.Id, "Ownership", "Memory safety", "http://video2.com");

        await MarkLeafComplete(branch.Id, createdLeaf1.Id);
        await MarkLeafComplete(branch.Id, createdLeaf2.Id);

        var progress = await GetProgress(branch.Id);
        Assert.NotNull(progress);
        Assert.Equal(branch.Id, progress.BranchId);
        Assert.Equal(2, progress.CompletedLeafIds.Count);
        Assert.True(progress.PercentageCompleted >= 100);
        Assert.NotNull(progress.CompletedAt);
    }

    private async Task<AuthResponse> LoginAsAdmin(string email, string password)
    {
        var payload = new { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);
        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Login failed: {response.StatusCode}, {content}");

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result!.Token));

        return result;
    }

    private void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<BranchResponse> CreateBranch(string title, string description)
    {
        var payload = new { Title = title, Description = description };
        var response = await _client.PostAsJsonAsync("/api/branch", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var branch = await response.Content.ReadFromJsonAsync<BranchResponse>();
        Assert.NotNull(branch);
        Assert.NotEqual(Guid.Empty, branch!.Id);

        return branch;
    }

    private async Task<LeafResponse> CreateLeaf(Guid branchId, string title, string content, string videoUrl)
    {
        var payload = new
        {
            Title = title,
            Content = content,
            VideoUrl = videoUrl,
            BranchId = branchId
        };

        var response = await _client.PostAsJsonAsync($"/api/leaf/{branchId}", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var leaf = await response.Content.ReadFromJsonAsync<LeafResponse>();
        Assert.NotNull(leaf);
        return leaf!;
    }

    private async Task MarkLeafComplete(Guid branchId, Guid leafId)
    {
        var payload = new
        {
            BranchId = branchId,
            CompletedLeafIds = new List<Guid> { leafId }
        };

        var response = await _client.PutAsJsonAsync("/api/UserLearningProgress", payload);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"ERROR: {response.StatusCode} => {content}");
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    private async Task<LearningProgressResponse> GetProgress(Guid branchId)
    {
        var response = await _client.GetAsync($"/api/UserLearningProgress?branchId={branchId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var progress = await response.Content.ReadFromJsonAsync<LearningProgressResponse>();
        Assert.NotNull(progress);

        return progress!;
    }

}
