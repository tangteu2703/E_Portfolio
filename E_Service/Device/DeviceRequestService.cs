using DocumentFormat.OpenXml.Bibliography;
using E_Contract.Repository;
using E_Contract.Service.Device;
using E_Model.Request.Device;
using E_Model.Request.WorkSheet;
using E_Model.Response;
using E_Model.Response.Device;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.Device
{
    public class DeviceRequestService :ServiceBase<DeviceManagement>, IDeviceRequestService
    {
        public DeviceRequestService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.DeviceManagement;
        }

        public async Task<DataTableResponse<DeviceManagementRespone>> SelectFilterAsync(DeviceRequest request)
        {
            return await _repositoryWrapper.DeviceManagement.SelectFilterAsync(request);
        }

    }
}
