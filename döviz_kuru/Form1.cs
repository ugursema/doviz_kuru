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
using System.Xml;

namespace döviz_kuru
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labels();
            string bugun = "https://www.tcmb.gov.tr/kurlar/today.xml";
            var xmldosya = new XmlDocument();

            xmldosya.Load(bugun);
            string dolaralis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteBuying").InnerXml;
            lblDolarAlis.Text = dolaralis;
            string dolarsatis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling").InnerXml;
            lblDolarSatis.Text = dolarsatis;
            string euroalis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteBuying").InnerXml;
            lblEuroAlis.Text = euroalis;
            string eurosatis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteSelling").InnerXml;
            lblEuroSatis.Text = eurosatis;
        }

        private void btnDolarAlis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarAlis.Text;
        }

        private void btnDolarSatis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarSatis.Text;
        }

        private void btnEuroAlis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroAlis.Text;
        }

        private void btnEuroSatis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroSatis.Text;
        }

        private void txtKur_TextChanged(object sender, EventArgs e)
        {
            //. virgüle çeviriyoruz kur dorğu alsın diye
            txtKur.Text = txtKur.Text.Replace(".", ",");
        }

        private void txtSatisYap_Click(object sender, EventArgs e)
        {
            double kur, miktar, tutar;
            kur = Convert.ToDouble(txtKur.Text);
            miktar = Convert.ToDouble(txtMiktar.Text);
            tutar = kur * miktar;
            txtTutar.Text = tutar.ToString();
        }

        private void btnsatis2_Click(object sender, EventArgs e)
        {
            double kur = Convert.ToDouble(txtKur.Text);
            int miktar = Convert.ToInt32(txtMiktar.Text);
            int tutar = Convert.ToInt32(miktar / kur);
            txtTutar.Text = tutar.ToString();

            double kalan;
            kalan = miktar % kur;
            txtKalan.Text = kalan.ToString();


        }

        private void btnUsdAl_Click(object sender, EventArgs e)
        {
            // 1️⃣ OKUMA
            decimal tl = Convert.ToDecimal(txtMiktar.Text);
            decimal kur = Convert.ToDecimal(txtKur.Text);

            // 2️⃣ HESAP
            decimal usd = Math.Floor((tl / kur) * 100) / 100; // 2 ondalık
            decimal harcananTL = usd * kur;

            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-OD8JBL6\\SQLEXPRESS;Initial Catalog=SU_Project;Integrated Security=True"))
            {
                con.Open();
                SqlTransaction tr = con.BeginTransaction();

                try
                {
                    // 3️⃣ USD ALIŞ KAYDI
                    SqlCommand cmdUsd = new SqlCommand(@"
                INSERT INTO Tbl_DovizIslemleri
                (ParaBirimi, IslemTuru, Miktar, Kur, KarsilikTL)
                VALUES
                ('USD', 'Alis', @usd, @kur, @tl)
            ", con, tr);

                    cmdUsd.Parameters.AddWithValue("@usd", usd);
                    cmdUsd.Parameters.AddWithValue("@kur", kur);
                    cmdUsd.Parameters.AddWithValue("@tl", harcananTL);
                    cmdUsd.ExecuteNonQuery();

                    // 4️⃣ TL SATIŞ KAYDI
                    SqlCommand cmdTl = new SqlCommand(@"
                INSERT INTO Tbl_DovizIslemleri
                (ParaBirimi, IslemTuru, Miktar, Kur, KarsilikTL)
                VALUES
                ('TL', 'Satis', @tl, 1, @tl)
            ", con, tr);

                    cmdTl.Parameters.AddWithValue("@tl", harcananTL);
                    cmdTl.ExecuteNonQuery();

                    // 5️⃣ ONAY
                    tr.Commit();

                    MessageBox.Show("USD alış işlemi başarılı");
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void bbtnEurAl_Click(object sender, EventArgs e)
        {
            decimal tl = Convert.ToDecimal(txtMiktar.Text);
            decimal kur = Convert.ToDecimal(txtKur.Text);
            decimal eur = Math.Floor((tl / kur) * 100) / 100;
            decimal harcananTL = eur * kur;

            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-OD8JBL6\\SQLEXPRESS;Initial Catalog=SU_Project;Integrated Security=True"))
            {
                con.Open(); //baglantı ac
                            // Bu işlemde 2 kayıt olacağı için transaction başlatıyoruz

                SqlTransaction tr = con.BeginTransaction();

                try
                {
                    SqlCommand commandEUR = new SqlCommand(@"INSERT INTO Tbl_DovizIslemleri (ParaBirimi, IslemTuru,Miktar, Kur,KarsilikTL) VALUES   ('EUR', 'Alis', @eur, @kur, @tl) ", con, tr);
                    commandEUR.Parameters.AddWithValue("@eur", eur);
                    commandEUR.Parameters.AddWithValue("@kur",kur);
                    commandEUR.Parameters.AddWithValue("@tl", harcananTL);
                    commandEUR.ExecuteNonQuery();

               //     'TL', 'Satis', @tl, 1, @tl)
                    SqlCommand commandTL = new SqlCommand(@"INSERT INTO Tbl_DovizIslemleri (ParaBirimi, IslemTuru, Miktar, Kur, KarsilikTL) VALUES ('TL', 'Satis', @tl, 1, @tl) ", con, tr);

                    commandTL.Parameters.AddWithValue("@tl", harcananTL);
                    commandTL.ExecuteNonQuery();
                    //   ONAY
                    tr.Commit();

                    MessageBox.Show("USD alış işlemi başarılı");
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    MessageBox.Show(ex.Message);
                }
             



            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void lbldolar_Click(object sender, EventArgs e)
        {
           
        }
        private void labels()
        {
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-OD8JBL6\\SQLEXPRESS;Initial Catalog=SU_Project;Integrated Security=True"))
            {
                con.Open();
                try
                {
                    SqlCommand komut = new SqlCommand(@"SELECT ISNULL(SUM(CASE WHEN IslemTuru = 'Alis' THEN Miktar ELSE -Miktar END),0)FROM Tbl_DovizIslemleri WHERE ParaBirimi = 'USD'", con);
                    object sonuc = komut.ExecuteScalar();
                    lbldolar.Text = sonuc.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-OD8JBL6\\SQLEXPRESS;Initial Catalog=SU_Project;Integrated Security=True"))
            {
                con.Open();
                try
                {
                    SqlCommand komut = new SqlCommand(@"SELECT ISNULL(SUM(CASE WHEN IslemTuru = 'Alis' THEN Miktar ELSE -Miktar END),0)FROM Tbl_DovizIslemleri WHERE ParaBirimi = 'EUR'", con);
                    object sonuc = komut.ExecuteScalar();
                    lbleuro.Text = sonuc.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-OD8JBL6\\SQLEXPRESS;Initial Catalog=SU_Project;Integrated Security=True"))
            {
                con.Open();
                try
                {
                    SqlCommand komut = new SqlCommand(@"SELECT ISNULL(SUM(CASE WHEN IslemTuru = 'Alis' THEN Miktar ELSE -Miktar END),0)FROM Tbl_DovizIslemleri WHERE ParaBirimi = 'TL'", con);
                    object sonuc = komut.ExecuteScalar();
                    lbltl.Text = sonuc.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
