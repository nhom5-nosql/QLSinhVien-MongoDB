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
            txtMasv = new TextBox();
            txtHoten = new TextBox();
            txtTuoi = new TextBox();
            txtMalop = new TextBox();
            cbPhai = new ComboBox();
            flpNgoaiNgu = new FlowLayoutPanel();
            flpMonHoc = new FlowLayoutPanel();
            btnAddNgoaiNgu = new Button();
            btnAddMonHoc = new Button();
            btnSave = new Button();
            SuspendLayout();
            // 
            // txtMasv
            // 
            txtMasv.BackColor = SystemColors.ActiveCaption;
            txtMasv.Location = new Point(72, 71);
            txtMasv.Name = "txtMasv";
            txtMasv.Size = new Size(150, 27);
            txtMasv.TabIndex = 0;
            txtMasv.Text = "Nhập mã số sinh viên";
            // 
            // txtHoten
            // 
            txtHoten.BackColor = SystemColors.ActiveCaption;
            txtHoten.Location = new Point(72, 114);
            txtHoten.Name = "txtHoten";
            txtHoten.Size = new Size(150, 27);
            txtHoten.TabIndex = 1;
            txtHoten.Text = "Nhập họ tên";
            // 
            // txtTuoi
            // 
            txtTuoi.BackColor = SystemColors.ActiveCaption;
            txtTuoi.Location = new Point(72, 161);
            txtTuoi.Name = "txtTuoi";
            txtTuoi.Size = new Size(150, 27);
            txtTuoi.TabIndex = 2;
            txtTuoi.Text = "Nhập tuổi";
            // 
            // txtMalop
            // 
            txtMalop.BackColor = SystemColors.ActiveCaption;
            txtMalop.Location = new Point(72, 208);
            txtMalop.Name = "txtMalop";
            txtMalop.Size = new Size(150, 27);
            txtMalop.TabIndex = 3;
            txtMalop.Text = "Nhập mã lớp";
            // 
            // cbPhai
            // 
            cbPhai.BackColor = SystemColors.ActiveCaption;
            cbPhai.FormattingEnabled = true;
            cbPhai.Items.AddRange(new object[] { "Nam", "Nữ" });
            cbPhai.Location = new Point(72, 254);
            cbPhai.Name = "cbPhai";
            cbPhai.Size = new Size(150, 28);
            cbPhai.TabIndex = 4;
            cbPhai.Text = "Chọn giới tính";
            // 
            // flpNgoaiNgu
            // 
            flpNgoaiNgu.AutoSize = true;
            flpNgoaiNgu.BackColor = SystemColors.Control;
            flpNgoaiNgu.FlowDirection = FlowDirection.TopDown;
            flpNgoaiNgu.Location = new Point(302, 71);
            flpNgoaiNgu.Name = "flpNgoaiNgu";
            flpNgoaiNgu.Size = new Size(318, 90);
            flpNgoaiNgu.TabIndex = 5;
            // 
            // flpMonHoc
            // 
            flpMonHoc.AutoSize = true;
            flpMonHoc.BackColor = SystemColors.Control;
            flpMonHoc.FlowDirection = FlowDirection.TopDown;
            flpMonHoc.Location = new Point(301, 196);
            flpMonHoc.Name = "flpMonHoc";
            flpMonHoc.Size = new Size(319, 86);
            flpMonHoc.TabIndex = 6;
            flpMonHoc.Paint += flpMonHoc_Paint;
            // 
            // btnAddNgoaiNgu
            // 
            btnAddNgoaiNgu.BackColor = SystemColors.ButtonShadow;
            btnAddNgoaiNgu.Location = new Point(646, 83);
            btnAddNgoaiNgu.Name = "btnAddNgoaiNgu";
            btnAddNgoaiNgu.Size = new Size(120, 45);
            btnAddNgoaiNgu.TabIndex = 7;
            btnAddNgoaiNgu.Text = "[+] Ngoại ngữ";
            btnAddNgoaiNgu.UseVisualStyleBackColor = false;
            // 
            // btnAddMonHoc
            // 
            btnAddMonHoc.BackColor = SystemColors.ButtonShadow;
            btnAddMonHoc.Location = new Point(646, 217);
            btnAddMonHoc.Name = "btnAddMonHoc";
            btnAddMonHoc.Size = new Size(120, 44);
            btnAddMonHoc.TabIndex = 8;
            btnAddMonHoc.Text = "[+] Môn học";
            btnAddMonHoc.UseVisualStyleBackColor = false;
            btnAddMonHoc.Click += btnAddMonHoc_Click_1;
            // 
            // btnSave
            // 
            btnSave.BackColor = SystemColors.ControlDarkDark;
            btnSave.Location = new Point(662, 343);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(85, 45);
            btnSave.TabIndex = 9;
            btnSave.Text = "Lưu";
            btnSave.UseVisualStyleBackColor = false;
            // 
            // FormSinhVienDetail
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSave);
            Controls.Add(btnAddMonHoc);
            Controls.Add(btnAddNgoaiNgu);
            Controls.Add(flpMonHoc);
            Controls.Add(flpNgoaiNgu);
            Controls.Add(cbPhai);
            Controls.Add(txtMalop);
            Controls.Add(txtTuoi);
            Controls.Add(txtHoten);
            Controls.Add(txtMasv);
            Name = "FormSinhVienDetail";
            Text = "Thêm thông tin sinh viên";
            Load += FormSinhVienDetail_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMasv;
        private TextBox txtHoten;
        private TextBox txtTuoi;
        private TextBox txtMalop;
        private ComboBox cbPhai;
        private FlowLayoutPanel flpNgoaiNgu;
        private FlowLayoutPanel flpMonHoc;
        private Button btnAddNgoaiNgu;
        private Button btnAddMonHoc;
        private Button btnSave;
    }
}