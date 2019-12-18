using System;

namespace DataVault
{
    public class EntityBulkCopyResult<TEntity> : BulkCopyResult
    {
        public TEntity Entity { get; set; }

        public static EntityBulkCopyResult<TEntity> Success(TEntity entity)
        {
            return new EntityBulkCopyResult<TEntity>
            {
                Code = "0",
                Message = "Success",
                Entity = entity,
            };
        }

        public static EntityBulkCopyResult<TEntity> Failed(TEntity entity, Exception exception)
        {
            return new EntityBulkCopyResult<TEntity>
            {
                Code = exception.GetType().Name,
                Message = exception.Message,
                Entity = entity,
            };
        }
    }
}