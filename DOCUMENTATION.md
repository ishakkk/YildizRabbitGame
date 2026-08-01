# Yıldız — Tavşanı Eve Götür

**Case Study Çözümü — .NET 10 C# Console Uygulaması**

---

## 1. Problem Özeti

Tavşan **"Yıldız"** ormanda kaybolmuştur ve bir tavşan deliğine ulaşmaya
çalışmaktadır. Orman, satranç tahtası gibi kare bir ızgara olarak
modellenmiştir (4x4, 8x8 veya 16x16). Izgara üzerinde rastgele yerleştirilmiş
4 farklı engel türü bulunur (her türden en fazla 4 adet):

| Engel        | Sembol | Davranış                                              |
|--------------|:------:|--------------------------------------------------------|
| Kurt         | `K`    | Temas halinde **tavşan ölür** (hangi komutla olursa olsun) |
| Tilki        | `T`    | Temas halinde **tavşan ölür** (hangi komutla olursa olsun) |
| Dikenli Tel  | `#`    | Sadece **Eğil (İ)** ile geçilir; yanlış komutla bloklanır  |
| Çit          | `=`    | Sadece **Zıpla (J)** ile geçilir; yanlış komutla bloklanır |

Tavşan, klavyeden girilen bir **senaryo** (komut dizisi) ile adım adım
yönlendirilir ve amaç, tavşan deliğine ölmeden ulaşmaktır.

---

## 2. Komutlar

| Girdi | Türkçe   | Anlamı                                                           |
|:-----:|----------|--------------------------------------------------------------------|
| `N`   | İleri    | Baktığı yönde 1 hücre ilerler                                     |
| `P`   | Geri     | Baktığı yönün **tersine** 1 hücre gider (yön değişmez)            |
| `R`   | Sağ      | 90° sağa döner (hareket etmez)                                    |
| `L`   | Sol      | 90° sola döner (hareket etmez)                                    |
| `J`   | Zıpla    | Baktığı yönde **2 hücre** ileri sıçrar; **Çit'i geçmenin tek yolu** |
| `İ`   | Eğil     | Baktığı yönde 1 hücre ilerler; **Dikenli Tel'i geçmenin tek yolu** |

Senaryo, virgülle ayrılmış bir metin olarak girilir. Örnek:

```
N,N,L,J,N,N,İ,P,J
```

Tavşan başlangıçta **Güney (S)** yönüne bakar; bu, örnek tabloda tavşanın üst
sol köşede (A8) ve deliğin alt sağ köşede (H1) olmasıyla tutarlıdır.

---

## 3. Tasarım Kararları ve Varsayımlar

Case study metni bazı davranışları tam olarak tanımlamadığı için, çözümde
aşağıdaki **açık ve belgelenmiş varsayımlar** kullanılmıştır. Kod tabanı bu
kuralların hepsini tek bir yerde (`GameEngine.cs`) uyguladığından, farklı bir
yorum gerekirse kolayca değiştirilebilir.

1. **Tahta ekseni:** Satır 0 tahtanın üst kısmıdır. Güney'e gitmek satır
   indeksini artırır — tavşanın örnekte üst-sol köşeden alt-sağ köşedeki
   deliğe Güney yönünde ilerlemesiyle birebir örtüşür.
2. **Başlangıç/bitiş konumları:** Tavşan sol-üst köşede başlar; tavşan
   deliği sağ-alt köşededir (case study örnek tablosuyla birebir aynı).
3. **Engel sayısı:** Her engel türünden **0 ile 4 arası rastgele** bir sayı
   (case study "maksimum 4" diyor, minimum belirtmiyor), aynı hücreye birden
   fazla engel konulmayacak şekilde rastgele dağıtılır. Tavşanın başlangıç
   hücresi ve delik her zaman boş bırakılır.
4. **Ölümcül engeller:** Sadece **Kurt** ve **Tilki** ölümcüldür — hangi
   komutla o hücreye gidilirse gidilsin tavşan ölür ve simülasyon o anda
   durur.
5. **Çit ve Dikenli Tel — "doğru komut zorunluluğu":** Case study'nin
   *"altından geçebileceği dikenli tel"* / *"üzerinden atlayabileceği çit"*
   ifadelerini, bu iki engelin **sadece belirli bir hareketle güvenle
   geçilebileceği** şeklinde yorumluyoruz:
   - Bir Çit hücresine **Zıpla (J) dışında** bir komutla girilmeye
     çalışılırsa, hareket **bloklanır** (tavşan olduğu yerde kalır, ölmez).
   - Bir Dikenli Tel hücresine **Eğil (İ) dışında** bir komutla girilmeye
     çalışılırsa, aynı şekilde **bloklanır**.
   - Bu blok, case study'nin *"tavşan diğer engellere çarparsa
     ölmeyecektir"* kuralıyla çelişmez: tavşan bu engellerden **asla ölmez**,
     sadece yanlış komutla geçemez ve yoluna doğru komutla devam etmesi
     gerekir.
6. **Zıpla (J) hareketi:** Baktığı yönde **2 hücre** ileri gider; aradaki
   (1 hücre ilerideki) hücrede ne olursa olsun (bir Çit dahi olsa) üzerinden
   atlanmış sayılır — sadece **iniş yaptığı** hücre (2 hücre ilerisi)
   değerlendirilir. İniş hücresi de bir Çit ise (arka arkaya iki çit),
   bu da bloklanır — tek seferde yalnızca bir çit atlanabilir.
7. **Eğil (İ) hareketi:** Baktığı yönde **1 hücre** ilerler; hedef hücre
   Dikenli Tel ise güvenle geçilir, başka bir engel/boş hücreyse normal
   `İleri` gibi davranır.
8. **Geri (P) hareketi:** Tavşan **dönmez**, sadece baktığı yönün tam tersi
   istikametinde 1 hücre geri gider. Geri giderken de Çit/Tel kuralları
   (5. madde) aynen geçerlidir.
9. **Tahta sınırları:** Bir hareket komutu tavşanı tahtanın dışına
   çıkaracaksa, hareket **iptal edilir** ve tavşan bulunduğu hücrede kalır;
   simülasyon bir sonraki komutla devam eder (ölüm veya hata oluşmaz).
10. **Senaryo bitişi:** Tüm komutlar tüketildiğinde tavşan hâlâ hayattaysa
    ve deliğe ulaşmamışsa, sonuç "Senaryo sona erdi, tavşan eve ulaşamadı"
    olarak raporlanır.

---

## 4. Algoritma / Çalışma Mantığı

### 4.1 Genel Akış (Program.cs)

```
1. Kullanıcıdan tahta boyutu seçilir (1->4x4, 2->8x8, 3->16x16).
2. (Opsiyonel) Tekrarlanabilir test için bir rastgelelik "seed" değeri istenir.
3. Board nesnesi oluşturulur ve engeller rastgele yerleştirilir.
4. Rabbit nesnesi başlangıç hücresinde, Güney yönüne bakacak şekilde oluşturulur.
5. Tahta konsola çizilir.
6. Kullanıcıdan virgülle ayrılmış senaryo metni alınır (ör. "N,N,L,J,...").
7. RabbitCommandParser senaryoyu RabbitCommand listesine çevirir.
8. GameEngine.Run(commands) çağrılır; her komut sırayla işlenir.
9. Her adımın sonucu (StepResult) bir listede loglanır ve ekrana basılır.
10. Simülasyon bitince (ölüm / eve varış / komutların tükenmesi) tahta son
    haliyle tekrar çizilir ve nihai sonuç yazdırılır.
```

### 4.2 Komut İşleme (GameEngine.ExecuteCommand / MoveBy)

Her komut, iki kategoriden birine girer:

- **Dönüş komutları** (`R`, `L`): Sadece `Rabbit.Facing` alanını günceller;
  konum değişmez.
- **Hareket komutları** (`N`, `P`, `J`, `İ`): `MoveBy(distance, direction)`
  yardımcı metoduna yönlendirilir. Bu metot, hedef hücreyi hesapladıktan
  sonra şu sırayla kontrol eder:
  1. Hedef hücre tahta dışındaysa → hareket iptal, `BlockedByBoundary`.
  2. Hedef hücre Çit ise → hareket iptal, `BlockedByObstacle` (Jump ile
     gelinse bile, çünkü art arda iki çit üstüne inilemez).
  3. Hedef hücre Dikenli Tel ise ve komut `Eğil` değilse → hareket iptal,
     `BlockedByObstacle`.
  4. Hedef hücrede Kurt/Tilki varsa → tavşan ölür, `Died`, simülasyon durur.
  5. Hedef hücre tavşan deliğiyse → `ArrivedHome`, simülasyon durur.
  6. Aksi halde (boş hücre, veya doğru komutla geçilen Çit/Tel) tavşan o
     hücreye taşınır → `Moved`.

### 4.3 Yön Sistemi (Direction.cs)

Yönler `North=0, East=1, South=2, West=3` şeklinde saat yönünde sıralı bir
`enum` olarak tutulur. Böylece:

- **Sağa dönüş** = `(current + 1) % 4`
- **Sola dönüş** = `(current + 3) % 4`
- **Ters yön** (Geri hareketi için) = `(current + 2) % 4`

### 4.4 Engel Yerleştirme (Board.PlaceObstaclesRandomly)

1. Tavşanın başlangıç hücresi ve delik hariç tüm hücreler bir listeye
   toplanır.
2. Liste **Fisher–Yates karıştırma** algoritmasıyla rastgele sıralanır.
3. Her engel türü için 0–4 arası rastgele bir adet, karıştırılmış listeden
   sırayla atanır — aynı hücreye iki engel asla düşmez.

### 4.5 Karmaşıklık

- Engel yerleştirme: `O(n²)` (n = tahta kenar uzunluğu).
- Senaryo çalıştırma: `O(m)`, m = komut sayısı.

---

## 5. Proje Yapısı

```
YildizRabbitGame.slnx
src/
  YildizRabbitGame/
    YildizRabbitGame.csproj   -> .NET 10 Console proje dosyası
    Program.cs                -> Giriş noktası, konsol arayüzü
    Direction.cs               -> Yön enum'u + rotasyon mantığı
    ObstacleType.cs             -> Engel türleri + görsel/metinsel karşılıklar
    Cell.cs                    -> (Row, Col) koordinat record struct'ı
    Board.cs                   -> Tahta, engel yerleştirme, konsola çizim
    RabbitCommand.cs             -> Komut enum'u + senaryo parser'ı
    Rabbit.cs                   -> Tavşanın konumu, yönü, durumu
    StepResult.cs               -> Tek bir adımın log kaydı
    GameEngine.cs               -> Ana simülasyon algoritması
tests/
  YildizRabbitGame.Tests/
    YildizRabbitGame.Tests.csproj -> xUnit test projesi (bağımsız derlenir)
    GameEngineTests.cs             -> Deterministik birim testleri
```

Kod, **tek sorumluluk ilkesine (SRP)** uygun şekilde küçük, test edilebilir
sınıflara bölünmüştür.

---

## 6. Çalıştırma ve Test Etme

```bash
# Uygulamayı çalıştır
dotnet run --project src/YildizRabbitGame/YildizRabbitGame.csproj

# Testleri çalıştır
dotnet test
```

Aynı `seed` değeriyle çalıştırılan iki oturum **birebir aynı** engel
dağılımını üretir — bu, tekrarlanabilirlik sağlamak için eklenmiştir.

---

## 7. Testler Neden Deterministik?

`Board` sınıfında `internal void SetObstacleForTesting(Cell, ObstacleType)`
adlı bir yardımcı metot bulunur ve ana projenin `.csproj` dosyasında
`<InternalsVisibleTo Include="YildizRabbitGame.Tests" />` tanımlıdır. Bu
sayede testler:

- Rastgele bir tahta üretip "doğru engel oraya düşene kadar dene" gibi
  şansa bağlı bir yönteme **ihtiyaç duymadan**,
- Tam olarak istediği hücreye istediği engeli koyarak,

her koşulu (Kurt'a ölüm, Tilki'ye ölüm, Tel'i Eğil ile geçme, Tel'e yanlış
komutla girmenin bloklanması, Çit'i Zıpla ile geçme, art arda iki çite
zıplamanın bloklanması, tahta sınırında durma, dönüş yönleri, Geri hareketi,
temiz bir yoldan eve ulaşma ve rastgele yerleşimin kurallara uyduğu) **%100
tekrarlanabilir** şekilde doğrular. `SetObstacleForTesting` yalnızca
`internal` olduğundan uygulamanın normal oynanışını (her zaman
`PlaceObstaclesRandomly` üzerinden rastgele engel koyar) etkilemez.

---

## 8. Bu Sürümde Düzeltilen Hatalar (önceki taslağa göre)

Bir önceki elle düzenlenmiş taslakta üç kritik derleme hatası vardı; bu
sürümde hepsi giderildi:

1. **Aynı klasörde iki `.csproj` dosyası** vardı (`RabbitForest.csproj` ve
   `YildizRabbitGame.csproj`). SDK-style projeler klasördeki tüm `.cs`
   dosyalarını otomatik derlemeye dahil ettiğinden, hangisi açılırsa açılsın
   ikisi de aynı dosya setini derlemeye çalışıyor ve `dotnet build`
   "Specify which project file to use..." hatası veriyordu. **Çözüm:** tek
   bir konsol projesi (`src/YildizRabbitGame`), ayrı bir klasörde.
2. **Geçersiz NuGet paket referansı**: `System.Console` v8.0.0 böyle bir
   sürümle mevcut değil ve zaten Console BCL'in bir parçası, pakete gerek
   yok. **Çözüm:** paket referansı tamamen kaldırıldı.
3. **Testler ana konsol projesinin içine karışmıştı** ve hiçbir yerde
   `xunit` paket referansı yoktu, bu yüzden derlenmiyordu. **Çözüm:** ayrı
   bir `tests/YildizRabbitGame.Tests` projesi; gerekli `xunit`,
   `Microsoft.NET.Test.Sdk`, `xunit.runner.visualstudio` paketleri eklendi;
   `ProjectReference` ile ana projeye bağlandı.
4. **Şansa bağlı, zayıf test**: Eski test, sabit bir seed ile 100 kere board
   üretip istenen engel çıkana kadar deniyor, çıkmazsa **hiçbir şey assert
   etmeden sessizce geçiyordu**. **Çözüm:** `SetObstacleForTesting` ile tüm
   testler tam deterministik hale getirildi ve kapsam genişletildi (11 test).
5. **(v2.1) .NET sürümü yükseltildi:** Hedef framework `net8.0`'dan
   `net10.0`'a çıkarıldı (.NET 10, Kasım 2025'te yayınlanan bir LTS sürümüdür
   ve Kasım 2028'e kadar destek alır; .NET 8'in desteği ise Kasım 2026'da
   sona eriyor). Kod tabanında ASP.NET Core/EF Core gibi .NET 10'daki kırıcı
   değişikliklerden etkilenen hiçbir API kullanılmadığından, geçiş yalnızca
   her iki `.csproj` dosyasındaki `TargetFramework` alanının güncellenmesiyle
   yapıldı; başka hiçbir kod değişikliği gerekmedi.

---

## 9. Genişletme Fikirleri

- Görsel bir arayüz (WPF/Blazor/Web) eklenmek istenirse, `GameEngine` ve
  `Board` katmanı hiç değiştirilmeden yeniden kullanılabilir; sadece
  `Program.cs`'deki konsol G/Ç katmanı değişir.
- Farklı başlangıç/bitiş konumları desteklemek için `Board` constructor'ına
  parametre eklemek yeterlidir.
- Otomatik yol bulma (BFS/A*) ile herhangi bir rastgele tahtada geçerli bir
  komut dizisi otomatik hesaplanabilir — istenirse bu bir sonraki adım
  olarak eklenebilir.
