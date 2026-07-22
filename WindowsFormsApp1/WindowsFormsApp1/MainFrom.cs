using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class MainFrom : Form
    {
        string secilenResimYolu = "";
        string secilenUrunResimYolu = "";

        private string kullaniciPozisyonu;
        int secilenPersonelID = -1;
        int secilenKategoriID= -1;
        int secilenUrunID = -1;
        
        public MainFrom(string pozisyon)
        {
            InitializeComponent();

            kullaniciPozisyonu = pozisyon;
        }

        private bool YoneticiYetkisiKontrolEt()
        {
            if (string.IsNullOrWhiteSpace(kullaniciPozisyonu) || kullaniciPozisyonu.Trim().ToLower() != "yönetici")
            {
                MessageBox.Show("Bu işlemi gerçekleştirmek için yalnızca YÖNETİCİ yetkisine sahip olmalısınız!", "Erişim Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        #region Formun yüklenmesi
        private void LoginForm_Load(object sender, EventArgs e)
        {
            VeritabaniMasaSutunKontrol();
            MasalariYukle();
            KategorileriYukle();
            MasaButonlariniBagla();
            SiparisListele();
            AnlikCiroGuncelle();
            string p = kullaniciPozisyonu.ToLower();
            if (panelMasaKontrol != null)
            {
                panelMasaKontrol.Visible = (kullaniciPozisyonu.Trim().ToLower() == "yönetici");
            }
            PozisyonComboBox.Visible = false;
            Kaydet.Visible = false;
            Kaydet2.Visible = false;
            SifreTxt.Visible = false;
            KategoriADtxt.Visible = false;
            KategoriKaydetBtn.Visible = false;
            KategoriResimBtn.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label10.Visible = false;
            UrunKtgComboBx.Visible = false;
            UrunKaydetBtn.Visible = false;
            UrunResimBtn.Visible = false;
            UrunFytGuncelTxt.Visible = false;
            UrunAdTxt.Visible = false;
            UrunFiyatTxt.Visible = false;

        }
        #endregion

        #region Yetki Kontrol Fonksiyonu
        private void YetkiKontrol(string yetkiliPozisyonlar, string islemAdi, TabPage tab)
        {
            string p = kullaniciPozisyonu.Trim().ToLower();

            // Yönetici her zaman erişebilir
            if (p == "yönetici" || yetkiliPozisyonlar.ToLower().Split(',').Contains(p))
            {
                // Yetkili ise tab’a geç
                if (tab != null)
                {
                    menu.SelectedTab = tab;
                }

               
            }
            else
            {
                MessageBox.Show($"Bu işlemi yapmaya yetkiniz yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();   // Main kapanınca tüm uygulama kapanır
        }
        #region butonlara tıklandığında yetki kontrolü
        private void btnMasalar_Click(object sender, EventArgs e)
        {
            YetkiKontrol("garson,mutfak", "Masalar", MasalarTabPage);
        }

        private void btnYonetim_Click(object sender, EventArgs e)
        {
            YetkiKontrol("", "Yönetim", YonetimTapPage);
        }

        private void btnMutfak_Click(object sender, EventArgs e)//mutfak kısmına girerken kontrol ve yüklenme
        {
            YetkiKontrol("mutfak", "Mutfak", MutfakTabPage);

            SiparisListele();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            YetkiKontrol("", "Raporlar", RaporlarTabPage);
            AnlikCiroGuncelle();
        }
        #endregion
        private void button20_Click(object sender, EventArgs e)
        {

        }
        #region masadurumlerinin yüklenmesi MasalariYukle()
        private void MasalariYukle()
        {
            int seciliMasaID = -1;
            if (cbSilinecekMasa != null && cbSilinecekMasa.SelectedItem is MasaComboItem eskiSecilen)
            {
                seciliMasaID = eskiSecilen.MasaID;
            }

            if (flpMasalar != null) flpMasalar.Controls.Clear();
            if (cbSilinecekMasa != null) cbSilinecekMasa.Items.Clear();

            using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT MasaID, Durum, HesapTutari, Aciklama, MasaAdi FROM Masalar ORDER BY MasaID", con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    int masaID = Convert.ToInt32(dr["MasaID"]);
                    string rawDurum = dr["Durum"] != DBNull.Value ? dr["Durum"].ToString().Trim() : "Boş";
                    decimal tutar = dr["HesapTutari"] != DBNull.Value ? Convert.ToDecimal(dr["HesapTutari"]) : 0m;
                    string masaAdi = dr["MasaAdi"] != DBNull.Value ? dr["MasaAdi"].ToString().Trim() : "";
                    if (string.IsNullOrWhiteSpace(masaAdi))
                        masaAdi = dr["Aciklama"] != DBNull.Value ? dr["Aciklama"].ToString().Trim() : "";
                    if (string.IsNullOrWhiteSpace(masaAdi))
                        masaAdi = "Masa " + masaID;

                    // Türkçe karakter ve renk kontrolü için normalizasyon
                    string durumLower = rawDurum
                        .Replace("ş", "s")
                        .Replace("ı", "i")
                        .Replace("ö", "o")
                        .Replace("ü", "u")
                        .Replace("ğ", "g")
                        .Replace("ç", "c")
                        .Trim()
                        .ToLower();

                    string durumGosterim = "Boş";
                    if (durumLower == "rezerve") durumGosterim = "Rezerve";
                    else if (durumLower == "dolu") durumGosterim = "Dolu";
                    else durumGosterim = rawDurum;

                    if (flpMasalar != null)
                    {
                        Button btn = new Button();
                        btn.Name = "btnMasa" + masaID;
                        btn.Tag = masaID;
                        btn.Size = new Size(150, 97);
                        btn.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(162)));
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.ForeColor = Color.White;
                        btn.Margin = new Padding(10);

                        // RENK UYGULA
                        if (durumLower == "bos") btn.BackColor = Color.FromArgb(10, 166, 23);
                        else if (durumLower == "dolu") btn.BackColor = Color.FromArgb(148, 17, 6);
                        else if (durumLower == "rezerve") btn.BackColor = Color.FromArgb(201, 169, 8);
                        else btn.BackColor = Color.FromArgb(10, 166, 23);

                        // YAZILARI UYGULA
                        btn.Text = $"{masaAdi}\n{tutar} ₺\n{durumGosterim}";
                        btn.Click += Masa_Click;
                        btn.MouseDown += Masa_MouseDown;

                        flpMasalar.Controls.Add(btn);
                    }

                    if (cbSilinecekMasa != null)
                    {
                        string cbText = masaAdi;
                        if (!masaAdi.ToLower().Contains(masaID.ToString()))
                            cbText += $" (ID: {masaID})";
                        cbSilinecekMasa.Items.Add(new MasaComboItem { MasaID = masaID, Text = cbText, MasaAdi = masaAdi, Durum = durumGosterim });
                    }
                }
            }

            if (cbSilinecekMasa != null && cbSilinecekMasa.Items.Count > 0)
            {
                bool secildi = false;
                if (seciliMasaID != -1)
                {
                    foreach (MasaComboItem item in cbSilinecekMasa.Items)
                    {
                        if (item.MasaID == seciliMasaID)
                        {
                            cbSilinecekMasa.SelectedItem = item;
                            secildi = true;
                            break;
                        }
                    }
                }
                if (!secildi && cbSilinecekMasa.SelectedIndex == -1)
                {
                    cbSilinecekMasa.SelectedIndex = 0;
                }
            }
        }
        #endregion

        #region personellerin datagridde listelenmesi
        private void personellistele()
        {
            SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True");
            string veriler = "SELECT PersonelID, Ad, KullaniciAdi, SifreHash, Pozisyon From Personel";
            SqlDataAdapter adapter = new SqlDataAdapter(veriler, con);
            DataTable dataTable = new DataTable();

            try
            {
                adapter.Fill(dataTable);
                dataGridPersonel.DataSource = dataTable;
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Veri yüklenirken hata oluştu: " + ex.Message);
            }
        }
        #endregion
        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        #region alt tabpage butonişlemleri
        private void Kategori_Click(object sender, EventArgs e)
        {
            AltMenuControl.SelectedTab = IslemlerKTabPage;
        }

        private void PersonelDuzen_Click(object sender, EventArgs e) //bu kısımda menü butonlarından tabpage yönlendirme yapıldı
        {
            AltMenuControl.SelectedTab = IslemlerPTabPage;
        }

        private void UrunDuzen_Click(object sender, EventArgs e)
        {
            AltMenuControl.SelectedTab = IslemlerUTabPage;
        }
        #endregion
        private void PListele_Click(object sender, EventArgs e)
        {
            personellistele();
        }
        #region pozisyon değiştirme kısmının görünür kılınması
        private void PozisyonDegistirmeBtn_Click(object sender, EventArgs e)
        {
            
            if (dataGridPersonel.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir personel seçin");
                return;
            }

            PozisyonComboBox.Visible = true;
            Kaydet.Visible = true;

            // Seçili personelin mevcut pozisyonunu ComboBox’ta göster
            PozisyonComboBox.Text = dataGridPersonel.SelectedRows[0]
                                .Cells["Pozisyon"].Value.ToString();
        }
        #endregion
        private void label1_Click_1(object sender, EventArgs e)
        {

        }
        #region şifre değiştirme kısmının görünür kılınması
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridPersonel.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir personel seçin");
                return;
            }

            Kaydet2.Visible = true;
            SifreTxt.Visible = true;
               
        }
        #endregion

        # region pozisyonun değiştirilip kaydedilmesi
        private void Kaydet_Click(object sender, EventArgs e)
        {
            if(secilenPersonelID == -1)
            {
                MessageBox.Show("Lütfen bir personel seçiniz");
                return;
            }

            if (PozisyonComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen yeni pozisyon seçiniz");
                return;
            }

            string yeniPozisyon = PozisyonComboBox.SelectedItem.ToString();

            SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True");
            SqlCommand cmd = new SqlCommand(
                "UPDATE Personel SET Pozisyon=@p WHERE PersonelID=@id", con);

            cmd.Parameters.AddWithValue("@p", yeniPozisyon);
            cmd.Parameters.AddWithValue("@id", secilenPersonelID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Pozisyon güncellendi");

            
            PozisyonComboBox.Visible=false;
            Kaydet.Visible=false;
        }
        #endregion

        #region şifrenin değiştirilip kaydedilmesi
        private void Kaydet2_Click(object sender, EventArgs e)
        {
            if (secilenPersonelID == -1)
            {
                MessageBox.Show("Lütfen bir personel seçiniz");
                return;
            }

            if (SifreTxt.Text == "")
            {
                MessageBox.Show("Lütfen şifre giriniz");
                return;
            }

            string yeniSifre = SifreTxt.Text.ToString();

            SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True");
            SqlCommand cmd = new SqlCommand(
                "UPDATE Personel SET SifreHash=@p WHERE PersonelID=@id", con);

            cmd.Parameters.AddWithValue("@p", yeniSifre);
            cmd.Parameters.AddWithValue("@id", secilenPersonelID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Sifre güncellendi");

            SifreTxt.Visible=false;
            Kaydet2.Visible=false;
            
        }
        #endregion
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridPersonel_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        #region personel datagridinden personel seçilip ıd sinin alınması
        private void dataGridPersonel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridPersonel.Rows[e.RowIndex];

                secilenPersonelID = Convert.ToInt32(row.Cells["PersonelID"].Value);


                // Mevcut pozisyonu combobox'ta seçili getir
                PozisyonComboBox.SelectedItem = row.Cells["Pozisyon"].Value.ToString();
            }
        }
        #endregion

        #region personel silme buton
        private void button1_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            if (secilenPersonelID == -1)
            {
                MessageBox.Show("Lütfen silinecek personeli seçiniz");
                return;
            }

            DialogResult sonuc = MessageBox.Show(
                "Seçilen personeli silmek istiyor musunuz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (sonuc == DialogResult.No)
                return;

            using (SqlConnection con = new SqlConnection(
                @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Personel WHERE PersonelID=@id", con);

                cmd.Parameters.AddWithValue("@id", secilenPersonelID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Personel silindi");

            // Seçimi sıfırla
            secilenPersonelID = -1;
        }
        #endregion
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            Form1 frm = new Form1();
            frm.ShowDialog();
        }

        #region kategori kısmının görünmesi resmin yüklenmesi kategorinin kaydedilmesi
        private void KategoriEkleBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            KategoriADtxt.Visible = true;
            KategoriResimBtn.Visible = true;
            KategoriKaydetBtn.Visible = true;
        }

        
        private void KategoriResimBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            KategoriOpenFileD.Title = "Kategori resmi seç";
            KategoriOpenFileD.Filter = "Resim Dosyaları|*.jpg;*.png;*.jpeg";

            if (KategoriOpenFileD.ShowDialog() == DialogResult.OK)
            {
                secilenResimYolu = KategoriOpenFileD.FileName;
                MessageBox.Show("Resim seçildi");
            }
        }

        private void KategoriKaydetBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            if (KategoriADtxt.Text == "")
            {
                MessageBox.Show("Kategori adı giriniz");
                return;
            }

            if (secilenResimYolu == "")
            {
                MessageBox.Show("Lütfen bir resim seçiniz");
                return;
            }

            SqlConnection con = new SqlConnection(
                @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True");

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Kategoriler (KategoriAdi, ResimYolu) VALUES (@ad, @resim)", con);

            cmd.Parameters.AddWithValue("@ad", KategoriADtxt.Text);
            cmd.Parameters.AddWithValue("@resim", secilenResimYolu);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Kategori başarıyla eklendi");

            
            KategoriADtxt.Clear();
            secilenResimYolu = "";
            KategoriADtxt.Visible = false;  
            KategoriKaydetBtn.Visible = false;
            KategoriResimBtn.Visible = false;
            
        }
        #endregion

        #region kategorilerin listelenmesi kısmı
        private void KategoriListeleBtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(
            @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT KategoriID, KategoriAdi, ResimYolu FROM Kategoriler", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                KategoriDataGrid.DataSource = dt;
            }
        }
        #endregion

        #region kategori ıd nin datagride seçilen kısımdan alınması
        private void KategoriDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = KategoriDataGrid.Rows[e.RowIndex];
                    secilenKategoriID = Convert.ToInt32(row.Cells["KategoriID"].Value);
                }
            }
        }
        #endregion

        #region kategorinin silinmesi
        private void KategoriSilBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            HashSet<int> silinecekIDs = new HashSet<int>();

            if (KategoriDataGrid != null)
            {
                foreach (DataGridViewRow row in KategoriDataGrid.SelectedRows)
                {
                    if (row.Cells["KategoriID"]?.Value != null && int.TryParse(row.Cells["KategoriID"].Value.ToString(), out int id))
                        silinecekIDs.Add(id);
                }
                foreach (DataGridViewCell cell in KategoriDataGrid.SelectedCells)
                {
                    if (cell.OwningRow?.Cells["KategoriID"]?.Value != null && int.TryParse(cell.OwningRow.Cells["KategoriID"].Value.ToString(), out int id))
                        silinecekIDs.Add(id);
                }
            }

            if (silinecekIDs.Count == 0 && secilenKategoriID != -1)
            {
                silinecekIDs.Add(secilenKategoriID);
            }

            if (silinecekIDs.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek kategori veya kategorileri seçiniz");
                return;
            }

            string soru = silinecekIDs.Count == 1 
                ? "Seçilen kategoriyi silmek istiyor musunuz?" 
                : $"Seçilen {silinecekIDs.Count} adet kategoriyi silmek istiyor musunuz?";

            DialogResult sonuc = MessageBox.Show(soru, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (sonuc == DialogResult.No) return;

            try
            {
                using (SqlConnection con = new SqlConnection(
                    @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                {
                    con.Open();
                    foreach (int id in silinecekIDs)
                    {
                        string detaySilSorgu = "DELETE FROM SiparisDetay WHERE UrunID IN (SELECT UrunID FROM Urunler WHERE KategoriID = @id)";
                        using (SqlCommand cmdDetay = new SqlCommand(detaySilSorgu, con))
                        {
                            cmdDetay.Parameters.AddWithValue("@id", id);
                            cmdDetay.ExecuteNonQuery();
                        }

                        string urunSilSorgu = "DELETE FROM Urunler WHERE KategoriID = @id";
                        using (SqlCommand cmdUrun = new SqlCommand(urunSilSorgu, con))
                        {
                            cmdUrun.Parameters.AddWithValue("@id", id);
                            cmdUrun.ExecuteNonQuery();
                        }

                        string kategoriSilSorgu = "DELETE FROM Kategoriler WHERE KategoriID = @id";
                        using (SqlCommand cmdKategori = new SqlCommand(kategoriSilSorgu, con))
                        {
                            cmdKategori.Parameters.AddWithValue("@id", id);
                            cmdKategori.ExecuteNonQuery();
                        }
                    }
                }
                secilenKategoriID = -1;
                KategoriListeleBtn_Click(null, null);
                UrunListeleBtn_Click(null, null);
                KategorileriYukle();
                string mesaj = silinecekIDs.Count == 1 ? "Kategori ve bağlı tüm ürünler başarıyla silindi." : $"{silinecekIDs.Count} adet kategori ve bağlı ürünleri başarıyla silindi.";
                MessageBox.Show(mesaj, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Kategori Silme Hatası: " + ex.ToString());
                MessageBox.Show("İşlem sırasında bir sistem hatası oluştu. Lütfen sistem yöneticinizle iletişime geçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
        #region ürün katagorileri combox ına  kategori bilgilerinin yüklenmesi KategorileriYukle()
        void KategorileriYukle()
        {
            using (SqlConnection con = new SqlConnection(
                @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT KategoriID, KategoriAdi FROM Kategoriler", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                UrunKtgComboBx.DisplayMember = "KategoriAdi";
                UrunKtgComboBx.ValueMember = "KategoriID";
                UrunKtgComboBx.DataSource = dt;
            }
        }
        #endregion

        #region ürün ekleme kısmının görünür kılınması
        private void UrunEkleBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            label6.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            UrunKtgComboBx.Visible = true;
            UrunKaydetBtn.Visible = true;
            UrunResimBtn.Visible = true;
            UrunAdTxt.Visible = true;
            UrunFiyatTxt.Visible = true;
        }
        #endregion

        #region ürün resmi seçilmesi
        private void UrunResimBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            KategoriOpenFileD.Title = "Ürün resmi seç";
            KategoriOpenFileD.Filter = "Resim Dosyaları|*.jpg;*.png;*.jpeg";

            if (KategoriOpenFileD.ShowDialog() == DialogResult.OK)
            {
                secilenUrunResimYolu = KategoriOpenFileD.FileName;
                MessageBox.Show("Resim seçildi");
            }
        }
        #endregion

        #region ürünlerin eklenmesi
        private void UrunKaydetBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            if (UrunKtgComboBx.SelectedIndex == -1 ||
                UrunAdTxt.Text == "" ||
                UrunFiyatTxt.Text == "")
            {
                MessageBox.Show("Tüm alanları doldurun");
                return;
            }

            int kategoriID = Convert.ToInt32(UrunKtgComboBx.SelectedValue);
            string urunAdi = UrunAdTxt.Text;
            decimal fiyat;

            if (!decimal.TryParse(UrunFiyatTxt.Text, out fiyat))
            {
                MessageBox.Show("Fiyat geçersiz");
                return;
            }

            using (SqlConnection con = new SqlConnection(
                @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, ResimYolu) VALUES (@k, @a, @f, @r)", con);

                cmd.Parameters.AddWithValue("@k", kategoriID);
                cmd.Parameters.AddWithValue("@a", urunAdi);
                cmd.Parameters.AddWithValue("@f", fiyat);
                cmd.Parameters.AddWithValue("@r", string.IsNullOrEmpty(secilenUrunResimYolu) ? (object)DBNull.Value : secilenUrunResimYolu);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Ürün eklendi");

            UrunAdTxt.Clear();
            UrunFiyatTxt.Clear();
            secilenUrunResimYolu = "";
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            UrunKtgComboBx.Visible = false;
            UrunKaydetBtn.Visible = false;
            UrunResimBtn.Visible = false;
            UrunAdTxt.Visible = false;
            UrunFiyatTxt.Visible = false;
        }
        #endregion

        #region ürünlerin ürün datagridinde listelenmesi
        private void UrunListeleBtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(
            @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT 
                    u.UrunID,
                    u.UrunAdi,
                    k.KategoriAdi,
                    u.Fiyat,
                    u.ResimYolu
                    FROM Urunler u
                    INNER JOIN Kategoriler k ON u.KategoriID = k.KategoriID",
                    con
                );

                DataTable dt = new DataTable();
                da.Fill(dt);

                UrunDataGrid.DataSource = dt;
            }
        }
        #endregion

        #region datagriden seçilen ürünlerin silinme işlemleri
        private void UrunSilBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            HashSet<int> silinecekIDs = new HashSet<int>();

            if (UrunDataGrid != null)
            {
                foreach (DataGridViewRow row in UrunDataGrid.SelectedRows)
                {
                    if (row.Cells["UrunID"]?.Value != null && int.TryParse(row.Cells["UrunID"].Value.ToString(), out int id))
                        silinecekIDs.Add(id);
                }
                foreach (DataGridViewCell cell in UrunDataGrid.SelectedCells)
                {
                    if (cell.OwningRow?.Cells["UrunID"]?.Value != null && int.TryParse(cell.OwningRow.Cells["UrunID"].Value.ToString(), out int id))
                        silinecekIDs.Add(id);
                }
            }

            if (silinecekIDs.Count == 0 && secilenUrunID != -1)
            {
                silinecekIDs.Add(secilenUrunID);
            }

            if (silinecekIDs.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek ürünü veya ürünleri seçiniz");
                return;
            }

            string soru = silinecekIDs.Count == 1 
                ? "Seçilen ürünü silmek istiyor musunuz?" 
                : $"Seçilen {silinecekIDs.Count} adet ürünü silmek istiyor musunuz?";

            DialogResult sonuc = MessageBox.Show(soru, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (sonuc == DialogResult.No) return;

            try
            {
                using (SqlConnection con = new SqlConnection(
                    @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                {
                    con.Open();
                    foreach (int id in silinecekIDs)
                    {
                        using (SqlCommand cmdDetay = new SqlCommand("DELETE FROM SiparisDetay WHERE UrunID=@id", con))
                        {
                            cmdDetay.Parameters.AddWithValue("@id", id);
                            cmdDetay.ExecuteNonQuery();
                        }
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Urunler WHERE UrunID=@id", con))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                secilenUrunID = -1;
                UrunListeleBtn_Click(null, null);
                string mesaj = silinecekIDs.Count == 1 ? "Ürün başarıyla silindi." : $"{silinecekIDs.Count} adet ürün başarıyla silindi.";
                MessageBox.Show(mesaj, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Ürün Silme Hatası: " + ex.ToString());
                MessageBox.Show("İşlem sırasında bir sistem hatası oluştu. Lütfen sistem yöneticinizle iletişime geçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region datagriden seçilen ürünün idsinin alınması
        private void UrunDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = UrunDataGrid.Rows[e.RowIndex];
                    secilenUrunID = Convert.ToInt32(row.Cells["UrunID"].Value);
                    UrunFytGuncelTxt.Text = row.Cells["Fiyat"].Value.ToString();
                }
            }
        }
        #endregion

        #region ürün fiyatı güncelleme kısmının görünür kılınması
        private void UrunGuncelleBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            UrunFytGuncelTxt.Visible = true;
            UrunGuncelKaydetBtn.Visible = true;
            label10.Visible = true;
        }
        #endregion

        #region fiyatın güncellenip kaydedilemsi
        private void UrunGuncelKaydetBtn_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            if (secilenUrunID == -1)
            {
                MessageBox.Show("Lütfen bir ürün seçin");
                return;
            }

            if (UrunFytGuncelTxt.Text == "")
            {
                MessageBox.Show("Yeni fiyat giriniz");
                return;
            }

            decimal yeniFiyat;
            if (!decimal.TryParse(UrunFytGuncelTxt.Text, out yeniFiyat))
            {
                MessageBox.Show("Geçerli bir fiyat giriniz");
                return;
            }

            using (SqlConnection con = new SqlConnection(
                @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Urunler SET Fiyat=@fiyat WHERE UrunID=@id", con);

                cmd.Parameters.AddWithValue("@fiyat", yeniFiyat);
                cmd.Parameters.AddWithValue("@id", secilenUrunID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Ürün fiyatı güncellendi");

            secilenUrunID = -1;
            UrunFytGuncelTxt.Clear();
            UrunFytGuncelTxt.Visible = false;
            UrunGuncelKaydetBtn.Visible = false;
            label10.Visible = false;

        }
        #endregion

        #region Masa Ekle - Sil - Revize ve Yardımcı Sınıflar
        private void VeritabaniMasaSutunKontrol()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                {
                    con.Open();
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Masalar' AND COLUMN_NAME = 'MasaAdi'", con);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists == 0)
                    {
                        SqlCommand alterCmd = new SqlCommand("ALTER TABLE Masalar ADD MasaAdi nvarchar(50) NULL", con);
                        alterCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception) { }
        }

        public class MasaComboItem
        {
            public int MasaID { get; set; }
            public string Text { get; set; }
            public string MasaAdi { get; set; }
            public string Durum { get; set; }
            public override string ToString() { return Text; }
        }

        private void cbSilinecekMasa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSilinecekMasa.SelectedItem is MasaComboItem secilen)
            {
                if (txtMasaAdi != null) txtMasaAdi.Text = secilen.MasaAdi;
                if (cbMasaDurumu != null)
                {
                    if (cbMasaDurumu.Items.Contains(secilen.Durum))
                    {
                        cbMasaDurumu.SelectedItem = secilen.Durum;
                    }
                    else if (!string.IsNullOrEmpty(secilen.Durum) && secilen.Durum.ToLower().Contains("rez"))
                    {
                        cbMasaDurumu.SelectedItem = "Rezerve";
                    }
                    else if (!string.IsNullOrEmpty(secilen.Durum) && secilen.Durum.ToLower().Contains("dol"))
                    {
                        cbMasaDurumu.SelectedItem = "Dolu";
                    }
                    else
                    {
                        cbMasaDurumu.SelectedItem = "Boş";
                    }
                }
            }
        }

        private void btnMasaDurumDegistir_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            if (cbSilinecekMasa.SelectedItem is MasaComboItem secilen)
            {
                string yeniDurum = cbMasaDurumu != null && cbMasaDurumu.SelectedItem != null ? cbMasaDurumu.SelectedItem.ToString().Trim() : "Boş";

                try
                {
                    using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                    {
                        con.Open();
                        string sql = string.Equals(yeniDurum, "Boş", StringComparison.OrdinalIgnoreCase)
                            ? "UPDATE Masalar SET Durum = @durum, HesapTutari = 0 WHERE MasaID = @id"
                            : "UPDATE Masalar SET Durum = @durum WHERE MasaID = @id";

                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@durum", yeniDurum);
                        cmd.Parameters.AddWithValue("@id", secilen.MasaID);
                        cmd.ExecuteNonQuery();
                    }
                    MasalariYukle();
                    MessageBox.Show($"{secilen.MasaAdi} (ID: {secilen.MasaID}) masasının durumu başarıyla '{yeniDurum}' olarak değiştirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Masa durumu değiştirilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen durumu değiştirilecek bir masa seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMasaRevize_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            if (cbSilinecekMasa.SelectedItem is MasaComboItem secilen)
            {
                string yeniAd = txtMasaAdi != null ? txtMasaAdi.Text.Trim() : "";
                string yeniDurum = cbMasaDurumu != null && cbMasaDurumu.SelectedItem != null ? cbMasaDurumu.SelectedItem.ToString().Trim() : "Boş";

                if (string.IsNullOrWhiteSpace(yeniAd))
                {
                    MessageBox.Show("Lütfen revize edilecek masa adını / açıklamasını yazın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Eğer boş masa revize ediliyorsa ve durum açılır kutusunda "Boş" olarak bırakılmışsa, revize işleminden dolayı otomatik olarak "Rezerve" yapılır!
                if (string.Equals(secilen.Durum, "Boş", StringComparison.OrdinalIgnoreCase) && string.Equals(yeniDurum, "Boş", StringComparison.OrdinalIgnoreCase))
                {
                    yeniDurum = "Rezerve";
                    if (cbMasaDurumu != null && cbMasaDurumu.Items.Contains("Rezerve"))
                    {
                        cbMasaDurumu.SelectedItem = "Rezerve";
                    }
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                    {
                        con.Open();
                        string sql = string.Equals(yeniDurum, "Boş", StringComparison.OrdinalIgnoreCase)
                            ? "UPDATE Masalar SET MasaAdi = @adi, Aciklama = @adi, Durum = @durum, HesapTutari = 0 WHERE MasaID = @id"
                            : "UPDATE Masalar SET MasaAdi = @adi, Aciklama = @adi, Durum = @durum WHERE MasaID = @id";

                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@adi", yeniAd);
                        cmd.Parameters.AddWithValue("@durum", yeniDurum);
                        cmd.Parameters.AddWithValue("@id", secilen.MasaID);
                        cmd.ExecuteNonQuery();
                    }
                    MasalariYukle();
                    MessageBox.Show($"{secilen.MasaAdi} (ID: {secilen.MasaID}) masası başarıyla güncellendi (revize edildi).\nYeni Durum: {yeniDurum}\nAçıklama/Ad: {yeniAd}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Masa güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen revize edilecek bir masa seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMasaEkle_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            try
            {
                using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                {
                    con.Open();
                    SqlCommand cmdNo = new SqlCommand("SELECT ISNULL(MAX(MasaNo), 0) + 1 FROM Masalar", con);
                    int yeniMasaNo = Convert.ToInt32(cmdNo.ExecuteScalar());

                    string girilenAd = txtMasaAdi != null && !string.IsNullOrWhiteSpace(txtMasaAdi.Text) 
                        ? txtMasaAdi.Text.Trim() 
                        : "Masa " + yeniMasaNo;

                    // Yeni eklenen masa her zaman otomatik olarak Boş atanır!
                    SqlCommand cmd = new SqlCommand("INSERT INTO Masalar (MasaNo, Durum, HesapTutari, Aciklama, MasaAdi) VALUES (@no, 'Boş', 0, @adi, @adi)", con);
                    cmd.Parameters.AddWithValue("@no", yeniMasaNo);
                    cmd.Parameters.AddWithValue("@adi", girilenAd);
                    cmd.ExecuteNonQuery();
                }
                if (txtMasaAdi != null) txtMasaAdi.Clear();
                if (cbMasaDurumu != null && cbMasaDurumu.Items.Count > 0) cbMasaDurumu.SelectedIndex = 0;
                MasalariYukle();
                MessageBox.Show("Yeni masa başarıyla eklendi ve durumu otomatik olarak 'Boş' atandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Masa eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMasaSil_Click(object sender, EventArgs e)
        {
            if (!YoneticiYetkisiKontrolEt()) return;

            if (cbSilinecekMasa.SelectedItem is MasaComboItem secilen)
            {
                DialogResult dialog = MessageBox.Show($"{secilen.Text} silmek istediğinize emin misiniz?", "Masa Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                        {
                            con.Open();
                            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Siparis WHERE MasaID = @id AND RTRIM(Durum) IN ('Yeni Sipariş', 'Yeni', 'Hazırlanıyor', 'Hazır')", con);
                            cmdCheck.Parameters.AddWithValue("@id", secilen.MasaID);
                            int aktifSiparis = Convert.ToInt32(cmdCheck.ExecuteScalar());

                            SqlCommand cmdMasaDurum = new SqlCommand("SELECT LOWER(RTRIM(ISNULL(Durum, 'bos'))) FROM Masalar WHERE MasaID = @id", con);
                            cmdMasaDurum.Parameters.AddWithValue("@id", secilen.MasaID);
                            string masaDurumu = Convert.ToString(cmdMasaDurum.ExecuteScalar());

                            if (aktifSiparis > 0 || (masaDurumu != "bos" && masaDurumu != "boş" && !string.IsNullOrEmpty(masaDurumu)))
                            {
                                MessageBox.Show("Bu masada aktif sipariş veya durum (dolu/rezerve) bulunduğu için masa silinemez! Önce siparişleri kapatın veya masayı boşaltın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // Önce bu masanın siparişlerine ait detayları sil (FK kısıtlaması nedeniyle)
                            SqlCommand cmdDelDetay = new SqlCommand("DELETE FROM SiparisDetay WHERE SiparisID IN (SELECT SiparisID FROM Siparis WHERE MasaID = @id)", con);
                            cmdDelDetay.Parameters.AddWithValue("@id", secilen.MasaID);
                            cmdDelDetay.ExecuteNonQuery();

                            // Sonra siparişleri sil
                            SqlCommand cmdDelSip = new SqlCommand("DELETE FROM Siparis WHERE MasaID = @id", con);
                            cmdDelSip.Parameters.AddWithValue("@id", secilen.MasaID);
                            cmdDelSip.ExecuteNonQuery();

                            // En son masayı sil
                            SqlCommand cmdDel = new SqlCommand("DELETE FROM Masalar WHERE MasaID = @id", con);
                            cmdDel.Parameters.AddWithValue("@id", secilen.MasaID);
                            cmdDel.ExecuteNonQuery();
                        }
                        MasalariYukle();
                        MessageBox.Show("Masa başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Masa silinirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silinecek bir masa seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region masaların butonlara bağlanması ve tüm masalara ortak click MasaButonlariniBagla()
        void MasaButonlariniBagla()
        {
            if (flpMasalar != null)
            {
                foreach (Control c in flpMasalar.Controls)
                {
                    if (c is Button btn && btn.Name.StartsWith("btnMasa"))
                    {
                        int masaID = int.Parse(btn.Name.Replace("btnMasa", ""));
                        btn.Tag = masaID;
                        btn.Click -= Masa_Click;
                        btn.Click += Masa_Click;
                        btn.MouseDown -= Masa_MouseDown;
                        btn.MouseDown += Masa_MouseDown;
                    }
                }
            }
            foreach (Control c in MasalarTabPage.Controls)
            {
                if (c is Button btn && btn.Name.StartsWith("btnMasa"))
                {
                    int masaID = int.Parse(btn.Name.Replace("btnMasa", ""));
                    btn.Tag = masaID;
                    btn.Click -= Masa_Click;
                    btn.Click += Masa_Click;
                    btn.MouseDown -= Masa_MouseDown;
                    btn.MouseDown += Masa_MouseDown;
                }
            }
        }
        private void Masa_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button btn = sender as Button;
                if (btn == null) return;
                int masaID = Convert.ToInt32(btn.Tag);

                if (cbSilinecekMasa != null && cbSilinecekMasa.Items.Count > 0)
                {
                    foreach (object item in cbSilinecekMasa.Items)
                    {
                        if (item is MasaComboItem secilen && secilen.MasaID == masaID)
                        {
                            cbSilinecekMasa.SelectedItem = secilen;
                            break;
                        }
                    }
                }
            }
        }

        private void Masa_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs me && me.Button == MouseButtons.Right)
            {
                return;
            }

            Button btn = sender as Button;
            if (btn == null) return;

            int masaID = Convert.ToInt32(btn.Tag);

            // Rezerve masa kontrolü - rezerve durumundaki masaya sipariş eklenmesini engelleyin
            try
            {
                using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                {
                    con.Open();
                    string durumSorgu = "SELECT ISNULL(RTRIM(Durum), 'Boş') FROM Masalar WHERE MasaID = @mid";
                    using (SqlCommand cmdDurum = new SqlCommand(durumSorgu, con))
                    {
                        cmdDurum.Parameters.AddWithValue("@mid", masaID);
                        object res = cmdDurum.ExecuteScalar();
                        if (res != null)
                        {
                            string durumStr = res.ToString().Trim();
                            if (durumStr.Equals("Rezerve", StringComparison.OrdinalIgnoreCase) || durumStr.ToLower().Contains("rez"))
                            {
                                MessageBox.Show("Bu masa şu anda 'REZERVE' durumundadır!\n\nRezerve edilmiş bir masaya sipariş ekleyebilmek veya müşteri alabilmek için öncelikle 'Masaları Düzenle' panelinden masanın durumunu 'Boş' olarak güncellemeniz gerekmektedir.\n\n(Bu önlem, rezerve masalara yanlışlıkla müşteri alınıp rezervasyon sahibinin mağdur olmasını engellemek içindir.)", 
                                                "Rezerve Masa Uyarısı - Sipariş Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                }
            }
            catch { }

            // Sipariş formunu aç
            SiparişlerForm frm = new SiparişlerForm(masaID);
            frm.ShowDialog();

            // Kapatınca masaları yenile
            MasalariYukle();
        }
        #endregion

        #region siparişlerin mutfak data gridinde listelenmesi fonksiyonu SiparisListele()
        private void SiparisListele()
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                {
                    // Sadece 'Yeni Sipariş' ve 'Hazırlanıyor' olanları getiriyoruz.
                    // RTRIM ve LTRIM ile sağdaki/soldaki tüm boşlukları temizliyoruz.
                    string sorgu = @"
                    SELECT 
                    s.SiparisID, 
                    m.MasaNo, 
                    STRING_AGG(RTRIM(u.UrunAdi) + ' x' + CAST(sd.Adet AS VARCHAR), ' | ') AS Urunler, 
                    s.ToplamTutar, 
                    s.Tarih, 
                    RTRIM(s.Durum) AS Durum 
                    FROM Siparis s 
                    JOIN Masalar m ON s.MasaID = m.MasaID 
                    JOIN SiparisDetay sd ON s.SiparisID = sd.SiparisID
                    JOIN Urunler u ON sd.UrunID = u.UrunID
                    WHERE RTRIM(s.Durum) = 'Yeni Sipariş' OR RTRIM(s.Durum) = 'Hazırlanıyor'
                    GROUP BY s.SiparisID, m.MasaNo, s.ToplamTutar, s.Tarih, s.Durum
                    ORDER BY s.SiparisID ASC";

                    SqlDataAdapter da = new SqlDataAdapter(sorgu, baglanti);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    MtfkDataGrid.DataSource = dt;

                    // Tasarım düzenlemeleri
                    if (MtfkDataGrid.Columns["Urunler"] != null)
                    {
                        MtfkDataGrid.Columns["Urunler"].HeaderText = "SİPARİŞ İÇERİĞİ";
                        MtfkDataGrid.Columns["Urunler"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mutfak listesi yüklenirken bir hata oluştu: " + ex.Message);
            }
        }
        #endregion

        #region seçilen siparişinin durumunun güncellenmesi durum güncelle fonksiyonu DurumGuncelle()
        private void DurumGuncelle(string yeniDurum)
        {
            if (MtfkDataGrid.CurrentRow != null)
            {
                int seciliSiparisID = Convert.ToInt32(MtfkDataGrid.CurrentRow.Cells["SiparisID"].Value);

               
                using (SqlConnection baglanti = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
                {
                    string sorgu = "UPDATE Siparis SET Durum = @durum WHERE SiparisID = @id";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@durum", yeniDurum);
                    komut.Parameters.AddWithValue("@id", seciliSiparisID);

                    baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                }

                SiparisListele(); 
            }
            else
            {
                MessageBox.Show("Lütfen bir sipariş seçin!");
            }
        }
        #endregion  

        #region siparişdurumunun güncellendiği butonlar
        private void MtfHazirlaniyorBtn_Click(object sender, EventArgs e)
        {
            DurumGuncelle("Hazırlanıyor");
            MessageBox.Show("Sipariş işleme alındı, hazırlanıyor olarak işaretlendi.");
        }

        private void MtfHazirBtn_Click(object sender, EventArgs e)
        {
            DurumGuncelle("Hazır");
        }

        private void MtfiptalBtn_Click(object sender, EventArgs e)
        {
            DurumGuncelle("İptal");
        }
        #endregion

        #region Anlık Ciro & Sanal PDF Detaylı Rapor Oluşturucu
        private void AnlikCiroGuncelle()
        {
            using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT ISNULL(SUM(ToplamTutar), 0) FROM Siparis 
                        WHERE RTRIM(Durum) = 'Tamamlandı' AND CAST(Tarih AS DATE) = CAST(GETDATE() AS DATE)", con);
                    decimal bugunCiro = Convert.ToDecimal(cmd.ExecuteScalar());
                    if (labelTutar != null)
                    {
                        labelTutar.Text = "ANLIK CİRO (GÜNLÜK TOPLAM): " + bugunCiro.ToString("N2") + " ₺";
                    }
                }
                catch { }
            }
        }

        private void DetayliSanalPdfRaporuOlustur(string raporTuru, string raporBaslik, string sqlTarihKosulu)
        {
            AnlikCiroGuncelle();

            decimal toplamCiro = 0;
            int toplamSiparis = 0;
            decimal nakitCiro = 0;
            int nakitAdet = 0;
            decimal kartCiro = 0;
            int kartAdet = 0;

            StringBuilder html = new StringBuilder();
            StringBuilder txt = new StringBuilder();

            using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                try
                {
                    con.Open();

                    try
                    {
                        using (SqlCommand cmdAlter = new SqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Siparis' AND COLUMN_NAME = 'OdemeTuru') BEGIN ALTER TABLE Siparis ADD OdemeTuru NVARCHAR(50) NULL END", con))
                        {
                            cmdAlter.ExecuteNonQuery();
                        }
                    }
                    catch { }

                    // 1. Özet Bilgiler ve Tahsilat Türü Kırılımı
                    string queryOzet = $@"SELECT ISNULL(SUM(S.ToplamTutar), 0) AS Ciro, COUNT(S.SiparisID) AS Adet 
                                          FROM Siparis S WHERE RTRIM(S.Durum) = 'Tamamlandı' AND {sqlTarihKosulu}";
                    using (SqlCommand cmdOzet = new SqlCommand(queryOzet, con))
                    using (SqlDataReader drOzet = cmdOzet.ExecuteReader())
                    {
                        if (drOzet.Read())
                        {
                            toplamCiro = Convert.ToDecimal(drOzet["Ciro"]);
                            toplamSiparis = Convert.ToInt32(drOzet["Adet"]);
                        }
                    }

                    // Ödeme türlerine göre dağılım
                    string queryOdeme = $@"SELECT ISNULL(S.OdemeTuru, 'Nakit') AS Tur, ISNULL(SUM(S.ToplamTutar), 0) AS Ciro, COUNT(S.SiparisID) AS Adet 
                                           FROM Siparis S WHERE RTRIM(S.Durum) = 'Tamamlandı' AND {sqlTarihKosulu} GROUP BY ISNULL(S.OdemeTuru, 'Nakit')";
                    using (SqlCommand cmdOdeme = new SqlCommand(queryOdeme, con))
                    using (SqlDataReader drOdeme = cmdOdeme.ExecuteReader())
                    {
                        while (drOdeme.Read())
                        {
                            string tur = drOdeme["Tur"].ToString().Trim();
                            decimal c = Convert.ToDecimal(drOdeme["Ciro"]);
                            int a = Convert.ToInt32(drOdeme["Adet"]);
                            if (tur.Equals("Kredi Kartı", StringComparison.OrdinalIgnoreCase) || tur.IndexOf("Kart", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                kartCiro += c;
                                kartAdet += a;
                            }
                            else
                            {
                                nakitCiro += c;
                                nakitAdet += a;
                            }
                        }
                    }

                    decimal ortalamaSepet = toplamSiparis > 0 ? (toplamCiro / toplamSiparis) : 0;
                    decimal kdvTutar = toplamCiro * 0.10m; // %10 ortalama KDV
                    decimal nakitPay = toplamCiro > 0 ? Math.Round((nakitCiro / toplamCiro) * 100, 1) : 0;
                    decimal kartPay = toplamCiro > 0 ? Math.Round((kartCiro / toplamCiro) * 100, 1) : 0;

                    // HTML Başlık & Metada
                    html.Append(@"<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <title>" + raporBaslik + @"</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f4f6f9; color: #333; margin: 0; padding: 30px; }
        .container { max-width: 900px; margin: 0 auto; background: #fff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
        .header { display: flex; justify-content: space-between; align-items: center; border-bottom: 3px solid #e67e22; padding-bottom: 20px; margin-bottom: 30px; }
        .header h1 { margin: 0; color: #2c3e50; font-size: 24px; }
        .header .meta { font-size: 13px; color: #7f8c8d; text-align: right; }
        .kpi-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 15px; margin-bottom: 35px; }
        .kpi-card { background: #f8f9fa; border-left: 4px solid #e67e22; padding: 15px; border-radius: 4px; }
        .kpi-card .label { font-size: 12px; color: #7f8c8d; text-transform: uppercase; font-weight: 600; }
        .kpi-card .value { font-size: 20px; font-weight: bold; color: #2c3e50; margin-top: 5px; }
        h2 { color: #2c3e50; font-size: 18px; border-bottom: 2px solid #ecf0f1; padding-bottom: 8px; margin-top: 30px; }
        table { width: 100%; border-collapse: collapse; margin-top: 15px; }
        th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #e9ecef; font-size: 14px; }
        th { background-color: #2c3e50; color: #ffffff; font-weight: 600; }
        tr:nth-child(even) { background-color: #f8f9fa; }
        .progress-bar { background: #e9ecef; border-radius: 4px; overflow: hidden; height: 16px; width: 100%; }
        .progress-fill { background: #e67e22; height: 100%; }
        .footer { margin-top: 50px; padding-top: 20px; border-top: 1px solid #dee2e6; text-align: center; font-size: 12px; color: #adb5bd; }
        .btn-print { background: #e67e22; color: #fff; border: none; padding: 10px 20px; border-radius: 5px; cursor: pointer; font-weight: bold; font-size: 14px; }
        .btn-print:hover { background: #d35400; }
        @media print {
            .no-print { display: none !important; }
            body { background: #fff !important; padding: 0 !important; }
            .container { box-shadow: none !important; border: none !important; width: 100% !important; max-width: 100% !important; padding: 0 !important; }
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='no-print' style='text-align: right; margin-bottom: 20px;'>
            <button class='btn-print' onclick='window.print()'>🖨️ PDF Olarak İndir / Yazdır</button>
        </div>
        <div class='header'>
            <div>
                <h1>RESTORAN OTOMASYONU</h1>
                <div style='font-size: 16px; color: #e67e22; font-weight: bold; margin-top: 5px;'>" + raporBaslik + @"</div>
            </div>
            <div class='meta'>
                <strong>Rapor Türü:</strong> " + raporTuru + @"<br>
                <strong>Oluşturulma Tarihi:</strong> " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + @"<br>
                <strong>Durum:</strong> Onaylı (Tamamlananlar)
            </div>
        </div>
        <div class='kpi-grid'>
            <div class='kpi-card'>
                <div class='label'>TOPLAM CİRO</div>
                <div class='value'>" + toplamCiro.ToString("N2") + @" ₺</div>
            </div>
            <div class='kpi-card' style='border-left-color: #27ae60;'>
                <div class='label'>💵 NAKİT TAHSİLAT</div>
                <div class='value'>" + nakitCiro.ToString("N2") + @" ₺ <span style='font-size:12px; font-weight:normal; color:#7f8c8d;'>(" + nakitAdet + @" Adet)</span></div>
            </div>
            <div class='kpi-card' style='border-left-color: #2980b9;'>
                <div class='label'>💳 KREDİ KARTI TAHSİLAT</div>
                <div class='value'>" + kartCiro.ToString("N2") + @" ₺ <span style='font-size:12px; font-weight:normal; color:#7f8c8d;'>(" + kartAdet + @" Adet)</span></div>
            </div>
            <div class='kpi-card'>
                <div class='label'>TOPLAM SİPARİŞ</div>
                <div class='value'>" + toplamSiparis + @" Adet</div>
            </div>
            <div class='kpi-card'>
                <div class='label'>ORTALAMA SEPET</div>
                <div class='value'>" + ortalamaSepet.ToString("N2") + @" ₺</div>
            </div>
            <div class='kpi-card'>
                <div class='label'>TAHMİNİ KDV (%10)</div>
                <div class='value'>" + kdvTutar.ToString("N2") + @" ₺</div>
            </div>
        </div>");

                    txt.AppendLine("================================================================================");
                    txt.AppendLine("                   RESTORAN OTOMASYONU - " + raporBaslik.ToUpper());
                    txt.AppendLine("================================================================================");
                    txt.AppendLine("Rapor Türü   : " + raporTuru);
                    txt.AppendLine("Tarih / Saat : " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    txt.AppendLine("--------------------------------------------------------------------------------");
                    txt.AppendLine($"TOPLAM CİRO     : {toplamCiro:N2} TL");
                    txt.AppendLine($"💵 NAKİT        : {nakitCiro:N2} TL ({nakitAdet} Adet İşlem)");
                    txt.AppendLine($"💳 KREDİ KARTI  : {kartCiro:N2} TL ({kartAdet} Adet İşlem)");
                    txt.AppendLine($"TOPLAM SİPARİŞ  : {toplamSiparis} Adet");
                    txt.AppendLine($"ORTALAMA SEPET  : {ortalamaSepet:N2} TL");
                    txt.AppendLine($"TAHMİNİ KDV     : {kdvTutar:N2} TL");
                    txt.AppendLine("================================================================================\r\n");

                    // Ödeme Türü Kırılım Özeti Tablosu
                    html.Append($@"<h2>Ödeme Türü Dağılım Özeti (Tahsilat Raporu)</h2>
        <table>
            <thead>
                <tr>
                    <th>Ödeme Türü</th>
                    <th>İşlem Adedi</th>
                    <th>Tahsil Edilen Tutar</th>
                    <th>Cirodaki Payı</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><strong>💵 Nakit Ödemeler</strong></td>
                    <td>{nakitAdet} Adet</td>
                    <td>{nakitCiro:N2} ₺</td>
                    <td>
                        <div style='display:flex; align-items:center; gap:10px;'>
                            <div class='progress-bar'><div class='progress-fill' style='width: {nakitPay}%; background:#27ae60;'></div></div>
                            <span style='font-weight:600; font-size:12px;'>%{nakitPay}</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td><strong>💳 Kredi / Banka Kartı</strong></td>
                    <td>{kartAdet} Adet</td>
                    <td>{kartCiro:N2} ₺</td>
                    <td>
                        <div style='display:flex; align-items:center; gap:10px;'>
                            <div class='progress-bar'><div class='progress-fill' style='width: {kartPay}%; background:#2980b9;'></div></div>
                            <span style='font-weight:600; font-size:12px;'>%{kartPay}</span>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>");

                    txt.AppendLine("--- ÖDEME TÜRÜ BAZLI TAHSİLAT ÖZETİ ---");
                    txt.AppendLine(string.Format("{0,-20} | {1,-12} | {2,-15} | {3,-10}", "Ödeme Türü", "İşlem Adedi", "Tahsilat", "Pay (%)"));
                    txt.AppendLine("--------------------------------------------------------------------------------");
                    txt.AppendLine(string.Format("{0,-20} | {1,-12} | {2,-15:N2} | %{3,-10:F1}", "💵 Nakit", nakitAdet + " Adet", nakitCiro, nakitPay));
                    txt.AppendLine(string.Format("{0,-20} | {1,-12} | {2,-15:N2} | %{3,-10:F1}", "💳 Kredi Kartı", kartAdet + " Adet", kartCiro, kartPay));
                    txt.AppendLine("================================================================================\r\n");

                    // 2. Kategori Bazlı Satışlar
                    html.Append(@"<h2>Kategori Bazlı Satış Özetleri</h2>
        <table>
            <thead>
                <tr>
                    <th>Kategori Adı</th>
                    <th>Satış Adedi</th>
                    <th>Toplam Tutar</th>
                    <th>Cirodaki Payı</th>
                </tr>
            </thead>
            <tbody>");

                    txt.AppendLine("--- KATEGORİ BAZLI SATIŞ DAĞILIMI ---");
                    txt.AppendLine(string.Format("{0,-25} | {1,-12} | {2,-15} | {3,-10}", "Kategori Adı", "Satış Adedi", "Toplam Tutar", "Pay (%)"));
                    txt.AppendLine("--------------------------------------------------------------------------------");

                    string queryKategori = $@"SELECT 
                        ISNULL(K.KategoriAdi, 'Diğer') AS KategoriAdi, 
                        ISNULL(SUM(SD.Adet), 0) AS ToplamAdet, 
                        ISNULL(SUM(SD.Adet * SD.Fiyat), 0) AS ToplamTutar 
                        FROM SiparisDetay SD 
                        INNER JOIN Siparis S ON SD.SiparisID = S.SiparisID 
                        LEFT JOIN Urunler U ON SD.UrunID = U.UrunID 
                        LEFT JOIN Kategoriler K ON U.KategoriID = K.KategoriID 
                        WHERE RTRIM(S.Durum) = 'Tamamlandı' AND {sqlTarihKosulu} 
                        GROUP BY K.KategoriAdi 
                        ORDER BY ToplamTutar DESC";

                    using (SqlCommand cmdKategori = new SqlCommand(queryKategori, con))
                    using (SqlDataReader drKategori = cmdKategori.ExecuteReader())
                    {
                        bool veriVar = false;
                        while (drKategori.Read())
                        {
                            veriVar = true;
                            string katAdi = drKategori["KategoriAdi"].ToString();
                            int katAdet = Convert.ToInt32(drKategori["ToplamAdet"]);
                            decimal katTutar = Convert.ToDecimal(drKategori["ToplamTutar"]);
                            decimal yuzde = toplamCiro > 0 ? Math.Round((katTutar / toplamCiro) * 100, 1) : 0;

                            html.Append($@"<tr>
                    <td><strong>{katAdi}</strong></td>
                    <td>{katAdet} Adet</td>
                    <td>{katTutar:N2} ₺</td>
                    <td>
                        <div style='display:flex; align-items:center; gap:10px;'>
                            <div class='progress-bar'><div class='progress-fill' style='width: {yuzde}%;'></div></div>
                            <span style='font-weight:600; font-size:12px;'>%{yuzde}</span>
                        </div>
                    </td>
                </tr>");

                            txt.AppendLine(string.Format("{0,-25} | {1,-12} | {2,-15:N2} | %{3,-10:F1}", katAdi, katAdet + " Adet", katTutar, yuzde));
                        }
                        if (!veriVar)
                        {
                            html.Append("<tr><td colspan='4' style='text-align:center; color:#888;'>Bu dönem için kayıtlı satış bulunamadı.</td></tr>");
                            txt.AppendLine("Kayıtlı satış bulunamadı.");
                        }
                    }

                    html.Append("</tbody></table>");
                    txt.AppendLine("================================================================================\r\n");

                    // 3. En Çok Satılan Ürünler (Top 10)
                    html.Append(@"<h2>En Çok Satılan Ürünler (Top 10)</h2>
        <table>
            <thead>
                <tr>
                    <th>Sıra</th>
                    <th>Ürün Adı</th>
                    <th>Satış Adedi</th>
                    <th>Toplam Tutar</th>
                </tr>
            </thead>
            <tbody>");

                    txt.AppendLine("--- EN ÇOK SATILAN ÜRÜNLER (TOP 10) ---");
                    txt.AppendLine(string.Format("{0,-5} | {1,-30} | {2,-12} | {3,-15}", "Sıra", "Ürün Adı", "Satış Adedi", "Toplam Tutar"));
                    txt.AppendLine("--------------------------------------------------------------------------------");

                    string queryUrun = $@"SELECT TOP 10 
                        ISNULL(U.UrunAdi, 'Silinmiş Ürün') AS UrunAdi, 
                        ISNULL(SUM(SD.Adet), 0) AS ToplamAdet, 
                        ISNULL(SUM(SD.Adet * SD.Fiyat), 0) AS ToplamTutar 
                        FROM SiparisDetay SD 
                        INNER JOIN Siparis S ON SD.SiparisID = S.SiparisID 
                        LEFT JOIN Urunler U ON SD.UrunID = U.UrunID 
                        WHERE RTRIM(S.Durum) = 'Tamamlandı' AND {sqlTarihKosulu} 
                        GROUP BY U.UrunAdi 
                        ORDER BY ToplamAdet DESC";

                    using (SqlCommand cmdUrun = new SqlCommand(queryUrun, con))
                    using (SqlDataReader drUrun = cmdUrun.ExecuteReader())
                    {
                        int sira = 1;
                        bool urunVar = false;
                        while (drUrun.Read())
                        {
                            urunVar = true;
                            string uAdi = drUrun["UrunAdi"].ToString();
                            int uAdet = Convert.ToInt32(drUrun["ToplamAdet"]);
                            decimal uTutar = Convert.ToDecimal(drUrun["ToplamTutar"]);

                            html.Append($@"<tr>
                    <td><strong>#{sira}</strong></td>
                    <td>{uAdi}</td>
                    <td>{uAdet} Adet</td>
                    <td>{uTutar:N2} ₺</td>
                </tr>");

                            txt.AppendLine(string.Format("{0,-5} | {1,-30} | {2,-12} | {3,-15:N2}", "#" + sira, uAdi, uAdet + " Adet", uTutar));
                            sira++;
                        }
                        if (!urunVar)
                        {
                            html.Append("<tr><td colspan='4' style='text-align:center; color:#888;'>Satılan ürün bulunamadı.</td></tr>");
                            txt.AppendLine("Satılan ürün bulunamadı.");
                        }
                    }

                    html.Append(@"</tbody>
        </table>
        <div class='footer'>
            Restoran Otomasyon Sistemi &copy; " + DateTime.Now.Year + @" | Sanal PDF Rapor Modülü<br>
            Bu belge otomatik olarak oluşturulmuştur ve resmî/iç denetim için geçerlidir.
        </div>
    </div>
</body>
</html>");
                    txt.AppendLine("================================================================================");
                    txt.AppendLine("Rapor Sonu - " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));

                    // Kaydetme Diyaloğu
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Title = raporBaslik + " - Kaydet";
                        sfd.Filter = "Sanal PDF / HTML Raporu (*.html)|*.html|Metin Raporu (*.txt)|*.txt";
                        string dosyaAdi = raporTuru.Replace(" ", "_").Replace("/", "_").Replace("(", "").Replace(")", "") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm");
                        sfd.FileName = dosyaAdi;

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            if (sfd.FilterIndex == 2 || sfd.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                            {
                                File.WriteAllText(sfd.FileName, txt.ToString(), Encoding.UTF8);
                            }
                            else
                            {
                                File.WriteAllText(sfd.FileName, html.ToString(), Encoding.UTF8);
                            }

                            MessageBox.Show(raporTuru + " başarıyla oluşturuldu ve kaydedildi!\nDosya şimdi açılacak; açılan ekrandaki 'PDF Olarak İndir / Yazdır' butonuna basarak anında PDF indirebilirsiniz.", "Rapor Hazır", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Rapor oluşturulurken hata meydana geldi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region rapor kısmının butonları (X Raporu, Z Raporu / Aylık, Yıllık Rapor)
        private void button11_Click(object sender, EventArgs e)
        {
            // X Raporu: Gün İçi Anlık Durum
            DetayliSanalPdfRaporuOlustur("X Raporu (Günlük Anlık)", "X RAPORU - GÜNLÜK SATIŞ VE CİRO ÖZETİ", "CAST(S.Tarih AS DATE) = CAST(GETDATE() AS DATE)");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Z Raporu & Aylık Rapor: Gün Sonu & Dönem Kapama
            DetayliSanalPdfRaporuOlustur("Z Raporu (Aylık & Gün Sonu)", "Z RAPORU / AYLIK KAPAMA VE SATIŞ ANALİZİ", "MONTH(S.Tarih) = MONTH(GETDATE()) AND YEAR(S.Tarih) = YEAR(GETDATE())");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // Yıllık Rapor: Genel Performans
            DetayliSanalPdfRaporuOlustur("Yıllık Rapor (Genel Performans)", "YILLIK CİRO VE SATIŞ DAĞILIM RAPORU", "YEAR(S.Tarih) = YEAR(GETDATE())");
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            SiparisListele();
            if (RaporlarTabPage != null && tabControl1.SelectedTab == RaporlarTabPage)
            {
                AnlikCiroGuncelle();
            }
        }

        private void MtfkDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
