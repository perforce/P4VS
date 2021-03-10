using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class NewUserDlg : Form
	{
		bool SetPasswordOnly { get; set; }
		public NewUserDlg(P4ScmProvider scm)
		{
			Scm = scm;
			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
		}

		//public P4.User CreatedUser(P4ScmProvider scm)
		//{
			
		//        P4.User newUser = new P4.User();
		//        newUser.Id = userNameTB.Text;
		//        newUser.FullName = fullNameTB.Text;
		//        newUser.Password = password1TB.Text;
		//        newUser.EmailAddress = emailTB.Text;
		//        return newUser;
			
		//}

		public P4ScmProvider Scm { get; private set; }

		public P4.User Show(P4ScmProvider scm)
		{
			SetPasswordOnly = false;
			Scm = scm;
			string oldPasswd = null;

			P4.User newUser = new P4.User();

            do
            {
                if (this.ShowDialog() == DialogResult.OK)
                {

					if (!SetPasswordOnly)
					{
						string name = userNameTB.Text;
						if (name.Contains(" "))
						{
							name = Regex.Replace(name, " ", "_");
						}
						P4.Options opts = new P4.Options();
						IList<string> users = new List<string>();
						users.Add(userNameTB.Text);
						if (Scm.GetUsers(users, opts) != null)
						{
							string msg = string.Format(Resources.NewUserDlg_UserExistsWarning, userNameTB.Text);
							MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
							continue;
						}

                        // Set connection options
                        P4.Options options = new P4.Options();
                        options["ProgramName"] = "P4VS";
                        options["ProgramVersion"] = Versions.product();

                        newUser.Id = name;
						newUser.FullName = fullNameTB.Text;
						newUser.EmailAddress = emailTB.Text;
                        scm.Connection.Repository.Connection.UserName = newUser.Id;
                        scm.Connection.Repository.Connection.Connect(options);

                        //scm.Connection.User = newUser.Id;//.Repository.Connection.UserName = newUser.Id;
                        //scm.Connection.Connect(null);//.Repository.Connection.Connect(null);
                    }
                    if (!string.IsNullOrEmpty(fullNameTB.Text))
					{
						newUser.Password = password1TB.Text;
					}
					try
					{
						if (SetPasswordOnly)
						{
							SetPasswordOnly = false;
                            scm.Connection.Repository.Connection.SetPassword(null, password1TB.Text);
						}
						else
						{
							SetPasswordOnly = false;
							newUser = scm.NewUser(newUser);
						}
						return newUser;
					}
					catch (P4.P4Exception p4ex)
					{
						// if from Connection.SetPassword(), error has not been shown
						if (P4.P4ClientError.IsBadPasswdError(p4ex.ErrorCode))
						{
							SetPasswordOnly = true;
						}
						if ((p4ex.ErrorCode == P4.P4ClientError.MsgServer_PasswordTooShort) ||
							(p4ex.ErrorCode == P4.P4ClientError.MsgServer_PasswordTooSimple))
						{
							MessageBox.Show(Resources.NewUserDlg_PasswordTooShortOrSimple, Resources.P4VS,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						else
						{
							scm.ShowException(p4ex);
						}
					}

                    P4.P4CommandResult results = scm.Connection.Repository.Connection.LastResults;
					oldPasswd = password1TB.Text;

                }
                else
                {
                    return null;
                }
            } while (true);
		}

		private void saveBtn_Click(object sender, EventArgs e)
		{
			if (userNameTB.Text.Contains(" "))
			{
				MessageBox.Show(Resources.NewUserDlg_NameContainsSpacesWarning, Resources.P4VS, 
					MessageBoxButtons.OK,MessageBoxIcon.Information);
			}

			if (!(string.IsNullOrEmpty(password1TB.Text))||!(string.IsNullOrEmpty(password2TB.Text)))
			{
				if (password1TB.Text != password2TB.Text)
				{
					MessageBox.Show(Resources.NewUserDlg_PasswordsDontMatchWarning, Resources.P4VS, 
						MessageBoxButtons.OK,MessageBoxIcon.Information);
					return;
				}
			}
			

			P4.Options opts = new P4.Options();
			IList<string> users = new List<string>();
			users.Add(userNameTB.Text);

			if ((SetPasswordOnly == true) || (Scm.GetUsers(users,opts) == null))
			{
				DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				string msg = string.Format(Resources.NewUserDlg_UserExistsWarning, userNameTB.Text);
				MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void userNameTB_TextChanged(object sender, EventArgs e)
		{
			saveBtn.Enabled = ((userNameTB.Text.Length > 0) && (fullNameTB.Text.Length > 0) && (emailTB.Text.Length > 0));
		}

		private void fullNameTB_TextChanged(object sender, EventArgs e)
		{
			saveBtn.Enabled = ((userNameTB.Text.Length > 0) && (fullNameTB.Text.Length > 0) && (emailTB.Text.Length > 0));
		}

		private void emailTB_TextChanged(object sender, EventArgs e)
		{
			saveBtn.Enabled = ((userNameTB.Text.Length > 0) && (fullNameTB.Text.Length > 0) && (emailTB.Text.Length > 0));
		}

		private void NewUserDlg_Load(object sender, EventArgs e)
		{
			userNameLbl.Enabled = !SetPasswordOnly;
			userNameTB.Enabled = !SetPasswordOnly;
			fullNameTB.Enabled = !SetPasswordOnly;
			password1TB.Enabled = true;
			password2TB.Enabled = true;
			emailTB.Enabled = !SetPasswordOnly;
		}
	}
}
