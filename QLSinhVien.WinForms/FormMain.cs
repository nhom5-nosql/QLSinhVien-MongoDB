using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLSinhVien.WinForms.Repositories;
using QLSinhVien.WinForms.Models;

namespace QLSinhVien.WinForms
{
    public partial class FormMain : Form
    {
        private readonly SinhVienRepository _repo = new SinhVienRepository();

        public FormMain()
        {
            InitializeComponent();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                var list = await _repo.GetAllAsync();
                dgvSinhVien.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string masv = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(masv))
            {
                var sv = await _repo.GetByMaSVAsync(masv);
                dgvSinhVien.DataSource = sv != null ? new List<SinhVien> { sv } : null;
            }
            else
            {
                await LoadDataAsync();
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.SelectedRows.Count > 0)
            {
                var masv = dgvSinhVien.SelectedRows[0].Cells["Masv"].Value.ToString();
                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa sinh viên {masv}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    await _repo.DeleteAsync(masv);
                    await LoadDataAsync();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng sinh viên cần xóa trên bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            var detailForm = new FormSinhVienDetail();
            if (detailForm.ShowDialog() == DialogResult.OK)
            {
                _ = LoadDataAsync();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Xử lý khi click vào ô trên bảng
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}