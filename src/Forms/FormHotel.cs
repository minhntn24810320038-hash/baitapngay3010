using System;
using System.Drawing;
using System.Windows.Forms;
using hoccsharp.BUS;
using hoccsharp.DTO;

namespace hoccsharp.Forms
{
    public partial class FormHotel : Form
    {
        private readonly HotelBUS _bus = new HotelBUS();
        private string _selectedSoPhong = "";
        private DataGridView dgvPhong;
        private TextBox txtCCCD, txtHoTen, txtSDT;
        private Button btnTimCCCD, btnCheckIn, btnCheckOut;

        public FormHotel()
        {
            InitUI();
            LoadPhong();
        }

        private void InitUI()
        {
            this.Text = "Bài 5.4: Quản lý Khách sạn";
            this.Size = new Size(850, 480);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvPhong = new DataGridView { Location = new Point(12, 12), Size = new Size(450, 410), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgvPhong.SelectionChanged += (s, e) => {
                if (dgvPhong.CurrentRow != null) _selectedSoPhong = dgvPhong.CurrentRow.Cells["SoPhong"].Value.ToString();
            };

            Label lblCCCD = new Label { Location = new Point(480, 20), Text = "Số CCCD:", AutoSize = true };
            txtCCCD = new TextBox { Location = new Point(560, 17), Size = new Size(150, 25) };
            btnTimCCCD = new Button { Location = new Point(720, 15), Size = new Size(80, 28), Text = "Tra cứu" };
            btnTimCCCD.Click += (s, e) => {
                KhachHangDTO kh = _bus.TimKhachHangByCCCD(txtCCCD.Text.Trim());
                if (kh != null) { txtHoTen.Text = kh.HoTen; txtSDT.Text = kh.SoDienThoai; MessageBox.Show("Đã tìm thấy khách hàng cũ!"); }
                else MessageBox.Show("Khách hàng mới! Nhập Tên và SĐT bên dưới.");
            };

            Label lblTen = new Label { Location = new Point(480, 60), Text = "Họ tên KH:", AutoSize = true };
            txtHoTen = new TextBox { Location = new Point(560, 57), Size = new Size(240, 25) };

            Label lblSDT = new Label { Location = new Point(480, 100), Text = "Số ĐT:", AutoSize = true };
            txtSDT = new TextBox { Location = new Point(560, 97), Size = new Size(240, 25) };

            btnCheckIn = new Button { Location = new Point(480, 150), Size = new Size(150, 45), Text = "CHECK-IN", BackColor = Color.LightSkyBlue };
            btnCheckIn.Click += (s, e) => {
                try { _bus.CheckIn(_selectedSoPhong, txtHoTen.Text, txtCCCD.Text, txtSDT.Text); MessageBox.Show("Check-in thành công!"); LoadPhong(); }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            };

            btnCheckOut = new Button { Location = new Point(650, 150), Size = new Size(150, 45), Text = "CHECK-OUT", BackColor = Color.LightCoral };
            btnCheckOut.Click += (s, e) => {
                try { decimal total = _bus.CheckOut(_selectedSoPhong); MessageBox.Show($"Check-out thành công!\nTổng tiền: {total:N0} VNĐ"); LoadPhong(); }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            };

            this.Controls.AddRange(new Control[] { dgvPhong, lblCCCD, txtCCCD, btnTimCCCD, lblTen, txtHoTen, lblSDT, txtSDT, btnCheckIn, btnCheckOut });
        }

        private void LoadPhong() => dgvPhong.DataSource = _bus.GetDanhSachPhong();
    }
}