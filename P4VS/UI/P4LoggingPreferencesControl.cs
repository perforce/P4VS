
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
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4LoggingPreferencesControl.
	/// </summary>
	public class P4LoggingPreferencesControl : System.Windows.Forms.UserControl
	{

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private I18nControls.GridCheckBox loggingChk;
		private I18nControls.GridCheckBox commandChk;
		private I18nControls.GridCheckBox reportingChk;
		private I18nControls.GridLabel loggingLbl;
		private I18nControls.GridGroupBox groupBox2;
		private I18nControls.GridTextBox pathTB;
		private I18nControls.GridTextBox sizeTB;
		private I18nControls.GridLabel nameLbl;
		private I18nControls.GridLabel sizeLbl;
		private I18nControls.GridButton selectBtn;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		// The parent page, use to persist data
		private P4LoggingPreferences _customPage;

		public P4LoggingPreferencesControl()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4LoggingPreferencesControl));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.loggingLbl = new Perforce.I18nControls.GridLabel();
            this.groupBox2 = new Perforce.I18nControls.GridGroupBox();
            this.reportingChk = new Perforce.I18nControls.GridCheckBox();
            this.commandChk = new Perforce.I18nControls.GridCheckBox();
            this.loggingChk = new Perforce.I18nControls.GridCheckBox();
            this.nameLbl = new Perforce.I18nControls.GridLabel();
            this.pathTB = new Perforce.I18nControls.GridTextBox();
            this.sizeLbl = new Perforce.I18nControls.GridLabel();
            this.selectBtn = new Perforce.I18nControls.GridButton();
            this.sizeTB = new Perforce.I18nControls.GridTextBox();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.reportingChk);
            this.gridLayoutPanel1.Controls.Add(this.commandChk);
            this.gridLayoutPanel1.Controls.Add(this.loggingChk);
            this.gridLayoutPanel1.Controls.Add(this.nameLbl);
            this.gridLayoutPanel1.Controls.Add(this.pathTB);
            this.gridLayoutPanel1.Controls.Add(this.sizeLbl);
            this.gridLayoutPanel1.Controls.Add(this.selectBtn);
            this.gridLayoutPanel1.Controls.Add(this.sizeTB);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 20;
            this.gridLayoutPanel1.MinimumRowHeight = 0;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.Column = 3;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 6;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 4;
            this.gridLayoutSubpanel1.Controls.Add(this.loggingLbl);
            this.gridLayoutSubpanel1.Controls.Add(this.groupBox2);
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
            // loggingLbl
            // 
            resources.ApplyResources(this.loggingLbl, "loggingLbl");
            this.loggingLbl.Column = 0;
            this.loggingLbl.ColumnsSpanned = 0;
            this.loggingLbl.Name = "loggingLbl";
            this.loggingLbl.Row = 0;
            this.loggingLbl.RowsSpanned = 0;
            this.loggingLbl.YOffset = 0;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Column = 1;
            this.groupBox2.ColumnsSpanned = 0;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Row = 0;
            this.groupBox2.RowsSpanned = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.YOffset = 0;
            // 
            // reportingChk
            // 
            resources.ApplyResources(this.reportingChk, "reportingChk");
            this.reportingChk.Column = 1;
            this.reportingChk.ColumnsSpanned = 3;
            this.reportingChk.Name = "reportingChk";
            this.reportingChk.Row = 1;
            this.reportingChk.RowsSpanned = 0;
            this.reportingChk.UseVisualStyleBackColor = true;
            this.reportingChk.YOffset = 0;
            // 
            // commandChk
            // 
            resources.ApplyResources(this.commandChk, "commandChk");
            this.commandChk.Column = 1;
            this.commandChk.ColumnsSpanned = 3;
            this.commandChk.Name = "commandChk";
            this.commandChk.Row = 2;
            this.commandChk.RowsSpanned = 0;
            this.commandChk.UseVisualStyleBackColor = true;
            this.commandChk.YOffset = 0;
            // 
            // loggingChk
            // 
            resources.ApplyResources(this.loggingChk, "loggingChk");
            this.loggingChk.Column = 1;
            this.loggingChk.ColumnsSpanned = 3;
            this.loggingChk.Name = "loggingChk";
            this.loggingChk.Row = 3;
            this.loggingChk.RowsSpanned = 0;
            this.loggingChk.UseVisualStyleBackColor = true;
            this.loggingChk.YOffset = 0;
            this.loggingChk.CheckedChanged += new System.EventHandler(this.loggingChk_CheckedChanged);
            // 
            // nameLbl
            // 
            resources.ApplyResources(this.nameLbl, "nameLbl");
            this.nameLbl.Column = 2;
            this.nameLbl.ColumnsSpanned = 0;
            this.nameLbl.Name = "nameLbl";
            this.nameLbl.Row = 4;
            this.nameLbl.RowsSpanned = 0;
            this.nameLbl.YOffset = 0;
            // 
            // pathTB
            // 
            resources.ApplyResources(this.pathTB, "pathTB");
            this.pathTB.Column = 3;
            this.pathTB.ColumnsSpanned = 0;
            this.pathTB.Name = "pathTB";
            this.pathTB.Row = 4;
            this.pathTB.RowsSpanned = 0;
            this.pathTB.YOffset = 0;
            // 
            // sizeLbl
            // 
            resources.ApplyResources(this.sizeLbl, "sizeLbl");
            this.sizeLbl.Column = 2;
            this.sizeLbl.ColumnsSpanned = 0;
            this.sizeLbl.Name = "sizeLbl";
            this.sizeLbl.Row = 5;
            this.sizeLbl.RowsSpanned = 0;
            this.sizeLbl.YOffset = 0;
            // 
            // selectBtn
            // 
            resources.ApplyResources(this.selectBtn, "selectBtn");
            this.selectBtn.Column = 4;
            this.selectBtn.ColumnsSpanned = 0;
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Row = 4;
            this.selectBtn.RowsSpanned = 0;
            this.selectBtn.UseVisualStyleBackColor = true;
            this.selectBtn.YOffset = 0;
            this.selectBtn.Click += new System.EventHandler(this.selectBtn_Click);
            // 
            // sizeTB
            // 
            resources.ApplyResources(this.sizeTB, "sizeTB");
            this.sizeTB.Column = 3;
            this.sizeTB.ColumnsSpanned = 0;
            this.sizeTB.Name = "sizeTB";
            this.sizeTB.Row = 5;
            this.sizeTB.RowsSpanned = 0;
            this.sizeTB.YOffset = 0;
            this.sizeTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sizeTB_KeyPress);
            // 
            // P4LoggingPreferencesControl
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "P4LoggingPreferencesControl";
            this.Load += new System.EventHandler(this.P4LoggingPreferencesControl_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public P4LoggingPreferences OptionsPage
		{
			set
			{
				_customPage = value;
			}
		}

		private void selectBtn_Click(object sender, EventArgs e)
		{
			Stream logSelectStream;
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			saveFileDialog.Filter = "txt files (*.txt)|*.txt";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.Title = "Enter a log file name:";

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				if ((logSelectStream = saveFileDialog.OpenFile()) != null)
				{
					pathTB.Text = saveFileDialog.FileName;
					logSelectStream.Close();
				}
			}

		}

		private void loggingChk_CheckedChanged(object sender, EventArgs e)
		{
			if (loggingChk.Checked == true)
			{
				pathTB.Enabled = true;
				sizeTB.Enabled = true;
				selectBtn.Enabled = true;
			}

			if (loggingChk.Checked == false)
			{
				pathTB.Enabled = false;
				sizeTB.Enabled = false;
				selectBtn.Enabled = false;
			}
		}

		private void P4LoggingPreferencesControl_Load(object sender, EventArgs e)
		{
			if (Preferences.LocalSettings.ContainsKey("Log_path"))
			{
				pathTB.Text = Preferences.LocalSettings.GetString("Log_path",string.Empty);
			}
			else
			{
				string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
				appPath = Path.GetDirectoryName(appPath);

				pathTB.Text = Path.Combine(appPath, "P4VS_Log.txt");
			}

			sizeTB.Text = Preferences.LocalSettings.GetInt("Log_size",50).ToString();
			reportingChk.Checked = Preferences.LocalSettings.GetBool("Log_reporting", false);
			commandChk.Checked = Preferences.LocalSettings.GetBool("Log_command", false);
			loggingChk.Checked = Preferences.LocalSettings.GetBool("Log_save", false);
		}

		public void OnApply()
		{
			Preferences.LocalSettings["Log_command"] = commandChk.Checked;
			Preferences.LocalSettings["Log_reporting"] = reportingChk.Checked;
			Preferences.LocalSettings["Log_save"] = loggingChk.Checked;

			if (!String.IsNullOrEmpty(pathTB.Text))
			{
				string logFilePath = pathTB.Text;
				string logFileDirectory = Path.GetDirectoryName(logFilePath);
				if (Directory.Exists(logFileDirectory) == false)
				{
					string msg = string.Format(Resources.P4LoggingPreferencesControl_CreateDirectory, logFileDirectory);
					if (DialogResult.Yes == MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						Directory.CreateDirectory(logFileDirectory);
					}
					else
					{
						logFilePath = null;
					}
				}
				Preferences.LocalSettings["Log_path"] = logFilePath;
			}
			else
			{
				Preferences.LocalSettings.Remove("Log_path");
			}

			if (!String.IsNullOrEmpty(sizeTB.Text))
			{
				Preferences.LocalSettings["Log_size"] = Convert.ToInt32(sizeTB.Text);
			}
			else
			{
				Preferences.LocalSettings.Remove("Log_path");
			}
			FileLogger.Init();
		}

		private void sizeTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
			{
				e.Handled = true;
			}

			if ((sizeTB.Text.Length > 4) && (!(char.IsControl(e.KeyChar))))
			{
				e.Handled = true;
			}
		}

	}


}
