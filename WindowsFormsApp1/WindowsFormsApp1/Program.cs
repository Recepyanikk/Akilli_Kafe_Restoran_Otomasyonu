using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Tersine Mühendislik ve Debugger (Anti-Decompile / Anti-Dump) Kalkanı
            if (!AntiReverseEngineering.KalkanlariDevreyeAl())
            {
                return;
            }

            // 2. Donanım Kimliği (HWID) ve Lisans Kontrolü
            if (!LisansKontrol.LisansDogrula())
            {
                return; // Lisans geçersiz veya iptal edildiyse uygulama açılmaz
            }

            LoginFrom login = new LoginFrom();

            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MainFrom(login.KullaniciPozisyonu)); // sadece OK ise açılır
            }

        }
    }
}
