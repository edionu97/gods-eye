using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Repository;

namespace GodsEye.Application.Persistence.Repository.Abstract
{
    public abstract partial class AbstractRepository<T, TContext>
    {
        /// <summary>
        /// This function it is used for including virtual fields
        /// </summary>
        /// <param name="queryable">the queryable</param>
        /// <param name="fieldsToBeIncluded">list of fields to be included</param>
        /// <returns>a new queryable instance</returns>
        protected static IQueryable<T> IncludeFields(IQueryable<T> queryable, IEnumerable<string> fieldsToBeIncluded)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var propertyName in fieldsToBeIncluded ?? new List<string>())
            {
                queryable =
                    queryable
                        .Include(typeof(T)
                            .GetProperty(propertyName)?.Name
                    ?? throw new Exception(Constants.NotFoundMessage));
            }

            return queryable;
        }
    }
}
