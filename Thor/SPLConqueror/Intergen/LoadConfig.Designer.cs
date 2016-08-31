namespace Intergen
{
    partial class LoadConfig
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.newConfig = new System.Windows.Forms.Button();
            this.loadConf = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.exit = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.newConfig, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.loadConf, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.exit, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(20);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(711, 630);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // newConfig
            // 
            this.newConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newConfig.Location = new System.Drawing.Point(40, 90);
            this.newConfig.Margin = new System.Windows.Forms.Padding(20);
            this.newConfig.Name = "newConfig";
            this.newConfig.Size = new System.Drawing.Size(631, 140);
            this.newConfig.TabIndex = 1;
            this.newConfig.Text = "Neue Config erzeugen";
            this.newConfig.UseVisualStyleBackColor = true;
            this.newConfig.Click += new System.EventHandler(this.newConfigEvent);
            // 
            // loadConf
            // 
            this.loadConf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadConf.Location = new System.Drawing.Point(40, 270);
            this.loadConf.Margin = new System.Windows.Forms.Padding(20);
            this.loadConf.Name = "loadConf";
            this.loadConf.Size = new System.Drawing.Size(631, 140);
            this.loadConf.TabIndex = 2;
            this.loadConf.Text = "Lade Configuration";
            this.loadConf.UseVisualStyleBackColor = true;
            this.loadConf.Click += new System.EventHandler(this.loadConfigEvent);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(665, 50);
            this.label1.TabIndex = 3;
            this.label1.Text = "Generator";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exit
            // 
            this.exit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exit.Location = new System.Drawing.Point(40, 450);
            this.exit.Margin = new System.Windows.Forms.Padding(20);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(631, 140);
            this.exit.TabIndex = 4;
            this.exit.Text = "Beenden";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.button3_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // LoadConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 630);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LoadConfig";
            this.Text = "LoadConfig";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button newConfig;
        private System.Windows.Forms.Button loadConf;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}