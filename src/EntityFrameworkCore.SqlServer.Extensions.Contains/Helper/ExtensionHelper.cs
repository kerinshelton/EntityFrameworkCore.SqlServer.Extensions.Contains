using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFrameworkCore.SqlServer.Extensions.Contains.Helper
{
    public static class ExtensionHelper
    {
        internal static string GetDbMappedType(Type type)
        {
            var typeConversion = new Dictionary<Type, Tuple<SqlDbType, string>>
            {
                {typeof(string), new Tuple<SqlDbType, string>(SqlDbType.NVarChar, "(100)")},
                {typeof(int), new Tuple<SqlDbType, string>(SqlDbType.Int, "")},
                {typeof(long), new Tuple<SqlDbType, string>(SqlDbType.BigInt, "")},
                {typeof(decimal), new Tuple<SqlDbType, string>(SqlDbType.Decimal, "(19,4)")},
                {typeof(bool), new Tuple<SqlDbType, string>(SqlDbType.Bit, "")},
                {typeof(DateTime), new Tuple<SqlDbType, string>(SqlDbType.DateTime, "")},
                {typeof(DateTimeOffset), new Tuple<SqlDbType, string>(SqlDbType.DateTimeOffset, "")}
            };

            if (typeConversion.ContainsKey(type))
            {
                return typeConversion[type].Item1 + typeConversion[type].Item2;
            }

            throw new NotSupportedException();
        }
        internal static MemberInfo GetProperty(Expression method)
        {
            if (!(method is LambdaExpression lambda))
                throw new ArgumentNullException("");

            MemberExpression memberExpr = null;

            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpr =
                        ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpr = lambda.Body as MemberExpression;
                    break;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr.Member;
        }
        internal static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
    }
}
