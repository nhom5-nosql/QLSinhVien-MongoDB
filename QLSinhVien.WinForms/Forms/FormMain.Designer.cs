namespace QLSinhVien.WinForms
{
    public partial class FormMain : Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvSinhVien = new DataGridView();
            label1 = new Label();
            btnDelete = new Button();
            btnThem = new Button();
            btnSua = new Button();
            btnSearch = new Button();
            txtSearch = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvSinhVien).BeginInit();
            SuspendLayout();
            // 
            // dgvSinhVien
            // 
            dgvSinhVien.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvSinhVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSinhVien.Location = new Point(12, 12);
            dgvSinhVien.Name = "dgvSinhVien";
            dgvSinhVien.RowHeadersWidth = 40;
            dgvSinhVien.Size = new Size(897, 365);
            dgvSinhVien.TabIndex = 0;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label1.ForeColor = Color.FromArgb(80, 80, 80);
            label1.Location = new Point(12, 392);
            label1.Name = "label1";
            label1.Size = new Size(130, 20);
            label1.TabIndex = 5;
            label1.Text = "Tổng: 0 sinh viên";
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtSearch.BackColor = SystemColors.Window;
            txtSearch.Location = new Point(200, 388);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Nhập mã SV hoặc họ tên...";
            txtSearch.Size = new Size(220, 27);
            txtSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            btnSearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSearch.BackColor = Color.FromArgb(30, 80, 162);
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(428, 386);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 31);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "🔍 Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnThem
            // 
            btnThem.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnThem.BackColor = Color.FromArgb(0, 140, 60);
            btnThem.Cursor = Cursors.Hand;
            btnThem.FlatStyle = FlatStyle.Flat;
            btnThem.ForeColor = Color.White;
            btnThem.Location = new Point(540, 386);
            btnThem.Name = "btnThem";
            btnThem.Size = new Size(110, 31);
            btnThem.TabIndex = 3;
            btnThem.Text = "➕ Thêm mới";
            btnThem.UseVisualStyleBackColor = false;
            btnThem.Click += btnThem_Click;
            // 
            // btnSua
            // 
            btnSua.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSua.BackColor = Color.FromArgb(230, 140, 20);
            btnSua.Cursor = Cursors.Hand;
            btnSua.FlatStyle = FlatStyle.Flat;
            btnSua.ForeColor = Color.White;
            btnSua.Location = new Point(660, 386);
            btnSua.Name = "btnSua";
            btnSua.Size = new Size(100, 31);
            btnSua.TabIndex = 4;
            btnSua.Text = "✏️ Sửa";
            btnSua.UseVisualStyleBackColor = false;
            btnSua.Click += btnSua_Click;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(770, 386);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(100, 31);
            btnDelete.TabIndex = 5;
            btnDelete.Text = "🗑️ Xóa";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(921, 430);
            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
            Controls.Add(btnThem);
            Controls.Add(btnSua);
            Controls.Add(btnDelete);
            Controls.Add(label1);
            Controls.Add(dgvSinhVien);
            Name = "FormMain";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Quản lý Sinh Viên (CRUD MongoDB)";
            Load += FormMain_Load;
            ((System.ComponentModel.ISupportInitialize)dgvSinhVien).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvSinhVien;
        private Label label1;
        private Button btnDelete;
        private Button btnThem;
        private Button btnSua;
        private Button btnSearch;
        private TextBox txtSearch;
    }
}
