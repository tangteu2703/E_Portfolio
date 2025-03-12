using Dapper;

namespace E_Common
{
    public static class ExceptionHelper
    {
        private static string m_exePath = string.Empty;

        public static void Write(string logMessage, string URL = "")
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@exception_message", logMessage);
                param.Add("@created_time", DateTime.Now);
                param.Add("@url", URL);
                Connection.Excute("Exception_Insert", param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SaveLog(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  :");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SaveLog(this Exception ex, string URL = "")
        {
            var m = ex.Message;
            ExceptionHelper.Write(ex.Message, URL);
        }
    }
}