using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CliAsistan.Models;
using Spectre.Console;

namespace CliAsistan.Services
{
    public class AgentService
    {
        private readonly AIService _aiService;
        private readonly List<AgentTool> _tools;
        private const int MaxSteps = 10;

        public AgentService(AIService aiService)
        {
            _aiService = aiService;
            _tools = new List<AgentTool>
            {
                new AgentTool 
                { 
                    Ad = "dosya_yaz", 
                    Aciklama = "Dosyaya içerik yazar. Parametreler: dosya_yolu, icerik", 
                    Fonksiyon = DosyaYaz 
                },
                new AgentTool 
                { 
                    Ad = "dosya_oku", 
                    Aciklama = "Dosya içeriğini okur. Parametreler: dosya_yolu", 
                    Fonksiyon = DosyaOku 
                },
                new AgentTool 
                { 
                    Ad = "sistem_bilgisi", 
                    Aciklama = "Sistem bilgilerini getirir.", 
                    Fonksiyon = SistemBilgisi 
                },
                new AgentTool
                {
                    Ad = "cmd_calistir",
                    Aciklama = "Terminal komutu çalıştırır. Parametreler: komut",
                    Fonksiyon = CmdCalistir
                }
            };
        }

        public void RunAgent(string task)
        {
            var messages = new List<Message>
            {
                new Message("system", PrepareSystemPrompt()),
                new Message("user", $"GÖREV: {task}")
            };

            AnsiConsole.MarkupLine($"[green]Ajan Başlatıldı. Görev:[/] {task}");

            for (int step = 1; step <= MaxSteps; step++)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Adım {step}/{MaxSteps}[/] Düşünülüyor...");
                
                string response = "";
                AnsiConsole.Status()
                    .Start("AI Yanıtı Bekleniyor...", ctx => 
                    {
                        response = _aiService.GenerateResponse(messages);
                    });

                messages.Add(new Message("assistant", response));
                AnsiConsole.MarkupLine($"[blue]AI Düşüncesi:[/] {response}");

                // Tool kontrolü
                var toolMatch = Regex.Match(response, @"<TOOL>(.*?)</TOOL>", RegexOptions.Singleline);
                
                if (toolMatch.Success)
                {
                    string toolContent = toolMatch.Groups[1].Value;
                    string toolResult = ExecuteTool(toolContent);
                    
                    AnsiConsole.MarkupLine($"[purple]Araç Sonucu:[/] {toolResult}");
                    messages.Add(new Message("user", $"ARAÇ SONUCU: {toolResult}"));
                }
                else
                {
                    if (response.Contains("GÖREV TAMAMLANDI"))
                    {
                        AnsiConsole.MarkupLine("[bold green]Görev Başarıyla Tamamlandı![/]");
                        break;
                    }
                    // Eğer tool yoksa ve görev tamamlanmadıysa, belki kullanıcıdan bilgi istiyordur.
                    // Şimdilik döngüye devam etsin, AI tekrar konuşsun.
                }
            }
        }

        private string PrepareSystemPrompt()
        {
            var toolsDesc = string.Join("\n", _tools.Select(t => $"- {t.Ad}: {t.Aciklama}"));
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            return $@"Sen zeki bir AI Ajanısın. Verilen görevi tamamlamak için adım adım düşün ve araçları kullan.

MEVCUT ARAÇLAR:
{toolsDesc}

SİSTEM BİLGİSİ:
Masaüstü Yolu: {desktopPath}
Çalışma Dizini: {Directory.GetCurrentDirectory()}

KURALLAR:
1. Düşünce zinciri (Chain of Thought) kullan. Önce ne yapacağını planla, sonra aracı çağır.
2. Araç kullanmak için şu formatı kullan: <TOOL>arac_adi|param1=deger1|param2=deger2</TOOL>
3. Her adımda SADECE BİR araç kullan.
4. Eğer kullanıcı bir dosya oluşturmanı isterse, dosyanın İÇERİĞİNİ SEN ÜRET. ""İçerik boş"" deme, yaratıcı ol.
5. Dosya yollarını tam yol (absolute path) olarak ver. Yukarıdaki 'Masaüstü Yolu'nu kullan.

ÖRNEK AKIŞ:
Kullanıcı: 'Masaüstüne elma.txt oluştur ve elmalar hakkında bilgi ver.'
Sen: Masaüstü yolunu biliyorum ({desktopPath}). Elma hakkında kısa bir bilgi yazıp dosyayı oluşturacağım.
<TOOL>dosya_yaz|dosya_yolu={Path.Combine(desktopPath, "elma.txt")}|icerik=Elma, gülgiller familyasından besleyici bir meyvedir.</TOOL>
Kullanıcı: ARAÇ SONUCU: Dosya başarıyla yazıldı.
Sen: Dosya masaüstüne oluşturuldu. GÖREV TAMAMLANDI.";
        }

        private string ExecuteTool(string toolCallString)
        {
            try
            {
                var parts = toolCallString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return "Hata: Boş araç çağrısı.";

                var toolName = parts[0].Trim();
                var args = new Dictionary<string, string>();

                for (int i = 1; i < parts.Length; i++)
                {
                    var kv = parts[i].Split(new[] { '=' }, 2);
                    if (kv.Length == 2)
                    {
                        args[kv[0].Trim()] = kv[1].Trim();
                    }
                }

                var tool = _tools.FirstOrDefault(t => t.Ad == toolName);
                if (tool != null && tool.Fonksiyon != null)
                {
                    return tool.Fonksiyon(args);
                }
                return $"Hata: '{toolName}' aracı bulunamadı.";
            }
            catch (Exception ex)
            {
                return $"Araç Hatası: {ex.Message}";
            }
        }

        #region Tool Functions
        private static string DosyaYaz(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("dosya_yolu") || !args.ContainsKey("icerik"))
                return "Hata: dosya_yolu ve icerik gerekli.";

            try 
            {
                File.WriteAllText(args["dosya_yolu"], args["icerik"].Replace("\\n", "\n"));
                return "Dosya başarıyla yazıldı.";
            }
            catch (Exception ex) { return $"Hata: {ex.Message}"; }
        }

        private static string DosyaOku(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("dosya_yolu")) return "Hata: dosya_yolu gerekli.";
            if (!File.Exists(args["dosya_yolu"])) return "Dosya bulunamadı.";
            return File.ReadAllText(args["dosya_yolu"]);
        }

        private static string SistemBilgisi(Dictionary<string, string> args)
        {
            return $"OS: {Environment.OSVersion}, User: {Environment.UserName}, Time: {DateTime.Now}";
        }

        private static string CmdCalistir(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("komut")) return "Hata: komut gerekli.";
            
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {args["komut"]}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return string.IsNullOrEmpty(error) ? output : $"Hata: {error}\nÇıktı: {output}";
            }
            catch (Exception ex) { return $"Komut Hatası: {ex.Message}"; }
        }
        #endregion
    }
}
