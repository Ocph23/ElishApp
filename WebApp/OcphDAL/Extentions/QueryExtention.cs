using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Ocph.DAL
{
    public static class QueryExtention
    {

        public static IQueryable<T> Includes<T, TProperty>
            (this IQueryable<T> source, Expression<Func<T, TProperty>> path) where T : class
        {
            var prov = source.Provider;
            var type = path.GetType();

            var aaa = type.GetGenericArguments().Single();

            var mex = path.Body as MemberExpression;

            var rr = typeof(TProperty);

            using (var scope = ServiceLocator.Instance.CreateScope())
            {
                var connection =  scope.ServiceProvider.GetRequiredService<IDbConnection>();
            }


            return source;
        }



        public static object LastID(this object[] query)
        {
            
            object ret = query[1];
            Type t = ret.GetType();

            if (t.Name == "Int32")
                ret = (Int32)ret;
            if (t.Name == "String")
                ret = ret.ToString();
            return ret;
        }

        public static bool IsInsert(this object[] query)
        {
            if ((Int32)query[0] > 0)
                return true;
            else
                return false;
        }

        /*
                public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
               IQueryable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter,
               TInner, TResult> resultSelector)
                {

                    throw new NotImplementedException();
                }

                public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
                 IQueryable<TInner> inner, Func<TOuter,
                 TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector,
                 IEqualityComparer<TKey> comparer)
                {
                    throw new NotImplementedException();
                }
                */
    }



    public static class CommonExtention
    {
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
