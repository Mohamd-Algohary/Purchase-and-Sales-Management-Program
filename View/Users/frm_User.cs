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
using Selling.Classes;
using Liphsoft.Crypto.Argon2;

namespace Selling.Forms
{
    public partial class frm_User : frm_master
    {
        DAL.User user;
        public frm_User()
        {
            InitializeComponent();
            Refresh_Data();
            New();
            but_delete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        public frm_User(int id)
        {
            InitializeComponent();
            Refresh_Data();
            using (var db = new DAL.dbDataContext())
            {
                user = db.Users.SingleOrDefault(x => x.ID == id); 
            }
            Getdata();
        }
        public override void Refresh_Data()
        {
            using (var db = new DAL.dbDataContext())
            {
                lkp_settingsProfile.InitializeData(db.UserSettingsProfiles.Select(x=> new { x.ID, x.Name }).ToList());
                lkp_entranceType.InitializeData(master.UserTypeList);
                lkp_screenProfile.InitializeData(db.UserAccessProfiles.Select(x => new { x.ID, x.Name }).ToList());
            }
                base.Refresh_Data();
        }
        public override void New()
        {
            user = new DAL.User() { 
             IsActive = true
            };
            base.New();
        }
        public override void Getdata()
        {
            txt_name.Text = user.Name;
            txt_username.Text = user.UserName;
            txt_password.Text = user.Password;
            lkp_settingsProfile.EditValue = user.SettingsProfileID;
            lkp_entranceType.EditValue = user.UserType;
            lkp_screenProfile.EditValue = user.ScreenProfileID;
            toggleSwitch1.IsOn = user.IsActive;
            base.Getdata();
        }
        public override void Setdata()
        {
            if(user.Password != txt_password.Text)
            {
                var hasher = new PasswordHasher();

                string myhash = hasher.Hash(txt_password.Text);
                txt_password.Text = myhash;
            }
            user.Name = txt_name.Text.Trim();
            user.UserName = txt_username.Text.Trim();
            user.Password = txt_password.Text.Trim();
            user.SettingsProfileID = Convert.ToInt32(lkp_settingsProfile.EditValue);
            user.UserType = Convert.ToByte(lkp_entranceType.EditValue);
            user.ScreenProfileID = Convert.ToInt32(lkp_screenProfile.EditValue);
            user.IsActive = toggleSwitch1.IsOn;
            base.Setdata();
        }
        public override bool IsDataValid()
        {
            int NumberOfError = 0;

            using(var db = new DAL.dbDataContext())
            {
                if(db.Users.Where(x=> x.UserName.Trim() == txt_username.Text.Trim() && x.ID != user.ID).Count() > 0)
                {
                    txt_username.ErrorText = "This username already existed!";
                    NumberOfError++;
                }
                if (db.Users.Where(x => x.Name.Trim() == txt_name.Text.Trim() && x.ID != user.ID).Count() > 0)
                {
                    txt_name.ErrorText = "This name already existed!";
                    NumberOfError++;
                }
            }
            NumberOfError += txt_name.IsTextValid() ? 0 : 1;
            NumberOfError += txt_username.IsTextValid() ? 0 : 1;
            NumberOfError += txt_password.IsTextValid() ? 0 : 1;
            NumberOfError += lkp_settingsProfile.IsEditeValueValid() ? 0 : 1;
            NumberOfError += lkp_entranceType.IsEditeValueValid() ? 0 : 1;
            NumberOfError += lkp_screenProfile.IsEditeValueValid() ? 0 : 1;

            return (NumberOfError == 0);
        }
        public override void Save()
        {
            using (var db = new DAL.dbDataContext())
            {
                if (user.ID == 0)
                {
                    db.Users.InsertOnSubmit(user);
                }
                else
                {
                    db.Users.Attach(user);
                }
                Setdata();
                db.SubmitChanges();
            }
            base.Save();
        }
    }
}