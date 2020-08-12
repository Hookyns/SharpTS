using System;

namespace SharpTS.Component.Attributes
{
    /// <summary>
    /// Mark Component as keep-alive
    /// </summary>
    /// <remarks>
    /// Keep-alive keep component instance in memory with its state, component is only deactivated and can be reactivated.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class KeepAliveAttribute : Attribute
    {
    }
}