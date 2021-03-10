
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
    /// <summary>
    /// Summary description for P4ToolWindowControl.
    /// </summary>
	public class SwarmToolWindowControl : P4ToolWindowControlBase
	{
		private Button ConnectBtn;
		private ComboBox SwarmUrlCB;
		private WebBrowser SwarmBrowser;

		public SwarmToolWindowControl()
        {
			Scm = P4VsProvider.CurrentScm;

			PreferenceKey = "SwarmToolWindowControl";
			
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
			base.Initialize();

			OnNewConnection(Scm);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwarmToolWindowControl));
			this.SwarmBrowser = new System.Windows.Forms.WebBrowser();
			this.ConnectBtn = new System.Windows.Forms.Button();
			this.SwarmUrlCB = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// SwarmBrowser
			// 
			resources.ApplyResources(this.SwarmBrowser, "SwarmBrowser");
			this.SwarmBrowser.Name = "SwarmBrowser";
			// 
			// ConnectBtn
			// 
			resources.ApplyResources(this.ConnectBtn, "ConnectBtn");
			this.ConnectBtn.Name = "ConnectBtn";
			this.ConnectBtn.UseVisualStyleBackColor = true;
			this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
			// 
			// SwarmUrlCB
			// 
			resources.ApplyResources(this.SwarmUrlCB, "SwarmUrlCB");
			this.SwarmUrlCB.FormattingEnabled = true;
			this.SwarmUrlCB.Name = "SwarmUrlCB";
			// 
			// SwarmToolWindowControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.SwarmUrlCB);
			this.Controls.Add(this.ConnectBtn);
			this.Controls.Add(this.SwarmBrowser);
			this.Name = "SwarmToolWindowControl";
			this.ResumeLayout(false);

        }
        #endregion

		public override void OnNewConnection(P4ScmProvider newScm)
		{
			if ((newScm != null) && (newScm.Connected))
			{
				IList<string> urls = newScm.GetProperties("Swarm.URL");
				if ((urls == null) || (urls.Count <= 0))
				{
                    string msg = string.Format("No Url for Swarm configured on {0}\r\n\r\n Please enter the Url for Swarm", newScm.Connection.Port);
					MessageBox.Show(msg, Resources.P4VS);
					SwarmUrlCB.Text = string.Empty;
					SwarmBrowser.Navigate("about:blank");
				}
				else
				{
					foreach (string url in urls)
					{
						SwarmUrlCB.Items.Add(url);
					}
					SwarmUrlCB.SelectedIndex = 0;

					SwarmBrowser.Navigate(SwarmUrlCB.Items[0] as string);
				}
			}
			else
			{
				SwarmUrlCB.Text = string.Empty;
				SwarmBrowser.Navigate("about:blank");
			}
		}

		private void ConnectBtn_Click(object sender, EventArgs e)
		{
			try
			{
				Uri url = new Uri(SwarmUrlCB.Text);
				SwarmBrowser.Navigate(url, null, null, null);
			}
			catch (Exception ex)
			{
				P4VsOutputWindow.AppendMessage("[E_ERROR] " + ex.Message);
				FileLogger.LogException("Swarm Connection Error", ex);
				DialogResult res = P4ErrorDlg.Show(ex.Message, false, false);
			}
		}
    }
}
