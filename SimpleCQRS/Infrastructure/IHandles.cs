using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Interface for handling messages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandles<T>
        where T : IMessage
    {
        /// <summary>
        /// Handle the message
        /// </summary>
        /// <param name="message"></param>
        Task HandleAsync(T message);
    }
}
