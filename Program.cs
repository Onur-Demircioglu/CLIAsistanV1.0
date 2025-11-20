using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using CliAsistan.Extensions;
using CliAsistan.Services;

namespace CliAsistan
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            try
            {
                // 1. Build Configuration
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfiguration configuration = builder.Build();

                // 2. Setup Dependency Injection
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddApplicationServices(configuration);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                // 3. Start Application
                var menuHandler = serviceProvider.GetRequiredService<MenuHandler>();
                
                // Show welcome message
                AnsiConsole.Write(
                    new FigletText("CLI Asistan")
                        .LeftJustified()
                        .Color(Color.Cyan1));

                menuHandler.RunLoop();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Kritik Hata:[/]{ex.Message}");
                AnsiConsole.WriteException(ex);
            }
        }
    }

    public class AppSettings
    {
        public string ModelName { get; set; } = "Unknown";
        public string ModelPath { get; set; } = "";
        public string PythonPath { get; set; } = "python";
        public string ApiUrl { get; set; } = "http://127.0.0.1:1234/v1/chat/completions";
    }
}
