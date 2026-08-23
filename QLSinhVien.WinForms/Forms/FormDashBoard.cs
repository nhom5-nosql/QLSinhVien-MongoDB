using QLSinhVien.WinForms.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLSinhVien.WinForms.Forms
{
    public partial class FormDashBoard : Form
    {
        private readonly DashboardRepository _dashboardRepo;

        public FormDashBoard(DashboardRepository dashboardRepo)
        {
            InitializeComponent();
            _dashboardRepo = dashboardRepo;
        }

        private async void FormDashboard_Load(object sender, EventArgs e)
        {
            await RefreshDashboardAsync();
        }

        // Gọi hàm này để cập nhật UI thời gian thực
        public async Task RefreshDashboardAsync()
        {
            // 1. Load KPI Cards
            var kpi = await _dashboardRepo.GetKpiSummaryAsync();
            lblTongSinhVien.Text = $"{kpi.TotalStudents} Sinh viên";
            lblTongSoLop.Text = $"{kpi.TotalClasses} Lớp";
            lblDiemTrungBinh.Text = $"{kpi.AvgScore} / 10";

            double totalGender = kpi.MaleCount + kpi.FemaleCount;
            double malePercent = totalGender > 0 ? Math.Round((kpi.MaleCount / totalGender) * 100, 1) : 0;
            double femalePercent = totalGender > 0 ? Math.Round((kpi.FemaleCount / totalGender) * 100, 1) : 0;
            lblTyLeNamNu.Text = $"Nam: {malePercent}% | Nữ: {femalePercent}%";

            // 2. Load Top 5 DataGrid
            var topStudents = await _dashboardRepo.GetTop5StudentsAsync();
            dgv_Top5SV.DataSource = topStudents;
        }
    }
}
