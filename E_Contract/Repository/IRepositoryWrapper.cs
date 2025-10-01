using E_Contract.Repository.Authentication;
using E_Contract.Repository.Department;
using E_Contract.Repository.Device;
using E_Contract.Repository.Dictionary;
using E_Contract.Repository.Hosted;
using E_Contract.Repository.User;
using E_Contract.Repository.WorkSheet;
using E_Contract.Service.Department;
using E_Contract.Service.WorkSheet;

namespace E_Contract.Repository
{
    public interface IRepositoryWrapper
    {
        #region Authen

        IMenuRepository Menu { get; }
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

        #region WorkSheet
        IWorkSheetRepository WorkSheet{ get; }
        IClockTransactionRepository ClockTransaction { get; }
        #endregion

        #region MasterData
        IDepartmentRepository Department { get; }
        #endregion

        #region Hosted
        ITaskHistoriedRepository TaskHistoried { get; }
        #endregion
        #region Device
        IDeviceManagementRepository DeviceManagement { get; }
        IDeviceTypeRepository DeviceType { get; }
        #endregion
    }
}