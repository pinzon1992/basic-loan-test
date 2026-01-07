using Fundo.Applications.Application.Loans.Models;
using Fundo.Applications.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public sealed class LoanManagementControllerTests : IAsyncLifetime
    {
        private MsSqlContainer _container = null!;
        private HttpClient _client = null!;
        private TestWebApplicationFactory _factory = null!;

        public async Task InitializeAsync()
        {
            _container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("Your_password123!")
                .WithCleanUp(true)
                .Build();

            await _container.StartAsync();

            _factory = new TestWebApplicationFactory(_container.GetConnectionString());
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            _factory.Dispose();
            await _container.DisposeAsync();
        }

        [Fact]
        public async Task GetAllLoans_ReturnsOk()
        {
            var response = await _client.GetAsync("/loans");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Get_And_Payment_Workflow()
        {
            var loan = new LoanDto
            {
                Amount = 1000m,
                CurrentBalance = 1000m,
                ApplicantName = "Test User",
                Status = LoanStatus.Active
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loan),
                Encoding.UTF8,
                "application/json");

            var createResp = await _client.PostAsync("/loans", content);
            createResp.EnsureSuccessStatusCode();

            var created = JsonSerializer.Deserialize<LoanDto>(
                await createResp.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(created);
            Assert.NotNull(created!.Id);

            var getResp = await _client.GetAsync($"/loans/{created.Id}");
            getResp.EnsureSuccessStatusCode();

            var payment = new { Amount = 200m };
            var payContent = new StringContent(
                JsonSerializer.Serialize(payment),
                Encoding.UTF8,
                "application/json");

            var payResp = await _client.PostAsync(
                $"/loans/{created.Id}/payment",
                payContent);

            payResp.EnsureSuccessStatusCode();

            var afterPayment = JsonSerializer.Deserialize<LoanDto>(
                await payResp.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(800m, afterPayment!.CurrentBalance);
        }
    }
}