# 🐇 Yıldız — Tavşanı Eve Götür

**Rastgele engellerle dolu bir orman ızgarasında, komut satırından yönlendirilen bir tavşanı güvenle evine (deliğine) ulaştıran .NET 10 / C# konsol simülasyonu.**

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![C%23](https://img.shields.io/badge/C%23-14-239120?logo=csharp&logoColor=white)
![Tests](https://img.shields.io/badge/tests-xUnit-25A162?logo=nunit&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-blue)
![Platform](https://img.shields.io/badge/platform-console-lightgrey)

---

## 📖 İçindekiler

- [Genel Bakış](#-genel-bakış)
- [Özellikler](#-özellikler)
- [Ekran Görüntüsü / Örnek Çalıştırma](#-ekran-görüntüsü--örnek-çalıştırma)
- [Gereksinimler](#-gereksinimler)
- [Kurulum](#-kurulum)
- [Kullanım](#-kullanım)
- [Komutlar](#-komutlar)
- [Oyun Kuralları](#-oyun-kuralları)
- [Proje Yapısı](#-proje-yapısı)
- [Mimari](#-mimari)
- [Algoritmalar ve Performans](#-algoritmalar-ve-performans)
- [Testler](#-testler)
- [Sık Sorulan Sorular](#-sık-sorulan-sorular)
- [Yol Haritası](#-yol-haritası)
- [Katkıda Bulunma](#-katkıda-bulunma)


---

## 🎯 Genel Bakış

**Yıldız**, ormanda kaybolmuş bir tavşandır. Orman, satranç tahtası gibi kare bir ızgara olarak modellenmiştir (**4x4**, **8x8** veya **16x16**). Işgara üzerinde, her türden en fazla 4 adet olacak şekilde rastgele yerleştirilmiş 4 farklı engel bulunur: **Kurt**, **Tilki**, **Dikenli Tel** ve **Çit**.

Tavşan, klavyeden virgülle ayrılmış bir **komut senaryosu** (`N,N,L,J,N,N,İ,P,J` gibi) ile adım adım yönlendirilir. Amaç, tavşanı **ölmeden** tahtanın diğer köşesindeki **tavşan deliğine** ulaştırmaktır.

Bu proje, bir case study çözümü olarak geliştirilmiştir ve şunları amaçlar:

- Temiz, test edilebilir bir **domain modeli** ile bir grid tabanlı simülasyon kurmak
- İş kurallarını (engel davranışları, ölüm koşulları, sınır kontrolleri) **tek bir yerde** (`GameEngine`) merkezi ve açık biçimde uygulamak
- **%100 deterministik** birim testlerle davranışı doğrulamak

---

## ✨ Özellikler

- 🗺️ **3 farklı tahta boyutu**: 4x4, 8x8, 16x16
- 🎲 **Rastgele engel yerleşimi** (Fisher–Yates algoritmasıyla), isteğe bağlı **seed** ile tekrarlanabilir
- 🧭 **4 yönlü pusula sistemi** (Kuzey/Doğu/Güney/Batı) ve saat yönü tabanlı dönüş mantığı
- 🐺🦊 **Ölümcül engeller**: Kurt ve Tilki — hangi komutla girilirse girilsin ölüm
- 🧵🚧 **Geçilebilir engeller**: Dikenli Tel (sadece Eğil ile) ve Çit (sadece Zıpla ile); yanlış komutla girilirse tavşan ölmez, sadece bloklanır
- 📜 **Adım adım hareket logu** — her komutun sonucu (`Moved`, `BlockedByObstacle`, `Died`, `ArrivedHome` vb.) ayrı ayrı raporlanır
- ✅ **11 deterministik xUnit testi** — rastgeleliğe bağlı kalmadan, tam kontrollü senaryolarla doğrulanmış iş kuralları
- 🏗️ **Temiz mimari**: Konsol uygulaması ve test projesi ayrı `.csproj` dosyalarında, SRP'ye uygun küçük sınıflar

---

## 🖥️ Ekran Görüntüsü / Örnek Çalıştırma

```
=====================================================
   YILDIZ - Tavşanı Eve Götür (.NET C# Console)
=====================================================

Orman büyüklüğünü seçin (tek rakam):
  1 -> 4x4
  2 -> 8x8
  3 -> 16x16
Seçiminiz: 1

Rastgelelik için bir sayı (seed) girmek ister misiniz? (Boş bırakabilirsiniz): 7

Orman büyüklüğü: 4x4
Tavşan başlangıcı: A4  |  Yön: Güney (S)
Tavşan deliği: D1

    A  B  C  D
 4  R  #  =  .
 3  T  #  =  #
 2  =  T  .  .
 1  .  .  #  H

R = Tavşan (Yıldız), H = Tavşan Deliği, K = Kurt, T = Tilki, # = Dikenli Tel, = = Çit

Senaryo: L,İ,J,R,İ,N,N

---------------------- Adım Adım İzleme ----------------------
Adım  1: Sol (L)     -> Yön değişti -> Doğu (E)
Adım  2: Eğil (İ)    -> A4 -> B4 (Dikenli Tel geçildi)
Adım  3: Zıpla (J)   -> B4 -> D4 (Çit geçildi)
Adım  4: Sağ (R)     -> Yön değişti -> Güney (S)
Adım  5: Eğil (İ)    -> D4 -> D3 (Dikenli Tel geçildi)
Adım  6: İleri (N)   -> D3 -> D2
Adım  7: İleri (N)   -> D2 -> D1 konumundaki tavşan deliğine ulaştı!

SONUÇ: Tebrikler! Yıldız tavşan deliğine güvenle ulaştı. 🐇🏡
```

---

## 🔧 Gereksinimler

| Araç | Sürüm |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | **10.0** veya üzeri (LTS, Kasım 2028'e kadar destekli) |
| IDE (isteğe bağlı) | Visual Studio 2026 / güncel VS 2022 / VS Code + C# Dev Kit |

Kurulumu doğrulamak için:

```bash
dotnet --version
# 10.x.x görmelisiniz
```

---

## 📦 Kurulum

```bash
# Depoyu klonlayın
git clone https://github.com/<kullanici-adiniz>/YildizRabbitGame.git
cd YildizRabbitGame

# Bağımlılıkları geri yükleyin (opsiyonel, dotnet run/test bunu otomatik yapar)
dotnet restore

# Derleyin
dotnet build
```

---

## ▶️ Kullanım

```bash
dotnet run --project src/YildizRabbitGame/YildizRabbitGame.csproj
```

Uygulama sırasıyla şunları soracaktır:

1. **Tahta boyutu**: `1` (4x4), `2` (8x8) veya `3` (16x16) — doğrudan `4`, `8`, `16` da girilebilir.
2. **Seed (isteğe bağlı)**: Aynı seed, aynı engel dizilimini tekrar üretir — test/tekrarlanabilirlik için kullanışlıdır. Boş bırakılırsa her çalıştırmada farklı bir tahta oluşur.
3. **Senaryo**: Virgülle ayrılmış komut dizisi, örn. `N,N,L,J,N,N,İ,P,J`.

Çalıştırma sonunda tahta son haliyle tekrar çizilir, tüm adımların logu ve nihai sonuç (**eve ulaştı** / **öldü** / **tamamlanamadı**) gösterilir.

### IDE ile Çalıştırma

Çözüm dosyası `.slnx` (yeni, XML tabanlı, `.sln`'in yerini alan format) olarak sağlanmıştır:

- **Visual Studio 2022 17.13+ / 2026**: `YildizRabbitGame.slnx` dosyasını **File → Open → Project/Solution** ile açın (bazı sürümlerde dosyaya çift tıklamak doğrudan Visual Studio'yu açmayabilir; bu durumda IDE içinden açın). `YildizRabbitGame` projesini "Startup Project" yapıp `F5`/`Ctrl+F5` ile çalıştırın.
- **VS Code + C# Dev Kit**: Klasörü açtığınızda `.slnx` otomatik algılanmazsa, `.vscode/settings.json` içine `"dotnet.defaultSolution": "YildizRabbitGame.slnx"` ekleyin.
- **Rider 2024.3+**: Doğrudan açabilirsiniz.
- **CLI**: `.slnx` için `.sln` ile aynı komutlar geçerlidir (`dotnet build`, `dotnet test` vb. çözümü otomatik bulur).

> `.slnx`, .NET 9.0.200+ SDK ve modern IDE'ler tarafından desteklenir; `.NET 10`'da `dotnet new sln` varsayılan olarak bu formatı üretir. Eski araçlarla çalışıyorsanız `dotnet sln YildizRabbitGame.slnx migrate` benzeri bir yolla klasik `.sln`'e dönüştürebilirsiniz (Microsoft aynı depoda iki formatı birden tutmamanızı önerir).

---

## ⌨️ Komutlar

| Girdi | Türkçe Adı | Etki |
|:---:|---|---|
| `N` | İleri | Baktığı yönde 1 hücre ilerler |
| `P` | Geri | Baktığı yönün **tam tersine** 1 hücre gider — yön değişmez |
| `R` | Sağ | 90° **saat yönünde** döner — konum değişmez |
| `L` | Sol | 90° **saat yönünün tersine** döner — konum değişmez |
| `J` | Zıpla | Baktığı yönde **2 hücre** ileri sıçrar — **Çit'i geçmenin tek yolu** |
| `İ` | Eğil | Baktığı yönde 1 hücre ilerler — **Dikenli Tel'i geçmenin tek yolu** |

> 💡 Yön döngüsü saat yönünde şu şekilde ilerler: **Kuzey → Doğu → Güney → Batı → Kuzey**. Dört kere aynı yöne dönmek sizi başlangıç yönüne geri getirir.

---

## 📜 Oyun Kuralları

| Engel | Sembol | Davranış |
|---|:---:|---|
| Kurt | `K` | Temas halinde tavşan **ölür** — komuttan bağımsız |
| Tilki | `T` | Temas halinde tavşan **ölür** — komuttan bağımsız |
| Dikenli Tel | `#` | Sadece **Eğil (İ)** ile güvenle geçilir; başka komutla girilmeye çalışılırsa hareket **bloklanır** (ölmez) |
| Çit | `=` | Sadece **Zıpla (J)** ile güvenle geçilir; başka komutla girilmeye çalışılırsa hareket **bloklanır** (ölmez) |

Ek kurallar:

- Tavşan sol üst köşede başlar, **Güney** yönüne bakar; delik sağ alt köşededir.
- Her engel türünden **0–4 arası** rastgele adet, aynı hücreye çakışmayacak şekilde dağıtılır.
- Zıpla (`J`) komutu 2 hücre ileri gider; aradaki hücre ne olursa olsun üzerinden atlanır, yalnızca **iniş hücresi** değerlendirilir. İniş hücresi de bir Çit ise hareket yine bloklanır (art arda iki çite zıplanamaz).
- Tahta sınırlarının dışına çıkan bir hareket **iptal edilir**; tavşan olduğu yerde kalır (ölümcül değildir).
- Tüm komutlar tüketildiğinde tavşan hâlâ hayattaysa ve deliğe ulaşmamışsa, sonuç *"senaryo sona erdi, tavşan eve ulaşamadı"* olarak raporlanır.

---

## 🗂️ Proje Yapısı

```
YildizRabbitGame.slnx
├── src/
│   └── YildizRabbitGame/
│       ├── YildizRabbitGame.csproj   # Konsol uygulaması (.NET 10)
│       ├── Program.cs                # Giriş noktası, konsol arayüzü
│       ├── Direction.cs              # Yön enum'u + rotasyon mantığı
│       ├── ObstacleType.cs           # Engel türleri
│       ├── Cell.cs                   # (Satır, Sütun) koordinatı
│       ├── Board.cs                  # Tahta, engel yerleştirme, çizim
│       ├── RabbitCommand.cs          # Komut enum'u + senaryo parser
│       ├── Rabbit.cs                 # Tavşanın durumu (konum/yön/hayatta mı)
│       ├── StepResult.cs             # Tek bir adımın log kaydı
│       └── GameEngine.cs             # Ana simülasyon algoritması
├── tests/
│   └── YildizRabbitGame.Tests/
│       ├── YildizRabbitGame.Tests.csproj  # xUnit test projesi
│       └── GameEngineTests.cs             # 11 deterministik test
├── DOCUMENTATION.md                  # Detaylı tasarım/algoritma dokümanı
├── SampleScenario.txt                # Örnek senaryo ve kullanım notu
└── README.md                         # Bu dosya
```

---

## 🏗️ Mimari

Uygulama, **tek sorumluluk ilkesine** uygun üç katmandan oluşur:

| Katman | Sorumluluk | Sınıflar |
|---|---|---|
| **Sunum / G-Ç** | Kullanıcı girdisi, konsola çizim | `Program` |
| **Alan (Domain) Modeli** | Tahta, tavşan, yön, engel, komut veri yapıları | `Board`, `Rabbit`, `Cell`, `Direction`, `ObstacleType`, `RabbitCommand` |
| **Simülasyon Motoru** | Komutları kurallara göre yürütme, sonucu üretme | `GameEngine`, `StepResult` |

```
Program.cs
  -> Board (tahta + rastgele engeller)
  -> Rabbit (başlangıç konumu + yönü)
  -> RabbitCommandParser.Parse(senaryo)
  -> GameEngine.Run(komutlar)
       -> her komut için ExecuteCommand -> MoveBy
       -> StepResult listesine kayıt
  -> Sonuç + adım adım log konsola yazdırılır
```

Detaylı sınıf açıklamaları ve tasarım kararları için bkz. [`DOCUMENTATION.md`](DOCUMENTATION.md).

---

## ⚙️ Algoritmalar ve Performans

- **Engel yerleştirme**: [Fisher–Yates karıştırma](https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle) — `O(n²)`, n = tahta kenar uzunluğu. Gerçek uniform rastgelelik garantisi verir.
- **Senaryo çalıştırma**: Komutlar sırayla, sabit zamanlı (`O(1)`) işlenir → toplam `O(m)`, m = komut sayısı.
- **Hücre erişimi**: 2 boyutlu dizi ile doğrudan `O(1)` erişim.

En büyük tahtada (16x16 = 256 hücre) dahi işlem hacmi ihmal edilebilir düzeydedir; darboğaz algoritma değil, konsoldan kullanıcı girdisi beklemektir (G/Ç).

> Bu problem bir arama/sıralama problemi değildir; bu yüzden Binary Search veya Bubble Sort gibi algoritmalar kod tabanında **kullanılmamıştır**.

---

## 🧪 Testler

```bash
dotnet test
```

Testler `Board.SetObstacleForTesting(...)` adlı `internal` bir yardımcı metotla (ana projenin `.csproj`'unda `InternalsVisibleTo` ile test projesine açılmıştır) **tamamen deterministik** tahtalar kurar — rastgele bir seed'in "doğru" engeli üretmesine bel bağlanmaz.

**Kapsanan senaryolar:**

- Eğil ile Dikenli Tel'in güvenle geçilmesi / İleri ile bloklanması
- Zıpla ile Çit'in atlanması / iniş hücresi de Çit ise bloklanması
- Kurt / Tilki'ye çarpan tavşanın komuttan bağımsız ölmesi
- Tahta sınırında bloklanmanın ölümcül olmaması
- Sağ/Sol dönüşlerin doğru yön hesaplaması
- Geri komutunun yön değiştirmeden ters yönde hareket etmesi
- Engelsiz bir yoldan tavşan deliğine başarıyla ulaşılması
- Rastgele yerleşimin maksimum adet ve çakışmama kurallarına uyması

---

## ❓ Sık Sorulan Sorular

<details>
<summary><b>Aynı komut her tahtada işe yarar mı?</b></summary>

Hayır. Engeller her çalıştırmada (seed girilmediği sürece) rastgele yerleştirilir, bu yüzden bir tahtada işe yarayan bir senaryo başka bir tahtada bir Kurt/Tilki'ye çarpabilir. Tekrarlanabilir test için bir **seed** girin.
</details>

<details>
<summary><b>Zıpla ile Çit'e, Eğil ile Tel'e neden zorunlu giriliyor?</b></summary>

Bu, case study'deki *"üzerinden atlayabileceği çit"* / *"altından geçebileceği dikenli tel"* ifadelerinin yorumudur: bu iki engel yalnızca doğru hareketle güvenle geçilebilir; yanlış komutla girilmeye çalışılırsa tavşan **ölmez**, sadece olduğu yerde kalır. Detaylar için [`DOCUMENTATION.md`](DOCUMENTATION.md) → *"Tasarım Kararları ve Varsayımlar"*.
</details>

<details>
<summary><b>Geçersiz bir komut girersem ne olur?</b></summary>

`RabbitCommandParser` bir `FormatException` fırlatır ve program anlamlı bir hata mesajıyla sonlanır. Yalnızca `N, P, R, L, J, İ` (büyük/küçük harf duyarsız) kabul edilir.
</details>

---

## 🗺️ Yol Haritası

- [ ] **Oto-çözücü**: BFS/A* ile herhangi bir rastgele tahtada geçerli bir komut dizisinin otomatik hesaplanması
- [ ] Görsel arayüz (WPF / Blazor / Web) — `GameEngine` ve `Board` katmanı değişmeden
- [ ] Yapılandırılabilir başlangıç/bitiş konumları
- [ ] CI pipeline (GitHub Actions ile `dotnet test` otomasyonu)

Katkıda bulunmak isterseniz bu maddelerden birini seçip bir issue/PR açabilirsiniz.

---

## 🤝 Katkıda Bulunma

1. Bu depoyu fork'layın
2. Bir özellik dalı oluşturun: `git checkout -b ozellik/harika-bir-seyler`
3. Değişikliklerinizi commit'leyin: `git commit -m "Harika bir şey eklendi"`
4. Dalınızı push'layın: `git push origin ozellik/harika-bir-seyler`
5. Bir Pull Request açın

Lütfen yeni davranışlar için `tests/YildizRabbitGame.Tests` altına deterministik testler eklemeyi unutmayın (`Board.SetObstacleForTesting` yardımcı metodunu kullanabilirsiniz).

---

