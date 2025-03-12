using E_Contract.Repository;
using E_Contract.Repository.Authentication;
using E_Contract.Repository.Dictionary;
using E_Contract.Repository.User;
using E_Repository.Authentication;
using E_Repository.Dictionary;
using E_Repository.User;

namespace E_Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        #region Authen

        private ITokenRepository _tokenRepository;
        public ITokenRepository Token => _tokenRepository ??= new TokenRepository();

        private IDataApiRepository _dataApiRepository;
        public IDataApiRepository DataApi => _dataApiRepository ??= new DataApiRepository();
       
        private ISysLdapSettingRepository _sysLdapSetting;
        public ISysLdapSettingRepository SysLdapSetting => _sysLdapSetting ??= new SysLdapSettingRepository();

        private ISysRefreshTokenRepository _sysRefreshToken;
        public ISysRefreshTokenRepository SysRefreshToken => _sysRefreshToken ??= new SysRefreshTokenRepository();

        #endregion Authen

        #region User
        private IDataUserRepository _dataUserRepository;
        public IDataUserRepository DataUser => _dataUserRepository ??= new DataUserRepository();

        private IDataApplicationRepository _dataApplication;
        public IDataApplicationRepository DataApplication => _dataApplication ??= new DataApplicationRepository();

        private IDataTitleRepository _dataTitle;
        public IDataTitleRepository DataTitle => _dataTitle ??= new DataTitleRepository();

        private IDataDepartmentRepository _dataDepartment;
        public IDataDepartmentRepository DataDepartment => _dataDepartment ??= new DataDepartmentRepository();

        private IDataDictionaryRepository _dataDictionary;
        public IDataDictionaryRepository DataDictionary => _dataDictionary ??= new DataDictionaryRepository();
        #endregion

    }
}