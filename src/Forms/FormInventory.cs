using System;
using System.Drawing;
using System.Windows.Forms;
using hoccsharp.BUS;
using hoccsharp.DTO;

namespace hoccsharp.Forms
{
    public partial class FormInventory : Form
    {
        private readonly InventoryBUS _bus = new InventoryBUS();
        private DataGridView dgvVatTu;
        private ComboBox cboLoaiPhieu;
        private TextBox txtMaVatTu, txtSoLuong, txtDonGia;
        private Button btnLuuPhieu;

        public FormInventory()
        {
            InitUI();
            LoadVatTu();
        }

        private void InitUI()
        {
            this.Text = "Bài 5.5: Quản lý Xuất - Nhập Kho";
            this.Size = new Size(800, 480);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvVatTu = new DataGridView { Location = new Point(12, 12), Size = new Size(420, 410), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };

            Label lblLoai = new Label { Location = new Point(450, 20), Text = "Loại phiếu:", AutoSize = true };
            cboLoaiPhieu = new ComboBox { Location = new Point(540, 17), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoaiPhieu.Items.AddRange(new object[] { "NHAP", "XUAT" });
            cboLoaiPhieu.SelectedIndex = 0;

            Label lblMa = new Label { Location = new Point(450, 60), Text = "Mã vật tư:", AutoSize = true };
            txtMaVatTu = new TextBox { Location = new Point(540, 57), Size = new Size(180, 25) };

            Label lblSL = new Label { Location = new Point(450, 100), Text = "Số lượng:", AutoSize = true };
            txtSoLuong = new TextBox { Location = new Point(540, 97), Size = new Size(180, 25) };

            Label lblGia = new Label { Location = new Point(450, 140), Text = "Đơn giá:", AutoSize = true };
            txtDonGia = new TextBox { Location = new Point(540, 137), Size = new Size(180, 25) };

            btnLuuPhieu = new Button { Location = new Point(540, 190), Size = new Size(180, 40), Text = "LƯU PHIẾU KHO", BackColor = Color.LightGreen, Font = new Font("Arial", 9, FontStyle.Bold) };
            btnLuuPhieu.Click += (s, e) => {
                try {
                    PhieuKhoDTO phieu = new PhieuKhoDTO { LoaiPhieu = cboLoaiPhieu.SelectedItem.ToString() };
                    phieu.DanhSachChiTiet.Add(new ChiTietPhieuDTO { MaVatTu = txtMaVatTu.Text.Trim(), SoLuong = Convert.ToDecimal(txtSoLuong.Text), DonGia = Convert.ToDecimal(txtDonGia.Text) });
                    if (_bus.LapPhieuKho(phieu)) { MessageBox.Show("Lưu phiếu kho thành công!"); LoadVatTu(); }
                }
                catch (Exception ex) {
                    MessageBox.Show("GIAO DỊCH LỖI (ROLLBACK): " + ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            this.Controls.AddRange(new Control[] { dgvVatTu, lblLoai, cboLoaiPhieu, lblMa, txtMaVatTu, lblSL, txtSoLuong, lblGia, txtDonGia, btnLuuPhieu });
        }

        private void LoadVatTu() => dgvVatTu.DataSource = _bus.GetDanhSachVatTu();
    }
}