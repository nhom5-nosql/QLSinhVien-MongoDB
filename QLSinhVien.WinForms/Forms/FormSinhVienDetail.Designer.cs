namespace QLSinhVien.WinForms
{
    partial class FormSinhVienDetail
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblMasv = new Label();
            txtMasv = new TextBox();
            lblHoten = new Label();
            txtHoten = new TextBox();
            lblTuoi = new Label();
            txtTuoi = new TextBox();
            lblMalop = new Label();
            txtMalop = new TextBox();
            lblPhai = new Label();
            cbPhai = new ComboBox();
            lblNgoaiNgu = new Label();
            flpNgoaiNgu = new FlowLayoutPanel();
            btnAddNgoaiNgu = new Button();
            lblMonHoc = new Label();
            flpMonHoc = new FlowLayoutPanel();
            btnAddMonHoc = new Button();
            btnSave = new Button();
            SuspendLayout();
            // 
            // lblMasv
            // 
            lblMasv.AutoSize = true;
            lblMasv.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMasv.ForeColor = Color.FromArgb(40, 50, 70);
            lblMasv.Location = new Point(18, 30);
            lblMasv.Name = "lblMasv";
            lblMasv.Size = new Size(58, 20);
            lblMasv.TabIndex = 10;
            lblMasv.Text = "Mã SV:";
            // 
            // txtMasv
            // 
            txtMasv.BackColor = SystemColors.Window;
            txtMasv.Location = new Point(95, 26);
            txtMasv.Name = "txtMasv";
            txtMasv.PlaceholderText = "VD: sv001";
            txtMasv.Size = new Size(185, 27);
            txtMasv.TabIndex = 0;
            // 
            // lblHoten
            // 
            lblHoten.AutoSize = true;
            lblHoten.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHoten.ForeColor = Color.FromArgb(40, 50, 70);
            lblHoten.Location = new Point(18, 75);
            lblHoten.Name = "lblHoten";
            lblHoten.Size = new Size(60, 20);
            lblHoten.TabIndex = 11;
            lblHoten.Text = "Họ tên:";
            // 
            // txtHoten
            // 
            txtHoten.BackColor = SystemColors.Window;
            txtHoten.Location = new Point(95, 71);
            txtHoten.Name = "txtHoten";
            txtHoten.PlaceholderText = "VD: Nguyễn Văn A";
            txtHoten.Size = new Size(185, 27);
            txtHoten.TabIndex = 1;
            // 
            // lblTuoi
            // 
            lblTuoi.AutoSize = true;
            lblTuoi.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTuoi.ForeColor = Color.FromArgb(40, 50, 70);
            lblTuoi.Location = new Point(18, 120);
            lblTuoi.Name = "lblTuoi";
            lblTuoi.Size = new Size(44, 20);
            lblTuoi.TabIndex = 12;
            lblTuoi.Text = "Tuổi:";
            // 
            // txtTuoi
            // 
            txtTuoi.BackColor = SystemColors.Window;
            txtTuoi.Location = new Point(95, 116);
            txtTuoi.Name = "txtTuoi";
            txtTuoi.PlaceholderText = "VD: 20";
            txtTuoi.Size = new Size(185, 27);
            txtTuoi.TabIndex = 2;
            // 
            // lblMalop
            // 
            lblMalop.AutoSize = true;
            lblMalop.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMalop.ForeColor = Color.FromArgb(40, 50, 70);
            lblMalop.Location = new Point(18, 165);
            lblMalop.Name = "lblMalop";
            lblMalop.Size = new Size(62, 20);
            lblMalop.TabIndex = 13;
            lblMalop.Text = "Mã lớp:";
            // 
            // txtMalop
            // 
            txtMalop.BackColor = SystemColors.Window;
            txtMalop.Location = new Point(95, 161);
            txtMalop.Name = "txtMalop";
            txtMalop.PlaceholderText = "VD: CNTT01";
            txtMalop.Size = new Size(185, 27);
            txtMalop.TabIndex = 3;
            // 
            // lblPhai
            // 
            lblPhai.AutoSize = true;
            lblPhai.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPhai.ForeColor = Color.FromArgb(40, 50, 70);
            lblPhai.Location = new Point(18, 210);
            lblPhai.Name = "lblPhai";
            lblPhai.Size = new Size(74, 20);
            lblPhai.TabIndex = 14;
            lblPhai.Text = "Giới tính:";
            // 
            // cbPhai
            // 
            cbPhai.BackColor = SystemColors.Window;
            cbPhai.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPhai.FormattingEnabled = true;
            cbPhai.Items.AddRange(new object[] { "Nam", "Nữ" });
            cbPhai.Location = new Point(95, 206);
            cbPhai.Name = "cbPhai";
            cbPhai.Size = new Size(185, 28);
            cbPhai.TabIndex = 4;
            // 
            // lblNgoaiNgu
            // 
            lblNgoaiNgu.AutoSize = true;
            lblNgoaiNgu.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNgoaiNgu.ForeColor = Color.FromArgb(20, 60, 140);
            lblNgoaiNgu.Location = new Point(310, 8);
            lblNgoaiNgu.Name = "lblNgoaiNgu";
            lblNgoaiNgu.Size = new Size(147, 20);
            lblNgoaiNgu.TabIndex = 15;
            lblNgoaiNgu.Text = "Danh sách Ngoại ngữ:";
            // 
            // flpNgoaiNgu
            // 
            flpNgoaiNgu.AutoScroll = true;
            flpNgoaiNgu.BackColor = SystemColors.Window;
            flpNgoaiNgu.BorderStyle = BorderStyle.FixedSingle;
            flpNgoaiNgu.FlowDirection = FlowDirection.TopDown;
            flpNgoaiNgu.Location = new Point(310, 30);
            flpNgoaiNgu.Name = "flpNgoaiNgu";
            flpNgoaiNgu.Size = new Size(350, 120);
            flpNgoaiNgu.TabIndex = 5;
            flpNgoaiNgu.WrapContents = false;
            // 
            // btnAddNgoaiNgu
            // 
            btnAddNgoaiNgu.BackColor = Color.FromArgb(30, 80, 162);
            btnAddNgoaiNgu.Cursor = Cursors.Hand;
            btnAddNgoaiNgu.FlatStyle = FlatStyle.Flat;
            btnAddNgoaiNgu.ForeColor = Color.White;
            btnAddNgoaiNgu.Location = new Point(670, 30);
            btnAddNgoaiNgu.Name = "btnAddNgoaiNgu";
            btnAddNgoaiNgu.Size = new Size(125, 36);
            btnAddNgoaiNgu.TabIndex = 7;
            btnAddNgoaiNgu.Text = "[+] Ngoại ngữ";
            btnAddNgoaiNgu.UseVisualStyleBackColor = false;
            btnAddNgoaiNgu.Click += btnAddNgoaiNgu_Click;
            // 
            // lblMonHoc
            // 
            lblMonHoc.AutoSize = true;
            lblMonHoc.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMonHoc.ForeColor = Color.FromArgb(20, 60, 140);
            lblMonHoc.Location = new Point(310, 162);
            lblMonHoc.Name = "lblMonHoc";
            lblMonHoc.Size = new Size(198, 20);
            lblMonHoc.TabIndex = 16;
            lblMonHoc.Text = "Danh sách Môn học && Điểm:";
            // 
            // flpMonHoc
            // 
            flpMonHoc.AutoScroll = true;
            flpMonHoc.BackColor = SystemColors.Window;
            flpMonHoc.BorderStyle = BorderStyle.FixedSingle;
            flpMonHoc.FlowDirection = FlowDirection.TopDown;
            flpMonHoc.Location = new Point(310, 185);
            flpMonHoc.Name = "flpMonHoc";
            flpMonHoc.Size = new Size(350, 180);
            flpMonHoc.TabIndex = 6;
            flpMonHoc.WrapContents = false;
            // 
            // btnAddMonHoc
            // 
            btnAddMonHoc.BackColor = Color.FromArgb(30, 80, 162);
            btnAddMonHoc.Cursor = Cursors.Hand;
            btnAddMonHoc.FlatStyle = FlatStyle.Flat;
            btnAddMonHoc.ForeColor = Color.White;
            btnAddMonHoc.Location = new Point(670, 185);
            btnAddMonHoc.Name = "btnAddMonHoc";
            btnAddMonHoc.Size = new Size(125, 36);
            btnAddMonHoc.TabIndex = 8;
            btnAddMonHoc.Text = "[+] Môn học";
            btnAddMonHoc.UseVisualStyleBackColor = false;
            btnAddMonHoc.Click += btnAddMonHoc_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(0, 140, 60);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(670, 316);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(125, 48);
            btnSave.TabIndex = 9;
            btnSave.Text = "💾 Lưu";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // FormSinhVienDetail
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 248, 253);
            ClientSize = new Size(815, 385);
            Controls.Add(lblNgoaiNgu);
            Controls.Add(lblMonHoc);
            Controls.Add(lblMasv);
            Controls.Add(txtMasv);
            Controls.Add(lblHoten);
            Controls.Add(txtHoten);
            Controls.Add(lblTuoi);
            Controls.Add(txtTuoi);
            Controls.Add(lblMalop);
            Controls.Add(txtMalop);
            Controls.Add(lblPhai);
            Controls.Add(cbPhai);
            Controls.Add(flpNgoaiNgu);
            Controls.Add(btnAddNgoaiNgu);
            Controls.Add(flpMonHoc);
            Controls.Add(btnAddMonHoc);
            Controls.Add(btnSave);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSinhVienDetail";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Thêm thông tin sinh viên";
            Load += FormSinhVienDetail_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMasv;
        private TextBox txtMasv;
        private Label lblHoten;
        private TextBox txtHoten;
        private Label lblTuoi;
        private TextBox txtTuoi;
        private Label lblMalop;
        private TextBox txtMalop;
        private Label lblPhai;
        private ComboBox cbPhai;
        private Label lblNgoaiNgu;
        private FlowLayoutPanel flpNgoaiNgu;
        private Label lblMonHoc;
        private FlowLayoutPanel flpMonHoc;
        private Button btnAddNgoaiNgu;
        private Button btnAddMonHoc;
        private Button btnSave;
    }
}