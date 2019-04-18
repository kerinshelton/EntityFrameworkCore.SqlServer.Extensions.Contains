using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using EntityFrameworkCore.SqlServer.Extensions.Contains.Helper;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.SqlServer.Extensions.Contains.Core
{
    public static class RelationalQueryableCustomExtensions
    {
        public static System.Linq.IQueryable<TEntity> Contains<TEntity, T>(this System.Linq.IQueryable<TEntity> source,
            IEnumerable<T> forCollection, string fullyQualifiedTableName, bool avoidReadLocks = false,
            params Expression<Func<TEntity, object>>[] joinColumns
        )
            where TEntity : class

        {
            var root = new XElement("nodes");

            //generate xml
            foreach (var entity in forCollection)
            {
                root.Add(new XElement("node",
                    joinColumns.Select(c => new XElement(ExtensionHelper.GetProperty(c).Name,
                        entity.GetType().GetProperty(ExtensionHelper.GetProperty(c).Name)?.GetValue(entity, null)))));
            }

            var sqlStr =
                new StringBuilder(
                    $"SELECT tbl.* FROM @xml.nodes('/nodes/node') nodes([node]) JOIN {fullyQualifiedTableName} tbl {(avoidReadLocks ? "WITH (NOLOCK)" : "")} ON ");

            //append join filter clauses
            sqlStr.Append(string.Join(" AND ",
                joinColumns.Select(c =>
                    $"tbl.[{ExtensionHelper.GetProperty(c).Name}] = [nodes].[node].value('({ExtensionHelper.GetProperty(c).Name})[1]', '{ExtensionHelper.GetDbMappedType(ExtensionHelper.GetProperty(c).GetUnderlyingType())}')")));

            var matched = source.FromSql(sqlStr.ToString(), new SqlParameter("@xml", new SqlXml(root.CreateReader())));

            return matched.AsQueryable();
        }
    }
}
