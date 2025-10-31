using E_Common;
using E_Contract.Repository;
using E_Contract.Service;
using E_Contract.Service.Authentication;
using E_Contract.Service.Department;
using E_Contract.Service.Device;
using E_Contract.Service.Dictionary;
using E_Contract.Service.Hosted;
using E_Contract.Service.Kiot;
using E_Contract.Service.News;
using E_Contract.Service.User;
using E_Contract.Service.WorkSheet;
using E_Model.Table_SQL.Hosted;
using E_Service.Authentication;
using E_Service.Department;
using E_Service.Device;
using E_Service.Dictionary;
using E_Service.Hosted;
using E_Service.Kiot;
using E_Service.News;
using E_Service.User;
using E_Service.WorkSheet;

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


        private Lazy<IMenuService> _menu => new(() => new MenuService(_repositoryWrapper));
        public IMenuService Menu => _menu.Value;

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

        #region WorkSheet
        private Lazy<WorkSheetService> _workSheet => new(() => new WorkSheetService(_repositoryWrapper));
        public IWorkSheetService WorkSheet => _workSheet.Value;
        private Lazy<ClockTransactionService> _clock => new(() => new ClockTransactionService(_repositoryWrapper));
        public IClockTransactionService ClockTransaction => _clock.Value;
        #endregion

        #region MasterData
        private Lazy<DepartmentService> _Department => new(() => new DepartmentService(_repositoryWrapper));
        public IDepartmentService Department => _Department.Value;
        #endregion

        #region Hosted
        private Lazy<TaskHistoriedService> _TaskHistoried => new(() => new TaskHistoriedService(_repositoryWrapper));
        public ITaskHistoriedService TaskHistoried => _TaskHistoried.Value;

        #endregion

        #region Device
        private Lazy<DeviceManagementService> _DeviceManagement => new(() => new DeviceManagementService(_repositoryWrapper));
        public IDeviceManagementService DeviceManagement => _DeviceManagement.Value;
        private Lazy<DeviceTypeService> _DeviceType => new(() => new DeviceTypeService(_repositoryWrapper));
        public IDeviceTypeService DeviceType => _DeviceType.Value;
        private Lazy<DeviceRequestService> _DeviceRequest => new(() => new DeviceRequestService(_repositoryWrapper));
        public IDeviceRequestService DeviceRequest => _DeviceRequest.Value;
        #endregion

        #region News
        private Lazy<DataNewsService> _News => new(() => new DataNewsService(_repositoryWrapper));
        public IDataNewsService News => _News.Value;
        #endregion

        #region Kiot

        private Lazy<KiotCategoryService> _KiotCategory => new(() => new KiotCategoryService(_repositoryWrapper));
        public IKiotCategoryService KiotCategory => _KiotCategory.Value;

        private Lazy<KiotMenuService> kiotMenu => new(() => new KiotMenuService(_repositoryWrapper));
        public IKiotMenuService KiotMenu => kiotMenu.Value;
        #endregion

    }
}