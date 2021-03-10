namespace Perforce.P4VS
{
    partial class WorkspacesBrowserDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkspacesBrowserDlg));
			this.OKBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.workspaceToolWindowControl1 = new Perforce.P4VS.WorkspaceToolWindowControl();
			this.SuspendLayout();
			// 
			// OKBtn
			// 
			resources.ApplyResources(this.OKBtn, "OKBtn");
			this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKBtn.Name = "OKBtn";
			this.OKBtn.UseVisualStyleBackColor = true;
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.UseVisualStyleBackColor = true;
			// 
			// workspaceToolWindowControl1
			// 
			resources.ApplyResources(this.workspaceToolWindowControl1, "workspaceToolWindowControl1");
			this.workspaceToolWindowControl1.BackColor = System.Drawing.SystemColors.Menu;
			this.workspaceToolWindowControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.workspaceToolWindowControl1.hostOnly = false;
			this.workspaceToolWindowControl1.Name = "workspaceToolWindowControl1";
			this.workspaceToolWindowControl1.NameFilterText = global::Perforce.P4VS.Resources.ResolveFileInteractiveControl_SuggestedAction_default;
			this.workspaceToolWindowControl1.OwnerCBText = global::Perforce.P4VS.Resources.ResolveFileInteractiveControl_SuggestedAction_default;
			this.workspaceToolWindowControl1.Scm = null;
			this.workspaceToolWindowControl1.sentOwner = null;
			this.workspaceToolWindowControl1.stream = null;
			this.workspaceToolWindowControl1.TreeListViewDoubleClicked += new System.Windows.Forms.MouseEventHandler(this.workspaceToolWindowControl1_TreeListViewDoubleClicked);
			// 
			// WorkspacesBrowserDlg
			// 
			this.AcceptButton = this.OKBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OKBtn);
			this.Controls.Add(this.workspaceToolWindowControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WorkspacesBrowserDlg";
			this.ResumeLayout(false);

        }

        #endregion

        private WorkspaceToolWindowControl workspaceToolWindowControl1;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}