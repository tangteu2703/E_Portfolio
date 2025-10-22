using E_Model.Request.Device;
using E_Model.Response;
using E_Model.Response.Device;
using E_Model.Table_SQL.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Contract.Repository.Device
{
    public interface IDeviceRequestRepository : IRepositoryBase<DeviceManagement>
    {
        Task<DataTableResponse<DeviceManagementRespone>> SelectFilterAsync(DeviceRequest request);
    }
}
