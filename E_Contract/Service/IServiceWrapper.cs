using E_Contract.Service.AI;
using E_Contract.Service.Authentication;
using E_Contract.Service.Department;
using E_Contract.Service.Device;
using E_Contract.Service.Dictionary;
using E_Contract.Service.Hosted;
using E_Contract.Service.User;
using E_Contract.Service.WorkSheet;

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

        #region WorkSheet
        IWorkSheetService WorkSheet { get; }
        IClockTransactionService ClockTransaction { get; }
        #endregion

        #region MasterData
        IDepartmentService Department { get; }
        #endregion

        #region Hosted
        ITaskHistoriedService TaskHistoried { get; }
        #endregion

        #region Device
        IDeviceTypeService DeviceType { get; }
        IDeviceManagementService DeviceManagement { get; }
        #endregion

    }
}