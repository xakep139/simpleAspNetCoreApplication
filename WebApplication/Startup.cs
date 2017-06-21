using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using WebApplication.Swashbuckle;

namespace WebApplication
{
    public sealed class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddVersionedApiExplorer()
                .AddApiExplorer()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddCors();

            services.AddApiVersioning(options => options.ReportApiVersions = true);

            services.AddSwaggerGen(
                x =>
                {
                    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        x.SwaggerDoc(description.GroupName, new Info { Title = $"Values API {description.ApiVersion}", Version = description.ApiVersion.ToString() });
                    }

                    x.OperationFilter<ImplicitApiVersionParameter>();
                });
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            app.UseMvc();
            app.UseApiVersioning();
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                    options.DocExpansion("list");
                    options.EnabledValidator();
                    options.ShowRequestHeaders();
                });
        }
    }
}
