
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

using Perforce.P4;
using Perforce.SwarmApi;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4ToolWindowControl.
	/// </summary>
	public class SwarmReviewsToolWindowControl : P4ToolWindowControlBase
	{
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		public I18nControls.GridTreeListView ReviewsLV;
		private ColumnHeader ReviewIdClm;
		private ColumnHeader DescriptionClm;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridGroupBox dividerGB;
		private I18nControls.GridLabel pathLbl;
		private I18nControls.GridFilterComboBox changesCB;
		private I18nControls.GridLabel participantsLbl;
		private I18nControls.GridFilterComboBox reviewsCB;
		private I18nControls.GridButton filterBtn;
		private I18nControls.GridLabel changesLbl;
		private I18nControls.GridFilterComboBox participantsCB;
		private I18nControls.GridLabel matchesLbl;
		private ToolTip toolTip1;
		private IContainer components;

		public SwarmReviewsToolWindowControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
        }

        public override void OnNewConnection(P4ScmProvider newScm)
		{
			filterBtn.Enabled = newScm != null;
		}

		/// <summary> 
		/// Let this control process the mnemonics.
		/// </summary>
		protected override bool ProcessDialogChar(char charCode)
		{
			// If we're the top-level form or control, we need to do the mnemonic handling
			if (charCode != ' ' && ProcessMnemonic(charCode))
			{
				return true;
			}
			return base.ProcessDialogChar(charCode);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private new void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwarmReviewsToolWindowControl));
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.ReviewsLV = new Perforce.I18nControls.GridTreeListView();
			this.ReviewIdClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.DescriptionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
			this.dividerGB = new Perforce.I18nControls.GridGroupBox();
			this.pathLbl = new Perforce.I18nControls.GridLabel();
			this.changesCB = new Perforce.I18nControls.GridFilterComboBox();
			this.participantsLbl = new Perforce.I18nControls.GridLabel();
			this.reviewsCB = new Perforce.I18nControls.GridFilterComboBox();
			this.filterBtn = new Perforce.I18nControls.GridButton();
			this.changesLbl = new Perforce.I18nControls.GridLabel();
			this.participantsCB = new Perforce.I18nControls.GridFilterComboBox();
			this.matchesLbl = new Perforce.I18nControls.GridLabel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.gridLayoutPanel1.SuspendLayout();
			this.gridLayoutSubpanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.ReviewsLV);
			this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = false;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// ReviewsLV
			// 
			this.ReviewsLV._maxLineOffset = 0;
			this.ReviewsLV.ActionColumn = -1;
			this.ReviewsLV.AllowColumnReorder = true;
			resources.ApplyResources(this.ReviewsLV, "ReviewsLV");
			this.ReviewsLV.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ReviewsLV.CellHeight = 230;
			this.ReviewsLV.CellWidth = 594;
			this.ReviewsLV.Column = 0;
			this.ReviewsLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ReviewIdClm,
            this.DescriptionClm});
			this.ReviewsLV.ColumnsSpanned = 0;
			this.ReviewsLV.EnableIconOverlays = false;
			this.ReviewsLV.EnableSorting = true;
			this.ReviewsLV.Name = "ReviewsLV";
			this.ReviewsLV.OverlayOffset = 0;
			this.ReviewsLV.RootCheckBoxes = false;
			this.ReviewsLV.Row = 1;
			this.ReviewsLV.RowsSpanned = 0;
			this.ReviewsLV.ScrollPosition = 0;
			this.ReviewsLV.TreeView = true;
			this.ReviewsLV.UseClassicImageList = false;
			this.ReviewsLV.UseCompatibleStateImageBehavior = false;
			this.ReviewsLV.View = System.Windows.Forms.View.Details;
			this.ReviewsLV.YOffset = 0;
			this.ReviewsLV.DoubleClick += new System.EventHandler(this.ReviewsLV_DoubleClick);
			// 
			// ReviewIdClm
			// 
			resources.ApplyResources(this.ReviewIdClm, "ReviewIdClm");
			// 
			// DescriptionClm
			// 
			resources.ApplyResources(this.DescriptionClm, "DescriptionClm");
			// 
			// gridLayoutSubpanel1
			// 
			resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
			this.gridLayoutSubpanel1.CellHeight = 62;
			this.gridLayoutSubpanel1.CellWidth = 594;
			this.gridLayoutSubpanel1.Column = 0;
			this.gridLayoutSubpanel1.ColumnsSpanned = 0;
			this.gridLayoutSubpanel1.Controls.Add(this.dividerGB);
			this.gridLayoutSubpanel1.Controls.Add(this.pathLbl);
			this.gridLayoutSubpanel1.Controls.Add(this.changesCB);
			this.gridLayoutSubpanel1.Controls.Add(this.participantsLbl);
			this.gridLayoutSubpanel1.Controls.Add(this.reviewsCB);
			this.gridLayoutSubpanel1.Controls.Add(this.filterBtn);
			this.gridLayoutSubpanel1.Controls.Add(this.changesLbl);
			this.gridLayoutSubpanel1.Controls.Add(this.participantsCB);
			this.gridLayoutSubpanel1.Controls.Add(this.matchesLbl);
			this.gridLayoutSubpanel1.EnableDesignerGrid = false;
			this.gridLayoutSubpanel1.EnableDesignerLayout = false;
			this.gridLayoutSubpanel1.EnableParentResize = false;
			this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
			this.gridLayoutSubpanel1.MinimumRowHeight = 10;
			this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
			this.gridLayoutSubpanel1.Row = 0;
			this.gridLayoutSubpanel1.RowsSpanned = 0;
			this.gridLayoutSubpanel1.YOffset = 0;
			// 
			// dividerGB
			// 
			resources.ApplyResources(this.dividerGB, "dividerGB");
			this.dividerGB.CellHeight = 61;
			this.dividerGB.CellWidth = 10;
			this.dividerGB.Column = 4;
			this.dividerGB.ColumnsSpanned = 0;
			this.dividerGB.Name = "dividerGB";
			this.dividerGB.Row = 0;
			this.dividerGB.RowsSpanned = 1;
			this.dividerGB.TabStop = false;
			this.dividerGB.YOffset = 0;
			// 
			// pathLbl
			// 
			resources.ApplyResources(this.pathLbl, "pathLbl");
			this.pathLbl.CellHeight = 33;
			this.pathLbl.CellWidth = 77;
			this.pathLbl.Column = 0;
			this.pathLbl.ColumnsSpanned = 0;
			this.pathLbl.Name = "pathLbl";
			this.pathLbl.Row = 0;
			this.pathLbl.RowsSpanned = 0;
			this.toolTip1.SetToolTip(this.pathLbl, resources.GetString("pathLbl.ToolTip"));
			this.pathLbl.YOffset = 7;
			// 
			// changesCB
			// 
			resources.ApplyResources(this.changesCB, "changesCB");
			this.changesCB.CellHeight = 28;
			this.changesCB.CellWidth = 182;
			this.changesCB.Column = 3;
			this.changesCB.ColumnsSpanned = 0;
			this.changesCB.FormattingEnabled = true;
			this.changesCB.Name = "changesCB";
			this.changesCB.Row = 1;
			this.changesCB.RowsSpanned = 0;
			this.toolTip1.SetToolTip(this.changesCB, resources.GetString("changesCB.ToolTip"));
			this.changesCB.YOffset = 0;
			// 
			// participantsLbl
			// 
			resources.ApplyResources(this.participantsLbl, "participantsLbl");
			this.participantsLbl.CellHeight = 28;
			this.participantsLbl.CellWidth = 77;
			this.participantsLbl.Column = 0;
			this.participantsLbl.ColumnsSpanned = 0;
			this.participantsLbl.Name = "participantsLbl";
			this.participantsLbl.Row = 1;
			this.participantsLbl.RowsSpanned = 0;
			this.toolTip1.SetToolTip(this.participantsLbl, resources.GetString("participantsLbl.ToolTip"));
			this.participantsLbl.YOffset = 4;
			// 
			// reviewsCB
			// 
			resources.ApplyResources(this.reviewsCB, "reviewsCB");
			this.reviewsCB.CellHeight = 33;
			this.reviewsCB.CellWidth = 426;
			this.reviewsCB.Column = 1;
			this.reviewsCB.ColumnsSpanned = 2;
			this.reviewsCB.FormattingEnabled = true;
			this.reviewsCB.Name = "reviewsCB";
			this.reviewsCB.Row = 0;
			this.reviewsCB.RowsSpanned = 0;
			this.toolTip1.SetToolTip(this.reviewsCB, resources.GetString("reviewsCB.ToolTip"));
			this.reviewsCB.YOffset = 3;
			// 
			// filterBtn
			// 
			resources.ApplyResources(this.filterBtn, "filterBtn");
			this.filterBtn.CellHeight = 33;
			this.filterBtn.CellWidth = 81;
			this.filterBtn.Column = 5;
			this.filterBtn.ColumnsSpanned = 0;
			this.filterBtn.Name = "filterBtn";
			this.filterBtn.Row = 0;
			this.filterBtn.RowsSpanned = 0;
			this.filterBtn.UseVisualStyleBackColor = true;
			this.filterBtn.YOffset = 2;
			this.filterBtn.Click += new System.EventHandler(this.filterBtn_Click);
			// 
			// changesLbl
			// 
			resources.ApplyResources(this.changesLbl, "changesLbl");
			this.changesLbl.CellHeight = 28;
			this.changesLbl.CellWidth = 61;
			this.changesLbl.Column = 2;
			this.changesLbl.ColumnsSpanned = 0;
			this.changesLbl.Name = "changesLbl";
			this.changesLbl.Row = 1;
			this.changesLbl.RowsSpanned = 0;
			this.toolTip1.SetToolTip(this.changesLbl, resources.GetString("changesLbl.ToolTip"));
			this.changesLbl.YOffset = 4;
			// 
			// participantsCB
			// 
			resources.ApplyResources(this.participantsCB, "participantsCB");
			this.participantsCB.CellHeight = 28;
			this.participantsCB.CellWidth = 183;
			this.participantsCB.Column = 1;
			this.participantsCB.ColumnsSpanned = 0;
			this.participantsCB.FormattingEnabled = true;
			this.participantsCB.Name = "participantsCB";
			this.participantsCB.Row = 1;
			this.participantsCB.RowsSpanned = 0;
			this.toolTip1.SetToolTip(this.participantsCB, resources.GetString("participantsCB.ToolTip"));
			this.participantsCB.YOffset = 0;
			// 
			// matchesLbl
			// 
			resources.ApplyResources(this.matchesLbl, "matchesLbl");
			this.matchesLbl.AutoEllipsis = true;
			this.matchesLbl.CellHeight = 28;
			this.matchesLbl.CellWidth = 81;
			this.matchesLbl.Column = 5;
			this.matchesLbl.ColumnsSpanned = 0;
			this.matchesLbl.Name = "matchesLbl";
			this.matchesLbl.Row = 1;
			this.matchesLbl.RowsSpanned = 0;
			this.matchesLbl.YOffset = 4;
			// 
			// SwarmReviewsToolWindowControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.gridLayoutPanel1);
			this.Name = "SwarmReviewsToolWindowControl";
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutSubpanel1.ResumeLayout(false);
			this.gridLayoutSubpanel1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void filterBtn_Click(object sender, EventArgs e)
		{
			ReviewsLV.Items.Clear();


			// for real 
			if (Scm == null)
			{
				// shouldn't happen but...
				return;
			}
			SwarmApi.Options searchOptions = new SwarmApi.Options();
			string s = reviewsCB.Text;
			if (string.IsNullOrEmpty(s) == false)
			{
				string ids = string.Format("[{0}]", s);
				searchOptions["ids[]"] = new JSONParser.JSONArray(ids);
			}
			s = participantsCB.Text;
			if (string.IsNullOrEmpty(s) == false)
			{
				string participants = string.Format("[{0}]", s);
				searchOptions["participants[]"] = new JSONParser.JSONArray(participants);
			}
			s = changesCB.Text;
			if (string.IsNullOrEmpty(s) == false)
			{
				string changes = string.Format("[{0}]", s);
				searchOptions["change[]"] = new JSONParser.JSONArray(changes);
			}
			searchOptions["max"] = new JSONParser.JSONNumericalField(Preferences.LocalSettings.GetInt("Number_specs", 100));
			try
			{
                SwarmServer sw = new SwarmServer(Scm.Connection.Swarm.SwarmUrl, Scm.Connection.User, Scm.Connection.Swarm.SwarmPassword);

				SwarmServer.ReviewList reviews = sw.GetReviews(searchOptions);

				foreach (SwarmServer.Review review in reviews)
				{
					TreeListViewItem it = new TreeListViewItem(null, review.id.ToString(), review.description);
					it.Tag = review;
					ReviewsLV.Items.Add(it);
				}

			}
			catch (Exception)
			{ }
		}

		public SwarmServer.Review SelectedReview
		{
			get
			{
				if (ReviewsLV.SelectedItems.Count > 0)
				{
					return ReviewsLV.SelectedItems[0].Tag as SwarmServer.Review;
				}
				return null;
			}
		}

		public event EventHandler ReviewsLVDoubleClicked;

		private void ReviewsLV_DoubleClick(object sender, EventArgs e)
		{
			if (ReviewsLVDoubleClicked != null)
			{
				ReviewsLVDoubleClicked(sender, e);
			}
		}
	}
}
