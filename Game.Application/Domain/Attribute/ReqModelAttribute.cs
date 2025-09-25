using System;

[AttributeUsage(AttributeTargets.Class)]
public class ReqModelAttribute : Attribute
{
    public Type RequestType { get; }
    public ReqModelAttribute(Type requestType)
    {
        RequestType = requestType;
    }
}
