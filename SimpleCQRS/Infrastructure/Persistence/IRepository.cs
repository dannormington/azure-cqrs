using System;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure.Persistence
{
    /// <summary>
    /// Represents an interface to a repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<out T> where T : class, IAggregateRoot, new()
    {
        /// <summary>
        /// Save the aggregate
        /// </summary>
        /// <param name="aggregate"></param>
        Task SaveAsync(IAggregateRoot aggregate);

        /// <summary>
        /// Get the aggregate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(Guid id);
    }
}
