using kWMS.Extras;
using System;
using System.Data;
using System.Linq;

namespace kWMS.Services
{
    public class DispatchManagement
    {
        Executer Exec = new Executer();
        public DispatchManagement()
        {

        }

        public DataTable download_TRANSPORTER_Mode(string Str_UserName, string Str_WH_ID, ref string err)
        {

            string strSql;

            DataTable dtsen = new DataTable();

            try
            {
                strSql = "SELECT    distinct SHIPMENT_MODE FROM TRANSPORTER_MASTER   where   WH_LOCATION_ID ='" + Str_WH_ID + "'  ORDER BY SHIPMENT_MODE ";

                dtsen = Exec.GetDataTable(strSql);
                err = "Success";

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }
            return dtsen;

        }

        public DataTable download_SHIPMENT_Dealer(string Str_UserName, string Str_WH_ID, string Str_Mode, ref string err)
        {

            string strSql;
            DataTable dtsen = new DataTable();

            try
            {
                strSql = "SELECT    distinct dealer_code FROM invoice_master  where   WH_LOCATION_ID ='" + Str_WH_ID + "'  and Bal_Qty<>0";

                dtsen = Exec.GetDataTable(strSql);
                err = "Success";

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }
            return dtsen;

        }


        public string comp_disp(string inv, string str_wh_loc, string str_mast_wh, ref string err)
        {

            try
            {
                string[] data;

                string dispsql = string.Empty;

                string whereCondition = string.Empty;
                if (inv.Contains(','))
                {
                    data = inv.Split(',');
                    for (int i = 0; i < data.Length; i++)
                    {
                        whereCondition = whereCondition + "PICKING_NO = '" + data[i].TrimEnd() + "' OR ";
                    }
                    whereCondition = whereCondition.Substring(0, whereCondition.Length - 3);
                }
                else
                {
                    whereCondition = "PICKING_NO = '" + inv.TrimEnd() + "'";
                }
                dispsql = "update INVOICE_MASTER SET FLAG='1',grp_flag='0' WHERE (" + whereCondition + ") AND WH_LOCATION_ID='" + str_wh_loc + "' and MASTER_WH_LOC_ID='" + str_mast_wh + "'";

                Exec.ExecuteNonQuery(dispsql);
                err = "Success";
            }
            catch (Exception ex)
            {
                err = ex.Message;
                throw;
            }
            return err;

        }

        public string Get_Box_Count_new(string pick, string Str_User_Name, string Str_WH_Id, string Str_Mas_WH_Id, string Str_Dealer_Code, string shipmode, string transp, string dock, ref int Int_Tot_Box, ref int Int_Bal_Box, ref string err)
        {

            DataTable dtsen = new DataTable();

            string Str_Output = string.Empty;
            string sql;

            try
            {

                string[] data;
                string order = string.Empty;
                string whereCondition = string.Empty;
                string whereCondition1 = string.Empty;
                if (pick.Contains(','))
                {
                    data = pick.Split(',');

                    for (int i = 0; i < data.Length; i++)
                    {
                        whereCondition = whereCondition + "PICKLIST_NO = '" + data[i].TrimEnd() + "' OR ";
                        whereCondition1 = whereCondition1 + "PICKING_NO = '" + data[i].TrimEnd() + "' OR ";
                    }

                    whereCondition = whereCondition.Substring(0, whereCondition.Length - 3);
                    whereCondition1 = whereCondition1.Substring(0, whereCondition1.Length - 3);
                }
                else
                {
                    whereCondition = "PICKLIST_NO = '" + pick.TrimEnd() + "'";
                    whereCondition1 = "PICKING_NO = '" + pick.TrimEnd() + "'";
                }

                sql = "select count(distinct box_unique) from TBL_BOX_MASTER where (" + whereCondition + ") and dealer_code='" + Str_Dealer_Code + "' and WH_LOC_ID='" + Str_WH_Id + "'";
                Int_Tot_Box = Convert.ToInt32(Exec.ExecuteScalar(sql));

                sql = "";
                sql = "select count(distinct box_unique) from TBL_BOX_MASTER where (" + whereCondition + ") and DISPATCH_SCAN_STATUS=0 and dealer_code='" + Str_Dealer_Code + "' and WH_LOC_ID='" + Str_WH_Id + "'";

                Int_Bal_Box = Convert.ToInt32(Exec.ExecuteScalar(sql));

                Str_Output = "1," + Int_Tot_Box + "," + Int_Bal_Box;
                sql = "";
                sql = "Update invoice_master set SHIPMENT_MODE='" + shipmode + "',SHIPMENT_TRANSPORTER='" + transp + "',Grp_Flag='1',Docket='" + dock + "' where (" + whereCondition1 + ") and WH_LOCATION_ID ='" + Str_WH_Id + "'";

                Exec.ExecuteNonQuery(sql);
                err = "Success";
            }
            catch (Exception ex)
            {
                err = ex.Message;
                throw;
            }
            return Str_Output;
        }

        public DataTable download_SHIPMENT_TRANSPORTER(string Str_UserName, string Str_WH_ID, string Str_Mode, ref string err)
        {

            string strSql;
            DataTable dtsen = new DataTable();

            try
            {
                strSql = "SELECT distinct SHIPMENT_TRANSPORTER FROM  TRANSPORTER_MASTER where  WH_LOCATION_ID ='" + Str_WH_ID + "'  and SHIPMENT_MODE='" + Str_Mode + "'  ORDER BY SHIPMENT_TRANSPORTER ";

                dtsen = Exec.GetDataTable(strSql);
                err = "Success";

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }
            return dtsen;

        }

        public String get_box_wt(string qrcode, string Str_User_Name, string Str_WH_Id, string Str_Mas_WH_Id, string Str_Dealer_Code, string pick, ref int Int_Tot_Box, ref int Int_Bal_Box, ref string err)
        {

            DataTableReader rd;
            DataTable dtsen = new DataTable();

            string Str_Output = string.Empty;

            try
            {

                string[] data;
                string sql;
                string order = string.Empty;
                string whereCondition = string.Empty;
                string whereCondition1 = string.Empty;

                if (pick.Contains(','))
                {
                    data = pick.Split(',');
                    for (int i = 0; i < data.Length; i++)
                    {
                        whereCondition = whereCondition + "PICKLIST_NO = '" + data[i].TrimEnd() + "' OR ";
                        whereCondition1 = whereCondition1 + "PICKING_NO = '" + data[i].TrimEnd() + "' OR ";
                    }
                    whereCondition = whereCondition.Substring(0, whereCondition.Length - 3);
                    whereCondition1 = whereCondition1.Substring(0, whereCondition1.Length - 3);
                }
                else
                {
                    whereCondition = "PICKLIST_NO = '" + pick.TrimEnd() + "'";
                    whereCondition1 = "PICKING_NO = '" + pick.TrimEnd() + "'";
                }

                sql = "";
                sql = "select BOX_WEIGHT from TBL_BOX_MASTER where (" + whereCondition + ") and BOX_UNIQUE='" + qrcode + "' and WH_LOC_ID='" + Str_WH_Id + "' ";

                rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    Str_Output = rd.GetValue(0).ToString();
                    err = "Success";

                    rd.Close();
                    sql = "";
                    sql = "Update invoice_master SET Grp_Flag='DSTART' where (" + whereCondition1 + ") and WH_LOCATION_ID='" + Str_WH_Id + "'";

                    Exec.ExecuteNonQuery(sql);
                }
                else
                {
                    err = "Invalid Box Scanned";
                }
                rd.Close();

            }
            catch (Exception ex)
            {
                err = ex.Message;
                throw;

            }
            return Str_Output;

        }

        public String Save_Dispatch_Box_new(string dispqr, string Str_User_Name, string fnlqr, string shipm, string transp, string Str_WH_Id, string Str_Mas_WH_Id, string Str_Dealer_Code, string Str_Box_No, string docketno, string boxc, string inv, string wt, string hht, string dea, string ship, ref string err)
        {

            DataTableReader rd;
            string Str_Output;
            DateTime dt = DateTime.Now;
            string box_count = string.Empty;
            string invo = string.Empty;
            string pickl = string.Empty;
            string sql;

            try
            {
                sql = "select picklist_no from tbl_box_master where BOX_UNIQUE='" + dispqr + "' and WH_LOC_ID='" + Str_WH_Id + "'";

                rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    pickl = rd.GetString(0);
                }

                rd.Close();
                sql = "";
                sql = "select invoice_no from invoice_master where picking_no='" + pickl.TrimEnd() + "' and dealer_code='" + dea + "'";

                rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    invo = rd.GetString(0).TrimEnd();
                }

                rd.Close();

                sql = "Update TBL_BOX_MASTER set DISPATCH_QR='" + fnlqr + "',SHIPMENT_TRANSPORTER='" + transp + "',SHIPMENT_MODE='" + shipm + "',docket_no='" + docketno + "',DISPATCH_DATE=getdate(), DISPATCH_SCAN_STATUS=1, DISPATCH_BY='" + Str_User_Name + "',DISP_BOX='" + boxc + "',DISPATCH_PRINT_STATUS='0',INVOICE_NO='" + invo.TrimEnd() + "',BOX_WEIGHT='" + wt + "',DISPATCH_DEVICE='" + hht + "',SHIPTO='" + ship + "' where BOX_UNIQUE='" + dispqr + "' and WH_LOC_ID='" + Str_WH_Id + "'";

                Exec.ExecuteNonQuery(sql);
                Str_Output = "Success";
                err = "Success";
            }
            catch (Exception ex)
            {
                err = ex.Message;
                throw;
            }

            return Str_Output;

        }

        public String valid_box_qrcode(string boxqr, string Str_User_Name, string Str_WH_Id, string Str_Mas_WH_Id, string Str_Dealer_Code, string Str_Box_No, ref int Int_Scan_Status, ref string err)
        {

            DataTableReader rd;
            string sql;

            string Str_Output;

            try
            {

                sql = "select distinct DISPATCH_SCAN_STATUS from TBL_BOX_MASTER where  BOX_UNIQUE='" + boxqr + "' and WH_LOC_ID='" + Str_WH_Id + "' ";

                rd = Exec.GetDtReader(sql);

                if (rd.Read())
                {
                    err = "Success";
                    Int_Scan_Status = Convert.ToInt32(rd.GetValue(0));
                    Str_Output = "1," + Convert.ToString(Int_Scan_Status);
                }
                else
                {
                    err = "Invalid Box No";
                    Str_Output = "0";
                }

                rd.Close();

            }
            catch (Exception ex)
            {
                err = ex.ToString();
                throw;
            }
            return Str_Output;

        }

        public string chk_deal(string dealer, ref string stro, ref string err)
        {

            try
            {
                string sql = "Select * from invoice_master where dealer_code='" + dealer + "' and FLAG=0";

                DataTableReader rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {

                    err = "Success";
                }
                else
                {

                    err = "Invalid Dealer Code";
                }

                rd.Close();

                return err;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                throw;

            }

        }

        public string chk_disp_inv(string dealer, string invoice, ref string ord, ref string err, ref string ship, ref string de)
        {

            DataTableReader rd;
            string sql;
            try
            {
                string dea = string.Empty;
                sql = "Select Dealer_code from invoice_master where invoice_no='" + invoice + "' and FLAG=0";
                rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    dea = rd.GetString(0);
                    de = dea.TrimEnd();
                    err = "Success";
                }
                else
                {
                    err = "Already Completed Dispatch Completed this Invoice";
                }

                rd.Close();
                sql = "";
                sql = "Select isnull(shipto,'A') from invoice_master where invoice_no='" + invoice + "' and FLAG=0";

                rd = Exec.GetDtReader(sql);
                if (rd.Read())
                {
                    ship = rd.GetString(0).TrimEnd();

                    err = "Success";
                }
                else
                {
                    err = "Already Completed Dispatch Completed this Invoice";
                }

                rd.Close();

                sql = "";
                sql = "Select distinct PICKING_NO from invoice_master where dealer_code='" + de.TrimEnd() + "' and shipTo='" + ship.TrimEnd() + "' and invoice_no='" + invoice + "' and FLAG=0";

                rd = Exec.GetDtReader(sql);

                while (rd.Read())
                {
                    ord = ord + rd.GetString(0).TrimEnd() + ",";
                }

                rd.Close();

                if (ord.Contains(','))
                {
                    ord = ord.Substring(0, ord.Length - 1);
                }

                return err;

            }
            catch (Exception ex)
            {
                err = ex.Message;
                return err;
            }

        }

        public DataTableReader Get_Dealer(string Mast_Inv_No)
        {
            DataTableReader rd;

            string sql = "SELECT DISTINCT DEALER_CODE FROM INVOICE_MASTER WHERE INVOICE_NO='" + Mast_Inv_No + "'";

            rd = Exec.GetDtReader(sql);
            return rd;
        }


        public DataTableReader Get_Lbl_Dtls(string Box_Unique, string Mast_Wh_Loc)
        {
            DataTableReader rd;
            string sql = "Select DISTINCT dealer_code,Box_unique,picklist_no,Box_Weight,Box,Disp_box,DISPATCH_QR,invoice_no,shipment_transporter,shipment_mode,Docket_no,shipto from  tbl_box_master(nolock) where BOX_UNIQUE='" + Box_Unique + "' AND PRINT_status=1 and DISPATCH_SCAN_STATUS=1 AND MAST_WH_LOC_ID='" + Mast_Wh_Loc + "' AND DISPATCH_PRINT_STATUS =0 ";
            rd = Exec.GetDtReader(sql);
            return rd;
        }

        public DataTableReader Get_Dealer_Catlog(string Str_Dealer_Code, string ship)
        {
            DataTableReader rd;
            string strSql;
            if (ship == "")
            {
                strSql = "SELECT * FROM  DEALER_CATALOG(nolock) where DEALER_CODE ='" + Str_Dealer_Code + "'";
            }
            else
            {
                strSql = "SELECT * FROM  DEALER_CATALOG(nolock) where DEALER_CODE ='" + Str_Dealer_Code + "' and ship_to_code='" + ship + "'";
            }
            rd = Exec.GetDtReader(strSql);
            return rd;
        }

        public object GetBoxCnt(string Str_Dealer_Code, string Mas_WH_ID)
        {
            object Res = null;
            string sql = "Select count(*) from  tbl_box_master(nolock) where dealer_code='" + Str_Dealer_Code + "'and MAST_WH_LOC_ID='" + Mas_WH_ID + "'";
            Res = Exec.ExecuteScalar(sql);
            return Res;
        }

        public bool Updt_Lbl_sts(string Str_Fnl_Frp_No)
        {
            bool b = false;
            string sql = "UPDATE TBL_BOX_MASTER SET DISPATCH_PRINT_STATUS = 1 WHERE DISPATCH_QR ='" + Str_Fnl_Frp_No + "'";
            b = Exec.ExecuteNonQuery(sql);
            return b;
        }

    }
}
