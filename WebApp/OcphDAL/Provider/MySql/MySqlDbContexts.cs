using MySql.Data.MySqlClient;
using Ocph.DAL.DbContext;
using Ocph.DAL.ExpressionHandler;
using Ocph.DAL.Mapping.MySql;
using Ocph.DAL.QueryBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ocph.DAL.Provider.MySql
{
    public class MySqlDbContexs : IDataTables
    {
        private IDbConnection _connection;

        public MySqlDbContexs(IDbConnection connection) {
            _connection = connection;
        }


        public MySqlDbContext<T> Create<T>() 
        {
            return new MySqlDbContext<T>(_connection);
        }

        public IQueryable<T> Select<T>()
        {
            throw new NotImplementedException();
        }
    }
}
