using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public partial class SccHistoryDetailsControl : UserControl
	{
		public SccHistoryDetailsControl()
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

		private P4.FileHistory _revisionDetail = null;

		public P4.FileHistory RevisionDetail
		{
			get { return _revisionDetail; }
			set
			{
				_revisionDetail = value;

				if (_revisionDetail != null)
				{
                    if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                    {
                        DateTime local = _revisionDetail.Date;

                        // we need a pref for local time, until then, don't do this:
                        //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                        SubmittedDateTB.Text = local.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    else
                    {
                        DateTime local = _revisionDetail.Date;

                        // we need a pref for local time, until then, don't do this:
                        //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                        SubmittedDateTB.Text = string.Format("{0} {1}", local.ToShortDateString(),
                                                            local.ToShortTimeString());
                    }

                    ChangelistTB.Text = _revisionDetail.ChangelistId.ToString();
					UserTB.Text = _revisionDetail.UserName;
					FileTypeTB.Text = _revisionDetail.FileType.ToString();
					WorkspaceTB.Text = _revisionDetail.ClientName;

					if (_revisionDetail.FileSize >= 0)
					{
						FileSizeTB.Text = P4FileTreeListViewItem.PrettyPrintFileSize(_revisionDetail.FileSize);
					}
					else
					{
						FileSizeTB.Text = string.Empty;
					}

					ActionTB.Text = _revisionDetail.Action.ToString();
					if (_revisionDetail.Description.Contains('\n') &&
						!_revisionDetail.Description.Contains('\r'))
					{
						// If the descriptions contains newlines with out carriage returns,  
						//	replace the solo newlines
						DescriptionTB.Text = _revisionDetail.Description.Replace("\n", "\r\n");
					}
					else if (_revisionDetail.Description.Contains('\r') &&
						!_revisionDetail.Description.Contains('\n'))
					{
						// If the descriptions contains carriage returns with out newlines,  
						//	replace the solo carriage returns
						DescriptionTB.Text = _revisionDetail.Description.Replace("\r", "\r\n");
					}
					else
					{
						DescriptionTB.Text = _revisionDetail.Description;
					}
				}
				else
				{
					SubmittedDateTB.Text = string.Empty;
					ChangelistTB.Text = string.Empty;
					UserTB.Text = string.Empty;
					FileTypeTB.Text = string.Empty;
					WorkspaceTB.Text = string.Empty;
					FileSizeTB.Text = string.Empty;
					ActionTB.Text = string.Empty;
					DescriptionTB.Text = string.Empty;
				}
			}
		}
	}
}
