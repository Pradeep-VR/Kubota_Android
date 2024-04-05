namespace kWMS.Extras
{
    class clsConnection
    {
        public static string User_Type = string.Empty;
        public static string user = string.Empty;
        public static string user_WH_ID = string.Empty;
        public static string user_Admin_ID = string.Empty;
        public static string User_Mas_WH_ID = string.Empty;
        public static string User_Main_Caption = string.Empty;
        public static string HHT_Serial_Number = string.Empty;
        public static string INV_Number = string.Empty;
        public static string IP_Address = string.Empty;
        public static string Printer_Name = string.Empty;

        public static string Copyrights = " © " + System.DateTime.Now.Year + " Teamliftss It System" ;

        public static string Load = "Loading Please Wait...";

        //Global_Connection String TEAMLIFTS server.
        //UN = 9171;Pass Raja@06333
        //public static string sName = "125.20.158.150";
        //public static string sdbName = "TEST_WMS_WIN";
        //public static string sUserName = "sa";
        //public static string sPassword = "Welcome@123";

        //Global_Connection String KUBOTA Main Server
        //UN = 9171;Pass Raja@068
        public static string sName = "172.20.243.139\\wms1sql";
        public static string sdbName = "KUBOTA_WMS_WIN";
        public static string sUserName = "sa";
        public static string sPassword = "#23130082020Q";

        //Global_Connection String KUBOTA Test Server.

        //public static string sName = "172.20.243.158\\WMS1SQL";
        //public static string sdbName = "TEST_WMS_WIN";
        //public static string sUserName = "sa";
        //public static string sPassword = "#4521550223S";

        //Global_Connection String KUBOTA Test Server.2

        //public static string sName = "172.20.243.183\\WMS1SQL";
        //public static string sdbName = "TEST2_WMS_WIN";
        //public static string sUserName = "sa";
        //public static string sPassword = "#292023Mayqwe";
    }
}
