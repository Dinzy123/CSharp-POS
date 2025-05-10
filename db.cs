using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Shop
{
    class db
    {
        // Use the correct connection string for SQL Server
        public static string connstr = @"Data Source=Dinzy;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;";
     
        public static SqlConnection con = new SqlConnection(connstr);

        public static void openconnection()
        {
            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                {
                    con.Open();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Database Connection Failed");
                Console.WriteLine(e);
            }
        }

        public static void closeconnection()
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }
    }
}
