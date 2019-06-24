namespace _06_24InformationSystem.View
{
    partial class ChoosePassPormpt
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
            this.label1 = new System.Windows.Forms.Label();
            this.savePassBtn = new System.Windows.Forms.Button();
            this.pass1 = new System.Windows.Forms.TextBox();
            this.pass2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(102, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Välj lösenord";
            // 
            // savePassBtn
            // 
            this.savePassBtn.Location = new System.Drawing.Point(105, 161);
            this.savePassBtn.Name = "savePassBtn";
            this.savePassBtn.Size = new System.Drawing.Size(75, 23);
            this.savePassBtn.TabIndex = 1;
            this.savePassBtn.Text = "Spara";
            this.savePassBtn.UseVisualStyleBackColor = true;
            this.savePassBtn.Click += new System.EventHandler(this.savePassBtn_Click);
            // 
            // pass1
            // 
            this.pass1.Location = new System.Drawing.Point(96, 74);
            this.pass1.Name = "pass1";
            this.pass1.PasswordChar = '*';
            this.pass1.Size = new System.Drawing.Size(100, 20);
            this.pass1.TabIndex = 2;
            // 
            // pass2
            // 
            this.pass2.Location = new System.Drawing.Point(96, 115);
            this.pass2.Name = "pass2";
            this.pass2.PasswordChar = '*';
            this.pass2.Size = new System.Drawing.Size(100, 20);
            this.pass2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(102, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Repetera lösenord";
            // 
            // ChoosePassPormpt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 279);
            this.Controls.Add(this.pass2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pass1);
            this.Controls.Add(this.savePassBtn);
            this.Controls.Add(this.label1);
            this.Name = "ChoosePassPormpt";
            this.Text = "Välj nytt lösenord";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button savePassBtn;
        private System.Windows.Forms.TextBox pass1;
        private System.Windows.Forms.TextBox pass2;
        private System.Windows.Forms.Label label2;
    }
}