using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ocph.DAL.QueryBuilder
{

    internal class InsertQuery
    {
        private readonly EntityInfo entity;

        public InsertQuery(EntityInfo entity)
        {
            this.entity = entity;
        }

        internal string GetQuery(object t)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Insert Into ").Append(entity.TableName).Append(" Values( ");

            foreach (PropertyInfo p in entity.DbTableProperty)
            {
                var att = p.Name;
                if (att != null)
                {
                    sb.Append(Helpers.ConverConstant(p.GetValue(t))).Append(", ");
                }
            }
            var result = sb.ToString();
            sb.Clear();
            sb.Append(result[0..(result.Length - 2)]);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Get the Next IdKey
        /// </summary>
        /// <remarks>Get the Next IdKey from Dettagli Table</remarks>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal Server Error</response>

        internal string GetQuerywithParameter(object t)
        {
            if (t is null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Insert Into ").Append(entity.TableName).Append("(");

            foreach (PropertyInfo p in entity.DbTableProperty)
            {
                sb.Append(p.Name).Append(", ");
            }

            var result = sb.ToString();
            sb.Clear();
            sb.Append(result[0..(result.Length - 2)]);
            sb.Append(") values (");
            foreach (PropertyInfo p in entity.DbTableProperty)
            {
                sb.Append("@").Append(p.Name).Append(", ");
            }
            result = sb.ToString();
            sb.Clear();
            sb.Append(result[0..(result.Length - 2)]);
            sb.Append(")");
            return sb.ToString();
        }

        internal string GetLastID()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ").Append(entity.GetAttributPrimaryKeyName())
               .Append(" From ").Append(entity.TableName)
                  .Append(" Order By ")
                  .Append(entity.GetAttributPrimaryKeyName())
                  .Append(" Desc Limit 1");
            return sb.ToString();
        }

        internal string GetChildInsertQuery(PropertyInfo p, object Item, EntityInfo entityChild)
        {
            string Query = string.Empty;


            PropertyInfo propParetn = entity.GetPropertyByPropertyName(p.Name);
            if (propParetn != null)
            {
                var foreignkeyinChild = (ForeignKeyAttribute)entityChild.GetAttributeForeignKeyByParentType(entity.GetEntityType());
                var PropertyPK = Helpers.ConverConstant(entity.PrimaryKeyProperty.GetValue(Item));
                Query = string.Format("Select * From {0} where {1}={2}",
                   entityChild.TableName,
                   foreignkeyinChild.Name,
                   PropertyPK);
            }

            return Query;
        }




    }


}
