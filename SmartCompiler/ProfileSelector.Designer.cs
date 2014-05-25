namespace SmartCompiler
{
    partial class ProfileSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileSelector));
            this.comboBoxProfiles = new System.Windows.Forms.ComboBox();
            this.btnGO = new System.Windows.Forms.Button();
            this.lblSelectAProfile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxProfiles
            // 
            this.comboBoxProfiles.FormattingEnabled = true;
            this.comboBoxProfiles.Location = new System.Drawing.Point(11, 38);
            this.comboBoxProfiles.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxProfiles.Name = "comboBoxProfiles";
            this.comboBoxProfiles.Size = new System.Drawing.Size(205, 21);
            this.comboBoxProfiles.TabIndex = 0;
            // 
            // btnGO
            // 
            this.btnGO.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGO.Location = new System.Drawing.Point(224, 11);
            this.btnGO.Margin = new System.Windows.Forms.Padding(2);
            this.btnGO.Name = "btnGO";
            this.btnGO.Size = new System.Drawing.Size(68, 48);
            this.btnGO.TabIndex = 1;
            this.btnGO.Text = "GO";
            this.btnGO.UseVisualStyleBackColor = true;
            this.btnGO.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblSelectAProfile
            // 
            this.lblSelectAProfile.AutoSize = true;
            this.lblSelectAProfile.Location = new System.Drawing.Point(13, 13);
            this.lblSelectAProfile.Name = "lblSelectAProfile";
            this.lblSelectAProfile.Size = new System.Drawing.Size(204, 13);
            this.lblSelectAProfile.TabIndex = 2;
            this.lblSelectAProfile.Text = "Please select one of the following profiles:";
            // 
            // ProfileSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 70);
            this.Controls.Add(this.lblSelectAProfile);
            this.Controls.Add(this.btnGO);
            this.Controls.Add(this.comboBoxProfiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "ProfileSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Profile selector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxProfiles;
        private System.Windows.Forms.Button btnGO;
        private System.Windows.Forms.Label lblSelectAProfile;
    }
}