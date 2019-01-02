using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic;
using System.Reflection;
using System.Linq.Expressions;

namespace Sameer.Shared
{
    public static class IQueryableExtensions
    {
        //Need this to construct the query correctly
        public static MethodInfo s_orderBy = typeof(Queryable).GetMethods().First(m => m.Name == "OrderBy");
        public static MethodInfo s_orderByDescending = typeof(Queryable).GetMethods().First(m => m.Name == "OrderByDescending");

        public static MethodInfo s_thenBy = typeof(Queryable).GetMethods().First(m => m.Name == "ThenBy");
        public static MethodInfo s_ThenByDescending = typeof(Queryable).GetMethods().First(m => m.Name == "ThenByDescending");

        public static MethodInfo s_Where = typeof(Queryable).GetMethods().First(m => m.Name == "Where");

        public static IOrderedQueryable<T> DynamicOrderBy<T>(this IQueryable<T> source, string sort)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (sort == null)
            {
                return source as IOrderedQueryable<T>;
            }

            IOrderedQueryable<T> results = source as IOrderedQueryable<T>;

            var lstSort = sort.Split(',');

            int counter = 0;
            foreach (var sortOption in lstSort)
            {
                string propertyName = sortOption;
                bool descending = sortOption.StartsWith("-");

                if (descending)
                {
                    propertyName = sortOption.Remove(0, 1);
                }

                var expr = results.Expression;
                var p = Expression.Parameter(typeof(T), "x");

                var propInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var sortExpr = Expression.Lambda(Expression.Property(p, propInfo), p);
                MethodInfo method = null;
                if (counter < 1)
                {
                    method = descending ? s_orderByDescending.MakeGenericMethod(typeof(T), propInfo.PropertyType) : s_orderBy.MakeGenericMethod(typeof(T), propInfo.PropertyType);
                }
                else
                {
                    method = descending ? s_ThenByDescending.MakeGenericMethod(typeof(T), propInfo.PropertyType) : s_thenBy.MakeGenericMethod(typeof(T), propInfo.PropertyType);
                }

                var call = Expression.Call(method, expr, sortExpr);
                results = results.Provider.CreateQuery<T>(call) as IOrderedQueryable<T>;
                counter++;
            }

            return results;
        }

        /*
        public static IEnumerable<T> ApplySort<T>(this IEnumerable<T> source, string sort)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (sort == null)
            {
                return source;
            }

            // split the sort string
            var lstSort = sort.Split(',');

            // run through the sorting options and apply them - in reverse
            // order, otherwise results will come out sorted by the last 
            // item in the string first!
            foreach (var sortOption in lstSort.Reverse())
            {
                // if the sort option starts with "-", we order
                // descending, ortherwise ascending

                if (sortOption.StartsWith("-"))
                {
                    source = source.OrderBy(sortOption.Remove(0, 1) + " descending");
                }
                else
                {
                    source = source.OrderBy(sortOption);
                }
            }

            return source;
        }

    */

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string sort)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (sort == null)
            {
                return source;
            }

            // split the sort string
            var lstSort = sort.Split(',');

            // run through the sorting options and apply them - in reverse
            // order, otherwise results will come out sorted by the last 
            // item in the string first!
            foreach (var sortOption in lstSort.Reverse())
            {
                // if the sort option starts with "-", we order
                // descending, ortherwise ascending

                if (sortOption.StartsWith("-"))
                {
                    source = source.OrderBy(sortOption.Remove(0, 1) + " descending");
                }
                else
                {
                    source = source.OrderBy(sortOption);
                }
            }

            return source;
        }

        //public static IEnumerable<T> ApplyFilter<T>(this IEnumerable<T> source, string filter)
        //{
        //    if (source == null)
        //    {
        //        throw new ArgumentNullException("source");
        //    }

        //    if (filter == null)
        //    {
        //        return source;
        //    }

        //    source = source.Where(filter);

        //    return source;
        //}
    }
}
