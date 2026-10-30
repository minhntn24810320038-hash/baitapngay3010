using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using hoccsharp.BUS;

namespace hoccsharp.Forms
{
    public partial class FormCafe : Form
    {
        private readonly CafeBUS _bus = new CafeBUS();
        private int _selectedMaBan = -1, _currentMaHoaDon = -1;
        private FlowLayoutPanel pnlTableLayout;
        private DataGridView dgvBill;
        private ComboBox cboMonAn;
        private NumericUpDown nudSoLuong;
        private Button btnThemMon, btnThanhToan;
        private Label lblTongTien, lblTitle;

        public FormCafe()
        {
            InitUI();
            LoadBanLayout();
            LoadMenu();
        }

        private void InitUI()
        {
            this.Text = "Bài 5.3: POS Quản lý Cà phê";
            this.Size = new Size(950, 580);
            this.StartPosition = FormStartPosition.CenterScreen;

            pnlTableLayout = new FlowLayoutPanel { Location = new Point(12, 12), Size = new Size(420, 510), AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };
            lblTitle = new Label { Location = new Point(450, 12), Size = new Size(460, 25), Font = new Font("Arial", 11, FontStyle.Bold), Text = "VUI LÒNG CHỌN BÀN" };
            dgvBill = new DataGridView { Location = new Point(450, 45), Size = new Size(460, 370), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false };

            cboMonAn = new ComboBox { Location = new Point(450, 430), Size = new Size(180, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            nudSoLuong = new NumericUpDown { Location = new Point(640, 430), Size = new Size(60, 25), Minimum = 1, Value = 1 };
            btnThemMon = new Button { Location = new Point(710, 428), Size = new Size(90, 28), Text = "Thêm món", BackColor = Color.LightSkyBlue };
            btnThemMon.Click += (s, e) => {
                if (_selectedMaBan == -1 || cboMonAn.SelectedItem == null) return;
                DataRowView r = (DataRowView)cboMonAn.SelectedItem;
                _bus.ThemMonVaoBan(_selectedMaBan, Convert.ToInt32(r["MaMon"]), (int)nudSoLuong.Value, Convert.ToDecimal(r["DonGia"]));
                LoadBanLayout(); LoadHoaDon(_selectedMaBan);
            };

            lblTongTien = new Label { Location = new Point(450, 480), Size = new Size(240, 30), Font = new Font("Arial", 11, FontStyle.Bold), ForeColor = Color.Red, Text = "Tổng tiền: 0 VNĐ" };
            btnThanhToan = new Button { Location = new Point(780, 470), Size = new Size(130, 45), Text = "THANH TOÁN", BackColor = Color.LightGreen, Font = new Font("Arial", 10, FontStyle.Bold) };
            btnThanhToan.Click += (s, e) => {
                if (_selectedMaBan == -1 || _currentMaHoaDon == -1) return;
                decimal total = lblTongTien.Tag != null ? Convert.ToDecimal(lblTongTien.Tag) : 0;
                if (_bus.ThanhToan(_currentMaHoaDon, _selectedMaBan, total)) {
                    MessageBox.Show("Thanh toán thành công!"); LoadBanLayout(); dgvBill.DataSource = null; lblTongTien.Text = "Tổng tiền: 0 VNĐ";
                }
            };

            this.Controls.AddRange(new Control[] { pnlTableLayout, lblTitle, dgvBill, cboMonAn, nudSoLuong, btnThemMon, lblTongTien, btnThanhToan });
        }

        private void LoadBanLayout()
        {
            pnlTableLayout.Controls.Clear();
            DataTable dt = _bus.GetDanhSachBan();
            foreach (DataRow r in dt.Rows)
            {
                Button btn = new Button { Width = 90, Height = 80, Text = $"{r["TenBan"]}\n({r["TrangThai"]})", Tag = r["MaBan"], Font = new Font("Arial", 9, FontStyle.Bold) };
                btn.BackColor = r["TrangThai"].ToString() == "Có khách" ? Color.IndianRed : Color.LightSeaGreen;
                btn.ForeColor = Color.White;
                btn.Click += (s, e) => {
                    _selectedMaBan = Convert.ToInt32(btn.Tag);
                    lblTitle.Text = $"ĐANG CHỌN: {r["TenBan"].ToString().ToUpper()}";
                    LoadHoaDon(_selectedMaBan);
                };
                pnlTableLayout.Controls.Add(btn);
            }
        }

        private void LoadMenu() { cboMonAn.DataSource = _bus.GetThucDon(); cboMonAn.DisplayMember = "TenMon"; cboMonAn.ValueMember = "MaMon"; }

        private void LoadHoaDon(int maBan)
        {
            DataTable dt = _bus.GetChiTietHoaDonByBan(maBan, out _currentMaHoaDon);
            dgvBill.DataSource = dt;
            decimal total = 0;
            foreach (DataRow r in dt.Rows) total += Convert.ToDecimal(r["ThanhTien"]);
            lblTongTien.Text = $"Tổng tiền: {total:N0} VNĐ";
            lblTongTien.Tag = total;
        }
    }
}