namespace Perforce.P4VS
{
    partial class NoDiffsDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoDiffsDlg));
            this.msgLbl = new System.Windows.Forms.Label();
            this.OkBtn = new System.Windows.Forms.Button();
            this.identicalFilesListView = new Perforce.P4VS.DoubleBufferedListView();
            this.File1Clm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.File2Clm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // msgLbl
            // 
            resources.ApplyResources(this.msgLbl, "msgLbl");
            this.msgLbl.Name = "msgLbl";
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // identicalFilesListView
            // 
            this.identicalFilesListView.AllowColumnReorder = true;
            resources.ApplyResources(this.identicalFilesListView, "identicalFilesListView");
            this.identicalFilesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.File1Clm,
            this.File2Clm});
            this.identicalFilesListView.HideActionsColumn = false;
            this.identicalFilesListView.Name = "identicalFilesListView";
            this.identicalFilesListView.OwnerDraw = true;
            this.identicalFilesListView.UseCompatibleStateImageBehavior = false;
            this.identicalFilesListView.View = System.Windows.Forms.View.Details;
            // 
            // File1Clm
            // 
            resources.ApplyResources(this.File1Clm, "File1Clm");
            // 
            // File2Clm
            // 
            resources.ApplyResources(this.File2Clm, "File2Clm");
            // 
            // NoDiffsDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.OkBtn;
            this.Controls.Add(this.identicalFilesListView);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.msgLbl);
            this.Name = "NoDiffsDlg";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label msgLbl;
        private System.Windows.Forms.Button OkBtn;
        private DoubleBufferedListView identicalFilesListView;
        private System.Windows.Forms.ColumnHeader File1Clm;
        private System.Windows.Forms.ColumnHeader File2Clm;
    }
}