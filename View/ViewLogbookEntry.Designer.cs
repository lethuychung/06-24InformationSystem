namespace _06_24InformationSystem.View
{
    partial class ViewLogbookEntry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewLogbookEntry));
            this.entryErrorText = new System.Windows.Forms.RichTextBox();
            this.closeBtn = new System.Windows.Forms.Button();
            this.entryCommentText = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // entryErrorText
            // 
            this.entryErrorText.Location = new System.Drawing.Point(12, 36);
            this.entryErrorText.Name = "entryErrorText";
            this.entryErrorText.ReadOnly = true;
            this.entryErrorText.Size = new System.Drawing.Size(255, 309);
            this.entryErrorText.TabIndex = 0;
            this.entryErrorText.Text = "";
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(231, 351);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 1;
            this.closeBtn.Text = "Stäng";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // entryCommentText
            // 
            this.entryCommentText.Location = new System.Drawing.Point(273, 36);
            this.entryCommentText.Name = "entryCommentText";
            this.entryCommentText.ReadOnly = true;
            this.entryCommentText.Size = new System.Drawing.Size(255, 309);
            this.entryCommentText.TabIndex = 2;
            this.entryCommentText.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Fel";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(380, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Kommentar";
            // 
            // ViewLogbookEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 386);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.entryCommentText);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.entryErrorText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewLogbookEntry";
            this.Text = "SO MUCH TEXT, WOW";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox entryErrorText;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.RichTextBox entryCommentText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}