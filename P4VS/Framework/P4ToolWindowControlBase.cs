using Perforce.P4Scm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public class P4ToolWindowControlBase : System.Windows.Forms.UserControl
	{
#if DEBUG
		private P4ScmProvider _scm;
		public P4ScmProvider Scm
		{
			get { return _scm; }
			set { _scm = value; }
		}
#else
		public P4ScmProvider Scm 
		{ 
			get; 
			set; 
		}
#endif
		P4VsProvider.NewConnectionDelegate newConection;

#if VS2012
        protected ThemeManager ThemeMgr = null;
#endif

        public P4ToolWindowControlBase()
        {
            Scm = P4VsProvider.CurrentScm;

            newConection = new P4VsProvider.NewConnectionDelegate(NewConnection);
            P4VsProvider.NewConnection += newConection;
        }

        public P4ToolWindowControlBase(P4ScmProvider scm)
        {
            Scm = scm;

            newConection = new P4VsProvider.NewConnectionDelegate(NewConnection);
            P4VsProvider.NewConnection += newConection;
        }

#if VS2012
        ~P4ToolWindowControlBase()
        {
            if (ThemeMgr != null)
            {
                ThemeMgr.Dispose();
            }
        }

        public void InitThemeManager(params object[] images)
        {
            if (ThemeMgr == null)
            {
                ThemeMgr = new ThemeManager(Controls, this.Name);
            }
            foreach (object o in images)
            {
                if (o is ImageList)
                {
                    ThemeMgr.RegisterImageList(o as ImageList);
                }
                if (o is System.Drawing.Image)
                {
                    ThemeMgr.RegisterImage(o as System.Drawing.Image);
                }
            }
            ThemeMgr.SetControlColors();
        }
#endif

        public virtual void OnNewConnection(P4ScmProvider newScm)
		{
			//throw new NotImplementedException("Classes derived from P4ToolWindowControlBase must impement OnNewConnection()");
		}

		public void NewConnection(P4ScmProvider newScm)
		{
			// save settings for the old connection
			if ((Scm != newScm) && (Scm != null) && (Scm.Connected))
			{
				SaveControlSettings();
			}

			Scm = newScm;

			// clear the cached annotated preference key
			_preferenceKey = null;

			// load settings for the new connection
			if ((Scm != null) && (Scm.Connected))
			{
				// set the clm widths based on the new connection
				Initialize();
			}

			// call the overridden function in the derived class
			this.OnNewConnection(newScm);

			// load settings for the new connection
			if ((Scm != null) && (Scm.Connected))
			{
				// set the clm widths based on the new connection
				Initialize();
			}
		}

		string _preferenceKeyBase = null;
		string _preferenceKey = null;

		protected string PreferenceKey
		{
			get
			{
				if ((Scm == null) ||
                    (Scm.Connection.Repository == null) ||
                    (_preferenceKeyBase == null))
				{
					return null;
				}
				if (_preferenceKey == null)
				{
                    _preferenceKey = _preferenceKeyBase + "_" + Scm.Connection.Repository.Connection.Server.Address.Uri.Replace(':', '_');
				}
				return _preferenceKey;
			}
			set
			{
				_preferenceKeyBase = value;
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (PreferenceKey != null)
			{
				foreach (Control c in Controls)
				{
					if (c is DoubleBufferedListView)
					{
						string listKey =
							string.Format("{0}_{1}", PreferenceKey, c.Name);
						((DoubleBufferedListView)c).OnClose(listKey);
					}
					else if (c is TreeListView)
					{
						string listKey =
							string.Format("{0}_{1}", PreferenceKey, c.Name);
						((TreeListView)c).OnClose(listKey);
					}
					else if (c is FilterComboBox)
					{
						string listKey =
							string.Format("{0}_{1}", PreferenceKey, c.Name);
						((FilterComboBox)c).OnClose(listKey);
					}
					if ((c.Controls != null) && (c.Controls.Count > 0))
					{
						SaveControlSettings(c.Controls);
					}
				}
			}
			base.OnHandleDestroyed(e);
		}

		private void InitControlSettings(Control.ControlCollection Controls)
		{
				foreach (Control c in Controls)
				{
					if (c is DoubleBufferedListView)
					{
						string listKey =
							string.Format("{0}_{1}", PreferenceKey, c.Name);
						((DoubleBufferedListView)c).OnLoad(listKey);
					}
					else if (c is TreeListView)
					{
						string listKey =
							string.Format("{0}_{1}", PreferenceKey, c.Name);
						((TreeListView)c).OnLoad(listKey);
					}
					else if (c is FilterComboBox)
					{
						string listKey =
							string.Format("{0}_{1}", PreferenceKey, c.Name);
						((FilterComboBox)c).OnLoad(listKey);
					}
                    else if (c is SplitContainer)
                    {
                        string listKey =
                            string.Format("{0}_{1}", PreferenceKey, c.Name);
                        if (listKey != null && Preferences.LocalSettings.ContainsKey(listKey + "_SplitterDistance"))
                        {
                            ((SplitContainer)c).SplitterDistance =
                                (int)Preferences.LocalSettings[listKey + "_SplitterDistance"];
                        }
                    }
					if ((c.Controls != null) && (c.Controls.Count > 0))
					{
						InitControlSettings(c.Controls);
					}
				}
		}

		public void Initialize()
		{
			if (PreferenceKey != null)
			{
				InitControlSettings(Controls);
			}
		}

		protected void InitializeComponent()
		{
		}

		private void SaveControlSettings(Control.ControlCollection Controls)
		{
			foreach (Control c in Controls)
			{
				if (c is DoubleBufferedListView)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((DoubleBufferedListView)c).OnClose(listKey);
				}
				else if (c is TreeListView)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((TreeListView)c).OnClose(listKey);
				}
				else if (c is FilterComboBox)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((FilterComboBox)c).OnClose(listKey);
				}
                else if (c is SplitContainer)
                {
                    string listKey =
                        string.Format("{0}_{1}", PreferenceKey, c.Name);
                    if (PreferenceKey != null && ((SplitContainer)c).SplitterDistance != 0)
                    {
                        Preferences.LocalSettings[listKey + "_SplitterDistance"] =
                            ((SplitContainer)c).SplitterDistance;
                    }
                }
				if ((c.Controls != null) && (c.Controls.Count > 0))
				{
					SaveControlSettings(c.Controls);
				}
			}
		}

		public void SaveControlSettings()
		{
			if (PreferenceKey != null)
			{
				SaveControlSettings(Controls);
			}
		}

		/// <include file='doc\WindowPane.uex' path='docs/doc[@for="WindowPane.Dispose1"]' />
		/// <devdoc>
		///     Called when this tool window pane is being disposed.
		/// </devdoc>
		override protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				SaveControlSettings();
				P4VsProvider.NewConnection -= newConection;
			}
			base.Dispose(disposing);
		}
	}
}