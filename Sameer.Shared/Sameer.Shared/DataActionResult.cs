using System;

namespace Sameer.Shared
{
    public class DataActionResult<T> where T : class
    {
        public T Entity { get; private set; }

        public RepositoryActionStatus Status { get; private set; }

        public Exception Exception { get; private set; }

        public DataActionResult(T entity, RepositoryActionStatus status)
        {
            Entity = entity;
            Status = status;
        }

        public DataActionResult(T entity, RepositoryActionStatus status, Exception exception)
            : this(entity, status)
        {
            Exception = exception;
        }
    }
}
