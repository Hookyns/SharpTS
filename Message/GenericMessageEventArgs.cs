using Newtonsoft.Json;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace SharpTS.Message
{
    /// <summary>
    /// Message argument object
    /// </summary>
    public class MessageEventArgs<TMessage> : IMessageEventArgs
        where TMessage : BaseMessage
    {
        #region Fields

        private readonly MessageEventArgs baseArgs;

        #endregion

        #region Properties

        /// <summary>
        /// PostData of parsed JSON
        /// </summary>
        public TMessage Message { get; }

        /// <inheritdoc />
        public BaseMessage BaseMessage => this.baseArgs.BaseMessage;

        /// <inheritdoc />
        public bool Served => this.baseArgs.Served;

        /// <summary>
        /// CefFrame, origin of the message
        /// </summary>
        internal CefFrame CefFrame => this.baseArgs.CefFrame;

        #endregion

        #region Ctors

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="baseArgs"></param>
        public MessageEventArgs(MessageEventArgs baseArgs)
        {
            this.baseArgs = baseArgs;
            this.Message = JsonConvert.DeserializeObject<MessageContainer<TMessage>>(baseArgs.RawJson).PostData;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Success(string response = null)
        {
            this.baseArgs.Success(response);
        }

        /// <inheritdoc />
        public void Failure(int errorCode, string errorMessage)
        {
            this.baseArgs.Failure(errorCode, errorMessage);
        }

        #endregion
    }
}