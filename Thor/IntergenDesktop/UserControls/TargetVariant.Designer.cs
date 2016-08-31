namespace IntergenDesktop.UserControls
{
    partial class TargetVariant
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
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.label41 = new System.Windows.Forms.Label();
            this.TargetPropertyBox = new System.Windows.Forms.ComboBox();
            this.label42 = new System.Windows.Forms.Label();
            this.TargetValueBox = new System.Windows.Forms.ComboBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.label41, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.TargetPropertyBox, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.label42, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.TargetValueBox, 1, 1);
            this.tableLayoutPanel10.Controls.Add(this.pictureBox5, 0, 3);
            this.tableLayoutPanel10.Controls.Add(this.button1, 1, 4);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 5;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(1249, 534);
            this.tableLayoutPanel10.TabIndex = 1;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(4, 0);
            this.label41.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(93, 17);
            this.label41.TabIndex = 0;
            this.label41.Text = "NFP Property";
            // 
            // TargetPropertyBox
            // 
            this.TargetPropertyBox.FormattingEnabled = true;
            this.TargetPropertyBox.Location = new System.Drawing.Point(628, 4);
            this.TargetPropertyBox.Margin = new System.Windows.Forms.Padding(4);
            this.TargetPropertyBox.Name = "TargetPropertyBox";
            this.TargetPropertyBox.Size = new System.Drawing.Size(160, 24);
            this.TargetPropertyBox.TabIndex = 1;
            this.TargetPropertyBox.SelectedIndexChanged += new System.EventHandler(this.TargetPropertyBox_SelectedIndexChanged);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(4, 39);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(128, 17);
            this.label42.TabIndex = 2;
            this.label42.Text = "Variant Distribution";
            // 
            // TargetValueBox
            // 
            this.TargetValueBox.FormattingEnabled = true;
            this.TargetValueBox.Location = new System.Drawing.Point(628, 43);
            this.TargetValueBox.Margin = new System.Windows.Forms.Padding(4);
            this.TargetValueBox.Name = "TargetValueBox";
            this.TargetValueBox.Size = new System.Drawing.Size(160, 24);
            this.TargetValueBox.TabIndex = 3;
            this.TargetValueBox.SelectedIndexChanged += new System.EventHandler(this.TargetValueBox_SelectedIndexChanged);
            // 
            // pictureBox5
            // 
            this.tableLayoutPanel10.SetColumnSpan(this.pictureBox5, 2);
            this.pictureBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox5.Location = new System.Drawing.Point(4, 131);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(1241, 360);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 4;
            this.pictureBox5.TabStop = false;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(627, 498);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(619, 33);
            this.button1.TabIndex = 5;
            this.button1.Text = "Save Variant Target";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TargetVariant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel10);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TargetVariant";
            this.Size = new System.Drawing.Size(1249, 534);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.ComboBox TargetPropertyBox;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.ComboBox TargetValueBox;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Button button1;
    }
}