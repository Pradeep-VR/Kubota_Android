using kWMS.Extras;
using kWMS.Services;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;

namespace kWMS.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dispatch : ContentPage
    {

        DispatchManagement DisMgnt = new DispatchManagement();

        string Printer_IP_Address = clsConnection.IP_Address;
        public class GrpNoListView
        {
            public string lst_grp_no { get; set; }
        }

        public ObservableCollection<string> grpnodata = new ObservableCollection<string>();

        public class InvoiceNo
        {
            public string invoiceno { get; set; }
        }

        public ObservableCollection<string> invoicedata = new ObservableCollection<string>();
        public class OrderList
        {
            public string listorder { get; set; }
        }

        public ObservableCollection<string> orderdata = new ObservableCollection<string>();

        public Dispatch()
        {
            InitializeComponent();
            Frm_Goods_Dispatch_Load();

            if (clsConnection.Printer_Name.Length != 0)
            {
                txtdocket_No.Focus();
            }
            else if (clsConnection.Printer_Name == "")
            {
                DisplayAlert("Alert", "Printer Not Connected.", "OK");
                Navigation.PushModalAsync(new DashBoard());
            }
        }

        private async void btnok_Click(object sender, EventArgs e)
        {
            try
            {
                btnok.IsEnabled = false;

                Proccess_save_new();

                if (Txt_Fnl_Dealer_Code.Text.Length == 0)
                {

                }
                else
                {
                    refresh_disp();
                }
                btnok.IsEnabled = true;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }

        }

        private void btnback_Click(object sender, EventArgs e)
        {

            Pnl_Invoice.IsVisible = true;
            Frm_Goods_Dispatch.IsVisible = false;

        }

        private async void btnbreak_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new DashBoard());
        }

        private async void Cmb_Shipment_Mode_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (Cmb_Shipment_Mode.SelectedIndex == -1)
                {
                    cmb_Shipment_tranporter.SelectedIndex = -1;
                }
                else if (Cmb_Shipment_Mode.SelectedIndex.ToString().Length != 0)
                {
                    load_Transporter();
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }

        }

        private async void Bln_Gen_Fnl_No_Clicked(object sender, EventArgs e)
        {

            try
            {

                if (txtdocket_No.Text.Length == 0)
                {
                    await DisplayAlert("Message", "Please Scan Docket No.", "OK");
                    txtdocket_No.Focus();
                }
                else if (Cmb_Shipment_Mode.SelectedItem.ToString().Length == 0)
                {
                    await DisplayAlert("Message", "Please Select Shipment Mode", "OK");
                    Cmb_Shipment_Mode.Focus();
                }
                else if (cmb_Shipment_tranporter.SelectedItem.ToString().Length == 0)
                {
                    await DisplayAlert("Message", "Please Select Shipment Transporter", "OK");
                    cmb_Shipment_tranporter.Focus();
                }
                else if (txt_Master_Invoice.Text.Length == 0)
                {
                    await DisplayAlert("Message", "Please Scan any one Invoice No.", "OK");
                    txt_Master_Invoice.Focus();
                }
                else if (Txt_Fnl_Dealer_Code.Text.Length == 0)
                {
                    await DisplayAlert("Message", "Please Scan Dealer Code", "OK");
                    Txt_Fnl_Dealer_Code.Focus();
                }
                else if (txt_Master_Invoice.Text.Length != 0 && Txt_Fnl_Dealer_Code.Text.Length != 0 && cmb_Shipment_tranporter.SelectedItem.ToString().Length != 0 && Cmb_Shipment_Mode.SelectedItem.ToString().Length != 0 && txtdocket_No.Text.Length != 0)
                {
                    refresh_disp();

                    txt_scan_Qr.Focus();

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }
        }

        private void btclear_Click_1(object sender, EventArgs e)
        {
            txtdocket_No.Text = string.Empty;
            txtcartonbarcode.Text = string.Empty;
            txt_Fnl_Grp_Number_Process.Text = string.Empty;
            Txt_Fnl_Dealer_Code.Text = string.Empty;
            txt_Master_Invoice.Text = string.Empty;

            lst_Fnl_Grp_No.SelectedItem = string.Empty;
            lst_Fnl_Grp_No.ItemsSource = string.Empty;
            grpnodata.Clear();

            Lst_Box_Invoice_No.SelectedItem = string.Empty;
            Lst_Box_Invoice_No.ItemsSource = string.Empty;
            invoicedata.Clear();

            lst_order.SelectedItem = string.Empty;
            lst_order.ItemsSource = string.Empty;
            orderdata.Clear();

            txtweight.Text = string.Empty;
            txtship.Text = string.Empty;
            txtqty.Text = string.Empty;
            txttotalqty.Text = string.Empty;
            txttotrecqty.Text = string.Empty;

            cmb_Shipment_tranporter.SelectedIndex = -1;
            Cmb_Shipment_Mode.SelectedIndex = -1;

            txtdocket_No.Focus();
        }

        private async void Proccess_save_new()
        {
            var lb_dis_itms = lst_Fnl_Grp_No.ItemsSource as IList;
            var count_lb_dis = lb_dis_itms.Count;
            try
            {
                string Str_Output = string.Empty;
                string str_out = string.Empty;
                string err = string.Empty;
                int tot = 0;
                int bal = 0;
                string ermsg = string.Empty;
                string dispqr = string.Empty;

                dispqr = "DP" + "_" + clsConnection.user_WH_ID + "_" + txt_scan_Qr.Text;

                refresh_disp();

                string ord = string.Empty;

                for (int i = 0; i < count_lb_dis; i++)
                {

                    ord = ord + grpnodata[i].ToString() + ",";
                }

                ord = ord.Substring(0, ord.Length - 1);

                tot = Convert.ToInt32(txt_tot_Box.Text);
                bal = Convert.ToInt32(txt_Bal_box.Text);

                string box_co = string.Empty;

                str_out = Convert.ToString(DisMgnt.get_box_wt(txt_scan_Qr.Text, clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, Txt_Fnl_Dealer_Code.Text, ord, ref tot, ref bal, ref err));
                if (err == "Success")
                {

                    if (bal == 0)
                    {
                        await DisplayAlert("Message", "Dispatch Completed ", "OK");

                        ord = string.Empty;
                        for (int i = 0; i < lb_dis_itms.Count; i++)
                        {

                            ord = ord + grpnodata[i].ToString() + ",";
                        }
                        ord = ord.Substring(0, ord.Length - 1);

                        string strout = Convert.ToString(DisMgnt.comp_disp(ord, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, ref ermsg));
                        if (strout == "Success")
                        {

                            Pnl_Invoice.IsVisible = true;
                            Frm_Goods_Dispatch.IsVisible = false;

                            txtdocket_No.Text = string.Empty;
                            txtcartonbarcode.Text = string.Empty;
                            txt_Fnl_Grp_Number_Process.Text = string.Empty;
                            Txt_Fnl_Dealer_Code.Text = string.Empty;
                            txt_Master_Invoice.Text = string.Empty;
                            lst_Fnl_Grp_No.ItemsSource = string.Empty;
                            Lst_Box_Invoice_No.SelectedItem = string.Empty;
                            lst_order.ItemsSource = string.Empty;
                            txtweight.Text = string.Empty;
                            txtship.Text = string.Empty;
                            txtqty.Text = string.Empty;
                            txttotalqty.Text = string.Empty;
                            txttotrecqty.Text = string.Empty;

                            cmb_Shipment_tranporter.SelectedIndex = -1;
                            Cmb_Shipment_Mode.SelectedIndex = -1;

                        }

                    }
                    else
                    {

                        if (bal == 0)
                        {
                            box_co = "1";
                        }
                        else
                        {
                            box_co = Convert.ToString(Math.Abs(tot - bal) + 1);
                        }

                        box_co = box_co + " / " + Convert.ToString(tot);

                        Str_Output = Convert.ToString(DisMgnt.Save_Dispatch_Box_new(txt_scan_Qr.Text, clsConnection.user, dispqr, Cmb_Shipment_Mode.SelectedItem.ToString(), cmb_Shipment_tranporter.SelectedItem.ToString(), clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, Txt_Fnl_Dealer_Code.Text, txt_scan_Qr.Text, txtdocket_No.Text, box_co, ord, txtwt.Text, clsConnection.HHT_Serial_Number, txt_Fnl_Grp_Number_Process.Text, txtship.Text, ref ermsg));
                        if (ermsg == "Success")
                        {
                            Address_Label_Print();
                            txt_scan_Qr.Text = string.Empty;

                            txtwt.Text = string.Empty;
                            txt_scan_Qr.Focus();
                            refresh_disp();
                        }
                        else
                        {
                            await DisplayAlert("Alert", ermsg, "OK");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
        }

        private async void refresh_disp()
        {

            if (Cmb_Shipment_Mode.SelectedItem.ToString().Length != 0)
            {
                string ord = string.Empty;
                string temp = string.Empty;
                orderdata.Clear();
                lst_order.ItemsSource = string.Empty;

                var lb_dis_itms = lst_Fnl_Grp_No.ItemsSource as IList;
                var uio = "";

                for (int i = 0; i < grpnodata.Count; i++)
                {
                    uio = grpnodata[i];

                    ord = ord + uio + ",";

                    temp = temp + uio;

                    orderdata.Add(temp);

                    temp = string.Empty;

                    lst_order.ItemsSource = orderdata;
                }
                if (ord.Length > 0)
                {
                    ord = ord.Substring(0, ord.Length - 1);

                }
                int str_tot = 0;
                int str_bal = 0;
                string err = string.Empty;

                string str_out = Convert.ToString(DisMgnt.Get_Box_Count_new(ord, clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, Txt_Fnl_Dealer_Code.Text, Cmb_Shipment_Mode.SelectedItem.ToString(), cmb_Shipment_tranporter.SelectedItem.ToString(), txtdocket_No.Text, ref str_tot, ref str_bal, ref err));

                if (err == "Success")
                {
                    txt_tot_Box.Text = Convert.ToString(str_tot);
                    if (str_bal == 0)
                    {

                        lb_dis_itms.Clear();

                        Pnl_Invoice.IsVisible = true;
                        txt_Fnl_Grp_Number_Process.Text = string.Empty;
                        Txt_Fnl_Dealer_Code.Text = string.Empty;
                        txtship.Text = string.Empty;
                        cmb_Shipment_tranporter.SelectedIndex = -1;
                        Cmb_Shipment_Mode.SelectedIndex = -1;

                        txt_Master_Invoice.Text = string.Empty;
                        txt_Master_Invoice.Focus();

                        await Navigation.PushModalAsync(new Dispatch());

                    }
                    else if (str_tot != str_bal)
                    {

                        txt_Bal_box.Text = Convert.ToString(str_bal);
                        txtb.Text = Convert.ToString(str_tot - str_bal);

                        Pnl_Invoice.IsVisible = false;
                        Frm_Goods_Dispatch.IsVisible = true;

                    }
                    else
                    {
                        txtb.Text = "0";
                        txt_Bal_box.Text = Convert.ToString(str_bal);
                        Pnl_Invoice.IsVisible = false;
                        Frm_Goods_Dispatch.IsVisible = true;

                    }

                }
            }
            else if (Cmb_Shipment_Mode.SelectedIndex == -1)
            {
                Cmb_Shipment_Mode.Items.Clear();
            }
        }

        protected async void load_Transporter()
        {
            try
            {
                string ermsg = string.Empty;
                DataTable dtreceive = new DataTable();

                dtreceive = DisMgnt.download_SHIPMENT_TRANSPORTER(clsConnection.user, clsConnection.user_WH_ID, Cmb_Shipment_Mode.SelectedItem.ToString(), ref ermsg);
                if (ermsg == "Success")
                {
                    if (dtreceive.Rows.Count > 0)
                    {
                        cmb_Shipment_tranporter.Items.Clear();
                        for (int i = 0; i < dtreceive.Rows.Count; i++)
                        {
                            cmb_Shipment_tranporter.Items.Add(dtreceive.Rows[i]["SHIPMENT_TRANSPORTER"].ToString());
                        }
                    }
                    else
                    {
                        await Navigation.PopAsync();
                        await DisplayAlert("Alert", "No SHIPMENT_TRANSPORTER Made For this Location ", "OK");
                    }
                }
                else
                {

                    await Navigation.PopAsync();
                    await DisplayAlert("Alert", "No SHIPMENT_TRANSPORTER Made For this Location ", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
        }

        private async void Frm_Goods_Dispatch_Load()
        {
            txt_Master_Invoice.Focus();

            string ermsg = string.Empty;
            DataTable dtreceive = new DataTable();
            DataTable dtreceivedealer = new DataTable();
            string Str_userName = clsConnection.user;

            dtreceive = DisMgnt.download_TRANSPORTER_Mode(Str_userName, clsConnection.user_WH_ID, ref ermsg);
            if (ermsg == "Success")
            {
                if (dtreceive.Rows.Count > 0)
                {

                    for (int i = 0; i < dtreceive.Rows.Count; i++)
                    {
                        Cmb_Shipment_Mode.Items.Add(dtreceive.Rows[i]["SHIPMENT_MODE"].ToString());
                    }
                }
                else
                {
                    await Navigation.PopAsync();
                    await DisplayAlert("Alert", "No SHIPMENT_MODE Made For this Location ", "OK");
                }
            }
            else
            {

                await Navigation.PopAsync();
                await DisplayAlert("Alert", "No SHIPMENT_MODE Made For this Location ", "OK");
            }

            dtreceive.Clear();
            dtreceive.Dispose();

            dtreceivedealer = DisMgnt.download_SHIPMENT_Dealer(clsConnection.user, clsConnection.user_WH_ID, cmb_Shipment_tranporter.Items.ToString(), ref ermsg);

            if (ermsg == "Success")
            {
                if (dtreceivedealer.Rows.Count > 0)
                {
                    for (int i = 0; i < dtreceivedealer.Rows.Count; i++)
                    {

                    }
                }
            }
            else
            {

            }

            string invo = string.Empty;
            string order = string.Empty;
            string cmbship = string.Empty;
            string cmbtrans = string.Empty;
            string deal = string.Empty;
            string doc = string.Empty;
            string err = string.Empty;
            txtdocket_No.Focus();

        }

        private async void button3_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new DashBoard());
        }


        bool validation_qrcode()
        {
            try
            {
                bool value = false;
                string Str_Output = string.Empty;
                int Int_Scan_Status = 0;
                string ermsg = string.Empty;
                string box = string.Empty;

                Str_Output = Convert.ToString(DisMgnt.valid_box_qrcode(txt_scan_Qr.Text, clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, Txt_Fnl_Dealer_Code.Text, box, ref Int_Scan_Status, ref ermsg));
                if (ermsg == "Success")
                {
                    if (Int_Scan_Status == 0)
                    {
                        value = true;
                    }
                    else
                    {
                        DisplayAlert("Alert", "Already Scaned Box No", "OK");
                        value = false;
                        return value;

                    }
                }
                else
                {
                    DisplayAlert("Alert", ermsg, "OK");
                    value = false;
                    return value;

                }

                return value;
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
                return false;

            }
        }

        private void Txt_Fnl_Dealer_Code_KeyDown_Clicked(object sender, EventArgs e)
        {
            string er = string.Empty;
            string stro = string.Empty;

            stro = Convert.ToString(DisMgnt.chk_deal(Txt_Fnl_Dealer_Code.Text, ref stro, ref er));
            if (er == "Success")
            {
                txt_Master_Invoice.Focus();
                txt_Fnl_Grp_Number_Process.Text = Txt_Fnl_Dealer_Code.Text;
            }
            else
            {
                DisplayAlert("Alert", er, "OK");
                Txt_Fnl_Dealer_Code.Text = string.Empty;
                Txt_Fnl_Dealer_Code.Focus();
            }

        }

        private void txtwt_KeyDown_Clicked(object sender, EventArgs e)
        {
            if (txtwt.Text.Length != 0)
            {
                btnok_Click(sender, e);

            }
        }

        //InvoiceNo INitems = new InvoiceNo();
        //static GrpNoListView grpnoitems = new GrpNoListView();
        //static OrderList odrList = new OrderList();

        bool Chk_Already_Scan()
        {
            try
            {
                bool val = false;

                if (Lst_Box_Invoice_No.ItemsSource == txt_Master_Invoice.Text)
                {
                    DisplayAlert("Message", "Invoice No Already Scan", "OK");
                    val = false;
                    return val;
                }
                else
                {
                    val = true;
                }
                return val;

            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
                return false;
            }
        }

        private async void Address_Label_Print()
        {

            string Str_Box_Label = string.Empty;
            string str_Sql_in = string.Empty;
            str_Sql_in = string.Empty;

            DataTable dt1 = new DataTable();
            string Str_Trans_Val = string.Empty;
            string ship = string.Empty;

            DataTableReader strrs_1;

            strrs_1 = DisMgnt.Get_Lbl_Dtls(txt_scan_Qr.Text.TrimEnd().ToString(), clsConnection.User_Mas_WH_ID);
            if (strrs_1.Read())
            {

                string Str_Fnl_Frp_No = string.Empty;
                string Str_Box_No_Caption = "Box No";
                string Str_To = "To :";
                string Str_From = "From :";
                string Str_Wt_Cap = "Wt";
                string Str_Dealer_Code = string.Empty;
                string Str_Mode_Val = string.Empty;
                string Str_Wt_val = string.Empty;
                string Str_Dealer_Name = string.Empty;
                string Str_Dealer_Add1 = string.Empty;
                string Str_Dealer_Add2 = string.Empty;
                string Str_Dealer_City = string.Empty;
                string Str_Dealer_State = string.Empty;
                string Str_Dealer_Pincode = string.Empty;
                string Str_Dealer_Contact_Person = string.Empty;
                string Str_Dealer_Contact_no = string.Empty;
                string Str_Docket = string.Empty;
                string Str_City_Pincode = string.Empty;
                string Box_No = string.Empty;
                string Str_Box_No_Val = string.Empty;
                string Str_DFrom_Contact_Person = string.Empty;
                string Str_From_City = string.Empty;
                string contact_person = string.Empty;
                string contact_no = string.Empty;
                string Str_From_Di = "SPARE PARTS DIVISION";
                string Str_From_Name = "KUBOTA AGRICULTURAL MACHINERY INDIA PVT LTD";
                string Str_From_Add1 = string.Empty;
                string Str_From_Add2 = string.Empty;
                string Str_From_State = string.Empty;
                string Str_From_Pincode = string.Empty;
                string Str_From_Contact_no = string.Empty;
                string totcoun = string.Empty;
                string sql = string.Empty;

                if (clsConnection.User_Mas_WH_ID == "CH0001")
                {
                    Str_From_Add1 = "INDOSPACE INDUSTRIAL PARK,104";
                    Str_From_Add2 = "B-500,POLIVAKKAM VILLAGE,";
                    Str_From_State = "TAMIL NADU";
                    Str_From_Pincode = "602 002";
                    Str_From_Contact_no = "PH : 044-40192000";

                }

                else if (clsConnection.User_Mas_WH_ID == "PU0001")
                {
                    Str_From_Add1 = "GAT NO-338,MAHALUNGE VILLAGE,";
                    Str_From_Add2 = "CHAKAN MIDC,PUNE,";
                    Str_From_State = "MAHARASHTRA";
                    Str_From_Pincode = "410 501";
                    Str_From_Contact_no = "PH : 02135-626400";
                }

                else if (clsConnection.User_Mas_WH_ID == "FA0001")
                {
                    Str_From_Add1 = "#2.5.1,APEEJAY GLOBAL IND&LOG PARK LTD,";
                    Str_From_Add2 = "DELHI MATHURA ROAD,BALLABGARH,";
                    Str_From_State = "FARIDABAD";
                    Str_From_Pincode = "121 004";
                    Str_From_Contact_no = "HARYANA,PH : 8925536272";
                }
                string Str_From_State_Pincode = Str_From_State + ",PIN-" + Str_From_Pincode;
                string str_All_invoice_no_cap = string.Empty;
                str_All_invoice_no_cap = "No: ";
                Str_Dealer_Code = strrs_1.GetValue(0).ToString();
                Str_Fnl_Frp_No = strrs_1.GetValue(6).ToString();
                Box_No = strrs_1.GetString(5).ToString().Trim();
                Str_Box_No_Val = strrs_1.GetValue(1).ToString();
                Str_Wt_val = strrs_1.GetValue(3).ToString() + "Kg";
                string[] dat;
                dat = Str_Box_No_Val.Split(',');
                if (dat.Length > 1)
                {
                    Str_Box_No_Val = string.Empty;
                    for (int j = 0; j < dat.Length; j++)
                    {
                        if (j == 1)
                        {
                            Str_Box_No_Val = Str_Box_No_Val + dat[j];
                        }
                        if (j == 2)
                        {
                            Str_Box_No_Val = Str_Box_No_Val + dat[j];
                        }
                    }
                }

                string inv = string.Empty;
                Str_Box_No_Val = clsConnection.user_WH_ID + Str_Box_No_Val.Replace("00", "");
                inv = strrs_1.GetString(7);
                Str_Trans_Val = strrs_1.GetString(8);
                Str_Mode_Val = strrs_1.GetString(9);
                Str_Docket = strrs_1.GetString(10);
                if (!strrs_1.IsDBNull(11))
                {
                    ship = strrs_1.GetString(11);
                }
                else
                {
                    ship = string.Empty;
                }
                string str_All_invoice_no = str_All_invoice_no_cap + inv.TrimEnd();
                string dealer_stat = string.Empty;
                string box_unique = string.Empty;

                strrs_1.Close();
                DataTableReader rd;
                if (ship.Length == 0)
                {
                    rd = DisMgnt.Get_Dealer_Catlog(Str_Dealer_Code, "");
                }
                else
                {
                    rd = DisMgnt.Get_Dealer_Catlog(Str_Dealer_Code, ship);
                }

                if (rd.Read())
                {
                    Str_Dealer_Name = rd.GetValue(2).ToString();
                    Str_Dealer_Add1 = rd.GetValue(3).ToString();
                    Str_Dealer_Add2 = rd.GetValue(4).ToString().Trim();
                    Str_Dealer_City = rd.GetValue(5).ToString();
                    Str_Dealer_State = "STATE : " + rd.GetValue(6).ToString();

                    Str_Dealer_Pincode = rd.GetValue(7).ToString();
                    Str_Dealer_Contact_Person = "CONTACT PERSON :" + rd.GetValue(8).ToString();

                    Str_Dealer_Contact_no = "CONTACT N0 :+91" + rd.GetValue(9).ToString();
                }
                rd.Close();

                totcoun = Convert.ToString(DisMgnt.GetBoxCnt(Str_Dealer_Code.TrimEnd(), clsConnection.User_Mas_WH_ID));

                Str_City_Pincode = Str_Dealer_City + " - " + Str_Dealer_Pincode;
                string str_mode_transp_val = Str_Mode_Val + " - " + Str_Trans_Val;

                string printerIpAddress = Printer_IP_Address.ToString();

                try
                {

                    string zplCommand;
                    TcpClient client = new TcpClient();
                    await client.ConnectAsync(printerIpAddress, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                    zplCommand = "^XA" +
                                "~TA000" +
                                "~JSN" +
                                "^LT0" +
                                "^MNW" +
                                "^MTT" +
                                "^PON" +
                                "^PMN" +
                                "^LH0,0" +
                                "^JMA" +
                                "^PR30,30" +
                                "~SD25" +
                                "^JUS" +
                                "^LRN" +
                                "^CI27" +
                                "^PA0,1,1,0" +
                                "^XZ" +
                                "^XA" +
                                "^MMT" +
                                "^PW760" +
                                "^LL1199" +
                                "^LS0" +
                                "^FT82,1133^A0B,34,33^FH\\^CI28^FD" + Str_To + "^FS^CI27" +
                                "^FT136,1138^A0B,39,38^FH\\^CI28^FD" + Str_Dealer_Name + "^FS^CI27" +
                                "^FT178,1138^A0B,34,33^FH\\^CI28^FD" + Str_Dealer_Add1 + "^FS^CI27" +
                                "^FT222,1138^A0B,34,33^FH\\^CI28^FD" + Str_Dealer_Add2 + "^FS^CI27" +
                                "^FT270,1138^A0B,42,43^FH\\^CI28^FD" + Str_City_Pincode + "^FS^CI27" +
                                "^FT313,1138^A0B,34,35^FH\\^CI28^FD" + Str_Dealer_State + "^FS^CI27" +
                                "^FT359,1138^A0B,34,33^FH\\^CI28^FD" + Str_Dealer_Contact_Person + "^FS^CI27" +
                                "^FT404,1138^A0B,34,33^FH\\^CI28^FD" + Str_Dealer_Contact_no + "^FS^CI27" +
                                "^FT437,761^A0B,32,33^FH\\^CI28^FD" + Str_From + "^FS^CI27" +
                                "^FT518,763^A0B,31,30^FH\\^CI28^FD" + Str_From_Name + "^FS^CI27" +
                                "^FT478,763^A0B,31,30^FH\\^CI28^FD" + Str_From_Di + "^FS^CI27" +
                                "^FT563,763^A0B,31,30^FH\\^CI28^FD" + Str_From_Add1 + "^FS^CI27" +
                                "^FT607,761^A0B,31,30^FH\\^CI28^FD" + Str_From_Add2 + "^FS^CI27" +
                                "^FT650,761^A0B,31,30^FH\\^CI28^FD" + Str_From_State_Pincode + "^FS^CI27" +
                                "^FT695,761^A0B,31,30^FH\\^CI28^FD" + Str_From_Contact_no + "^FS^CI27" +
                                "^FO436,779^GB161,360,2^FS" +
                                "^FO607,778^GB70,360,2^FS" +
                                "^FO32,53^GB678,1103,4^FS" +
                                "^FT481,1023^A0B,35,35^FH\\^CI28^FD" + Str_Box_No_Caption + "^FS^CI27" +
                                "^FT569,1007^A0B,45,46^FH\\^CI28^FD" + Box_No + "^FS^CI27" +
                                "^FT654,1123^A0B,35,41^FH\\^CI28^FD" + Str_Wt_Cap + "^FS^CI27" +
                                "^FT652,984^A0B,35,35^FH\\^CI28^FD" + Str_Wt_val + "^FS^CI27" +
                                "^FT552,291^BQN,2,4" +
                                "^FH\\^FDLA," + Str_Fnl_Frp_No + "^FS" +
                                "^FT746,1138^A0B,31,30^FH\\^CI28^FD" + str_All_invoice_no + "^FS^CI27" +
                                "^FT743,377^A0B,31,30^FH\\^CI28^FD" + Str_Box_No_Val + "^FS^CI27" +
                                "^FO45,69^GB49,587,2^FS" +
                                "^FT80,262^A0B,25,25^FH\\^CI28^FD" + Str_Docket + "^FS^CI27" +
                                "^FT78,644^A0B,23,23^FH\\^CI28^FD" + str_mode_transp_val + "^FS^CI27" +
                                "^FO45,331^GB48,0,3^FS" +
                                "^FO506,780^GB0,358,3^FS" +
                                "^FO607,1060^GB67,0,3^FS" +
                                "^PQ1,0,1,Y" +
                                "^XZ";

                    bool result = false;
                    result = DisMgnt.Updt_Lbl_sts(Str_Fnl_Frp_No);


                    byte[] zplCommandBytes = Encoding.ASCII.GetBytes(zplCommand);
                    await client.GetStream().WriteAsync(zplCommandBytes, 0, zplCommandBytes.Length);

                    zplCommand = string.Empty;
                    client.Close();
                    client.Dispose();

                }
                catch (ConnectionException ex)
                {
                    Console.WriteLine("Connection error: " + ex.Message);
                }
                catch (ZebraPrinterLanguageUnknownException ex)
                {
                    Console.WriteLine("Printer language unknown: " + ex.Message);
                }

            }
        }

        private void txtdocket_No_Completed(object sender, EventArgs e)
        {
            try
            {
                if (txtdocket_No.Text.Length != 0)
                {
                    txt_scan_Qr.Focus();
                }
                else
                {
                    DisplayAlert("Message", "Enter Docket No", "OK");
                    txtdocket_No.Focus();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");

            }
        }

        private async void txt_Master_Invoice_Completed(object sender, EventArgs e)
        {

            var lb_invoice_no = Lst_Box_Invoice_No.ItemsSource as IList;
            var lb_dis_itms = lst_Fnl_Grp_No.ItemsSource as IList;
            var lb_lst_order = lst_order.ItemsSource as IList;

            try
            {

                if (txt_Master_Invoice.Text.Trim().Length != 0)
                {

                    if (Chk_Already_Scan())
                    {
                        string err = string.Empty;

                        DataTableReader rd;

                        if (Txt_Fnl_Dealer_Code.Text == null || Txt_Fnl_Dealer_Code.Text == "")
                        {
                            string dlr = string.Empty;

                            rd = DisMgnt.Get_Dealer(txt_Master_Invoice.Text.ToString().TrimEnd());
                            if (rd.Read())
                            {
                                Txt_Fnl_Dealer_Code.Text = rd.GetString(0);
                            }
                            else
                            {
                                await DisplayAlert("Message", "No Dealer Alert For This Invoice", "OK");
                            }

                            rd.Close();

                        }

                        string ord = string.Empty;
                        string[] data;
                        string ship = string.Empty;
                        string de = string.Empty;

                        string chkinv = DisMgnt.chk_disp_inv(Txt_Fnl_Dealer_Code.Text.ToString().TrimEnd(), txt_Master_Invoice.Text.ToString().TrimEnd(), ref ord, ref err, ref ship, ref de);
                        if (err == "Success")
                        {
                            if (Txt_Fnl_Dealer_Code.Text.Length != 0)
                            {
                                if (Txt_Fnl_Dealer_Code.Text.TrimEnd() == de.TrimEnd())
                                {
                                    if (Lst_Box_Invoice_No.ItemsSource != txt_Master_Invoice.Text.Trim())
                                    {
                                        if (invoicedata.Contains(txt_Master_Invoice.Text) == false)
                                        {
                                            string INitems = txt_Master_Invoice.Text;
                                            invoicedata.Add(INitems);
                                            Lst_Box_Invoice_No.ItemsSource = invoicedata;
                                        }

                                    }

                                    data = ord.Split(',');

                                    if (ship == "A")
                                    { }
                                    else
                                    {
                                        txtship.Text = ship.TrimEnd();
                                    }

                                    txt_Fnl_Grp_Number_Process.Text = de;
                                    if (grpnodata.Contains(txt_Master_Invoice.Text) == false)
                                    {
                                        if (data.Length > 0)
                                        {
                                            for (int i = 0; i < data.Length; i++)
                                            {
                                                string addata = data[i].ToString();
                                                grpnodata.Add(addata);

                                                string order = data[i].ToString();
                                                orderdata.Add(order);
                                            }
                                        }
                                        lst_Fnl_Grp_No.ItemsSource = grpnodata;

                                        lst_order.ItemsSource = orderdata;
                                    }

                                    txt_Master_Invoice.Focus();

                                }
                                else
                                {
                                    await DisplayAlert("Message", "Scanned Invoice and Before Invoice Dealer is Mismatch", "OK");
                                    txt_Master_Invoice.Text = string.Empty;
                                    txt_Master_Invoice.Focus();

                                }
                            }
                            else
                            {
                                if (lb_invoice_no.Contains(txt_Master_Invoice.Text) == false)
                                {
                                    lb_invoice_no.Add(txt_Master_Invoice.Text);
                                }
                                data = ord.Split(',');

                                if (ship == "A")
                                {

                                }
                                else
                                {
                                    txtship.Text = ship.TrimEnd();
                                }

                                Txt_Fnl_Dealer_Code.Text = de;

                                txt_Fnl_Grp_Number_Process.Text = de;

                                if (data.Length > 0)
                                {
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        string grpd = data[i].ToString();

                                        grpnodata.Add(grpd);

                                        string orddta = data[i].ToString();
                                        orderdata.Add(orddta);
                                    }
                                }
                                lst_Fnl_Grp_No.ItemsSource = grpnodata;

                                lst_order.ItemsSource = orderdata;
                                txt_Master_Invoice.Focus();
                                txt_Master_Invoice.Text = string.Empty;

                            }
                        }
                        else
                        {
                            await DisplayAlert("Message", "Invalid Invoice No.", "OK");
                            txt_Master_Invoice.Text = string.Empty;
                            txt_Master_Invoice.Focus();

                        }

                    }
                }
                else
                {
                    await DisplayAlert("Message", "Scan Invoice Number", "OK");
                    txt_Master_Invoice.Text = string.Empty;
                    txt_Master_Invoice.Focus();
                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }
            txt_Master_Invoice.CursorPosition = 0;
            txt_Master_Invoice.SelectionLength = txt_Master_Invoice.MaxLength;
            txt_Master_Invoice.Focus();
        }
        private async void txt_scan_Qr_Completed(object sender, EventArgs e)
        {
            var lst1 = lst_Fnl_Grp_No.ItemsSource as IList;
            try
            {

                if (txt_scan_Qr.Text.Trim().ToString().Length != 0)
                {

                    if (validation_qrcode())
                    {

                        string str_out = string.Empty;
                        string err = string.Empty;
                        int tot = 0;
                        int bal = 0;
                        string ord = string.Empty;
                        for (int i = 0; i < lst1.Count; i++)
                        {

                            ord = ord + orderdata[i].ToString() + ",";
                        }
                        ord = ord.Substring(0, ord.Length - 1);

                        str_out = Convert.ToString(DisMgnt.get_box_wt(txt_scan_Qr.Text, clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, Txt_Fnl_Dealer_Code.Text, ord, ref tot, ref bal, ref err));
                        if (err == "Success")
                        {
                            txtwt.Text = str_out;
                            btnok.IsVisible = true;
                        }
                        else
                        {
                            await DisplayAlert("Alert", err, "OK");
                            txt_scan_Qr.Text = string.Empty;
                            txt_scan_Qr.Focus();

                        }
                    }
                    else
                    {
                        txt_scan_Qr.Text = string.Empty;
                        txt_scan_Qr.Focus();
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Scan Box Barcode", "OK");
                    txt_scan_Qr.Focus();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }


        }

        private void txtwt_Completed(object sender, EventArgs e)
        {

        }
    }
}