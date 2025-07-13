using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NppesIntake.Core.Services;
using NppesIntake.Infrastructure;
using NppesIntake.Infrastructure.Services;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) => {

        services.AddHttpClient("NppesApiClient", client =>
        {
            client.BaseAddress = new Uri("https://npiregistry.cms.hhs.gov/api/");
        });

        services.AddScoped<INppesApiService, NppesApiService>();
        services.AddScoped<IPrescriberIntakeService, PrescriberIntakeService>();

        var connectionString = context.Configuration.GetConnectionString("SqlConnectionString");
        services.AddDbContext<NppesIntakeDbContext>((sp, options) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("SqlConnectionString");
            options.UseSqlServer(connectionString,
                sqlServerOptions => sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            );
        });
    })
    .Build();

host.Run();