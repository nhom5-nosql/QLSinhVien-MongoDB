using System;
using System.Windows.Forms;
using QLSinhVien.WinForms.Models;
using QLSinhVien.WinForms.Repositories;

namespace QLSinhVien.WinForms
{
    public partial class FormSinhVienDetail : Form
    {
        private readonly SinhVienRepository _repo = new SinhVienRepository();

        public FormSinhVienDetail()
        {
            InitializeComponent();
        }
        private void FormSinhVienDetail_Load(object sender, EventArgs e)
        {
            // Có thể để trống
        }
        // 1. Nút [+] Thêm ngoại ngữ động
        private void btnAddNgoaiNgu_Click(object sender, EventArgs e)
        {
            var rowPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, WrapContents = false };
            var txtNgoaiNgu = new TextBox { Width = 180 };
            var btnRemove = new Button { Text = "X", Width = 30, ForeColor = System.Drawing.Color.Red };

            btnRemove.Click += (s, ev) => flpNgoaiNgu.Controls.Remove(rowPanel);

            rowPanel.Controls.Add(txtNgoaiNgu);
            rowPanel.Controls.Add(btnRemove);
            flpNgoaiNgu.Controls.Add(rowPanel);
        }

        // 2. Nút [+] Thêm môn học động (Mã môn, Tên môn, Điểm)
        private void btnAddMonHoc_Click(object sender, EventArgs e)
        {
            var rowPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, WrapContents = false };
            var txtMaMon = new TextBox { Width = 70, PlaceholderText = "Mã môn" };
            var txtTenMon = new TextBox { Width = 110, PlaceholderText = "Tên môn" };
            var numDiem = new NumericUpDown { Width = 60, DecimalPlaces = 1, Maximum = 10, Minimum = 0, Value = 5 };
            var btnRemove = new Button { Text = "X", Width = 30, ForeColor = System.Drawing.Color.Red };

            btnRemove.Click += (s, ev) => flpMonHoc.Controls.Remove(rowPanel);

            rowPanel.Controls.Add(txtMaMon);
            rowPanel.Controls.Add(txtTenMon);
            rowPanel.Controls.Add(numDiem);
            rowPanel.Controls.Add(btnRemove);
            flpMonHoc.Controls.Add(rowPanel);
        }

        // 3. Nút Lưu dữ liệu vào MongoDB
        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var sv = new SinhVien
                {
                    Masv = txtMasv.Text.Trim(),
                    Hoten = txtHoten.Text.Trim(),
                    Tuoi = int.Parse(txtTuoi.Text.Trim()),
                    Phai = cbPhai.Text,
                    Malop = txtMalop.Text.Trim()
                };

                // Lấy mảng ngoại ngữ từ các dòng động
                foreach (Control row in flpNgoaiNgu.Controls)
                {
                    if (row is FlowLayoutPanel panel)
                    {
                        foreach (Control ctrl in panel.Controls)
                        {
                            if (ctrl is TextBox txt && !string.IsNullOrWhiteSpace(txt.Text))
                            {
                                sv.Ngoaingu.Add(txt.Text.Trim());
                            }
                        }
                    }
                }

                // Lấy mảng môn học từ các dòng động
                foreach (Control row in flpMonHoc.Controls)
                {
                    if (row is FlowLayoutPanel panel)
                    {
                        string mamon = "", tenmon = "";
                        double diem = 0;
                        foreach (Control ctrl in panel.Controls)
                        {
                            if (ctrl is TextBox txt)
                            {
                                if (string.IsNullOrEmpty(mamon)) mamon = txt.Text.Trim();
                                else tenmon = txt.Text.Trim();
                            }
                            if (ctrl is NumericUpDown num)
                            {
                                diem = (double)num.Value;
                            }
                        }
                        if (!string.IsNullOrEmpty(mamon))
                        {
                            sv.Monhoc.Add(new MonHoc { Mamon = mamon, Tenmon = tenmon, Diem = diem });
                        }
                    }
                }

                // Gọi Repository để Insert
                await _repo.InsertAsync(sv);
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void flpMonHoc_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAddMonHoc_Click_1(object sender, EventArgs e)
        {

        }
    }
}