using QLSinhVien.WinForms.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace QLSinhVien.WinForms.Forms
{
    public partial class FormThongTinSV : Form
    {
        private readonly SinhVien _sv;
        private readonly double   _diemTB;
        private readonly string   _xepLoai;

        public FormThongTinSV(SinhVien sv)
        {
            _sv      = sv;
            _diemTB  = sv.Monhoc.Count > 0 ? Math.Round(sv.Monhoc.Average(m => m.Diem), 2) : 0;
            _xepLoai = _diemTB >= 8.5 ? "Xuất sắc"
                     : _diemTB >= 7.0 ? "Giỏi"
                     : _diemTB >= 5.5 ? "Khá"
                     : "TB / Yếu";

            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            // ── Cấu hình Form ──
            Text            = $"Thông tin sinh viên — {_sv.Hoten}";
            Size            = new Size(720, 580);
            MinimumSize     = new Size(640, 500);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.FromArgb(240, 245, 252);
            FormBorderStyle = FormBorderStyle.Sizable;
            Font            = new Font("Segoe UI", 9f);
            Controls.Clear();

            // ════════════ 1. HEADER (Top) ════════════
            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 92,
                BackColor = Color.FromArgb(20, 60, 140)
            };

            // Avatar tròn chữ cái đầu
            var avatarPanel = new Panel { Size = new Size(58, 58), Location = new Point(18, 17), BackColor = Color.Transparent };
            avatarPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var bg   = new SolidBrush(Color.FromArgb(50, 255, 255, 255));
                using var pen  = new Pen(Color.White, 2);
                using var font = new Font("Segoe UI", 20f, FontStyle.Bold);
                e.Graphics.FillEllipse(bg, 0, 0, 57, 57);
                e.Graphics.DrawEllipse(pen, 1, 1, 55, 55);
                string init = _sv.Hoten.Length > 0 ? _sv.Hoten.Split(' ')[^1].Substring(0, 1).ToUpper() : "?";
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(init, font, Brushes.White, new RectangleF(0, 0, 58, 58), sf);
            };
            pnlHeader.Controls.Add(avatarPanel);

            pnlHeader.Controls.Add(new Label
            {
                Text      = _sv.Hoten.ToUpper(),
                Font      = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(88, 18)
            });
            pnlHeader.Controls.Add(new Label
            {
                Text      = $"Mã SV: {_sv.Masv.ToUpper()}   •   Lớp: {_sv.Malop.ToUpper()}   •   {_sv.Phai}   •   {_sv.Tuoi} tuổi",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(180, 210, 255),
                AutoSize  = true,
                Location  = new Point(90, 53)
            });

            // ════════════ 2. FOOTER (Bottom) ════════════
            var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = Color.White };
            pnlFooter.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(220, 230, 245));
                e.Graphics.DrawLine(pen, 0, 0, ((Control)s!).Width, 0);
            };
            var btnClose = new Button
            {
                Text      = "✕  Đóng",
                Size      = new Size(110, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(20, 60, 140),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Location  = new Point(0, 8)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click  += (_, _) => Close();
            pnlFooter.Controls.Add(btnClose);
            pnlFooter.Resize += (_, _) => btnClose.Left = pnlFooter.Width - btnClose.Width - 16;

            // ════════════ 3. CONTENT (Fill) ════════════
            var pnlContent = new Panel
            {
                Dock      = DockStyle.Fill,
                Padding   = new Padding(16, 14, 16, 10),
                BackColor = Color.FromArgb(240, 245, 252)
            };

            // ── Hàng 1: Card ĐTB (trái) + Card Ngoại ngữ (phải) ──
            var row1 = new TableLayoutPanel
            {
                Dock        = DockStyle.Top,
                Height      = 115,
                ColumnCount = 2,
                RowCount    = 1,
                BackColor   = Color.Transparent,
                Margin      = new Padding(0, 0, 0, 10)
            };
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));

            // Card ĐTB
            Color rankColor = _xepLoai switch
            {
                "Xuất sắc" => Color.FromArgb(0, 140, 60),
                "Giỏi"     => Color.FromArgb(0, 100, 210),
                "Khá"      => Color.FromArgb(190, 110, 0),
                _          => Color.FromArgb(190, 30, 30)
            };
            var cardScore = CreateCard();
            var tlpScore = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Color.Transparent
            };
            tlpScore.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tlpScore.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tlpScore.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));
            tlpScore.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));
            tlpScore.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));

            AddScoreRow(tlpScore, 0, "Điểm trung bình:", _diemTB.ToString("F2"), rankColor, true);
            AddScoreRow(tlpScore, 1, "Xếp loại:",        _xepLoai, rankColor, true);
            AddScoreRow(tlpScore, 2, "Số môn học:",       _sv.Monhoc.Count.ToString(), Color.FromArgb(40, 50, 70), false);
            cardScore.Controls.Add(tlpScore);
            row1.Controls.Add(cardScore, 0, 0);

            // Card Ngoại ngữ
            var cardLang = CreateCard();
            var tlpLang = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 1,
                RowCount    = 2,
                BackColor   = Color.Transparent
            };
            tlpLang.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
            tlpLang.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            tlpLang.Controls.Add(new Label
            {
                Text      = "🌐  Ngoại ngữ",
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 60, 140),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 0);

            var flp = new FlowLayoutPanel
            {
                Dock         = DockStyle.Fill,
                BackColor    = Color.Transparent,
                WrapContents = true,
                AutoScroll   = true,
                Padding      = new Padding(0, 2, 0, 0),
                Margin       = new Padding(0)
            };
            if (_sv.Ngoaingu == null || _sv.Ngoaingu.Count == 0)
            {
                flp.Controls.Add(new Label
                {
                    Text      = "Không có",
                    ForeColor = Color.Gray,
                    AutoSize  = true,
                    Font      = new Font("Segoe UI", 9f)
                });
            }
            else
            {
                foreach (var lang in _sv.Ngoaingu)
                    flp.Controls.Add(CreateTag(lang));
            }
            tlpLang.Controls.Add(flp, 0, 1);
            cardLang.Controls.Add(tlpLang);
            row1.Controls.Add(cardLang, 1, 0);

            // ── Tiêu đề môn học ──
            var lblMon = new Label
            {
                Text      = "📚  Bảng điểm các môn học",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 60, 140),
                Dock      = DockStyle.Top,
                Height    = 28
            };

            // ── DataGridView môn học ──
            var dgv = new DataGridView
            {
                Dock                = DockStyle.Fill,
                AutoGenerateColumns = false,
                RowHeadersVisible   = false,
                AllowUserToAddRows  = false,
                ReadOnly            = true,
                BorderStyle         = BorderStyle.None,
                BackgroundColor     = Color.White,
                GridColor           = Color.FromArgb(230, 238, 248),
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Mamon",  HeaderText = "Mã môn",  Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Tenmon", HeaderText = "Tên môn học", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Diem",   HeaderText = "Điểm",    Width = 90  });

            dgv.DefaultCellStyle.Font              = new Font("Segoe UI", 9f);
            dgv.DefaultCellStyle.BackColor         = Color.White;
            dgv.DefaultCellStyle.ForeColor         = Color.FromArgb(40, 50, 70);
            dgv.DefaultCellStyle.SelectionBackColor= Color.FromArgb(220, 235, 255);
            dgv.DefaultCellStyle.SelectionForeColor= Color.FromArgb(20, 30, 60);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 250, 255);

            dgv.ColumnHeadersVisible                = true;
            dgv.ColumnHeadersHeightSizeMode         = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgv.ColumnHeadersHeight                 = 34;
            dgv.EnableHeadersVisualStyles           = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 50, 120);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.RowTemplate.Height                  = 30;

            dgv.CellFormatting += (_, e) =>
            {
                if (e.ColumnIndex == 2 && e.Value is double d)
                {
                    e.CellStyle.Font     = new Font("Segoe UI", 9f, FontStyle.Bold);
                    e.CellStyle.ForeColor= d >= 8.5 ? Color.FromArgb(0, 140, 60)
                                        : d >= 7.0 ? Color.FromArgb(0, 100, 210)
                                        : d >= 5.5 ? Color.FromArgb(180, 100, 0)
                                        :            Color.FromArgb(190, 30, 30);
                }
            };
            dgv.DataSource = _sv.Monhoc;

            // Thứ tự add control vào pnlContent để Dock hoạt động chính xác
            pnlContent.Controls.Add(dgv);    // Fill (add trước)
            pnlContent.Controls.Add(lblMon); // Top
            pnlContent.Controls.Add(row1);   // Top

            // Thứ tự add control vào Form (Fill -> Bottom -> Top để không bao giờ bị đè)
            Controls.Add(pnlContent);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────
        private static Panel CreateCard()
        {
            var p = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White,
                Margin    = new Padding(4),
                Padding   = new Padding(12, 8, 12, 8)
            };
            p.Paint += (s, e) =>
            {
                var c = (Control)s!;
                using var pen = new Pen(Color.FromArgb(215, 228, 248), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            };
            return p;
        }

        private static void AddScoreRow(TableLayoutPanel tlp, int row, string label, string value, Color valColor, bool bold)
        {
            tlp.Controls.Add(new Label
            {
                Text      = label,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 110, 130),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, row);

            tlp.Controls.Add(new Label
            {
                Text      = value,
                Font      = new Font("Segoe UI", 9f, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = valColor,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            }, 1, row);
        }

        private static Label CreateTag(string text) => new Label
        {
            Text        = text,
            AutoSize    = true,
            BackColor   = Color.FromArgb(220, 235, 255),
            ForeColor   = Color.FromArgb(20, 60, 140),
            Font        = new Font("Segoe UI", 9f, FontStyle.Bold),
            Padding     = new Padding(10, 5, 10, 5),
            Margin      = new Padding(4, 3, 6, 3),
            TextAlign   = ContentAlignment.MiddleCenter
        };
    }
}
