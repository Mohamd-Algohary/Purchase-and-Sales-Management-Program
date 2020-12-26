using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Liphsoft.Crypto.Argon2;
using DevExpress.XtraSplashScreen;
using System.Reflection;
using System.Diagnostics;

namespace Selling.Forms
{
    public partial class frm_login : DevExpress.XtraEditors.XtraForm
    {
        public frm_login()
        {
            InitializeComponent();
          
        }

        public void but_login_Click(object sender, EventArgs e) //Make it public for debugger
        {
            using (var db = new DAL.dbDataContext())
            {
                var username = txt_username.Text;    //Press F12 to convert Public for debugger
                var password = txt_password.Text;

                var user = db.Users.SingleOrDefault(x => x.UserName == username);
                if(user == null)
                {
                    goto LoginFaild;
                }
                else 
                {
                    if(user.IsActive == false)
                    {
                      XtraMessageBox.Show("This account was Disabled", "Login Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    var pass = user.Password;
                    var hasher = new PasswordHasher();
                    if (hasher.Verify(pass, password))
                    {
                        this.Hide();
                        SplashScreenManager.ShowForm(parentForm : frm_main.Instance, typeof(Start));

                        Type T = typeof(Classes.session);
                        var proprties = T.GetProperties(BindingFlags.Public | BindingFlags.Static);
                        foreach (var item in proprties)
                        {
                            var obj = item.GetValue(null);

                        }
                        Classes.session.SetUser(user);
                        frm_main.Instance.Show();
                        this.Close();
                        SplashScreenManager.CloseForm();
                        return;
                    }
                    else
                    {
                        goto LoginFaild;
                    }
                }
                LoginFaild:
                XtraMessageBox.Show("Username or Password is incorrect", "Login Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}