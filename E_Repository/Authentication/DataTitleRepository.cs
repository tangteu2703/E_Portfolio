using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Common;
using E_Contract.Repository.Authentication;
using E_Model.Authentication;

namespace E_Repository.Authentication
{
    public class DataTitleRepository : RepositoryBase<data_title>, IDataTitleRepository
    {
        public async Task<data_title> SelectByTitleNameAsync(string title)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@title", title);
                var result = await Connection.SelectAsync<data_title>("data_title_select_by_title", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
