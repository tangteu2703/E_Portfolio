using E_Contract.Repository.Authentication;
using E_Contract.Repository.Dictionary;
using E_Contract.Repository.User;

namespace E_Contract.Repository
{
    public interface IRepositoryWrapper
    {
        #region Authen

        ITokenRepository Token { get; }
        IDataApiRepository DataApi { get; }
        IDataApplicationRepository DataApplication { get; }
        ISysLdapSettingRepository SysLdapSetting { get; }
        ISysRefreshTokenRepository SysRefreshToken { get; }

        #endregion Authen
        #region User

        IDataTitleRepository DataTitle { get; }
        IDataDepartmentRepository DataDepartment { get; }
        IDataUserRepository DataUser { get; }
        IDataDictionaryRepository DataDictionary { get; }

        #endregion User
    }
}