using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using GodsEye.Utility.Application.Helpers.Helpers.Reflection;

namespace GodsEye.Application.Persistence.Repository.Abstract
{
    public abstract partial class AbstractRepository<T, TContext> : IRepository<T>
                                                                    where TContext : DbContext
                                                                    where T : class
    {
        private readonly TContext _dbContext;

        protected AbstractRepository(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected abstract DbSet<T> GetDatabaseSet(TContext context);

        public async Task<T> FindByIdAsync(int id, IList<string> fieldsToBeIncluded = null)
        {
            //get all items from database
            var resultList = await IncludeFields(GetDatabaseSet(_dbContext), fieldsToBeIncluded).ToListAsync();

            //get the id property
            var idProperty = await ReflectionHelpers.GetPropertyAnnotatedWithAttributeAsync<T, KeyAttribute>();

            //get the the item thar respects the condition
            return resultList.FirstOrDefault(x =>
            {
                var value = idProperty.GetValue(x);
                return value is int @int && @int == id;
            });
        }

        public async Task<IEnumerable<T>> GetAllAsync(IList<string> fieldsToBeIncluded = null)
        {
            //get all items from database
            return await IncludeFields(GetDatabaseSet(_dbContext), fieldsToBeIncluded).ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {

            //begin new transaction
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //update the entity
                _dbContext.Update(entity);

                //save the changes
                await _dbContext.SaveChangesAsync();

                //commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddAsync(T entity)
        {
            //begin new transaction
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //add the entity
                await _dbContext.AddAsync(entity);

                //save the changes
                await _dbContext.SaveChangesAsync();

                //commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task DeleteAsync(T entity)
        {
            //begin new transaction
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //add the entity
                _dbContext.Remove(entity);

                //save the changes
                await _dbContext.SaveChangesAsync();

                //commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
