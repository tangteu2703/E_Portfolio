using E_Common;
using E_Contract.Repository;
using E_Contract.Service;
using E_Contract.Service.Authentication;
using E_Contract.Service.Dictionary;
using E_Contract.Service.User;
using E_Service.Authentication;
using E_Service.Dictionary;
using E_Service.User;

namespace E_Service
{
    public class ServiceWrapper : IServiceWrapper
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly JwtHelper _jwtHelper;

        public ServiceWrapper(IRepositoryWrapper repositoryWrapper, JwtHelper jwtHelper)
        {
            _repositoryWrapper = repositoryWrapper ?? throw new ArgumentNullException(nameof(repositoryWrapper));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
        }

        #region Authen


        private Lazy<IDataApiService> _dataApi => new(() => new DataApiService(_repositoryWrapper));
        public IDataApiService DataApi => _dataApi.Value;

        private Lazy<ITokenService> _tokenService => new(() => new TokenService(_repositoryWrapper, _jwtHelper));
        public ITokenService TokenService => _tokenService.Value;

        private Lazy<IDataApplicationService> _dataApplication => new(() => new DataApplicationService(_repositoryWrapper));
        public IDataApplicationService DataApplication => _dataApplication.Value;
        
        private Lazy<ISysRefreshTokenService> _sysRefreshToken => new(() => new SysRefreshTokenService(_repositoryWrapper));
        public ISysRefreshTokenService SysRefreshToken => _sysRefreshToken.Value;

        #endregion Authen

        #region User
        private Lazy<IDataUserService> _dataUser => new(() => new DataUserService(_repositoryWrapper));
        public IDataUserService DataUser => _dataUser.Value;
        private Lazy<IDataDepartmentService> _dataDepartment => new(() => new DataDepartmentService(_repositoryWrapper));
        public IDataDepartmentService DataDepartment => _dataDepartment.Value;
        private Lazy<IDataDictionaryService> _dataDictionary => new(() => new DataDictionaryService(_repositoryWrapper));
        public IDataDictionaryService DataDictionary => _dataDictionary.Value;
        #endregion

    }
}