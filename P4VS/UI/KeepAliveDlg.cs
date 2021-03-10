using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Perforce.P4VS
{
	public partial class KeepAliveDlg : AutoSizeForm, IDisposable
	{
		private uint CommandId { get; set; }

		//Thread AnimateProgressBarThread = null;

		public KeepAliveDlg(uint commandId, string cmdLine)
		{
			IsShown = false;
			CommandId = commandId;

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			CommandLine.Text = cmdLine;

			ClosedEvent = new AutoResetEvent(true);
		}

		public void CommandCompleted()
		{
			progressBar1.Visible = false;
			CancelingCmdLbl.Visible = true;

			this.DialogResult = DialogResult.OK;

			if (this.IsShown)
			{
				Close();
			}
		}

		public void Cancel_Int()
		{
			CancelCommandBtn.Enabled = false;
			this.ControlBox = true;
			this.DialogResult = DialogResult.Cancel;

			if (this.IsShown)
			{
				Close();
			}
			//AnimateProgressBarThread.Abort();
			//AnimateProgressBarThread.Join(TimeSpan.FromSeconds(2));
		}

		public void Cancel()
		{
			if (CancelCommandBtn.InvokeRequired || this.InvokeRequired)
			{
				CancelCommandBtn.Invoke(new VoidVoidDelegate(Cancel_Int));
			}
			else
			{
				Cancel_Int();
			}
			//AnimateProgressBarThread.Abort();
			//AnimateProgressBarThread.Join(TimeSpan.FromSeconds(2));
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Cancel();
		}

		public delegate void VoidVoidDelegate();

		public AutoResetEvent ClosedEvent { get; private set; }
		//private void AdvanceProgressBar()
		//{
		//    //progressBar1.Value = (progressBar1.Value+1) % progressBar1.Maximum;
		//}

		public bool IsShown { get; private set; }

		protected override void OnShown(EventArgs e)
		{
			IsShown = true;
			base.OnShown(e);
		}

		protected override void OnClosed(EventArgs e)
		{
			ClosedEvent.Set();
			IsShown = false;
			base.OnClosed(e);
			ClosedEvent.Dispose();
		}
		//private void StartProgressBar()
		//{
		//    //AnimateProgressBarThread = new Thread(new ThreadStart(AnimateProgressBarThreadProc));
		//    //AnimateProgressBarThread.Start();
		//}

		//private void AnimateProgressBarThreadProc()
		//{
		//    try
		//    {
		//        VoidVoidDelegate del = new VoidVoidDelegate(AdvanceProgressBar);
		//        while (true)
		//        {
		//            if (progressBar1.InvokeRequired)
		//            {
		//                progressBar1.Invoke(del);
		//            }
		//            else
		//            {
		//                AdvanceProgressBar();
		//            }
		//            Thread.Sleep(10);
		//        }
		//    }
		//    catch (ThreadAbortException) 
		//    {
		//        Thread.ResetAbort();
		//        return; 
		//    }
		//    catch (Exception ex)
		//    {
		//        System.Diagnostics.Trace.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
		//        return; 
		//    }
		//}

		private void KeepAliveDlg_Shown(object sender, EventArgs e)
		{
			//StartProgressBar();
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if (ClosedEvent != null)
			{
				ClosedEvent.Dispose();
				ClosedEvent = null;
			}
		}

		#endregion
	}
}
