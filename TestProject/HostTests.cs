using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using WebApplication;
using Xunit;
using System.Linq;

namespace TestProject
{
    public class HostTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public HostTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
            ServicePointManager.DefaultConnectionLimit = 100;
        }

        [Theory]
        [InlineData("1.0")]
        public async Task TestGetAvailableElementDescriptors(string version)
        {
            var @params = new[]
            {
                "123/versions",
                "321/versions",
                "12345/versions",
                "54321/versions",
                "1230/versions",
                "3210/versions",
                "123450/versions",
                "543210/versions",
                "12345/my_version",
                "54321/my_second_version"
            };

            var tasks = @params.Select(async param => await _client.GetAsync($"/api/{version}/values/{param}"));

            var results = await Task.WhenAll(tasks);
            foreach (var response in results)
            {
                response.EnsureSuccessStatusCode();
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
        }
    }
}
