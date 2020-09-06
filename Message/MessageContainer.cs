namespace SharpTS.Message
{
    /// <summary>
    /// Container for Chromely interop message
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    internal class MessageContainer<TData>
    {
        public string Method { get; set; }

        public string Url { get; set; }

        public string Parameters { get; set; }

        public TData PostData { get; set; }
    }
}