using System;
using System.Collections.Generic;
using Game.Application.Interface;

public class ContextAccessor : IContextAccessor
{
    private readonly Dictionary<Type, object> _contexts = new Dictionary<Type, object>();

    public void SetContextAccessor<TContext>(TContext context) where TContext : class
    {
        _contexts[typeof(TContext)] = context;
    }

    public TContext GetContextAccessor<TContext>() where TContext : class
    {
        if (_contexts.TryGetValue(typeof(TContext), out var ctx))
            return (TContext)ctx;
        return null;
    }
}