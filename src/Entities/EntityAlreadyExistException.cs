using System;

using Humanizer;

namespace DataVault.Entities
{
    /// <summary>
    /// This exception is thrown if an entity excepted to be found but not found.
    /// </summary>
    public class EntityAlreadyExistException : Exception
    {
        public Type EntityType { get; set; }

        public object Id { get; set; }

        public EntityAlreadyExistException()
        {
        }

        public EntityAlreadyExistException(Type entityType)
            : this(entityType, null, null)
        {
        }

        public EntityAlreadyExistException(Type entityType, object id)
            : this(entityType, id, null)
        {
        }

        public EntityAlreadyExistException(Type entityType, object id, Exception innerException)
            : base($"{EntityHelper.GetEntityName(entityType).Humanize(LetterCasing.Title)} '{id}' already exists.", innerException)
        {
            EntityType = entityType;
            Id = id;
        }

        public EntityAlreadyExistException(string message)
            : base(message)
        {
        }

        public EntityAlreadyExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}