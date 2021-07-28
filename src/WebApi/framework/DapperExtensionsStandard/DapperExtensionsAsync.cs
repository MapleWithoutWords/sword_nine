using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public static partial class DapperExtensions
    {

        /// <summary>
        /// Executes a query for the specified id, returning the data typed as per T
        /// </summary>
        public static async Task<T> GetAsync<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var result = await Instance.GetAsync<T>(connection, id, transaction, commandTimeout);
            return (T)result;
        }

        /// <summary>
        /// Executes an insert query for the specified entity.
        /// </summary>
        public static async Task InsertAsync<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            await Instance.InsertAsync<T>(connection, entities, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an insert query for the specified entity, returning the primary key.  
        /// If the entity has a single key, just the value is returned.  
        /// If the entity has a composite key, an IDictionary&lt;string, object&gt; is returned with the key values.
        /// The key value for the entity will also be updated if the KeyType is a Guid or Identity.
        /// </summary>
        public static async Task<dynamic> InsertAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.InsertAsync<T>(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static async Task<bool> UpdateAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null, bool ignoreAllKeyProperties = false) where T : class
        {
            return await Instance.UpdateAsync<T>(connection, entity, transaction, commandTimeout, ignoreAllKeyProperties);
        }


        /// <summary>
        /// Executes a delete query for the specified entity.
        /// </summary>
        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.DeleteAsync<T>(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a delete query using the specified predicate.
        /// </summary>
        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.DeleteAsync<T>(connection, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// </summary>
        public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class
        {
            return await Instance.GetListAsync<T>(connection, predicate, sort, transaction, commandTimeout, buffered);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static async Task<IEnumerable<T>> GetPageAsync<T>(this IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class
        {
            return await Instance.GetPageAsync<T>(connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified firstResult and maxResults.
        /// </summary>
        public static async Task<IEnumerable<T>> GetSetAsync<T>(this IDbConnection connection, object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class
        {
            return await Instance.GetSetAsync<T>(connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout, buffered);
        }


        /// <summary>
        /// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        /// </summary>
        public static async Task<int> CountAsync<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.CountAsync<T>(connection, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a select query for multiple objects, returning IMultipleResultReader for each predicate.
        /// </summary>
        public static async Task<IMultipleResultReader> GetMultipleAsync(this IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return await Instance.GetMultipleAsync(connection, predicate, transaction, commandTimeout);
        }
    }
}
