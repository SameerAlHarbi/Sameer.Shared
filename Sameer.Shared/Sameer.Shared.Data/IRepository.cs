using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sameer.Shared.Data
{
    public interface IRepository : IDisposable
    {
        #region Get Methods

        IQueryable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> predicate = null
            , params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class;

        IIncludableQueryable<TEntity, TElement> AddInclude<TEntity, TElement>(IQueryable<TEntity> expression
        , Expression<Func<TEntity, TElement>> includeProperty) where TEntity : class where TElement : class;

        IIncludableQueryable<TEntity, TProperty> AddThenInclude<TEntity, TElement, TProperty>(IIncludableQueryable<TEntity, TElement> expression
            , Expression<Func<TElement, TProperty>> includeProperty) where TEntity : class where TElement : class;

        TEntity GetSingleItem<TEntity>(Expression<Func<TEntity, bool>> predicate
        , params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class;

        Task<TEntity> GetSingleItemAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class;

        List<TEntity> GetAllLocals<TEntity>() where TEntity : class;

        #endregion

        #region Insert Update Delete

        RepositoryActionResult<TEntity> Insert<TEntity>(TEntity newItem,
        bool checkConcurrency = true,
        bool mergeValues = false,
        bool validateBeforeSave = true) where TEntity : class, new();

        Task<RepositoryActionResult<TEntity>> InsertAsync<TEntity>
            (TEntity newItem, bool checkConcurrency, bool mergeValues, bool validateBeforeSave) 
            where TEntity : class, new();

        IEnumerable<RepositoryActionResult<TEntity>> InsertMany<TEntity>(IEnumerable<TEntity> newItems,
        bool checkConcurrency = true,
        bool mergeValues = false,
        bool validateBeforeSave = true) where TEntity : class, new();

        Task<IEnumerable<RepositoryActionResult<TEntity>>> InsertManyAsync<TEntity>
        (IEnumerable<TEntity> newItems, bool checkConcurrency, bool mergeValues, bool validateBeforeSave) 
            where TEntity : class, new();

        RepositoryActionResult<TEntity> Update<TEntity>(TEntity newItem,
        bool checkConcurrency = true,
        bool mergeValues = false,
        bool validateBeforeSave = true) where TEntity : class, ISameerObject, new();

        Task<RepositoryActionResult<TEntity>> UpdateAsync<TEntity>
        (TEntity newItem, bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
        where TEntity : class, ISameerObject, new();

        RepositoryActionResult<TEntity> Update<TEntity>(TEntity newItem,
        Expression<Func<TEntity, bool>> existingItemPredicate,
        bool checkConcurrency = true,
        bool mergeValues = false,
        bool validateBeforeSave = true) where TEntity : class, new();

        Task<RepositoryActionResult<TEntity>> UpdateAsync<TEntity>
            (TEntity newItem, Expression<Func<TEntity, bool>> existingItemPredicate, bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
            where TEntity : class, new();

        RepositoryActionResult<TEntity> Delete<TEntity>(int itemId) where TEntity : class, ISameerObject, new();

        Task<RepositoryActionResult<TEntity>> DeleteAsync<TEntity>(int itemId) where TEntity : class, ISameerObject, new();

        RepositoryActionResult<TEntity> Delete<TEntity>(Expression<Func<TEntity, bool>> existingItemPredicate)
            where TEntity : class, new();

        Task<RepositoryActionResult<TEntity>> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> existingItemPredicate)
            where TEntity : class, new();

        RepositoryActionResult<TEntity> Delete<TEntity, TElement>(int itemId, params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
            where TEntity : class, ISameerObject, new()
            where TElement : class;

        Task<RepositoryActionResult<TEntity>> DeleteAsync<TEntity, TElement>(int itemId
          , params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
          where TEntity : class, ISameerObject, new()
          where TElement : class;

        RepositoryActionResult<TEntity> Delete<TEntity, TElement>(Expression<Func<TEntity, bool>> existingItemPredicate
            , params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
            where TEntity : class, new()
            where TElement : class;

        Task<RepositoryActionResult<TEntity>> DeleteAsync<TEntity, TElement>(Expression<Func<TEntity, bool>> existingItemPredicate
          , params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
            where TEntity : class, new()
            where TElement : class;

        IEnumerable<RepositoryActionResult<TEntity>> DeleteAll<TEntity>(int[] itemsIds) where TEntity : class, ISameerObject, new();

        Task<IEnumerable<RepositoryActionResult<TEntity>>> DeleteAllAsync<TEntity>(int[] itemsIds) where TEntity : class, ISameerObject, new();

        IEnumerable<RepositoryActionResult<TEntity>> DeleteAll<TEntity>(Expression<Func<TEntity, bool>> existingItemsPredicate) where TEntity : class, new();

        Task<IEnumerable<RepositoryActionResult<TEntity>>> DeleteAllAsync<TEntity>(Expression<Func<TEntity, bool>> existingItemsPredicate)
           where TEntity : class, new();

        #endregion
    }
}
