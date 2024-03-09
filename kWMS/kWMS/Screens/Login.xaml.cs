using kWMS.Extras;
using System.Data;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kWMS.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        Executer Exec = new Executer();
        //public class UserMaster
        //{
        //    public string userName { get; set; }
        //    public string UserPassword { get; set; }
        //    public string email { get; set; }
        //}

        //public class LoggedInUser
        //{
        //    public static string userName { get; set; }
        //    public static string email { get; set; }
        //    public string WH_LOCATION_ID { get; set; }
        //}

        public Login()
        {
            
            InitializeComponent();
            lblCompanyname.Text = clsConnection.Copyrights.ToString();
            Loader.IsRunning = false;
        }
        //public static class GlobalVariables1
        //{
        //    public static string User_Type = "";
        //    public static string User_WH_ID = "";
        //    public static string Str_MAS_WH_ID = "";

        //}

        public async void signinbutton_Clicked(object sender, EventArgs e)
        {
            Loader.IsRunning = true;
            DataTable dt = new DataTable();
            try
            {
                string username = usernametxt.Text;
                string password = passwordEntrytxt.Text;
                string enpassword = Utils.EncryptPassword(password);

                string qry = "SELECT * FROM UserMaster WHERE EmpID='" + username + "' AND UserPassword='" + enpassword + "'";
                dt = Exec.GetDataTable(qry);
                if (dt.Rows.Count > 0)
                {
                    clsConnection.User_Type = dt.Rows[0][3].ToString();
                    clsConnection.user_WH_ID = dt.Rows[0][10].ToString();
                    clsConnection.User_Mas_WH_ID = dt.Rows[0][11].ToString();
                    clsConnection.user = dt.Rows[0][1].ToString();

                    //LoggedInUser.userName = username;
                    Loader.IsRunning = false;
                    await Navigation.PushModalAsync(new DashBoard());

                }
                else
                {
                    clsConnection.User_Type = "Invalid User / Password";
                    clsConnection.user_WH_ID = "Invalid User / Password";
                    Loader.IsRunning = false;
                    await App.Current.MainPage.DisplayAlert("Login Verification", "Check UserName / Password.", "OK");

                }

            }
            catch (Exception ex)
            {
                Loader.IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
                throw;
            }

        }

        public void viewbutton_Clicked(object sender, EventArgs e)
        {
            passwordEntrytxt.IsPassword = !passwordEntrytxt.IsPassword;

        }

        private void usernametxt_Completed(object sender, EventArgs e)
        {
            passwordEntrytxt.Focus();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Vibration.Vibrate();
            await Task.Delay(1000);
            Vibration.Cancel();
        }

    }
}