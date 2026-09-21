using System;
using System.Windows.Forms;

namespace modbus_portal
{
    public partial class FormOzellik : Form
    {
        private string _seciliScriptTuru;

        // Form1'den hangi script türünün seçildiğini parametre olarak alıyoruz
        public FormOzellik(string scriptTuru)
        {
            InitializeComponent();
            _seciliScriptTuru = scriptTuru;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim().ToLower();
            rtbHelp.Clear();

            if (query == "--help")
            {
                rtbHelp.Text = "=== KURALLAR ===\n1. Her satıra tek bir komut yazın.\n2. Değişkenleri önce 'Değişken Ata' kısmından tanımlayın.\n3. Assembly için format: KOMUT KAYNAK1 KAYNAK2 HEDEF";
                return;
            }

            // Seçilen script türüne göre dinamik yardım dökümanı
            if (_seciliScriptTuru == "Assembly")
            {
                if (query.Contains("topla")) rtbHelp.Text = "Kullanım: ADD [Kaynak1] [Kaynak2] [Hedef]\nÖrnek: ADD Sensor1 50 SicaklikFarki";
                else if (query.Contains("çarp") || query.Contains("carp")) rtbHelp.Text = "Kullanım: MUL [Kaynak1] [Kaynak2] [Hedef]\nÖrnek: MUL MotorHizi 2 YeniHiz";
                else rtbHelp.Text = "Sonuç bulunamadı. Kurallar için '--help' yazın.";
            }
            else if (_seciliScriptTuru == "Python")
            {
                // İleride eklenecek Python makroları için
                if (query.Contains("döngü")) rtbHelp.Text = "Kullanım: for i in range(x):\nÖrnek:\nfor i in range(10):\n  deger += 1";
                else rtbHelp.Text = "Sonuç bulunamadı.";
            }
            else if (_seciliScriptTuru == "C")
            {
                if (query.Contains("timer")) rtbHelp.Text = "Kullanım: delay(ms);\nÖrnek: delay(1000); // 1 saniye bekle";
                else rtbHelp.Text = "Sonuç bulunamadı.";
            }
            else
            {
                MessageBox.Show("Geçerli bir script türü seçilmediği için özellik aranamıyor.");
            }
        }
    }
}