using E_Model.Authentication;
using E_Model.Request;
using E_Model.Request.Token;
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

namespace E_Contract.Service.User
{
    public interface IDataUserService : IServiceBase<UserData>
    {
        Task<IEnumerable<UserResponse>> SelectFilterAsync(UserRequest request);

        Task<UserData> SelectUserByEmailAsync(string email);
    }
}
