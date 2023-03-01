namespace SharpBrowser
{
    partial class TitleBarForm
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
            this.checkBoxH = new System.Windows.Forms.CheckBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxT = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxH
            // 
            this.checkBoxH.AutoSize = true;
            this.checkBoxH.Checked = true;
            this.checkBoxH.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxH.Location = new System.Drawing.Point(6, 3);
            this.checkBoxH.Name = "checkBoxH";
            this.checkBoxH.Size = new System.Drawing.Size(35, 19);
            this.checkBoxH.TabIndex = 0;
            this.checkBoxH.Text = "H";
            this.checkBoxH.UseVisualStyleBackColor = true;
            // 
            // checkBoxB
            // 
            this.checkBoxB.AutoSize = true;
            this.checkBoxB.Checked = true;
            this.checkBoxB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxB.Location = new System.Drawing.Point(47, 3);
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.Size = new System.Drawing.Size(33, 19);
            this.checkBoxB.TabIndex = 0;
            this.checkBoxB.Text = "B";
            this.checkBoxB.UseVisualStyleBackColor = true;
            // 
            // checkBoxT
            // 
            this.checkBoxT.AutoSize = true;
            this.checkBoxT.Location = new System.Drawing.Point(86, 3);
            this.checkBoxT.Name = "checkBoxT";
            this.checkBoxT.Size = new System.Drawing.Size(32, 19);
            this.checkBoxT.TabIndex = 0;
            this.checkBoxT.Text = "T";
            this.checkBoxT.UseVisualStyleBackColor = false;
            // 
            // TitleBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 24);
            this.Controls.Add(this.checkBoxT);
            this.Controls.Add(this.checkBoxB);
            this.Controls.Add(this.checkBoxH);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TitleBarForm";
            this.ShowInTaskbar = false;
            this.Text = "TitleBarForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public CheckBox checkBoxH;
        public CheckBox checkBoxB;
        public CheckBox checkBoxT;
    }
}