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
            btnSearch = new Button();
            txtSearch = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvSinhVien).BeginInit();
            SuspendLayout();
            // 
            // dgvSinhVien
            // 
            dgvSinhVien.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvSinhVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSinhVien.Location = new Point(1, 2);
            dgvSinhVien.Name = "dgvSinhVien";
            dgvSinhVien.RowHeadersWidth = 51;
            dgvSinhVien.Size = new Size(917, 355);
            dgvSinhVien.TabIndex = 0;
            dgvSinhVien.CellContentClick += dataGridView1_CellContentClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(252, 371);
            label1.Name = "label1";
            label1.Size = new Size(0, 20);
            label1.TabIndex = 5;
            label1.Click += label1_Click;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Bottom;
            btnDelete.BackColor = SystemColors.GradientActiveCaption;
            btnDelete.Location = new Point(608, 387);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(110, 29);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnThem
            // 
            btnThem.Anchor = AnchorStyles.Bottom;
            btnThem.BackColor = SystemColors.GradientActiveCaption;
            btnThem.Location = new Point(492, 387);
            btnThem.Name = "btnThem";
            btnThem.Size = new Size(110, 29);
            btnThem.TabIndex = 3;
            btnThem.Text = "Thêm";
            btnThem.UseVisualStyleBackColor = false;
            btnThem.Click += btnThem_Click;
            // 
            // btnSearch
            // 
            btnSearch.Anchor = AnchorStyles.Bottom;
            btnSearch.BackColor = SystemColors.GradientActiveCaption;
            btnSearch.Location = new Point(378, 387);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(110, 27);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Bottom;
            txtSearch.BackColor = SystemColors.InactiveCaption;
            txtSearch.Location = new Point(184, 387);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(188, 27);
            txtSearch.TabIndex = 1;
            txtSearch.TextAlign = HorizontalAlignment.Center;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(921, 446);
            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
            Controls.Add(label1);
            Controls.Add(btnThem);
            Controls.Add(dgvSinhVien);
            Controls.Add(btnDelete);
            Name = "FormMain";
            Text = "Quản lí sinh viên";
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
        private Button btnSearch;
        private TextBox txtSearch;
    }
}
