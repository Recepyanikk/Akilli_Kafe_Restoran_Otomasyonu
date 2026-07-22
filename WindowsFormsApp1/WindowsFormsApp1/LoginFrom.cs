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
    public partial class LoginFrom : Form
    {

        public LoginFrom()
        {
            InitializeComponent();
        }


        public string KullaniciPozisyonu { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text;
            string pass = textBox2.Text;

            using (SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                con.Open();

                try
                {
                    using (SqlCommand cmdAlter = new SqlCommand("ALTER TABLE Personel ALTER COLUMN SifreHash NVARCHAR(256)", con))
                    {
                        cmdAlter.ExecuteNonQuery();
                    }
                }
                catch { }

                SqlCommand cmd = new SqlCommand(
                    "SELECT PersonelID, Pozisyon, SifreHash FROM Personel WHERE KullaniciAdi=@u", con);
                cmd.Parameters.AddWithValue("@u", user);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        int personelID = Convert.ToInt32(dr["PersonelID"]);
                        string pozisyon = dr["Pozisyon"].ToString();
                        string dbSifre = dr["SifreHash"].ToString().Trim();

                        string hashedInput = SecurityHelper.HashPassword(pass);

                        if (string.Equals(dbSifre, hashedInput, StringComparison.OrdinalIgnoreCase))
                        {
                            KullaniciPozisyonu = pozisyon;
                            dr.Close();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                        else if (string.Equals(dbSifre, pass, StringComparison.Ordinal))
                        {
                            dr.Close();
                            using (SqlCommand cmdUpgrade = new SqlCommand("UPDATE Personel SET SifreHash=@newHash WHERE PersonelID=@id", con))
                            {
                                cmdUpgrade.Parameters.AddWithValue("@newHash", hashedInput);
                                cmdUpgrade.Parameters.AddWithValue("@id", personelID);
                                cmdUpgrade.ExecuteNonQuery();
                            }

                            KullaniciPozisyonu = pozisyon;
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                    }
                }

                MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
