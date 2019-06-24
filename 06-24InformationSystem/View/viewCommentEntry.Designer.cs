namespace _06_24InformationSystem.View
{
    partial class viewCommentEntry
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
            this.viewCommentBox = new System.Windows.Forms.RichTextBox();
            this.closeCommentBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // viewCommentBox
            // 
            this.viewCommentBox.Location = new System.Drawing.Point(1, 1);
            this.viewCommentBox.Name = "viewCommentBox";
            this.viewCommentBox.ReadOnly = true;
            this.viewCommentBox.Size = new System.Drawing.Size(312, 312);
            this.viewCommentBox.TabIndex = 0;
            this.viewCommentBox.Text = "";
            // 
            // closeCommentBtn
            // 
            this.closeCommentBtn.Location = new System.Drawing.Point(117, 319);
            this.closeCommentBtn.Name = "closeCommentBtn";
            this.closeCommentBtn.Size = new System.Drawing.Size(75, 23);
            this.closeCommentBtn.TabIndex = 1;
            this.closeCommentBtn.Text = "Stäng";
            this.closeCommentBtn.UseVisualStyleBackColor = true;
            this.closeCommentBtn.Click += new System.EventHandler(this.closeCommentBtn_Click);
            // 
            // viewCommentEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 354);
            this.Controls.Add(this.closeCommentBtn);
            this.Controls.Add(this.viewCommentBox);
            this.Name = "viewCommentEntry";
            this.Text = "Kommentar";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox viewCommentBox;
        private System.Windows.Forms.Button closeCommentBtn;
    }
}