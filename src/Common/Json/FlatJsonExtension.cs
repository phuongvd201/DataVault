using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DataVault.Common.Extensions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataVault.Common.Json
{
    public static class FlatJsonExtension
    {
        public static string ToNestedJson(this string input)
        {
            var token = JToken.Parse(input);

            if (token is JObject obj)
            {
                return ConvertJObject(obj).ToString();
            }

            if (token is JArray array)
            {
                return ConvertArray(array).ToString();
            }

            return input;
        }

        private static JObject ConvertJObject(JObject input)
        {
            var enumerable = ((IEnumerable<KeyValuePair<string, JToken>>)input).OrderBy(kvp => kvp.Key);

            var result = new JObject();
            foreach (var outerField in enumerable)
            {
                var key = outerField.Key;
                var value = outerField.Value;

                if (value is JArray array)
                {
                    value = ConvertArray(array);
                }

                var fieldNames = key.Split('.');
                var currentObj = result;

                for (var fieldNameIndex = 0; fieldNameIndex < fieldNames.Length; fieldNameIndex++)
                {
                    var fieldName = fieldNames[fieldNameIndex];

                    if (fieldNameIndex == fieldNames.Length - 1)
                    {
                        currentObj[fieldName] = value;
                        continue;
                    }

                    if (currentObj.ContainsKey(fieldName))
                    {
                        currentObj = (JObject)currentObj[fieldName];
                        continue;
                    }

                    var newObj = new JObject();
                    currentObj[fieldName] = newObj;
                    currentObj = newObj;
                }
            }

            return result;
        }

        private static JArray ConvertArray(JArray array)
        {
            var resultArray = new JArray();

            foreach (var arrayItem in array)
            {
                if (!(arrayItem is JObject))
                {
                    resultArray.Add(arrayItem);
                    continue;
                }

                var itemObj = (JObject)arrayItem;

                resultArray.Add(ConvertJObject(itemObj));
            }

            return resultArray;
        }

        public static Dictionary<string, object> ToJsonFormat(this string input)
        {
            var token = JToken.Parse(input);

            if (token is JObject obj)
            {
                return ConvertJObjectToFlat(obj);
            }

            return new Dictionary<string, object>();
        }

        public static T GetValue<T>(this JToken jToken, string key, T defaultValue = default(T))
        {
            dynamic ret = jToken[key];
            if (ret == null) return defaultValue;
            if (ret is JObject) return JsonConvert.DeserializeObject<T>(ret.ToString());
            return (T)ret;
        }

        private static Dictionary<string, object> ConvertJObjectToFlat(JObject input)
        {
            var enumerable = ((IEnumerable<KeyValuePair<string, JToken>>)input).OrderBy(kvp => kvp.Key);

            Dictionary<string, object> listInput = new Dictionary<string, object>();

            var result = new JObject();
            foreach (var outerField in enumerable)
            {
                var values = outerField.Value;
                foreach (var value in values)
                {
                    listInput.Add(value.Path, "");
                }
            }

            return listInput;
        }

        public static string DictionaryToJson(this Dictionary<string, string> dict)
        {
            var entries = dict.Select(
                d =>
                    string.Format("\"{0}\": {1}", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }

        public static string ToArrayJson(this Dictionary<string, string> dict)
        {
            string entries = string.Empty;
            foreach (var d in dict)
            {
                if (d.Key.Contains("[]."))
                {
                    string[] array = d.Key.Split("[].");
                    entries = string.Format("\"{0}\": {1}", array[1], string.Join(",", d.Value));
                    entries = "{" + string.Join(",", entries) + "}";
                    entries = $"\"{array[0]}\" :[ {entries}]";
                    entries = "{" + entries + "}";
                }
            }

            return entries;
        }

        public static Dictionary<string, string> GetClonePropertes(this Dictionary<string, string> dict)
        {
            return dict.ToDictionary(kvp => kvp.Key, kvp => string.Empty);
        }

        public static Dictionary<string, object> DeserializeAndFlatten(this string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (json.IsNullOrWhiteSpace())
            {
                return dict;
            }

            JToken token = JToken.Parse(json);

            FillDictionaryFromJToken(dict, token, "");
            return dict;
        }

        public static List<string> GetPropertiesName(object atype)
        {
            if (atype == null)
            {
                return new List<string>();
            }

            var jsonPropertiesName = new List<string>();

            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties();
            jsonPropertiesName.AddRange(props.Select(x => x.GetCustomAttribute<JsonPropertyAttribute>().PropertyName).ToList<string>());

            return jsonPropertiesName;
        }

        private static void FillDictionaryFromJToken(Dictionary<string, object> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                    }

                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
                        index++;
                    }

                    break;

                default:
                    dict.Add(prefix, ((JValue)token).Value);
                    break;
            }
        }

        private static string Join(string prefix, string name)
        {
            return (string.IsNullOrEmpty(prefix) ? name : prefix + "." + name);
        }

        public static Dictionary<string, object> MappingFilterData(this Dictionary<string, string> filter, Dictionary<string, object> strOutdata)
        {
            Dictionary<string, object> newDic = new Dictionary<string, object>();
            foreach (var item in filter)
            {
                foreach (var stritem in strOutdata)
                {
                    if (item.Key.Equals(stritem.Value != null ? stritem.Value.ToString() : "", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (stritem.Key.EndsWith("_ID") && stritem.Key.Contains("PK_"))
                        {
                            newDic.AddIfNotContains($"`{stritem.Key}`", $"{item.Value} UNHEX(\'{stritem.Value}\')");
                        }
                        else
                        {
                            if (stritem.Value != null && stritem.Value.GetType() == Type.GetType("System.Boolean"))
                            {
                                var boolValue = (bool)stritem.Value ? 1 : 0;
                                newDic.AddIfNotContains($"`{stritem.Key}`", $"{item.Value} {boolValue}");
                            }
                            else
                            {
                                newDic.AddIfNotContains($"`{stritem.Key}`", $"{item.Value} \'{stritem.Value}\'");
                            }
                        }
                    }
                }
            }

            if (newDic.Count < strOutdata.Count)
            {
                foreach (var item in strOutdata)
                {
                    bool isExist = false;
                    foreach (var itemDic in newDic)
                    {
                        if (itemDic.Key.Equals(item.Key))
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if (isExist == false)
                    {
                        foreach (var itemFilter in filter)
                        {
                            if (itemFilter.Key.Equals(item.Value))
                            {
                                newDic.AddIfNotContains($"`{item.Key}`", $"{itemFilter.Value} \'{item.Value}\'");
                            }
                            else
                            {
                                bool isDateTimeType = item.Value != null && item.Value.GetType() == typeof(DateTime);
                                if (isDateTimeType)
                                {
                                    newDic.Add($"`{item.Key}`", $"{itemFilter.Value} \'{item.Value}\'");
                                }
                                else
                                {
                                    // for another case
                                    newDic.AddIfNotContains($"`{item.Key}`", $"{itemFilter.Value} \'{item.Value}\'");
                                }
                            }
                        }
                    }
                }
            }

            // if still <, resoled problem for id of trading acccount(BK and Platform): 500006MT4
            if (newDic.Count < strOutdata.Count)
            {
                foreach (var itemFilter in filter)
                {
                    if (itemFilter.Value.Trim().Equals("="))
                    {
                        foreach (var item in strOutdata)
                        {
                            newDic.AddIfNotContains($"`{item.Key}`", $" = \'{item.Value}\'");
                        }
                    }
                }
            }

            return newDic;
        }

        public static List<string> ToDatabaseFormat(this Dictionary<string, object> dict)
        {
            List<string> lists = new List<string>();
            foreach (var item in dict)
            {
                lists.Add($"{item.Key} {item.Value}");
            }

            return lists;
        }

        public static Dictionary<string, object> MappingSortData(this Dictionary<string, string> sortData, Dictionary<string, object> strOutdata)
        {
            Dictionary<string, object> newDic = new Dictionary<string, object>();

            //foreach (var item in sortData)
            //{
            //    foreach (var stritem in strOutdata)
            //    {
            //        if (item.Key.Equals(stritem.Value != null ? stritem.Value.ToString() : "", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            newDic.Add($"`{item.Key}`", Convert.ToInt32(stritem.Value) == 0 ? "DESC" : "ASC");
            //        }

            //    }
            //}

            if (newDic.Count < strOutdata.Count && strOutdata.Count == 1 && sortData.Count == 1)
            {
                string sortOperator = Convert.ToInt32(sortData.ElementAt(0).Value) == 0 ? "DESC" : "ASC";
                newDic.AddIfNotContains($"`{strOutdata.ElementAt(0).Key}`", $" {sortOperator} ");
            }

            return newDic;
        }

        public static bool AddIfNotContains<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.ContainsKey(key))
            {
                return false;
            }

            source.Add(key, value);
            return true;
        }
    }
}