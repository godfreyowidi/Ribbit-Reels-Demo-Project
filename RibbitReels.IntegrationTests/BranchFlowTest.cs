using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using RibbitReels.Data.DTOs;

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
        // A. login as admin
        var adminToken = await RegisterAndLoginTestAdminAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // B. admin creates a branch
        var branch = await CreateBranch("Introduction to Sustainable Afforestation", "Learn how to increase forest cover");

        // C. admin adds leaves
        var createdLeaf1 = await CreateLeaf(branch.Id, "What is Afforestation", "How it differs with reforestation", "http://video1.com");
        var createdLeaf2 = await CreateLeaf(branch.Id, "Methods", "Allowing natural regeneration", "http://video2.com");

        // D. register user & get userId + token
        var (userId, userToken) = await RegisterTestUserAndReturnIdAndTokenAsync();

        // E. admin assigns branch to the user
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var assignPayload = new
        {
            UserId = userId,
            BranchId = branch.Id,
            AssignedByManagerId = await GetUserIdFromTokenAsync(adminToken)
        };

        var assignResponse = await _client.PostAsJsonAsync("/api/UserBranchAssignment/assign", assignPayload);
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);

        // F. switch to learner context
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        // G. learner marks leaf as complete
        await MarkLeafComplete(branch.Id, createdLeaf1.Id);
        await MarkLeafComplete(branch.Id, createdLeaf2.Id);

        // H. check learning progress
        var progress = await GetProgress(branch.Id);
        Assert.NotNull(progress);
        Assert.Equal(branch.Id, progress.BranchId);
        Assert.Equal(2, progress.CompletedLeafIds.Count);
        Assert.True(progress.PercentageCompleted >= 100);
        Assert.NotNull(progress.CompletedAt);
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

    private async Task<(Guid userId, string token)> RegisterTestUserAndReturnIdAndTokenAsync()
    {
        var testEmail = $"user-{Guid.NewGuid()}@test.com";
        var password = "StrongP@ssw0rd!";

        var registerPayload = new
        {
            Email = testEmail,
            Password = password,
            ConfirmPassword = password,
            DisplayName = "Assigned User"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var registerData = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = Guid.Parse(registerData.GetProperty("userId").GetString()!);

        var loginPayload = new { Email = testEmail, Password = password };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginPayload);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return (userId, loginResult!.Token);
    }

    private async Task<string> RegisterAndLoginTestAdminAsync()
    {
        var testEmail = $"admin-{Guid.NewGuid()}@test.com";
        var password = "AdminTest123!";

        var registerPayload = new
        {
            Email = testEmail,
            DisplayName = "CI Test Admin",
            Password = password,
            ConfirmPassword = password
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register-admin", registerPayload);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginPayload = new
        {
            Email = testEmail,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginPayload);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginResult?.Token);

        return loginResult!.Token!;
    }

    private async Task<Guid> GetUserIdFromTokenAsync(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await response.Content.ReadFromJsonAsync<JsonElement>();
        return Guid.Parse(data.GetProperty("id").GetString()!);
    }
}
