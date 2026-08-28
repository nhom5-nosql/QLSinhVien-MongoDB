using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLSinhVien.WinForms.Forms;
using QLSinhVien.WinForms.Models;
using QLSinhVien.WinForms.Repositories;

namespace QLSinhVien.WinForms
{
    public partial class FormMain : Form
    {
        private readonly SinhVienRepository _repo;
        private List<SinhVien> _currentList = new();

        public FormMain() : this(new SinhVienRepository())
        {
        }

        public FormMain(SinhVienRepository repo)
        {
            _repo = repo;
            InitializeComponent();
            SetupDataGridView();

            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnSearch.PerformClick();
                }
            };
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private void SetupDataGridView()
        {
            dgvSinhVien.AutoGenerateColumns = false;
            dgvSinhVien.Columns.Clear();

            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMasv", DataPropertyName = "Masv", HeaderText = "Mã SV", Width = 90 });
            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colHoten", DataPropertyName = "Hoten", HeaderText = "Họ và Tên", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTuoi", DataPropertyName = "Tuoi", HeaderText = "Tuổi", Width = 55 });
            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPhai", DataPropertyName = "Phai", HeaderText = "Giới tính", Width = 80 });
            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMalop", DataPropertyName = "Malop", HeaderText = "Lớp", Width = 70 });
            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNgoaingu", DataPropertyName = "NgoaiNguStr", HeaderText = "Ngoại ngữ", Width = 120 });
            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDiemTB", DataPropertyName = "DiemTB", HeaderText = "ĐTB", Width = 65 });
            dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { Name = "colXepLoai", DataPropertyName = "XepLoai", HeaderText = "Xếp loại", Width = 85 });

            dgvSinhVien.RowHeadersVisible = false;
            dgvSinhVien.AllowUserToAddRows = false;
            dgvSinhVien.ReadOnly = true;
            dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSinhVien.MultiSelect = false;
            dgvSinhVien.BorderStyle = BorderStyle.None;
            dgvSinhVien.BackgroundColor = Color.White;
            dgvSinhVien.GridColor = Color.FromArgb(228, 236, 248);
            dgvSinhVien.Cursor = Cursors.Hand;

            dgvSinhVien.ColumnHeadersHeight = 34;
            dgvSinhVien.EnableHeadersVisualStyles = false;
            dgvSinhVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 50, 120);
            dgvSinhVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSinhVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvSinhVien.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvSinhVien.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            dgvSinhVien.DefaultCellStyle.BackColor = Color.White;
            dgvSinhVien.DefaultCellStyle.ForeColor = Color.FromArgb(40, 50, 70);
            dgvSinhVien.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 228, 255);
            dgvSinhVien.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 30, 80);
            dgvSinhVien.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 250, 255);
            dgvSinhVien.RowTemplate.Height = 30;

            dgvSinhVien.CellFormatting += (s, e) =>
            {
                if (e.CellStyle != null && dgvSinhVien.Columns[e.ColumnIndex]?.Name == "colXepLoai" && e.Value != null)
                {
                    e.CellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                    e.CellStyle.ForeColor = e.Value.ToString() switch
                    {
                        "Xuất sắc" => Color.FromArgb(0, 140, 60),
                        "Giỏi" => Color.FromArgb(0, 100, 210),
                        "Khá" => Color.FromArgb(180, 100, 0),
                        _ => Color.FromArgb(190, 30, 30)
                    };
                }
            };

            // Double click mở thông tin sinh viên
            dgvSinhVien.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    OpenSelectedStudentDetail();
                }
            };
        }

        private async Task LoadDataAsync()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                _currentList = await _repo.GetAllAsync();
                BindData(_currentList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BindData(List<SinhVien> list)
        {
            var displayList = list.Select(sv =>
            {
                double dtb = sv.Monhoc.Count > 0 ? Math.Round(sv.Monhoc.Average(m => m.Diem), 2) : 0;
                return new
                {
                    sv.Masv,
                    sv.Hoten,
                    sv.Tuoi,
                    sv.Phai,
                    sv.Malop,
                    NgoaiNguStr = sv.Ngoaingu != null ? string.Join(", ", sv.Ngoaingu) : "",
                    DiemTB = dtb,
                    XepLoai = dtb >= 8.5 ? "Xuất sắc"
                            : dtb >= 7.0 ? "Giỏi"
                            : dtb >= 5.5 ? "Khá"
                            : "TB/Yếu"
                };
            }).ToList();

            dgvSinhVien.DataSource = displayList;
            label1.Text = $"Tổng: {list.Count} sinh viên (Double-click dòng để xem chi tiết)";
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                await LoadDataAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                _currentList = await _repo.SearchPartialAsync(keyword);
                BindData(_currentList);
                if (_currentList.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy sinh viên nào khớp với '{keyword}'!", "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btnThem_Click(object sender, EventArgs e)
        {
            using var detailForm = new FormSinhVienDetail(_repo);
            if (detailForm.ShowDialog(this) == DialogResult.OK)
            {
                await LoadDataAsync();
            }
        }

        private async void btnSua_Click(object sender, EventArgs e)
        {
            var sv = GetSelectedSinhVien();
            if (sv == null)
            {
                MessageBox.Show("Vui lòng chọn 1 sinh viên trên bảng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var detailForm = new FormSinhVienDetail(_repo, sv);
            if (detailForm.ShowDialog(this) == DialogResult.OK)
            {
                await LoadDataAsync();
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var sv = GetSelectedSinhVien();
            if (sv == null)
            {
                MessageBox.Show("Vui lòng chọn dòng sinh viên cần xóa trên bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa sinh viên '{sv.Hoten}' (Mã: {sv.Masv}) khỏi MongoDB?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    await _repo.DeleteAsync(sv.Masv);
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private SinhVien? GetSelectedSinhVien()
        {
            if (dgvSinhVien.SelectedRows.Count == 0) return null;

            string masv = dgvSinhVien.SelectedRows[0].Cells["colMasv"].Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(masv)) return null;

            return _currentList.FirstOrDefault(s => s.Masv.Equals(masv, StringComparison.OrdinalIgnoreCase));
        }

        private void OpenSelectedStudentDetail()
        {
            var sv = GetSelectedSinhVien();
            if (sv != null)
            {
                using var frm = new FormThongTinSV(sv);
                frm.ShowDialog(this);
            }
        }
    }
}