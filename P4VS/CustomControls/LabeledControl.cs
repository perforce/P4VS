using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.I18nControls
{
	public partial class LabeledControl : Panel
	{
		public LabeledControl()
		{
			InnerMargin = 3;
			InitializeComponent();
			if ((Controls != null) && (Controls.Count > 1))
			{
				_control = Controls[1] as Control;
			}
			//this.AutoSize = true;
		}

		protected Control _control;

		//public static implicit operator Control(LabeledControl it) 
		//{ 
		//    return it._control; 
		//}

		public string Label
		{
			get { return mLabel.Text; }
			set { mLabel.Text = value; }
		}

		public int InnerMargin { get; set; }

		private void OnSizeChanged()
		{
			Size tbSize = mLabel.Size;

			if ((_control == null) && (Controls != null) && (Controls.Count > 1))
			{
				_control = Controls[1] as Control;
			}
			if (_control != null)
			{
				//_control.Location = new Point(tbSize.Width + _control.Margin.Right, 0);
				_control.Left = tbSize.Width + InnerMargin;
				_control.Width = this.Width - _control.Location.X;
				this.Height = System.Math.Max(_control.Height + _control.Top, mLabel.Height + mLabel.Top);
			}
		}
		private void mLabel_SizeChanged(object sender, EventArgs e)
		{
			OnSizeChanged();
		}

		private void LabeledControl_SizeChanged(object sender, EventArgs e)
		{
			OnSizeChanged();
		}
	}
	public class LabeledControl<T> : LabeledControl where T : Control
	{
		public static implicit operator T(LabeledControl<T> it) 
		{ 
			return it._control as T; 
		}
	}
	class LabeledTextBox : LabeledControl<System.Windows.Forms.TextBox> { };
	class LabeledComboBox : LabeledControl<System.Windows.Forms.ComboBox> { };

	class LabeledSeperator : LabeledControl
	{
		public LabeledSeperator()
		{
			this.SuspendLayout();
			_control = new GroupBox();

			_control.Location = new Point(0, this.Height / 2);
			_control.Size = new Size(this.Width, 2);

			//_control.SendToBack();

			this.Controls.Add(_control);

			this.ResumeLayout(false);
			this.PerformLayout();
		}
		public static implicit operator GroupBox(LabeledSeperator it) 
		{
			return it._control as GroupBox; 
		}

		//protected override void OnSizeChanged(EventArgs e)
		//{
		//    base.OnSizeChanged(e);
		//}
	}
}
