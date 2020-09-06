using Newtonsoft.Json;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace SharpTS.Message
{
    /// <summary>
    /// Message argument object
    /// </summary>
    public class MessageEventArgs
    {
        #region Fields

        /// <summary>
        /// Callback
        /// </summary>
        protected CefMessageRouterBrowserSide.Callback callback;

        /// <summary>
        /// Marked true when served by somebody.
        /// </summary>
        private bool served;

        /// <summary>
        /// Base message
        /// </summary>
        private BaseMessage baseMessage;

        #endregion

        #region Properties

        /// <summary>
        /// Callback
        /// </summary>
        internal CefMessageRouterBrowserSide.Callback Callback => this.callback;

        /// <summary>
        /// Base message
        /// </summary>
        public BaseMessage BaseMessage
        {
            get => this.baseMessage ??= JsonConvert.DeserializeObject<MessageContainer<BaseMessage>>(this.RawJson)?.PostData;
            protected set => this.baseMessage = value;
        }

        /// <summary>
        /// Base message
        /// </summary>
        internal CefFrame CefFrame { get; }

        /// <summary>
        /// Raw JSON message
        /// </summary>
        internal string RawJson { get; }

        /// <summary>
        /// True if served by some code/service/somebody
        /// </summary>
        public bool Served => this.served;

        #endregion

        #region Ctors

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cefFrame"></param>
        /// <param name="rawJson"></param>
        /// <param name="callback"></param>
        public MessageEventArgs(CefFrame cefFrame, string rawJson, CefMessageRouterBrowserSide.Callback callback)
        {
            this.CefFrame = cefFrame;
            this.callback = callback;
            this.RawJson = rawJson;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send success response
        /// </summary>
        /// <param name="response"></param>
        public void Success(string response = null)
        {
            if (this.served)
            {
                return;
            }

            this.served = true;

            this.callback.Success(response);
        }

        /// <summary>
        /// Send failure response
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        public void Failure(int errorCode, string errorMessage)
        {
            if (this.served)
            {
                return;
            }

            this.served = true;

            this.callback.Failure(errorCode, errorMessage);
        }

        /// <summary>
        /// Cast general message arg to arg with specific message
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public MessageEventArgs<TMessage> Cast<TMessage>() where TMessage : BaseMessage
        {
            return new MessageEventArgs<TMessage>(this);
        }

        #endregion
    }
}