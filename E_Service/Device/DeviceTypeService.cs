using E_Contract.Repository;
using E_Contract.Service.Device;
using E_Model.Table_SQL.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.Device
{
    public class DeviceTypeService : ServiceBase<DeviceType>, IDeviceTypeService
    { 
        public DeviceTypeService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.DeviceType;
        }
    }
}
