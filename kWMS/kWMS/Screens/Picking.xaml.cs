using kWMS.Extras;
using kWMS.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kWMS.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Picking : ContentPage
    {

        PickingManagement PickMgmt = new PickingManagement();
        public Picking()
        {
            InitializeComponent();
            Frm_Picking_Load();

        }

        string err1 = string.Empty;
        string picklistdata;
        int chflag = 0;
        int pickcunt;

        private async void txtqty_KeyDown(object sender, EventArgs e)
        {
            try
            {

                if (txtqty.Text.ToString().Trim().Length != 0)
                {
                    txt_Bin_Loc.Focus();

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }
        }


        private async void txtpartno_KeyDown(object sender, EventArgs e)
        {

            try
            {

                if (txtpartno.Text.ToString().Trim().Length != 0)
                {
                    DataTableReader rd;
                    rd = PickMgmt.GetPick_Mast(txtpartno.Text, txt_Bin_Loc.Text);
                    if (rd.Read())
                    {
                        txtqty.Focus();
                    }
                    else
                    {
                        await DisplayAlert("Message", "Invalid Part Number", "OK");
                        txtpartno.Text = string.Empty;
                        txtpartno.Focus();

                    }

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }
        }

        private void rdosing_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (rdosing.IsChecked == true)
            {

                panel2.IsVisible = true;
                btscpiclst.IsVisible = true;
                listBox1.IsVisible = false;
                btlb1.IsVisible = false;
                listBox2.IsVisible = false;
                btlb2.IsVisible = false;
                rdomulti.IsChecked = false;
                lable21.IsVisible = false;
                lable24.IsVisible = false;
                cmb_pickList.IsVisible = false;
                btcbpiclst.IsVisible = false;
                cmbdealer.IsVisible = false;
                btcmdlr.IsVisible = false;

            }
            else if (rdomulti.IsChecked == true)
            {

                panel2.IsVisible = false;
                btscpiclst.IsVisible = false;
                listBox1.IsVisible = true;
                btlb1.IsVisible = true;
                rdosing.IsChecked = false;

                lable21.IsVisible = true;
                lable24.IsVisible = true;
                cmb_pickList.IsVisible = true;
                btcbpiclst.IsVisible = true;
                cmbdealer.IsVisible = true;
                btcmdlr.IsVisible = true;
                btcbpiclst.IsVisible = true;
                listBox2.IsVisible = true;
                btlb2.IsVisible = true;

            }
        }

        private void rdomulti_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

            if (rdomulti.IsChecked == true)
            {

                listBox1.IsVisible = true;
                btlb1.IsVisible = true;

                panel2.IsVisible = false;
                btscpiclst.IsVisible = false;
                rdosing.IsChecked = false;
                btcmdlr.IsVisible = true;
                listBox2.IsVisible = true;
                btlb2.IsVisible = true;

            }
            else if (rdosing.IsChecked == true)
            {
                rdomulti.IsChecked = false;

                panel2.IsVisible = true;
                btscpiclst.IsVisible = true;
                listBox1.IsVisible = false;
                btlb1.IsVisible = false;

                lable21.IsVisible = true;
                cmb_pickList.IsVisible = true;
                btcbpiclst.IsVisible = true;
                listBox2.IsVisible = false;
                btlb2.IsVisible = false;

            }

        }


        public void btnpendclear_Clicked(object sender, EventArgs e)
        {
            string clear = string.Empty;
            string fla = string.Empty;
            fla = "U";
            string pick = string.Empty;
            string bin = string.Empty;
            string er = string.Empty;

            var lbpicitms = listBox1.ItemsSource as IList;

            if (lbpicitms.Count > 0)
            {
                for (int g = 0; g < lbpicitms.Count; g++)
                {

                    pick = pick + lbl_txt_scan_picklist.Text + ",";
                }
            }

            pick = pick.Substring(0, pick.Length - 1);

            clear = PickMgmt.save_pending_pick(pick, bin, clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, fla, ref er);
            if (clear == "Success")
            {
                cmbdealer.SelectedItem = clear;
                cmb_pickList.SelectedItem = clear;
                listBox1.ItemsSource = string.Empty;
                listBox2.ItemsSource = string.Empty;
                lbl_txt_scan_picklist.Text = string.Empty;
                plnodata.Clear();
                plqtydata.Clear();

                Frm_Picking_Load();

            }
        }
        private async void button3_Clicked(object sender, EventArgs e)
        {
            string clear = string.Empty;
            string fla = string.Empty;

            string pick = string.Empty;
            string bin = string.Empty;
            string er = string.Empty;
            plnodata.Clear();
            plqtydata.Clear();
            await Navigation.PushModalAsync(new DashBoard());

        }

        private async void button5_Clicked(object sender, EventArgs e)
        {
            try
            {
                Skip_Part_No();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }
        }

        private void button4_Clicked(object sender, EventArgs e)
        {
            txt_partno_Master.Text = string.Empty;
            txt_qty_Master.Text = string.Empty;
        }

        private async void button6_Clicked(object sender, EventArgs e)
        {

            var lbpicitms = listBox1.ItemsSource as IList;

            string flag = string.Empty;

            if (rdomulti.IsChecked == true)
            {
                string retur = string.Empty;
                string picklists = string.Empty;
                string err = string.Empty;
                string fla = string.Empty;
                fla = "S";
                for (int i = 0; i < lbpicitms.Count; i++)
                {
                    if (i == 0)
                    {
                        picklists = lbl_txt_scan_picklist.Text;
                    }
                    else
                    {
                        picklists = picklists + "," + lbl_txt_scan_picklist.Text;
                    }
                }

                retur = PickMgmt.save_pending_pick(picklists, txtbind.Text, clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, fla, ref err);
                if (retur == "Success")
                {

                }

                pnl_pick.IsVisible = true;
                trns_pick.IsVisible = false;
            }
            else
            {
                await Navigation.PushModalAsync(new Picking());
            }

        }


        private async void Process_Save()
        {

            try
            {

                if (vildate_Txt())
                {
                    string Err = string.Empty;

                    string Str_WH_ID = clsConnection.user_WH_ID;

                    if (PickMgmt.Save_PickList(Txt_pick_List_Number.Text, txtpartno.Text, Convert.ToInt32(txtqty.Text), txt_Bin_Loc_Master.Text, txt_Bin_Loc.Text, clsConnection.user, lblUnique_id.Text, txt_Scan_Barcode.Text, Str_WH_ID, clsConnection.User_Mas_WH_ID, ref Err))
                    {

                        txt_old_Part.Text = txtpartno.Text;
                        txt_Bin_Loc_old.Text = txt_Bin_Loc.Text;

                        txt_Scan_Barcode.Text = string.Empty;
                        txtqty.Text = string.Empty;
                        txtpartno.Text = string.Empty;
                        lblUnique_id.Text = string.Empty;

                        txt_Bin_Loc_Master.Text = string.Empty;
                        txt_partno_Master.Text = string.Empty;
                        txt_qty_Master.Text = string.Empty;
                        txt_Bin_Loc.Text = string.Empty;

                        txt_Tot_Bal_Qty.Text = string.Empty;
                        txt_Tot_Qty.Text = string.Empty;
                        chksplit.IsChecked = false;

                        Select_Next_Part();

                    }
                    else
                    {
                        await DisplayAlert("Alert", Err, "OK");
                    }

                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }

        }

        bool vildate_Check_Already_Picking()
        {

            try
            {

                bool vaild = false;

                string Err = string.Empty;

                if (PickMgmt.Validate_Picking_Done_Chk(Txt_pick_List_Number.Text, txt_partno_Master.Text.Trim(), Convert.ToInt32(txt_qty_Master.Text), lblUnique_id.Text, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, ref Err))
                {

                    vaild = false;
                    DisplayAlert("Message", "Already Picking Done", "OK");

                    txt_Scan_Barcode.Text = string.Empty;
                    txt_Scan_Barcode.Focus();
                    txt_Bin_Loc.Text = string.Empty;

                }
                else
                {

                    vaild = true;
                }

                return vaild;

            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
                return false;

            }

        }
        bool vildate_Txt()
        {

            try
            {

                bool vaild = false;

                string PART = txt_partno_Master.Text.TrimEnd();
                if (txt_partno_Master.Text.TrimEnd() != txtpartno.Text.TrimEnd())
                {
                    vaild = false;
                    DisplayAlert("Message", "Part No Mismatch", "OK");
                    txt_Scan_Barcode.Text = string.Empty;
                    txt_Scan_Barcode.Focus();

                }
                else if (Convert.ToInt32(txt_part_Bal_Qty.Text) < Convert.ToInt32(txtqty.Text))
                {
                    vaild = false;
                    DisplayAlert("Message", "Scan Qty Greater Than Pick List Qty", "OK");
                    txt_Scan_Barcode.Focus();
                    txt_Scan_Barcode.Text = string.Empty;

                    txtqty.Text = txt_part_Bal_Qty.Text;

                }
                else
                {
                    vaild = true;
                }

                return vaild;

            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");

                return false;

            }

        }




        public async void Select_Next_Part()
        {

            try
            {
                var lbpicitms = listBox1.ItemsSource as IList;

                DataTable dtreceive = new DataTable();

                string ermsg = string.Empty;
                string Str_userName = clsConnection.user;
                string Str_PickList_No = string.Empty;
                int Int_Total_Row_Count = 0;
                int Int_Total_Qty = 0;
                int Int_Recv_Qty = 0;
                int Int_Bal_Qty = 0;
                int Int_Skip_Status = 0;
                string Str_Next_Part_No = string.Empty;
                string Str_Next_Location = string.Empty;
                int Int_Next_Qty = 0;
                string Str_Tot_Line = string.Empty;
                string Str_Rec_Item = string.Empty;
                string Str_Bal_Item = string.Empty;
                string str_rec = string.Empty;
                string parnam = string.Empty;

                if (cmb_pickList.Items.ToString().Length != 0 && rdomulti.IsChecked == true)
                {
                    if (lbpicitms.Count > 0)
                    {

                        pickcunt = lbpicitms.Count;

                        str_rec = PickMgmt.download_PickList_Using_No1(picklistdata, Str_userName, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, txt_partno_Master.Text.Trim(), ref Str_Next_Location, ref Str_Tot_Line, ref Str_Rec_Item, ref Str_Bal_Item, ref parnam, ref ermsg);

                        if (ermsg != "Success")
                        {

                        }
                    }
                }
                else
                {

                    str_rec = PickMgmt.download_PickList_Using_No(Txt_pick_List_Number.Text, Str_userName, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, txt_partno_Master.Text.Trim(), ref Str_Next_Location, ref Str_Tot_Line, ref Str_Rec_Item, ref Str_Bal_Item, ref parnam, ref ermsg);
                }
                if (ermsg == "Success")
                {

                    if (Convert.ToInt32(Str_Tot_Line) > 0)
                    {
                        txt_Bin_Loc_Master.Text = string.Empty;
                        txt_partno_Master.Text = string.Empty;
                        txt_qty_Master.Text = string.Empty;

                        Int_Total_Row_Count = Convert.ToInt32(Str_Tot_Line);
                        Int_Total_Qty = Convert.ToInt32(Str_Tot_Line);
                        Int_Recv_Qty = Convert.ToInt32(Str_Rec_Item);
                        Int_Bal_Qty = Convert.ToInt32(Str_Bal_Item);

                        string[] data;
                        data = Str_Next_Location.Split(',');
                        Str_Next_Part_No = data[0];

                        lblpartname.Text = parnam;

                        Int_Next_Qty = Convert.ToInt32(data[2]);

                        Str_Next_Location = data[3];
                        Int_Skip_Status = Convert.ToInt32(data[4]);

                        txt_partno_Master.Text = Str_Next_Part_No;
                        txt_Bin_Loc_Master.Text = Str_Next_Location;

                        txt_qty_Master.Text = Convert.ToString(Int_Next_Qty);

                        txt_part_Bal_Qty.Text = data[6];
                        string buffloc = string.Empty;

                        buffloc = PickMgmt.getbinnn4Pick(txt_partno_Master.Text.Trim());
                        txtbuff.Text = buffloc;
                        txt_Tot_Qty.Text = Convert.ToString(Int_Total_Qty);
                        txt_Tot_Bal_Qty.Text = Convert.ToString(Int_Bal_Qty);

                        if (txt_part_Bal_Qty.Text == "0")
                        {
                            if (chflag == 1)
                            {

                            }
                            else
                            {
                                Select_Next_Part();
                                chflag = 1;
                            }
                        }
                        txtpartno.IsEnabled = false;
                        txtqty.IsEnabled = true;
                        txt_Scan_Barcode.Focus();
                        txt_Scan_Barcode.Text = string.Empty;

                        if (rdomulti.IsChecked == true)
                        {
                            Txt_pick_List_Number.Text = data[7];
                            txtdealer.Text = data[8];

                            for (int j = 0; j < lbpicitms.Count; j++)
                            {
                                if (lbpicitms[j].ToString() != Txt_pick_List_Number.Text)
                                {
                                    txtbind.Text = Convert.ToString(j + 1);
                                }
                            }
                        }
                        else
                        {
                            txtdealer.Text = data[7];
                            txtbind.Text = "1";
                        }
                    }
                    else
                    {
                        txt_Bin_Loc_Master.Text = string.Empty;
                        txt_partno_Master.Text = string.Empty;
                        txt_qty_Master.Text = string.Empty;
                        txt_Bin_Loc.Text = string.Empty;

                        await Navigation.PopAsync();
                        await DisplayAlert("Alert", "No Picklist Made For this User ", "OK");
                    }
                }
                else
                {

                    if (Str_Tot_Line != "0")
                    {

                        bool res = false;
                        res = PickMgmt.Ins_Picking(Txt_pick_List_Number.Text.ToString(), 0);
                        if (res != false)
                        {
                            await DisplayAlert("Alert", "Picklist Completed ", "OK");
                            await Navigation.PopAsync();
                        }

                    }
                    else
                    {
                        await DisplayAlert("Alert", "No Record Found", "OK");
                        await Navigation.PopModalAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
        }


        private async void Skip_Part_No()
        {

            try
            {

                Process_Update();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }

        }
        private async void Process_Update()
        {
            try
            {

                string Err = string.Empty;

                string Str_WH_ID = clsConnection.user_WH_ID;

                if (PickMgmt.Update_PickList_Skip(Txt_pick_List_Number.Text, txt_partno_Master.Text.Trim(), clsConnection.user, Str_WH_ID, clsConnection.User_Mas_WH_ID, ref Err))
                {

                    txt_Scan_Barcode.Text = string.Empty;
                    txtqty.Text = string.Empty;
                    txtpartno.Text = string.Empty;
                    lblUnique_id.Text = string.Empty;

                    Select_Next_Part();

                }
                else
                {
                    await DisplayAlert("Alert", Err, "OK");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }

        }

        public class picklistitems
        {
            public string PICKLIST_NO { get; set; }
        }
        public ObservableCollection<string> plnodata = new ObservableCollection<string>();
        public class picklistcount
        {
            public int QTY { get; set; }
        }
        public ObservableCollection<string> plqtydata = new ObservableCollection<string>();


        private void cmb_pickList_SelectedIndexChanged_1(object sender, EventArgs e)
        {

            var lbpicitms = listBox1.ItemsSource;

            var lbpicitms2 = listBox2.ItemsSource;

            string mast = clsConnection.User_Mas_WH_ID;
            string Str_WH_Id = clsConnection.user_WH_ID;

            try
            {
                DataTable dt = new DataTable();

                dt = PickMgmt.Get_PicklstNo(Str_UserName, mast, cmbdealer.SelectedItem.ToString(), cmb_pickList.SelectedItem.ToString());
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][0].ToString() != "")
                        {
                            try
                            {
                                string dta = dt.Rows[i]["PICKLIST_NO"].ToString();
                                plnodata.Add(dta);
                            }
                            catch
                            {

                            }

                        }

                    }
                    try
                    {
                        string QTY = Convert.ToString(dt.Rows.Count);
                        plqtydata.Add(QTY);
                    }
                    catch
                    {

                    }
                    listBox1.ItemsSource = plnodata;
                    listBox2.ItemsSource = plqtydata;

                }
            }
            catch (Exception ex)
            {
                err1 = ex.ToString();
                throw;
            }

        }

        private async void cmbdealer_SelectedIndexChanged(object sender, EventArgs e)
        {

            var lbpicitms = listBox1.ItemsSource as IList;

            if (cmbdealer.Items.ToString().Length != 0)
            {

                cmb_pickList.Items.Clear();
                cmb_pickList.ItemsSource = null;

                DataTable dtpic = new DataTable();
                string ermsg = string.Empty;
                string Str_Next_Location = string.Empty;

                dtpic = PickMgmt.download_PickList_Pic(cmbdealer.SelectedItem.ToString(), clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, ref Str_Next_Location, ref ermsg);
                if (ermsg == "Success")
                {
                    if (dtpic.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtpic.Rows.Count; i++)
                        {
                            cmb_pickList.Items.Add(dtpic.Rows[i]["PICKLIST_NO"].ToString());
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Error to Load Picklist_No", "OK");

                    }
                }
            }
            else if (cmbdealer.Items.ToString().Length == 0)
            {

                DataTable dtpi = new DataTable();
                string ermsg = string.Empty;
                string st_user = clsConnection.user.ToString();
                string Str_Next_Location = string.Empty;
                cmb_pickList.Items.Clear();

                dtpi = PickMgmt.download_PickList_Only(st_user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, ref Str_Next_Location, ref ermsg);
                if (ermsg == "Success")
                {
                    if (dtpi.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtpi.Rows.Count; i++)
                        {
                            cmb_pickList.Items.Add(dtpi.Rows[i]["PICKLIST_NO"].ToString());

                            pnl_pick.IsVisible = true;
                            trns_pick.IsVisible = false;
                            txt_scan_Picklist.Text = string.Empty;

                        }
                    }
                    else
                    {
                        await DisplayAlert("Message", "No Picklist Made For this User ", "OK");
                    }
                }
            }
            else
            {
                if (cmbdealer.Items.ToString().Length != 0 && cmb_pickList.Items.ToString().Length != 0)
                {
                    if (rdosing.IsChecked == true)
                    {
                        txt_scan_Picklist.Text = string.Empty;
                    }
                    else if (rdomulti.IsChecked == true)
                    {
                        if (lbpicitms.Contains(cmb_pickList.Items))
                        {

                        }
                        else
                        {
                            lbpicitms.Add(cmb_pickList.Items.ToString());
                        }

                    }
                }
            }
        }

        public async void Frm_Picking_Load()
        {

            var lbpicitms = listBox1.ItemsSource as IList<string> ?? new List<string>();

            var lbpicitms2 = listBox2.ItemsSource as IList<string> ?? new List<string>();

            try
            {

                DataTable dtreceive = new DataTable();

                string ermsg = string.Empty;
                string Str_userName = clsConnection.user;

                chksplit.IsChecked = false;

                string Str_PickList_No = string.Empty;
                string Str_Next_Part_No = string.Empty;
                string Str_Next_Location = string.Empty;

                rdosing.IsChecked = true;

                dtreceive = PickMgmt.download_PickList_Only(Str_userName, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, ref Str_Next_Location, ref ermsg);
                if (ermsg == "Success")
                {
                    if (dtreceive.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtreceive.Rows.Count; i++)
                        {
                            cmb_pickList.Items.Add(dtreceive.Rows[i]["PICKLIST_NO"].ToString());
                            cmbdealer.Items.Add(dtreceive.Rows[i]["DEALER_CODE"].ToString());
                            pnl_pick.IsVisible = true;

                            txt_scan_Picklist.Text = string.Empty;
                            txt_scan_Picklist.Focus();
                        }
                    }
                    else
                    {

                        await Navigation.PopAsync();
                        await DisplayAlert("Alert", "No Picklist Made For this User ", "OK");
                    }
                }
                else
                {

                    await Navigation.PopAsync();

                    await DisplayAlert("Alert", "No Picklist Made For this User ", "OK");

                }

                string[] data;
                string inv = string.Empty;
                string grn = string.Empty;

                string strpick = PickMgmt.getpendpick(clsConnection.user, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID);
                if (strpick == "")
                {

                }
                else
                {
                    data = strpick.Split(',');
                    string data1 = data[0];

                    if (data.Length > 0)
                    {
                        for (int j = 0; j < data.Length; j++)
                        {

                            lbpicitms.Add(data[j]);
                            lbpicitms2.Add(Convert.ToString(lbpicitms.Count));
                        }
                        rdomulti.IsChecked = true;
                    }

                }

                rdosing.IsChecked = true;
                txt_scan_Picklist.Focus();
                string ret = string.Empty;
                string fla = string.Empty;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");

            }

        }

        string Str_UserName = clsConnection.user;

        public void btn_ok_Clicked(object sender, EventArgs e)
        {
            txt_scan_Picklist_Completed(sender, e);
        }


        private async void txt_scan_Picklist_Completed(object sender, EventArgs e)
        {

            var lbpicitms = listBox1.ItemsSource as IList;

            try
            {

                if (rdomulti.IsChecked == true)
                {
                    txt_scan_Picklist.Text = plnodata[0].ToString();

                }
                if (txt_scan_Picklist.Text.Length != 0 && rdomulti.IsChecked == false)
                {
                    if (lbpicitms == null || lbpicitms.Count == 0)
                    {
                        Txt_pick_List_Number.Text = txt_scan_Picklist.Text;
                        Select_Next_Part();
                        pnl_pick.IsVisible = false;
                        trns_pick.IsVisible = true;

                    }
                    else
                    {
                        if (lbpicitms.Contains(txt_scan_Picklist.Text.ToString()))
                        {

                            await DisplayAlert("Alert", "This Picklist Already Open in Multiple Picklist,Please Complete multiple Picklist", "OK");
                            txt_scan_Picklist.Text = string.Empty;

                        }
                        else
                        {
                            Txt_pick_List_Number.Text = txt_scan_Picklist.Text;
                            Select_Next_Part();
                            pnl_pick.IsVisible = false;
                            trns_pick.IsVisible = true;

                        }
                    }
                }
                else if (cmb_pickList.SelectedItem.ToString().Length != 0 && rdosing.IsChecked == true)
                {

                    if (lbpicitms == null || lbpicitms.Count == 0)
                    {
                        Txt_pick_List_Number.Text = cmb_pickList.SelectedItem.ToString();
                        Select_Next_Part();
                        pnl_pick.IsVisible = false;
                        trns_pick.IsVisible = true;
                        panel2.IsVisible = false;

                    }
                    else
                    {
                        if (lbpicitms.Contains(cmb_pickList.SelectedItem))
                        {

                            await DisplayAlert("Message", "This Picklist Already Open in Multiple Picklist,Please Complete multiple Picklist", "OK");
                            cmb_pickList.SelectedItem = string.Empty;

                        }
                        else
                        {
                            Txt_pick_List_Number.Text = cmb_pickList.SelectedItem.ToString();
                            Select_Next_Part();
                            pnl_pick.IsVisible = false;
                            trns_pick.IsVisible = true;

                        }
                    }

                }
                else if (cmb_pickList.SelectedItem.ToString().Length != 0 && rdomulti.IsChecked == true)
                {

                    if (lbpicitms.Count > 1)
                    {
                        for (int i = 0; i < lbpicitms.Count; i++)
                        {
                            if (i == 0)
                            {
                                Txt_pick_List_Number.Text = lbl_txt_scan_picklist.ToString();
                                picklistdata = plnodata[i].ToString();

                            }
                            else
                            {
                                Txt_pick_List_Number.Text = Txt_pick_List_Number.Text + "," + plnodata[i].ToString();
                                picklistdata = picklistdata + "," + plnodata[i].ToString();

                                if (i == 0)
                                {
                                    picklistdata = plnodata[i].ToString();

                                }
                                else if (i == lbpicitms.Count - 1)
                                {
                                    Select_Next_Part();
                                    pnl_pick.IsVisible = false;
                                    trns_pick.IsVisible = true;
                                }

                            }
                        }
                    }
                    else if (lbpicitms.Count == 1)
                    {

                        Txt_pick_List_Number.Text = lbl_txt_scan_picklist.Text;
                        picklistdata = lbl_txt_scan_picklist.Text;
                        Select_Next_Part();
                        pnl_pick.IsVisible = false;
                        trns_pick.IsVisible = true;
                    }
                }

            }
            catch (Exception ex)
            {

                await DisplayAlert("Alert", ex.Message, "OK");
            }

            pnl_pick.IsVisible = false;
            panel2.IsVisible = false;
        }

        private async void txt_Scan_Barcode_Completed(object sender, EventArgs e)
        {
            try
            {

                txt_Scan_Barcode.Text.ToString();

                if (txt_Scan_Barcode.Text.Length != 0)
                {
                    string[] data;
                    string getqr = string.Empty;
                    string err = string.Empty;
                    string qrc = string.Empty;

                    data = txt_Scan_Barcode.Text.Split(',');

                    if (data.Length > 2)
                    {
                        txtpartno.Text = data[0];
                        txtqty.Text = (data[1]);

                        int stcount1 = 0;
                        string errm = string.Empty;

                        stcount1 = Convert.ToInt32(PickMgmt.testing2(txt_Scan_Barcode.Text, ref errm));
                        if (stcount1 == 1)
                        {
                            lblUnique_id.Text = data[2];
                        }
                        else
                        {
                            lblUnique_id.Text = data[3];
                        }
                        if (Convert.ToInt32(txtqty.Text) <= Convert.ToInt32(txt_part_Bal_Qty.Text))
                        {

                            if (vildate_Check_Already_Picking())
                            {

                                if (vildate_Txt())
                                {

                                    if (txt_old_Part.Text == txtpartno.Text)
                                    {
                                        txt_Bin_Loc.Text = txt_Bin_Loc_old.Text;

                                        if (txt_Bin_Loc.Text.Length != 0)
                                        {
                                            if (chksplit.IsChecked == true)
                                            {
                                                string err1 = string.Empty;
                                                string reterr = string.Empty;
                                                int qty = Convert.ToInt32(txtqty.Text);
                                                string uni = string.Empty;

                                                reterr = PickMgmt.save_split_pick(txt_scan_Picklist.Text, txt_Bin_Loc.Text, txtpartno.Text, qty, clsConnection.user, clsConnection.HHT_Serial_Number, txtdealer.Text, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, uni, txtbind.Text, ref err1);

                                                if (reterr == "Success")
                                                {
                                                    label15.Text = "Split Completed";
                                                }
                                            }

                                            Process_Save();
                                        }
                                        else
                                        {

                                            await DisplayAlert("Message", "Scan Bin Location", "OK");
                                            txt_Bin_Loc.Focus();
                                        }

                                    }
                                    else
                                    {
                                        txt_Bin_Loc.Text = string.Empty;
                                        txt_Bin_Loc.Focus();
                                    }

                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                            await DisplayAlert("Message", "Qty Mismatch", "OK");

                        }

                    }
                    else
                    {
                        lblUnique_id.Text = "NA";

                        string valid_part_value = string.Empty;
                        string err4 = string.Empty;
                        string str_ack = string.Empty;

                        if (txt_Scan_Barcode.Text.Contains('/'))
                        {

                            str_ack = PickMgmt.get_valid_part(txt_Scan_Barcode.Text, ref valid_part_value, ref err4);
                            if (err == "Success")
                            {

                                txtpartno.Text = valid_part_value;
                            }
                            else
                            {

                                await DisplayAlert("Message", err.ToString(), "OK");
                                txt_Scan_Barcode.Text = string.Empty;
                                txt_Scan_Barcode.Focus();
                            }
                        }
                        else if (txt_Scan_Barcode.Text.Length > 34 && txt_Scan_Barcode.Text.Length <= 38)
                        {
                            txtpartno.Text = txt_Scan_Barcode.Text.Substring(0, 5) + "-" + txt_Scan_Barcode.Text.Substring(5, 5);
                        }
                        else
                        {
                            str_ack = PickMgmt.get_valid_part(txt_Scan_Barcode.Text, ref valid_part_value, ref err);
                            if (err == "Success")
                            {
                                txtpartno.Text = valid_part_value;
                            }
                            else
                            {

                                await DisplayAlert("Message", err.ToString(), "OK");
                                txt_Scan_Barcode.Text = string.Empty;
                                txt_Scan_Barcode.Focus();
                            }
                        }

                        txtqty.Text = "1";

                        if (txt_old_Part.Text == txtpartno.Text)
                        {
                            txt_Bin_Loc.Text = txt_Bin_Loc_old.Text;

                            if (txt_Bin_Loc.Text.Length != 0)
                            {

                                Process_Save();
                            }
                            else
                            {

                                await DisplayAlert("Message", "Scan Bin Location", "OK");
                                txt_Bin_Loc.Focus();

                            }

                        }
                        else
                        {
                            txt_Bin_Loc.Text = string.Empty;
                            txt_Bin_Loc.Focus();
                        }
                    }

                }
                else
                {

                    await DisplayAlert("Message", "Scan QR Barcode", "OK");
                    txt_Scan_Barcode.Text = string.Empty;
                    txt_Scan_Barcode.Focus();
                }

            }
            catch (Exception ex)
            {

                await DisplayAlert("Message", ex.Message, "OK");

            }

        }

        private async void txt_Bin_Loc_Completed(object sender, EventArgs e)
        {

            txt_Bin_Loc.Text.ToString();
            try
            {
                if (txt_Bin_Loc.Text.Length != 0)
                {
                    string errf = string.Empty;

                    if (PickMgmt.valid_bin_loc(txt_Bin_Loc.Text, ref errf))
                    {

                        if (chksplit.IsChecked == true)
                        {
                            string err = string.Empty;
                            string reterr = string.Empty;
                            int qty = Convert.ToInt32(txtqty.Text);
                            string uni = string.Empty;

                            reterr = PickMgmt.save_split_pick(txt_scan_Picklist.Text, txt_Bin_Loc.Text, txtpartno.Text, qty, clsConnection.user, clsConnection.HHT_Serial_Number, txtdealer.Text, clsConnection.user_WH_ID, clsConnection.User_Mas_WH_ID, uni, txtbind.Text, ref err);

                            if (reterr == "Success")
                            {
                                label15.Text = "Split Completed";
                            }
                        }
                        Process_Save();

                    }
                    else
                    {

                        await DisplayAlert("Message", "Invalid Location ID", "OK");
                        txt_Bin_Loc.Text = string.Empty;
                        txt_Bin_Loc.Focus();

                    }
                }
                else
                {

                    await DisplayAlert("Message", "Scan Bin Location", "OK");
                    txt_Bin_Loc.Focus();

                }

            }
            catch (Exception ex)
            {

                await DisplayAlert("Alert", ex.Message, "OK");

            }


        }

        private void txtqty_Completed(object sender, EventArgs e)
        {
            txt_Bin_Loc.Focus();
        }
    }
}