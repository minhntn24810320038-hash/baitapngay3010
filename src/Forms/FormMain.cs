using System;
using System.Drawing;
using System.Windows.Forms;

namespace hoccsharp.Forms
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            this.Text = "HỆ THỐNG QUẢN LÝ TỔNG HỢP (BÀI TẬP 5.3 - 5.4 - 5.5)";
            this.Size = new Size(500, 360);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitle = new Label { Location = new Point(20, 20), Size = new Size(440, 30), Text = "CHỌN CHỨC NĂNG CẦN THỰC THI", Font = new Font("Arial", 13, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };

            Button btn1 = new Button { Location = new Point(80, 70), Size = new Size(320, 50), Text = "Bài 5.3: POS Cà phê (Visual Table)", BackColor = Color.LightSkyBlue, Font = new Font("Arial", 10, FontStyle.Bold) };
            btn1.Click += (s, e) => new FormCafe().ShowDialog();

            Button btn2 = new Button { Location = new Point(80, 135), Size = new Size(320, 50), Text = "Bài 5.4: Quản lý Khách sạn (Hotel)", BackColor = Color.LightGreen, Font = new Font("Arial", 10, FontStyle.Bold) };
            btn2.Click += (s, e) => new FormHotel().ShowDialog();

            Button btn3 = new Button { Location = new Point(80, 200), Size = new Size(320, 50), Text = "Bài 5.5: Quản lý Kho (Inventory)", BackColor = Color.LightCoral, Font = new Font("Arial", 10, FontStyle.Bold) };
            btn3.Click += (s, e) => new FormInventory().ShowDialog();

            this.Controls.AddRange(new Control[] { lblTitle, btn1, btn2, btn3 });
        }
    }
}