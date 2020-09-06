namespace SharpTS.Message
{
    public interface IMessageEventArgs
    {
        /// <summary>
        /// Base message
        /// </summary>
        BaseMessage BaseMessage { get; }

        /// <summary>
        /// True if served by some code/service/somebody
        /// </summary>
        bool Served { get; }

        /// <summary>
        /// Send success response
        /// </summary>
        /// <param name="response"></param>
        void Success(string response = null);

        /// <summary>
        /// Send failure response
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        void Failure(int errorCode, string errorMessage);
    }
}