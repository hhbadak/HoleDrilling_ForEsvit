using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoleDrillingApp
{
    public partial class Home : Form
    {
        public static Employee LoginUser;
        DataModel dm = new DataModel();
        int QualityID = 0;
        int FaultID = 0;
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            LoginPage frm = new LoginPage();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Assume Helpers.isLogin is a static property holding the logged-in user's info
                Employee model = Helpers.isLogin;
                Home.LoginUser.ID = model.ID; // Assuming QualityPersonalID is obtained from the logged-in user
            }

            //ComboBox'a verileri ekliyoruz
            cb_quality.ValueMember = "ID";
            cb_quality.DisplayMember = "Definition";
            cb_quality.DataSource = dm.GetResult();

            loadGrid();
        }

        private void loadGrid()
        {
          
            // Veriyi çek
            var result = dm.logEntryListHoleDrilling(new HoleDrilling
            {
                Barcode = tb_barcode.Text,
                QualityID = cb_quality.SelectedIndex,
                ProductCode = tb_code.Text,
            });

            if (result != null)
            {
                var rt = result.OrderByDescending(r => r.ID).ToList();
                DataTable dt = new DataTable();

                dt.Columns.Add("ID");
                dt.Columns.Add("Barkod No");
                dt.Columns.Add("Kontrol Tarihi");
                dt.Columns.Add("Kalite");
                dt.Columns.Add("Kod");
                dt.Columns.Add("Delik Delme Personeli");

                foreach (var item in rt)
                {
                    DataRow r = dt.NewRow();
                    r["ID"] = item.ID;
                    r["Barkod No"] = item.Barcode;
                    r["Kontrol Tarihi"] = item.DateTime;
                    r["Kalite"] = item.Quality;
                    r["Kod"] = item.ProductCode;
                    r["Delik Delme Personeli"] = item.QualityPersonal;
                    dt.Rows.Add(r);
                }

                dataGridView1.DataSource = dt;

                // Yalnızca veri içeren satırları say
                int nonEmptyRowCount = dataGridView1.Rows.Cast<DataGridViewRow>()
                    .Count(row => !row.IsNewRow && row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Value != null && cell.Value.ToString() != ""));

                lbl_number.Text = "Bakılan Ürün sayısı: " + nonEmptyRowCount;
            }
            else
            {
                MessageBox.Show("Veri yüklenirken bir hata oluştu.");
            }

            tb_barcode.Select(); // Barkod textbox'ına odaklan
        }

        private void tb_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tb_barcode.Text.Length == 10)
                {
                    if (dm.isThereBarcode(tb_barcode.Text))
                    {
                        // Get the quality from the Products table
                        var products = dm.getBarcodeQuality(tb_barcode.Text);
                        if (products != null && products.Any())
                        {
                            // Set the quality to cb_quality if not manually changed
                            if (cb_quality.SelectedIndex == 0)
                            {
                                var quality = products.FirstOrDefault()?.QualityID ?? 0;
                                cb_quality.SelectedIndex = cb_quality.Items.Cast<ListQuality>().ToList().FindIndex(q => q.ID == quality);
                            }
                        }

                        tb_code.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Ürün Kalite Masasına Uğramamış");
                        tb_barcode.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Geçerli Barkod Numarası Giriniz");
                    tb_barcode.Text = "";
                }
            }
        }

        private void cb_quality_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Check if the user manually changed the quality
            if (cb_quality.SelectedIndex > 0)
            {
                // Update Products table with the new quality
                var barcode = tb_barcode.Text;
                var newQualityId = ((ListQuality)cb_quality.SelectedItem).ID;

                // Update Products table with the new quality
                dm.UpdateProductQuality(barcode, Convert.ToByte(newQualityId));
            }

        }
        
        private void tb_code_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tb_code.Text.Length == 3)
                {
                    int productCodeID = dm.GetProductCodeID(tb_code.Text);
                    if (productCodeID > 0)
                    {
                        // Ürün kodu geçerli
                        HoleDrilling hd = new HoleDrilling
                        {
                            Barcode = tb_barcode.Text,
                            DateTime = DateTime.Now,
                            QualityID = cb_quality.SelectedIndex > 0 ? ((ListQuality)cb_quality.SelectedItem).ID : 0,
                            ProductCodeID = productCodeID,
                            QualityPersonalID = Home.LoginUser.ID
                        };

                        if (dm.createHoleDrilling(hd))
                        {
                            // TextBox ve ComboBox'ları sıfırla
                            tb_barcode.Text = tb_code.Text = "";
                            cb_quality.SelectedIndex = 0;

                            // Grid'i yeniden yükle
                            loadGrid();
                        }
                        else
                        {
                            MessageBox.Show("Kayıt oluşturulurken bir hata oluştu.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Geçerli bir ürün kodu giriniz.");
                    }
                }
                else
                {
                    MessageBox.Show("Ürün kodu 3 karakter uzunluğunda olmalıdır.");
                }
            }
        }

    }
}
