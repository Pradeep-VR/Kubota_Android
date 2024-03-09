using kWMS.Extras;
using kWMS.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kWMS.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Binning : ContentPage
    {
        BinningManagement BinMgt = new BinningManagement();
        public static class GlobalVariablesStatus
        {
            public static string BinFlag = "";

        }

        public class ItemViewModel
        {
            public string PartNo { get; set; }
            public string PartName { get; set; }
            public string Qty { get; set; }
            public string Bal_Qty { get; set; }
        }
        //public class LoggedInUser
        //{
        //    public static string userName { get; set; }
        //    public static string email { get; set; }
        //    public static string WH_LOCATION_ID { get; set; }
        //}

        public string mast = clsConnection.User_Mas_WH_ID;

        public Binning()
        {
            //string uname = Login.LoggedInUser.userName;
            //string swhid = Login.GlobalVariables1.User_WH_ID;
            
            string uname =clsConnection.user;
            string swhid = clsConnection.user_WH_ID;

            InitializeComponent();
            frmBinning_Load();


        }



        public void frmBinning_Load()
        {
            //168, 237
            radioButton1.IsChecked = false;

            //label5.IsVisible = false;
            lbltotqty.Text = string.Empty;
            lblbalqty.Text = string.Empty;

            txtSBinLoc.IsEnabled = false;
            lblstatusmessage.Text = string.Empty;
            txt_old_partno.Text = string.Empty;//txt_old_partno.clear
                                               //  lstundprd.IsVisible = false;
            txtinvocenumber.IsEnabled = false;
            txtcart.IsEnabled = false;
            txt2DBarcode.Focus();

        }

        public string server;
        public string database;
        public string uid;
        public string password;
        //string sql;
        string Inique_No = string.Empty;
        string Err = string.Empty;
        int binflag = 0;
        int errflag22 = 0;
        string fl1 = string.Empty;
        public string sbar = string.Empty;
        public string barfl = string.Empty;
        public string STflag = string.Empty;
        public string comfl = string.Empty;
        string[] data;
        string Str_Invoice;
        public int k;


        public void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value == true)
            {
                txtinvocenumber.IsEnabled = true;
                txtcart.IsEnabled = true;
                txtinvocenumber.BackgroundColor = System.Drawing.Color.White;
                txtcart.BackgroundColor = System.Drawing.Color.White;

                txtQty.IsEnabled = false;

                txtinvocenumber.Focus();
            }
            else
            {
                //txtinvocenumber.Text = "";
                txtcart.Text = "";
                txtinvocenumber.IsEnabled = false;
                txtcart.IsEnabled = false;
                txtinvocenumber.BackgroundColor = System.Drawing.Color.LightGray;
                txtcart.BackgroundColor = System.Drawing.Color.LightGray;

                txtQty.IsEnabled = true;

                txt2DBarcode.Focus();
            }
        }

        
        public async void Get_Grn_BalQty(string inv, string partno)
        {
            DataTable dt23 = new DataTable();
            try
            {
                dt23 = BinMgt.Get_Grn_Balance(inv, partno, clsConnection.User_Mas_WH_ID);
                if (dt23.Rows.Count > 0)
                {
                    int v1 = 0;
                    for (int i = 0; i < dt23.Rows.Count; i++)
                    {
                        if (v1 == 0)
                        {
                            v1 = Convert.ToInt16(dt23.Rows[i]["BAL_QTY"].ToString());
                        }
                        else
                        {
                            v1 += Convert.ToInt16(dt23.Rows[i]["BAL_QTY"].ToString());
                        }
                    }
                    txt_bal_Qty.Text = v1.ToString();

                    if (txt_bal_Qty.Text.Length == 0)
                    {
                        await DisplayAlert("Alert", "Already Binning Completed...", "OK");
                        clearForm();

                    }
                }
                else
                {
                }
            }
            catch (Exception)
            {
                throw new Exception("Balance Quantity Not Available.");
            }

        }

        
        public bool saveBinningDetails(string GRNNo, string TwoDBarcode, string PartNo, int Qty, string BinLocation, string ScannedBinLocation, string BinningBy, int Bin_Loc_Cr, string Str_Uniq, string Invoice_No, string Str_WH_Id, ref string Part_St, ref string Err)
        {             
            try
            {

                Err = string.Empty;
                bool res = false;
                DateTime dt = DateTime.Now;

                DataTableReader rd = BinMgt.Get_Grn_Qtys(Invoice_No, PartNo, mast);
                string grnno = string.Empty;
                int Int_Tot_GRN_Qty = 0;
                int Int_Bal_GRN_Qty = 0;
                int Int_Rec_GRN_Qty = 0;
                if (rd.Read())
                {
                    Int_Tot_GRN_Qty = Convert.ToInt32(rd.GetValue(0));
                    Int_Bal_GRN_Qty = Convert.ToInt32(rd.GetValue(2));
                    Int_Rec_GRN_Qty = Convert.ToInt32(rd.GetValue(1));
                    grnno = rd.GetString(3);
                }

                rd.Close();
                int Int_Fin_Rec_Qty = Int_Rec_GRN_Qty + Qty;
                int Int_Fin_Bal_Qty = Int_Tot_GRN_Qty - Int_Fin_Rec_Qty;

                if (Int_Fin_Rec_Qty <= Int_Tot_GRN_Qty)
                {
                   
                    res = BinMgt.Insert_GrnDtls(grnno, TwoDBarcode, PartNo, Qty, BinLocation, ScannedBinLocation, BinningBy, Bin_Loc_Cr, Str_WH_Id);
                    if(res != false)
                    {
                        Err = "GRNDetails completed";
                    }
                    
                    scaned_bin_loc.Text = ScannedBinLocation.ToString();

                    if (GlobalVariablesStatus.BinFlag == "M")
                    {
                        if (txt_bal_Qty.Text == txtQty.Text)
                        {
                            res = false;
                            res = BinMgt.Updt_IwRecMast(BinningBy, ScannedBinLocation, Invoice_No, PartNo, mast);
                            if (res != false)
                            {
                                Err = "GRNDetails completed" + "INWARD_RECEIPT_Master updated";
                            }

                        }

                        res = false;

                        res = BinMgt.Updt_InwRec_Sts_M(txtQty.Text, Invoice_No, PartNo, mast, BinningBy, ScannedBinLocation);
                        if (res != false)
                        {
                            Err = "GRNDetails completed" + "INWARD_RECEIPT_Master updated" + "INWARD_RECEIPT updated";
                        }
                        
                    }
                    else if (GlobalVariablesStatus.BinFlag == "C")
                    {
                        res = false;

                        res = res = BinMgt.Updt_InwRec_Sts_C(txtQty.Text, Invoice_No, PartNo, mast,Str_Uniq,  BinningBy, ScannedBinLocation);
                        if(res != false)
                        {
                            Err = "GRNDetails completed" + "INWARD_RECEIPT updated";
                        }

                    }
                    res = false;

                    res = BinMgt.Updt_GrnM_Bal(Int_Fin_Rec_Qty, Int_Fin_Bal_Qty, Invoice_No, PartNo, mast, grnno);
                    if(res != false)
                    {
                        Err = "GRN All Completed";
                    }
                    
                    Part_St = PartNo + " Bal.Qty - " + Int_Fin_Bal_Qty;

                    return true;
                }
                else
                {
                    Err = "Invalid Qty Compared Total&Rec. Qty";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Err = Err + ex.Message;
                return false;
            }
        }


        

        private void btnSave_Click(object sender, EventArgs e)
        {
            Fin_Save();
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            txt2DBarcode.Text = string.Empty;
            txtPartNo.Text = string.Empty;
            txtQty.Text = string.Empty;
            txtBinLoc.Text = string.Empty;
            txtSBinLoc.Text = string.Empty;
            cmbGRNNos.Text = string.Empty;
        }


        private async void txtQty_KeyDown(object sender, EventArgs e)
        {

            if (Convert.ToInt32(txtQty.Text) <= Convert.ToInt32(txt_bal_Qty.Text))
            {

                if (txtSBinLoc.Text.Length != 0)
                {
                    txtSBinLoc_Completed(sender, e);
                }
                else
                {
                    txtSBinLoc.Focus();
                }

            }
            else if (Convert.ToInt32(txtQty.Text) > Convert.ToInt32(txt_bal_Qty.Text))
            {
                await DisplayAlert("Alert", "Mismatch Qty", "OK");
                txtQty.Text = string.Empty;
                txtQty.Focus();
            }


        }

        bool vildate_Check_Inward()
        {

            try
            {
                bool vaild = false;
                Err = string.Empty;

                if (BinMgt.Validate_Binning_Inward(txtinvocenumber.Text, txtPartNo.Text, Convert.ToInt32(txtQty.Text), Inique_No, clsConnection.user_WH_ID, mast , ref Err))
                {
                    vaild = true;
                }
                else
                {
                    vaild = false;
                    DisplayAlert("Alert", Err, "OK");
                }

                return vaild;

            }
            catch (Exception)
            {
                DisplayAlert("Alert", "This Barcode Already Binned.", "OK");
                return false;

            }

        }

        bool vildate_Check_Already_Binning()
        {


            try
            {

                bool vaild = false;

                Err = string.Empty;

                if (BinMgt.Validate_Binning_Done_Chk(txtinvocenumber.Text, txtPartNo.Text, Convert.ToInt32(txtQty.Text), Inique_No, clsConnection.user_WH_ID, ref Err))
                {
                    vaild = true;

                }
                else
                {
                    vaild = false;
                    DisplayAlert("Alert", Err, "OK");

                    txt2DBarcode.Focus();
                    clearForm();
                    txt2DBarcode.Text = string.Empty;

                }

                return vaild;

            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
                return false;

            }

        }

        bool vildate_Check_GRN()
        {

            try
            {
                bool vaild = false;
                Err = string.Empty;
                if (BinMgt.Validate_Binning_GRN(txtinvocenumber.Text, txtPartNo.Text, Convert.ToInt32(txtQty.Text), clsConnection.user_WH_ID, mast , ref Err))
                {
                    vaild = true;
                }
                else
                {
                    vaild = false;
                    DisplayAlert("Alert", Err, "OK");
                    txt2DBarcode.Focus();
                    txt2DBarcode.Text = string.Empty;
                }
                return vaild;
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
                return false;

            }

        }

        bool vildate_Check_GRN_Qty_Avl()
        {
            try
            {
                bool vaild = false;

                Err = string.Empty;
                int Int_Tot_GRN_Qty = 0;
                int Int_Bal_GRN_Qty = 0;
                int Int_Rec_GRN_Qty = 0;

                if (BinMgt.Validate_Binning_GRN_Avl_Qty(txtinvocenumber.Text, txtPartNo.Text, Convert.ToInt32(txtQty.Text), clsConnection.user_WH_ID , mast , ref Int_Tot_GRN_Qty, ref Int_Bal_GRN_Qty, ref Int_Rec_GRN_Qty, ref Err))
                {
                    txt_bal_Qty.Text = Convert.ToString(Int_Bal_GRN_Qty);
                    vaild = true;
                }
                else
                {
                    vaild = false;
                    txt_bal_Qty.Text = Convert.ToString(Int_Bal_GRN_Qty);
                    DisplayAlert("Alert", Err, "OK");
                    txt2DBarcode.Focus();
                    clearForm();
                    txt2DBarcode.Text = string.Empty;
                }
                return vaild;

            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
                return false;

            }

        }

        private void txtcart_KeyDown(object sender, EventArgs e)
        {

            txt2DBarcode.Focus();
            lblstatusmessage.Text = "Scan QR Barcode";
        }

        private void btn_next_Carton_Click_1(object sender, EventArgs e)
        {
            txtcart.Text = string.Empty;
            txtcart.Focus();
        }

        public async void Process_Srtart()
        {
            try
            {
                if (chkbinng.IsChecked == false)
                {
                    if (vildate_Check_Inward())
                    {
                        if (vildate_Check_GRN())
                        {
                            if (vildate_Check_Already_Binning())
                            {
                                if (vildate_Check_GRN_Qty_Avl())
                                {
                                    string StrOut = BinMgt.GetBinLocation(txtPartNo.Text, txtinvocenumber.Text, cmbGRNNos.Text, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID);
                                    if (StrOut == "Invalid Part Number")
                                    {
                                        await DisplayAlert("Alert", "Invalid Part Number", "OK");
                                        txtSBinLoc.Text = string.Empty;
                                        txtSBinLoc.IsEnabled = false;
                                        txt2DBarcode.Text = string.Empty;
                                        txt2DBarcode.Focus();
                                    }
                                    else
                                    {
                                        string[] loc;
                                        loc = StrOut.Split(',');

                                        if (loc.Length > 1)
                                        {
                                            txtBinLoc.Text = loc[0];
                                            if (loc[1] == "A")
                                            {

                                            }
                                            {
                                                textBox1.Text = loc[1];
                                            }
                                        }
                                        string neloc = string.Empty;



                                        if (txtBinLoc.Text == "")
                                        {
                                            neloc = BinMgt.getbinnn(txtPartNo.Text, txtinvocenumber.Text);
                                            txtBinLoc.Text = neloc;
                                        }

                                        string binloc = string.Empty;
                                        int totqty = 0;
                                        int balqty = 0;
                                        if (BinMgt.getqty(txtPartNo.Text, txtinvocenumber.Text, clsConnection.user_WH_ID, mast , ref binloc, ref totqty, ref balqty))
                                        {
                                            lbltotqty.Text = Convert.ToString(totqty);
                                            lblbalqty.Text = Convert.ToString(balqty);
                                            txt_bal_Qty.Text = Convert.ToString(balqty);
                                        }
                                        else
                                        {
                                            //invalid data
                                        }

                                        if (chkbinng.IsChecked == false)
                                        {
                                            if (txt_old_partno.Text.TrimEnd() == txtPartNo.Text.TrimEnd())
                                            {
                                                if (radioButton1.IsChecked == true)
                                                {
                                                    txtSBinLoc.Text = textBox1.Text;
                                                }
                                                else
                                                {//CHECH HERE BIN LOC
                                                    txtSBinLoc.Text = scaned_bin_loc.Text.Trim();
                                                }
                                                if (STflag == "ST")
                                                {
                                                    //txtSBinLoc.Focus();
                                                    After_Loc_Scan();

                                                    comfl = "OK";
                                                    chkbinng.IsChecked = true;
                                                }
                                                else
                                                {
                                                    if (fl1 == "B")
                                                    {
                                                        After_Loc_Scan();
                                                        comfl = "OK";
                                                        chkbinng.IsChecked = true;
                                                    }
                                                    else
                                                    {
                                                        After_Loc_Scan();
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                if (fl1 == "B")
                                                {
                                                    txtSBinLoc.Focus();
                                                }
                                                else
                                                {
                                                    txtSBinLoc.Text = string.Empty;
                                                    txtSBinLoc.IsEnabled = true;
                                                    txtSBinLoc.Focus();
                                                }
                                            }
                                        }

                                        if (txtqtylen.Text == "0")
                                        {

                                            if (fl1 == "B")
                                            {

                                                if (STflag == "ST")
                                                {
                                                    txtSBinLoc.Focus();

                                                }
                                                else
                                                {
                                                    txtSBinLoc.Focus();
                                                }
                                            }
                                            else
                                            {

                                                txtSBinLoc.IsEnabled = true;
                                                txtSBinLoc.Focus();
                                                txtQty.Focus();
                                            }
                                        }
                                        else
                                        {
                                            if (txt_old_partno.Text.TrimEnd() == txtPartNo.Text.TrimEnd())

                                            {
                                                After_Loc_Scan();
                                                comfl = "OK";

                                            }
                                            else
                                            {

                                                if (fl1 == "B")
                                                {

                                                }
                                                else
                                                {
                                                    if (STflag == "" && fl1 == "" && barfl == "")
                                                    {

                                                    }
                                                    else
                                                    {
                                                        txtSBinLoc.Text = string.Empty;
                                                        txtSBinLoc.IsEnabled = true;
                                                        txtSBinLoc.Focus();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                                else
                                {
                                    lblstatusmessage.Text = "Qty Missmatch";
                                    txtSBinLoc.Text = string.Empty;
                                    txtSBinLoc.IsEnabled = false;
                                    txt2DBarcode.Focus();
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                            txtSBinLoc.Text = string.Empty;
                            txtSBinLoc.IsEnabled = false;

                        }
                    }

                }
                else//Direct Binning Process
                {
                    txtSBinLoc.Text = string.Empty;
                    txtSBinLoc.IsEnabled = false;

                    txtinvocenumber.IsEnabled = true;
                    txtPartNo.IsEnabled = true;

                    if (data.Length > 3)
                    {
                        txtPartNo.Text = data[0];
                        txtQty.Text = data[1];
                    }
                    else
                    {

                        string valid_part_value = string.Empty;
                        string err = string.Empty;
                        string str_ack = string.Empty;

                        if (txt2DBarcode.Text.Contains('/'))
                        {
                            str_ack = BinMgt.get_valid_part(txt2DBarcode.Text, ref valid_part_value, ref err);
                            if (err == "Success")
                            {
                                txtPartNo.Text = valid_part_value;
                            }
                            else
                            {
                                await DisplayAlert("Alert", err.ToString(), "OK");
                                txt2DBarcode.Text = string.Empty;
                                txt2DBarcode.Focus();
                            }
                        }
                        else if (txt2DBarcode.Text.Length > 34 && txt2DBarcode.Text.Length <= 38)
                        {
                            txtPartNo.Text = txt2DBarcode.Text.Substring(0, 5) + "-" + txt2DBarcode.Text.Substring(5, 5);
                        }
                        else
                        {
                            str_ack = BinMgt.get_valid_part(txt2DBarcode.Text, ref valid_part_value, ref err);
                            if (err == "Success")
                            {
                                txtPartNo.Text = valid_part_value;
                            }
                            else
                            {
                                await DisplayAlert("Alert", err.ToString(), "OK");
                                txt2DBarcode.Text = string.Empty;
                                txt2DBarcode.Focus();
                            }
                        }

                    }

                    if (txtinvocenumber.Text.Length == 0)
                    {
                        await DisplayAlert("Alert", "Please Enter/Scan Invoice No., after Scan QR Code", "OK");
                        txtinvocenumber.Text = string.Empty;
                        txt2DBarcode.Text = string.Empty;

                        txtinvocenumber.Focus();
                    }
                    else if (txtPartNo.Text.Length == 0)
                    {
                        await DisplayAlert("Alert", "Please Enter the Carton No., after Scan QR Code", "OK");
                        txtPartNo.Text = string.Empty;
                        txt2DBarcode.Text = string.Empty;

                        txtPartNo.Focus();
                    }
                    if (txtinvocenumber.Text.TrimEnd().Length != 0 && txtPartNo.Text.TrimEnd().Length != 0)
                    {
                        try
                        {

                            int Int_Rtn_Code = 0;
                            string Str_Part_Name = string.Empty;
                            string ermsg = string.Empty;
                            string str = null;
                            string[] strArr = null;
                            string part_no;
                            str = txt2DBarcode.Text;

                            char[] splitchar = { ',' };
                            strArr = str.Split(splitchar);
                            string Str_Scan_Part;
                            int Str_Scan_Qty;

                            if (txt2DBarcode.Text.Length != 0)
                            {
                                if (txt2DBarcode.Text.Length >= 10)
                                {
                                    Str_Invoice = txtinvocenumber.Text;
                                    string Str_Output_Length = string.Empty;
                                    if (data.Length > 3)
                                    {
                                        Str_Scan_Part = data[0];
                                    }
                                    else
                                    {
                                        Str_Scan_Part = txt2DBarcode.Text.Substring(0, 5) + "-" + txt2DBarcode.Text.Substring(5, 5);
                                    }

                                    string len;
                                    len = string.Empty;
                                    len = "B";
                                    lblstatusmessage.Text = txtPartNo.Text;
                                    string str1 = null;
                                    string[] strArr1 = null;

                                    Str_Output_Length = Convert.ToString(BinMgt.Get_In_Qty_From_QR(txtinvocenumber.Text, txt2DBarcode.Text, txtPartNo.Text, clsConnection.user, txtcart.Text, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, clsConnection.User_Type, len, ref ermsg));

                                    str1 = Str_Output_Length;
                                    char[] splitchar1 = { ',' };
                                    strArr1 = str1.Split(splitchar1);
                                    Int_Rtn_Code = Convert.ToInt32(strArr1[0]);

                                    if (Int_Rtn_Code == 0)
                                    {
                                        await DisplayAlert("Alert", "Already Completed/Invoice No. & Part No. MisMatch", "OK");
                                        txt2DBarcode.Text = string.Empty;
                                        txtPartNo.Text = string.Empty;
                                        txt2DBarcode.Focus();
                                    }
                                    else
                                    {
                                        if (txtqtylen.Text == "0")
                                        {
                                            if (data.Length > 3)
                                            {
                                                Str_Scan_Qty = Convert.ToInt32(data[1]);
                                                txtQty.Text = Convert.ToString(Str_Scan_Qty);
                                            }
                                            else
                                            {

                                                txtQty.Text = "1";
                                            }

                                        }
                                        else
                                        {

                                            string partl = string.Empty;
                                            partl = txtPartNo.Text.Replace("-", "");
                                            int partlen = partl.Length;
                                            try
                                            {
                                                string rtt = txt2DBarcode.Text.Substring(partlen, 0);

                                            }
                                            catch (Exception)
                                            {

                                                errflag22 = 1;

                                                await DisplayAlert("Alert", "Qty Length Mismatch", "OK");
                                                txt2DBarcode.Text = string.Empty;
                                                txt2DBarcode.IsEnabled = true;
                                                txtPartNo.Text = string.Empty;
                                                Str_Scan_Qty = 0;
                                                txt2DBarcode.Focus();

                                            }

                                        }

                                        Str_Scan_Qty = Convert.ToInt32(txtQty.Text);

                                        int Int_Bal_Qty = 0;
                                        int Int_Tot_Asn_Qty = 0;

                                        Int_Bal_Qty = Convert.ToInt32(strArr1[3]);
                                        Int_Tot_Asn_Qty = Convert.ToInt32(strArr1[4]);
                                        txt_bal_Qty.Text = Convert.ToString(Int_Bal_Qty);

                                        if (Convert.ToInt32(txtQty.Text) > Int_Bal_Qty)
                                        {
                                            await DisplayAlert("Alert", txtPartNo.Text + " - " + Int_Tot_Asn_Qty + ". Scan Qty Greater Than Uploaded Qty", "OK");
                                            txtQty.Text = string.Empty;
                                            txtPartNo.Text = string.Empty;
                                            txt2DBarcode.Text = string.Empty;
                                            txt2DBarcode.Focus();
                                            errflag22 = 1;
                                            Str_Scan_Qty = 0;
                                        }

                                        lbltotqty.Text = strArr1[3];
                                        lblbalqty.Text = strArr1[4];
                                        txt_bal_Qty.Text = strArr1[4];

                                        part_no = strArr1[1];
                                        if (Int_Bal_Qty == 0)
                                        {
                                            string Str_Output = part_no + " : " + Int_Tot_Asn_Qty + " Qty " + " Scan Already  Done  ";
                                            await DisplayAlert("Alert", Str_Output, "OK");

                                        }
                                        else
                                        {
                                            if (Str_Scan_Qty == 0)
                                            {

                                            }
                                            else
                                            {

                                                if (chkbinng.IsChecked == true)
                                                {
                                                    if (txtSBinLoc.Text.TrimEnd().Length != 0)
                                                    {
                                                        Process_Save();
                                                    }
                                                    else
                                                    {
                                                        string fl = "I";
                                                        string StrOut = BinMgt.GetBinLocation1(txtPartNo.Text, txtinvocenumber.Text, clsConnection.user_WH_ID, clsConnection.user, fl);
                                                        if (StrOut == "Invalid QR Number")
                                                        {

                                                        }
                                                        else
                                                        {

                                                            txtBinLoc.Text = StrOut.TrimEnd();

                                                            txtcart.IsEnabled = true;

                                                            if (txtqtylen.Text == "0")
                                                            {

                                                                txtQty.Focus();
                                                            }
                                                            else
                                                            {
                                                            }
                                                        }

                                                        txtSBinLoc.IsEnabled = true;
                                                        txtSBinLoc.Focus();
                                                    }
                                                }
                                                else
                                                {
                                                    Process_Save();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            txtSBinLoc.Focus();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }

        }


        public void clearForm()
        {

            lblstatusmessage.Text = string.Empty;
            txt2DBarcode.Text = string.Empty;
            txtPartNo.Text = string.Empty;
            txtQty.Text = string.Empty;
            Inique_No = string.Empty;
            textBox1.Text = string.Empty;
            radioButton1.IsChecked = false;
            txt2DBarcode.Focus();

            STflag = "";
            fl1 = "";
            barfl = "";

        }



        private void Clear_All()
        {
            txt2DBarcode.Text = string.Empty;
            txtinvocenumber.Text = string.Empty;
            txtPartNo.Text = string.Empty;
            txtQty.Text = string.Empty;
            txtBinLoc.Text = string.Empty;
            txtSBinLoc.Text = string.Empty;
            lblbalqty.Text = string.Empty;
            lbltotqty.Text = string.Empty;

            txt2DBarcode.Focus();
        }


        public void Process_Save()
        {
            try
            {

                int Int_Rtn_Code = 0;
                string Str_Part_Name = string.Empty;
                string Str_Err_Message;
                string ermsg = string.Empty;
                string str = null;
                string[] strArr = null;
                str = txt2DBarcode.Text;

                char[] splitchar = { ',' };
                strArr = str.Split(splitchar);

                string Str_Output = string.Empty;

                string qty_bin = txtQty.Text;

                Str_Output = Convert.ToString(BinMgt.Chk_Inward_Part_Save_N(txtinvocenumber.Text, lblstatusmessage.Text, Convert.ToInt32(txtQty.Text), clsConnection.user, txtPartNo.Text, clsConnection.user_WH_ID, clsConnection.HHT_Serial_Number, clsConnection.User_Mas_WH_ID, txt2DBarcode.Text, STflag, ref ermsg));

                if (k == Convert.ToInt32(txtQty.Text))
                {
                    str = Str_Output;
                    char[] splitchar1 = { ',' };
                    strArr = str.Split(splitchar1);
                    Int_Rtn_Code = Convert.ToInt32(strArr[0]);

                    if (Int_Rtn_Code == 0)
                    {
                        Str_Err_Message = strArr[1];
                        lblstatusmessage.Text = "Inward completed, Please Scan Direct Binning Part";
                        txt2DBarcode.Focus();


                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public async void After_Loc_Scan()
        {

            if (txt2DBarcode.Text.Length == 0)
            {
                await DisplayAlert("Alert", "Scan Part No", "ok");
            }
            else
            {
                Fin_Save();
            }
        }



        public async void Fin_Save()
        {
            try
            {
                if (txtBinLoc.Text.Trim().Length == 0 || txtBinLoc.Text.Trim() == "")
                {
                    string errm = string.Empty;
                    if (radioButton1.IsChecked == true)
                    {

                    }
                    else
                    { 
                        string locsave = BinMgt.Location_save_toACCPAC(txtPartNo.Text, txtSBinLoc.Text, clsConnection.user, clsConnection.user_WH_ID, Convert.ToInt32(txtQty.Text), ref errm, txtinvocenumber.Text.Trim());
                        if (locsave == "Success")
                        {
                            lblstatusmessage.Text = "Loc. Save To ACCPAC";
                        }
                    }

                    Binning_save(1);

                }
                else if (txtBinLoc.Text.TrimEnd() == "0")
                {
                    Binning_save(1);
                }
                else
                {
                    if ((txtBinLoc.Text.TrimEnd() == txtSBinLoc.Text.TrimEnd()) || (textBox1.Text.TrimEnd() == txtSBinLoc.Text.TrimEnd()))
                    {
                        Binning_save(0);
                    }
                    else
                    {
                        if (radioButton1.IsChecked == true)
                        {
                            Binning_save(0);
                        }
                        else
                        {
                            //if (clsConnection.User_Type != "Operator")
                            if (clsConnection.User_Type != "")
                            {
                                string errm = string.Empty;
                                if (radioButton1.IsChecked == true)
                                {

                                }
                                else
                                {//CHANGING THE BIN LOCATION .
                                    string locsave = BinMgt.Location_save_toACCPAC(txtPartNo.Text, txtSBinLoc.Text, clsConnection.user, clsConnection.user_WH_ID, Convert.ToInt32(txtQty.Text), ref errm, txtinvocenumber.Text.Trim());
                                    if (locsave == "Success")
                                    {
                                        lblstatusmessage.Text = "Loc. Save To ACCPAC";
                                    }
                                }

                                Binning_save(0);

                            }
                            else
                            {
                                await DisplayAlert("Alert", "Invalid Location", "OK");
                                txtSBinLoc.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
        }




        public async void Binning_save(int Int_save)
        {
            try
            {

                if (Inique_No.Length == 0)
                {
                    await DisplayAlert("Alert", "Scan Correct Barcode", "OK");

                    txtSBinLoc.Text = string.Empty;
                    txtSBinLoc.IsEnabled = false;
                    txt2DBarcode.Focus();
                }
                else
                {

                    string Str_St = string.Empty;
                    if (radioButton1.IsChecked == true)
                    {
                        if (BinMgt.saveBinningDetails2(cmbGRNNos.Text, txt2DBarcode.Text, txtPartNo.Text, Convert.ToInt32(txtQty.Text), txtBinLoc.Text.TrimEnd(), txtSBinLoc.Text.TrimEnd(), clsConnection.user, Int_save, Inique_No, txtinvocenumber.Text, clsConnection.user_WH_ID,clsConnection.User_Mas_WH_ID, ref Str_St, ref Err))
                        {
                            txt_old_partno.Text = txtPartNo.Text.TrimEnd();

                            lblstatusmessage.Text = "Binning Success: " + Str_St;
                            lblstatusmessage.IsVisible = true;
                            txtSBinLoc.Text = string.Empty;
                            txt2DBarcode.Focus();
                            string[] bal;
                            bal = Str_St.Split('-');
                            if (bal.Length > 1)
                            {
                                lblbalqty.Text = bal[2];
                            }
                            barfl = "OK";
                            clearForm();
                        }
                        else
                        {
                            await DisplayAlert("Alert", Err, "OK");
                        }
                    }
                    else if (radioButton1.IsChecked == false)
                    {
                        if (saveBinningDetails(cmbGRNNos.Text, txt2DBarcode.Text, txtPartNo.Text, Convert.ToInt32(txtQty.Text), txtBinLoc.Text, txtSBinLoc.Text, clsConnection.user, Int_save, Inique_No, txtinvocenumber.Text, clsConnection.user_WH_ID, ref Str_St, ref Err))
                        {
                            txt_old_partno.Text = txtPartNo.Text.TrimEnd();

                            lblstatusmessage.Text = "Binning Success: " + Str_St;
                            lblstatusmessage.IsVisible = true;
                            txtSBinLoc.Text = string.Empty;
                            txt2DBarcode.Focus();
                            string[] bal;
                            bal = Str_St.Split('-');
                            if (bal.Length > 1)
                            {
                                lblbalqty.Text = bal[2];
                            }
                            barfl = "OK";

                            clearForm();
                        }
                        else
                        {
                            await DisplayAlert("Alert", Err, "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");


            }
        }



        public async void load_listviewBB(DataTable dt, ListView lbo, int columncount, string orange_colorcheck)
        {
            try
            {
                lbo.ItemsSource = null;
            }
            catch
            {
            }
            if (dt.Rows.Count > 0)
            {
                List<ItemViewModel> listItems = new List<ItemViewModel>();

                for (int i = 0; i <= dt.Rows.Count - 1; i += 1)
                {
                    if (dt.Rows[i][0].ToString() != "")
                    {
                        try
                        {
                            ItemViewModel item = new ItemViewModel();

                            item.PartNo = dt.Rows[i]["PartNo"].ToString();
                            item.PartName = dt.Rows[i]["PartName"].ToString();
                            item.Qty = dt.Rows[i]["Qty"].ToString();
                            item.Bal_Qty = dt.Rows[i]["Bal_Qty"].ToString();

                            listItems.Add(item);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Alert", ex.Message, "OK");
                        }
                    }
                }

                lbo.ItemsSource = listItems;

                for (int i = 0; i <= listItems.Count - 1; i++)
                {
                    string[] orange_clrcheck = new string[4];
                    try
                    {
                        orange_clrcheck = orange_colorcheck.Split(',');
                    }
                    catch
                    {
                        orange_clrcheck[0] = orange_colorcheck;
                    }
                    try
                    {
                        var item = (ItemViewModel)listItems[i];

                        if (item.Bal_Qty == orange_clrcheck[0])
                        {     }
                    }
                    catch
                    {

                    }
                }
            }
        }


        public void cmbGRNNos_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt2DBarcode.Focus();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Clear_All();
            clearForm();
        }


        private async void btnExit_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushModalAsync(new DashBoard());

        }

        
        private async void btnpending_Clicked(object sender, EventArgs e)
        {
            List<string> invoiceNumbers = new List<string>();

            if (panel2.IsVisible == true)
            {
                panel2.IsVisible = false;
                panel1.IsVisible = true;

            }
            else if (panel2.IsVisible == false)
            {
                panel1.IsVisible = false;
                panel2.IsVisible = true;

                string errmsg = string.Empty;
                try
                {
                    DataTable dt = BinMgt.Get_InvNo(clsConnection.user_WH_ID);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            cmbinv.Items.Add(dt.Rows[i][0].ToString());
                        }

                    }

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Alert", ex.Message, "OK");
                }
            }

        }

        private void cmbinv_SelectedIndexChanged(object sender, EventArgs e)
        {

            DataTable dtt = new DataTable();
            string errm = string.Empty;

            dtt = BinMgt.get_pend_bin((string)cmbinv.SelectedItem, ref errm);

            if (dtt.Rows.Count > 0)
            {
                load_listviewBB(dtt, lstundprd, 4, "PCK_PEND");

            }
            else
            {
                //Empty
            }
        }

        private async void txt2DBarcode_Completed(object sender, EventArgs e)
        {
            try
            {
                txt2DBarcode.Text.ToString();

                string Str_Scan_Part = string.Empty;
                lblstatusmessage.Text = string.Empty;
                string Scan_Invoice_no = string.Empty;
                string Scan_PartNo = string.Empty;
                string Scan_qty = string.Empty;
                string strst = string.Empty;
                strst = "";

                if (cmbGRNNos.Text.Length == 0)
                {

                    Inique_No = string.Empty;

                    data = txt2DBarcode.Text.Split(',');


                    if (sbar == txt2DBarcode.Text.ToString())
                    {
                        if (barfl == "OK")
                        {
                            lblstatusmessage.Text = "Already Scanned Barcode";
                            txt2DBarcode.Text = string.Empty;
                            txt2DBarcode.Focus();
                            barfl = string.Empty;
                        }
                        else
                        {

                        }
                    }

                    if (txt2DBarcode.Text.Length > 9)
                    {
                        if (data.Length > 3 && chkbinng.IsChecked == false)
                        {
                            string errmrp = string.Empty;
                            string stmr = string.Empty;
                            int stcount = 0;
                            //call fun.done
                            stcount = Convert.ToInt32(BinMgt.testing1(txt2DBarcode.Text, ref errmrp));
                            if (stcount == 1)
                            {
                                lblstatusmessage.Text = "Stock Tranfer , direct Binning Process";

                                Str_Scan_Part = data[0];
                                Scan_Invoice_no = data[2];
                                Inique_No = data[3];
                                txtPartNo.Text = Str_Scan_Part;
                                txtinvocenumber.Text = Scan_Invoice_no;

                                Get_Grn_BalQty(txtinvocenumber.Text, txtPartNo.Text);
                                txtQty.Focus();

                            }
                            else
                            {
                                Str_Scan_Part = data[0];
                                Scan_Invoice_no = data[2];
                                Inique_No = data[3];
                                txtPartNo.Text = Str_Scan_Part;
                                txtinvocenumber.Text = Scan_Invoice_no;
                                txtQty.Focus();
                            }
                        }
                        else
                        {
                            if (chkbinng.IsChecked == true)
                            {

                                binflag = 1;
                                errflag22 = 0;

                                string stavalfl = string.Empty;

                                string errm = string.Empty;

                                int stcount1 = Convert.ToInt32(BinMgt.testing2(txt2DBarcode.Text, ref errm));

                                if (stcount1 == 1)
                                {
                                    lblstatusmessage.Text = "Stock Tranfer , direct Binning Process";
                                    STflag = "ST";

                                    string valqr = string.Empty;
                                    string erret = string.Empty;
                                    if (erret == "Success")
                                    {
                                        lblstatusmessage.Text = "Already Scanned Barcode";
                                        txt2DBarcode.Text = string.Empty;
                                        txt2DBarcode.Focus();
                                        stavalfl = "BR";

                                        barfl = string.Empty;
                                    }

                                }
                                else
                                {
                                    STflag = "NST";
                                }
                                if (stavalfl == "BR")
                                {
                                    stavalfl = string.Empty;
                                    txt2DBarcode.Focus();
                                }
                                else
                                {
                                    Process_Srtart();

                                    if (errflag22 == 1)
                                    {
                                        txt2DBarcode.Focus();
                                    }
                                    else
                                    {
                                        if (txt_old_partno.Text == txtPartNo.Text)
                                        {
                                            if (radioButton1.IsChecked == true)
                                            {
                                                txtSBinLoc.Text = textBox1.Text;
                                                txtSBinLoc_Completed(sender, e);
                                            }
                                            else
                                            {
                                                txtSBinLoc.Text = txtBinLoc.Text;
                                                txtSBinLoc_Completed(sender, e);
                                            }
                                        }
                                        else
                                        {
                                            if (txtSBinLoc.Text.TrimEnd().Length != 0)
                                            {
                                                chkbinng.IsChecked = false;
                                                txtSBinLoc_Completed(sender, e);
                                            }
                                            else
                                            {
                                                txtSBinLoc.IsEnabled = true;
                                                txtSBinLoc.Focus();
                                            }
                                        }
                                    }
                                }
                            }
                            else if (txtSBinLoc.Text.TrimEnd().Length == 0)
                            {
                                await DisplayAlert("Alert", "Invalid 2-D BarCode", "OK");
                                txt2DBarcode.Text = string.Empty;
                                txtSBinLoc.Text = string.Empty;
                                txtSBinLoc.IsEnabled = false;
                                txt2DBarcode.Focus();

                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Invalid Barcode", "OK");
                        txt2DBarcode.Text = string.Empty;
                        txtSBinLoc.Text = string.Empty;
                        txtSBinLoc.IsEnabled = false;
                        txt2DBarcode.Focus();

                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Select GRN No", "OK");
                    txt2DBarcode.Text = string.Empty;
                    txtSBinLoc.Text = string.Empty;
                    txtSBinLoc.IsEnabled = false;
                    txt2DBarcode.Focus();

                }

                txtBinLoc.Focus();
            }
            catch (Exception)
            {
                await Navigation.PopModalAsync();
            }

        }

        private async void txtSBinLoc_Completed(object sender, EventArgs e)
        { 
            try
            {
                txtSBinLoc.Text.ToString();

                if (chkbinng.IsChecked == true)
                {
                    string errf = string.Empty;
                    if (BinMgt.valid_bin_loc(txtSBinLoc.Text.TrimEnd(), ref errf))
                    {
                        if (txtBinLoc.Text.Length != 0 || txtBinLoc.Text == "0")
                        {
                            Process_Save();
                            sbar = txt2DBarcode.Text.ToString();

                            chkbinng.IsChecked = false;
                            fl1 = "B";
                            string StrOut = BinMgt.GetBinLocation1(txtPartNo.Text, txtinvocenumber.Text, clsConnection.user_WH_ID, clsConnection.user, fl1);
                            if (StrOut == "Invalid QR Number")
                            {

                            }
                            else
                            {
                                txt2DBarcode.Text = StrOut;
                                txtPartNo.Text = string.Empty;
                                txtinvocenumber.Text = string.Empty;
                                txt2DBarcode_Completed(sender, e);

                                if (comfl != "OK")
                                {

                                    txtcart.IsEnabled = true;
                                    if (txtqtylen.Text == "0")
                                    {

                                        if (STflag == "ST")
                                        {
                                            txtSBinLoc_Completed(sender, e);

                                        }
                                        else
                                        {
                                            if (txt_old_partno.Text == txtPartNo.Text)
                                            {
                                                txtQty.IsEnabled = true;
                                                txtQty.Focus();
                                            }
                                            else
                                            {
                                                txtSBinLoc_Completed(sender, e);

                                            }
                                        }

                                    }
                                    else
                                    {
                                        txtQty.IsEnabled = false;
                                        txtSBinLoc_Completed(sender, e);
                                    }
                                }
                                else
                                {
                                    comfl = string.Empty;
                                    chkbinng.IsChecked = true;
                                }
                                txt2DBarcode.Focus();
                            }
                        }
                        else
                        {
                            if ((txtBinLoc.Text == txtSBinLoc.Text) || (textBox1.Text == txtSBinLoc.Text) || (txtBinLoc.Text != txtSBinLoc.Text))
                            {
                                Process_Save();
                                sbar = txt2DBarcode.Text.ToString();

                                chkbinng.IsChecked = false;
                                fl1 = "B";
                                string StrOut = BinMgt.GetBinLocation1(txtPartNo.Text, txtinvocenumber.Text, clsConnection.user_WH_ID, clsConnection.user, fl1);
                                if (StrOut == "Invalid QR Number")
                                {

                                }
                                else
                                {
                                    txt2DBarcode.Text = StrOut;
                                    txtPartNo.Text = string.Empty;
                                    txtinvocenumber.Text = string.Empty;

                                    if (comfl != "OK")
                                    {

                                        txtcart.IsEnabled = true;
                                        if (txtqtylen.Text == "0")
                                        {

                                            if (STflag == "ST")
                                            {
                                                txtSBinLoc_Completed(sender, e);

                                            }
                                            else
                                            {
                                                if (txt_old_partno.Text == txtPartNo.Text)
                                                {
                                                    txtQty.IsEnabled = true;
                                                    txtQty.Focus();
                                                }
                                                else
                                                {
                                                    txtSBinLoc_Completed(sender, e);

                                                }
                                            }

                                        }
                                        else
                                        {
                                            txtQty.IsEnabled = false;
                                            txtSBinLoc_Completed(sender, e);
                                        }
                                    }
                                    else
                                    {
                                        comfl = string.Empty;
                                        chkbinng.IsChecked = true;
                                    }
                                    txt2DBarcode.Focus();
                                }
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Invalid Location ID", "OK");
                        txtSBinLoc.Text = string.Empty;
                        txtSBinLoc.Focus();
                        await Navigation.PushModalAsync(new Binning());
                    }
                }
                else
                {
                    string errf = string.Empty;
                    if (txt2DBarcode.Text.TrimEnd().Length == 0 && txtSBinLoc.Text.TrimEnd().Length == 0)
                    {
                        txt2DBarcode.Focus();

                    }
                    else
                    {
                        if (BinMgt.valid_bin_loc(txtSBinLoc.Text.TrimEnd(), ref errf))
                        {
                            After_Loc_Scan();
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Invalid Location ID", "OK");
                            txtSBinLoc.Text = string.Empty;
                            txtSBinLoc.Focus();
                        }
                        if (binflag == 1)
                        {
                            chkbinng.IsChecked = true;
                            txt2DBarcode.Focus();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        }

        private async void txtQty_Completed(object sender, EventArgs e)
        {
            try
            {
                if (txtQty.Text.Length <= 3)
                {
                    Process_Srtart();
                }
                else
                {
                    Err = "Maximum 3 Digts Only";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", Err + ex.Message, "OK");
            }


        }

        private void qtyFocbtn_Clicked(object sender, EventArgs e)
        {
            txtQty.Focus();
        }

        private async void txtinvocenumber_Completed(object sender, EventArgs e)
        {            
            if (txtinvocenumber.Text.Length != 0)
            {
                txt2DBarcode.Focus();
            }
            else
            {
                await DisplayAlert("Alert", "Invoice Number is Empty.", "OK");
                txtinvocenumber.Focus();
            }
        }




    }
}