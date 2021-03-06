
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

namespace Perforce.P4VS
{
    /// <summary>
    /// Summary description for P4ToolWindowControl.
    /// </summary>
    public class P4ToolWindowControl : System.Windows.Forms.UserControl
    {
        private Label label1;

        public P4ToolWindowControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4ToolWindowControl));
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// P4ToolWindowControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.label1);
			this.Name = "P4ToolWindowControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion
    }
}
