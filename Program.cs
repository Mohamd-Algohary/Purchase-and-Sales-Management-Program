using DevExpress.LookAndFeel;
using Selling.Forms;
using Selling.Properties;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Selling
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UserLookAndFeel.Default.SkinName = Settings.Default.SkinName.ToString();
            UserLookAndFeel.Default.SetSkinStyle(Settings.Default.SkinName.ToString(), Settings.Default.PalettaName);

            //new frm_login().Show();
            var frm = new frm_login();
            frm.Show();
            if (Debugger.IsAttached)
            {
                frm.txt_username.Text = "admin";
                frm.txt_password.Text = "123456";
                frm.but_login_Click(null, null);
            }
            Application.Run();
        }

    }
}
