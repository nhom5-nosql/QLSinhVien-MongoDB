using QLSinhVien.WinForms.Repositories;
using QLSinhVien.WinForms.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace QLSinhVien.WinForms.Forms
{
    public partial class FormDashBoard : Form
    {
        private readonly DashboardRepository _dashboardRepo;
        private readonly SinhVienRepository  _svRepo;

        // Cache dữ liệu để rebuild khi resize
        private Dictionary<string, int> _rankStats        = new();
        private List<LanguageStatDto>   _languagesCache   = new();
        private List<ClassStatDto>      _classStatsCache  = new();

        // Debounce search
        private CancellationTokenSource _searchCts = new();

        // Màu sắc cho từng loại xếp hạng
        private static readonly Color[] PieColors = new[]
        {
            Color.FromArgb(52, 199, 89),   // Xuất sắc - xanh lá
            Color.FromArgb(0, 122, 255),   // Giỏi      - xanh dương
            Color.FromArgb(255, 159, 10),  // Khá       - cam
            Color.FromArgb(255, 59, 48),   // TB/Yếu    - đỏ
        };
        private static readonly string[] RankKeys = { "Xuất sắc", "Giỏi", "Khá", "TB/Yếu" };

        public FormDashBoard(DashboardRepository dashboardRepo, SinhVienRepository svRepo)
        {
            InitializeComponent();
            _dashboardRepo = dashboardRepo;
            _svRepo        = svRepo;

            this.Load         += FormDashboard_Load;
            this.Resize       += FormDashBoard_Resize;
            guna2Panel6.Paint += GunaPiePanel_Paint;

            // Thiết lập ô tìm kiếm
            txtTimSV.Text = "";
            txtTimSV.PlaceholderText = "Nhập mã SV hoặc họ tên (Nhấn Enter để tìm)...";
            txtTimSV.KeyDown += TxtTimSV_KeyDown;

            // Nút Quản lý Sinh viên (CRUD) trên sidebar
            guna2Button1.Click += BtnCrud_Click;

            // Nút Danh sách Sinh viên trên sidebar
            guna2Button2.Click += BtnDanhSach_Click;

            // Nút Thoát trên sidebar
            guna2Button3.Click += BtnExit_Click;

            // Nút Home trên sidebar (tải lại dữ liệu dashboard)
            btnHome.Click += async (_, _) => await RefreshDashboardAsync();
        }

        // ─── NÚT CRUD SINH VIÊN (guna2Button1) ────────────────────────────────────
        private async void BtnCrud_Click(object? sender, EventArgs e)
        {
            using var frmCrud = new FormMain(_svRepo);
            frmCrud.ShowDialog(this);
            // Sau khi thao tác CRUD (Thêm/Sửa/Xóa), tự động làm mới số liệu trên Dashboard
            await RefreshDashboardAsync();
        }

        // ─── NÚT XEM TOÀN BỘ DANH SÁCH (guna2Button2) ────────────────────────────
        private async void BtnDanhSach_Click(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var allStudents = await _svRepo.GetAllStudentsAsync();
                using var frmDs = new FormDanhSachSV(allStudents, _svRepo);
                frmDs.Text = "Danh sách toàn bộ sinh viên";
                frmDs.ShowDialog(this);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ─── NÚT THOÁT ỨNG DỤNG (guna2Button3) ───────────────────────────────────
        private void BtnExit_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát chương trình?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // ─── TÌM KIẾM SINH VIÊN ──────────────────────────────────────────────────
        private async void TxtTimSV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            string keyword = txtTimSV.Text.Trim();
            if (string.IsNullOrEmpty(keyword)) return;

            // Hiển thị loading cursor
            Cursor = Cursors.WaitCursor;
            try
            {
                await HandleSearchAsync(keyword);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task HandleSearchAsync(string keyword)
        {
            // 1. Tìm CHÍNH XÁC theo mã SV
            var svByMasv = await _svRepo.FindByMasvExactAsync(keyword);
            if (svByMasv != null)
            {
                using var frm = new FormThongTinSV(svByMasv);
                frm.ShowDialog(this);
                return;
            }

            // 2. Tìm CHÍNH XÁC theo họ tên
            var svByHoten = await _svRepo.FindByHotenExactAsync(keyword);
            if (svByHoten != null)
            {
                using var frm = new FormThongTinSV(svByHoten);
                frm.ShowDialog(this);
                return;
            }

            // 3. Tìm GẦN ĐÚNG (partial) → FormDanhSachSV
            var list = await _svRepo.SearchPartialAsync(keyword);
            if (list.Count == 0)
            {
                MessageBox.Show(
                    $"Không tìm thấy sinh viên nào khớp với '{keyword}'.",
                    "Không có kết quả",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using var frmDs = new FormDanhSachSV(list, _svRepo);
            frmDs.ShowDialog(this);
        }

        private async void FormDashboard_Load(object sender, EventArgs e)
        {
            ApplyLayout(); // layout lần đầu theo kích thước mặc định
            await RefreshDashboardAsync();
        }

        // ─── LAYOUT CO GIÃN ────────────────────────────────────────────────────────
        private void FormDashBoard_Resize(object sender, EventArgs e)
        {
            ApplyLayout();
        }

        private void ApplyLayout()
        {
            // Hằng số layout
            const int margin  = 12;
            const int sideW   = 52;
            const int kpiH    = 100;
            const int searchH = 44;
            const int statsH  = 115;
            const int kpiGap  = 12; // khoảng cách giữa các KPI card

            int contentX = margin + sideW + margin;            // X bắt đầu vùng nội dung
            int contentW = ClientSize.Width - contentX - margin; // Chiều rộng khả dụng

            if (contentW < 200) return; // Quá nhỏ, bỏ qua

            // ── Sidebar ──────────────────────────────────────────────────
            SideMenuPanel.Height = ClientSize.Height - margin * 2;

            // ── Thanh tìm kiếm ───────────────────────────────────────────
            guna2Panel1.Left  = contentX;
            guna2Panel1.Width = contentW;
            txtTimSV.Location = new Point(8, (guna2Panel1.Height - txtTimSV.Height) / 2);
            txtTimSV.Width    = guna2Panel1.Width - 16;

            // ── KPI Cards (4 card chia đều) ──────────────────────────────
            int kpiY    = margin + searchH + margin;
            int kpiCardW = (contentW - kpiGap * 3) / 4;

            guna2Panel2.SetBounds(contentX,                           kpiY, kpiCardW, kpiH);
            guna2Panel3.SetBounds(contentX + (kpiCardW + kpiGap),     kpiY, kpiCardW, kpiH);
            guna2Panel4.SetBounds(contentX + (kpiCardW + kpiGap) * 2, kpiY, kpiCardW, kpiH);
            guna2Panel5.SetBounds(contentX + (kpiCardW + kpiGap) * 3, kpiY, kpiCardW, kpiH);

            // Căn giữa text trong từng KPI card
            CenterLabelsInKpiCard(guna2Panel2, new[] { label,  lblTongSinhVien  });
            CenterLabelsInKpiCard(guna2Panel3, new[] { label1, lblTongSoLop     });
            CenterLabelsInKpiCard(guna2Panel4, new[] { label2, lblDiemTrungBinh });
            CenterLabelsInKpiCard(guna2Panel5, new[] { label3, lblTyLeNamNu     });

            // ── Vùng giữa: Pie + Top5 ────────────────────────────────────
            int midY = kpiY + kpiH + margin;
            int midH = ClientSize.Height - midY - margin - statsH - margin;
            if (midH < 80) midH = 80;

            int pieW  = (int)(contentW * 0.37);
            int top5W = contentW - pieW - margin;

            guna2Panel6.SetBounds(contentX,              midY, pieW,  midH);
            guna2Panel7.SetBounds(contentX + pieW + margin, midY, top5W, midH);

            // Resize DataGridView bên trong panel7
            dgv_Top5SV.SetBounds(
                dgv_Top5SV.Left, dgv_Top5SV.Top,
                guna2Panel7.Width  - dgv_Top5SV.Left - 12,
                guna2Panel7.Height - dgv_Top5SV.Top  - 12);

            // ── Panel thống kê ───────────────────────────────────────────
            int statsY = midY + midH + margin;
            guna2Panel8.SetBounds(contentX, statsY, contentW, statsH);

            // Rebuild nội dung động của panel thống kê (nếu đã có dữ liệu)
            if (_languagesCache.Count > 0 || _classStatsCache.Count > 0)
                BuildStatsPanel(_languagesCache, _classStatsCache);

            // Redraw pie chart
            guna2Panel6.Invalidate();
        }

        /// <summary>Căn giữa 2 label (tiêu đề + giá trị) trong KPI card theo chiều ngang.</summary>
        private static void CenterLabelsInKpiCard(Control card, Label[] labels)
        {
            foreach (var lbl in labels)
                lbl.Left = (card.Width - lbl.Width) / 2;
        }

        // ─── LOAD DỮ LIỆU TỪ MONGODB ──────────────────────────────────────────────
        public async Task RefreshDashboardAsync()
        {
            // 1. KPI Cards
            var kpi = await _dashboardRepo.GetKpiSummaryAsync();

            lblTongSinhVien.Text  = $"{kpi.TotalStudents} SV";
            lblTongSoLop.Text     = $"{kpi.TotalClasses} Lớp";
            lblDiemTrungBinh.Text = kpi.AvgScore.ToString("F2");

            double totalGender  = kpi.MaleCount + kpi.FemaleCount;
            double malePercent  = totalGender > 0 ? Math.Round((kpi.MaleCount   / totalGender) * 100, 1) : 0;
            double femalePercent= totalGender > 0 ? Math.Round((kpi.FemaleCount / totalGender) * 100, 1) : 0;
            lblTyLeNamNu.Text   = $"Nam: {malePercent}% | Nữ: {femalePercent}%";

            // Căn lại sau khi có text mới
            ApplyLayout();

            // 2. Pie Chart học lực
            _rankStats = await _dashboardRepo.GetAcademicRankStatsAsync();
            guna2Panel6.Invalidate();

            // 3. Top 5 DataGridView
            var topStudents = await _dashboardRepo.GetTop5StudentsAsync();
            SetupDataGridView();
            dgv_Top5SV.DataSource = topStudents;
            StyleDataGridViewRows();

            // 4. Thống kê ngoại ngữ + theo lớp
            _languagesCache  = await _dashboardRepo.GetPopularLanguagesAsync();
            _classStatsCache = await _dashboardRepo.GetClassStatsAsync();
            BuildStatsPanel(_languagesCache, _classStatsCache);
        }

        // ─── VẼ PIE CHART (DONUT) ─────────────────────────────────────────────────
        private void GunaPiePanel_Paint(object sender, PaintEventArgs e)
        {
            if (_rankStats == null || _rankStats.Count == 0) return;

            int total = _rankStats.Values.Sum();
            if (total == 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Kích thước pie tính theo panel (co giãn)
            int panelW   = guna2Panel6.Width;
            int panelH   = guna2Panel6.Height;
            int pieSize  = Math.Min(panelW, panelH - 55);
            pieSize      = Math.Max(80, Math.Min(pieSize - 30, 160));
            int pieX     = 15;
            int pieY     = 48;
            var pieRect  = new Rectangle(pieX, pieY, pieSize, pieSize);

            // Tính các phần của donut
            float startAngle = -90f;
            var segments = new List<(string label, int count, Color color, float sweep)>();
            int ci = 0;
            foreach (var key in RankKeys)
            {
                int val   = _rankStats.ContainsKey(key) ? _rankStats[key] : 0;
                float sw  = (float)val / total * 360f;
                segments.Add((key, val, PieColors[ci % PieColors.Length], sw));
                ci++;
            }

            foreach (var seg in segments)
            {
                using var b = new SolidBrush(seg.color);
                g.FillPie(b, pieRect, startAngle, seg.sweep);
                startAngle += seg.sweep;
            }

            // Donut hole
            int holeSize = (int)(pieSize * 0.53);
            int holeX    = pieX + (pieSize - holeSize) / 2;
            int holeY    = pieY + (pieSize - holeSize) / 2;
            using var holeBrush = new SolidBrush(Color.White);
            g.FillEllipse(holeBrush, holeX, holeY, holeSize, holeSize);

            // Text giữa donut
            using var cf = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString($"{total}\nSV", cf, Brushes.DimGray, new RectangleF(holeX, holeY, holeSize, holeSize), sfCenter);

            // Legend bên phải
            int lx = pieX + pieSize + 14;
            int ly = pieY + 8;
            using var lf = new Font("Segoe UI", 7.5f);

            foreach (var seg in segments)
            {
                double pct = Math.Round((double)seg.count / total * 100, 1);
                using var db = new SolidBrush(seg.color);
                g.FillEllipse(db, lx, ly + 3, 9, 9);
                g.DrawString($"{seg.label}: {pct}%  ({seg.count} SV)", lf, Brushes.DimGray, lx + 13, ly);
                ly += 22;
            }
        }

        // ─── DATAGRIDVIEW TOP 5 ───────────────────────────────────────────────────
        private void SetupDataGridView()
        {
            var dgv = dgv_Top5SV;
            if (dgv.Columns.Count > 0) return; // Đã setup rồi

            dgv.AutoGenerateColumns = false;
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Masv",    HeaderText = "Mã SV",     Width = 65  });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Hoten",   HeaderText = "Họ và Tên", Width = 145 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Malop",   HeaderText = "Lớp",       Width = 50  });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemTB",  HeaderText = "ĐTB",       Width = 50  });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "XepLoai", HeaderText = "Xếp loại",  Width = 75  });

            dgv.RowHeadersVisible    = false;
            dgv.AllowUserToAddRows   = false;
            dgv.ReadOnly             = true;
            dgv.SelectionMode        = DataGridViewSelectionMode.FullRowSelect;
            dgv.BorderStyle          = BorderStyle.None;
            dgv.GridColor            = Color.FromArgb(235, 240, 245);
            dgv.BackgroundColor      = Color.White;
            dgv.DefaultCellStyle.Font              = new Font("Segoe UI", 9f);
            dgv.DefaultCellStyle.BackColor         = Color.White;
            dgv.DefaultCellStyle.ForeColor         = Color.FromArgb(40, 50, 70);
            dgv.DefaultCellStyle.SelectionBackColor= Color.FromArgb(220, 235, 255);
            dgv.DefaultCellStyle.SelectionForeColor= Color.FromArgb(20, 30, 60);
            dgv.AlternatingRowsDefaultCellStyle.BackColor            = Color.FromArgb(247, 250, 255);
            dgv.ColumnHeadersDefaultCellStyle.Font                   = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor              = Color.FromArgb(30, 80, 162);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor              = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Alignment              = DataGridViewContentAlignment.MiddleCenter;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight       = 32;
            dgv.RowTemplate.Height        = 30;

            // Cột cuối tự co giãn lấp đầy chiều rộng
            dgv.Columns[^1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgv.CellFormatting += Dgv_CellFormatting;
            dgv.CellDoubleClick += async (sender, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Rows[e.RowIndex].DataBoundItem is StudentRankDto topSv)
                {
                    var sv = await _svRepo.FindByMasvExactAsync(topSv.Masv);
                    if (sv != null)
                    {
                        using var frm = new FormThongTinSV(sv);
                        frm.ShowDialog(this);
                    }
                }
            };
        }

        private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv != null && dgv.Columns[e.ColumnIndex].DataPropertyName == "XepLoai" && e.Value != null)
            {
                string xepLoai = e.Value.ToString() ?? "";
                e.CellStyle.ForeColor = xepLoai switch
                {
                    "Xuất sắc" => Color.FromArgb(0, 140, 60),
                    "Giỏi"     => Color.FromArgb(0, 90, 200),
                    "Khá"      => Color.FromArgb(180, 100, 0),
                    _          => Color.FromArgb(190, 30, 30),
                };
                e.CellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            }
        }

        private void StyleDataGridViewRows()
        {
            if (dgv_Top5SV.Rows.Count > 0)
                dgv_Top5SV.Rows[0].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
        }

        // ─── PANEL THỐNG KÊ (guna2Panel8) ────────────────────────────────────────
        private void BuildStatsPanel(List<LanguageStatDto> languages, List<ClassStatDto> classStats)
        {
            var panel = guna2Panel8;
            panel.SuspendLayout();
            panel.Controls.Clear();

            int halfW  = panel.Width / 2;
            int rightX = halfW + 10;

            // ── Ngoại ngữ (trái) ──
            panel.Controls.Add(new Label
            {
                Text      = "THỐNG KÊ NGOẠI NGỮ",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 162),
                Location  = new Point(12, 10),
                AutoSize  = true
            });

            int langY = 32;
            foreach (var lang in languages.Take(4))
            {
                panel.Controls.Add(new Label
                {
                    Text      = $"• {lang.NgoaiNgu}: {lang.Count} SV",
                    Font      = new Font("Segoe UI", 8.5f),
                    ForeColor = Color.FromArgb(50, 60, 80),
                    Location  = new Point(12, langY),
                    AutoSize  = true
                });
                langY += 20;
            }

            // ── Đường kẻ dọc giữa ──
            panel.Controls.Add(new Panel
            {
                Location  = new Point(halfW - 1, 8),
                Size      = new Size(1, panel.Height - 16),
                BackColor = Color.FromArgb(200, 215, 230)
            });

            // ── Thống kê theo lớp (phải) ──
            panel.Controls.Add(new Label
            {
                Text      = "THỐNG KÊ THEO LỚP",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 162),
                Location  = new Point(rightX, 10),
                AutoSize  = true
            });

            panel.Controls.Add(new Label
            {
                Text      = $"{"Lớp",-8} {"SV",4}   {"Max ĐTB",10}   {"Min ĐTB",10}",
                Font      = new Font("Consolas", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 100, 140),
                Location  = new Point(rightX, 30),
                AutoSize  = true
            });

            int classY = 46;
            foreach (var cls in classStats.Take(4))
            {
                panel.Controls.Add(new Label
                {
                    Text      = $"{cls.MaLop,-8} {cls.TotalStudents,4} SV   Max: {cls.MaxAvgScore:F2}   Min: {cls.MinAvgScore:F2}",
                    Font      = new Font("Consolas", 7.5f),
                    ForeColor = Color.FromArgb(50, 60, 80),
                    Location  = new Point(rightX, classY),
                    AutoSize  = true
                });
                classY += 18;
            }

            panel.ResumeLayout();
        }
    }
}
