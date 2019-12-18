using System;
using System.Collections;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataVault.Common.Optional
{
    public static class ObjectUtil
    {
        /// <summary>
        /// Determines any object is null or empty
        /// </summary>
        /// <param name="dynamicObj"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(object dynamicObj)
        {
            if (dynamicObj == null || dynamicObj == DBNull.Value || dynamicObj == (object)""
                || dynamicObj is IList && (dynamicObj as IList).Count == 0)
            {
                return true;
            }

            var dynamicObj_ = JToken.Parse(JsonConvert.SerializeObject(dynamicObj));
            return dynamicObj_ is JObject && dynamicObj_.Count() == 0;
        }
    }
}