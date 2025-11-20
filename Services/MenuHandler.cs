using System;
using System.Collections.Generic;
using System.IO;
using CliAsistan.Models;
using Spectre.Console;

namespace CliAsistan.Services
{
    public class MenuHandler
    {
        private readonly AIService _aiService;
        private readonly AgentService _agentService;
        private readonly List<Message> _chatHistory = new List<Message>();

        public MenuHandler(AIService aiService, AgentService agentService)
        {
            _aiService = aiService;
            _agentService = agentService;
        }

        public void RunLoop()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("CLI Asistan")
                        .LeftJustified()
                        .Color(Color.Cyan1));

                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Ne yapmak istersiniz?[/]")
                        .PageSize(10)
                        .AddChoices(new[] {
                            "1. AI ile Sohbet Et",
                            "2. Agent Modu (Görev Ver)",
                            "3. Hesaplama Yap",
                            "4. Sohbet Geçmişi",
                            "5. Sistem Bilgisi",
                            "6. Çıkış"
                        }));

                switch (selection.Substring(0, 1))
                {
                    case "1": HandleAI(); break;
                    case "2": HandleAgent(); break;
                    case "3": Calculate(); break;
                    case "4": ShowHistory(); break;
                    case "5": ShowInfo(); break;
                    case "6": return;
                }

                if (selection.Substring(0, 1) != "6")
                {
                    AnsiConsole.MarkupLine("\n[grey]Devam etmek için bir tuşa basın...[/]");
                    Console.ReadKey();
                }
            }
        }

        private void HandleAI()
        {
            AnsiConsole.MarkupLine("[yellow]Sohbet Modu (Çıkmak için 'iptal' yazın)[/]");
            
            while (true)
            {
                var question = AnsiConsole.Ask<string>("[green]Siz:[/] ");
                if (question.ToLower() == "iptal") break;

                _chatHistory.Add(new Message("user", question));

                string answer = "";
                AnsiConsole.Status()
                    .Start("AI Düşünüyor...", ctx => 
                    {
                        answer = _aiService.GenerateResponse(_chatHistory);
                    });

                AnsiConsole.MarkupLine($"[blue]AI:[/] {answer}");
                _chatHistory.Add(new Message("assistant", answer));
                AnsiConsole.WriteLine();
            }
        }

        private void HandleAgent()
        {
            AnsiConsole.MarkupLine("\n[bold yellow]=== AGENT MODU ===[/]");
            var task = AnsiConsole.Ask<string>("Göreviniz nedir? (Çıkmak için 'iptal'): ");

            if (string.IsNullOrEmpty(task) || task.ToLower() == "iptal") return;

            _agentService.RunAgent(task);
        }

        private void ShowInfo()
        {
            var table = new Table();
            table.AddColumn("Özellik");
            table.AddColumn("Değer");

            table.AddRow("İşletim Sistemi", Environment.OSVersion.ToString());
            table.AddRow("Kullanıcı", Environment.UserName);
            table.AddRow("Makine Adı", Environment.MachineName);
            table.AddRow("Tarih", DateTime.Now.ToString());

            AnsiConsole.Write(table);
        }

        private void Calculate()
        {
            AnsiConsole.MarkupLine("\n[bold]Hesaplama[/]");
            var s1 = AnsiConsole.Ask<double>("Sayı 1: ");
            var s2 = AnsiConsole.Ask<double>("Sayı 2: ");

            var table = new Table();
            table.AddColumn("İşlem");
            table.AddColumn("Sonuç");

            table.AddRow("Toplam", (s1 + s2).ToString());
            table.AddRow("Fark", (s1 - s2).ToString());
            table.AddRow("Çarpım", (s1 * s2).ToString());
            if (s2 != 0) table.AddRow("Bölüm", (s1 / s2).ToString("F2"));

            AnsiConsole.Write(table);
        }

        private void ShowHistory()
        {
            if (_chatHistory.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Geçmiş boş.[/]");
                return;
            }

            foreach (var msg in _chatHistory)
            {
                var color = msg.Role == "user" ? "green" : "blue";
                AnsiConsole.MarkupLine($"[{color}]{msg.Role.ToUpper()}:[/] {msg.Content}");
            }
        }
    }
}
