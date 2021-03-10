using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Perforce;

namespace Perforce.P4VS
{
	public partial class SccHistoryIntegrationsControl : UserControl
	{
		public SccHistoryIntegrationsControl()
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

				if (_revisionDetail == null)
				{
					IntegrationsTreeListView.Nodes.Clear();
					IntegrationsTreeListView.Nodes.Add(new TreeListViewItem(null, 
						Resources.JobsToolWindowControl_NoItemsAvailable, true));
					IntegrationsTreeListView.BuildTreeList();
					return;
				}
				TreeListViewItem sources = null;
				TreeListViewItem targets = null;
				TreeListViewItem others = null;
				foreach (P4.RevisionIntegrationSummary ris in _revisionDetail.IntegrationSummaries)
				{
					if (ris.How.Contains("into"))
					{
						if (targets == null)
						{
							targets = new TreeListViewItem(null, 
								Resources.SccHistoryIntegrationsControl_TargetsReceivingContent, true);
						}

						TreeListViewItem node = new TreeListViewItem(targets, new string[] { 
							ris.FromFile.DepotPath.Path,
							ris.FromFile.Version.ToString(),
							ris.How
						});
						targets.ChildNodes.Add(node);
					}
					else if (ris.How.Contains("from"))
					{
						if (sources == null)
						{
							sources = new TreeListViewItem(null, 
								Resources.SccHistoryIntegrationsControl_SourcesContributingContent, true);
						}

						TreeListViewItem node = new TreeListViewItem(sources, new string[] { 
							ris.FromFile.DepotPath.Path,
							ris.FromFile.Version.ToString(),
							ris.How
							});
						sources.ChildNodes.Add(node);
					}
					else
					{
						if (others == null)
						{
							others = new TreeListViewItem(null,
								Resources.SccHistoryIntegrationsControl_OtherIntegrations, true);
						}

						TreeListViewItem node = new TreeListViewItem(others, new string[] { 
							ris.FromFile.DepotPath.Path,
							ris.FromFile.Version.ToString(),
							ris.How
							});
						others.ChildNodes.Add(node);
					}
				}
				IntegrationsTreeListView.Nodes.Clear();
				if (targets != null)
				{
					IntegrationsTreeListView.Nodes.Add(targets);
					targets.Expanded = true;
				}
				if (sources != null)
				{
					IntegrationsTreeListView.Nodes.Add(sources);
					sources.Expanded = true;
				}
				if (others != null)
				{
					IntegrationsTreeListView.Nodes.Add(others);
					others.Expanded = true;
				}
				if (IntegrationsTreeListView.Nodes.Count <= 0)
				{
					IntegrationsTreeListView.Nodes.Add(new TreeListViewItem(null, 
						Resources.JobsToolWindowControl_NoItemsAvailable, true));
				}
				IntegrationsTreeListView.BuildTreeList();
			}
		}
	}
}
