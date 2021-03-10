namespace Perforce.P4VS.UI
{
    partial class P4VSMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4VSMessage));
            this.OKBtn = new System.Windows.Forms.Button();
            this.messageTB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OKBtn.Location = new System.Drawing.Point(103, 99);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 0;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // messageTB
            // 
            this.messageTB.BackColor = System.Drawing.SystemColors.Control;
            this.messageTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messageTB.Location = new System.Drawing.Point(13, 13);
            this.messageTB.Margin = new System.Windows.Forms.Padding(3, 3, 15, 50);
            this.messageTB.MinimumSize = new System.Drawing.Size(0, 20);
            this.messageTB.Multiline = true;
            this.messageTB.Name = "messageTB";
            this.messageTB.ReadOnly = true;
            this.messageTB.Size = new System.Drawing.Size(165, 67);
            this.messageTB.TabIndex = 1;
            this.messageTB.TextChanged += new System.EventHandler(this.messageTB_TextChanged);
            this.messageTB.DoubleClick += new System.EventHandler(this.messageTB_DoubleClick);
            this.messageTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.messageTB_KeyDown);
            // 
            // Message
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.OKBtn;
            this.ClientSize = new System.Drawing.Size(189, 134);
            this.Controls.Add(this.messageTB);
            this.Controls.Add(this.OKBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Message";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Message";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.TextBox messageTB;
    }
}