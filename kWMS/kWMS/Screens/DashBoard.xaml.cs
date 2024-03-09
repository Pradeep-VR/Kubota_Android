using kWMS.Extras;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kWMS.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashBoard : ContentPage
    {

        //public static class UserMaster
        //{
        //    public static string userName { get; set; }
        //    public static string WH_LOCATION_ID { get; set; }
        //    public static string usertype { get; set; }
        //}


        //public static class LoggedInUser
        //{
        //    public static string userName { get; set; }
        //    public static string WH_LOCATION_ID { get; set; }
        //    public static string usertype { get; set; }
        //}


        public DashBoard()
        {
            InitializeComponent();

            lblCompanyname.Text = clsConnection.Copyrights.ToString();
            emailtxt.Text = clsConnection.user_WH_ID;
            usertypetxt.Text = clsConnection.User_Type;
            usernametxt.Text = clsConnection.user;

            //showing conection string ip adress in dashboard.            
            conectionstringip.Text = clsConnection.sName.ToString();

            Loader.IsRunning = false;
        }

        public async void Binningbutton_Clicked(object sender, EventArgs e)
        {
            Loader.IsRunning = true;
            await Navigation.PushModalAsync(new Binning());

        }

        private async void Pickingbutton_Clicked(object sender, EventArgs e)
        {
            Loader.IsRunning = true;
            await Navigation.PushModalAsync(new Picking());

        }

        private async void Dispatchbtn_Clicked(object sender, EventArgs e)
        {
            Loader.IsRunning = true;
            await Navigation.PushModalAsync(new Dispatch());

        }

        private async void logoutbtn_Clicked(object sender, EventArgs e)
        {
            Loader.IsRunning = true;
            await Navigation.PushModalAsync(new Login());

        }

        private async void printerbtn_Clicked(object sender, EventArgs e)
        {
            Loader.IsRunning = true;
            await Navigation.PushModalAsync(new Printer());

        }

    }
}