using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;
        throw new Exception($"Service {typeof(T).Name} not registered.");
    }

    public static bool TryGet<T>(out T service)
    {
        if (_services.TryGetValue(typeof(T), out var obj))
        {
            service = (T)obj;
            return true;
        }
        service = default;
        return false;
    }

    public static void Clear() => _services.Clear();
}