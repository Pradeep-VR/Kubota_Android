using kWMS.Extras;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kWMS.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Printer : ContentPage
    {
        Executer Exec = new Executer();

        public static class GlobalVariableIp
        {
            public static string IP_Address = "";
            public static string Name_Printer = "";
        }

        public Printer()
        {
            InitializeComponent();

            username.Text = clsConnection.user;
            wh_loc.Text = clsConnection.user_WH_ID;
            master_wh_loc.Text = clsConnection.User_Mas_WH_ID;

            string strSql;
            DataTable dtsen = new DataTable();

            strSql = "SELECT DISTINCT PRINTER_NAME FROM tbl_Printer_Settings  where MASTER_WH_LOC_ID ='" + master_wh_loc.Text.Trim() + "' AND PRINTER_STATUS=1 ";
            dtsen = Exec.GetDataTable(strSql);
            if (dtsen.Rows.Count > 0)
            {
                for (int i = 0; i < dtsen.Rows.Count; i++)
                {
                    sltprinter.Items.Add(dtsen.Rows[i]["PRINTER_NAME"].ToString());
                    GlobalVariableIp.Name_Printer = dtsen.Rows[i]["PRINTER_NAME"].ToString();
                }
            }
            else
            {
                
                DisplayAlert("Alert", "Error to Load Printer's.", "OK");

            }
            
        }

       
        private async void savebtn_Clicked(object sender, EventArgs e)
        {
            GlobalVariableIp.IP_Address = ipaddress.Text.ToString();
            clsConnection.IP_Address = ipaddress.Text;

            clsConnection.Printer_Name = GlobalVariableIp.Name_Printer;
            await Navigation.PushModalAsync(new DashBoard());

        }

        private void sltprinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Sql = " SELECT DISTINCT PRINTER_IP_ADDRESS FROM tbl_Printer_Settings WHERE PRINTER_NAME ='" + sltprinter.SelectedItem.ToString() + "' AND PRINTER_STATUS=1 ";
            DataTable dt = Exec.GetDataTable(Sql);
            if (dt.Rows.Count > 0)
            {
                ipaddress.Text = dt.Rows[0]["PRINTER_IP_ADDRESS"].ToString().TrimEnd();
            }
        }

        private void test_con_btn_Clicked(object sender, EventArgs e)
        {
            
            PrinterFunction();
            

        }


        public async void PrinterFunction()
        {
            //GlobalVariableIp.IP_Address = ipaddress.Text;
            string printerIpAddress = ipaddress.Text; // replace with the actual IP address of the printer
            GlobalVariableIp.IP_Address = ipaddress.Text;
            int printerPort = 9100; // the default port number for Zebra printers

            TcpClient client = new TcpClient();
            await client.ConnectAsync(printerIpAddress.TrimEnd(), printerPort);
            string test = "Test_Print";

            string zplCommand = "^XA^FT399,873^A0B,34,33^FH\\^FD" + printerIpAddress + "-" + test + "^FS^XZ"; // sample ZPL command to print a QR code

            byte[] zplCommandBytes = Encoding.ASCII.GetBytes(zplCommand);

            await client.GetStream().WriteAsync(zplCommandBytes, 0, zplCommandBytes.Length);

            client.Close();


        }
    }
}