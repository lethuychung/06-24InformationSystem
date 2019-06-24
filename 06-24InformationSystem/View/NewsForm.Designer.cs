namespace _06_24InformationSystem
{
    partial class NewsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewsForm));
            this.newsBrowser = new System.Windows.Forms.WebBrowser();
            this.confirmReadCheckBox = new System.Windows.Forms.CheckBox();
            this.closeNewsPromptBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // newsBrowser
            // 
            this.newsBrowser.Location = new System.Drawing.Point(0, -1);
            this.newsBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.newsBrowser.Name = "newsBrowser";
            this.newsBrowser.Size = new System.Drawing.Size(416, 649);
            this.newsBrowser.TabIndex = 0;
            // 
            // confirmReadCheckBox
            // 
            this.confirmReadCheckBox.AutoSize = true;
            this.confirmReadCheckBox.Location = new System.Drawing.Point(149, 654);
            this.confirmReadCheckBox.Name = "confirmReadCheckBox";
            this.confirmReadCheckBox.Size = new System.Drawing.Size(130, 17);
            this.confirmReadCheckBox.TabIndex = 1;
            this.confirmReadCheckBox.Text = "Jag har läst nyheterna";
            this.confirmReadCheckBox.UseVisualStyleBackColor = true;
            this.confirmReadCheckBox.CheckedChanged += new System.EventHandler(this.confirmReadCheckBox_CheckedChanged);
            // 
            // closeNewsPromptBtn
            // 
            this.closeNewsPromptBtn.Enabled = false;
            this.closeNewsPromptBtn.Location = new System.Drawing.Point(169, 677);
            this.closeNewsPromptBtn.Name = "closeNewsPromptBtn";
            this.closeNewsPromptBtn.Size = new System.Drawing.Size(75, 23);
            this.closeNewsPromptBtn.TabIndex = 2;
            this.closeNewsPromptBtn.Text = "Stäng";
            this.closeNewsPromptBtn.UseVisualStyleBackColor = true;
            this.closeNewsPromptBtn.Click += new System.EventHandler(this.closeNewsPromptBtn_Click);
            // 
            // NewsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 712);
            this.Controls.Add(this.closeNewsPromptBtn);
            this.Controls.Add(this.confirmReadCheckBox);
            this.Controls.Add(this.newsBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewsForm";
            this.Text = "Nyheter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser newsBrowser;
        private System.Windows.Forms.CheckBox confirmReadCheckBox;
        private System.Windows.Forms.Button closeNewsPromptBtn;
    }
}