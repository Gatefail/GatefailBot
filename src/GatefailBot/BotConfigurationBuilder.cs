using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;

namespace GatefailBot
{
    public static class BotConfigurationBuilder
    {
        public static IConfigurationRoot Build()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var pathToAppSettings = Directory.GetCurrentDirectory();

            return new ConfigurationBuilder()
                .SetBasePath(pathToAppSettings)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static string GetBotDbConnectionString(this IConfigurationRoot configuration)
        {
            var dbHost = configuration.GetValue<string>("dbhost");
            var dbName = configuration.GetValue<string>("dbname");
            var dbpass = configuration.GetValue<string>("dbpass");
            var dbuser = configuration.GetValue<string>("dbuser");
            
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
            connectionStringBuilder.Database = dbName;
            connectionStringBuilder.Host = dbHost;
            connectionStringBuilder.Username = dbuser;
            connectionStringBuilder.Password = dbpass;
            connectionStringBuilder.Port = 5432;
            return connectionStringBuilder.ToString();
        }
    }
}