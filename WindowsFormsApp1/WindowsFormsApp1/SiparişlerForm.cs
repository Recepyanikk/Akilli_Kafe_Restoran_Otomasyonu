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

namespace WindowsFormsApp1
{

    public partial class SiparişlerForm : Form
    {
        int _masaID;
        string connStr = @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True";

        public SiparişlerForm(int MasaID)
        {
            InitializeComponent();
            _masaID = MasaID;

        }

        #region formun yüklenmesi kısmında gerçekleyen olaylar  
        private void SiparişlerForm_Load_1(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string durumSorgu = "SELECT ISNULL(RTRIM(Durum), 'Boş') FROM Masalar WHERE MasaID = @mid";
                    using (SqlCommand cmdDurum = new SqlCommand(durumSorgu, con))
                    {
                        cmdDurum.Parameters.AddWithValue("@mid", _masaID);
                        object res = cmdDurum.ExecuteScalar();
                        if (res != null)
                        {
                            string durumStr = res.ToString().Trim();
                            if (durumStr.Equals("Rezerve", StringComparison.OrdinalIgnoreCase) || durumStr.ToLower().Contains("rez"))
                            {
                                MessageBox.Show("Bu masa şu anda 'REZERVE' durumundadır!\n\nRezerve edilmiş bir masaya sipariş ekleyebilmek veya müşteri alabilmek için öncelikle 'Masaları Düzenle' panelinden masanın durumunu 'Boş' olarak güncellemeniz gerekmektedir.\n\n(Bu önlem, rezerve masalara yanlışlıkla müşteri alınıp rezervasyon sahibinin mağdur olmasını engellemek içindir.)", 
                                                "Rezerve Masa Uyarısı - Sipariş Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.BeginInvoke(new Action(() => this.Close()));
                                return;
                            }
                        }
                    }
                }
            }
            catch { }

            dgvSiparisler.Columns.Clear();

            dgvSiparisler.Columns.Add("UrunAdi", "Ürün Adı");
            dgvSiparisler.Columns.Add("Adet", "Adet");
            dgvSiparisler.Columns.Add("BirimFiyat", "Birim Fiyat");
            dgvSiparisler.Columns.Add("Toplam", "Toplam");
            dgvSiparisler.Columns.Add("UrunID", "ID");
            dgvSiparisler.Columns["UrunID"].Visible = false;
            dgvSiparisler.Columns.Add("SiparisDurumu", "Durum");
            dgvSiparisler.Columns["SiparisDurumu"].Visible = false;
            dgvSiparisler.Columns.Add("SiparisID", "SiparisID");
            dgvSiparisler.Columns["SiparisID"].Visible = false;

            dgvSiparisler.Columns["UrunAdi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Alt satırdaki boşluğu kaldır
            dgvSiparisler.AllowUserToAddRows = false;
            MasanoLabel.Text = "Masa" + _masaID.ToString();
            KategoriYukle();
            MevcutSiparisleriGetir();
            flpKategoriler.AutoScroll = false;
            flpKategoriler.HorizontalScroll.Maximum = 0;
            flpKategoriler.AutoScroll = true;
            flpKategoriler.HorizontalScroll.Visible = false;
            Masalbl.Text = "Masa "+_masaID.ToString();
            ToplamHesapla();
            this.Resize += (s, ev) => FormYerlesimDuzenle();
            FormYerlesimDuzenle();
        }
        #endregion

        #region Dinamik Form ve Panel Yerleşim Düzeni (Responsive Layout)
        void FormYerlesimDuzenle()
        {
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0) return;

            int solGenislik = flpKategoriler != null ? flpKategoriler.Width : 234;
            int kalanGenislik = this.ClientSize.Width - solGenislik;
            int altBarYukseklik = 75; // Alt buton panellerinin (panel1 ve panel2) yüksekliği

            // Sağ taraftaki adisyon / sipariş listesi genişliği (kalan genişliğin %35'i veya minimum 420 px)
            int sagGenislik = (int)(kalanGenislik * 0.35);
            if (sagGenislik < 420) sagGenislik = 420;
            if (sagGenislik > 650) sagGenislik = 650;

            // Orta alan (Ürünler listesi) genişliği
            int ortaGenislik = kalanGenislik - sagGenislik;

            // 1. Orta panel (flpUrunler)
            if (flpUrunler != null)
            {
                flpUrunler.Location = new Point(solGenislik, 0);
                flpUrunler.Size = new Size(ortaGenislik, this.ClientSize.Height - altBarYukseklik);
            }

            // 2. Orta alt panel (panel1 -> Ödeme Al, İptal, Çıkış Yap)
            if (panel1 != null)
            {
                panel1.Location = new Point(solGenislik, this.ClientSize.Height - altBarYukseklik);
                panel1.Size = new Size(ortaGenislik, altBarYukseklik);

                // panel1 içindeki 3 butonun (SiparisOdemeAlBtn, SiparisUrunIptalBtn, button3) genişliğini dinamik yay
                int btnW = (ortaGenislik - 40) / 3;
                if (SiparisOdemeAlBtn != null)
                {
                    SiparisOdemeAlBtn.Location = new Point(10, (altBarYukseklik - SiparisOdemeAlBtn.Height) / 2);
                    SiparisOdemeAlBtn.Width = btnW;
                }
                if (SiparisUrunIptalBtn != null && SiparisOdemeAlBtn != null)
                {
                    SiparisUrunIptalBtn.Location = new Point(SiparisOdemeAlBtn.Right + 10, (altBarYukseklik - SiparisUrunIptalBtn.Height) / 2);
                    SiparisUrunIptalBtn.Width = btnW;
                }
                if (button3 != null && SiparisUrunIptalBtn != null)
                {
                    button3.Location = new Point(SiparisUrunIptalBtn.Right + 10, (altBarYukseklik - button3.Height) / 2);
                    button3.Width = btnW;
                }
            }

            // 3. Sağ panel (dgvSiparisler)
            if (dgvSiparisler != null)
            {
                dgvSiparisler.Location = new Point(solGenislik + ortaGenislik, 0);
                dgvSiparisler.Size = new Size(sagGenislik, this.ClientSize.Height - altBarYukseklik);
            }

            // 4. Sağ alt panel (panel2 -> Toplam Tutar ve Siparişi Onayla Butonu)
            if (panel2 != null)
            {
                panel2.Location = new Point(solGenislik + ortaGenislik, this.ClientSize.Height - altBarYukseklik);
                panel2.Size = new Size(sagGenislik, altBarYukseklik);

                if (SiparisOnaylaBtn != null)
                {
                    SiparisOnaylaBtn.Location = new Point(sagGenislik - SiparisOnaylaBtn.Width - 15, (altBarYukseklik - SiparisOnaylaBtn.Height) / 2);
                }
                if (Masalbl != null)
                {
                    Masalbl.Location = new Point(15, (altBarYukseklik - Masalbl.Height) / 2);
                }
                if (lblToplamTutar != null && Masalbl != null)
                {
                    lblToplamTutar.Location = new Point(Masalbl.Right + 20, (altBarYukseklik - lblToplamTutar.Height) / 2);
                }
            }
        }
        #endregion

        #region kategori menüsüne kategorilerin yüklenmesi sol panel KategoriYukle()
        void KategoriYukle()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT KategoriID, KategoriAdi FROM Kategoriler", con);
                SqlDataReader dr = cmd.ExecuteReader();

                flpKategoriler.Controls.Clear();

                while (dr.Read())
                {
                    Button btn = new Button
                    {
                        Width = 140,
                        Height = 65,
                        BackColor = Color.FromArgb(45, 45, 48),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Text = dr["KategoriAdi"].ToString(),
                        Tag = dr["KategoriID"],
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        Margin = new Padding(5),
                        Cursor = Cursors.Hand
                    };
                    btn.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 80);

                    // Butona tıklandığında altındaki ürünleri getir
                    btn.Click += (s, ev) =>
                    {
                        Button b = (Button)s;
                        UrunleriYukle(Convert.ToInt32(b.Tag));
                    };

                    flpKategoriler.Controls.Add(btn);
                }
            }
        }
        #endregion

        #region ürünlerin yüklenmesi sağ panel
        void UrunleriYukle(int kategoriID)
        {
            flpUrunler.Controls.Clear();
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string query = "SELECT UrunID, UrunAdi, Fiyat, ResimYolu FROM Urunler WHERE KategoriID = @kID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@kID", kategoriID);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string uAdi = dr["UrunAdi"].ToString().Trim();
                        decimal uFiyat = Convert.ToDecimal(dr["Fiyat"]);
                        int uID = Convert.ToInt32(dr["UrunID"]);

                        // Şık Modern Kart Yapısı (Ana Panel)
                        Panel pnlCard = new Panel
                        {
                            Width = 175,
                            Height = 165,
                            BackColor = Color.FromArgb(40, 40, 44),
                            Margin = new Padding(10),
                            Cursor = Cursors.Hand
                        };

                        // Üst Alan: Yemek Fotoğrafı (Yazı kesinlikle üstüne gelmez, net görünür)
                        PictureBox pb = new PictureBox
                        {
                            Dock = DockStyle.Top,
                            Height = 105,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            BackColor = Color.FromArgb(50, 50, 55),
                            Cursor = Cursors.Hand
                        };

                        try
                        {
                            if (dr["ResimYolu"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["ResimYolu"].ToString()))
                            {
                                string resimYolu = dr["ResimYolu"].ToString().Trim();
                                if (System.IO.File.Exists(resimYolu))
                                {
                                    pb.Image = Image.FromFile(resimYolu);
                                }
                            }
                        }
                        catch { }

                        // Alt Alan: Ürün Adı ve Altın Sarısı Fiyat Şeridi
                        Panel pnlInfo = new Panel
                        {
                            Dock = DockStyle.Bottom,
                            Height = 60,
                            BackColor = Color.FromArgb(32, 32, 36),
                            Cursor = Cursors.Hand
                        };

                        Label lblFiyat = new Label
                        {
                            Dock = DockStyle.Bottom,
                            Height = 26,
                            Text = $"{uFiyat:N2} ₺",
                            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                            ForeColor = Color.FromArgb(255, 180, 0), // Modern altın/turuncu şık fiyat rengi
                            TextAlign = ContentAlignment.MiddleCenter,
                            BackColor = Color.Transparent,
                            Cursor = Cursors.Hand
                        };

                        Label lblAdi = new Label
                        {
                            Dock = DockStyle.Fill,
                            Text = uAdi,
                            Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold),
                            ForeColor = Color.White,
                            TextAlign = ContentAlignment.MiddleCenter,
                            AutoEllipsis = true,
                            BackColor = Color.Transparent,
                            Cursor = Cursors.Hand
                        };

                        pnlInfo.Controls.Add(lblAdi);
                        pnlInfo.Controls.Add(lblFiyat);

                        pnlCard.Controls.Add(pnlInfo);
                        pnlCard.Controls.Add(pb);

                        // Kartın herhangi bir yerine tıklandığında siparişe ekleme olayı
                        Action<object, EventArgs> urunSecOlayi = (s, ev) =>
                        {
                            bool urunVarMi = false;

                            foreach (DataGridViewRow row in dgvSiparisler.Rows)
                            {
                                int sid = 0;
                                if (row.Cells["SiparisID"].Value != null && int.TryParse(row.Cells["SiparisID"].Value.ToString(), out int parsedSid))
                                {
                                    sid = parsedSid;
                                }

                                // Sadece yeni eklenmekte olan (SiparisID == 0) satırlarda artırım yap
                                if (sid == 0 && row.Cells["UrunID"].Value != null && Convert.ToInt32(row.Cells["UrunID"].Value) == uID)
                                {
                                    int adet = Convert.ToInt32(row.Cells["Adet"].Value);
                                    row.Cells["Adet"].Value = adet + 1;
                                    row.Cells["Toplam"].Value = (adet + 1) * uFiyat;
                                    urunVarMi = true;
                                    break;
                                }
                            }

                            if (!urunVarMi)
                            {
                                dgvSiparisler.Rows.Add(uAdi, 1, uFiyat, uFiyat, uID, "Yeni Sipariş", 0);
                            }

                            ToplamHesapla();
                        };

                        pnlCard.Click += new EventHandler(urunSecOlayi);
                        pb.Click += new EventHandler(urunSecOlayi);
                        pnlInfo.Click += new EventHandler(urunSecOlayi);
                        lblAdi.Click += new EventHandler(urunSecOlayi);
                        lblFiyat.Click += new EventHandler(urunSecOlayi);

                        // Fare ile kartın üstüne gelince 3 Boyutlu Büyüme & Parlama (Hover Pop Effect)
                        Action<object, EventArgs> hoverEnter = (s, ev) =>
                        {
                            if (pnlCard.Width != 181)
                            {
                                pnlCard.Width = 181;
                                pnlCard.Height = 171;
                                pnlCard.Margin = new Padding(7);
                                pb.Height = 109;
                                pnlCard.BackColor = Color.FromArgb(75, 75, 85);
                                pnlInfo.BackColor = Color.FromArgb(45, 45, 52);
                                lblFiyat.Font = new Font("Segoe UI", 11.2f, FontStyle.Bold);
                            }
                        };

                        Action<object, EventArgs> hoverLeave = (s, ev) =>
                        {
                            Point pt = pnlCard.PointToClient(Cursor.Position);
                            if (!pnlCard.ClientRectangle.Contains(pt))
                            {
                                pnlCard.Width = 175;
                                pnlCard.Height = 165;
                                pnlCard.Margin = new Padding(10);
                                pb.Height = 105;
                                pnlCard.BackColor = Color.FromArgb(40, 40, 44);
                                pnlInfo.BackColor = Color.FromArgb(32, 32, 36);
                                lblFiyat.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                            }
                        };

                        // Tıklama esnasında fiziksel 3D buton basılma hissi (MouseDown & MouseUp)
                        MouseEventHandler mouseDown = (s, ev) =>
                        {
                            pnlCard.Width = 177;
                            pnlCard.Height = 167;
                            pnlCard.Margin = new Padding(9);
                            pnlCard.BackColor = Color.FromArgb(30, 30, 35);
                        };

                        MouseEventHandler mouseUp = (s, ev) =>
                        {
                            pnlCard.Width = 181;
                            pnlCard.Height = 171;
                            pnlCard.Margin = new Padding(7);
                            pnlCard.BackColor = Color.FromArgb(75, 75, 85);
                        };

                        pnlCard.MouseEnter += new EventHandler(hoverEnter);
                        pb.MouseEnter += new EventHandler(hoverEnter);
                        pnlInfo.MouseEnter += new EventHandler(hoverEnter);
                        lblAdi.MouseEnter += new EventHandler(hoverEnter);
                        lblFiyat.MouseEnter += new EventHandler(hoverEnter);

                        pnlCard.MouseLeave += new EventHandler(hoverLeave);
                        pb.MouseLeave += new EventHandler(hoverLeave);
                        pnlInfo.MouseLeave += new EventHandler(hoverLeave);
                        lblAdi.MouseLeave += new EventHandler(hoverLeave);
                        lblFiyat.MouseLeave += new EventHandler(hoverLeave);

                        pnlCard.MouseDown += mouseDown;
                        pb.MouseDown += mouseDown;
                        pnlInfo.MouseDown += mouseDown;
                        lblAdi.MouseDown += mouseDown;
                        lblFiyat.MouseDown += mouseDown;

                        pnlCard.MouseUp += mouseUp;
                        pb.MouseUp += mouseUp;
                        pnlInfo.MouseUp += mouseUp;
                        lblAdi.MouseUp += mouseUp;
                        lblFiyat.MouseUp += mouseUp;

                        flpUrunler.Controls.Add(pnlCard);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün Yükleme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region sipariş kısmındaki toplamın hesabı ToplamHesapla()
        void ToplamHesapla()
        {
            decimal toplam = 0;
            foreach (DataGridViewRow row in dgvSiparisler.Rows)
            {
                if (row.Cells["Toplam"].Value != null)
                {
                    toplam += Convert.ToDecimal(row.Cells["Toplam"].Value);
                }
            }
            lblToplamTutar.Text = " Toplam: " + toplam.ToString("N2") + " ₺";
        }
        #endregion
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        #region Mevcut Siparişlerin Listelenmesi
        void MevcutSiparisleriGetir()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                
                string query = @"
                    SELECT u.UrunAdi, sd.Adet, sd.Fiyat, (sd.Adet * sd.Fiyat) as Toplam, u.UrunID, RTRIM(s.Durum) as Durum, s.SiparisID
                    FROM Siparis s
                    JOIN SiparisDetay sd ON s.SiparisID = sd.SiparisID
                    JOIN Urunler u ON sd.UrunID = u.UrunID
                    WHERE s.MasaID = @masaID 
                    AND RTRIM(s.Durum) IN ('Yeni Sipariş', 'Hazırlanıyor', 'Hazır')
                    ORDER BY s.SiparisID ASC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@masaID", _masaID);
                SqlDataReader dr = cmd.ExecuteReader();

                dgvSiparisler.Rows.Clear(); // Mükerrer kayıt olmaması için tabloyu temizleyin
                while (dr.Read())
                {
                    string sDurum = dr["Durum"].ToString().Trim();
                    string uAdi = dr["UrunAdi"].ToString().Trim();
                    int sid = Convert.ToInt32(dr["SiparisID"]);

                    if (sDurum == "Hazırlanıyor") uAdi += " [Hazırlanıyor]";
                    else if (sDurum == "Hazır") uAdi += " [Hazır]";

                    int rowIndex = dgvSiparisler.Rows.Add(
                        uAdi,
                        dr["Adet"],
                        dr["Fiyat"],
                        dr["Toplam"],
                        dr["UrunID"],
                        sDurum,
                        sid
                    );

                    dgvSiparisler.Rows[rowIndex].ReadOnly = true;
                    if (sDurum == "Hazırlanıyor" || sDurum == "Hazır")
                    {
                        dgvSiparisler.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Orange;
                    }
                    else
                    {
                        dgvSiparisler.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.LightBlue;
                    }
                }
            }
            ToplamHesapla();
        }
        #endregion

        #region Siparişi Onaylama ve Kaydetme Transaction (İşlem Grubu)
        private void SiparisOnaylaBtn_Click(object sender, EventArgs e)
        {
            if (dgvSiparisler.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen önce ürün ekleyin!");
                return;
            }

            try
            {
                using (SqlConnection conCheck = new SqlConnection(connStr))
                {
                    conCheck.Open();
                    string durumSorgu = "SELECT ISNULL(RTRIM(Durum), 'Boş') FROM Masalar WHERE MasaID = @mid";
                    using (SqlCommand cmdDurum = new SqlCommand(durumSorgu, conCheck))
                    {
                        cmdDurum.Parameters.AddWithValue("@mid", _masaID);
                        object res = cmdDurum.ExecuteScalar();
                        if (res != null && (res.ToString().Trim().Equals("Rezerve", StringComparison.OrdinalIgnoreCase) || res.ToString().Trim().ToLower().Contains("rez")))
                        {
                            MessageBox.Show("Bu masa 'REZERVE' durumundadır! Lütfen önce masanın durumunu 'Boş' olarak güncelleyin.", "Rezerve Masa Engeli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }
            catch { }

            // Sadece yeni eklenen satırları bul (SiparisID == 0 veya null olanlar)
            List<DataGridViewRow> yeniSatirlar = new List<DataGridViewRow>();
            decimal yeniToplam = 0;

            foreach (DataGridViewRow row in dgvSiparisler.Rows)
            {
                int sid = 0;
                if (row.Cells["SiparisID"].Value != null && int.TryParse(row.Cells["SiparisID"].Value.ToString(), out int pSid))
                {
                    sid = pSid;
                }

                if (sid == 0)
                {
                    yeniSatirlar.Add(row);
                    if (row.Cells["Toplam"].Value != null)
                        yeniToplam += Convert.ToDecimal(row.Cells["Toplam"].Value);
                }
            }

            if (yeniSatirlar.Count == 0)
            {
                MessageBox.Show("Masada yeni eklenmiş bir ürün bulunmamaktadır. Mevcut siparişler zaten mutfağa iletilmiştir.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // HANGİ MASA VEYA DURUM OLURSA OLSUN HER ZAMAN YENİ VE BAĞIMSIZ BİR SİPARİŞ SATIRI AÇ (HER ZAMAN MUTFAK LİSTESİNİN BİR ALTINA EKLENSİN)
                    string querySiparis = @"INSERT INTO Siparis (MasaID, PersonelID, ToplamTutar, Tarih, Durum) 
                                           VALUES (@masaID, @personelID, @toplam, @tarih, 'Yeni Sipariş');
                                           SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdInsert = new SqlCommand(querySiparis, con, transaction);
                    cmdInsert.Parameters.AddWithValue("@masaID", _masaID);
                    cmdInsert.Parameters.AddWithValue("@personelID", 1);
                    cmdInsert.Parameters.AddWithValue("@toplam", yeniToplam);
                    cmdInsert.Parameters.AddWithValue("@tarih", DateTime.Now);

                    int yeniSiparisID = Convert.ToInt32(cmdInsert.ExecuteScalar());

                    foreach (DataGridViewRow row in yeniSatirlar)
                    {
                        string queryDetay = @"INSERT INTO SiparisDetay (SiparisID, UrunID, Adet, Fiyat) 
                                             VALUES (@siparisID, @urunID, @adet, @fiyat)";

                        SqlCommand cmdDetay = new SqlCommand(queryDetay, con, transaction);
                        cmdDetay.Parameters.AddWithValue("@siparisID", yeniSiparisID);
                        cmdDetay.Parameters.AddWithValue("@urunID", row.Cells["UrunID"].Value);
                        cmdDetay.Parameters.AddWithValue("@adet", row.Cells["Adet"].Value);
                        cmdDetay.Parameters.AddWithValue("@fiyat", row.Cells["BirimFiyat"].Value);
                        cmdDetay.ExecuteNonQuery();
                    }

                    // Masanın genel toplamını (tüm aktif sipariş biletlerinin toplamı) güncelle
                    SqlCommand cmdGenelToplam = new SqlCommand("SELECT ISNULL(SUM(ToplamTutar), 0) FROM Siparis WHERE MasaID = @masaID AND RTRIM(Durum) NOT IN ('Tamamlandı', 'İptal')", con, transaction);
                    cmdGenelToplam.Parameters.AddWithValue("@masaID", _masaID);
                    decimal genelToplam = Convert.ToDecimal(cmdGenelToplam.ExecuteScalar());

                    string queryMasa = "UPDATE Masalar SET Durum = 'Dolu', HesapTutari = @toplam WHERE MasaID = @masaID";
                    SqlCommand cmdMasa = new SqlCommand(queryMasa, con, transaction);
                    cmdMasa.Parameters.AddWithValue("@toplam", genelToplam);
                    cmdMasa.Parameters.AddWithValue("@masaID", _masaID);
                    cmdMasa.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Yeni siparişler mutfağa başarıyla iletildi ve listenin en altına eklendi.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
        }
        #endregion

        #region Restoran Masası Güvenli Ödeme Fonksiyonu ve Ödeme Türü Seçimi
        private string OdemeTuruSecimiAl(decimal toplamTutar)
        {
            using (Form form = new Form())
            {
                form.Text = "Ödeme Türü Seçimi";
                form.Size = new Size(420, 260);
                form.StartPosition = FormStartPosition.CenterScreen;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.FromArgb(40, 40, 40);

                Label lbl = new Label();
                lbl.Text = $"Masa {_masaID} - Ödenecek Toplam Tutar: {toplamTutar:N2} ₺\n\nLütfen alınan tahsilat türünü seçiniz:";
                lbl.ForeColor = Color.White;
                lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lbl.AutoSize = false;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Dock = DockStyle.Top;
                lbl.Height = 80;
                form.Controls.Add(lbl);

                Button btnNakit = new Button();
                btnNakit.Text = "💵 NAKİT ÖDEME";
                btnNakit.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                btnNakit.BackColor = Color.FromArgb(39, 174, 96);
                btnNakit.ForeColor = Color.White;
                btnNakit.FlatStyle = FlatStyle.Flat;
                btnNakit.Size = new Size(170, 60);
                btnNakit.Location = new Point(25, 95);
                btnNakit.DialogResult = DialogResult.Yes;
                form.Controls.Add(btnNakit);

                Button btnKart = new Button();
                btnKart.Text = "💳 KREDİ KARTI";
                btnKart.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                btnKart.BackColor = Color.FromArgb(41, 128, 185);
                btnKart.ForeColor = Color.White;
                btnKart.FlatStyle = FlatStyle.Flat;
                btnKart.Size = new Size(170, 60);
                btnKart.Location = new Point(210, 95);
                btnKart.DialogResult = DialogResult.No;
                form.Controls.Add(btnKart);

                Button btnIptal = new Button();
                btnIptal.Text = "❌ İptal / Vazgeç";
                btnIptal.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                btnIptal.BackColor = Color.FromArgb(192, 57, 43);
                btnIptal.ForeColor = Color.White;
                btnIptal.FlatStyle = FlatStyle.Flat;
                btnIptal.Size = new Size(355, 35);
                btnIptal.Location = new Point(25, 170);
                btnIptal.DialogResult = DialogResult.Cancel;
                form.Controls.Add(btnIptal);

                DialogResult res = form.ShowDialog();
                if (res == DialogResult.Yes) return "Nakit";
                if (res == DialogResult.No) return "Kredi Kartı";
                return null;
            }
        }

        private void SiparisOdemeAlBtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Veritabanında OdemeTuru sütunu yoksa otomatik ekle
                try
                {
                    using (SqlCommand cmdAlter = new SqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Siparis' AND COLUMN_NAME = 'OdemeTuru') BEGIN ALTER TABLE Siparis ADD OdemeTuru NVARCHAR(50) NULL END", con))
                    {
                        cmdAlter.ExecuteNonQuery();
                    }
                }
                catch { }

                string kontrolSorgu = @"SELECT COUNT(*) FROM Siparis 
                               WHERE MasaID = @masaID 
                               AND RTRIM(Durum) IN ('Yeni Sipariş', 'Hazırlanıyor')";

                SqlCommand cmdKontrol = new SqlCommand(kontrolSorgu, con);
                cmdKontrol.Parameters.AddWithValue("@masaID", _masaID);

                int hazirlananSayisi = (int)cmdKontrol.ExecuteScalar();

                if (hazirlananSayisi > 0)
                {
                    MessageBox.Show($"Bu masada henüz hazırlanmakta olan {hazirlananSayisi} adet sipariş grubu var.\n\n" +
                                    "Ödeme alabilmek için mutfağın tüm ürünleri 'Hazır' olarak işaretlemesi gerekir!",
                                    "Ödeme Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; 
                }

                SqlCommand cmdToplam = new SqlCommand("SELECT ISNULL(SUM(ToplamTutar), 0) FROM Siparis WHERE MasaID = @masaID AND RTRIM(Durum) NOT IN ('Tamamlandı', 'İptal')", con);
                cmdToplam.Parameters.AddWithValue("@masaID", _masaID);
                decimal odenecekTutar = Convert.ToDecimal(cmdToplam.ExecuteScalar());

                string odemeTuru = OdemeTuruSecimiAl(odenecekTutar);
                if (string.IsNullOrEmpty(odemeTuru))
                {
                    return; // İptal / Vazgeç basıldı
                }

                SqlTransaction tr = con.BeginTransaction();
                try
                {
                    string querySiparis = "UPDATE Siparis SET Durum = 'Tamamlandı', OdemeTuru = @odemeTuru WHERE MasaID = @masaID AND RTRIM(Durum) NOT IN ('Tamamlandı', 'İptal')";
                    SqlCommand cmdSiparis = new SqlCommand(querySiparis, con, tr);
                    cmdSiparis.Parameters.AddWithValue("@odemeTuru", odemeTuru);
                    cmdSiparis.Parameters.AddWithValue("@masaID", _masaID);
                    cmdSiparis.ExecuteNonQuery();

                    string queryMasa = "UPDATE Masalar SET Durum = 'Boş', HesapTutari = 0 WHERE MasaID = @masaID";
                    SqlCommand cmdMasa = new SqlCommand(queryMasa, con, tr);
                    cmdMasa.Parameters.AddWithValue("@masaID", _masaID);
                    cmdMasa.ExecuteNonQuery();

                    tr.Commit();
                    MessageBox.Show($"Masa hesabı başarıyla kapatıldı.\n\nAlınan Ödeme Türü: {odemeTuru}\nTahsil Edilen Tutar: {odenecekTutar:N2} ₺", "Ödeme Alındı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
        }
        #endregion

        #region sipariş iptali butonu
        private void SiparisUrunIptalBtn_Click(object sender, EventArgs e)
        {
            if (dgvSiparisler.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvSiparisler.SelectedRows)
                {
                    string sDurum = row.Cells["SiparisDurumu"]?.Value?.ToString();
                    if (sDurum == "Hazırlanıyor" || sDurum == "Hazır")
                    {
                        MessageBox.Show("Mutfakta hazırlanmakta olan veya hazır durumundaki ürünleri doğrudan masadan kaldıramazsınız! İptal için mutfak veya yönetici onayı gereklidir.", "İptal Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                DialogResult sor = MessageBox.Show("Seçili ürünü iptal etmek istiyor musunuz?", "Ürün İptali", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (sor == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dgvSiparisler.SelectedRows)
                    {
                        int sid = 0;
                        if (row.Cells["SiparisID"].Value != null && int.TryParse(row.Cells["SiparisID"].Value.ToString(), out int pSid))
                        {
                            sid = pSid;
                        }

                        // Eğer veritabanında kayıtlı bir "Yeni Sipariş" satırı ise veritabanından da sil
                        if (sid > 0 && row.Cells["UrunID"].Value != null)
                        {
                            int urunId = Convert.ToInt32(row.Cells["UrunID"].Value);
                            try
                            {
                                using (SqlConnection con = new SqlConnection(connStr))
                                {
                                    con.Open();
                                    using (SqlCommand cmdDel = new SqlCommand("DELETE FROM SiparisDetay WHERE SiparisID = @sid AND UrunID = @uid", con))
                                    {
                                        cmdDel.Parameters.AddWithValue("@sid", sid);
                                        cmdDel.Parameters.AddWithValue("@uid", urunId);
                                        cmdDel.ExecuteNonQuery();
                                    }

                                    using (SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM SiparisDetay WHERE SiparisID = @sid", con))
                                    {
                                        cmdCheck.Parameters.AddWithValue("@sid", sid);
                                        int detaySayisi = (int)cmdCheck.ExecuteScalar();
                                        if (detaySayisi == 0)
                                        {
                                            using (SqlCommand cmdDelSip = new SqlCommand("DELETE FROM Siparis WHERE SiparisID = @sid", con))
                                            {
                                                cmdDelSip.Parameters.AddWithValue("@sid", sid);
                                                cmdDelSip.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            using (SqlCommand cmdUpdSip = new SqlCommand("UPDATE Siparis SET ToplamTutar = (SELECT ISNULL(SUM(Adet*Fiyat),0) FROM SiparisDetay WHERE SiparisID = @sid) WHERE SiparisID = @sid", con))
                                            {
                                                cmdUpdSip.Parameters.AddWithValue("@sid", sid);
                                                cmdUpdSip.ExecuteNonQuery();
                                            }
                                        }
                                    }

                                    using (SqlCommand cmdUpdMasa = new SqlCommand("UPDATE Masalar SET HesapTutari = (SELECT ISNULL(SUM(ToplamTutar), 0) FROM Siparis WHERE MasaID = @mid AND RTRIM(Durum) NOT IN ('Tamamlandı', 'İptal')) WHERE MasaID = @mid", con))
                                    {
                                        cmdUpdMasa.Parameters.AddWithValue("@mid", _masaID);
                                        cmdUpdMasa.ExecuteNonQuery();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Veritabanından silinirken hata: " + ex.Message);
                            }
                        }

                        dgvSiparisler.Rows.Remove(row);
                    }

                    ToplamHesapla();
                    MessageBox.Show("Seçili ürün kaldırıldı.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen iptal etmek istediğiniz ürünü tablodan seçin.");
            }
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
