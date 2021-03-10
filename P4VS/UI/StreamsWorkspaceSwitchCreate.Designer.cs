namespace Perforce.P4VS
{
    partial class StreamsWorkspaceSwitchCreate
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
            this.msgLbl = new Perforce.I18nControls.GridLabel();
            this.yesBtn = new Perforce.I18nControls.GridButton();
            this.noBtn = new Perforce.I18nControls.GridButton();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // msgLbl
            // 
            this.msgLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.msgLbl.CellHeight = 0;
            this.msgLbl.CellWidth = 0;
            this.msgLbl.Column = 0;
            this.msgLbl.ColumnsSpanned = 2;
            this.msgLbl.Location = new System.Drawing.Point(20, 19);
            this.msgLbl.Name = "msgLbl";
            this.msgLbl.Row = 0;
            this.msgLbl.RowsSpanned = 0;
            this.msgLbl.Size = new System.Drawing.Size(242, 187);
            this.msgLbl.TabIndex = 0;
            this.msgLbl.Text = "You must switch to workspace \'{0]\' in order to complete the {1} operation. Procee" +
    "d?";
            this.msgLbl.YOffset = 0;
            // 
            // yesBtn
            // 
            this.yesBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.yesBtn.CellHeight = 0;
            this.yesBtn.CellWidth = 0;
            this.yesBtn.Column = 1;
            this.yesBtn.ColumnsSpanned = 0;
            this.yesBtn.Location = new System.Drawing.Point(116, 219);
            this.yesBtn.Name = "yesBtn";
            this.yesBtn.Row = 1;
            this.yesBtn.RowsSpanned = 0;
            this.yesBtn.Size = new System.Drawing.Size(75, 23);
            this.yesBtn.TabIndex = 1;
            this.yesBtn.Text = "Yes";
            this.yesBtn.UseVisualStyleBackColor = true;
            this.yesBtn.YOffset = 0;
            this.yesBtn.Click += new System.EventHandler(this.yesBtn_Click);
            // 
            // noBtn
            // 
            this.noBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.noBtn.CellHeight = 0;
            this.noBtn.CellWidth = 0;
            this.noBtn.Column = 2;
            this.noBtn.ColumnsSpanned = 0;
            this.noBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.noBtn.Location = new System.Drawing.Point(197, 219);
            this.noBtn.Name = "noBtn";
            this.noBtn.Row = 1;
            this.noBtn.RowsSpanned = 0;
            this.noBtn.Size = new System.Drawing.Size(75, 23);
            this.noBtn.TabIndex = 2;
            this.noBtn.Text = "No";
            this.noBtn.UseVisualStyleBackColor = true;
            this.noBtn.YOffset = 0;
            this.noBtn.Click += new System.EventHandler(this.noBtn_Click);
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.yesBtn);
            this.gridLayoutPanel1.Controls.Add(this.msgLbl);
            this.gridLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            this.gridLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.gridLayoutPanel1.Size = new System.Drawing.Size(284, 262);
            this.gridLayoutPanel1.TabIndex = 3;
            // 
            // gridPanel1
            // 
            this.gridPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPanel1.CellHeight = 0;
            this.gridPanel1.CellWidth = 0;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Location = new System.Drawing.Point(20, 219);
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 1;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.Size = new System.Drawing.Size(90, 23);
            this.gridPanel1.TabIndex = 1;
            this.gridPanel1.YOffset = 0;
            // 
            // StreamsWorkspaceSwitchCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.noBtn;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.noBtn);
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "StreamsWorkspaceSwitchCreate";
            this.Text = "Switch Workspace";
            this.gridLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private I18nControls.GridLabel msgLbl;
        private I18nControls.GridButton yesBtn;
        private I18nControls.GridButton noBtn;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;

    }
}