using Newtonsoft.Json;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace SharpTS.Message
{
    /// <summary>
    /// Message argument object
    /// </summary>
    public class MessageEventArgs<TMessage> : MessageEventArgs
        where TMessage : BaseMessage
    {
        #region Properties

        /// <summary>
        /// PostData of parsed JSON
        /// </summary>
        public TMessage Message { get; }

        #endregion

        #region Ctors

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="baseArgs"></param>
        public MessageEventArgs(MessageEventArgs baseArgs)
            : base(baseArgs.CefFrame, baseArgs.RawJson, baseArgs.Callback)
        {
            this.Message = JsonConvert.DeserializeObject<MessageContainer<TMessage>>(this.RawJson).PostData;
            this.BaseMessage = this.Message;
        }

        #endregion

        #region Methods

        #endregion
    }
}