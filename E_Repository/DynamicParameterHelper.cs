using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using E_Model;

namespace E_Repository
{
    public class DynamicParameterHelper
    {
        public static void AddModifyInsertParameters(DynamicParameters param, modify_info obj)
        {
            param.Add("is_deleted", obj.is_deleted);
            param.Add("updated_by", obj.updated_by);
            param.Add("updated_at", obj.updated_at);
        }

        public static void AddModifyUpdateParameters(DynamicParameters param, modify_info obj)
        {
            param.Add("updated_by", obj.updated_by);
            param.Add("updated_at", obj.updated_at);
        }

        /// <summary>
        /// Convert object về danh sách các parameter để truyền vào storedprocedure
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DynamicParameters ConvertAll(object obj)
        {
            var type = obj.GetType();
            var param = new DynamicParameters();
            var props = (IList<PropertyInfo>)type.GetProperties();
            foreach (var prop in props)
            {
                param.Add("@" + prop.Name, prop.GetValue(obj));
            }
            return param;
        }

        /// <summary>
        /// Convert object về danh sách các parameter để truyền vào storedprocedure - ngoại trừ các propery trong tham số withOutParams. Các param ngăn cách bằng dấu ","
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DynamicParameters ConvertWithOutParams(object obj, string withOutParams)
        {
            var insertParamNames = withOutParams.Split(',');
            var type = obj.GetType();
            var param = new DynamicParameters();
            var props = (IList<PropertyInfo>)type.GetProperties();
            foreach (var prop in props)
            {
                if (!insertParamNames.Contains(prop.Name))
                {
                    param.Add("@" + prop.Name, prop.GetValue(obj));
                }
            }
            return param;
        }

        public static DynamicParameters ConvertWithOutCreatitonParams(object obj, string withOutParams = "is_deleted,created_time,created_user_id")
        {
            return ConvertWithOutParams(obj, withOutParams);
        }

        /// <summary>
        /// Convert object về danh sách các parameter để truyền vào storedprocedure, trong đó add param returnField là giá trị trả về
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="returnField"></param>
        /// <returns></returns>
        public static DynamicParameters ConvertWithReturnParam(object obj, string returnField = "ID", string withoutParams = null)
        {
            var type = obj.GetType();
            var param = new DynamicParameters();
            var props = (IList<PropertyInfo>)type.GetProperties();
            var withoutParamNames = withoutParams?.Split(',').ToList() ?? new List<string>();

            foreach (var prop in props.Where(x => !withoutParamNames.Contains(x.Name)))
            {
                if (prop.Name == returnField)
                {
                    param.Add("@" + prop.Name, null, System.Data.DbType.Int64, System.Data.ParameterDirection.ReturnValue);
                }
                else
                {
                    param.Add("@" + prop.Name, prop.GetValue(obj));
                }
            }
            return param;
        }
    }
}