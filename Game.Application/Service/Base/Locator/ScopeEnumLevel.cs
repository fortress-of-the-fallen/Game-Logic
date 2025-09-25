using System;

namespace Game.Application.Service.Base.Locator
{
    public static class ScopeEnumLevel
    {
        public static int GetScopeLevel(this Scope scope)
        {
            switch (scope)
            {
                case Scope.Singleton:
                    return 2;
                case Scope.Scope:
                    return 1;
                case Scope.Transient:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }
    }
}