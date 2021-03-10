using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Perforce;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class SccHistoryLabelsControl : UserControl
	{
		public SccHistoryLabelsControl()
		{
			InitializeComponent();
		}

		public new bool Visible
		{
			get { return base.Visible; }
			set
			{
				base.Visible = value;
				if (value)
				{
					BringToFront();
				}
				else
				{
					SendToBack();
				}
				foreach (Control c in Controls)
				{
					c.Visible = value;
					if (value)
					{
						c.BringToFront();
					}
					else
					{
						c.SendToBack();
					}
				}
			}
		}


		public P4ScmProvider Scm { get; set; }

		private P4.FileSpec _file;
		public P4.FileSpec File
		{
			get { return _file; }
			set
			{
				if (_file == value)
				{
					return;
				}
				_file = value;
				LabelsListVew.Items.Clear();

				if (_file == null)
				{
					return;
				}

				IList<P4.Label> labels = Scm.GetLabels(File);

				if (labels == null)
				{
					return;
				}
				foreach (P4.Label label in labels)
				{
				    string access = "";
				    string update = "";
                    if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                    {
                        DateTime localAccess = label.Access;
                        // we need a pref for local time, until then, don't do this:
                        //DateTime localAccess = TimeZone.CurrentTimeZone.ToLocalTime(label.Access);
                        access = localAccess.ToString("yyyy/MM/dd HH:mm:ss");

                        DateTime localUpdate = label.Update;
                        // we need a pref for local time, until then, don't do this:
                        //DateTime localUpdate = TimeZone.CurrentTimeZone.ToLocalTime(label.Update);
                        update = localUpdate.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    else
                    {
                        DateTime localAccess = label.Access;
                        // we need a pref for local time, until then, don't do this:
                        //DateTime localAccess = TimeZone.CurrentTimeZone.ToLocalTime(label.Access);
                        access = string.Format("{0} {1}", localAccess.ToShortDateString(),
                                                            localAccess.ToShortTimeString());
                        DateTime localUpdate = label.Update;
                        // we need a pref for local time, until then, don't do this:
                        //DateTime localUpdate = TimeZone.CurrentTimeZone.ToLocalTime(label.Update);
                        update = string.Format("{0} {1}", localUpdate.ToShortDateString(),
                                                            localUpdate.ToShortTimeString());
                    }
					ListViewItem item = new ListViewItem(new string[]{
							label.Id,
							update,
                            access,
                            label.Owner,
							label.Description
						});
					LabelsListVew.Items.Add(item);
				}
			}
		}
	}
}
