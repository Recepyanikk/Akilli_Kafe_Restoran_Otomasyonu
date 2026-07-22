using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class LisansKontrol
    {
        // Donanım kimliğini şifrelerken kullanılan gizli tuz (salt) anahtarı
        private const string GIZLI_TUZ_ANAHTAR = "AKILLI_KAFE_REST_GIZLI_SALT_2026_TR_X987654";
        private static string lisansDosyaYolu = Path.Combine(Application.StartupPath, "lisans.key");

        /// <summary>
        /// Bilgisayara özel benzersiz donanım kimliği (HWID: CPU + Anakart + BIOS) oluşturur.
        /// </summary>
        public static string DonanimKimligiAl()
        {
            try
            {
                string cpuId = WmiSorguOlustur("Win32_Processor", "ProcessorId");
                string boardId = WmiSorguOlustur("Win32_BaseBoard", "SerialNumber");
                string biosId = WmiSorguOlustur("Win32_BIOS", "SerialNumber");

                if (string.IsNullOrWhiteSpace(cpuId) && string.IsNullOrWhiteSpace(boardId))
                {
                    cpuId = Environment.MachineName + "_" + Environment.ProcessorCount;
                }

                string hamHwid = $"{cpuId}#{boardId}#{biosId}#{Environment.MachineName}";
                return Sha256Ozet(hamHwid).Substring(0, 24).ToUpper(); // 24 Karakterlik Şık HWID
            }
            catch
            {
                string yedekHwid = $"{Environment.MachineName}_{Environment.OSVersion}_{Environment.ProcessorCount}";
                return Sha256Ozet(yedekHwid).Substring(0, 24).ToUpper();
            }
        }

        private static string WmiSorguOlustur(string tablo, string ozellik)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT {ozellik} FROM {tablo}"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        if (obj[ozellik] != null && !string.IsNullOrWhiteSpace(obj[ozellik].ToString()))
                        {
                            return obj[ozellik].ToString().Trim();
                        }
                    }
                }
            }
            catch { }
            return "";
        }

        /// <summary>
        /// Belirtilen HWID için geçerli 64 karakterlik SHA-256 lisans anahtarını üretir.
        /// </summary>
        public static string LisansAnahtariUret(string hwid)
        {
            string hamLisans = $"{hwid.Trim().ToUpper()}::{GIZLI_TUZ_ANAHTAR}";
            return Sha256Ozet(hamLisans).ToUpper();
        }

        /// <summary>
        /// Uygulama açılırken lisansın bu bilgisayara (HWID) kilitli olup olmadığını denetler.
        /// </summary>
        public static bool LisansDogrula()
        {
            string mevcutHwid = DonanimKimligiAl();
            string beklenenAnahtar = LisansAnahtariUret(mevcutHwid);

            // 1. Dosya var ve içerik bu bilgisayarın HWID anahtarı ile tam uyuşuyorsa doğrudan aç
            if (File.Exists(lisansDosyaYolu))
            {
                string girilenAnahtar = File.ReadAllText(lisansDosyaYolu).Trim().ToUpper();
                if (girilenAnahtar == beklenenAnahtar)
                {
                    return true; // Lisans geçerli, uygulama çalışmaya devam etsin
                }
            }

            // 2. Lisans yok veya başka bilgisayardan kopyalanmışsa Aktivasyon ve Doğrulama Ekranı
            using (Form aktivasyonForm = new Form())
            {
                aktivasyonForm.Text = "🔒 Akıllı Restoran Otomasyonu - Lisans ve Donanım Doğrulama";
                aktivasyonForm.Size = new System.Drawing.Size(530, 365);
                aktivasyonForm.StartPosition = FormStartPosition.CenterScreen;
                aktivasyonForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                aktivasyonForm.MaximizeBox = false;
                aktivasyonForm.MinimizeBox = false;
                aktivasyonForm.BackColor = System.Drawing.Color.FromArgb(36, 36, 36);
                aktivasyonForm.ForeColor = System.Drawing.Color.White;

                Label lblBilgi = new Label()
                {
                    Text = "GÜVENLİK VE LİSANS DOĞRULAMA KİLİDİ\n\n" +
                           "Bu bilgisayarın Donanım Kimliği (HWID):\n" + mevcutHwid + "\n\n" +
                           "Lütfen yazılım sahibinden aldığınız 64 karakterlik lisans anahtarını giriniz:",
                    Left = 20, Top = 18, Width = 480, Height = 85,
                    Font = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Regular)
                };

                TextBox txtHWID = new TextBox()
                {
                    Text = mevcutHwid, Left = 20, Top = 105, Width = 360, ReadOnly = true,
                    Font = new System.Drawing.Font("Consolas", 10.5f, System.Drawing.FontStyle.Bold),
                    BackColor = System.Drawing.Color.FromArgb(50, 50, 50), ForeColor = System.Drawing.Color.Gold
                };

                Button btnHwidKopyala = new Button()
                {
                    Text = "HWID Kopyala", Left = 390, Top = 104, Width = 105, Height = 26,
                    BackColor = System.Drawing.Color.FromArgb(60, 60, 60), ForeColor = System.Drawing.Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnHwidKopyala.Click += (s, e) => { Clipboard.SetText(mevcutHwid); MessageBox.Show("Donanım Kimliği panoya kopyalandı:\n" + mevcutHwid); };

                TextBox txtLisans = new TextBox()
                {
                    Left = 20, Top = 150, Width = 475, Height = 30,
                    Font = new System.Drawing.Font("Consolas", 9f, System.Drawing.FontStyle.Regular),
                    BackColor = System.Drawing.Color.FromArgb(24, 24, 24), ForeColor = System.Drawing.Color.White
                };

                Button btnAktiveEt = new Button()
                {
                    Text = "✔ LİSANSI AKTİF ET VE KAYDET", Left = 20, Top = 195, Width = 285, Height = 42,
                    BackColor = System.Drawing.Color.FromArgb(39, 174, 96), ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold), FlatStyle = FlatStyle.Flat
                };

                Button btnUstaUret = new Button()
                {
                    Text = "⚙ [GELİŞTİRİCİ] ANAHTAR ÜRET", Left = 315, Top = 195, Width = 180, Height = 42,
                    BackColor = System.Drawing.Color.FromArgb(211, 84, 0), ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold), FlatStyle = FlatStyle.Flat
                };

                Label lblNot = new Label()
                {
                    Text = "* Geliştirici Anahtar Üret butonu sizin test ve kurulum işlemleriniz içindir.\nMüşteriye kurulum yaptıktan sonra bu butonu gizleyerek teslim edebilirsiniz.",
                    Left = 20, Top = 255, Width = 480, Height = 55,
                    Font = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Italic),
                    ForeColor = System.Drawing.Color.LightGray
                };

                bool aktivasyonBasarili = false;

                btnAktiveEt.Click += (s, e) =>
                {
                    if (txtLisans.Text.Trim().ToUpper() == beklenenAnahtar)
                    {
                        File.WriteAllText(lisansDosyaYolu, beklenenAnahtar);
                        MessageBox.Show("✔ Lisans başarıyla doğrulandı ve bu bilgisayara kilitlendi!\nArtık sistemi sınırsız ve güvenle kullanabilirsiniz.", "Aktivasyon Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        aktivasyonBasarili = true;
                        aktivasyonForm.Close();
                    }
                    else
                    {
                        MessageBox.Show("❌ Geçersiz Lisans Anahtarı!\nGirilen anahtar bu bilgisayarın Donanım Kimliği (HWID) ile uyuşmuyor.", "Aktivasyon Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                btnUstaUret.Click += (s, e) =>
                {
                    txtLisans.Text = beklenenAnahtar;
                    MessageBox.Show("Geliştirici Modu: Bu bilgisayara özel ürettiğimiz 64 karakterlik anahtar kutuya yapıştırıldı!\n'Lisansı Aktif Et' butonuna basarak kalıcı olarak kaydedebilirsiniz.", "Geliştirici Usta Anahtarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                aktivasyonForm.Controls.Add(lblBilgi);
                aktivasyonForm.Controls.Add(txtHWID);
                aktivasyonForm.Controls.Add(btnHwidKopyala);
                aktivasyonForm.Controls.Add(txtLisans);
                aktivasyonForm.Controls.Add(btnAktiveEt);
                aktivasyonForm.Controls.Add(btnUstaUret);
                aktivasyonForm.Controls.Add(lblNot);

                aktivasyonForm.ShowDialog();

                return aktivasyonBasarili;
            }
        }

        private static string Sha256Ozet(string metin)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(metin));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
