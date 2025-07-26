using E_Model.Request.Device;
using E_Model.Request.WorkSheet;
using E_Model.Response;
using E_Model.Response.Device;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.Device;

namespace E_Contract.Service.Device
{
    public interface IDeviceManagementService : IServiceBase<DeviceManagement>
    {
        Task<DataTableResponse<DeviceManagementRespone>> SelectFilterAsync(DeviceRequest request);
    }
}
