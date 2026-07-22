using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class AntiReverseEngineering
    {
        // Win32 API Kernel32 Korumaları
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static bool _korumaAktif = false;
        private static Thread _bekciIpligi = null;

        // Bilinen Tersine Mühendislik (Reverse Engineering), Decompile, Debugger ve Memory Dump Araçları
        private static readonly string[] YasakliAraclar = new string[]
        {
            "dnspy", "ilspy", "de4dot", "x64dbg", "x32dbg", "ollydbg", "cheatengine", "cheat engine",
            "processhacker", "process hacker", "ida64", "ida", "ghidra", "megadumper", "scylla",
            "fiddler", "charles", "wireshark", "dump", "debugger", "httpdebugger"
        };

        /// <summary>
        /// Uygulama başlarken tüm tersine mühendislik (Anti-Reverse Engineering) kalkanlarını devreye sokar.
        /// </summary>
        public static bool KalkanlariDevreyeAl()
        {
#if DEBUG
            // Geliştirici (Visual Studio Debug) modunda çalışırken uygulamanın hata ayıklayıcı (Debugger) tarafından 
            // engellenmemesi için koruma kalkanları otomatik olarak pasif hale getirilir.
            // (Release modunda derlendiğinde tüm korumalar %100 aktif olur).
            return true;
#else
            try
            {
                // 1. Anlık Debugger (Hata Ayıklayıcı) Kontrolü
                if (DebuggerKontrol())
                {
                    GuvenlikIhlaliVeKapat("Sistemde aktif bir Hata Ayıklayıcı (Debugger) veya Tersine Mühendislik aracı algılandı!");
                    return false;
                }

                // 2. Çalışan Süreçleri (Process) Tarama (dnSpy, ILSpy, CheatEngine vb.)
                string tespitEdilenArac = YasakliSurecTaramasi();
                if (!string.IsNullOrEmpty(tespitEdilenArac))
                {
                    GuvenlikIhlaliVeKapat($"Sistemde yasaklı analiz/tersine mühendislik aracı algılandı: [{tespitEdilenArac.ToUpper()}]");
                    return false;
                }

                // 3. Bellekteki PE (Program Header) Başlıklarını Gizleme (Anti-Memory Dump)
                BellekBasliklariniKarart();

                // 4. Arka Plan Güvenlik Bekçisini (Watchdog Thread) Başlat
                if (!_korumaAktif)
                {
                    _korumaAktif = true;
                    _bekciIpligi = new Thread(ArkaPlanBekciDöngüsü);
                    _bekciIpligi.IsBackground = true;
                    _bekciIpligi.Priority = ThreadPriority.Highest;
                    _bekciIpligi.Start();
                }

                return true;
            }
            catch
            {
                return true;
            }
#endif
        }

        private static bool DebuggerKontrol()
        {
            try
            {
                // Managed Debugger (Visual Studio / dnSpy vb.)
                if (Debugger.IsAttached) return true;

                // Win32 Native Debugger
                if (IsDebuggerPresent()) return true;

                // Remote / Kernel Debugger
                bool remoteDebugger = false;
                CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref remoteDebugger);
                if (remoteDebugger) return true;
            }
            catch { }
            return false;
        }

        private static string YasakliSurecTaramasi()
        {
            try
            {
                Process[] surecler = Process.GetProcesses();
                foreach (Process p in surecler)
                {
                    try
                    {
                        string surecAdi = p.ProcessName.ToLower().Trim();
                        string pencereBasligi = p.MainWindowTitle.ToLower().Trim();

                        foreach (string yasakli in YasakliAraclar)
                        {
                            if (surecAdi.Contains(yasakli) || pencereBasligi.Contains(yasakli))
                            {
                                // Kendi uygulamamızın adı rastgele eşleşmesin
                                if (!surecAdi.Contains("windowsformsapp1") && !surecAdi.Contains("akilli"))
                                {
                                    return yasakli;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// RAM üzerinden .exe dosyasının dökümünü (Dump) almalarını engellemek için bellek başlığındaki 'MZ' baytlarını temizler.
        /// </summary>
        private static void BellekBasliklariniKarart()
        {
            try
            {
                IntPtr baslikAdresi = GetModuleHandle(null);
                if (baslikAdresi != IntPtr.Zero)
                {
                    uint eskiKoruma;
                    // Bellek alanına yazma izni ver
                    if (VirtualProtect(baslikAdresi, (UIntPtr)4096, 0x40 /* PAGE_EXECUTE_READWRITE */, out eskiKoruma))
                    {
                        // MZ (4D 5A) başlığını 0 sıfır baytıyla ez
                        Marshal.Copy(new byte[] { 0, 0, 0, 0 }, 0, baslikAdresi, 4);
                        // Koruma seviyesini eski haline getir
                        VirtualProtect(baslikAdresi, (UIntPtr)4096, eskiKoruma, out _);
                    }
                }
            }
            catch { }
        }

        private static void ArkaPlanBekciDöngüsü()
        {
            while (_korumaAktif)
            {
                try
                {
                    Thread.Sleep(2500); // 2.5 saniyede bir sessiz tarama

                    if (DebuggerKontrol())
                    {
                        SessizcaSondur();
                        break;
                    }

                    string arac = YasakliSurecTaramasi();
                    if (!string.IsNullOrEmpty(arac))
                    {
                        SessizcaSondur();
                        break;
                    }
                }
                catch { }
            }
        }

        private static void GuvenlikIhlaliVeKapat(string mesaj)
        {
            try
            {
                MessageBox.Show(
                    "🚨 GÜVENLİK VE TERSİNE MÜHENDİSLİK KORUMASI DEVREDE!\n\n" +
                    mesaj + "\n\n" +
                    "Sistem bütünlüğünü korumak amacıyla uygulama güvenli şekilde sonlandırılıyor.",
                    "Anti-Reverse Engineering Kalkanı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop
                );
            }
            catch { }
            finally
            {
                SessizcaSondur();
            }
        }

        private static void SessizcaSondur()
        {
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
                Environment.Exit(0);
            }
        }
    }
}
