using E_Common;
using E_Contract.Repository.Device;
using E_Model.Request.Device;
using E_Model.Request.WorkSheet;
using E_Model.Response;
using E_Model.Response.Device;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.Device;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Repository.Device
{
    public class DeviceRequestRepository : RepositoryBase<DeviceManagement>, IDeviceRequestRepository
    {
        public async Task<DataTableResponse<DeviceManagementRespone>> SelectFilterAsync(DeviceRequest request)
        { 
            try
            {
                var data = new DataTableResponse<DeviceManagementRespone>();

                var param = request.ToDynamicParameters();
                param.Add("@recordsTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@recordsFiltered", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var storeName = "DeviceManagement_select_filter_page";

                var result = await Connection.SelectAsync<DeviceManagementRespone>(
                    storeName,
                    param,
                    "E_PortalConnection"
                );

                data.recordsTotal = param.Get<int>("@recordsTotal");
                data.recordsFiltered = param.Get<int>("@recordsFiltered");
                data.listData = result;

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       
    }
}
