using E_Contract.Repository;
using E_Contract.Service.Dictionary;
using E_Model.Table_SQL.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.Dictionary
{
    public class DataDictionaryService : ServiceBase<data_dictionary>, IDataDictionaryService
    {
        public DataDictionaryService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.DataDictionary;
        }
    }
}
