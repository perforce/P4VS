using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Perforce.P4VS
{
	public class SlidingPanel : Panel
	{
		public ImageList ButtonImages;

		private System.ComponentModel.IContainer components;

		private Button _showBtn = null;
		//public Button ShowBtn 
		//{
		//    get { return _showBtn; }
		//    set 
		//    { 
		//        _showBtn = value;

		//        _showBtn.FlatAppearance.BorderSize = 0;
		//        _showBtn.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
		//        _showBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
		//        _showBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		//        _showBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
		//        _showBtn.ImageIndex = 0;
		//        _showBtn.ImageList = this.ButtonImages;
		//        _showBtn.Location = new System.Drawing.Point(1, 2);
		//        _showBtn.Name = "ShowDescriptionBtn";
		//        _showBtn.Size = new System.Drawing.Size(172, 23);
		//        _showBtn.Text = ButtonText;
		//        _showBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		//        _showBtn.UseVisualStyleBackColor = true;

		//        _showBtn.Click += new System.EventHandler(this.ShowBtn_Click);
		//    } 
		//}

		public bool Collapsed { get; set; }

		public bool Hidden { get; set; }

		bool _showAlert = false;
		public bool ShowAlert 
		{
			get { return _showAlert; }
			set
			{
				_showAlert = value;
				if (_showBtn != null)
				{
					_showBtn.ImageIndex = Collapsed ? 2 : 0;
					if (_showAlert)
					{
						_showBtn.ImageIndex += 1;
						_showBtn.ForeColor = Color.Red;
					}
					else
					{
						_showBtn.ForeColor = SystemColors.ControlText;
					}
				}
			}
		}

		public int CollapsedHeight { get; set; }
		public int Weight { get; set; }

		private string _buttonText;
		private const string _buttonTextPad = "       ";
		public string ButtonText
		{
			get
			{
				if (_showBtn != null)
				{
					return _buttonText; 
				}
				return null;
			}
			set
			{
				_buttonText = value;
				if (_showBtn != null)
				{
					// add padding spaces to allow space for icon
					_showBtn.Text = _buttonTextPad + value;
				}
			}
		}

		private int _buttonWidth;
		public int ButtonWidth
		{
			get
			{
				if (_showBtn != null)
				{
					return _showBtn.Size.Width;
				}
				return _buttonWidth;
			}
			set
			{
				_buttonWidth = value;
				if (_showBtn != null)
				{
					Size newSize = new System.Drawing.Size(value, _showBtn.Size.Height);
					_showBtn.Size = newSize;
					_showBtn.MinimumSize = newSize;
				}
			}
		}

		private bool _buttonVisible = true;
		public bool ButtonVisible
		{
			get
			{
				if (_showBtn != null)
				{
					return _showBtn.Visible;
				}
				return _buttonVisible;
			}
			set
			{
				_buttonVisible = value;
				if (_showBtn != null)
				{
					_showBtn.Visible = value;
				}
			}
		}

		public string PreferencesKey { get; set; }

		public int PanelHeight { get; set; }

//		public Button ShowBtn;

		public SlidingPanel()
		{
			Collapsed = false;
			Hidden = false;
			ShowAlert = false;
			CollapsedHeight = 30;
			Weight = 10;
			ButtonText = this.Name;

			InitializeComponent();

#if DEBUG_LAYOUT
			this.BorderStyle = BorderStyle.FixedSingle;
#else
			this.BorderStyle = BorderStyle.None;
#endif

			_showBtn.Text = ButtonText;

			this.ButtonImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// ButtonImages
			// 
			ButtonImages.TransparentColor = System.Drawing.Color.White;
			ButtonImages.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
			ButtonImages.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
			ButtonImages.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
			ButtonImages.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

			_showBtn.ImageList = this.ButtonImages;
		}
		private void ShowBtn_Click(object sender, EventArgs e)
		{
			Collapsed = !Collapsed;
			LayoutPanel();
		}

		public void LayoutPanel()
		{
			_showBtn.ImageIndex = Collapsed ? 2 : 0;
			if (ShowAlert)
			{
				_showBtn.ImageIndex += 1;
				_showBtn.ForeColor = Color.Red;
			}
			else
			{
				_showBtn.ForeColor = SystemColors.ControlText;
			}
			Collapse();
			SlidingPanelContainer parent = this.Parent as SlidingPanelContainer;
			if (parent != null)
			{
				parent.LayoutPanel();
			}
		}

		public void Collapse()
		{
			foreach (Control c in Controls)
			{
				if (c != _showBtn)
				{
					c.Visible = !Collapsed;
				}
			}

		}

		//bool ScaleFactorDisplayed = false;

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			//if (ScaleFactorDisplayed == false)
			//{
			//    string msg = string.Format("SlidingPanel.ScaleControl factor(x,y): {0}, {1}", factor.Width, factor.Height);
			//    msg += string.Format("\r\nthis.Size(x,y): {0}, {1}", this.Size.Width, this.Size.Height);
			//    int totHeight = (int)(this.Size.Height * factor.Height);
			//    msg += string.Format("\r\ntotalHeight: {0}", totHeight);
			//    MessageBox.Show(msg);
			//    ScaleFactorDisplayed = true;
			//}
			//base.ScaleControl(factor, specified);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlidingPanel));
			this._showBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ShowBtn
			// 
			_showBtn.AutoSize = true;
			_showBtn.FlatAppearance.BorderSize = 0;
			_showBtn.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
			_showBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
			_showBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_showBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			_showBtn.ImageIndex = 0;
			_showBtn.Location = new System.Drawing.Point(1, 2);
			_showBtn.Name = "ShowDescriptionBtn";
			_showBtn.MinimumSize = new System.Drawing.Size(50, 23);
			_showBtn.Size = new System.Drawing.Size(50, 23);
			_showBtn.Text = "";
			_showBtn.TextAlign = ContentAlignment.MiddleLeft;
			_showBtn.UseVisualStyleBackColor = true;

			_showBtn.Click += new System.EventHandler(this.ShowBtn_Click);

			this.Controls.Add(_showBtn);

			LayoutPanel();

			this.ResumeLayout(false);

			_buttonText = string.Empty;
		}
	}
}
