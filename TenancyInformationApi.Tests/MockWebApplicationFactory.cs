using System.Data.Common;
using TenancyInformationApi;
using TenancyInformationApi.V1.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TenancyInformationApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly DbConnection _connection;

        public MockWebApplicationFactory(DbConnection connection)
        {
            _connection = connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            builder.ConfigureServices((System.Action<IServiceCollection>) (services =>
             {
                 var dbBuilder = new DbContextOptionsBuilder();
                 dbBuilder.UseNpgsql(_connection);
                 var context = new UhContext((DbContextOptions) dbBuilder.Options);
                 services.AddSingleton(context);

                 var serviceProvider = services.BuildServiceProvider();
                 var dbContext = serviceProvider.GetRequiredService<UhContext>();

                 dbContext.Database.EnsureCreated();
             }));
        }
    }
}
