using E_Contract.Repository;
using E_Contract.Repository.Authentication;
using E_Contract.Repository.Department;
using E_Contract.Repository.Device;
using E_Contract.Repository.Dictionary;
using E_Contract.Repository.Hosted;
using E_Contract.Repository.News;
using E_Contract.Repository.User;
using E_Contract.Repository.WorkSheet;
using E_Repository.Authentication;
using E_Repository.Department;
using E_Repository.Device;
using E_Repository.Dictionary;
using E_Repository.Hosted;
using E_Repository.News;
using E_Repository.User;
using E_Repository.WorkSheet;

namespace E_Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        #region Authen

        private IMenuRepository _menu;
        public IMenuRepository Menu => _menu ??= new MenuRepository();

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

        #region workSheet

        private IWorkSheetRepository _workSheet;
        public IWorkSheetRepository WorkSheet => _workSheet ??= new WorkSheetRepository();

        private IClockTransactionRepository _clockTransaction;
        public IClockTransactionRepository ClockTransaction => _clockTransaction ??= new ClockTransactionRepository();
        #endregion

        #region MasterData

        private IDepartmentRepository _Department;
        public IDepartmentRepository Department => _Department ??= new DepartmentRepository();
        #endregion

        #region Task Job

        private ITaskHistoriedRepository _TaskHistoried;
        public ITaskHistoriedRepository TaskHistoried => _TaskHistoried ??= new TaskHistoriedRepository();

        #endregion
        #region Device
        private IDeviceManagementRepository _DeviceManagement;
        public IDeviceManagementRepository DeviceManagement => _DeviceManagement ??= new DeviceManagementRepository();

        private IDeviceTypeRepository _DeviceType;
        public IDeviceTypeRepository DeviceType => _DeviceType ??= new DeviceTypeRepository();
        #endregion

        #region News
        private IDataNewsRepository _News;
        public IDataNewsRepository News => _News ??= new DataNewsRepository();
        #endregion

    }
}