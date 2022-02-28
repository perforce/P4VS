
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

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4DataRetrievalPreferencesControl.
	/// </summary>
	public class P4DataRetrievalPreferencesControl : System.Windows.Forms.UserControl
	{

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private I18nControls.GridCheckBox AutoUpdateStatudCB;
		private I18nControls.GridLabel infoLbl;
		private I18nControls.GridTextBox updateTB;
		private I18nControls.GridTextBox numberFilesTB;
		private I18nControls.GridTextBox sizeTB;
		private I18nControls.GridTextBox numberSpecsTB;
		private I18nControls.GridLabel minutesLbl;
		private I18nControls.GridLabel label3;
		private I18nControls.GridLabel numberSpecsLbl;
		private I18nControls.GridLabel numberFilesLbl;
		private I18nControls.GridLabel allLbl;
		private I18nControls.GridLabel sizeLbl;
		private I18nControls.GridLabel updateLbl;
		private I18nControls.GridLabel dataRetrievalLbl;
		private I18nControls.GridGroupBox dataRetrievalGB;
        private I18nControls.GridCheckBox LLFullMenuCB;
        private I18nControls.GridCheckBox PreloadCacheCB;
        private I18nControls.GridCheckBox TreatProjectsAsDirsCB;
        private I18nControls.GridRadioButton NoFstatOptimizatioRDO;
        private I18nControls.GridRadioButton LazyLoadRDO;
        private I18nControls.GridRadioButton OptimizeFstatsRDO;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel2;
        private I18nControls.GridLabel gridLabel2;
        private I18nControls.GridGroupBox gridGroupBox1;
        private I18nControls.GridLabel gridLabel3;
        private I18nControls.GridLabel gridLabel1;
        private I18nControls.GridCheckBox disableParSubmitCB;
        private I18nControls.GridCheckBox disableParShelveCB;
        private I18nControls.GridCheckBox disableParSyncCB;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel3;
        private I18nControls.GridLabel gridLabel4;
        private I18nControls.GridGroupBox gridGroupBox2;
        private I18nControls.GridTextBox numberUpdatedFilesTB;
        private I18nControls.GridLabel numberUpdateFilesLbl;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;














        // The parent page, use to persist data
        private P4DataRetrievalPreferences _customPage;

		public P4DataRetrievalPreferencesControl()
		{
			P4DataRetrievalPreferencesControl.Instance = this;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4DataRetrievalPreferencesControl));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.numberUpdatedFilesTB = new Perforce.I18nControls.GridTextBox();
            this.numberUpdateFilesLbl = new Perforce.I18nControls.GridLabel();
            this.disableParSubmitCB = new Perforce.I18nControls.GridCheckBox();
            this.disableParShelveCB = new Perforce.I18nControls.GridCheckBox();
            this.disableParSyncCB = new Perforce.I18nControls.GridCheckBox();
            this.gridLayoutSubpanel3 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridLabel4 = new Perforce.I18nControls.GridLabel();
            this.gridGroupBox2 = new Perforce.I18nControls.GridGroupBox();
            this.gridLabel3 = new Perforce.I18nControls.GridLabel();
            this.gridLabel1 = new Perforce.I18nControls.GridLabel();
            this.gridLayoutSubpanel2 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridLabel2 = new Perforce.I18nControls.GridLabel();
            this.gridGroupBox1 = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.dataRetrievalLbl = new Perforce.I18nControls.GridLabel();
            this.dataRetrievalGB = new Perforce.I18nControls.GridGroupBox();
            this.LLFullMenuCB = new Perforce.I18nControls.GridCheckBox();
            this.PreloadCacheCB = new Perforce.I18nControls.GridCheckBox();
            this.TreatProjectsAsDirsCB = new Perforce.I18nControls.GridCheckBox();
            this.NoFstatOptimizatioRDO = new Perforce.I18nControls.GridRadioButton();
            this.LazyLoadRDO = new Perforce.I18nControls.GridRadioButton();
            this.OptimizeFstatsRDO = new Perforce.I18nControls.GridRadioButton();
            this.AutoUpdateStatudCB = new Perforce.I18nControls.GridCheckBox();
            this.infoLbl = new Perforce.I18nControls.GridLabel();
            this.updateTB = new Perforce.I18nControls.GridTextBox();
            this.numberFilesTB = new Perforce.I18nControls.GridTextBox();
            this.sizeTB = new Perforce.I18nControls.GridTextBox();
            this.numberSpecsTB = new Perforce.I18nControls.GridTextBox();
            this.minutesLbl = new Perforce.I18nControls.GridLabel();
            this.label3 = new Perforce.I18nControls.GridLabel();
            this.numberSpecsLbl = new Perforce.I18nControls.GridLabel();
            this.numberFilesLbl = new Perforce.I18nControls.GridLabel();
            this.allLbl = new Perforce.I18nControls.GridLabel();
            this.sizeLbl = new Perforce.I18nControls.GridLabel();
            this.updateLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel3.SuspendLayout();
            this.gridLayoutSubpanel2.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.Controls.Add(this.numberUpdatedFilesTB);
            this.gridLayoutPanel1.Controls.Add(this.numberUpdateFilesLbl);
            this.gridLayoutPanel1.Controls.Add(this.disableParSubmitCB);
            this.gridLayoutPanel1.Controls.Add(this.disableParShelveCB);
            this.gridLayoutPanel1.Controls.Add(this.disableParSyncCB);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel3);
            this.gridLayoutPanel1.Controls.Add(this.gridLabel3);
            this.gridLayoutPanel1.Controls.Add(this.gridLabel1);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel2);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.LLFullMenuCB);
            this.gridLayoutPanel1.Controls.Add(this.PreloadCacheCB);
            this.gridLayoutPanel1.Controls.Add(this.TreatProjectsAsDirsCB);
            this.gridLayoutPanel1.Controls.Add(this.NoFstatOptimizatioRDO);
            this.gridLayoutPanel1.Controls.Add(this.LazyLoadRDO);
            this.gridLayoutPanel1.Controls.Add(this.OptimizeFstatsRDO);
            this.gridLayoutPanel1.Controls.Add(this.AutoUpdateStatudCB);
            this.gridLayoutPanel1.Controls.Add(this.infoLbl);
            this.gridLayoutPanel1.Controls.Add(this.updateTB);
            this.gridLayoutPanel1.Controls.Add(this.numberFilesTB);
            this.gridLayoutPanel1.Controls.Add(this.sizeTB);
            this.gridLayoutPanel1.Controls.Add(this.numberSpecsTB);
            this.gridLayoutPanel1.Controls.Add(this.minutesLbl);
            this.gridLayoutPanel1.Controls.Add(this.label3);
            this.gridLayoutPanel1.Controls.Add(this.numberSpecsLbl);
            this.gridLayoutPanel1.Controls.Add(this.numberFilesLbl);
            this.gridLayoutPanel1.Controls.Add(this.allLbl);
            this.gridLayoutPanel1.Controls.Add(this.sizeLbl);
            this.gridLayoutPanel1.Controls.Add(this.updateLbl);
            this.gridLayoutPanel1.EnableDesignerGrid = true;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 0;
            this.gridLayoutPanel1.MinimumRowHeight = 0;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // numberUpdatedFilesTB
            // 
            resources.ApplyResources(this.numberUpdatedFilesTB, "numberUpdatedFilesTB");
            this.numberUpdatedFilesTB.CellHeight = 26;
            this.numberUpdatedFilesTB.CellWidth = 187;
            this.numberUpdatedFilesTB.Column = 2;
            this.numberUpdatedFilesTB.ColumnsSpanned = 0;
            this.numberUpdatedFilesTB.Name = "numberUpdatedFilesTB";
            this.numberUpdatedFilesTB.Row = 2;
            this.numberUpdatedFilesTB.RowsSpanned = 0;
            this.numberUpdatedFilesTB.YOffset = 0;
            // 
            // numberUpdateFilesLbl
            // 
            resources.ApplyResources(this.numberUpdateFilesLbl, "numberUpdateFilesLbl");
            this.numberUpdateFilesLbl.AutoEllipsis = true;
            this.numberUpdateFilesLbl.CellHeight = 26;
            this.numberUpdateFilesLbl.CellWidth = 208;
            this.numberUpdateFilesLbl.Column = 0;
            this.numberUpdateFilesLbl.ColumnsSpanned = 1;
            this.numberUpdateFilesLbl.Name = "numberUpdateFilesLbl";
            this.numberUpdateFilesLbl.Row = 2;
            this.numberUpdateFilesLbl.RowsSpanned = 0;
            this.numberUpdateFilesLbl.YOffset = 0;
            // 
            // disableParSubmitCB
            // 
            resources.ApplyResources(this.disableParSubmitCB, "disableParSubmitCB");
            this.disableParSubmitCB.CellHeight = 29;
            this.disableParSubmitCB.CellWidth = 571;
            this.disableParSubmitCB.Column = 0;
            this.disableParSubmitCB.ColumnsSpanned = 3;
            this.disableParSubmitCB.Name = "disableParSubmitCB";
            this.disableParSubmitCB.Row = 17;
            this.disableParSubmitCB.RowsSpanned = 0;
            this.disableParSubmitCB.UseVisualStyleBackColor = true;
            this.disableParSubmitCB.YOffset = 0;
            // 
            // disableParShelveCB
            // 
            resources.ApplyResources(this.disableParShelveCB, "disableParShelveCB");
            this.disableParShelveCB.CellHeight = 27;
            this.disableParShelveCB.CellWidth = 571;
            this.disableParShelveCB.Column = 0;
            this.disableParShelveCB.ColumnsSpanned = 3;
            this.disableParShelveCB.Name = "disableParShelveCB";
            this.disableParShelveCB.Row = 18;
            this.disableParShelveCB.RowsSpanned = 0;
            this.disableParShelveCB.UseVisualStyleBackColor = true;
            this.disableParShelveCB.YOffset = 0;
            // 
            // disableParSyncCB
            // 
            resources.ApplyResources(this.disableParSyncCB, "disableParSyncCB");
            this.disableParSyncCB.CellHeight = 29;
            this.disableParSyncCB.CellWidth = 571;
            this.disableParSyncCB.Column = 0;
            this.disableParSyncCB.ColumnsSpanned = 3;
            this.disableParSyncCB.Name = "disableParSyncCB";
            this.disableParSyncCB.Row = 16;
            this.disableParSyncCB.RowsSpanned = 0;
            this.disableParSyncCB.UseVisualStyleBackColor = true;
            this.disableParSyncCB.YOffset = 0;
            // 
            // gridLayoutSubpanel3
            // 
            resources.ApplyResources(this.gridLayoutSubpanel3, "gridLayoutSubpanel3");
            this.gridLayoutSubpanel3.CellHeight = 29;
            this.gridLayoutSubpanel3.CellWidth = 571;
            this.gridLayoutSubpanel3.Column = 0;
            this.gridLayoutSubpanel3.ColumnsSpanned = 3;
            this.gridLayoutSubpanel3.Controls.Add(this.gridLabel4);
            this.gridLayoutSubpanel3.Controls.Add(this.gridGroupBox2);
            this.gridLayoutSubpanel3.EnableDesignerGrid = false;
            this.gridLayoutSubpanel3.EnableDesignerLayout = false;
            this.gridLayoutSubpanel3.EnableParentResize = false;
            this.gridLayoutSubpanel3.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel3.MinimumRowHeight = 10;
            this.gridLayoutSubpanel3.Name = "gridLayoutSubpanel3";
            this.gridLayoutSubpanel3.Row = 15;
            this.gridLayoutSubpanel3.RowsSpanned = 0;
            this.gridLayoutSubpanel3.YOffset = 0;
            // 
            // gridLabel4
            // 
            resources.ApplyResources(this.gridLabel4, "gridLabel4");
            this.gridLabel4.CellHeight = 13;
            this.gridLabel4.CellWidth = 101;
            this.gridLabel4.Column = 0;
            this.gridLabel4.ColumnsSpanned = 0;
            this.gridLabel4.Name = "gridLabel4";
            this.gridLabel4.Row = 0;
            this.gridLabel4.RowsSpanned = 0;
            this.gridLabel4.YOffset = 0;
            // 
            // gridGroupBox2
            // 
            resources.ApplyResources(this.gridGroupBox2, "gridGroupBox2");
            this.gridGroupBox2.CellHeight = 13;
            this.gridGroupBox2.CellWidth = 470;
            this.gridGroupBox2.Column = 1;
            this.gridGroupBox2.ColumnsSpanned = 0;
            this.gridGroupBox2.Name = "gridGroupBox2";
            this.gridGroupBox2.Row = 0;
            this.gridGroupBox2.RowsSpanned = 0;
            this.gridGroupBox2.TabStop = false;
            this.gridGroupBox2.YOffset = 5;
            // 
            // gridLabel3
            // 
            resources.ApplyResources(this.gridLabel3, "gridLabel3");
            this.gridLabel3.CellHeight = 38;
            this.gridLabel3.CellWidth = 571;
            this.gridLabel3.Column = 0;
            this.gridLabel3.ColumnsSpanned = 3;
            this.gridLabel3.Name = "gridLabel3";
            this.gridLabel3.Row = 14;
            this.gridLabel3.RowsSpanned = 0;
            this.gridLabel3.YOffset = 0;
            // 
            // gridLabel1
            // 
            resources.ApplyResources(this.gridLabel1, "gridLabel1");
            this.gridLabel1.CellHeight = 0;
            this.gridLabel1.CellWidth = 0;
            this.gridLabel1.Column = 0;
            this.gridLabel1.ColumnsSpanned = 0;
            this.gridLabel1.Name = "gridLabel1";
            this.gridLabel1.Row = 0;
            this.gridLabel1.RowsSpanned = 0;
            this.gridLabel1.YOffset = 0;
            // 
            // gridLayoutSubpanel2
            // 
            resources.ApplyResources(this.gridLayoutSubpanel2, "gridLayoutSubpanel2");
            this.gridLayoutSubpanel2.CellHeight = 29;
            this.gridLayoutSubpanel2.CellWidth = 571;
            this.gridLayoutSubpanel2.Column = 0;
            this.gridLayoutSubpanel2.ColumnsSpanned = 3;
            this.gridLayoutSubpanel2.Controls.Add(this.gridLabel2);
            this.gridLayoutSubpanel2.Controls.Add(this.gridGroupBox1);
            this.gridLayoutSubpanel2.EnableDesignerGrid = false;
            this.gridLayoutSubpanel2.EnableDesignerLayout = false;
            this.gridLayoutSubpanel2.EnableParentResize = false;
            this.gridLayoutSubpanel2.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel2.MinimumRowHeight = 10;
            this.gridLayoutSubpanel2.Name = "gridLayoutSubpanel2";
            this.gridLayoutSubpanel2.Row = 7;
            this.gridLayoutSubpanel2.RowsSpanned = 0;
            this.gridLayoutSubpanel2.YOffset = 0;
            // 
            // gridLabel2
            // 
            resources.ApplyResources(this.gridLabel2, "gridLabel2");
            this.gridLabel2.CellHeight = 13;
            this.gridLabel2.CellWidth = 140;
            this.gridLabel2.Column = 0;
            this.gridLabel2.ColumnsSpanned = 0;
            this.gridLabel2.Name = "gridLabel2";
            this.gridLabel2.Row = 0;
            this.gridLabel2.RowsSpanned = 0;
            this.gridLabel2.YOffset = 0;
            // 
            // gridGroupBox1
            // 
            resources.ApplyResources(this.gridGroupBox1, "gridGroupBox1");
            this.gridGroupBox1.CellHeight = 13;
            this.gridGroupBox1.CellWidth = 431;
            this.gridGroupBox1.Column = 1;
            this.gridGroupBox1.ColumnsSpanned = 0;
            this.gridGroupBox1.Name = "gridGroupBox1";
            this.gridGroupBox1.Row = 0;
            this.gridGroupBox1.RowsSpanned = 0;
            this.gridGroupBox1.TabStop = false;
            this.gridGroupBox1.YOffset = 5;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 28;
            this.gridLayoutSubpanel1.CellWidth = 571;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 3;
            this.gridLayoutSubpanel1.Controls.Add(this.dataRetrievalLbl);
            this.gridLayoutSubpanel1.Controls.Add(this.dataRetrievalGB);
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
            // dataRetrievalLbl
            // 
            resources.ApplyResources(this.dataRetrievalLbl, "dataRetrievalLbl");
            this.dataRetrievalLbl.CellHeight = 13;
            this.dataRetrievalLbl.CellWidth = 76;
            this.dataRetrievalLbl.Column = 0;
            this.dataRetrievalLbl.ColumnsSpanned = 0;
            this.dataRetrievalLbl.Name = "dataRetrievalLbl";
            this.dataRetrievalLbl.Row = 0;
            this.dataRetrievalLbl.RowsSpanned = 0;
            this.dataRetrievalLbl.YOffset = 0;
            // 
            // dataRetrievalGB
            // 
            resources.ApplyResources(this.dataRetrievalGB, "dataRetrievalGB");
            this.dataRetrievalGB.CellHeight = 13;
            this.dataRetrievalGB.CellWidth = 489;
            this.dataRetrievalGB.Column = 1;
            this.dataRetrievalGB.ColumnsSpanned = 0;
            this.dataRetrievalGB.Name = "dataRetrievalGB";
            this.dataRetrievalGB.Row = 0;
            this.dataRetrievalGB.RowsSpanned = 0;
            this.dataRetrievalGB.TabStop = false;
            this.dataRetrievalGB.YOffset = 5;
            // 
            // LLFullMenuCB
            // 
            resources.ApplyResources(this.LLFullMenuCB, "LLFullMenuCB");
            this.LLFullMenuCB.CellHeight = 23;
            this.LLFullMenuCB.CellWidth = 571;
            this.LLFullMenuCB.Column = 0;
            this.LLFullMenuCB.ColumnsSpanned = 3;
            this.LLFullMenuCB.Name = "LLFullMenuCB";
            this.LLFullMenuCB.Row = 12;
            this.LLFullMenuCB.RowsSpanned = 0;
            this.LLFullMenuCB.UseVisualStyleBackColor = true;
            this.LLFullMenuCB.YOffset = 0;
            // 
            // PreloadCacheCB
            // 
            resources.ApplyResources(this.PreloadCacheCB, "PreloadCacheCB");
            this.PreloadCacheCB.CellHeight = 23;
            this.PreloadCacheCB.CellWidth = 571;
            this.PreloadCacheCB.Column = 0;
            this.PreloadCacheCB.ColumnsSpanned = 3;
            this.PreloadCacheCB.Name = "PreloadCacheCB";
            this.PreloadCacheCB.Row = 10;
            this.PreloadCacheCB.RowsSpanned = 0;
            this.PreloadCacheCB.UseVisualStyleBackColor = true;
            this.PreloadCacheCB.YOffset = 0;
            // 
            // TreatProjectsAsDirsCB
            // 
            resources.ApplyResources(this.TreatProjectsAsDirsCB, "TreatProjectsAsDirsCB");
            this.TreatProjectsAsDirsCB.CellHeight = 23;
            this.TreatProjectsAsDirsCB.CellWidth = 571;
            this.TreatProjectsAsDirsCB.Column = 0;
            this.TreatProjectsAsDirsCB.ColumnsSpanned = 3;
            this.TreatProjectsAsDirsCB.Name = "TreatProjectsAsDirsCB";
            this.TreatProjectsAsDirsCB.Row = 9;
            this.TreatProjectsAsDirsCB.RowsSpanned = 0;
            this.TreatProjectsAsDirsCB.UseVisualStyleBackColor = true;
            this.TreatProjectsAsDirsCB.YOffset = 0;
            // 
            // NoFstatOptimizatioRDO
            // 
            resources.ApplyResources(this.NoFstatOptimizatioRDO, "NoFstatOptimizatioRDO");
            this.NoFstatOptimizatioRDO.CellHeight = 29;
            this.NoFstatOptimizatioRDO.CellWidth = 571;
            this.NoFstatOptimizatioRDO.Column = 0;
            this.NoFstatOptimizatioRDO.ColumnsSpanned = 3;
            this.NoFstatOptimizatioRDO.Name = "NoFstatOptimizatioRDO";
            this.NoFstatOptimizatioRDO.Row = 13;
            this.NoFstatOptimizatioRDO.RowsSpanned = 0;
            this.NoFstatOptimizatioRDO.TabStop = true;
            this.NoFstatOptimizatioRDO.UseVisualStyleBackColor = true;
            this.NoFstatOptimizatioRDO.YOffset = 0;
            this.NoFstatOptimizatioRDO.CheckedChanged += new System.EventHandler(this.NoFstatOptimizatioRDO_CheckedChanged);
            // 
            // LazyLoadRDO
            // 
            resources.ApplyResources(this.LazyLoadRDO, "LazyLoadRDO");
            this.LazyLoadRDO.CellHeight = 29;
            this.LazyLoadRDO.CellWidth = 571;
            this.LazyLoadRDO.Column = 0;
            this.LazyLoadRDO.ColumnsSpanned = 3;
            this.LazyLoadRDO.Name = "LazyLoadRDO";
            this.LazyLoadRDO.Row = 11;
            this.LazyLoadRDO.RowsSpanned = 0;
            this.LazyLoadRDO.TabStop = true;
            this.LazyLoadRDO.UseVisualStyleBackColor = true;
            this.LazyLoadRDO.YOffset = 0;
            this.LazyLoadRDO.CheckedChanged += new System.EventHandler(this.LazyLoadRDO_CheckedChanged);
            // 
            // OptimizeFstatsRDO
            // 
            resources.ApplyResources(this.OptimizeFstatsRDO, "OptimizeFstatsRDO");
            this.OptimizeFstatsRDO.CellHeight = 29;
            this.OptimizeFstatsRDO.CellWidth = 571;
            this.OptimizeFstatsRDO.Column = 0;
            this.OptimizeFstatsRDO.ColumnsSpanned = 3;
            this.OptimizeFstatsRDO.Name = "OptimizeFstatsRDO";
            this.OptimizeFstatsRDO.Row = 8;
            this.OptimizeFstatsRDO.RowsSpanned = 0;
            this.OptimizeFstatsRDO.TabStop = true;
            this.OptimizeFstatsRDO.UseVisualStyleBackColor = true;
            this.OptimizeFstatsRDO.YOffset = 0;
            this.OptimizeFstatsRDO.CheckedChanged += new System.EventHandler(this.OptimizeFstatsRDO_CheckedChanged);
            // 
            // AutoUpdateStatudCB
            // 
            resources.ApplyResources(this.AutoUpdateStatudCB, "AutoUpdateStatudCB");
            this.AutoUpdateStatudCB.CellHeight = 29;
            this.AutoUpdateStatudCB.CellWidth = 571;
            this.AutoUpdateStatudCB.Column = 0;
            this.AutoUpdateStatudCB.ColumnsSpanned = 3;
            this.AutoUpdateStatudCB.Name = "AutoUpdateStatudCB";
            this.AutoUpdateStatudCB.Row = 5;
            this.AutoUpdateStatudCB.RowsSpanned = 0;
            this.AutoUpdateStatudCB.UseVisualStyleBackColor = true;
            this.AutoUpdateStatudCB.YOffset = 0;
            // 
            // infoLbl
            // 
            resources.ApplyResources(this.infoLbl, "infoLbl");
            this.infoLbl.AutoEllipsis = true;
            this.infoLbl.CellHeight = 37;
            this.infoLbl.CellWidth = 571;
            this.infoLbl.Column = 0;
            this.infoLbl.ColumnsSpanned = 3;
            this.infoLbl.Name = "infoLbl";
            this.infoLbl.Row = 6;
            this.infoLbl.RowsSpanned = 0;
            this.infoLbl.YOffset = 0;
            // 
            // updateTB
            // 
            resources.ApplyResources(this.updateTB, "updateTB");
            this.updateTB.CellHeight = 32;
            this.updateTB.CellWidth = 174;
            this.updateTB.Column = 2;
            this.updateTB.ColumnsSpanned = 0;
            this.updateTB.Name = "updateTB";
            this.updateTB.Row = 1;
            this.updateTB.RowsSpanned = 0;
            this.updateTB.YOffset = 0;
            // 
            // numberFilesTB
            // 
            resources.ApplyResources(this.numberFilesTB, "numberFilesTB");
            this.numberFilesTB.CellHeight = 32;
            this.numberFilesTB.CellWidth = 174;
            this.numberFilesTB.Column = 2;
            this.numberFilesTB.ColumnsSpanned = 0;
            this.numberFilesTB.Name = "numberFilesTB";
            this.numberFilesTB.Row = 2;
            this.numberFilesTB.RowsSpanned = 0;
            this.numberFilesTB.YOffset = 0;
            // 
            // sizeTB
            // 
            resources.ApplyResources(this.sizeTB, "sizeTB");
            this.sizeTB.CellHeight = 32;
            this.sizeTB.CellWidth = 174;
            this.sizeTB.Column = 2;
            this.sizeTB.ColumnsSpanned = 0;
            this.sizeTB.Name = "sizeTB";
            this.sizeTB.Row = 3;
            this.sizeTB.RowsSpanned = 0;
            this.sizeTB.YOffset = 0;
            // 
            // numberSpecsTB
            // 
            resources.ApplyResources(this.numberSpecsTB, "numberSpecsTB");
            this.numberSpecsTB.CellHeight = 32;
            this.numberSpecsTB.CellWidth = 174;
            this.numberSpecsTB.Column = 2;
            this.numberSpecsTB.ColumnsSpanned = 0;
            this.numberSpecsTB.Name = "numberSpecsTB";
            this.numberSpecsTB.Row = 4;
            this.numberSpecsTB.RowsSpanned = 0;
            this.numberSpecsTB.YOffset = 0;
            // 
            // minutesLbl
            // 
            resources.ApplyResources(this.minutesLbl, "minutesLbl");
            this.minutesLbl.AutoEllipsis = true;
            this.minutesLbl.CellHeight = 32;
            this.minutesLbl.CellWidth = 105;
            this.minutesLbl.Column = 3;
            this.minutesLbl.ColumnsSpanned = 0;
            this.minutesLbl.Name = "minutesLbl";
            this.minutesLbl.Row = 1;
            this.minutesLbl.RowsSpanned = 0;
            this.minutesLbl.YOffset = 3;
            this.minutesLbl.SizeChanged += new System.EventHandler(this.label3_SizeChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.AutoEllipsis = true;
            this.label3.CellHeight = 32;
            this.label3.CellWidth = 105;
            this.label3.Column = 3;
            this.label3.ColumnsSpanned = 0;
            this.label3.Name = "label3";
            this.label3.Row = 3;
            this.label3.RowsSpanned = 0;
            this.label3.YOffset = 3;
            this.label3.SizeChanged += new System.EventHandler(this.label3_SizeChanged);
            // 
            // numberSpecsLbl
            // 
            resources.ApplyResources(this.numberSpecsLbl, "numberSpecsLbl");
            this.numberSpecsLbl.AutoEllipsis = true;
            this.numberSpecsLbl.CellHeight = 32;
            this.numberSpecsLbl.CellWidth = 292;
            this.numberSpecsLbl.Column = 0;
            this.numberSpecsLbl.ColumnsSpanned = 1;
            this.numberSpecsLbl.Name = "numberSpecsLbl";
            this.numberSpecsLbl.Row = 4;
            this.numberSpecsLbl.RowsSpanned = 0;
            this.numberSpecsLbl.YOffset = 3;
            // 
            // numberFilesLbl
            // 
            resources.ApplyResources(this.numberFilesLbl, "numberFilesLbl");
            this.numberFilesLbl.AutoEllipsis = true;
            this.numberFilesLbl.CellHeight = 32;
            this.numberFilesLbl.CellWidth = 292;
            this.numberFilesLbl.Column = 0;
            this.numberFilesLbl.ColumnsSpanned = 1;
            this.numberFilesLbl.Name = "numberFilesLbl";
            this.numberFilesLbl.Row = 2;
            this.numberFilesLbl.RowsSpanned = 0;
            this.numberFilesLbl.YOffset = 3;
            // 
            // allLbl
            // 
            resources.ApplyResources(this.allLbl, "allLbl");
            this.allLbl.AutoEllipsis = true;
            this.allLbl.CellHeight = 32;
            this.allLbl.CellWidth = 105;
            this.allLbl.Column = 3;
            this.allLbl.ColumnsSpanned = 0;
            this.allLbl.Name = "allLbl";
            this.allLbl.Row = 4;
            this.allLbl.RowsSpanned = 0;
            this.allLbl.YOffset = 3;
            this.allLbl.SizeChanged += new System.EventHandler(this.label3_SizeChanged);
            // 
            // sizeLbl
            // 
            resources.ApplyResources(this.sizeLbl, "sizeLbl");
            this.sizeLbl.AutoEllipsis = true;
            this.sizeLbl.CellHeight = 32;
            this.sizeLbl.CellWidth = 292;
            this.sizeLbl.Column = 0;
            this.sizeLbl.ColumnsSpanned = 1;
            this.sizeLbl.Name = "sizeLbl";
            this.sizeLbl.Row = 3;
            this.sizeLbl.RowsSpanned = 0;
            this.sizeLbl.YOffset = 3;
            // 
            // updateLbl
            // 
            resources.ApplyResources(this.updateLbl, "updateLbl");
            this.updateLbl.AutoEllipsis = true;
            this.updateLbl.CellHeight = 32;
            this.updateLbl.CellWidth = 292;
            this.updateLbl.Column = 0;
            this.updateLbl.ColumnsSpanned = 1;
            this.updateLbl.Name = "updateLbl";
            this.updateLbl.Row = 1;
            this.updateLbl.RowsSpanned = 0;
            this.updateLbl.YOffset = 3;
            // 
            // P4DataRetrievalPreferencesControl
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "P4DataRetrievalPreferencesControl";
            this.Load += new System.EventHandler(this.P4DataRetrievalPreferencesControl_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridLayoutSubpanel3.ResumeLayout(false);
            this.gridLayoutSubpanel3.PerformLayout();
            this.gridLayoutSubpanel2.ResumeLayout(false);
            this.gridLayoutSubpanel2.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public P4DataRetrievalPreferences OptionsPage
		{
			set
			{
				_customPage = value;
			}
		}

		private void P4DataRetrievalPreferencesControl_Load(object sender, EventArgs e)
		{
			bool LocalIgnore = false;
			if (P4IgnorePreferencesControl.Instance != null)
			{
				LocalIgnore = P4IgnorePreferencesControl.Instance.LocalIgnoreChkChecked;
			}
			else if (P4IgnorePreferencesControl.Scm != null)
			{
				LocalIgnore = P4IgnorePreferencesControl.Scm.EnforceLocalIgnore;
			}

			if (Preferences.LocalSettings.ContainsKey("Update_status"))
			{
				updateTB.Text = Preferences.LocalSettings["Update_status"].ToString();
			}

			if (LocalIgnore)
			{
				UpdateIntervalEnabled = false;
			}
			else
			{
				updateTB.Text = Preferences.LocalSettings.GetInt("Update_status", 5).ToString();
			}
			numberFilesTB.Text = Preferences.LocalSettings.GetInt("Number_files",1000).ToString();
            numberUpdatedFilesTB.Text = Preferences.LocalSettings.GetInt("Number_files_cache", 500).ToString();
            sizeTB.Text = Preferences.LocalSettings.GetInt("Size_files", 500).ToString();

			numberSpecsTB.Text = Preferences.LocalSettings.GetInt("Number_specs",100).ToString();

			AutoUpdateStatudCB.Checked = Preferences.LocalSettings.GetBool("AutoUpdateFileData", false);

            bool TreatProjectsAsDirs = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", false);
            bool LazyLoad = Preferences.LocalSettings.GetBool("LazyLoadStatus", false);
            bool PreloadCache = Preferences.LocalSettings.GetBool("PreloadScmCache", true);
            bool LLFullMenu = Preferences.LocalSettings.GetBool("LazyLoadFullMenu", false);

            if (LazyLoad)
            {
                TreatProjectsAsDirs = false;
                TreatProjectsAsDirsCB.Checked = false;

                PreloadCache = false;
                PreloadCacheCB.Checked = false;

                LazyLoadRDO.Checked = true;

                LLFullMenuCB.Checked = LLFullMenu;
            }
            else if (LocalIgnore || !(TreatProjectsAsDirs || PreloadCache))
            {
                TreatProjectsAsDirs = false;
                TreatProjectsAsDirsCB.Checked = false;

                PreloadCache = false;
                PreloadCacheCB.Checked = false;

                NoFstatOptimizatioRDO.Checked = true;

                LLFullMenuCB.Checked = LLFullMenu;
            }
            else
            {
                OptimizeFstatsRDO.Checked = true;
                TreatProjectsAsDirsCB.Checked = TreatProjectsAsDirs;
                PreloadCacheCB.Checked = PreloadCache;
            }

            disableParSyncCB.Checked = Preferences.LocalSettings.GetBool("DisableParallelSync", false);
            disableParSubmitCB.Checked = Preferences.LocalSettings.GetBool("DisableParallelSubmit", false);
            disableParShelveCB.Checked = Preferences.LocalSettings.GetBool("DisableParallelShelve", false);
        }

        private Int32 SafeConvertToInt32(string str)
        {
            Int32 v = 0;
            if (Int32.TryParse(str, out v))
            {
                return v;
            }
            return 0;
        }

        public void OnApply()
        {
            if (!String.IsNullOrEmpty(updateTB.Text))
            {
                Preferences.LocalSettings["Update_status"] = SafeConvertToInt32(updateTB.Text);
            }
            if (!String.IsNullOrEmpty(numberFilesTB.Text))
            {
                Preferences.LocalSettings["Number_files"] = SafeConvertToInt32(numberFilesTB.Text);
            }
            if (!String.IsNullOrEmpty(numberUpdatedFilesTB.Text))
            {
                Preferences.LocalSettings["Number_files_cache"] = SafeConvertToInt32(numberUpdatedFilesTB.Text);
            }
            if (!String.IsNullOrEmpty(sizeTB.Text))
            {
                Preferences.LocalSettings["Size_files"] = SafeConvertToInt32(sizeTB.Text);
            }
            if (!String.IsNullOrEmpty(numberSpecsTB.Text))
            {
                Preferences.LocalSettings["Number_specs"] = SafeConvertToInt32(numberSpecsTB.Text);
            }

            Preferences.LocalSettings["AutoUpdateFileData"] = AutoUpdateStatudCB.Checked;

            Preferences.LocalSettings["LazyLoadStatus"] = LazyLoadRDO.Checked;
            if (LazyLoadRDO.Checked)
            {
                Preferences.LocalSettings["TreatProjectsAsFolders"] = false;
                Preferences.LocalSettings["PreloadScmCache"] = false;
                Preferences.LocalSettings["LazyLoadFullMenu"] = LLFullMenuCB.Checked;
            }
            else
            {
                Preferences.LocalSettings["TreatProjectsAsFolders"] = TreatProjectsAsDirsCB.Checked && !NoFstatOptimizatioRDO.Checked;
                Preferences.LocalSettings["PreloadScmCache"] = PreloadCacheCB.Checked && !NoFstatOptimizatioRDO.Checked;
                Preferences.LocalSettings["LazyLoadFullMenu"] = LLFullMenuCB.Checked;
            }

            Preferences.LocalSettings["DisableParallelSync"] = disableParSyncCB.Checked;
            Preferences.LocalSettings["DisableParallelSubmit"] = disableParSubmitCB.Checked;
            Preferences.LocalSettings["DisableParallelShelve"] = disableParShelveCB.Checked;
        }

        private void updateTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
			{
				e.Handled = true;
			}

			if ((updateTB.Text.Length > 3) && (!(char.IsControl(e.KeyChar))))
			{
				e.Handled = true;
			}
		}

		private void numberFilesTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
			{
				e.Handled = true;
			}

			if ((numberFilesTB.Text.Length > 6) && (!(char.IsControl(e.KeyChar))))
			{
				e.Handled = true;
			}
        }

        private void numberUpdatedFilesTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }

            if ((numberUpdatedFilesTB.Text.Length > 6) && (!(char.IsControl(e.KeyChar))))
            {
                e.Handled = true;
            }
        }

        private void sizeTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
			{
				e.Handled = true;
			}
			if ((sizeTB.Text.Length > 6) && (!(char.IsControl(e.KeyChar))))
			{
				e.Handled = true;
			}
		}

		private void numberSpecsTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
			{
				e.Handled = true;
			}

			if ((numberSpecsTB.Text.Length > 4) && (!(char.IsControl(e.KeyChar))))
			{
				e.Handled = true;
			}
		}

		public static P4DataRetrievalPreferencesControl Instance { get; private set; }

		public bool UpdateIntervalEnabled
		{
			get { return updateTB.Enabled; }
			set
			{
				updateTB.Enabled = value;
				updateLbl.Enabled = value;
				minutesLbl.Enabled = value;
				if (!value)
				{
					updateTB.Text = "0";
				}
				else
				{
					updateTB.Text = Preferences.LocalSettings.GetInt("Update_status",5).ToString();
				}
			}
		}

		private void label3_SizeChanged(object sender, EventArgs e)
		{
			//gridLayoutPanel1.OnSizeChanged();
		}

        private void OptimizeFstatsRDO_CheckedChanged(object sender, EventArgs e)
        {
            RDO_CheckedChanged(sender, e);
        }

        private void LazyLoadRDO_CheckedChanged(object sender, EventArgs e)
        {
            RDO_CheckedChanged(sender, e);
        }

        private void NoFstatOptimizatioRDO_CheckedChanged(object sender, EventArgs e)
        {
            RDO_CheckedChanged(sender, e);
        }
        private void RDO_CheckedChanged(object sender, EventArgs e)
        {
            TreatProjectsAsDirsCB.Enabled = OptimizeFstatsRDO.Checked;
            PreloadCacheCB.Enabled = OptimizeFstatsRDO.Checked;

            LLFullMenuCB.Enabled = LazyLoadRDO.Checked;


        }
    }
}
