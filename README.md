# 🍽️ Akıllı Kafe ve Restoran Yönetim Otomasyonu (RBAC & POS & Mutfak & Raporlama)

Bu depo (Repository); kafe, restoran ve lokanta işletmelerinde **Salon Garsonları**, **Mutfak Aşçıları**, **Kasa Kasiyerleri** ve **İşletme Yöneticileri** arasındaki tüm operasyonel süreci gerçek zamanlı, yüksek güvenlikli ve hatasız yönetmek için geliştirilmiş katmanlı bir **.NET Windows Forms (C#)** otomasyon platformunu barındırır.

> [!CAUTION]
> **⚠️ YASAL UYARI VE KULLANIM KISITLAMASI (TÜM HAKLARI SAKLIDIR / ALL RIGHTS RESERVED)**
> Bu projenin kaynak kodları, mimarisi, veritabanı şeması ve arayüz tasarımı 5846 sayılı Fikir ve Sanat Eserleri Kanunu kapsamında korunmaktadır. Yazılım sahibinin resmi ve yazılı lisans onayı olmadan bu projenin **hiçbir şekilde, hiçbir kafe, restoran veya ticari/kişisel işletmede kurularak kullanılması, kopyalanması, çoğaltılması, dağıtılması, satılması ve tersine mühendislikle (decompile) incelenmesi KESİNLİKLE YASAKTIR.** Sistemde yerleşik olarak çalışan **Donanım Kilidi (HWID Binding)** ve **Tersine Mühendislik Kalkanı (`AntiReverseEngineering`)** izinsiz erişimleri anında engeller. Detaylı yasal şartlar ve yaptırımlar için lütfen **[📄 LİSANS VE YASAL ŞARTLAR DOSYASINI (LICENSE)](LICENSE)** inceleyiniz.

---

## 📖 Kapsamlı Teknik, Akademik ve Mimari İnceleme Raporu

Projenin sektörel problem analizleri, çözdüğü inovatif darboğazlar, **3NF İlişkisel Veritabanı Şeması**, **OWASP Top 10** güvenlik standartlarına uyumu (SHA-256 Kriptografi, %100 Parametrik SQL, RBAC Yetkilendirmesi, ACID `SqlTransaction` güvencesi) ve modül detayları hakkında hazırlanmış **20 sayfalık kapsamlı inceleme raporuna** aşağıdaki bağlantıdan ulaşabilirsiniz:

👉 **[📄 PROJE KAPSAMLI TEKNİK VE AKADEMİK RAPORUNU OKU (PROJE_KAPSAMLI_TEKNIK_VE_AKADEMIK_RAPORU.md)](PROJE_KAPSAMLI_TEKNIK_VE_AKADEMIK_RAPORU.md)**

---

## 🚀 Projenin Öne Çıkan Temel Özellikleri

1. **İki Bölmeli Dinamik Ürün Kartları & 3D Dokunmatik Animasyonlar:**
   - Yazı ve arka plan fotoğrafını birbirine karıştırmayan, üstte `Zoom` fotoğraf, altta siyah okunaklı şerit barındıran kart arayüzü.
   - Fare ve dokunmatik ekran temasında `+6 piksel` büyüme ve `-4 piksel` mekanik basılma hissi.
2. **Gerçek Zamanlı Mutfak ve FIFO Sıra Garantisi:**
   - Eklenen her yeni sipariş adisyonun ve mutfak listesinin kesinlikle **en altına (`APPEND`)** kronolojik sırayla eklenir. `Hazırlanıyor` ➔ `Hazır` ➔ `Tamamlandı` döngüsü kusursuz çalışır.
3. **Rezervasyon Güvenlik Kilidi:**
   - `Rezerve` statüsündeki bir masaya garsonun sipariş girmesi ve adisyon açması sistem seviyesinde engelleyerek müşteri mağduriyeti sıfırlanır.
4. **ACID Uyumlu Bankacılık Standardında İşlem Güvenliği (`SqlTransaction`):**
   - Sipariş ekleme, ödeme tahsilatı (Nakit / Kredi Kartı) veya ürün iptali esnasında oluşan herhangi bir bağlantı hatasında tüm veritabanı işlemleri `Rollback` ile geri alınarak hesap tutarsızlığı önlenir.
5. **Katmanlı Rol Bazlı Erişim Denetimi (RBAC):**
   - Yönetici, Garson, Kasiyer ve Mutfak personeli yalnızca kendi yetki alanlarına giren panellere erişir. Şifreler `SHA-256` tek yönlü hashleme algoritmasıyla korunur.
6. **Detaylı İndirilebilir Sanal PDF / HTML Finansal Raporlama:**
   - Anlık ciro, X Raporu (Nakit/Kart Kırılımı), Z Raporu, Aylık ve Yıllık finansal dökümler tek tıkla şık HTML ve sanal PDF belgelerine dönüştürülür.
7. **Donanım Kilidi (HWID Binding) & Tersine Mühendislik Zırhı (`Anti-Reverse Engineering`):**
   - **HWID Lisanslama (`LisansKontrol`):** İşlemci, anakart ve BIOS kimliklerini SHA-256 ile özetleyerek uygulamayı tek bir bilgisayara kilitler. İzinsiz kopyalamaları ve şube kaçaklarını engeller.
   - **Tersine Mühendislik Kalkanı (`AntiReverseEngineering`):** Native Debugger, *dnSpy, ILSpy, Cheat Engine, Memory Dump* ve RAM dökümü girişimlerini algılayarak kodu ve bellek başlıklarını (`PE Header Wiping`) tam zamanlı korur.

---

## 🛠️ Kurulum ve Çalıştırma Rehberi

### 1. Veritabanının Hazırlanması (`SQL Server`)
1. Microsoft SQL Server Management Studio (SSMS) uygulamasını açın.
2. Depo içerisindeki **`Database/Veritabani_Kurulum_Scripti.sql`** dosyasını açıp (`F5` veya `Execute`) çalıştırarak `Restorant` veritabanını ve ilişkisel tablolarını (`Personel`, `Kategoriler`, `Urunler`, `Masalar`, `Siparis`) otomatik kurun.

### 2. Uygulamanın Derlenmesi ve Çalıştırılması
1. **`WindowsFormsApp1/WindowsFormsApp1.sln`** çözüm dosyasını **Visual Studio 2022** (veya 2019) ile açın.
2. `F5` tuşuna basarak projeyi başlatın.

---

## 📸 Ekran Görüntüleri

> *(Not: Ekran görüntülerini eklemek için aldığınız görselleri `images/` klasörüne yapıştırıp aşağıdaki sözdizimini kullanabilirsiniz.)*

```markdown
![Giriş Ekranı](images/login_ekrani.png)
![Masalar ve Rezervasyon Yönetimi](images/masalar.png)
![Sipariş Ekranı](images/siparis_ekrani.png)
```

---

## 🔒 Yasal Uyarı, Telif Hakkı ve Lisans Şartları (LICENSE)

Bu yazılım ve içeriğindeki tüm materyaller **TÜM HAKLARI SAKLIDIR (ALL RIGHTS RESERVED / PROPRIETARY LICENSE)** lisansı ile korunmaktadır.

* **Hiçbir Şekilde Kullanılamaz:** Yazılım sahibinden resmi yazılı izin, ıslak imzalı sözleşme veya benzersiz şifreli lisans anahtarı (`lisans.key`) alınmadığı sürece bu projenin kodları veya derlenmiş çıktıları hiçbir işletmede, restoranda, kafede veya ticari/kişisel ortamda çalıştırılamaz ve kullanılamaz.
* **Tersine Mühendislik Yasağı:** Kodların kopyalanması, türetilmesi veya `.exe/.dll` dosyalarının dezenformasyon/decompile araçlarıyla incelenmesi hukuki ihlaldir.
* **Lisans Şartlarını İnceleyin:** Tüm kullanım yasakları, donanım kilidi yaptırımları ve telif hakları hakkında tam bilgi almak için deponun ana dizininde yer alan resmi lisans belgesini mutlaka okuyunuz:

👉 **[📄 RESMİ LİSANS VE YASAL KULLANIM ŞARTLARI DOSYASINA GİT (LICENSE)](LICENSE)**

---
*Geliştirme ve Mimari Tasarım: Akıllı Kafe ve Restoran Yönetim Otomasyonu Ekibi | Copyright © 2026 Tüm Hakları Saklıdır.*
