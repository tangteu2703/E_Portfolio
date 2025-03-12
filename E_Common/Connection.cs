using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace E_Common
{
    public static class Connection
    {
        private static Dictionary<string, string> ConnectionStrings = new Dictionary<string, string>();
        private static string LDapServer = "";
        private static int LDapPort = 0;

        public static void AddConnectionString(string ConnectionName, string ConnectionString)
        {
            ConnectionStrings.Add(ConnectionName, ConnectionString);
        }

        public static void AddLDapInfoString(string lDapServer, int lDapPort)
        {
            LDapServer = lDapServer;
            LDapPort = lDapPort;
        }

        public static IDbConnection getConnection(string ConnectionName)
        {
            var ConnectionString = ConnectionStrings[ConnectionName];
            IDbConnection _db = new SqlConnection(ConnectionString);
            return _db;
        }

        /// <summary>
        /// Thực thi store procedure
        /// </summary>
        /// <param name="StoreProcedureName">Tên store</param>
        /// <param name="param">DynamicParameters</param>
        public static async Task<bool> ExecuteAsync(string StoreProcedureName, DynamicParameters param, string ConnectionName = "CMSConnection")
        {
            await Connection.getConnection(ConnectionName).ExecuteAsync(StoreProcedureName, param, commandType: CommandType.StoredProcedure);
            return true;
        }

        /// <summary>
        /// Thực thi querry
        /// </summary>
        /// <param name="StoreProcedureName">Tên store</param>
        /// <param name="param">DynamicParameters</param>
        public static async Task<bool> ExcuteQuerryAsync(string Querry, string ConnectionName = "CMSConnection")
        {
            await Connection.getConnection(ConnectionName).ExecuteAsync(Querry, null, commandType: CommandType.Text);
            return true;
        }

        /// <summary>
        /// Thực thi store procedure và trả về return value của store (Thường dùng để lấy ID tự tăng -> Store phải return value)
        /// </summary>
        /// <param name="StoreProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="returnValueParamName"></param>
        /// <returns></returns>
        public static async Task<int> ExcuteScalarAsync(string StoreProcedureName, DynamicParameters param, string returnValueParamName, string ConnectionName = "CMSConnection")
        {
            await Connection.getConnection(ConnectionName).ExecuteAsync(StoreProcedureName, param, commandType: CommandType.StoredProcedure);
            return param.Get<int>(returnValueParamName);
        }

        public static async Task<IEnumerable<T>> SelectAsync<T>(string StoreProcedueName, DynamicParameters param = null, string ConnectionName = "CMSConnection")
        //where T : class
        {
            var result = await SqlMapper.QueryAsync<T>(Connection.getConnection(ConnectionName), StoreProcedueName, param, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public static async Task<IEnumerable<T>> SelectByQueryAsync<T>(string Querry, string ConnectionName = "CMSConnection")
         where T : class
        {
            var result = await SqlMapper.QueryAsync<T>(Connection.getConnection(ConnectionName), Querry, commandType: System.Data.CommandType.Text);
            return result;
        }

        /// <summary>
        /// Thực thi store procedure
        /// </summary>
        /// <param name="StoreProcedureName">Tên store</param>
        /// <param name="param">DynamicParameters</param>
        public static void Excute(string StoreProcedureName, DynamicParameters param, string ConnectionName = "CMSConnection")
        {
            Connection.getConnection(ConnectionName).Execute(StoreProcedureName, param, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Thực thi querry
        /// </summary>
        /// <param name="StoreProcedureName">Tên store</param>
        /// <param name="param">DynamicParameters</param>
        public static void ExcuteQuerry(string Querry, string ConnectionName = "CMSConnection")
        {
            Connection.getConnection(ConnectionName).Execute(Querry, null, commandType: CommandType.Text);
        }

        /// <summary>
        /// Thực thi store procedure và trả về return value của store (Thường dùng để lấy ID tự tăng -> Store phải return value)
        /// </summary>
        /// <param name="StoreProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="returnValueParamName"></param>
        /// <returns></returns>
        public static int ExcuteScalar(string StoreProcedureName, DynamicParameters param, string returnValueParamName, string ConnectionName = "CMSConnection")
        {
            Connection.getConnection(ConnectionName).Execute(StoreProcedureName, param, commandType: CommandType.StoredProcedure);
            return param.Get<int>(returnValueParamName);
        }

        public static List<T> Select<T>(string StoreProcedueName, DynamicParameters param = null, string ConnectionName = "CMSConnection")
         where T : new()
        {
            var result = SqlMapper.Query<T>(Connection.getConnection(ConnectionName), StoreProcedueName, param, commandType: System.Data.CommandType.StoredProcedure).ToList<T>();
            return result;
        }

        public static List<T> SelectByQuery<T>(string Querry, string ConnectionName = "CMSConnection")
         where T : new()
        {
            var result = SqlMapper.Query<T>(Connection.getConnection(ConnectionName), Querry, commandType: System.Data.CommandType.Text).ToList<T>();
            return result;
        }

        //Mutil table
        public static async Task<SqlMapper.GridReader> SelectMultipleAsync(string StoreProcedueName, DynamicParameters param = null, string ConnectionName = "CMSConnection")
        //where T : class
        {
            var result = await SqlMapper.QueryMultipleAsync(Connection.getConnection(ConnectionName), StoreProcedueName, param, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }
    }
}