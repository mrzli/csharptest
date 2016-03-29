using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string sql = "Alter Database master SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                sql += "Restore Database master FROM DISK ='" + @"D:\Devlab\Data\HeadofficeLite_Lukoil.bak" + "' WITH REPLACE;";

                string connectionString = ConfigurationManager.ConnectionStrings["HeadofficeDb"].ConnectionString;
                //string connectionString = "Data Source =.; Initial Catalog = master; Integrated Security = True";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(sql, con);
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }
    }
}
