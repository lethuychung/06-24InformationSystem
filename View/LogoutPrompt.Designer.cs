namespace _06_24InformationSystem.View
{
    partial class LogoutPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogoutPrompt));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.logoutCommentTextBox = new System.Windows.Forms.RichTextBox();
            this.logoutPromptButton = new System.Windows.Forms.Button();
            this.LogoutPromptLogoutButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.PassComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(90, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Antal ologgade samtal: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(91, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Summering av pass:";
            // 
            // logoutCommentTextBox
            // 
            this.logoutCommentTextBox.Location = new System.Drawing.Point(94, 127);
            this.logoutCommentTextBox.Name = "logoutCommentTextBox";
            this.logoutCommentTextBox.Size = new System.Drawing.Size(235, 121);
            this.logoutCommentTextBox.TabIndex = 2;
            this.logoutCommentTextBox.Text = "";
            // 
            // logoutPromptButton
            // 
            this.logoutPromptButton.Location = new System.Drawing.Point(57, 276);
            this.logoutPromptButton.Name = "logoutPromptButton";
            this.logoutPromptButton.Size = new System.Drawing.Size(75, 23);
            this.logoutPromptButton.TabIndex = 3;
            this.logoutPromptButton.Text = "Logga ut";
            this.logoutPromptButton.UseVisualStyleBackColor = true;
            this.logoutPromptButton.Click += new System.EventHandler(this.logoutPromptButton_Click);
            // 
            // LogoutPromptLogoutButton
            // 
            this.LogoutPromptLogoutButton.Location = new System.Drawing.Point(286, 276);
            this.LogoutPromptLogoutButton.Name = "LogoutPromptLogoutButton";
            this.LogoutPromptLogoutButton.Size = new System.Drawing.Size(75, 23);
            this.LogoutPromptLogoutButton.TabIndex = 4;
            this.LogoutPromptLogoutButton.Text = "Avbryt";
            this.LogoutPromptLogoutButton.UseVisualStyleBackColor = true;
            this.LogoutPromptLogoutButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(91, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ditt arbetspass";
            // 
            // PassComboBox
            // 
            this.PassComboBox.FormattingEnabled = true;
            this.PassComboBox.Items.AddRange(new object[] {
            "06-12",
            "12-18",
            "18-24"});
            this.PassComboBox.Location = new System.Drawing.Point(94, 74);
            this.PassComboBox.Name = "PassComboBox";
            this.PassComboBox.Size = new System.Drawing.Size(121, 21);
            this.PassComboBox.TabIndex = 6;
            // 
            // LogoutPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 314);
            this.Controls.Add(this.PassComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LogoutPromptLogoutButton);
            this.Controls.Add(this.logoutPromptButton);
            this.Controls.Add(this.logoutCommentTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogoutPrompt";
            this.Text = "Logga ut";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox logoutCommentTextBox;
        private System.Windows.Forms.Button logoutPromptButton;
        private System.Windows.Forms.Button LogoutPromptLogoutButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox PassComboBox;
    }
}