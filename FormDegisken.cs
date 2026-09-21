using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace modbus_portal
{
    public partial class FormDegisken : Form
    {
        public class VariableInfo
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Value { get; set; }
            public string Type { get; set; }
        }

        public static Dictionary<string, VariableInfo> VariableMap = new Dictionary<string, VariableInfo>();

        public FormDegisken()
        {
            InitializeComponent();

            if (cmbValueType.Items.Count == 0)
            {
                cmbValueType.Items.AddRange(new object[] { "INT", "REAL", "BOOL", "STRING" });
                cmbValueType.SelectedIndex = 0;
            }

            if (!VariableMap.ContainsKey("sysTick"))
            {
                VariableMap.Add("sysTick", new VariableInfo
                {
                    Name = "sysTick",
                    Address = "8999",
                    Type = "INT",
                    Value = "0"
                });
            }

            GuncelleListe();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string varName = txtVarName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string val = txtValue.Text.Trim();
            string type = cmbValueType.SelectedItem?.ToString() ?? "INT";

            if (string.IsNullOrEmpty(varName) || string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Lütfen değişken adı ve adresini girin.");
                return;
            }

            if (VariableMap.ContainsKey(varName))
            {
                MessageBox.Show("Bu isimde bir değişken zaten mevcut!");
            }
            else
            {
                VariableInfo vInfo = new VariableInfo
                {
                    Name = varName,
                    Address = address,
                    Value = val,
                    Type = type
                };
                VariableMap.Add(varName, vInfo);
                GuncelleListe();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (lstVariables.SelectedItem != null)
            {
                string secilen = lstVariables.SelectedItem.ToString();
                string varName = secilen.Split('|')[0].Trim();
                VariableMap.Remove(varName);
                GuncelleListe();
            }
        }

        private void btnImportCSV_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Dosyaları (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*";
            ofd.Title = "Değişken Haritası (CSV) Seçin";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // UTF-8 formatında oku
                    string[] lines = File.ReadAllLines(ofd.FileName, Encoding.UTF8);

                    int eklendi = 0;
                    // İlk satır başlık olduğu için i=1'den başlıyoruz
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i])) continue;

                        // Dosya virgül(,) veya noktalı virgül(;) ile ayrılmış olabilir, otomatik algıla
                        char separator = lines[i].Contains(";") ? ';' : ',';
                        string[] columns = lines[i].Split(separator);

                        // Yeni dosyada en az 6 sütun var
                        if (columns.Length >= 6)
                        {
                            string newAddress = columns[0].Trim(); // Sütun 0: Gateway Adresi (Örn: 9000)
                            string varName = columns[3].Trim();    // Sütun 3: Değişken Kodu (Örn: battery_soc)

                            // Gateway'in belleğe kaydettiği (Yayın Veri Tipi) her zaman 32-bit Float'tır.
                            // Orijinal tip (INT16) arayüzü ilgilendirmiyor, bu yüzden REAL atıyoruz.
                            string type = "REAL";

                            // Eğer adres ve isim boş değilse ve listede yoksa ekle
                            if (!string.IsNullOrEmpty(newAddress) &&
                                !string.IsNullOrEmpty(varName) &&
                                !VariableMap.ContainsKey(varName))
                            {
                                VariableMap.Add(varName, new VariableInfo
                                {
                                    Name = varName,
                                    Address = newAddress,
                                    Type = type,
                                    Value = ""
                                });
                                eklendi++;
                            }
                        }
                    }

                    GuncelleListe();
                    MessageBox.Show($"{eklendi} adet değişken başarıyla Gateway formatında (REAL) haritaya eklendi!", "İçe Aktarma Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CSV okunurken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GuncelleListe()
        {
            lstVariables.Items.Clear();
            foreach (var item in VariableMap)
            {
                var v = item.Value;
                lstVariables.Items.Add($"{v.Name} | Adres: {v.Address} | Tür: {v.Type} | Varsayılan: {v.Value}");
            }
        }
    }
}