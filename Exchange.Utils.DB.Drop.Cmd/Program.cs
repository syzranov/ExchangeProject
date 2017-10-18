using System;
using System.Data.SqlClient;

namespace Exchange.Utils.DB.Drop.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DAL.Managers.DalManager.Instance.DbManager.DeleteDb(
                    ConfigTools.Instance.Get<string>("connection-string"));
            }
            catch (SqlException exc)
            {
                Console.WriteLine(exc);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }
    }
}
