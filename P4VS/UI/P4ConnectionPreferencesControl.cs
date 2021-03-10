
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4ConnectionPreferencesControl.
	/// </summary>
	public class P4ConnectionPreferencesControl : System.Windows.Forms.UserControl
	{

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Perforce.I18nControls.GridCheckBox autoLogoffChk;
		private Perforce.I18nControls.GridCheckBox useIPChk;
		private Perforce.I18nControls.GridLabel openCloseLbl;
		private Perforce.I18nControls.GridGroupBox opencCloseGB;
		private Perforce.I18nControls.GridLabel connectLbl;
		private Perforce.I18nControls.GridGroupBox connectGB;
		private Perforce.I18nControls.GridRadioButton ConnectDialogRB;
		private Perforce.I18nControls.GridRadioButton ConnectRecentRB;
		private Perforce.I18nControls.GridRadioButton ConnectSolutionRB;
		private Perforce.I18nControls.GridRadioButton ConnectEnvironmentRB;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel2;
		private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridListBox mruLB;
        private I18nControls.GridButton removeBtn;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel3;
        private I18nControls.GridLabel savedLbl;
        private I18nControls.GridGroupBox savedGB;

        // The parent page, use to persist data
        private P4ConnectionPreferences _customPage;

		public P4ConnectionPreferencesControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
				GC.SuppressFinalize(this);
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4ConnectionPreferencesControl));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridLayoutSubpanel3 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.savedLbl = new Perforce.I18nControls.GridLabel();
            this.savedGB = new Perforce.I18nControls.GridGroupBox();
            this.mruLB = new Perforce.I18nControls.GridListBox();
            this.removeBtn = new Perforce.I18nControls.GridButton();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.ConnectDialogRB = new Perforce.I18nControls.GridRadioButton();
            this.ConnectRecentRB = new Perforce.I18nControls.GridRadioButton();
            this.ConnectSolutionRB = new Perforce.I18nControls.GridRadioButton();
            this.ConnectEnvironmentRB = new Perforce.I18nControls.GridRadioButton();
            this.useIPChk = new Perforce.I18nControls.GridCheckBox();
            this.autoLogoffChk = new Perforce.I18nControls.GridCheckBox();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.connectLbl = new Perforce.I18nControls.GridLabel();
            this.connectGB = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutSubpanel2 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.openCloseLbl = new Perforce.I18nControls.GridLabel();
            this.opencCloseGB = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel3.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.gridLayoutSubpanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel3);
            this.gridLayoutPanel1.Controls.Add(this.mruLB);
            this.gridLayoutPanel1.Controls.Add(this.removeBtn);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.ConnectDialogRB);
            this.gridLayoutPanel1.Controls.Add(this.ConnectRecentRB);
            this.gridLayoutPanel1.Controls.Add(this.ConnectSolutionRB);
            this.gridLayoutPanel1.Controls.Add(this.ConnectEnvironmentRB);
            this.gridLayoutPanel1.Controls.Add(this.useIPChk);
            this.gridLayoutPanel1.Controls.Add(this.autoLogoffChk);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel2);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 0;
            this.gridLayoutPanel1.MinimumRowHeight = 0;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridLayoutSubpanel3
            // 
            resources.ApplyResources(this.gridLayoutSubpanel3, "gridLayoutSubpanel3");
            this.gridLayoutSubpanel3.CellHeight = 25;
            this.gridLayoutSubpanel3.CellWidth = 453;
            this.gridLayoutSubpanel3.Column = 0;
            this.gridLayoutSubpanel3.ColumnsSpanned = 1;
            this.gridLayoutSubpanel3.Controls.Add(this.savedLbl);
            this.gridLayoutSubpanel3.Controls.Add(this.savedGB);
            this.gridLayoutSubpanel3.EnableDesignerGrid = false;
            this.gridLayoutSubpanel3.EnableDesignerLayout = true;
            this.gridLayoutSubpanel3.EnableParentResize = false;
            this.gridLayoutSubpanel3.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel3.MinimumRowHeight = 10;
            this.gridLayoutSubpanel3.Name = "gridLayoutSubpanel3";
            this.gridLayoutSubpanel3.Row = 8;
            this.gridLayoutSubpanel3.RowsSpanned = 0;
            this.gridLayoutSubpanel3.YOffset = 0;
            // 
            // savedLbl
            // 
            resources.ApplyResources(this.savedLbl, "savedLbl");
            this.savedLbl.CellHeight = 16;
            this.savedLbl.CellWidth = 102;
            this.savedLbl.Column = 0;
            this.savedLbl.ColumnsSpanned = 0;
            this.savedLbl.Name = "savedLbl";
            this.savedLbl.Row = 0;
            this.savedLbl.RowsSpanned = 0;
            this.savedLbl.YOffset = 0;
            // 
            // savedGB
            // 
            resources.ApplyResources(this.savedGB, "savedGB");
            this.savedGB.CellHeight = 16;
            this.savedGB.CellWidth = 351;
            this.savedGB.Column = 1;
            this.savedGB.ColumnsSpanned = 0;
            this.savedGB.Name = "savedGB";
            this.savedGB.Row = 0;
            this.savedGB.RowsSpanned = 0;
            this.savedGB.TabStop = false;
            this.savedGB.YOffset = 6;
            // 
            // mruLB
            // 
            resources.ApplyResources(this.mruLB, "mruLB");
            this.mruLB.CellHeight = 75;
            this.mruLB.CellWidth = 372;
            this.mruLB.Column = 0;
            this.mruLB.ColumnsSpanned = 0;
            this.mruLB.FormattingEnabled = true;
            this.mruLB.Name = "mruLB";
            this.mruLB.Row = 9;
            this.mruLB.RowsSpanned = 0;
            this.mruLB.YOffset = 0;
            // 
            // removeBtn
            // 
            resources.ApplyResources(this.removeBtn, "removeBtn");
            this.removeBtn.CellHeight = 75;
            this.removeBtn.CellWidth = 81;
            this.removeBtn.Column = 1;
            this.removeBtn.ColumnsSpanned = 0;
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.Row = 9;
            this.removeBtn.RowsSpanned = 0;
            this.removeBtn.UseVisualStyleBackColor = true;
            this.removeBtn.YOffset = 0;
            this.removeBtn.Click += new System.EventHandler(this.removeBtn_Click);
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 125;
            this.gridPanel1.CellWidth = 453;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 1;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 11;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // ConnectDialogRB
            // 
            resources.ApplyResources(this.ConnectDialogRB, "ConnectDialogRB");
            this.ConnectDialogRB.CellHeight = 23;
            this.ConnectDialogRB.CellWidth = 372;
            this.ConnectDialogRB.Checked = true;
            this.ConnectDialogRB.Column = 0;
            this.ConnectDialogRB.ColumnsSpanned = 0;
            this.ConnectDialogRB.Name = "ConnectDialogRB";
            this.ConnectDialogRB.Row = 1;
            this.ConnectDialogRB.RowsSpanned = 0;
            this.ConnectDialogRB.TabStop = true;
            this.ConnectDialogRB.UseVisualStyleBackColor = true;
            this.ConnectDialogRB.YOffset = 0;
            // 
            // ConnectRecentRB
            // 
            resources.ApplyResources(this.ConnectRecentRB, "ConnectRecentRB");
            this.ConnectRecentRB.CellHeight = 23;
            this.ConnectRecentRB.CellWidth = 372;
            this.ConnectRecentRB.Column = 0;
            this.ConnectRecentRB.ColumnsSpanned = 0;
            this.ConnectRecentRB.Name = "ConnectRecentRB";
            this.ConnectRecentRB.Row = 2;
            this.ConnectRecentRB.RowsSpanned = 0;
            this.ConnectRecentRB.TabStop = true;
            this.ConnectRecentRB.UseVisualStyleBackColor = true;
            this.ConnectRecentRB.YOffset = 0;
            // 
            // ConnectSolutionRB
            // 
            resources.ApplyResources(this.ConnectSolutionRB, "ConnectSolutionRB");
            this.ConnectSolutionRB.CellHeight = 23;
            this.ConnectSolutionRB.CellWidth = 372;
            this.ConnectSolutionRB.Column = 0;
            this.ConnectSolutionRB.ColumnsSpanned = 0;
            this.ConnectSolutionRB.Name = "ConnectSolutionRB";
            this.ConnectSolutionRB.Row = 3;
            this.ConnectSolutionRB.RowsSpanned = 0;
            this.ConnectSolutionRB.TabStop = true;
            this.ConnectSolutionRB.UseVisualStyleBackColor = true;
            this.ConnectSolutionRB.YOffset = 0;
            // 
            // ConnectEnvironmentRB
            // 
            resources.ApplyResources(this.ConnectEnvironmentRB, "ConnectEnvironmentRB");
            this.ConnectEnvironmentRB.CellHeight = 23;
            this.ConnectEnvironmentRB.CellWidth = 372;
            this.ConnectEnvironmentRB.Column = 0;
            this.ConnectEnvironmentRB.ColumnsSpanned = 0;
            this.ConnectEnvironmentRB.Name = "ConnectEnvironmentRB";
            this.ConnectEnvironmentRB.Row = 4;
            this.ConnectEnvironmentRB.RowsSpanned = 0;
            this.ConnectEnvironmentRB.TabStop = true;
            this.ConnectEnvironmentRB.UseVisualStyleBackColor = true;
            this.ConnectEnvironmentRB.YOffset = 0;
            // 
            // useIPChk
            // 
            resources.ApplyResources(this.useIPChk, "useIPChk");
            this.useIPChk.CellHeight = 23;
            this.useIPChk.CellWidth = 372;
            this.useIPChk.Column = 0;
            this.useIPChk.ColumnsSpanned = 0;
            this.useIPChk.Name = "useIPChk";
            this.useIPChk.Row = 6;
            this.useIPChk.RowsSpanned = 0;
            this.useIPChk.UseVisualStyleBackColor = true;
            this.useIPChk.YOffset = 0;
            // 
            // autoLogoffChk
            // 
            resources.ApplyResources(this.autoLogoffChk, "autoLogoffChk");
            this.autoLogoffChk.CellHeight = 23;
            this.autoLogoffChk.CellWidth = 372;
            this.autoLogoffChk.Column = 0;
            this.autoLogoffChk.ColumnsSpanned = 0;
            this.autoLogoffChk.Name = "autoLogoffChk";
            this.autoLogoffChk.Row = 7;
            this.autoLogoffChk.RowsSpanned = 0;
            this.autoLogoffChk.UseVisualStyleBackColor = true;
            this.autoLogoffChk.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 26;
            this.gridLayoutSubpanel1.CellWidth = 453;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 1;
            this.gridLayoutSubpanel1.Controls.Add(this.connectLbl);
            this.gridLayoutSubpanel1.Controls.Add(this.connectGB);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 0;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // connectLbl
            // 
            resources.ApplyResources(this.connectLbl, "connectLbl");
            this.connectLbl.CellHeight = 17;
            this.connectLbl.CellWidth = 224;
            this.connectLbl.Column = 0;
            this.connectLbl.ColumnsSpanned = 0;
            this.connectLbl.Name = "connectLbl";
            this.connectLbl.Row = 0;
            this.connectLbl.RowsSpanned = 0;
            this.connectLbl.YOffset = 0;
            // 
            // connectGB
            // 
            resources.ApplyResources(this.connectGB, "connectGB");
            this.connectGB.CellHeight = 17;
            this.connectGB.CellWidth = 229;
            this.connectGB.Column = 1;
            this.connectGB.ColumnsSpanned = 0;
            this.connectGB.Name = "connectGB";
            this.connectGB.Row = 0;
            this.connectGB.RowsSpanned = 0;
            this.connectGB.TabStop = false;
            this.connectGB.YOffset = 5;
            // 
            // gridLayoutSubpanel2
            // 
            resources.ApplyResources(this.gridLayoutSubpanel2, "gridLayoutSubpanel2");
            this.gridLayoutSubpanel2.CellHeight = 28;
            this.gridLayoutSubpanel2.CellWidth = 453;
            this.gridLayoutSubpanel2.Column = 0;
            this.gridLayoutSubpanel2.ColumnsSpanned = 1;
            this.gridLayoutSubpanel2.Controls.Add(this.openCloseLbl);
            this.gridLayoutSubpanel2.Controls.Add(this.opencCloseGB);
            this.gridLayoutSubpanel2.EnableDesignerGrid = false;
            this.gridLayoutSubpanel2.EnableDesignerLayout = true;
            this.gridLayoutSubpanel2.EnableParentResize = false;
            this.gridLayoutSubpanel2.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel2.MinimumRowHeight = 10;
            this.gridLayoutSubpanel2.Name = "gridLayoutSubpanel2";
            this.gridLayoutSubpanel2.Row = 5;
            this.gridLayoutSubpanel2.RowsSpanned = 0;
            this.gridLayoutSubpanel2.YOffset = 0;
            // 
            // openCloseLbl
            // 
            resources.ApplyResources(this.openCloseLbl, "openCloseLbl");
            this.openCloseLbl.CellHeight = 13;
            this.openCloseLbl.CellWidth = 168;
            this.openCloseLbl.Column = 0;
            this.openCloseLbl.ColumnsSpanned = 0;
            this.openCloseLbl.Name = "openCloseLbl";
            this.openCloseLbl.Row = 0;
            this.openCloseLbl.RowsSpanned = 0;
            this.openCloseLbl.YOffset = 0;
            // 
            // opencCloseGB
            // 
            resources.ApplyResources(this.opencCloseGB, "opencCloseGB");
            this.opencCloseGB.CellHeight = 13;
            this.opencCloseGB.CellWidth = 285;
            this.opencCloseGB.Column = 1;
            this.opencCloseGB.ColumnsSpanned = 0;
            this.opencCloseGB.Name = "opencCloseGB";
            this.opencCloseGB.Row = 0;
            this.opencCloseGB.RowsSpanned = 0;
            this.opencCloseGB.TabStop = false;
            this.opencCloseGB.YOffset = 5;
            // 
            // P4ConnectionPreferencesControl
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "P4ConnectionPreferencesControl";
            this.Load += new System.EventHandler(this.P4ConnectionPreferencesControl_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridLayoutSubpanel3.ResumeLayout(false);
            this.gridLayoutSubpanel3.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.gridLayoutSubpanel2.ResumeLayout(false);
            this.gridLayoutSubpanel2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        public P4ConnectionPreferences OptionsPage
		{
			set
			{
				_customPage = value;
			}
		}

		private void P4ConnectionPreferencesControl_Load(object sender, EventArgs e)
		{
			ConnectDialogRB.Checked = true;
			if (Preferences.LocalSettings.ContainsKey("ConnectPreference"))
			{
				ConnectionPreference pref = (ConnectionPreference)((int)Preferences.LocalSettings["ConnectPreference"]);

				switch (pref)
				{
					case ConnectionPreference.UseEnvironment:
						ConnectEnvironmentRB.Checked = true;
						break;
					case ConnectionPreference.ShowDialog:
						ConnectDialogRB.Checked = true;
						break;
					case ConnectionPreference.UseRecent:
						ConnectRecentRB.Checked = true;
						break;
					default:
					case ConnectionPreference.UseSolution:
						ConnectSolutionRB.Checked = true;
						break;
				}
			}
			useIPChk.Checked = Preferences.LocalSettings.GetBool("Use_IP", false);
			autoLogoffChk.Checked = Preferences.LocalSettings.GetBool("Auto_logoff", false);

            mruLB.Items.Clear();
            _recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];
            if (_recentConnections != null)
            {
                foreach (ConnectionData con in _recentConnections)
                {
                    if (con != null)
                    {
                        mruLB.Items.Add(con.ToString());
                    }
                }
                if (mruLB.Items.Count > 0)
                {
                    mruLB.SelectedIndex = 0;
                    removeBtn.Enabled = true;
                }
                else
                {
                    mruLB.SelectedIndex = -1;
                    removeBtn.Enabled = false;
                }
            }
            //CommandTimeoutTB.Text = Preferences.LocalSettings.GetTimeSpan("CommandTimeOut", TimeSpan.FromSeconds(5)).TotalSeconds.ToString();
        }

        MRUList _recentConnections = null;

        public void OnApply()
		{
			if (ConnectDialogRB.Checked == true)
			{
				Preferences.LocalSettings["ConnectPreference"] = (int) ConnectionPreference.ShowDialog;
			}
			else if (ConnectRecentRB.Checked == true)
			{
				Preferences.LocalSettings["ConnectPreference"] = (int) ConnectionPreference.UseRecent;
			}
			else if (ConnectSolutionRB.Checked == true)
			{
				Preferences.LocalSettings["ConnectPreference"] = (int) ConnectionPreference.UseSolution;
			}
			else
			{
				Preferences.LocalSettings["ConnectPreference"] = (int) ConnectionPreference.UseEnvironment;
			}

			if (useIPChk.Checked == true)
			{
				Preferences.LocalSettings["Use_IP"] = true;
			}
			else
			{
				Preferences.LocalSettings["Use_IP"] = false;
			}
			
			if (autoLogoffChk.Checked == true)
			{
				Preferences.LocalSettings["Auto_logoff"] = true;
			}
			else
			{
				Preferences.LocalSettings["Auto_logoff"] = false;
			}

            _recentConnections = new MRUList(5);
            if (mruLB.Items.Count>0)
            {
                foreach (string item in mruLB.Items)
                {
                    string[] connection = item.ToString().Split(',');
                    ConnectionData cd = new ConnectionData();
                    cd.ServerPort = connection[0];
                    cd.UserName = connection[1].Trim();
                    cd.Workspace = connection[2].Trim();
                    _recentConnections.Add(cd);
                }
                Preferences.LocalSettings["RecentConnections"] = _recentConnections;
            }
            else
            {
                Preferences.LocalSettings.Remove("RecentConnections");
            }

            //double val = -1;
            //double.TryParse(CommandTimeoutTB.Text, out val);
            //if (val > 0)
            //{
            //    Preferences.LocalSettings["CommandTimeOut"] = TimeSpan.FromSeconds(val);
            //}
            //else
            //{
            //    Preferences.LocalSettings.Remove("CommandTimeOut");
            //}
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            int selected = mruLB.SelectedIndex;
            mruLB.Items.Remove(mruLB.SelectedItem);
            if (mruLB.Items.Count>0 &&  mruLB.Items.Count>=selected+1 && mruLB.Items[selected]!=null)
            {
                mruLB.SelectedIndex = selected;
            }
            else
            {
                mruLB.SelectedIndex = selected-1;
            }
            if (mruLB.SelectedIndex==-1)
            {
                removeBtn.Enabled = false;
            }
        }

        //private void unicodeChk_CheckStateChanged(object sender, EventArgs e)
        //{
        //    if (unicodeChk.Checked == true)
        //    {
        //        unicodeCB.Enabled = true;
        //    }
        //    else
        //    {
        //        unicodeCB.Enabled = false;
        //    }
        //}


    }

    public enum ConnectionPreference {ShowDialog, UseRecent, UseSolution, UseEnvironment};
}
