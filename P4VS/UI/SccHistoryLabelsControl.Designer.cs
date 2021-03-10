namespace Perforce.P4VS
{
	partial class SccHistoryLabelsControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SccHistoryLabelsControl));
			this.LabelsListVew = new Perforce.P4VS.DoubleBufferedListView();
			this.LabelNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.UpdateTimeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.AccessTimeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.OwnerClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.DescriptionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// LabelsListVew
			// 
			this.LabelsListVew.AllowColumnReorder = true;
			this.LabelsListVew.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LabelNameClm,
            this.UpdateTimeClm,
            this.AccessTimeClm,
            this.OwnerClm,
            this.DescriptionClm});
			resources.ApplyResources(this.LabelsListVew, "LabelsListVew");
			this.LabelsListVew.HideActionsColumn = false;
			this.LabelsListVew.Name = "LabelsListVew";
			this.LabelsListVew.OwnerDraw = true;
			this.LabelsListVew.UseCompatibleStateImageBehavior = false;
			this.LabelsListVew.View = System.Windows.Forms.View.Details;
			// 
			// LabelNameClm
			// 
			resources.ApplyResources(this.LabelNameClm, "LabelNameClm");
			// 
			// UpdateTimeClm
			// 
			resources.ApplyResources(this.UpdateTimeClm, "UpdateTimeClm");
			// 
			// AccessTimeClm
			// 
			resources.ApplyResources(this.AccessTimeClm, "AccessTimeClm");
			// 
			// OwnerClm
			// 
			resources.ApplyResources(this.OwnerClm, "OwnerClm");
			// 
			// DescriptionClm
			// 
			resources.ApplyResources(this.DescriptionClm, "DescriptionClm");
			// 
			// SccHistoryLabelsControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.LabelsListVew);
			this.MinimumSize = new System.Drawing.Size(430, 130);
			this.Name = "SccHistoryLabelsControl";
			this.ResumeLayout(false);

		}

		#endregion

        private Perforce.P4VS.DoubleBufferedListView LabelsListVew;
		private System.Windows.Forms.ColumnHeader LabelNameClm;
		private System.Windows.Forms.ColumnHeader UpdateTimeClm;
		private System.Windows.Forms.ColumnHeader AccessTimeClm;
		private System.Windows.Forms.ColumnHeader OwnerClm;
		private System.Windows.Forms.ColumnHeader DescriptionClm;
	}
}
