namespace Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UsesHandlerAttribute : Attribute
    {
        public Type HandlerType { get; }
        public UsesHandlerAttribute(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }
}
