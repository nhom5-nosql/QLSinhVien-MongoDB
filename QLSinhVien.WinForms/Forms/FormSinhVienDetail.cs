using System;
using System.Drawing;
using System.Windows.Forms;
using QLSinhVien.WinForms.Models;
using QLSinhVien.WinForms.Repositories;

namespace QLSinhVien.WinForms
{
    public partial class FormSinhVienDetail : Form
    {
        private readonly SinhVienRepository _repo;
        private readonly SinhVien? _editingSv;
        private readonly bool _isEditMode;

        public FormSinhVienDetail() : this(new SinhVienRepository())
        {
        }

        public FormSinhVienDetail(SinhVienRepository repo, SinhVien? sv = null)
        {
            _repo = repo;
            _editingSv = sv;
            _isEditMode = sv != null;

            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            if (cbPhai.SelectedIndex < 0)
                cbPhai.SelectedIndex = 0;
        }

        private void FormSinhVienDetail_Load(object sender, EventArgs e)
        {
            if (_isEditMode && _editingSv != null)
            {
                Text = $"Cập nhật thông tin sinh viên — {_editingSv.Masv}";
                txtMasv.Text = _editingSv.Masv;
                txtMasv.Enabled = false; // Mã SV không được đổi
                txtHoten.Text = _editingSv.Hoten;
                txtTuoi.Text = _editingSv.Tuoi.ToString();
                txtMalop.Text = _editingSv.Malop;
                cbPhai.SelectedItem = _editingSv.Phai;

                // Nạp ngoại ngữ cũ
                if (_editingSv.Ngoaingu != null)
                {
                    foreach (var nn in _editingSv.Ngoaingu)
                        AddNgoaiNguRow(nn);
                }

                // Nạp môn học cũ
                if (_editingSv.Monhoc != null)
                {
                    foreach (var mh in _editingSv.Monhoc)
                        AddMonHocRow(mh.Mamon, mh.Tenmon, mh.Diem);
                }
            }
            else
            {
                Text = "Thêm mới sinh viên";
                if (cbPhai.SelectedIndex < 0)
                    cbPhai.SelectedIndex = 0;
            }
        }

        // 1. Nút [+] Thêm ngoại ngữ động
        private void btnAddNgoaiNgu_Click(object sender, EventArgs e)
        {
            AddNgoaiNguRow();
        }

        private void AddNgoaiNguRow(string value = "")
        {
            var rowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(2, 2, 2, 2)
            };

            var txtNgoaiNgu = new TextBox
            {
                Width = 200,
                PlaceholderText = "Tên ngoại ngữ (VD: Anh văn)",
                Text = value
            };

            var btnRemove = new Button
            {
                Text = "✕",
                Width = 32,
                Height = txtNgoaiNgu.Height,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 53, 69),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Click += (s, ev) => flpNgoaiNgu.Controls.Remove(rowPanel);

            rowPanel.Controls.Add(txtNgoaiNgu);
            rowPanel.Controls.Add(btnRemove);
            flpNgoaiNgu.Controls.Add(rowPanel);
        }

        // 2. Nút [+] Thêm môn học động (Mã môn, Tên môn, Điểm)
        private void btnAddMonHoc_Click(object sender, EventArgs e)
        {
            AddMonHocRow();
        }

        private void AddMonHocRow(string mamon = "", string tenmon = "", double diem = 5.0)
        {
            var rowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(2, 2, 2, 2)
            };

            var txtMaMon = new TextBox { Width = 70, PlaceholderText = "Mã môn", Text = mamon };
            var txtTenMon = new TextBox { Width = 110, PlaceholderText = "Tên môn", Text = tenmon };
            var numDiem = new NumericUpDown
            {
                Width = 55,
                DecimalPlaces = 1,
                Maximum = 10,
                Minimum = 0,
                Value = (decimal)Math.Clamp(diem, 0, 10),
                Increment = 0.5m
            };

            var btnRemove = new Button
            {
                Text = "✕",
                Width = 32,
                Height = txtMaMon.Height,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 53, 69),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRemove.FlatAppearance.BorderSize = 0;
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
            string masv = txtMasv.Text.Trim();
            string hoten = txtHoten.Text.Trim();
            string malop = txtMalop.Text.Trim();
            string phai = cbPhai.Text.Trim();

            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(masv))
            {
                MessageBox.Show("Vui lòng nhập Mã sinh viên!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMasv.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(hoten))
            {
                MessageBox.Show("Vui lòng nhập Họ và tên!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoten.Focus();
                return;
            }

            if (!int.TryParse(txtTuoi.Text.Trim(), out int tuoi) || tuoi <= 0 || tuoi > 120)
            {
                MessageBox.Show("Vui lòng nhập số tuổi hợp lệ (1 - 120)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTuoi.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(phai))
                phai = "Nam";

            if (string.IsNullOrWhiteSpace(malop))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMalop.Focus();
                return;
            }

            try
            {
                var sv = new SinhVien
                {
                    Id = _editingSv?.Id,
                    Masv = masv,
                    Hoten = hoten,
                    Tuoi = tuoi,
                    Phai = phai,
                    Malop = malop
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
                            else if (ctrl is NumericUpDown num)
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

                if (_isEditMode)
                {
                    await _repo.UpdateFullAsync(sv.Masv, sv);
                    MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Kiểm tra xem mã SV đã tồn tại chưa
                    var existing = await _repo.FindByMasvExactAsync(sv.Masv);
                    if (existing != null)
                    {
                        MessageBox.Show($"Mã sinh viên '{sv.Masv}' đã tồn tại trong CSDL!", "Trùng mã sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMasv.Focus();
                        return;
                    }

                    await _repo.InsertAsync(sv);
                    MessageBox.Show("Thêm sinh viên mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}