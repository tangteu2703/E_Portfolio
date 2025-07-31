using DocumentFormat.OpenXml.Office2016.Excel;
using E_Contract.Repository;
using E_Contract.Service.User;
using E_Model.Authentication;
using E_Model.Request;
using E_Model.Request.User;
using E_Model.Request.WorkSheet;
using E_Model.Response.User;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.User
{
    public class DataUserService : ServiceBase<UserData>, IDataUserService
    {
        public DataUserService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.DataUser;
        }
        public async Task<IEnumerable<UserResponse>> SelectFilterAsync(UserRequest request)
        {
            return await _repositoryWrapper.DataUser.SelectFilterAsync(request);
        }

        public async Task<UserData> SelectUserByEmailAsync(string email)
        {
            return await _repositoryWrapper.DataUser.SelectUserByEmailAsync(email);
        }
    }
}
