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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtAd.Text == "" || txtKullaniciAdi.Text == "" || txtSifre.Text == "" || cmbPozisyon.SelectedIndex == -1)
            {
                MessageBox.Show("Tüm alanları doldurun");
                return;
            }

            using (SqlConnection con = new SqlConnection(
                @"Server=.\SQLEXPRESS;Database=Restorant;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Personel (Ad, KullaniciAdi, SifreHash, Pozisyon) VALUES (@a,@k,@s,@p)", con);

                cmd.Parameters.AddWithValue("@a", txtAd.Text);
                cmd.Parameters.AddWithValue("@k", txtKullaniciAdi.Text);
                cmd.Parameters.AddWithValue("@s", SecurityHelper.HashPassword(txtSifre.Text)); 
                cmd.Parameters.AddWithValue("@p", cmbPozisyon.SelectedItem.ToString());

                con.Open();
                try
                {
                    using (SqlCommand cmdAlter = new SqlCommand("ALTER TABLE Personel ALTER COLUMN SifreHash NVARCHAR(256)", con))
                    {
                        cmdAlter.ExecuteNonQuery();
                    }
                }
                catch { }
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Personel başarıyla kaydedildi");

            this.DialogResult = DialogResult.OK; 
            this.Close();
        }
    }
}
