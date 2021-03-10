using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Perforce.P4VS.UI
{
	public partial class ThreadMonitorControl : UserControl
	{
		public ThreadMonitorControl()
		{
			InitializeComponent();
		}

		public new bool Visible
		{
			get { return base.Visible; }
			set
			{
				base.Visible = value;
				ProgressBar.Visible = value;
				CancelBtn.Visible = value;
			}
		}

		private bool _cancelPressed { get; set; }
		public bool CancelPressed 
		{
			get { return _cancelPressed; }
			set
			{
				if (value)
				{
					CancelBtn_Click(null, null);
				}
				_cancelPressed = value;
			}
		}
		private delegate void ShowHideDelegate();

		public void Show(int max)
		{
			Show(null, max);
		}

		private void Show(Thread thread, int max)
		{
			try
			{
				CancelPressed = false;

				Monitored = thread;
				Maximum = max;
				if (InvokeRequired)
				{
					Invoke(new ShowHideDelegate(Show));
				}
				else
				{
					Show();
				}
			}
			catch { }
		}

		public new void Show()
		{
			if (Parent != null)
			{
				Left = Parent.Left + 75;
				Width = Parent.Width - 150;
				Height = 46;

				CancelBtn.Top = 12;
				CancelBtn.Left = 11;
				CancelBtn.Width = 59;
				CancelBtn.Height = 23;

				ProgressBar.Top = 12;
				ProgressBar.Left = 76;

				ProgressBar.Width = Width - 88;
				ProgressBar.Height = 23;
			}
			//else if (Container != null)
			//{
			//    Left = Container.Left + 75;
			//    Width = Parent.Width - 150;
			//}
			BringToFront();
			this.Visible = true;
		}

		private void HideInt()
		{
			if (this.Visible)
			{
				SendToBack();
				this.Visible = false;
			}
		}

		public new void Hide()
		{
			if (Visible)
			{
				try
				{
					if (InvokeRequired)
					{
						Invoke(new ShowHideDelegate(HideInt));
					}
					else
					{
						HideInt();
					}
				}
				catch { }
			}
		}

		private delegate void ProgressBarSetDelegate(int value);

		private void ProgressBarSetMaximum(int value)
		{
			ProgressBar.Maximum = value;
		}

		private void ProgressBarSetStep(int value)
		{
			ProgressBar.Step = value;
		}

		private void ProgressBarSetValue(int value)
		{
			ProgressBar.Value = value;
			BringToFront();
		}

		public int Maximum
		{
			get { return ProgressBar.Maximum; }
			set
			{
				try
				{
					if (ProgressBar.InvokeRequired)
					{
						ProgressBar.Invoke(new ProgressBarSetDelegate(ProgressBarSetMaximum), value);
					}
					else
					{
						ProgressBar.Maximum = value;
					}
				}
				catch { }
			}
		}

		public int Step
		{
			get { return ProgressBar.Step; }
			set
			{
				try
				{
					if (ProgressBar.InvokeRequired)
					{
						ProgressBar.Invoke(new ProgressBarSetDelegate(ProgressBarSetStep), value);
					}
					else
					{
						ProgressBar.Step = value;
					}
				}
				catch { }
			}
		}

		public int Value
		{
			get { return ProgressBar.Value; }
			set
			{
				try
				{
					if (value > Maximum)
					{
						value = Maximum;
					}
					if (ProgressBar.InvokeRequired)
					{
						ProgressBar.Invoke(new ProgressBarSetDelegate(ProgressBarSetValue), value);
					}
					else
					{
						ProgressBar.Value = value;
						BringToFront();
					}
				}
				catch { }
			}
		}

		public Thread Monitored = null;

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			_cancelPressed = true;
			if ((Monitored != null) && Monitored.IsAlive)
			{
				Monitored.Abort();
			}
		}
	}
}
