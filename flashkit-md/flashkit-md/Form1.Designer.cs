namespace flashkit_md
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_rd_rom = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_wr_rom = new System.Windows.Forms.Button();
            this.btn_check = new System.Windows.Forms.Button();
            this.btn_wr_ram = new System.Windows.Forms.Button();
            this.btn_rd_ram = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.consoleBox = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_rd_rom
            // 
            this.btn_rd_rom.Location = new System.Drawing.Point(6, 19);
            this.btn_rd_rom.Name = "btn_rd_rom";
            this.btn_rd_rom.Size = new System.Drawing.Size(205, 51);
            this.btn_rd_rom.TabIndex = 0;
            this.btn_rd_rom.Text = "Read ROM";
            this.btn_rd_rom.UseVisualStyleBackColor = true;
            this.btn_rd_rom.Click += new System.EventHandler(this.btn_rd_rom_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_wr_rom);
            this.groupBox1.Controls.Add(this.btn_rd_rom);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 144);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ROM";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btn_wr_rom
            // 
            this.btn_wr_rom.Location = new System.Drawing.Point(6, 76);
            this.btn_wr_rom.Name = "btn_wr_rom";
            this.btn_wr_rom.Size = new System.Drawing.Size(205, 51);
            this.btn_wr_rom.TabIndex = 1;
            this.btn_wr_rom.Text = "Write ROM";
            this.btn_wr_rom.UseVisualStyleBackColor = true;
            this.btn_wr_rom.Click += new System.EventHandler(this.btn_wr_rom_Click);
            // 
            // btn_check
            // 
            this.btn_check.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_check.Location = new System.Drawing.Point(12, 312);
            this.btn_check.Name = "btn_check";
            this.btn_check.Size = new System.Drawing.Size(217, 68);
            this.btn_check.TabIndex = 4;
            this.btn_check.Text = "Cart info";
            this.btn_check.UseVisualStyleBackColor = false;
            this.btn_check.Click += new System.EventHandler(this.btn_check_Click);
            // 
            // btn_wr_ram
            // 
            this.btn_wr_ram.Location = new System.Drawing.Point(6, 76);
            this.btn_wr_ram.Name = "btn_wr_ram";
            this.btn_wr_ram.Size = new System.Drawing.Size(205, 51);
            this.btn_wr_ram.TabIndex = 3;
            this.btn_wr_ram.Text = "Write RAM";
            this.btn_wr_ram.UseVisualStyleBackColor = true;
            this.btn_wr_ram.Click += new System.EventHandler(this.btn_wr_ram_Click);
            // 
            // btn_rd_ram
            // 
            this.btn_rd_ram.Location = new System.Drawing.Point(6, 19);
            this.btn_rd_ram.Name = "btn_rd_ram";
            this.btn_rd_ram.Size = new System.Drawing.Size(205, 51);
            this.btn_rd_ram.TabIndex = 2;
            this.btn_rd_ram.Text = "Read RAM";
            this.btn_rd_ram.UseVisualStyleBackColor = true;
            this.btn_rd_ram.Click += new System.EventHandler(this.btn_rd_ram_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 386);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(701, 56);
            this.progressBar1.TabIndex = 3;
            // 
            // consoleBox
            // 
            this.consoleBox.BackColor = System.Drawing.SystemColors.Control;
            this.consoleBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.consoleBox.Location = new System.Drawing.Point(239, 12);
            this.consoleBox.Multiline = true;
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ReadOnly = true;
            this.consoleBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleBox.Size = new System.Drawing.Size(474, 368);
            this.consoleBox.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_wr_ram);
            this.groupBox2.Controls.Add(this.btn_rd_ram);
            this.groupBox2.Location = new System.Drawing.Point(12, 162);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(217, 144);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RAM";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 451);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_check);
            this.Controls.Add(this.consoleBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Flashkit-md";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_rd_rom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_wr_rom;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_check;
        private System.Windows.Forms.Button btn_wr_ram;
        private System.Windows.Forms.Button btn_rd_ram;
        private System.Windows.Forms.TextBox consoleBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

