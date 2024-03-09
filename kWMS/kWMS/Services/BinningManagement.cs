using System.Data;
using System;
using kWMS.Extras;
using static kWMS.Screens.Binning;
using System.Linq;
using System.Collections.Generic;

namespace kWMS.Services
{
    public class BinningManagement
    {
        Executer Exec = new Executer();
        public BinningManagement()
        {

        }

        public string testing1(string Str_invoice_no, ref string errmr)
        {
            string Str_Final_Chk_Count = string.Empty;
            string strsql;
            try
            {
                strsql = "Select * from INWARD_RECEIPT where INWARD_QR_CODE ='" + Str_invoice_no + "' and INWARD_PRINT_STATUS='1'";
                DataTableReader strrs = Exec.GetDtReader(strsql);
                errmr = "Success";
                if (strrs.Read())
                {
                    Str_Final_Chk_Count = "1";
                    GlobalVariablesStatus.BinFlag = "C";

                }
                else
                {
                    strsql = "";
                    strsql = "Select * from INWARD_RECEIPT where MasterQRcode ='" + Str_invoice_no + "' and INWARD_PRINT_STATUS='1'";
                    strrs = Exec.GetDtReader(strsql);
                    errmr = "Master_Success";
                    if (strrs.Read())
                    {
                        Str_Final_Chk_Count = "1";
                        GlobalVariablesStatus.BinFlag = "M";

                    }

                    else
                    {

                        Str_Final_Chk_Count = "0";
                        GlobalVariablesStatus.BinFlag = "";
                        errmr = "Invalid QR Number";

                    }
                }
            }
            catch (Exception ex)
            {
                errmr = "Not OK" + ex.ToString();
            }
            return Str_Final_Chk_Count;

        }
        public string testing2(string Str_invoice_no, ref string errmr)
        {
            DataTable dtsen = new DataTable();

            string Str_Final_Chk_Count = string.Empty;
            try
            {
                string strsql = "Select * from INWARD_RECEIPT where INWARD_QR_CODE ='" + Str_invoice_no + "' and INWARD_PRINT_STATUS='1'";
                dtsen = Exec.GetDataTable(strsql);
                errmr = "Success";
                if (dtsen.Rows.Count > 0)
                {
                    Str_Final_Chk_Count = "1";

                }
                else
                {

                    Str_Final_Chk_Count = "0";
                    errmr = "Invalid QR Number";
                }
            }
            catch (Exception ex)
            {
                errmr = "Not OK" + ex.ToString();
            }
            return Str_Final_Chk_Count;

        }

        public bool Validate_Binning_GRN(string Str_Invoice_No, string Str_Part_No, int Int_qty, string Str_WH_Id, string Mast_Wh_Loc, ref string Err)
        {
            DataTable dts = new DataTable();

            try
            {
                string sql = "select * from GRNMaster where InvoiceNumber LIKE '%" + Str_Invoice_No.TrimEnd() + "%' and PartNo='" + Str_Part_No.TrimEnd() + "' and master_wh_loc_id='" + Mast_Wh_Loc + "'";
                dts = Exec.GetDataTable(sql);
                if (dts.Rows.Count > 0)
                {
                    Err = "OK";
                    return true;
                }
                else
                {
                    Err = "GRN Not Created";
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool valid_bin_loc(string locat, ref string Err)
        {
            DataTable dtr = new DataTable();
            try
            {
                string sql = "select * from wh_location_master where location_id='" + locat + "'";
                dtr = Exec.GetDataTable(sql);
                if (dtr.Rows.Count > 0)
                {
                    Err = "OK";
                    return true;

                }
                else
                {
                    Err = "Invalid Bin Location";
                    return false;

                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetBinLocation1(string partNo, string invoice_No, string Str_WH_Id, string user, string fl)
        {
            DataTable dty = new DataTable();
            try
            {
                string strOutput = string.Empty;
                string sql = "";
                if (fl == "B")
                {
                    sql = "SELECT inward_QR_code FROM INWARD_RECEIPT where VENDOR_INVOICE_NO='" + invoice_No + "' and part_number='" + partNo + "' and inward_by='" + user + "' order by inward_date desc ";
                }
                else
                {
                    sql = "SELECT Isnull(bin_location,'') FROM INWARD_RECEIPT_asn where VENDOR_INVOICE_NO='" + invoice_No + "' and part_number='" + partNo + "'";
                }
                dty = Exec.GetDataTable(sql);
                if (dty.Rows.Count > 0)
                {
                    strOutput = dty.Rows[0][0].ToString();
                }
                else
                {
                    return strOutput = "Invalid QR Number";
                }
                return strOutput;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool Validate_Binning_Done_Chk(string Str_Invoice_No, string Str_Part_No, int Int_qty, string Inique_No, string Str_WH_Id, ref string Err)
        {
            try
            {
                string mast = clsConnection.User_Mas_WH_ID;
                string bQtyFlag = string.Empty;

                if (GlobalVariablesStatus.BinFlag == "M")
                {
                    string sql1 = "select SUM(QTY) as sQTY from INWARD_RECEIPT where VENDOR_INVOICE_NO='" + Str_Invoice_No.TrimEnd() + "' and PART_NUMBER = '" + Str_Part_No + "'  and BINNING_STATUS = 0 and MASTER_WH_LOC_ID = '" + mast + "' AND INWARD_DATE >= DATEADD(DAY, -100, GETDATE())";
                    DataTableReader temrd = Exec.GetDtReader(sql1);
                    if (temrd.Read())
                    {
                        Int32 bQty = temrd.GetInt32(0);
                        Int32 sQty = Int_qty;

                        if (bQty >= sQty)
                        {
                            bQtyFlag = "Y";
                        }
                        else
                        {
                            bQtyFlag = "NA";
                        }
                    }
                    else
                    {
                        Int32 bQty = 0;
                        if (bQty == 0)
                        {
                            bQtyFlag = "";
                        }
                    }
                    temrd.Close();
                }
                else if (GlobalVariablesStatus.BinFlag == "C")
                {
                    bQtyFlag = "N";

                }
                string sql = "";
                if (GlobalVariablesStatus.BinFlag == "M" && bQtyFlag == "Y")
                {
                    sql = "select * from INWARD_RECEIPT_Master where VENDOR_INVOICE_NO='" + Str_Invoice_No.TrimEnd() + "' and PART_NUMBER='" + Str_Part_No + "' and BINNING_STATUS=0 and  MASTER_WH_LOC_ID='" + mast + "' AND INWARD_DATE >= DATEADD(DAY, -100, GETDATE())";

                }
                else if (GlobalVariablesStatus.BinFlag == "C" && bQtyFlag == "N")
                {
                    sql = "select * from INWARD_RECEIPT where VENDOR_INVOICE_NO='" + Str_Invoice_No.TrimEnd() + "' and PART_NUMBER='" + Str_Part_No + "'  and  BINNING_STATUS=0 and  MASTER_WH_LOC_ID='" + mast + "' AND INWARD_DATE >= DATEADD(DAY, -100, GETDATE())";

                }
                else if (GlobalVariablesStatus.BinFlag == "M" && bQtyFlag == "NA")
                {
                    Err = "Entered Qty Is Greater Than Balance Qty";
                    return false;
                }
                DataTableReader rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    Err = "OK";
                    return true;
                }
                else
                {
                    Err = "Binning Already Done";
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Validate_Binning_GRN_Avl_Qty(string Str_Invoice_No, string Str_Part_No, int Int_qty, string Str_WH_Id, string Mast_Wh_Loc, ref int Int_Tot_GRN_Qty, ref int Int_Bal_GRN_Qty, ref int Int_Rec_GRN_Qty, ref string Err)
        {
            try
            {
                Int_Tot_GRN_Qty = 0;
                Int_Bal_GRN_Qty = 0;
                Int_Rec_GRN_Qty = 0;

                string sql = "select Qty,REC_QTY,BAL_QTY from GRNMaster where InvoiceNumber LIKE '%" + Str_Invoice_No + "%' and PartNo='" + Str_Part_No + "' and  master_wh_loc_id='" + Mast_Wh_Loc + "'"; //whlocation='" + Str_WH_Id + "'";

                DataTableReader rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    Int_Tot_GRN_Qty = Convert.ToInt32(rd.GetValue(0));
                    Int_Bal_GRN_Qty = Convert.ToInt32(rd.GetValue(2));
                    Int_Rec_GRN_Qty = Convert.ToInt32(rd.GetValue(1));

                    if (Int_Bal_GRN_Qty > Int_qty)
                    {
                        Err = "Binning Qty - " + Convert.ToString(Int_qty) + "Should Not more than GRN Bal Qty - " + Convert.ToString(Int_Bal_GRN_Qty) + "GRN Qty - " + Int_Tot_GRN_Qty;
                        return true;
                    }
                    else
                    {
                        Err = "OK";
                        return true;
                    }


                }
                else
                {
                    rd.Close();
                    Err = "GRN Not Created";
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        //Check this 
        public bool DBGetBalQty(string strInvNo, string strPrtno, string strmastWhLoc, string BalQty)
        {
            bool stats = false;
            try
            {

                string qry = "select BAL_QTY  from INWARD_RECEIPT_ASN WHERE [VENDOR_INVOICE_NO]='" + strInvNo + "' and  [PART_NUMBER]='" + strPrtno + "'  AND MASTER_WH_LOC_ID='" + strmastWhLoc + "'";
                DataTableReader rdf = Exec.GetDtReader(qry);
                if (rdf.Read())
                {

                    //txt_bal_Qty.Text = rdf.GetValue(0).ToString();
                    BalQty = rdf.GetValue(0).ToString();
                    stats = true;
                    rdf.Close();
                }
                return stats;

            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return false;
            }

        }
        //Check this 

        public bool Validate_Binning_Inward(string Str_Invoice_No, string Str_Part_No, int Int_qty, string Inique_No, string whid, string Mast_Wh_Loc, ref string Err)
        {
            try
            {
                string qrys = "";

                if (GlobalVariablesStatus.BinFlag == "M")
                {
                    qrys = "SELECT * FROM INWARD_RECEIPT_Master WHERE VENDOR_INVOICE_NO='" + Str_Invoice_No + "' AND PART_NUMBER='" + Str_Part_No + "'  AND MASTER_WH_LOC_ID='" + Mast_Wh_Loc + "' AND INWARD_DATE >= DATEADD(DAY, -100, GETDATE())";

                }
                else if (GlobalVariablesStatus.BinFlag == "C")
                {
                    qrys = "SELECT * FROM INWARD_RECEIPT WHERE VENDOR_INVOICE_NO='" + Str_Invoice_No + "' AND PART_NUMBER='" + Str_Part_No + "' AND MASTER_WH_LOC_ID='" + Mast_Wh_Loc + "' AND INWARD_DATE >= DATEADD(DAY, -100, GETDATE())";

                }
                DataTableReader rd = Exec.GetDtReader(qrys);
                if (rd.Read())
                {
                    Err = "OK";
                    return true;
                }
                else
                {
                    Err = "Invalid Barcode";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return false;

            }

        }

        public string GetBinLocation(string partNo, string invoice_No, string GRNNo, string Str_WH_Id, string Str_MAS_WH_Id)
        {

            try
            {
                string strOutput = string.Empty;
                string buff = string.Empty;
                string main = string.Empty;

                string sql = "";
                sql = "SELECT isnull(BINLOCATION,'') FROM GRNMASTER WHERE PARTNO='" + partNo.ToString() + "' AND InvoiceNumber LIKE '%" + invoice_No + "%' and MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "' and LOAD_DATE >= DATEADD (DAY, -100, GETDATE())";
                DataTableReader rd = Exec.GetDtReader(sql);

                if (rd.Read())
                {
                    main = rd.GetString(0);
                }
                else
                {
                    return strOutput = "Invalid Part Number";
                }
                rd.Close();

                sql = "SELECT isnull(buffer_location,'A') FROM GRNMASTER WHERE PARTNO='" + partNo.ToString() + "' and buffer_location<>'NULL' and WH_LOCATION_ID='" + Str_WH_Id + "' and LOAD_DATE >= DATEADD (DAY, -100, GETDATE())";
                rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    buff = rd.GetString(0);
                }
                rd.Close();

                strOutput = main + "," + buff;

                return strOutput;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }

        public bool getqty(string partNo, string invoice_No, string Str_WH_Id, string Mast_Wh_Loc, ref string binloc, ref int totqty, ref int balqty)
        {

            try
            {
                string sql = "SELECT BINLOCATION,QTY,BAL_QTY FROM GRNMASTER WHERE PARTNO='" + partNo.ToString() + "' AND InvoiceNumber='" + invoice_No + "' and MASTER_WH_LOC_ID='" + Mast_Wh_Loc + "' and LOAD_DATE >= DATEADD (DAY, -100, GETDATE())";
                DataTable dt = new DataTable();
                dt = Exec.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    binloc = dt.Rows[0]["BINLOCATION"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        totqty = totqty + Convert.ToInt32(dt.Rows[i]["QTY"].ToString());
                        balqty = balqty + Convert.ToInt32(dt.Rows[i]["BAL_QTY"].ToString());
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public string getbinnn(string partNo, string invo)
        {
            try
            {
                string strOutput = string.Empty;
                string sql = "SELECT a.Scannedbinlocation FROM grndetails a,grnmaster b WHERE  a.partno=b.partno and b.invoicenumber='" + invo + "' and a.partno='" + partNo + "' and a.BINNINGDATE= GETDATE() and a.binningdate <>''";

                DataTableReader rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    strOutput = rd.GetString(0);
                }
                else
                {
                    return strOutput = "";
                }
                return strOutput;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string get_valid_part(string Qrcode, ref string part, ref string err)
        {
            string[] firstchk;
            int flag = 0;
            int flag1 = 0;
            int totl = 0;
            string strfirst;
            DataTableReader readf1;
            string chkqr;

            try
            {

                if (Qrcode.Contains("/"))
                {
                    firstchk = Qrcode.Split('/');
                    chkqr = firstchk[0].TrimEnd();
                    if (chkqr.Contains('-'))
                    {
                        strfirst = "select * from item_master where kai_part_number='" + chkqr.TrimEnd() + "'";
                        readf1 = Exec.GetDtReader(strfirst);
                        if (readf1.Read())
                        {

                            part = chkqr.TrimEnd();
                            err = "Success";
                            readf1.Close();
                            return part;
                        }
                        else
                        {
                            readf1.Close();
                            err = "Invalid QR Code";
                            return part;
                        }
                    }
                    else if (chkqr.TrimEnd().Length == 10)
                    {
                        part = chkqr.Substring(0, 5) + "-" + chkqr.Substring(5, 5);
                        err = "Success";
                        return part;
                    }
                    else
                    {
                        string partl;
                        int parl = 0;
                        partl = "select * from TBL_PART_LEN";
                        readf1 = Exec.GetDtReader(partl);
                        if (readf1.Read())
                        {
                            parl = Convert.ToInt32(readf1.GetString(0));
                        }
                        readf1.Close();

                        int spartlen = 0;
                        spartlen = Convert.ToInt32(chkqr.TrimEnd().Length);

                        for (int k = spartlen; k <= parl; k++)
                        {
                            strfirst = "select * from item_master where vendor_part_number='" + chkqr.Substring(0, k) + "'";
                            readf1 = Exec.GetDtReader(strfirst);
                            if (readf1.Read())
                            {
                                totl = Convert.ToInt32(readf1.GetString(11));
                                flag = Convert.ToInt32(readf1.GetString(12));
                                flag1 = Convert.ToInt32(readf1.GetString(13));

                                readf1.Close();

                                if (totl == (flag + flag1))
                                {
                                    part = chkqr.Substring(0, flag) + "-" + chkqr.Substring(flag, flag1);
                                    err = "Success";
                                }
                                else
                                {
                                    if (k == parl)
                                    {
                                        readf1.Close();
                                        err = "Invalid QR Code";
                                        return part;
                                    }
                                }
                                readf1.Close();

                                break;
                            }
                            readf1.Close();
                            err = "Success";

                        }
                    }
                }
                else if (Qrcode.Contains(','))
                {
                    firstchk = Qrcode.Split(',');
                    chkqr = firstchk[0].TrimEnd();
                    if (chkqr.Contains('-'))
                    {
                        strfirst = "select * from item_master where kai_part_number='" + chkqr.TrimEnd() + "'";
                        readf1 = Exec.GetDtReader(strfirst);
                        if (readf1.Read())
                        {
                            part = chkqr.TrimEnd();

                            err = "Success";
                            readf1.Close();
                            return part;
                        }
                        else
                        {
                            readf1.Close();
                            err = "Invalid QR Code";
                            return part;
                        }

                    }
                    else if (chkqr.TrimEnd().Length == 10)
                    {
                        part = chkqr.Substring(0, 5) + "-" + chkqr.Substring(5, 5);
                        err = "Success";
                        return part;
                    }
                    else
                    {


                        string partl;
                        int parl = 0;

                        partl = "select * from TBL_PART_LEN";
                        readf1 = Exec.GetDtReader(partl);

                        if (readf1.Read())
                        {
                            parl = Convert.ToInt32(readf1.GetString(0));
                        }
                        readf1.Close();

                        int spartlen = 0;
                        spartlen = Convert.ToInt32(chkqr.TrimEnd().Length);

                        for (int k = spartlen; k <= parl; k++)
                        {
                            strfirst = "select * from item_master where vendor_part_number='" + chkqr.Substring(0, k) + "'";
                            readf1 = Exec.GetDtReader(strfirst);
                            if (readf1.Read())
                            {
                                totl = Convert.ToInt32(readf1.GetString(11));
                                flag = Convert.ToInt32(readf1.GetString(12));
                                flag1 = Convert.ToInt32(readf1.GetString(13));

                                readf1.Close();
                                if (totl == (flag + flag1))
                                {
                                    part = chkqr.Substring(0, flag) + "-" + chkqr.Substring(flag, flag1);
                                    err = "Success";
                                }
                                else
                                {
                                    if (k == parl)
                                    {
                                        readf1.Close();
                                        err = "Invalid QR Code";
                                        return part;
                                    }
                                }
                                readf1.Close();
                                break;
                            }
                            readf1.Close();
                            err = "Success";

                        }
                    }
                }
                else if (Qrcode.Length == 10)
                {
                    part = Qrcode.Substring(0, 5) + "-" + Qrcode.Substring(5, 5);
                    err = "Success";
                    return part;
                }
                else
                {
                    string partl;
                    int parl = 0;

                    partl = "select * from TBL_PART_LEN";
                    readf1 = Exec.GetDtReader(partl);
                    if (readf1.Read())
                    {
                        parl = Convert.ToInt32(readf1.GetString(0));
                    }
                    readf1.Close();
                    int LL = 0;

                    if ((Qrcode.TrimEnd().Length <= 12))
                    {
                        LL = Qrcode.TrimEnd().Length;
                        for (int k = LL; k <= parl; k++)
                        {
                            strfirst = "select * from item_master where vendor_part_number='" + Qrcode.Substring(0, k) + "'";
                            readf1 = Exec.GetDtReader(strfirst);
                            if (readf1.Read())
                            {
                                totl = Convert.ToInt32(readf1.GetString(11));
                                flag = Convert.ToInt32(readf1.GetString(12));
                                flag1 = Convert.ToInt32(readf1.GetString(13));

                                readf1.Close();
                                if (totl == (flag + flag1))
                                {
                                    part = Qrcode.Substring(0, flag) + "-" + Qrcode.Substring(flag, flag1);
                                    err = "Success";
                                }
                                else
                                {

                                    if (k == parl)
                                    {
                                        readf1.Close();
                                        err = "Invalid QR Code";
                                        return part;

                                    }
                                }
                                readf1.Close();

                                break;

                            }
                            readf1.Close();

                            err = "Success";

                        }
                    }
                    else
                    {
                        for (int k = 10; k <= parl; k++)
                        {
                            strfirst = "select * from item_master where vendor_part_number='" + Qrcode.Substring(0, k) + "'";
                            readf1 = Exec.GetDtReader(strfirst);
                            if (readf1.Read())
                            {
                                totl = Convert.ToInt32(readf1.GetString(11));
                                flag = Convert.ToInt32(readf1.GetString(12));
                                flag1 = Convert.ToInt32(readf1.GetString(13));
                                readf1.Close();
                                if (totl == (flag + flag1))
                                {
                                    part = Qrcode.Substring(0, flag) + "-" + Qrcode.Substring(flag, flag1);
                                    err = "Success";
                                }
                                else
                                {

                                    if (k == parl)
                                    {
                                        readf1.Close();
                                        err = "Invalid QR Code";
                                        return part;
                                    }
                                }
                                readf1.Close();

                                break;

                            }
                            readf1.Close();
                            err = "Success";

                        }
                    }


                }
                err = "Success";
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }
            return part;

        }

        public string Get_In_Qty_From_QR(string Str_invoice_no, string Str_Qr_Code, string par, string Str_User_Name, string Str_Carton, string Str_WH_Id, string Str_MAS_WH_Id, string Int_User_Type, string len, ref string err)
        {

            DataTableReader rd;
            DataTableReader strrs;
            DataTable dtsen = new DataTable();
            int Str_Count = 0;
            string strSql;
            int Int_Tot_Asn_Qty = 0;
            int Int_Bal_Asn_Qty = 0;
            int Int_Rec_Asn_Qty = 0;
            int Int_Hdr_ID = 0;
            string Str_Part_Name = string.Empty;
            string Str_Vendor_code = string.Empty;
            string Str_Output = string.Empty;
            string Str_Sql = string.Empty;

            string Str_Part_name = string.Empty;
            string Str_Part_Type = string.Empty;
            string Str_Asn_Number = string.Empty;
            string Str_Bin_Location = string.Empty;
            string Str_Uniqe_No = string.Empty;
            int Int_Part_Qty = 0;
            string Str_Scan_Part = string.Empty;
            string Str_Part_No = string.Empty;
            int Str_Part_Qty = 0;
            int Int_Qty_length = 0;
            string[] data;
            string strsql = "";

            if (Str_Qr_Code.Contains("/"))
            {
                data = Str_Qr_Code.Split('/');
            }
            else
            {
                data = Str_Qr_Code.Split(',');
            }

            try
            {

                strsql = "Select count(*) from INWARD_RECEIPT where VENDOR_INVOICE_NO='" + Str_invoice_no + "' and BAL_QTY IS NULL and MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "'";
                strrs = Exec.GetDtReader(strsql);
                if (strrs.Read())
                {
                    Str_Count = Convert.ToInt32(strrs.GetValue(0));
                }
                else
                {
                    Str_Count = 0;
                    err = "Invoice Qty.Fully Scan Completed ";

                    Str_Output = "0" + "," + " Invoice Qty. Fully Scan Completed";
                    return Str_Output;
                }

                strrs.Close();
                if (data.Length > 3)
                {
                    Str_Scan_Part = data[0];
                }
                else
                {
                    Str_Scan_Part = Str_Carton;
                }

                Str_Part_No = par;
                Str_Scan_Part = par;



                strSql = "select [QTY],[REC_QTY],[BAL_QTY] ,ID,VENDOR_CODE,PART_NAME ,ASN_NUMBER," +
                 "Isnull(BIN_LOCATION,''),PART_TYPE,INWARD_QTY_LENGTH " +
                 "from INWARD_RECEIPT_ASN WHERE [VENDOR_INVOICE_NO]='" + Str_invoice_no + "' and  " +
                 "[PART_NUMBER]='" + Str_Scan_Part + "'  AND MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "'";


                Str_Output = strsql;
                rd = Exec.GetDtReader(strsql);
                if (rd.Read())
                {
                    Int_Tot_Asn_Qty = Convert.ToInt32(rd.GetValue(0));
                    Int_Bal_Asn_Qty = Convert.ToInt32(rd.GetValue(2));
                    Int_Rec_Asn_Qty = Convert.ToInt32(rd.GetValue(1));
                    Int_Hdr_ID = Convert.ToInt32(rd.GetValue(3));
                    Str_Part_Name = rd.GetString(5);
                    Str_Vendor_code = rd.GetString(4);
                    Str_Bin_Location = rd.GetString(7);

                    Str_Part_Type = rd.GetString(8);

                    if (len == "B")
                    {
                    }
                    else
                    {
                        Int_Qty_length = Convert.ToInt32(len);
                    }

                    if (Int_Bal_Asn_Qty == 0)
                    {
                        rd.Close();
                        Str_Output = "0" + "," + Str_Scan_Part + " - " + Int_Tot_Asn_Qty + ". Inward Check Done";
                        return Str_Output;
                    }

                }
                else
                {
                    rd.Close();
                    Str_Output = "0" + "," + "Invalid Part Number";
                    return Str_Output;
                }
                rd.Close();

                if (Str_Qr_Code.Length >= (Int_Qty_length + 9))
                {
                    if (Int_Qty_length > 0)
                    {
                        int plen = 0;
                        string dummy = string.Empty;
                        string partreal = string.Empty;
                        partreal = par.Replace("-", "");
                        plen = partreal.Trim().Length;

                        Int_Part_Qty = Convert.ToInt32(Str_Qr_Code.Substring(plen, Int_Qty_length));
                        Str_Part_Qty = Int_Part_Qty;

                    }
                    else
                    {
                        Int_Part_Qty = 0;
                        Str_Part_Qty = Int_Part_Qty;

                    }

                    Str_Output = "1" + "," + Str_Scan_Part + "," + Int_Part_Qty + "," + Int_Bal_Asn_Qty + "," + Int_Tot_Asn_Qty;

                    Int_Hdr_ID = 0;
                }

                else
                {
                    rd.Close();
                    Str_Output = "0" + "," + " QR Code Length Mismatch";
                    return Str_Output;

                }
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }

            return Str_Output;

        }

        public string Chk_Inward_Part_Save_N(string Str_invoice_no, string Str_Part_No, int Str_Part_Qty, string Str_User_Name, string Str_Carton, string Str_WH_Id, string HHT_Serial_No, string Str_MAS_WH_Id, string vendor_qr, string stf, ref string err)
        {
            DataTableReader rd;
            DataTable dtsen = new DataTable();
            string strSql;
            int Int_Tot_Asn_Qty = 0;
            int Int_Bal_Asn_Qty = 0;
            int Int_Rec_Asn_Qty = 0;
            int Int_Hdr_ID = 0;
            string Str_Part_Name;
            string Str_Vendor_code;
            string Str_Output;
            string Str_Sql;
            string Str_Qr_Code;
            string Str_Part_Type;
            int Int_Fin_Rec_Qty = 0;
            int Int_Fin_Bal_Qty = 0;
            string Str_Asn_Number = "";
            string Str_Bin_Location;
            string Str_Uniqe_No;
            int Int_Part_Qty = Convert.ToInt32(Str_Part_Qty);

            try
            {
                strSql = "select [QTY],[REC_QTY],[BAL_QTY] ,ID,VENDOR_CODE,PART_NAME ,ASN_NUMBER,Isnull(BIN_LOCATION,''),PART_TYPE from INWARD_RECEIPT_ASN WHERE [VENDOR_INVOICE_NO]='" + Str_invoice_no + "' and [PART_NUMBER]='" + Str_Part_No + "'  and MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "'  ";
                Str_Output = strSql;
                rd = Exec.GetDtReader(strSql);
                if (rd.Read())
                {
                    Int_Tot_Asn_Qty = Convert.ToInt32(rd.GetValue(0));

                    Int_Bal_Asn_Qty = Convert.ToInt32(rd.GetValue(2));
                    Int_Rec_Asn_Qty = Convert.ToInt32(rd.GetValue(1));
                    Int_Hdr_ID = Convert.ToInt32(rd.GetValue(3));
                    Str_Part_Name = rd.GetString(5);
                    Str_Vendor_code = rd.GetString(4);
                    Str_Bin_Location = rd.GetString(7);

                    Str_Part_Type = rd.GetString(8);
                }
                else
                {
                    rd.Close();
                    Str_Output = "0" + "," + "Invalid Part Number";
                    return Str_Output;

                }

                rd.Close();

                if (Convert.ToInt32(Int_Bal_Asn_Qty) == 0)
                {

                    Str_Output = "0" + "," + Str_Part_No + " : " + Int_Tot_Asn_Qty + " Qty " + "   Scan Already  Done  ";
                    return Str_Output;
                }
                else if (Convert.ToInt32(Int_Part_Qty) > Convert.ToInt32(Int_Bal_Asn_Qty))
                {


                    Str_Output = "0" + "," + Str_Part_No + " : " + Int_Tot_Asn_Qty + " Qty " + "  Scan Qty Greater Than Invoice Qty ";
                    return Str_Output;
                }

                else
                {

                    Int_Fin_Rec_Qty = Convert.ToInt32(Int_Rec_Asn_Qty) + Convert.ToInt32(Str_Part_Qty);
                    Int_Fin_Bal_Qty = Convert.ToInt32(Int_Tot_Asn_Qty) - Int_Fin_Rec_Qty;

                    DateTime dt = DateTime.Now;

                    int Dt_Month = dt.Month;

                    string Str_Mo_Val = string.Empty;

                    if (Dt_Month == 1)
                    {
                        Str_Mo_Val = "A";
                    }
                    else if (Dt_Month == 2)
                    {
                        Str_Mo_Val = "B";
                    }
                    else if (Dt_Month == 3)
                    {
                        Str_Mo_Val = "C";
                    }
                    else if (Dt_Month == 4)
                    {
                        Str_Mo_Val = "D";
                    }
                    else if (Dt_Month == 5)
                    {
                        Str_Mo_Val = "E";
                    }
                    else if (Dt_Month == 6)
                    {
                        Str_Mo_Val = "F";
                    }
                    else if (Dt_Month == 7)
                    {
                        Str_Mo_Val = "G";
                    }
                    else if (Dt_Month == 8)
                    {
                        Str_Mo_Val = "H";
                    }
                    else if (Dt_Month == 9)
                    {
                        Str_Mo_Val = "J";
                    }
                    else if (Dt_Month == 10)
                    {
                        Str_Mo_Val = "K";
                    }
                    else if (Dt_Month == 11)
                    {
                        Str_Mo_Val = "L";
                    }
                    else if (Dt_Month == 12)
                    {
                        Str_Mo_Val = "M";
                    }


                    int Int_ID = 0;

                    string sq1;
                    sq1 = "select isnull(max(INWARD_RECPT_SRNO),0)+1  from INWARD_RECEIPT WHERE [VENDOR_INVOICE_NO]='" + Str_invoice_no + "'  and WH_LOCATION_ID='" + Str_WH_Id + "' ";
                    Int_ID = Convert.ToInt32(Exec.ExecuteScalar(sq1));

                    string string_Unoq_Srno = string.Empty;

                    if (Int_ID.ToString().Length == 1)
                    {
                        string_Unoq_Srno = "00000" + Convert.ToString(Int_ID);
                    }
                    else if (Int_ID.ToString().Length == 2)
                    {
                        string_Unoq_Srno = "0000" + Convert.ToString(Int_ID);
                    }
                    else if (Int_ID.ToString().Length == 3)
                    {
                        string_Unoq_Srno = "000" + Convert.ToString(Int_ID);
                    }
                    else if (Int_ID.ToString().Length == 4)
                    {
                        string_Unoq_Srno = "00" + Convert.ToString(Int_ID);
                    }
                    else if (Int_ID.ToString().Length == 5)
                    {
                        string_Unoq_Srno = "0" + Convert.ToString(Int_ID);
                    }
                    else if (Int_ID.ToString().Length == 6)
                    {
                        string_Unoq_Srno = Convert.ToString(Int_ID);
                    }


                    string Str_Recv_By = Str_User_Name;
                    string Str_unq_Date = Convert.ToString(dt.Day) + Str_Mo_Val + Convert.ToString(dt.Year).Substring(2, 2);
                    string Str_Unq_vendor_Code = Str_Vendor_code.Substring(0, 1) + Str_Vendor_code.Substring(4, 2);
                    Str_Uniqe_No = Str_Unq_vendor_Code + Str_invoice_no + "_" + Str_unq_Date + "_" + string_Unoq_Srno;
                    Str_Qr_Code = Str_Part_No + "," + Int_Part_Qty + "," + Str_invoice_no + "," + Str_Uniqe_No + "," + Str_Recv_By;

                    if (stf == "ST" || stf == "NST")
                    {
                        Str_Sql = "insert into INWARD_RECEIPT( HDR_ID, VENDOR_INVOICE_NO, PART_NUMBER,REC_QTY, " +
                            "QTY ,INWARD_BY,INWARD_DATE ,ASN_NUMBER,INWARD_UNIQUE_NUMBER,INWARD_PRINT_STATUS, " +
                            "INWARD_QR_CODE,BIN_LOCATION ,PART_NAME,PART_TYPE,INWARD_RECPT_SRNO,BINNING_STATUS," +
                            "CARTON_BARCODE,WH_LOCATION_ID,INWARD_SYS_ID, MASTER_WH_LOC_ID,VENDOR_CODE,VENDOR_QR) values( " +
                                  "'" + Int_Hdr_ID + "','" + Str_invoice_no + "','" + Str_Part_No + "'," +
                                  "'" + Int_Tot_Asn_Qty + "','" + Str_Part_Qty + "','" + Str_Recv_By + "',  getdate() , " +
                           "'" + Str_Asn_Number + "','" + Str_Uniqe_No + "',1,'" + Str_Qr_Code + "'," +
                           "'" + Str_Bin_Location + "','" + Str_Part_Name + "','" + Str_Part_Type + "', " +
                           "'" + Int_ID + "',0,'" + Str_Carton + "','" + Str_WH_Id + "','" + HHT_Serial_No + "',  " +
                           "'" + Str_MAS_WH_Id + "','" + Str_Vendor_code + "','" + vendor_qr + "')";

                        Exec.ExecuteNonQuery(Str_Sql);
                    }
                    else
                    {
                        Str_Sql = "insert into INWARD_RECEIPT( HDR_ID, VENDOR_INVOICE_NO, PART_NUMBER,REC_QTY, " +
                           "QTY ,INWARD_BY,INWARD_DATE ,ASN_NUMBER,INWARD_UNIQUE_NUMBER,INWARD_PRINT_STATUS, " +
                           "INWARD_QR_CODE,BIN_LOCATION ,PART_NAME,PART_TYPE,INWARD_RECPT_SRNO,BINNING_STATUS," +
                           "CARTON_BARCODE,WH_LOCATION_ID,INWARD_SYS_ID, MASTER_WH_LOC_ID,VENDOR_CODE,VENDOR_QR) values( " +
                                 "'" + Int_Hdr_ID + "','" + Str_invoice_no + "','" + Str_Part_No + "'," +
                                 "'" + Int_Tot_Asn_Qty + "','" + Str_Part_Qty + "','" + Str_Recv_By + "'," +
                                 "getdate() , " +
                          "'" + Str_Asn_Number + "','" + Str_Uniqe_No + "',0,'" + Str_Qr_Code + "'," +
                          "'" + Str_Bin_Location + "','" + Str_Part_Name + "','" + Str_Part_Type + "', " +
                          "'" + Int_ID + "',0,'" + Str_Carton + "','" + Str_WH_Id + "','" + HHT_Serial_No + "',  " +
                          "'" + Str_MAS_WH_Id + "','" + Str_Vendor_code + "','" + vendor_qr + "')";

                        Exec.ExecuteNonQuery(Str_Sql);
                    }


                    Str_Sql = "update   INWARD_RECEIPT_ASN set REC_QTY ='" + Int_Fin_Rec_Qty + "', BAL_QTY='" + Int_Fin_Bal_Qty + "' " +
                        " where  VENDOR_INVOICE_NO='" + Str_invoice_no + "' and PART_NUMBER='" + Str_Part_No + "' and id='" + Int_Hdr_ID + "' AND  MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "'";

                    Exec.ExecuteNonQuery(Str_Sql);


                    Str_Sql = " UPDATE  INWARD_RECEIPT SET " +
                                     "INWARD_RECEIPT.VENDOR_INVOICE_DATE =  INWARD_RECEIPT_ASN.VENDOR_INVOICE_DATE ,INWARD_RECEIPT.VENDOR_CODE=INWARD_RECEIPT_ASN.VENDOR_CODE " +
                                     "FROM  INWARD_RECEIPT, INWARD_RECEIPT_ASN " +
                                     "WHERE     INWARD_RECEIPT.VENDOR_INVOICE_NO =  INWARD_RECEIPT_ASN.VENDOR_INVOICE_NO " +
                                     "and INWARD_RECEIPT_ASN.VENDOR_INVOICE_NO ='" + Str_invoice_no + "' and   " +
                                     " INWARD_RECEIPT.PART_NUMBER='" + Str_Part_No + "' AND  INWARD_RECEIPT.WH_LOCATION_ID='" + Str_WH_Id + "'";

                    Exec.ExecuteNonQuery(Str_Sql);

                    Str_Output = "1" + "," + Int_Tot_Asn_Qty + "," + Int_Fin_Bal_Qty + "," + Int_Fin_Rec_Qty + "," + Str_Part_Name;

                    Int_Hdr_ID = 0;
                    Int_Fin_Bal_Qty = 0;
                    Int_Fin_Rec_Qty = 0;

                }



            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }

            return Str_Output;

        }

        public string Location_save_toACCPAC(string part, string loca, string userid, string whloc, int bin_qty, ref string err, string inv)
        {
            string strsql;
            List<string> Qrys = new List<string>();
            bool res = false;
            try
            {
                string whlocid = string.Empty;
                err = "1";
                strsql = "Select Isnull(wh_location_id,'') from grnmaster WHERE partno='" + part.TrimEnd() + "' and invoicenumber LIKE '%" + inv + "%'";
                DataTableReader rd = Exec.GetDtReader(strsql);
                if (rd.Read())
                {
                    whlocid = rd.GetString(0);
                }
                rd.Close();
                rd.Dispose();

                strsql = string.Empty;

                //location update for accpac or Oracal
                strsql = "Insert into WMS_GRNMASTER (PART_NO,WH_LOC_ID,BIN_LOCATION,FLAG,BIN_QTY) VALUES('" + part + "','" + whlocid + "','" + loca + "','0','" + bin_qty + "')";
                Qrys.Add(strsql);
                //res = Exec.ExecuteNonQuery(strsql);
                //err = res == false ? "Err 2" : "Success";

                strsql = string.Empty;
                //location update for accpac or Oracal
                strsql = "update GRNMASTER_TEMP set Picking_Sequence='" + loca.ToString() + "' where InvoiceNumber='" + inv.Trim() + "' and PartNo='" + part.TrimEnd() + "'";
                Qrys.Add(strsql);
                //res =  Exec.ExecuteNonQuery(strsql);
                //err = res == false?"Error" : "Success";

                res = Exec.UpdateUsingExecuteNonQueryList(Qrys);
                return err = res == false ? "Error" : "Success";

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                return err;
                throw;
            }


        }


        public bool saveBinningDetails2(string GRNNo, string TwoDBarcode, string PartNo, int Qty, string BinLocation, string ScannedBinLocation, string BinningBy, int Bin_Loc_Cr, string Str_Uniq, string Invoice_No, string Str_WH_Id, string Mast_Wh_Loc, ref string Part_St, ref string Err)
        {
            string sql = "";
            try
            {
                Err = string.Empty;

                DateTime dt = DateTime.Now;

                sql = "select TOP(1) Qty,REC_QTY,BAL_QTY,GRNNO from GRNMaster where bal_qty<>0 and InvoiceNumber='" + Invoice_No + "' and PartNo='" + PartNo + "' and MASTER_WH_LOC_ID='" + Mast_Wh_Loc + "'";

                DataTableReader rd = Exec.GetDtReader(sql);
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

                bool Res = false;

                if (Int_Fin_Rec_Qty <= Int_Tot_GRN_Qty)
                {
                    sql = string.Empty;
                    sql = "INSERT INTO [GRNDetails]([GRNNo],[2DBarcode],[PartNo],[Qty],[BinLocation],[ScannedBinLocation],[BinningBy],[BinningDate],Bin_Location_Create,WH_LOCATION_ID,BUFFER_LOCATION)VALUES('" + grnno.ToString() + "','" + TwoDBarcode.ToString() + "','" + PartNo.ToString() + "'," + Qty.ToString() + ",'" + BinLocation.ToString() + "','" + ScannedBinLocation.ToString() + "','" + BinningBy.ToString() + "',getdate(),'" + Bin_Loc_Cr + "','" + Str_WH_Id + "','" + ScannedBinLocation.ToString() + "')";
                    Res = Exec.ExecuteNonQuery(sql);
                    Err = Res == false ? "Err 1" : "GRNDetails completed";

                    Res = false;
                    sql = string.Empty;
                    sql = "update INWARD_RECEIPT set BINNING_BY='" + BinningBy + "', BINNING_DATE=getdate(), BINNING_STATUS=1,BUFFER_LOCATION='" + ScannedBinLocation.ToString() + "',SCANNED_LOCATION='" + ScannedBinLocation.TrimEnd().ToString() + "' where VENDOR_INVOICE_NO='" + Invoice_No + "' and INWARD_UNIQUE_NUMBER='" + Str_Uniq + "' and PART_NUMBER ='" + PartNo + "' and MASTER_WH_LOC_ID='" + Mast_Wh_Loc + "'";
                    Res = Exec.ExecuteNonQuery(sql);
                    Err = Res == false ? "Err 2" : Err += "INWARD_RECEIPT updated";

                    Res = false;
                    sql = string.Empty;
                    sql = "update GRNMaster set REC_QTY='" + Int_Fin_Rec_Qty + "', BAL_QTY='" + Int_Fin_Bal_Qty + "',BUFFER_LOCATION='" + ScannedBinLocation.ToString() + "' where InvoiceNumber='" + Invoice_No + "'  and PartNo ='" + PartNo + "' and MASTER_WH_LOC_ID='" + Mast_Wh_Loc + "' and grnno='" + grnno.ToString() + "'";
                    Res = Exec.ExecuteNonQuery(sql);

                    Err = Res == false ? "Err 3" : Err += "GRN All Completed";

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

        //Check this
        public DataTable get_binning_pending(string whloc, ref string err)
        {
            DataTable dtbin = new DataTable();
            try
            {
                string sqlbin;
                sqlbin = "select distinct invoicenumber from grnmaster where bal_qty<>0 AND wh_location_id='" + whloc + "' and LOAD_DATE >= '2019-01-01 00:00:00.000'";
                dtbin = Exec.GetDataTable(sqlbin);
                dtbin.TableName = "grnmaster";
                err = "Success";

            }
            catch (Exception ex)
            {
                err = ex.Message;
                throw;

            }

            return dtbin;
        }
        //Check this
        public DataTable get_pend_bin(string invoc, ref string err)
        {
            DataTable dtbin = new DataTable();
            try
            {
                string sqlbin;
                sqlbin = "select DISTINCT partno,partname,qty,bal_qty,status,wh_location_id from grnmaster where bal_qty<>0 and invoicenumber='" + invoc + "'";
                dtbin = Exec.GetDataTable(sqlbin);
                dtbin.TableName = "grnmaster";
                err = "Success";
            }
            catch (Exception ex)
            {
                err = ex.Message;
                throw;

            }

            return dtbin;
        }

        //check This
        public string getlen2(string invoi, ref string Str_Len, ref string err)
        {

            string sq1;
            DataTable dtsen = new DataTable();

            try
            {

                sq1 = "select Inward_Qty_Length  from Inward_Receipt_ASN where Vendor_Invoice_No='" + invoi + "' and bal_qty<>'0'";
                Str_Len = Convert.ToString(Exec.ExecuteScalar(sq1));

                err = "Success";
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }

            return Str_Len;


        }

        public DataTable Get_Grn_Balance(string inv, string partno, string masterwhloc)
        {
            DataTable dt = new DataTable();
            string Query = "select  BAL_QTY from GRNMaster where InvoiceNumber like '%" + inv + "%' and PartNo = '" + partno + "' and MASTER_WH_LOC_ID = '" + masterwhloc + "' ";
            dt = Exec.GetDataTable(Query);
            return dt;
        }

        public DataTable Get_InvNo(string whLocId)
        {
            DataTable dt = new DataTable();
            string query = "select distinct invoicenumber from grnmaster where bal_qty<>0 AND wh_location_id='" + whLocId + "' and LOAD_DATE >= '2019-01-01 00:00:00.000'";
            dt = Exec.GetDataTable(query);
            return dt;
        }

        public DataTableReader Get_Grn_Qtys(string Invoice_No, string PartNo, string MastWhLoc)
        {
            string sql = "select Qty,REC_QTY,BAL_QTY,grnno from GRNMaster where InvoiceNumber='" + Invoice_No + "' and PartNo='" + PartNo + "' and MASTER_WH_LOC_ID='" + MastWhLoc + "' and bal_qty<>0";
            DataTableReader rd = Exec.GetDtReader(sql);
            return rd;
        }

        public bool Insert_GrnDtls(string grnno, string TwoDBarcode, string PartNo, int Qty, string BinLocation, string ScannedBinLocation, string BinningBy, int Bin_Loc_Cr, string Str_WH_Id)
        {
            bool b = false;
            string sql = "INSERT INTO [GRNDetails]([GRNNo],[2DBarcode],[PartNo],[Qty],[BinLocation],[ScannedBinLocation],[BinningBy],[BinningDate],Bin_Location_Create,WH_LOCATION_ID) " +
                "VALUES('" + grnno.ToString() + "','" + TwoDBarcode.ToString() + "','" + PartNo.ToString() + "'," + Qty.ToString() + ",'" + BinLocation.ToString() + "','" + ScannedBinLocation.ToString() + "','" + BinningBy.ToString() + "'," +
                "getdate(),'" + Bin_Loc_Cr + "','" + Str_WH_Id + "')";
            b = Exec.ExecuteNonQuery(sql);
            return b;
        }

        public bool Updt_IwRecMast(string BinningBy, string ScannedBinLocation, string Invoice_No, string PartNo, string mast)
        {
            bool b = false;
            string sql = "update  INWARD_RECEIPT_Master set BINNING_BY='" + BinningBy + "', BINNING_DATE=getdate(), BINNING_STATUS=1,SCANNED_LOCATION='" + ScannedBinLocation.TrimEnd().ToString() + "' where VENDOR_INVOICE_NO='" + Invoice_No + "'  and PART_NUMBER ='" + PartNo + "' and MASTER_WH_LOC_ID='" + mast + "'";
            b = Exec.ExecuteNonQuery(sql);
            return b;
        }

        public bool Updt_InwRec_Sts_M(string qty, string Invoice_No, string PartNo, string mast, string BinningBy, string ScannedBinLocation)
        {
            bool b = false;

            string sql = "WITH CTE1 AS ( SELECT TOP(" + qty + ") * FROM INWARD_RECEIPT WHERE ( VENDOR_INVOICE_NO = '" + Invoice_No + "' AND PART_NUMBER = '" + PartNo + "'";
            sql += "AND MASTER_WH_LOC_ID = '" + mast + "') ORDER BY INWARD_UNIQUE_NUMBER)";
            sql += "UPDATE CTE1 SET BINNING_BY = '" + BinningBy + "', BINNING_DATE = GETDATE(), BINNING_STATUS = 1, SCANNED_LOCATION = '" + ScannedBinLocation.TrimEnd().ToString() + "';";
            b = Exec.ExecuteNonQuery(sql);
            return b;
        }

        public bool Updt_InwRec_Sts_C(string qty, string Invoice_No, string PartNo, string mast, string Str_Uniq, string BinningBy, string ScannedBinLocation)
        {
            bool b = false;

            string sql = "WITH CTE AS ( SELECT TOP(" + qty + ") * FROM INWARD_RECEIPT WHERE ( VENDOR_INVOICE_NO = '" + Invoice_No + "' AND PART_NUMBER = '" + PartNo + "'";
            sql += "AND MASTER_WH_LOC_ID = '" + mast + "' AND INWARD_UNIQUE_NUMBER >= '" + Str_Uniq + "') ORDER BY INWARD_UNIQUE_NUMBER)";
            sql += "UPDATE CTE SET BINNING_BY = '" + BinningBy + "', BINNING_DATE = GETDATE(), BINNING_STATUS = 1, SCANNED_LOCATION = '" + ScannedBinLocation.TrimEnd().ToString() + "';";

            b = Exec.ExecuteNonQuery(sql);
            return b;
        }

        public bool Updt_GrnM_Bal(int Int_Fin_Rec_Qty, int Int_Fin_Bal_Qty, string Invoice_No, string PartNo, string mast, string grnno)
        {
            bool b = false;
            string sql = "update  GRNMaster set REC_QTY='" + Int_Fin_Rec_Qty + "', BAL_QTY='" + Int_Fin_Bal_Qty + "' where InvoiceNumber='" + Invoice_No + "'  and PartNo ='" + PartNo + "' and MASTER_WH_LOC_ID='" + mast + "' and GRNNO='" + grnno.ToString() + "'";
            b = Exec.ExecuteNonQuery(sql);
            return b;
        }

        //Check This
        public int Chk_Inward_Invoice_Count(string Str_invoice_no, string Str_WH_Id, string masterWhLoc, ref string err)
        {
            DataTableReader strrs;
            DataTable dtsen = new DataTable();
            int Str_Count = 0;
            string strsql = "";
            try
            {

                strsql = "Select count(*) from INWARD_RECEIPT_ASN where VENDOR_INVOICE_NO='" + Str_invoice_no + "' and BAL_QTY <>0 and MASTER_WH_LOC_ID='" + masterWhLoc.TrimEnd() + "'";

                strrs = Exec.GetDtReader(strsql);
                err = "Success";

                if (strrs.Read())
                {

                    Str_Count = Convert.ToInt32(strrs.GetValue(0));
                }
                else
                {
                    Str_Count = 0;
                    err = "Invalid Invoice Number";
                }


                strrs.Close();


            }
            catch (Exception ex)
            {
                err = strsql + ex.ToString();
            }
            return Str_Count;

        }




    }
}
