﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sameer.Shared.Data
{
    public class Repository<C> : IRepository 
        where C : DbContext
    {
        public bool Disposed { get; set; }
        protected C context;

        public Repository(C ctx)
        {
            context = ctx;
        }

        #region Get Methods

        public IQueryable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> predicate = null
            , params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            try
            {
                IQueryable<TEntity> query = this.context.Set<TEntity>().AsQueryable();
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }
                if (includeProperties != null)
                {
                    foreach (var including in includeProperties)
                    {
                        query = query.Include(including);
                    }
                }
                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IIncludableQueryable<TEntity, TElement> AddInclude<TEntity, TElement>(IQueryable<TEntity> expression
            , Expression<Func<TEntity, TElement>> includeProperty) where TEntity : class where TElement : class
        {
            try
            {
                return expression.Include(includeProperty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IIncludableQueryable<TEntity, TProperty> AddThenInclude<TEntity, TElement, TProperty>(IIncludableQueryable<TEntity, TElement> expression
            , Expression<Func<TElement, TProperty>> includeProperty) where TEntity : class where TElement : class
        {
            try
            {
                return expression.ThenInclude(includeProperty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TEntity GetSingleItem<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            try
            {
                return GetAll(predicate, includeProperties).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TEntity> GetSingleItemAsync<TEntity>(Expression<Func<TEntity, bool>> predicate
            , params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            try
            {
                return await GetAll(predicate, includeProperties).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<TEntity> GetAllLocals<TEntity>() where TEntity : class
        {
            try
            {
                return this.context.Set<TEntity>().Local.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Insert Update Delete

        public ICollection<ValidationResult> ValidateBeforeSave()
        {
            var result = new List<ValidationResult>();

            IEnumerable<ValidationResult> errors = context.ChangeTracker
                                     .Entries<IValidatableObject>()
                                     .SelectMany(e => e.Entity.Validate(validationContext: null))
                                     .Where(r => r != ValidationResult.Success);

            foreach (var error in errors)
            {
                result.Add(new ValidationResult(error.ErrorMessage, error.MemberNames));
            }

            return result;
        }

        private object MergeNewAndOldValues(PropertyValues original, PropertyValues current, PropertyValues database)
        {
            PropertyValues result = original.Clone();
            foreach (var propertyName in original.Properties.Select(p => p.Name))
            {
                if (original[propertyName] is PropertyValues)
                {
                    object mergedComplexValues = MergeNewAndOldValues((PropertyValues)original[propertyName],
                        (PropertyValues)current[propertyName], (PropertyValues)database[propertyName]);

                    ((PropertyValues)result[propertyName]).SetValues(mergedComplexValues);
                }
                else
                {
                    if (!object.Equals(current[propertyName], original[propertyName]))
                    {
                        result[propertyName] = current[propertyName];
                    }
                    else if (!object.Equals(database[propertyName], original[propertyName]))
                    {
                        result[propertyName] = database[propertyName];
                    }
                }
            }

            return result;
        }

        protected int SaveChanges(bool checkConcurrency = true, bool mergeValues = false, bool validateBeforeSave = true)
        {
            try
            {
                if (validateBeforeSave)
                {
                    ICollection<ValidationResult> vl = ValidateBeforeSave();
                    if (vl.Count > 0)
                    {
                        throw new ValidationException(vl.First(), validatingAttribute: null, value: null);
                    }
                }

                return this.context.SaveChanges();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (!checkConcurrency)
                    {
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                    else
                    {
                        if (!mergeValues)
                        {
                            entry.Reload();
                            throw new DbUpdateConcurrencyException(ex.Message, ex.Entries.Select(m => m as Microsoft.EntityFrameworkCore.Update.IUpdateEntry).ToList());
                        }
                        else
                        {
                            PropertyValues databaseValues = entry.GetDatabaseValues();
                            object mergedValues = MergeNewAndOldValues(entry.OriginalValues, entry.CurrentValues, databaseValues);
                            entry.OriginalValues.SetValues(databaseValues);
                            entry.CurrentValues.SetValues(mergedValues);
                        }
                    }
                }
                return SaveChanges(checkConcurrency, mergeValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected async Task<int> SaveChangesAsync(bool checkConcurrency = true, bool mergeValues = false, bool validateBeforeSave = true)
        {
            try
            {
                if (validateBeforeSave)
                {
                    ICollection<ValidationResult> vl = ValidateBeforeSave();
                    if (vl.Count > 0)
                    {
                        throw new ValidationException(vl.First(), validatingAttribute: null, value: null);
                    }
                }

                return await this.context.SaveChangesAsync();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (!checkConcurrency)
                    {
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                    else
                    {
                        if (!mergeValues)
                        {
                            entry.Reload();
                            throw new DbUpdateConcurrencyException(ex.Message, ex.Entries.Select(m => m as Microsoft.EntityFrameworkCore.Update.IUpdateEntry).ToList());
                        }
                        else
                        {
                            PropertyValues databaseValues = await entry.GetDatabaseValuesAsync();
                            object mergedValues = MergeNewAndOldValues(entry.OriginalValues, entry.CurrentValues, databaseValues);
                            entry.OriginalValues.SetValues(databaseValues);
                            entry.CurrentValues.SetValues(mergedValues);
                        }
                    }
                }
                return await SaveChangesAsync(checkConcurrency, mergeValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataActionResult<TEntity> Insert<TEntity>
            (TEntity newItem, bool checkConcurrency, bool mergeValues, bool validateBeforeSave) where TEntity : class, new()
        {
            try
            {
                //TODO:Try this
                //this.context.SetAsAdded(newItem);
                this.context.Set<TEntity>().Add(newItem);

                int result = this.SaveChanges(checkConcurrency, mergeValues, validateBeforeSave);
                if (result > 0)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.Created);
                }
                else
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NothingModified, exception: null);
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataActionResult<TEntity>> InsertAsync<TEntity>
            (TEntity newItem, bool checkConcurrency, bool mergeValues, bool validateBeforeSave) where TEntity : class, new()
        {
            try
            {
                //TODO:Try this
                //this.context.SetAsAdded(newItem);
                await this.context.Set<TEntity>().AddAsync(newItem);

                int result = await this.SaveChangesAsync(checkConcurrency, mergeValues, validateBeforeSave);
                if (result > 0)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.Created);
                }
                else
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NothingModified, exception: null);
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<DataActionResult<TEntity>> InsertMany<TEntity>
            (IEnumerable<TEntity> newItems, bool checkConcurrency, bool mergeValues, bool validateBeforeSave) where TEntity : class, new()
        {
            try
            {
                foreach (var newItem in newItems)
                {
                    //TODO:Try this
                    //this.context.SetAsAdded(newItem);
                    this.context.Set<TEntity>().Add(newItem);
                }

                int result = this.SaveChanges(checkConcurrency, mergeValues, validateBeforeSave);

                var results = new List<DataActionResult<TEntity>>();

                if (result > 0)
                {
                    foreach (var newItem in newItems)
                    {
                        results.Add(new DataActionResult<TEntity>(newItem, RepositoryActionStatus.Created));
                    }
                }

                return results;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DataActionResult<TEntity>>> InsertManyAsync<TEntity>
    (IEnumerable<TEntity> newItems, bool checkConcurrency, bool mergeValues, bool validateBeforeSave) where TEntity : class, new()
        {
            try
            {
                foreach (var newItem in newItems)
                {
                    //TODO:Try this
                    //this.context.SetAsAdded(newItem);
                    await this.context.Set<TEntity>().AddAsync(newItem);
                }

                int result = await this.SaveChangesAsync(checkConcurrency, mergeValues, validateBeforeSave);

                var results = new List<DataActionResult<TEntity>>();

                if (result > 0)
                {
                    foreach (var newItem in newItems)
                    {
                        results.Add(new DataActionResult<TEntity>(newItem, RepositoryActionStatus.Created));
                    }
                }

                return results;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataActionResult<TEntity> updateItem<TEntity>(TEntity newItem, TEntity itemToUpdate
            , bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
            where TEntity : class, new()
        {
            try
            {
                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                context.Entry(itemToUpdate).State = EntityState.Detached;

                // attach & save
                context.Set<TEntity>().Attach(newItem);

                // set the updated entity state to modified, so it gets updated.
                context.Entry(newItem).State = EntityState.Modified;

                int result = this.SaveChanges(checkConcurrency, mergeValues, validateBeforeSave);
                if (result > 0)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NothingModified, exception: null);
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<DataActionResult<TEntity>> updateItemAsync<TEntity>(TEntity newItem, TEntity itemToUpdate
    , bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
    where TEntity : class, new()
        {
            try
            {
                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                context.Entry(itemToUpdate).State = EntityState.Detached;

                // attach & save
                context.Set<TEntity>().Attach(newItem);

                // set the updated entity state to modified, so it gets updated.
                context.Entry(newItem).State = EntityState.Modified;

                int result = await this.SaveChangesAsync(checkConcurrency, mergeValues, validateBeforeSave);
                if (result > 0)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NothingModified, exception: null);
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataActionResult<TEntity> updateItemWithoutSave<TEntity>(TEntity newItem, TEntity itemToUpdate)
            where TEntity : class, new()
        {
            try
            {
                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                context.Entry(itemToUpdate).State = EntityState.Detached;

                // attach & save
                context.Set<TEntity>().Attach(newItem);

                // set the updated entity state to modified, so it gets updated.
                context.Entry(newItem).State = EntityState.Modified;

                return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NothingModified, exception: null);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataActionResult<TEntity> Update<TEntity>
            (TEntity newItem, bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
            where TEntity : class, ISameerObject, new()
        {
            try
            {
                // you can only update when an data already exists for this id

                TEntity itemToUpdate = this.GetSingleItem<TEntity>(t => t.Id == newItem.Id);

                if (itemToUpdate == null)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NotFound);
                }

                return updateItem(newItem, itemToUpdate, checkConcurrency, mergeValues, validateBeforeSave);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataActionResult<TEntity>> UpdateAsync<TEntity>
    (TEntity newItem, bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
    where TEntity : class, ISameerObject, new()
        {
            try
            {
                // you can only update when an data already exists for this id

                TEntity itemToUpdate = await this.GetSingleItemAsync<TEntity>(t => t.Id == newItem.Id);

                if (itemToUpdate == null)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NotFound);
                }

                return await updateItemAsync(newItem, itemToUpdate, checkConcurrency, mergeValues, validateBeforeSave);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataActionResult<TEntity> Update<TEntity>
            (TEntity newItem, Expression<Func<TEntity, bool>> existingItemPredicate, bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
            where TEntity : class, new()
        {
            try
            {
                // you can only update when an data already exists for this.

                TEntity itemToUpdate = this.GetSingleItem(existingItemPredicate);

                if (itemToUpdate == null)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NotFound);
                }

                return updateItem(newItem, itemToUpdate, checkConcurrency, mergeValues, validateBeforeSave);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataActionResult<TEntity>> UpdateAsync<TEntity>
            (TEntity newItem, Expression<Func<TEntity, bool>> existingItemPredicate, bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
            where TEntity : class, new()
        {
            try
            {
                // you can only update when an data already exists for this.

                TEntity itemToUpdate = await this.GetSingleItemAsync(existingItemPredicate);

                if (itemToUpdate == null)
                {
                    return new DataActionResult<TEntity>(newItem, RepositoryActionStatus.NotFound);
                }

                return await updateItemAsync(newItem, itemToUpdate, checkConcurrency, mergeValues, validateBeforeSave);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DataActionResult<TEntity>>> UpdateManyAsync<TEntity>
            (IEnumerable<TEntity> newItems, bool checkConcurrency, bool mergeValues, bool validateBeforeSave)
            where TEntity : class, ISameerObject, new()
        {
            try
            {

                // you can only update when an data already exists for this id
                List<int> ids = newItems?.Select(d => d.Id).ToList() ?? new List<int>();
                List<TEntity> itemsToUpdate = await this.GetAll<TEntity>(t => ids.Contains(t.Id)).ToListAsync();

                var result = new List<DataActionResult<TEntity>>();
                foreach (var itemToUpdate in itemsToUpdate)
                {
                    var newItem = newItems.FirstOrDefault(i => i.Id == itemToUpdate.Id);
                    updateItemWithoutSave(newItem, itemToUpdate);
                }

                int saveResult = await this.SaveChangesAsync(checkConcurrency, mergeValues, validateBeforeSave);
                if (saveResult > 0)
                {
                    foreach (var item in newItems)
                    {
                        result.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.Updated));
                    }
                }
                else
                {
                    foreach (var item in newItems)
                    {
                        result.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.NothingModified, exception: null));
                    }
                }

                return result;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataActionResult<TEntity> deleteItem<TEntity>(TEntity itemToDelete) where TEntity : class, new()
        {
            try
            {
                context.Set<TEntity>().Remove(itemToDelete);
                int result = this.SaveChanges(checkConcurrency: false, mergeValues: false, validateBeforeSave: false);
                if (result > 0)
                {
                    return new DataActionResult<TEntity>(itemToDelete, RepositoryActionStatus.Deleted);
                }
                else
                {
                    return new DataActionResult<TEntity>(itemToDelete, RepositoryActionStatus.NothingModified, exception: null);
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<DataActionResult<TEntity>> deleteItemAsync<TEntity>(TEntity itemToDelete) where TEntity : class, new()
        {
            try
            {
                context.Set<TEntity>().Remove(itemToDelete);
                int result = await this.SaveChangesAsync(checkConcurrency: false, mergeValues: false, validateBeforeSave: false);
                if (result > 0)
                {
                    return new DataActionResult<TEntity>(itemToDelete, RepositoryActionStatus.Deleted);
                }
                else
                {
                    return new DataActionResult<TEntity>(itemToDelete, RepositoryActionStatus.NothingModified, exception: null);
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataActionResult<TEntity> Delete<TEntity>(int itemId) where TEntity : class, ISameerObject, new()
        {
            try
            {
                TEntity itemToDelete = this.GetSingleItem<TEntity>(t => t.Id == itemId);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                return deleteItem(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataActionResult<TEntity>> DeleteAsync<TEntity>(int itemId) where TEntity : class, ISameerObject, new()
        {
            try
            {
                TEntity itemToDelete = await this.GetSingleItemAsync<TEntity>(t => t.Id == itemId);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                return await deleteItemAsync(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataActionResult<TEntity> Delete<TEntity>(Expression<Func<TEntity, bool>> existingItemPredicate) where TEntity : class, new()
        {
            try
            {
                TEntity itemToDelete = this.GetSingleItem(existingItemPredicate);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                return deleteItem(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataActionResult<TEntity>> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> existingItemPredicate) where TEntity : class, new()
        {
            try
            {
                TEntity itemToDelete = await this.GetSingleItemAsync(existingItemPredicate);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                return await deleteItemAsync(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataActionResult<TEntity> Delete<TEntity, TElement>(int itemId
            , params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
            where TEntity : class, ISameerObject, new()
            where TElement : class
        {
            try
            {
                TEntity itemToDelete = this.GetSingleItem<TEntity>(t => t.Id == itemId);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                foreach (var navigaionPrperty in navigaionPrperties)
                {
                    context.Entry(itemToDelete).Collection(navigaionPrperty).Load();
                }

                return deleteItem(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataActionResult<TEntity>> DeleteAsync<TEntity, TElement>(int itemId
          , params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
          where TEntity : class, ISameerObject, new()
          where TElement : class
        {
            try
            {
                TEntity itemToDelete = await this.GetSingleItemAsync<TEntity>(t => t.Id == itemId);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                foreach (var navigaionPrperty in navigaionPrperties)
                {
                    context.Entry(itemToDelete).Collection(navigaionPrperty).Load();
                }

                return await deleteItemAsync(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataActionResult<TEntity> Delete<TEntity, TElement>(Expression<Func<TEntity, bool>> existingItemPredicate
            , params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
            where TEntity : class, new()
            where TElement : class
        {
            try
            {
                TEntity itemToDelete = this.GetSingleItem<TEntity>(existingItemPredicate);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                foreach (var navigaionPrperty in navigaionPrperties)
                {
                    context.Entry(itemToDelete).Collection(navigaionPrperty).Load();
                }

                return deleteItem(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataActionResult<TEntity>> DeleteAsync<TEntity, TElement>(Expression<Func<TEntity, bool>> existingItemPredicate
          , params Expression<Func<TEntity, IEnumerable<TElement>>>[] navigaionPrperties)
            where TEntity : class, new()
            where TElement : class
        {
            try
            {
                TEntity itemToDelete = await this.GetSingleItemAsync<TEntity>(existingItemPredicate);

                if (itemToDelete == null)
                {
                    return new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                foreach (var navigaionPrperty in navigaionPrperties)
                {
                    context.Entry(itemToDelete).Collection(navigaionPrperty).Load();
                }

                return await deleteItemAsync(itemToDelete);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<DataActionResult<TEntity>> DeleteAll<TEntity>(int[] itemsIds) where TEntity : class, ISameerObject, new()
        {
            try
            {
                List<TEntity> itemsToDelete = this.GetAll<TEntity>(itm => itemsIds.Contains(itm.Id)).ToList();

                if (itemsToDelete == null || itemsToDelete.Count() < 1)
                {
                    return new List<DataActionResult<TEntity>> { new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound) };
                }

                foreach (var item in itemsToDelete)
                {
                    context.Set<TEntity>().Remove(item);
                }

                int result = this.SaveChanges(checkConcurrency: false, mergeValues: false, validateBeforeSave: false);

                var results = new List<DataActionResult<TEntity>>();


                foreach (var item in itemsToDelete)
                {
                    if (result > 0)
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.Deleted));
                    }
                    else
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.NothingModified));
                    }
                }

                return results;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DataActionResult<TEntity>>> DeleteAllAsync<TEntity>(int[] itemsIds) where TEntity : class, ISameerObject, new()
        {
            try
            {
                List<TEntity> itemsToDelete = await this.GetAll<TEntity>(itm => itemsIds.Contains(itm.Id)).ToListAsync();

                if (itemsToDelete == null || itemsToDelete.Count() < 1)
                {
                    return new List<DataActionResult<TEntity>> { new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound) };
                }

                foreach (var item in itemsToDelete)
                {
                    context.Set<TEntity>().Remove(item);
                }

                int result = await this.SaveChangesAsync(checkConcurrency: false, mergeValues: false, validateBeforeSave: false);

                var results = new List<DataActionResult<TEntity>>();


                foreach (var item in itemsToDelete)
                {
                    if (result > 0)
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.Deleted));
                    }
                    else
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.NothingModified));
                    }
                }

                return results;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<DataActionResult<TEntity>> DeleteAll<TEntity>(Expression<Func<TEntity, bool>> existingItemsPredicate)
            where TEntity : class, new()
        {
            try
            {
                List<TEntity> itemsToDelete = this.GetAll(existingItemsPredicate).ToList();

                if (itemsToDelete == null || itemsToDelete.Count() < 1)
                {
                    return new List<DataActionResult<TEntity>> { new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound) };
                }

                foreach (var item in itemsToDelete)
                {
                    context.Set<TEntity>().Remove(item);
                }

                int result = this.SaveChanges(checkConcurrency: false, mergeValues: false, validateBeforeSave: false);

                var results = new List<DataActionResult<TEntity>>();


                foreach (var item in itemsToDelete)
                {
                    if (result > 0)
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.Deleted));
                    }
                    else
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.NothingModified));
                    }
                }

                return results;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DataActionResult<TEntity>>> DeleteAllAsync<TEntity>(Expression<Func<TEntity, bool>> existingItemsPredicate)
           where TEntity : class, new()
        {
            try
            {
                List<TEntity> itemsToDelete = await this.GetAll(existingItemsPredicate).ToListAsync();

                if (itemsToDelete == null || itemsToDelete.Count() < 1)
                {
                    return new List<DataActionResult<TEntity>> { new DataActionResult<TEntity>(entity: null, status: RepositoryActionStatus.NotFound) };
                }

                foreach (var item in itemsToDelete)
                {
                    context.Set<TEntity>().Remove(item);
                }

                int result = await this.SaveChangesAsync(checkConcurrency: false, mergeValues: false, validateBeforeSave: false);

                var results = new List<DataActionResult<TEntity>>();


                foreach (var item in itemsToDelete)
                {
                    if (result > 0)
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.Deleted));
                    }
                    else
                    {
                        results.Add(new DataActionResult<TEntity>(item, RepositoryActionStatus.NothingModified));
                    }
                }

                return results;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
                if (disposing)
                    context.Dispose();

            this.Disposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
