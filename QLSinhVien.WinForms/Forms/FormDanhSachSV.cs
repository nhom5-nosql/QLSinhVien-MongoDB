using QLSinhVien.WinForms.Models;
using QLSinhVien.WinForms.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLSinhVien.WinForms.Forms
{
    public partial class FormDanhSachSV : Form
    {
        private readonly List<SinhVien>      _students;
        private readonly SinhVienRepository _svRepo;

        // Controls cần truy cập từ nhiều method
        private DataGridView _dgv         = null!;
        private Button       _btnXemThongTin = null!;
        private Label        _lblCount    = null!;

        public FormDanhSachSV(List<SinhVien> students, SinhVienRepository svRepo)
        {
            _students = students;
            _svRepo   = svRepo;

            InitializeComponent();
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            Text            = "Kết quả tìm kiếm";
            Size            = new Size(860, 520);
            MinimumSize     = new Size(700, 400);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.FromArgb(240, 245, 252);
            FormBorderStyle = FormBorderStyle.Sizable;
            Font            = new Font("Segoe UI", 9f);
            Controls.Clear();

            // ════════════ HEADER ════════════
            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 54,
                BackColor = Color.FromArgb(20, 60, 140),
                Padding   = new Padding(16, 0, 16, 0)
            };
            pnlHeader.Controls.Add(new Label
            {
                Text      = "Danh sách sinh viên tìm được",
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Left,
                AutoSize  = false,
                Width     = 400,
                TextAlign = ContentAlignment.MiddleLeft
            });

            _lblCount = new Label
            {
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(170, 200, 255),
                Dock      = DockStyle.Right,
                Width     = 200,
                TextAlign = ContentAlignment.MiddleRight
            };
            pnlHeader.Controls.Add(_lblCount);

            // ════════════ DATAGRIDVIEW ════════════
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 14, 14, 0), BackColor = Color.Transparent };
            _dgv = new DataGridView
            {
                Dock                = DockStyle.Fill,
                AutoGenerateColumns = false,
                RowHeadersVisible   = false,
                AllowUserToAddRows  = false,
                ReadOnly            = true,
                MultiSelect         = false,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle         = BorderStyle.None,
                BackgroundColor     = Color.White,
                GridColor           = Color.FromArgb(228, 236, 248),
                Cursor              = Cursors.Hand
            };
            // Cột dữ liệu
            _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMasv",  DataPropertyName = "Masv",   HeaderText = "Mã SV",      Width = 90  });
            _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colHoten", DataPropertyName = "Hoten",  HeaderText = "Họ và Tên",  AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTuoi",  DataPropertyName = "Tuoi",   HeaderText = "Tuổi",       Width = 55  });
            _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPhai",  DataPropertyName = "Phai",   HeaderText = "Giới tính",  Width = 80  });
            _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMalop", DataPropertyName = "Malop",  HeaderText = "Lớp",        Width = 70  });
            _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDiemTB",DataPropertyName = "DiemTB", HeaderText = "ĐTB",        Width = 65  });
            _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colXepLoai",DataPropertyName= "XepLoai",HeaderText = "Xếp loại",   Width = 85  });

            // Style
            _dgv.ColumnHeadersVisible                = true;
            _dgv.ColumnHeadersHeightSizeMode         = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            _dgv.ColumnHeadersHeight                 = 36;
            _dgv.EnableHeadersVisualStyles           = false;
            _dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 50, 120);
            _dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            _dgv.DefaultCellStyle.Font               = new Font("Segoe UI", 9f);
            _dgv.DefaultCellStyle.BackColor          = Color.White;
            _dgv.DefaultCellStyle.ForeColor          = Color.FromArgb(40, 50, 70);
            _dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 228, 255);
            _dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 30, 80);
            _dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 250, 255);
            _dgv.RowTemplate.Height                  = 30;

            // Màu xếp loại
            _dgv.CellFormatting += (_, e) =>
            {
                if (e.CellStyle != null && _dgv.Columns[e.ColumnIndex]?.Name == "colXepLoai" && e.Value != null)
                {
                    e.CellStyle.Font     = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                    e.CellStyle.ForeColor= e.Value.ToString() switch
                    {
                        "Xuất sắc" => Color.FromArgb(0, 140, 60),
                        "Giỏi"     => Color.FromArgb(0, 100, 210),
                        "Khá"      => Color.FromArgb(180, 100, 0),
                        _          => Color.FromArgb(190, 30, 30)
                    };
                }
            };

            // Double-click → mở thẳng FormThongTinSV
            _dgv.CellDoubleClick += OnDoubleClickRow;

            pnlGrid.Controls.Add(_dgv);

            // ════════════ FOOTER ════════════
            var pnlFooter = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 52,
                BackColor = Color.White,
                Padding   = new Padding(14, 0, 14, 0)
            };
            pnlFooter.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(215, 228, 248));
                e.Graphics.DrawLine(pen, 0, 0, ((Control)s!).Width, 0);
            };

            // Nút "Xem thông tin"
            _btnXemThongTin = new Button
            {
                Text      = "Xem thông tin",
                Size      = new Size(150, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(20, 60, 140),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Enabled   = false,
                Anchor    = AnchorStyles.Right | AnchorStyles.Top,
                Location  = new Point(0, 8)
            };
            _btnXemThongTin.FlatAppearance.BorderSize = 0;
            _btnXemThongTin.Click += OnXemThongTin;
            pnlFooter.Controls.Add(_btnXemThongTin);
            pnlFooter.Resize += (_, _) => _btnXemThongTin.Left = pnlFooter.Width - _btnXemThongTin.Width - 14;

            // Nút "Đóng"
            var btnClose = new Button
            {
                Text      = "Đóng",
                Size      = new Size(100, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 245, 252),
                ForeColor = Color.FromArgb(60, 80, 120),
                Font      = new Font("Segoe UI", 9.5f),
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Right | AnchorStyles.Top,
                Location  = new Point(0, 8)
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(200, 215, 240);
            btnClose.FlatAppearance.BorderSize  = 1;
            btnClose.Click += (_, _) => Close();
            pnlFooter.Controls.Add(btnClose);
            pnlFooter.Resize += (_, _) => btnClose.Left = pnlFooter.Width - _btnXemThongTin.Width - btnClose.Width - 22;

            // Sắp xếp thứ tự Controls.Add để Dock hoạt động chính xác (Fill -> Bottom -> Top)
            Controls.Add(pnlGrid);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);

            // Bật nút khi chọn dòng
            _dgv.SelectionChanged += (_, _) =>
                _btnXemThongTin.Enabled = _dgv.SelectedRows.Count > 0;
        }

        // ─── Nạp dữ liệu vào DataGridView ────────────────────────────────────────
        private void LoadData()
        {
            var displayList = _students.Select(sv =>
            {
                double dtb = sv.Monhoc.Count > 0 ? Math.Round(sv.Monhoc.Average(m => m.Diem), 2) : 0;
                return new
                {
                    sv.Masv,
                    sv.Hoten,
                    sv.Tuoi,
                    sv.Phai,
                    sv.Malop,
                    DiemTB  = dtb,
                    XepLoai = dtb >= 8.5 ? "Xuất sắc"
                            : dtb >= 7.0 ? "Giỏi"
                            : dtb >= 5.5 ? "Khá"
                            : "TB/Yếu"
                };
            }).ToList();

            _dgv.DataSource   = displayList;
            _lblCount.Text    = $"Tìm thấy: {_students.Count} sinh viên";
        }

        // ─── Xem thông tin sinh viên được chọn ───────────────────────────────────
        private void OnXemThongTin(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;

            int rowIdx = _dgv.SelectedRows[0].Index;
            // Lấy Masv từ cell để map sang object SinhVien đầy đủ
            string masv  = _dgv["colMasv", rowIdx].Value?.ToString() ?? "";
            var sv       = _students.FirstOrDefault(s => s.Masv.Equals(masv, StringComparison.OrdinalIgnoreCase));
            if (sv == null) return;

            using var frm = new FormThongTinSV(sv);
            frm.ShowDialog(this);
        }

        // Double-click dòng → mở thẳng FormThongTinSV
        private void OnDoubleClickRow(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _dgv.ClearSelection();
            _dgv.Rows[e.RowIndex].Selected = true;
            OnXemThongTin(null, EventArgs.Empty);
        }
    }
}
