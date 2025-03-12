using E_Contract.Service.Authentication;
using E_Contract.Service.Dictionary;
using E_Contract.Service.User;

namespace E_Contract.Service
{
    public interface IServiceWrapper
    {
        #region Authentication

        ITokenService TokenService { get; }
        IDataApiService DataApi { get; }
        IDataApplicationService DataApplication { get; }
        ISysRefreshTokenService SysRefreshToken { get; }

        #endregion Authentication

        #region User

        IDataUserService DataUser { get; }
        IDataDepartmentService DataDepartment { get; }
        IDataDictionaryService DataDictionary { get; }

        #endregion User

    }
}