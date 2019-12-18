using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Newtonsoft.Json;

namespace DataVault.Common.Extensions
{
    public static class ReflectionHelper
    {
        public static Type GetUnderlyingType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type t = type;
            bool isNullable = t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (isNullable)
                return Nullable.GetUnderlyingType(t);

            return t;
        }

        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType)
                return true;

            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static object Default(this Type type)
        {
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }

        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(Type type, TAttribute defaultValue = default, bool inherit = true)
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (type.IsDefined(typeof(TAttribute), inherit))
            {
                return type.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        public static object GetValueByPath(object obj, Type objectType, string propertyPath)
        {
            var value = obj;
            var currentType = objectType;
            var objectPath = currentType.FullName;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.Ordinal))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            foreach (var propertyName in absolutePropertyPath.Split('.'))
            {
                var property = currentType.GetProperty(propertyName);
                value = property.GetValue(value, null);
                currentType = property.PropertyType;
            }

            return value;
        }

        public static void SetValueByPath(object obj, Type objectType, string propertyPath, object value)
        {
            var currentType = objectType;
            PropertyInfo property;
            var objectPath = currentType.FullName;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.Ordinal))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            var properties = absolutePropertyPath.Split('.');

            if (properties.Length == 1)
            {
                property = objectType.GetProperty(properties.First());
                property.SetValue(obj, value);
                return;
            }

            for (int i = 0; i < properties.Length - 1; i++)
            {
                property = currentType.GetProperty(properties[i]);
                obj = property.GetValue(obj, null);
                currentType = property.PropertyType;
            }

            property = currentType.GetProperty(properties.Last());
            property.SetValue(obj, value);
        }

        public static object GetPropertyValue(this object instance, string path)
        {
            if (instance == null) return null;
            var pp = path.Split('.');
            Type t = instance.GetType();
            foreach (var prop in pp)
            {
                PropertyInfo propInfo = t.GetProperty(prop);
                if (propInfo != null)
                {
                    instance = propInfo.GetValue(instance, null);
                    t = propInfo.PropertyType;
                    if (instance == null) return null;
                }
                else throw new ArgumentException($"Properties path is not correct. Object Type: {instance.GetType()}. Path: {path}");
            }

            return instance;
        }

        public static List<TFieldType> GetFieldsOfType<TFieldType>(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(p => type.IsAssignableFrom(p.FieldType))
                .Select(pi => (TFieldType)pi.GetValue(null))
                .ToList();
        }

        public static TAttribute GetAttribute<TAttribute>(PropertyInfo prop) where TAttribute : Attribute
        {
            return (TAttribute)prop.GetCustomAttribute(typeof(TAttribute));
        }

        public static TAttribute GetAttribute<TAttribute, TResource>(Expression<Func<TResource, object>> navigationExpression) where TAttribute : Attribute
        {
            return (TAttribute)ParseNavigationExpression(navigationExpression).GetCustomAttribute(typeof(TAttribute));
        }

        private static PropertyInfo ParseNavigationExpression<TResource>(Expression<Func<TResource, object>> navigationExpression)
        {
            MemberExpression Exp = null;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            if (navigationExpression.Body is UnaryExpression)
            {
                var UnExp = (UnaryExpression)navigationExpression.Body;
                if (UnExp.Operand is MemberExpression)
                {
                    Exp = (MemberExpression)UnExp.Operand;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else if (navigationExpression.Body is MemberExpression)
            {
                Exp = (MemberExpression)navigationExpression.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)Exp.Member;
        }

        public static string GetPropertyPath(LambdaExpression expression)
        {
            var memberNames = new Stack<string>();

            var getMemberExp = new Func<Expression, MemberExpression>(
                toUnwrap =>
                {
                    if (toUnwrap is UnaryExpression)
                    {
                        return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
                    }

                    return toUnwrap as MemberExpression;
                });

            var memberExp = getMemberExp(expression.Body);

            while (memberExp != null)
            {
                memberNames.Push(memberExp.Member.Name);

                memberExp = getMemberExp(memberExp.Expression);
            }

            return memberNames.JoinNotEmpty(".");
        }

        public static string GetJsonPropertyPath(LambdaExpression expression)
        {
            var memberNames = new Stack<string>();

            var getMemberExp = new Func<Expression, MemberExpression>(
                toUnwrap =>
                {
                    if (toUnwrap is UnaryExpression)
                    {
                        return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
                    }

                    return toUnwrap as MemberExpression;
                });

            var memberExp = getMemberExp(expression.Body);

            while (memberExp != null)
            {
                var jsonAttribute = memberExp.Member?.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonAttribute != null)
                {
                    memberNames.Push(jsonAttribute.PropertyName);
                }

                memberExp = getMemberExp(memberExp.Expression);
            }

            return memberNames.JoinNotEmpty(".");
        }
    }
}