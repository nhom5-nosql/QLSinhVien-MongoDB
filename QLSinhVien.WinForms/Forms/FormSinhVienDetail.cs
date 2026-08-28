using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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

        // Bộ nhớ cache danh sách đã có từ MongoDB
        private List<string> _dbLanguages = new();
        private List<MonHoc> _dbSubjects = new();

        // Nguồn dữ liệu Autocomplete / Suggestion
        private readonly AutoCompleteStringCollection _autoNgoaiNgu = new();
        private readonly AutoCompleteStringCollection _autoMaMon = new();
        private readonly AutoCompleteStringCollection _autoTenMon = new();

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

        private async void FormSinhVienDetail_Load(object sender, EventArgs e)
        {
            // 1. Tải danh mục ngoại ngữ & môn học đã có trong CSDL để phục vụ gợi ý (trigger search)
            await LoadMasterDataSuggestionsAsync();

            // 2. Nạp dữ liệu cũ nếu ở chế độ Sửa (Edit)
            if (_isEditMode && _editingSv != null)
            {
                Text = $"Cập nhật thông tin sinh viên — {_editingSv.Masv}";
                txtMasv.Text = _editingSv.Masv;
                txtMasv.Enabled = false; // Mã SV không được đổi, không cho trỏ vào
                txtMasv.ReadOnly = true;
                txtMasv.TabStop = false;
                txtMasv.BackColor = Color.FromArgb(238, 242, 246);
                txtMasv.Font = new Font(txtMasv.Font, FontStyle.Bold);
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

                ActiveControl = txtHoten;
            }
            else
            {
                Text = "Thêm mới sinh viên";
                txtMasv.Enabled = false; // Tự động sinh mã, không cho trỏ vào / chỉnh sửa
                txtMasv.ReadOnly = true;
                txtMasv.TabStop = false;
                txtMasv.BackColor = Color.FromArgb(238, 242, 246);
                txtMasv.Font = new Font(txtMasv.Font, FontStyle.Bold);
                txtMasv.PlaceholderText = "Đang tạo mã...";

                // Tự động sinh mã sinh viên từ MongoDB
                try
                {
                    txtMasv.Text = await _repo.GenerateNextMasvAsync();
                }
                catch
                {
                    txtMasv.Text = "SV001";
                }

                if (cbPhai.SelectedIndex < 0)
                    cbPhai.SelectedIndex = 0;

                ActiveControl = txtHoten;
                txtHoten.Focus();
            }

            // Đồng bộ dữ liệu gợi ý với các dòng đang hiển thị
            RefreshSuggestions();
        }

        // Tải danh sách Ngoại ngữ & Môn học từ DB
        private async Task LoadMasterDataSuggestionsAsync()
        {
            try
            {
                _dbLanguages = await _repo.GetAllDistinctLanguagesAsync();
                _dbSubjects = await _repo.GetAllDistinctSubjectsAsync();
                RefreshSuggestions();
            }
            catch
            {
                // Không chặn người dùng nếu mạng/DB bận lúc tải gợi ý
            }
        }

        // Cập nhật lại toàn bộ danh sách AutoComplete (DB + Dữ liệu người dùng vừa nhập)
        private void RefreshSuggestions()
        {
            // 1. Ngoại ngữ
            var allLangs = new HashSet<string>(_dbLanguages, StringComparer.OrdinalIgnoreCase);
            foreach (Control row in flpNgoaiNgu.Controls)
            {
                if (row is FlowLayoutPanel panel)
                {
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is TextBox txt && !string.IsNullOrWhiteSpace(txt.Text))
                            allLangs.Add(txt.Text.Trim());
                    }
                }
            }
            _autoNgoaiNgu.Clear();
            _autoNgoaiNgu.AddRange(allLangs.OrderBy(s => s).ToArray());

            // 2. Môn học (Mã môn & Tên môn)
            var allMaMon = new HashSet<string>(_dbSubjects.Select(s => s.Mamon).Where(s => !string.IsNullOrWhiteSpace(s)), StringComparer.OrdinalIgnoreCase);
            var allTenMon = new HashSet<string>(_dbSubjects.Select(s => s.Tenmon).Where(s => !string.IsNullOrWhiteSpace(s)), StringComparer.OrdinalIgnoreCase);

            foreach (Control row in flpMonHoc.Controls)
            {
                if (row is FlowLayoutPanel panel)
                {
                    string ma = "", ten = "";
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is TextBox txt)
                        {
                            if (string.IsNullOrEmpty(ma)) ma = txt.Text.Trim();
                            else ten = txt.Text.Trim();
                        }
                    }
                    if (!string.IsNullOrEmpty(ma)) allMaMon.Add(ma);
                    if (!string.IsNullOrEmpty(ten)) allTenMon.Add(ten);
                }
            }

            _autoMaMon.Clear();
            _autoMaMon.AddRange(allMaMon.OrderBy(s => s).ToArray());

            _autoTenMon.Clear();
            _autoTenMon.AddRange(allTenMon.OrderBy(s => s).ToArray());
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
                Text = value,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.CustomSource,
                AutoCompleteCustomSource = _autoNgoaiNgu
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
            btnRemove.Click += (s, ev) =>
            {
                flpNgoaiNgu.Controls.Remove(rowPanel);
                RefreshSuggestions();
            };

            rowPanel.Controls.Add(txtNgoaiNgu);
            rowPanel.Controls.Add(btnRemove);
            flpNgoaiNgu.Controls.Add(rowPanel);
            txtNgoaiNgu.Focus();
        }

        // 2. Nút [+] Thêm môn học động (Mã môn, Tên môn, Điểm)
        private void btnAddMonHoc_Click(object sender, EventArgs e)
        {
            AddMonHocRow();
            RefreshSuggestions();
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

            var txtMaMon = new TextBox
            {
                Width = 70,
                PlaceholderText = "Mã môn",
                Text = mamon,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.CustomSource,
                AutoCompleteCustomSource = _autoMaMon
            };

            var txtTenMon = new TextBox
            {
                Width = 110,
                PlaceholderText = "Tên môn",
                Text = tenmon,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.CustomSource,
                AutoCompleteCustomSource = _autoTenMon
            };

            // Tự động điền Mã môn khi chọn / gõ Tên môn có sẵn trong CSDL
            txtTenMon.Leave += (s, ev) =>
            {
                string inputTen = txtTenMon.Text.Trim();
                if (!string.IsNullOrEmpty(inputTen) && string.IsNullOrEmpty(txtMaMon.Text.Trim()))
                {
                    var match = _dbSubjects.FirstOrDefault(m => string.Equals(m.Tenmon, inputTen, StringComparison.OrdinalIgnoreCase));
                    if (match != null && !string.IsNullOrEmpty(match.Mamon))
                    {
                        txtMaMon.Text = match.Mamon;
                    }
                }
            };

            // Tự động điền Tên môn khi chọn / gõ Mã môn có sẵn trong CSDL
            txtMaMon.Leave += (s, ev) =>
            {
                string inputMa = txtMaMon.Text.Trim();
                if (!string.IsNullOrEmpty(inputMa) && string.IsNullOrEmpty(txtTenMon.Text.Trim()))
                {
                    var match = _dbSubjects.FirstOrDefault(m => string.Equals(m.Mamon, inputMa, StringComparison.OrdinalIgnoreCase));
                    if (match != null && !string.IsNullOrEmpty(match.Tenmon))
                    {
                        txtTenMon.Text = match.Tenmon;
                    }
                }
            };

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
            btnRemove.Click += (s, ev) =>
            {
                flpMonHoc.Controls.Remove(rowPanel);
                RefreshSuggestions();
            };

            rowPanel.Controls.Add(txtMaMon);
            rowPanel.Controls.Add(txtTenMon);
            rowPanel.Controls.Add(numDiem);
            rowPanel.Controls.Add(btnRemove);
            flpMonHoc.Controls.Add(rowPanel);
            txtMaMon.Focus();
        }

        // 3. Nút Lưu dữ liệu vào MongoDB với RÀNG BUỘC & VALIDATION
        private async void btnSave_Click(object sender, EventArgs e)
        {
            string masv = txtMasv.Text.Trim();
            string hoten = txtHoten.Text.Trim();
            string malop = txtMalop.Text.Trim();
            string phai = cbPhai.Text.Trim();

            // ── Validate thông tin cơ bản ──
            if (string.IsNullOrWhiteSpace(masv))
            {
                if (!_isEditMode)
                {
                    masv = await _repo.GenerateNextMasvAsync();
                    txtMasv.Text = masv;
                }
                else
                {
                    MessageBox.Show("Mã sinh viên không hợp lệ!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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

                // ── Ràng buộc và lấy Ngoại ngữ ──
                var seenLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (Control row in flpNgoaiNgu.Controls)
                {
                    if (row is FlowLayoutPanel panel)
                    {
                        foreach (Control ctrl in panel.Controls)
                        {
                            if (ctrl is TextBox txt)
                            {
                                string lang = txt.Text.Trim();
                                if (string.IsNullOrEmpty(lang))
                                    continue; // Bỏ qua dòng rỗng

                                // Ràng buộc chống trùng lặp Ngoại ngữ trong 1 sinh viên
                                if (seenLanguages.Contains(lang))
                                {
                                    MessageBox.Show(
                                        $"Ngoại ngữ '{lang}' bị trùng lặp! Mỗi ngoại ngữ chỉ được thêm 1 lần cho sinh viên.",
                                        "Trùng lặp ngoại ngữ",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                    txt.Focus();
                                    return;
                                }

                                seenLanguages.Add(lang);
                                sv.Ngoaingu.Add(lang);
                            }
                        }
                    }
                }

                // ── Ràng buộc và lấy Môn học & Điểm ──
                var seenSubjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (Control row in flpMonHoc.Controls)
                {
                    if (row is FlowLayoutPanel panel)
                    {
                        string mamon = "", tenmon = "";
                        double diem = 0;
                        TextBox? txtMa = null;
                        TextBox? txtTen = null;

                        foreach (Control ctrl in panel.Controls)
                        {
                            if (ctrl is TextBox txt)
                            {
                                if (txtMa == null) { txtMa = txt; mamon = txt.Text.Trim(); }
                                else { txtTen = txt; tenmon = txt.Text.Trim(); }
                            }
                            else if (ctrl is NumericUpDown num)
                            {
                                diem = (double)num.Value;
                            }
                        }

                        // Nếu dòng hoàn toàn để trống thì bỏ qua
                        if (string.IsNullOrEmpty(mamon) && string.IsNullOrEmpty(tenmon))
                            continue;

                        // Ràng buộc phải nhập đủ cả Mã môn và Tên môn
                        if (string.IsNullOrEmpty(mamon))
                        {
                            MessageBox.Show($"Vui lòng nhập Mã môn cho môn '{tenmon}'!", "Thiếu thông tin môn học", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtMa?.Focus();
                            return;
                        }

                        if (string.IsNullOrEmpty(tenmon))
                        {
                            MessageBox.Show($"Vui lòng nhập Tên môn cho mã môn '{mamon}'!", "Thiếu thông tin môn học", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtTen?.Focus();
                            return;
                        }

                        // Ràng buộc chống trùng lặp Mã môn học trong cùng 1 sinh viên
                        if (seenSubjects.Contains(mamon))
                        {
                            MessageBox.Show(
                                $"Mã môn học '{mamon}' bị trùng lặp! Mỗi môn học chỉ được đăng ký 1 lần cho sinh viên.",
                                "Trùng lặp môn học",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            txtMa?.Focus();
                            return;
                        }

                        // Ràng buộc điểm từ 0 đến 10
                        if (diem < 0 || diem > 10)
                        {
                            MessageBox.Show($"Điểm môn '{tenmon}' ({diem}) không hợp lệ! Điểm phải từ 0 đến 10.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        seenSubjects.Add(mamon);
                        sv.Monhoc.Add(new MonHoc { Mamon = mamon, Tenmon = tenmon, Diem = diem });
                    }
                }

                // ── Lưu vào MongoDB ──
                if (_isEditMode)
                {
                    await _repo.UpdateFullAsync(sv.Masv, sv);
                    MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Kiểm tra xem mã SV đã tồn tại chưa, nếu có xung đột thì tự động lấy mã mới tiếp theo
                    var existing = await _repo.FindByMasvExactAsync(sv.Masv);
                    if (existing != null)
                    {
                        sv.Masv = await _repo.GenerateNextMasvAsync();
                        txtMasv.Text = sv.Masv;
                    }

                    await _repo.InsertAsync(sv);
                    MessageBox.Show($"Thêm sinh viên mới thành công (Mã SV: {sv.Masv})!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
