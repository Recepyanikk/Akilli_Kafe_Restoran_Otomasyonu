namespace WindowsFormsApp1
{
    partial class SiparişlerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvSiparisler = new System.Windows.Forms.DataGridView();
            this.MasanoLabel = new System.Windows.Forms.Label();
            this.flpKategoriler = new System.Windows.Forms.FlowLayoutPanel();
            this.flpUrunler = new System.Windows.Forms.FlowLayoutPanel();
            this.lblToplamTutar = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SiparisOnaylaBtn = new System.Windows.Forms.Button();
            this.Masalbl = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.SiparisUrunIptalBtn = new System.Windows.Forms.Button();
            this.SiparisOdemeAlBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSiparisler)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSiparisler
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvSiparisler.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSiparisler.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.dgvSiparisler.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSiparisler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSiparisler.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSiparisler.EnableHeadersVisualStyles = false;
            this.dgvSiparisler.Location = new System.Drawing.Point(837, 0);
            this.dgvSiparisler.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvSiparisler.Name = "dgvSiparisler";
            this.dgvSiparisler.RowHeadersVisible = false;
            this.dgvSiparisler.RowHeadersWidth = 51;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.dgvSiparisler.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSiparisler.RowTemplate.Height = 24;
            this.dgvSiparisler.Size = new System.Drawing.Size(481, 505);
            this.dgvSiparisler.TabIndex = 0;
            this.dgvSiparisler.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // MasanoLabel
            // 
            this.MasanoLabel.AutoSize = true;
            this.MasanoLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.MasanoLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.MasanoLabel.Location = new System.Drawing.Point(12, 11);
            this.MasanoLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MasanoLabel.Name = "MasanoLabel";
            this.MasanoLabel.Size = new System.Drawing.Size(57, 21);
            this.MasanoLabel.TabIndex = 0;
            this.MasanoLabel.Text = "label1";
            // 
            // flpKategoriler
            // 
            this.flpKategoriler.AutoScroll = true;
            this.flpKategoriler.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.flpKategoriler.Dock = System.Windows.Forms.DockStyle.Left;
            this.flpKategoriler.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpKategoriler.Location = new System.Drawing.Point(0, 0);
            this.flpKategoriler.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpKategoriler.Name = "flpKategoriler";
            this.flpKategoriler.Size = new System.Drawing.Size(176, 559);
            this.flpKategoriler.TabIndex = 1;
            this.flpKategoriler.WrapContents = false;
            // 
            // flpUrunler
            // 
            this.flpUrunler.AutoScroll = true;
            this.flpUrunler.Location = new System.Drawing.Point(172, 0);
            this.flpUrunler.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpUrunler.Name = "flpUrunler";
            this.flpUrunler.Size = new System.Drawing.Size(672, 505);
            this.flpUrunler.TabIndex = 2;
            // 
            // lblToplamTutar
            // 
            this.lblToplamTutar.AutoSize = true;
            this.lblToplamTutar.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblToplamTutar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblToplamTutar.Location = new System.Drawing.Point(176, 18);
            this.lblToplamTutar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblToplamTutar.Name = "lblToplamTutar";
            this.lblToplamTutar.Size = new System.Drawing.Size(50, 19);
            this.lblToplamTutar.TabIndex = 0;
            this.lblToplamTutar.Text = "label1";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.panel2.Controls.Add(this.SiparisOnaylaBtn);
            this.panel2.Controls.Add(this.Masalbl);
            this.panel2.Controls.Add(this.lblToplamTutar);
            this.panel2.Location = new System.Drawing.Point(837, 502);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(481, 57);
            this.panel2.TabIndex = 3;
            // 
            // SiparisOnaylaBtn
            // 
            this.SiparisOnaylaBtn.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.ony;
            this.SiparisOnaylaBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SiparisOnaylaBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SiparisOnaylaBtn.Location = new System.Drawing.Point(365, 7);
            this.SiparisOnaylaBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SiparisOnaylaBtn.Name = "SiparisOnaylaBtn";
            this.SiparisOnaylaBtn.Size = new System.Drawing.Size(38, 41);
            this.SiparisOnaylaBtn.TabIndex = 2;
            this.SiparisOnaylaBtn.UseVisualStyleBackColor = true;
            this.SiparisOnaylaBtn.Click += new System.EventHandler(this.SiparisOnaylaBtn_Click);
            // 
            // Masalbl
            // 
            this.Masalbl.AutoSize = true;
            this.Masalbl.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Masalbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Masalbl.Location = new System.Drawing.Point(13, 18);
            this.Masalbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Masalbl.Name = "Masalbl";
            this.Masalbl.Size = new System.Drawing.Size(50, 19);
            this.Masalbl.TabIndex = 1;
            this.Masalbl.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.SiparisUrunIptalBtn);
            this.panel1.Controls.Add(this.SiparisOdemeAlBtn);
            this.panel1.Location = new System.Drawing.Point(172, 501);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(674, 58);
            this.panel1.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.Çkş_yap;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(458, 0);
            this.button3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(178, 58);
            this.button3.TabIndex = 3;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SiparisUrunIptalBtn
            // 
            this.SiparisUrunIptalBtn.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.Ürün_İptali;
            this.SiparisUrunIptalBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SiparisUrunIptalBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SiparisUrunIptalBtn.Location = new System.Drawing.Point(248, 1);
            this.SiparisUrunIptalBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SiparisUrunIptalBtn.Name = "SiparisUrunIptalBtn";
            this.SiparisUrunIptalBtn.Size = new System.Drawing.Size(178, 58);
            this.SiparisUrunIptalBtn.TabIndex = 2;
            this.SiparisUrunIptalBtn.UseVisualStyleBackColor = true;
            this.SiparisUrunIptalBtn.Click += new System.EventHandler(this.SiparisUrunIptalBtn_Click);
            // 
            // SiparisOdemeAlBtn
            // 
            this.SiparisOdemeAlBtn.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.Ödeme_Al;
            this.SiparisOdemeAlBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SiparisOdemeAlBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SiparisOdemeAlBtn.Location = new System.Drawing.Point(33, 1);
            this.SiparisOdemeAlBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SiparisOdemeAlBtn.Name = "SiparisOdemeAlBtn";
            this.SiparisOdemeAlBtn.Size = new System.Drawing.Size(178, 58);
            this.SiparisOdemeAlBtn.TabIndex = 1;
            this.SiparisOdemeAlBtn.UseVisualStyleBackColor = true;
            this.SiparisOdemeAlBtn.Click += new System.EventHandler(this.SiparisOdemeAlBtn_Click);
            // 
            // SiparişlerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1318, 559);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.flpUrunler);
            this.Controls.Add(this.flpKategoriler);
            this.Controls.Add(this.dgvSiparisler);
            this.Controls.Add(this.MasanoLabel);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SiparişlerForm";
            this.Text = "SiparişlerForm";
            this.Load += new System.EventHandler(this.SiparişlerForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSiparisler)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvSiparisler;
        private System.Windows.Forms.Label MasanoLabel;
        private System.Windows.Forms.FlowLayoutPanel flpKategoriler;
        private System.Windows.Forms.FlowLayoutPanel flpUrunler;
        private System.Windows.Forms.Label lblToplamTutar;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button SiparisOdemeAlBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Masalbl;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button SiparisUrunIptalBtn;
        private System.Windows.Forms.Button SiparisOnaylaBtn;
    }
}