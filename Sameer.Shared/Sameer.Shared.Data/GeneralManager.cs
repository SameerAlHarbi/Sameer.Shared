using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sameer.Shared.Data
{
    public class GeneralManager<T> : IDisposable where T : class, ISameerObject, new()
    {
        protected IRepository repository;

        public GeneralManager(IRepository repo)
        {
            this.repository = repo;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null
            , params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                return this.repository.GetAll(predicate, includeProperties);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<T> AddInclude(IQueryable<T> expression
            , params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                foreach (var includ in includeProperties)
                {
                    expression = this.repository.AddInclude(expression, includ);
                }
                return expression;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IIncludableQueryable<T, O> AddInclude<O>(IQueryable<T> expression
            , Expression<Func<T, O>> includeProperty) where O : class
        {
            try
            {
                return this.repository.AddInclude(expression, includeProperty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IIncludableQueryable<T, P> AddThenInclude<O, P>(IIncludableQueryable<T, O> expression
            , Expression<Func<O, P>> includeProperty) where O : class
        {
            try
            {
                return this.repository.AddThenInclude(expression, includeProperty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetById(int itemId
            , params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                return GetSingleItem(t => t.Id == itemId, includeProperties);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<T> GetAllByIds(int[] itemsIds
            , params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                return this.GetAll(r => itemsIds.Contains(r.Id), includeProperties);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public T GetSingleItem(Expression<Func<T, bool>> predicate = null
            , params Expression<Func<T, object>>[] includeProperties)
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

        protected virtual ICollection<ValidationResult> ValidateItemInformation(T newItem)
        {
            var validateable = newItem as IValidatableObject;

            var results = new List<ValidationResult>();

            if (validateable != null)
            {
                results.AddRange(validateable.Validate(new ValidationContext(newItem)));
            }

            Validator.TryValidateObject(newItem, new ValidationContext(newItem, serviceProvider: null, items: null), results, validateAllProperties: true);

            return results;
        }

        protected virtual ICollection<ValidationResult> CustomNewRulesValidate(T newItem)
        {
            return new List<ValidationResult>();
        }

        protected virtual ICollection<ValidationResult> CustomUpdateRulesValidate(T newItem)
        {
            return new List<ValidationResult>();
        }

        protected virtual ICollection<ValidationResult> CustomDeleteRulesValidate(T itemToDelete)
        {
            return new List<ValidationResult>();
        }

        protected ICollection<ValidationResult> ValidateNewItem(T newItem)
        {
            List<ValidationResult> itemValidationResult = ValidateItemInformation(newItem).ToList();
            itemValidationResult.AddRange(ValidateUniqueItem(newItem));
            itemValidationResult.AddRange(CustomNewRulesValidate(newItem));
            return itemValidationResult;
        }

        protected ICollection<ValidationResult> ValidateUpdateItem(T currentItem)
        {
            List<ValidationResult> itemValidationResult = ValidateItemInformation(currentItem).ToList();
            itemValidationResult.AddRange(ValidateUniqueItem(currentItem));
            itemValidationResult.AddRange(CustomUpdateRulesValidate(currentItem));
            return itemValidationResult;
        }

        public virtual ICollection<ValidationResult> ValidateDeleteItem(T itemToDelete)
        {
            return CustomDeleteRulesValidate(itemToDelete);
        }

        protected ICollection<ValidationResult> ValidateUniqueItem(T newItem)
        {
            var vResults = new List<ValidationResult>();

            PropertyInfo[] properties = newItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var valProps = from prp in properties
                           where prp.GetCustomAttributes(typeof(UniqueAttribute), inherit: true).Count() > 0
                           select new
                           {
                               property = prp,
                               uniqAttr = prp.GetCustomAttributes(typeof(UniqueAttribute), inherit: true)
                           };

            if (valProps.Any())
            {
                var expressionsList = new List<Expression>();
                ParameterExpression e = Expression.Parameter(typeof(T), name: "e");

                foreach (var prp in valProps)
                {
                    Expression left = Expression.MakeMemberAccess(e, prp.property);
                    Expression right;

                    object propertyValue = newItem.GetType().GetProperty(prp.property.Name).GetValue(newItem, index: null);
                    if (propertyValue == null)
                    {
                        right = Expression.Constant(value: null);
                    }
                    else
                    {
                        right = Expression.Constant(propertyValue, propertyValue.GetType());
                    }

                    BinaryExpression resultExpression = Expression.Equal(left, right);

                    var unqAttr = prp.uniqAttr.First() as UniqueAttribute;
                    foreach (var parentName in unqAttr.ParentsPropertiesNames)
                    {
                        left = Expression.MakeMemberAccess(e, newItem.GetType().GetProperty(parentName));
                        propertyValue = newItem.GetType().GetProperty(parentName).GetValue(newItem, index: null);

                        if (propertyValue == null)
                        {
                            right = Expression.Constant(value: null);
                        }
                        else
                        {
                            right = Expression.Constant(propertyValue, newItem.GetType().GetProperty(parentName).PropertyType);
                        }

                        BinaryExpression resultExpression2 = Expression.Equal(left, right);
                        resultExpression = Expression.AndAlso(resultExpression, resultExpression2);
                    }

                    expressionsList.Add(resultExpression);

                }

                if (expressionsList.Any())
                {
                    Expression exps = expressionsList.First();
                    if (expressionsList.Count > 1)
                    {
                        for (int i = 1; i < expressionsList.Count; i++)
                        {
                            exps = Expression.OrElse(exps, expressionsList[i]);
                        }
                    }

                    MethodCallExpression whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { this.GetAll().ElementType },
                    this.GetAll(i => i.Id != newItem.Id).Expression,
                    Expression.Lambda<Func<T, bool>>(exps, new ParameterExpression[] { e }));

                    List<T> results = this.GetAll(i => i.Id != newItem.Id).Provider.CreateQuery<T>(whereCallExpression).ToList();

                    if (results.Count() > 0)
                    {
                        foreach (var prp in valProps)
                        {

                            object prpValue = newItem.GetType().GetProperty(prp.property.Name).GetValue(newItem, index: null);

                            if (prpValue == null)
                            {
                                if (results.Any(i => i.GetType().GetProperty(prp.property.Name).GetValue(i, index: null) == null))
                                {
                                    vResults.Add(new ValidationResult((prp.uniqAttr.First() as UniqueAttribute).ErrorMessage, new string[] { prp.property.Name }));
                                }
                            }
                            else if (prp.property.PropertyType == typeof(string))
                            {
                                if (results.Where(i => i.GetType().GetProperty(prp.property.Name).GetValue(i, index: null) != null)
                                    .Any(i => i.GetType().GetProperty(prp.property.Name).GetValue(i, index: null).ToString().Trim().ToUpper()
                                    == prpValue.ToString().Trim().ToUpper()))
                                {
                                    vResults.Add(new ValidationResult((prp.uniqAttr.First() as UniqueAttribute).ErrorMessage, new string[] { prp.property.Name }));
                                }
                            }
                            else
                            {
                                if (results.Where(i => i.GetType().GetProperty(prp.property.Name).GetValue(i, index: null) != null).Any(i => i.GetType().GetProperty(prp.property.Name).GetValue(i, index: null).Equals(prpValue)))
                                {
                                    vResults.Add(new ValidationResult((prp.uniqAttr.First() as UniqueAttribute).ErrorMessage, new string[] { prp.property.Name }));
                                }
                            }
                        }
                    }
                }
            }

            return vResults;
        }

        public RepositoryActionResult<T> InsertNew(T newItem, bool checkConcurrency = true, bool mergeValues = false, bool validateBeforeSave = true)
        {
            try
            {
                ICollection<ValidationResult> itemValidationResult = ValidateNewItem(newItem);

                if (itemValidationResult.Any())
                {
                    throw new ValidationException(itemValidationResult.First(), validatingAttribute: null, value: newItem);
                }

                return this.DirectInsertNew(newItem, checkConcurrency, mergeValues, validateBeforeSave);

            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected RepositoryActionResult<T> DirectInsertNew(T newItem, bool checkConcurrency = true, bool mergeValues = false, bool validateBeforeSave = true)
        {
            try
            {
                return repository.Insert(newItem, checkConcurrency, mergeValues, validateBeforeSave);
            }
            catch (ValidationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<RepositoryActionResult<T>> InsertMany(IEnumerable<T> newItems, bool checkConcurrency = true, bool mergeValues = false, bool validateBeforeSave = true)
        {
            try
            {
                foreach (var newItem in newItems)
                {
                    ICollection<ValidationResult> itemValidationResult = ValidateNewItem(newItem);

                    if (itemValidationResult.Any())
                    {
                        throw new ValidationException(itemValidationResult.First(), validatingAttribute: null, value: newItem);
                    }
                }

                return repository.InsertMany(newItems, checkConcurrency, mergeValues, validateBeforeSave);

            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public RepositoryActionResult<T> UpdateItem(T currentItem, bool checkConcurrency = true, bool mergeValues = false, bool validateBeforeSave = true)
        {
            try
            {
                ICollection<ValidationResult> itemValidationResult = ValidateUpdateItem(currentItem);

                if (itemValidationResult.Any())
                {
                    throw new ValidationException(itemValidationResult.First(), validatingAttribute: null, value: currentItem);
                }

                return this.DirectUpdateItem(currentItem, checkConcurrency, mergeValues, validateBeforeSave);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected RepositoryActionResult<T> DirectUpdateItem(T newItem, bool checkConcurrency = true, bool mergeValues = false, bool validateBeforeSave = true)
        {
            try
            {
                return repository.Update(newItem, checkConcurrency, mergeValues, validateBeforeSave);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public RepositoryActionResult<T> DeleteItem(int itemId)
        {
            try
            {
                T itemToDelete = this.GetById(itemId);

                if (itemToDelete == null)
                {
                    return new RepositoryActionResult<T>(entity: null, status: RepositoryActionStatus.NotFound);
                }

                ICollection<ValidationResult> itemValidationResult = ValidateDeleteItem(itemToDelete);

                if (itemValidationResult.Any())
                {
                    throw new ValidationException(itemValidationResult.First(), validatingAttribute: null, value: null);
                }

                return this.DirectDeleteItem(itemId);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<RepositoryActionResult<T>> DeleteItems(int[] itemsIds)
        {
            try
            {
                List<T> itemsToDelete = this.GetAll(itm => itemsIds.Contains(itm.Id)).ToList();

                if (itemsToDelete == null || itemsToDelete.Count < 1)
                {
                    return new List<RepositoryActionResult<T>> { new RepositoryActionResult<T>(entity: null, status: RepositoryActionStatus.NotFound) };
                }

                var itemsValidationResult = new List<ValidationResult>();

                foreach (var item in itemsToDelete)
                {
                    itemsValidationResult.AddRange(ValidateDeleteItem(item));
                }

                if (itemsValidationResult.Any())
                {
                    throw new ValidationException(itemsValidationResult.First(), validatingAttribute: null, value: null);
                }

                return this.repository.DeleteAll<T>(itemsIds);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual RepositoryActionResult<T> DirectDeleteItem(int itemId)
        {
            try
            {
                return repository.Delete<T>(itemId);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}
