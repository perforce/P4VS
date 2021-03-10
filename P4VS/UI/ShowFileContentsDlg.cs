using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace Perforce.P4VS
{
	public partial class ShowFileContentsDlg : AutoSizeForm
	{
		public ShowFileContentsDlg()
		{
			PreferenceKey = "ShowFileContentsDlg";

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
		}

		public string Content
		{
			get { return FileContents.Text; }
			set 
			{ 
				FileContents.Text = value;
				FileContents.Select(0, 0);
			}
		}

		private string _path;
		public string FilePath
		{
			get { return _path; }
			set
			{
				try
				{
					_path = value;
					using (StreamReader sr = new StreamReader(_path))
					{
						Content = sr.ReadToEnd();
					}
					this.Text = Path.GetFileName(_path);
				}
				catch
				{
					this.Text = string.Format(Resources.ShowFileContentsDlg_FileNotFoundWarning, _path);
				}
			}
		}

		private TempFile _tempFile;
		public TempFile TempFile
		{
			get { return _tempFile; }
			set
			{
				_tempFile = value;
				FilePath = value;
			}
		}

		public string Title
		{
			get { return this.Text; }
			set { this.Text = value; }
		}

		private void ShowFileContentsDlg_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_tempFile != null)
			{
				_tempFile.Dispose();
			}
		}
	}
}
