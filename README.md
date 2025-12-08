# 🤖 C# CLI Asistan

**CLI Asistan**, yerel LLM (Large Language Model) sunucularınızla (örneğin LM Studio) etkileşime geçen, gelişmiş bir komut satırı aracıdır. .NET 9.0 ve `Spectre.Console` kullanılarak geliştirilmiş modern, rekli ve kullanıcı dostu bir arayüz sunar. Sadece sohbet etmekle kalmaz, "Ajan Modu" sayesinde dosya işlemleri yapabilir ve sistem komutlarını çalıştırabilir.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Framework](https://img.shields.io/badge/.NET-9.0-purple.svg)

## 🌟 Özellikler

*   **Zeki Ajan Modu**: Karmaşık görevleri adımlara bölerek çözer (Chain of Thought).
*   **Araç Desteği (Function Calling)**:
    *   📁 `dosya_yaz`: Yeni dosyalar oluşturur ve içerik yazar.
    *   📖 `dosya_oku`: Mevcut dosyaları okur ve analiz eder.
    *   💻 `cmd_calistir`: Terminal komutlarını çalıştırır.
    *   ℹ️ `sistem_bilgisi`: Sistem özelliklerini görüntüler.
*   **Modern Arayüz**: `Spectre.Console` ile zengin metin biçimlendirme, renkli çıktılar ve yükleme animasyonları.
*   **Esnek Yapılandırma**: `appsettings.json` üzerinden model ve API ayarlarını kolayca değiştirebilme.
*   **Modüler Mimari**: Dependency Injection ve Clean Architecture prensiplerine uygun kod yapısı.

## 🚀 Başlangıç

Bu projeyi çalıştırmak için aşağıdaki adımları takip edin.

### Gereksinimler

*   [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   Yerel bir LLM Sunucusu (Önerilen: [LM Studio](https://lmstudio.ai/))
    *   *Not: Uygulama varsayılan olarak `http://127.0.0.1:1234/v1/chat/completions` adresini dinler.*

### Kurulum

1.  **Repo'yu klonlayın:**
    ```bash
    git clone https://github.com/kullaniciadi/C-CliAsistan.git
    cd C-CliAsistan
    ```

2.  **Bağımlılıkları yükleyin:**
    ```bash
    dotnet restore
    ```

3.  **Yapılandırma:**
    `ClıAsistan/appsettings.json` dosyasını açın ve gerekirse API adresinizi düzenleyin:
    ```json
    {
      "AppSettings": {
        "ApiUrl": "http://127.0.0.1:1234/v1/chat/completions",
        "ModelName": "Llama-3-8B-Instruct" 
      }
    }
    ```

### Çalıştırma

Terminal üzerinden proje dizinindeyken:

```bash
dotnet run --project ClıAsistan
```

## 🎮 Kullanım

Uygulama açıldığında sizi ana menü karşılar. Buradan **Sohbet Modu** veya **Ajan Modu** seçebilirsiniz.

*   **Sohbet Modu**: Model ile standart soru-cevap şeklinde konuşabilirsiniz.
*   **Ajan Modu**: "Masaüstünde 'notlarım.txt' adında bir dosya oluştur ve içine alışveriş listesi yaz" gibi komutlar verebilirsiniz. Ajan, bu görevi yerine getirmek için gerekli araçları otomatik olarak kullanır.

## 🛠️ Geliştirme

Proje modüler bir yapıya sahiptir:
*   `Services/AIService.cs`: API haberleşmesini sağlar.
*   `Services/AgentService.cs`: Ajan mantığını ve araç kullanımını yönetir.
*   `Program.cs`: Uygulamanın giriş noktası ve servis kayıtları.

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Lütfen bir "Issue" açarak veya "Pull Request" göndererek projeye destek olun.

1.  Projeyi Fork'layın
2.  Kendi dalınızı (branch) oluşturun (`git checkout -b ozellik/YeniOzellik`)
3.  Değişikliklerinizi commit edin (`git commit -m 'Yeni özellik eklendi'`)
4.  Dalınıza push yapın (`git push origin ozellik/YeniOzellik`)
5.  Bir Pull Request oluşturun

## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakınız.
