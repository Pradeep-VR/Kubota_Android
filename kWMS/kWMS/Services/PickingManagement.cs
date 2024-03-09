using kWMS.Extras;
using System;
using System.Data;
using System.Linq;

namespace kWMS.Services
{
    public class PickingManagement
    {
        Executer Exec = new Executer();
        public PickingManagement()
        {

        }

        public DataTableReader GetPick_Mast(string PartNo,string BinLoc)
        {
            string strSql;
            DataTableReader rd;

            strSql = "select * from [PICKING_MASTER] where PART_NUMBER='" + PartNo + "' and PICKING_SEQUENCE='" + BinLoc + "' and BAL_QTY<>0 ";
            rd = Exec.GetDtReader(strSql);
            return rd;
        }

        public DataTable Get_PicklstNo(string Str_UserName, string Mast_Wh_Loc, string DealerCode, string PicklistNo)
        {
            DataTable dt = new DataTable();
            string sqlc = "SELECT DISTINCT PICKLIST_NO FROM PICKING_MASTER  where  PICKING_STATUS=0  and BAL_QTY <>0  and PICKING_USER='" + Str_UserName + "' and  " +
                    " MASTER_WH_LOC_ID ='" + Mast_Wh_Loc + "' and DEALER_CODE='" + DealerCode + "' and PICKLIST_NO='" + PicklistNo + "' ORDER BY PICKLIST_NO ";
            dt = Exec.GetDataTable(sqlc);
            return dt;
        }

        public bool Ins_Picking(string PickListNo, int Flag)
        {
            bool b = false;
            string strsql = "Insert into TBL_PICK_COMPLETED_HEAD (PICKLIST_NO,FLAG) VALUES('" + PickListNo + "','" + Flag + "')";
            b = Exec.ExecuteNonQuery(strsql);

            return b;
        }

        public string save_split_pick(string picklist, string loc, string part, int Str_Part_Qty, string Str_User_Name, string deviceid, string dealer, string Str_WH_Id, string Str_Mas_WH_ID, string str_box_unique_no, string bin, ref string err)
        {

            try
            {
                string qrcode = string.Empty;
                string uniq = string.Empty;

                string strsql = string.Empty;

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

                uniq = Str_Mo_Val + dealer.TrimEnd() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString() + Str_User_Name;

                qrcode = part + "," + Str_Part_Qty + "," + uniq;

                strsql = "Insert into SPLIT_PICK_DETAILS(PICKLIST_NO,SCAN_LOC,PART_NO,QR_CODE,QTY,SPLIT_USER, " +
                " SPLIT_DATE,DEVICE_ID,DEALER_CODE,FLAG,WH_LOCATION,MAST_WH_LOC,BIN,PRINT_QTY) VALUES('" + picklist + "' " +
                " ,'" + loc + "','" + part + "','" + qrcode + "','" + Str_Part_Qty + "','" + Str_User_Name + "' " +
                " ,getdate(),'" + deviceid + "','" + dealer + "','0','" + Str_WH_Id + "','" + Str_Mas_WH_ID + "','" + bin + "','0')";

                Exec.ExecuteNonQuery(strsql);
                err = "Success";
                return err;

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }

        }

        public bool valid_bin_loc(string locat, ref string Err)
        {

            string sql;
            DataTableReader rd;
            try
            {

                sql = "select * from wh_location_master where location_id='" + locat + "'";

                rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    rd.Close();

                    Err = "OK";

                    return true;
                }

                else
                {
                    rd.Close();

                    Err = "Invalid Bin Location";

                    return false;

                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Validate_Picking_Done_Chk(string Str_PickList_No, string Str_Part_No, int Int_qty, string Inique_No, string Str_WH_Id, string Str_MAS_WH_Id, ref string Err)
        {

            string sql;
            try
            {

                sql = "select * from PICKING_DETAILS where  PICKLIST_NO='" + Str_PickList_No + "' and PART_NUMBER='" + Str_Part_No + "'  and INWARD_UNIQUE_NUMBER='" + Inique_No + "' and MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "' and PICK_DATE >= DATEADD(DAY, -15, GETDATE())";

                DataTableReader rd = Exec.GetDtReader(sql);

                if (rd.Read())
                {
                    rd.Close();

                    Err = "Picking Already Done";

                    return true;
                }
                else
                {
                    rd.Close();

                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool Save_PickList(string Str_PickList_No, string PartNo, int Qty, string BinLocation, string ScannedBinLocation, string Str_Pick_By, string Str_Unique, string Str_Qr_Code, string Str_WH_Id, string Str_MAS_WH_Id, ref string Err)
        {

            string sql;
            try
            {
                DateTime dt = DateTime.Now;

                sql = "INSERT INTO PICKING_DETAILS(PICKLIST_NO,  PART_NUMBER,  QTY, PICKING_SEQUENCE,SCAN_BIN_LOCATION, " +
                                " PICK_BY, PICK_DATE,  PICKING_STATUS, WH_LOCATION_ID,INWARD_UNIQUE_NUMBER,SCANED_QR_CODE,MRP_PRINT_STATUS,MRP_PRINT_READY,MASTER_WH_LOC_ID)VALUES " +
                                "('" + Str_PickList_No + "','" + PartNo + "','" + Qty + "','" + BinLocation + "','" + ScannedBinLocation + "', " +
                                "'" + Str_Pick_By + "',getdate(),1,'" + Str_WH_Id + "','" + Str_Unique + "','" + Str_Qr_Code + "',0,0,'" + Str_MAS_WH_Id + "')";

                Exec.ExecuteNonQuery(sql);

                sql = "";
                sql = "select Qty,REC_QTY,BAL_QTY from PICKING_MASTER where PICKLIST_NO='" + Str_PickList_No + "'  " +
                   "and PART_NUMBER='" + PartNo + "' and  PICKING_USER ='" + Str_Pick_By + "' and MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "' and PICK_DATE >= DATEADD (DAY, -15, GETDATE()) ";

                DataTableReader rd = Exec.GetDtReader(sql);

                int Int_Tot_Pick_Qty = 0;
                int Int_Bal_Pick_Qty = 0;
                int Int_Rec_Pick_Qty = 0;
                if (rd.Read())
                {
                    Int_Tot_Pick_Qty = Convert.ToInt32(rd.GetValue(0));
                    Int_Bal_Pick_Qty = Convert.ToInt32(rd.GetValue(2));
                    Int_Rec_Pick_Qty = Convert.ToInt32(rd.GetValue(1));

                }

                rd.Close();

                int Int_Fin_Rec_Qty = Int_Rec_Pick_Qty + Qty;
                int Int_Fin_Bal_Qty = Int_Tot_Pick_Qty - Int_Fin_Rec_Qty;

                sql = "";
                sql = "update  PICKING_MASTER set REC_QTY='" + Int_Fin_Rec_Qty + "', BAL_QTY='" + Int_Fin_Bal_Qty + "' where " +
                    "PICKLIST_NO='" + Str_PickList_No + "'  " +
                    "and  PART_NUMBER='" + PartNo + "' and  PICKING_USER ='" + Str_Pick_By + "' and MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "'";

                Exec.ExecuteNonQuery(sql);
                Err = sql;

                return true;
            }
            catch (Exception ex)
            {
                Err = Err + ex.Message;
                return false;
            }
        }


        public string get_valid_part(string Qrcode, ref string part, ref string err)
        {

            string[] firstchk;
            int flag = 0;
            int flag1 = 0;
            int totl = 0;
            string strfirst = string.Empty;

            DataTableReader readf1;

            string chkqr = string.Empty;

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

                            readf1.Close();
                            err = "Success";
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
                            readf1.Close();
                            err = "Success";
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

        public string testing2(string Str_invoice_no, ref string errmr)
        {
            DataTableReader strrs;
            DataTable dtsen = new DataTable();
            string Str_Final_Chk_Count = string.Empty;
            string strsql;
            try
            {

                strsql = "Select * from MRP_DETAILS where mrp_qr_barcode='" + Str_invoice_no + "'";

                strrs = Exec.GetDtReader(strsql);
                errmr = "Success";
                if (strrs.Read())
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
            finally
            {

            }
            return Str_Final_Chk_Count;

        }


        public DataTable download_PickList_Pic(string deal, string Str_UserName, string Str_WH_ID, string Str_MAS_WH_ID, ref string Str_Next_Location, ref string err)
        {

            string strSql;
            DataTable dtsen = new DataTable();

            try
            {

                strSql = "SELECT DISTINCT PICKLIST_NO FROM PICKING_MASTER  " +
                       "where  PICKING_STATUS=0  and BAL_QTY <>0  and PICKING_USER='" + Str_UserName + "' and  " +
                       " MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "' and DEALER_CODE='" + deal + "' ORDER BY PICKLIST_NO ";

                dtsen = Exec.GetDataTable(strSql);
                dtsen.TableName = "PICKLIST_NO";
                err = "Success";

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }
            finally
            {

            }

            return dtsen;
        }

        public string save_pending_pick(string picklist, string bin, string user, string whloc, string Mwhloc, string flag, ref string err)
        {

            try
            {

                string wherecondition = string.Empty;
                string[] data;

                string whereCondition = string.Empty;

                if (picklist.Contains(','))
                {
                    data = picklist.Split(',');

                    for (int i = 0; i < data.Length; i++)
                    {
                        whereCondition = whereCondition + "PICKLIST_NO = '" + data[i].TrimEnd() + "' OR";
                    }
                    whereCondition = whereCondition.Substring(0, whereCondition.Length - 3);
                }
                else
                {
                    whereCondition = "PICKLIST_NO = '" + picklist.TrimEnd();
                }
                if (flag == "S")
                {
                    data = picklist.Split(',');
                    for (int i = 0; i < data.Length; i++)
                    {
                        string strsql = "Insert into PICK_PENDING_DETAILS (PICKLIST_NO,PICK_USER,BREAK_DATE,BIN,FLAG,WHLOC,MWHLOC) " +
                        " VALUES('" + data[i].TrimEnd() + "','" + user + "',GETDATE(),'" + bin + "','0','" + whloc + "','" + Mwhloc + "')";

                        Exec.ExecuteNonQuery(strsql);
                        err = "Success";

                    }

                    return err;
                }
                else if (flag == "U")
                {

                    data = picklist.Split(',');
                    for (int i = 0; i < data.Length; i++)
                    {
                        string strsql = "UPDATE PICK_PENDING_DETAILS SET FLAG='1' WHERE flag ='0' and PICKLIST_NO='" + data[i].TrimEnd() + "' and PICK_USER='" + user + "' and WHLOC='" + whloc + "'";

                        Exec.ExecuteNonQuery(strsql);
                        err = "Success";
                    }
                    return err;
                }

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;

            }
            finally
            {

            }
            return err;
        }

        public string getbinnn4Pick(string partNo)
        {

            try
            {

                string strOutput = string.Empty;

                string sql = "SELECT Isnull(Buffer_location,'  ') FROM grnmaster WHERE partno='" + partNo + "' and Buffer_location<>''";

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

        public string download_PickList_Using_No(string Str_PickList_No, string Str_UserName, string Str_WH_ID, string Str_MAS_WH_ID, string Str_Part_No, ref string Str_Next_Location, ref string Str_Tot_Line, ref string Str_Rec_Item, ref string Str_Bal_item, ref string parnam, ref string err)
        {

            DataTableReader rd;
            string strSql;
            string sq1;
            DataTable dtsen = new DataTable();

            try
            {

                sq1 = "select COUNT(DISTINCT PART_NUMBER)  from    PICKING_MASTER where  PICKING_STATUS=0  and PICKING_USER='" + Str_UserName + "' " +
                       "and PICKLIST_NO='" + Str_PickList_No + "'   and   MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'   ";

                Str_Tot_Line = Convert.ToString(Exec.ExecuteScalar(sq1));

                sq1 = "";
                sq1 = "select COUNT(DISTINCT PART_NUMBER)  from    PICKING_MASTER  where  PICKING_STATUS=0  and PICKING_USER='" + Str_UserName + "' " +
                   "and PICKLIST_NO='" + Str_PickList_No + "'   and BAL_QTY<>0  AND  MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'   ";

                Str_Bal_item = Convert.ToString(Exec.ExecuteScalar(sq1));

                sq1 = "";
                sq1 = "select COUNT(DISTINCT PART_NUMBER)  from    PICKING_MASTER " +
                  "where  PICKING_STATUS=0  and PICKING_USER='" + Str_UserName + "' " +
                   "and PICKLIST_NO='" + Str_PickList_No + "'   and BAL_QTY=0 AND " +
                   "MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'   ";

                Str_Rec_Item = Convert.ToString(Exec.ExecuteScalar(sq1));

                Str_Next_Location = string.Empty;
                strSql = "";
                strSql = "select top 1 PART_NUMBER +','+ 'PART_NAME' +','+ CAST(QTY AS VARCHAR(10)) +','+  " +
                        "PICKING_SEQUENCE  +','+  CAST(SKIP_STATUS AS VARCHAR(10)) " +
                        "+','+  CAST(rec_qty AS VARCHAR(10)) " +
                        "+','+ CAST( bal_qty AS VARCHAR(10)) +','+ DEALER_CODE " +
                          " as f1 from    PICKING_MASTER " +
                        " where  PICKING_STATUS=0 and BAL_QTY<>0 and PICKING_USER='" + Str_UserName + "' and PICKLIST_NO='" + Str_PickList_No + "'  " +
                        "and MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'  and PART_NUMBER <> '" + Str_Part_No + "' and PICK_DATE >= DATEADD (DAY, -15, GETDATE())" +
                        "order by SKIP_STATUS, PICKING_SEQUENCE  asc";

                rd = Exec.GetDtReader(strSql);

                if (rd.Read())
                {
                    Str_Next_Location = rd.GetString(0);
                }
                else
                {
                    err = "No Record Found";

                    return Str_Next_Location;
                }
                err = "Success";

                rd.Close();

                if (Str_Next_Location.Contains(','))
                {
                    string[] panam;
                    panam = Str_Next_Location.Split(',');
                    if (panam.Length > 1)
                    {
                        parnam = panam[0];

                        string sql = "select part_description from item_master where kai_part_number='" + panam[0] + "'";

                        DataTableReader rdd1;
                        rdd1 = Exec.GetDtReader(sql);
                        if (rdd1.Read())
                        {
                            parnam = rdd1.GetString(0);

                        }

                        rdd1.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }

            return Str_Next_Location;
        }

        public string download_PickList_Using_No1(string Str_PickList_No, string Str_UserName, string Str_WH_ID, string Str_MAS_WH_ID, string Str_Part_No, ref string Str_Next_Location, ref string Str_Tot_Line, ref string Str_Rec_Item, ref string Str_Bal_item, ref string parnam, ref string err)
        {
            DataTableReader rd;
            string strSql;
            string sq1;
            string whereCondition = string.Empty;

            string[] data;

            if (Str_PickList_No.Contains(','))
            {
                data = Str_PickList_No.Split(',');

                for (int i = 0; i < data.Length; i++)
                {
                    whereCondition = whereCondition + "PICKLIST_NO = '" + data[i].TrimEnd() + "' OR ";
                }

                whereCondition = whereCondition.Substring(0, whereCondition.Length - 3);
            }
            else
            {
                whereCondition = "PICKLIST_NO = '" + Str_PickList_No.TrimEnd() + "'";
            }

            try
            {

                sq1 = "select COUNT(DISTINCT PART_NUMBER)  from    PICKING_MASTER where  PICKING_STATUS=0  and PICKING_USER='" + Str_UserName + "' and (" + whereCondition + ") and  MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'";

                Str_Tot_Line = Convert.ToString(Exec.ExecuteScalar(sq1));

                sq1 = "";
                sq1 = "select COUNT(DISTINCT PART_NUMBER)  from    PICKING_MASTER where  PICKING_STATUS=0  and PICKING_USER='" + Str_UserName + "' and BAL_QTY<>0 AND MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "' AND (" + whereCondition + ") ";

                Str_Bal_item = Convert.ToString(Exec.ExecuteScalar(sq1));

                sq1 = "select COUNT(DISTINCT PART_NUMBER)  from    PICKING_MASTER where  PICKING_STATUS=0  and PICKING_USER='" + Str_UserName + "' and (" + whereCondition + ")   and BAL_QTY=0 AND MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'   ";

                Str_Rec_Item = Convert.ToString(Exec.ExecuteScalar(sq1));

                Str_Next_Location = string.Empty;

                strSql = "select top 1 PART_NUMBER +','+ 'PART_NAME' +','+ CAST(QTY AS VARCHAR(10)) +','+ PICKING_SEQUENCE  +','+  CAST(SKIP_STATUS AS VARCHAR(10)) +','+  CAST(rec_qty AS VARCHAR(10)) +','+ CAST( bal_qty AS VARCHAR(10)) +','+ picklist_no +','+ DEALER_CODE  as f1 from    PICKING_MASTER where  PICKING_STATUS=0 and BAL_QTY<>0 and PICKING_USER='" + Str_UserName + "' and (" + whereCondition + ") and MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'  and PART_NUMBER <> '" + Str_Part_No + "' order by SKIP_STATUS, PICKING_SEQUENCE  asc";

                rd = Exec.GetDtReader(strSql);

                if (rd.Read())
                {
                    Str_Next_Location = rd.GetString(0);

                    rd.Close();

                    string[] data2;
                    string partnam = string.Empty;

                    data2 = Str_Next_Location.Split(',');
                    if (data2.Length > 0)
                    {
                        string get = "Select Part_description from item_master where vendor_part_number='" + data2[0] + "'";

                        DataTableReader rdd;
                        rdd = Exec.GetDtReader(get);
                        if (rdd.Read())
                        {

                        }

                        rdd.Close();
                    }

                }
                else
                {
                    err = "No Record Found";

                    return Str_Next_Location;

                }

                rd.Close();

                err = "Success";

                if (Str_Next_Location.Contains(','))
                {
                    string[] panam;
                    panam = Str_Next_Location.Split(',');
                    if (panam.Length > 1)
                    {
                        parnam = panam[0];

                        string sql = "select part_description from item_master where kai_part_number='" + panam[0] + "'";

                        DataTableReader rdd1;
                        rdd1 = Exec.GetDtReader(sql);
                        if (rdd1.Read())
                        {
                            parnam = rdd1.GetString(0);

                        }

                        rdd1.Close();

                    }
                }

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }

            return Str_Next_Location;

        }

        public bool Update_PickList_Skip(string Str_PickList_No, string PartNo, string Str_Pick_By, string Str_WH_Id, string Str_MAS_WH_Id, ref string Err)
        {

            try
            {

                Err = string.Empty;

                DateTime dt = DateTime.Now;

                string sql = "update  PICKING_MASTER set SKIP_STATUS= SKIP_STATUS +1 where  PICKLIST_NO='" + Str_PickList_No + "'" +
                    "and  PART_NUMBER='" + PartNo + "' and  PICKING_USER ='" + Str_Pick_By + "' and MASTER_WH_LOC_ID='" + Str_MAS_WH_Id + "'";

                Exec.ExecuteNonQuery(sql);
                Err = sql;

                return true;
            }
            catch (Exception ex)
            {
                Err = Err + ex.Message;
                return false;
            }
        }

        public DataTable download_PickList_Only(string Str_UserName, string Str_WH_ID, string Str_MAS_WH_ID, ref string Str_Next_Location, ref string err)
        {

            string strSql;

            DataTable dtsen = new DataTable();
            try
            {

                strSql = "SELECT DISTINCT PICKLIST_NO,DEALER_CODE FROM PICKING_MASTER  where  PICKING_STATUS=0  and BAL_QTY <>0  and PICKING_USER='" + Str_UserName + "' and  MASTER_WH_LOC_ID ='" + Str_MAS_WH_ID + "'  ORDER BY PICKLIST_NO ";

                dtsen = Exec.GetDataTable(strSql);
                dtsen.TableName = "PICKLIST_NO";
                err = "Success";

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }

            return dtsen;
        }

        public string getpendpick(string str_user, string wh_loc, string Mast_wh_loc)
        {

            try
            {

                string strOutput = string.Empty;

                string sql = "SELECT PICKLIST_NO FROM PICK_PENDING_DETAILS WHERE PICK_USER='" + str_user + "' AND flag='0' and MWHLOC='" + Mast_wh_loc + "' AND WHLOC='" + wh_loc + "' order by BIN";

                DataTableReader rd = Exec.GetDtReader(sql);
                while (rd.Read())
                {
                    strOutput = strOutput + rd.GetString(0) + ",";
                }

                if (strOutput.Trim() != "")
                    strOutput = strOutput.Substring(0, strOutput.Length - 1);

                rd.Close();

                return strOutput;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


    }
}
