using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;

namespace Aether48.Foundation;

public class EventBus
{
    private readonly Dictionary<Type, List<object>> _subscribers = new();
    private readonly IPluginLog? _log;

    public EventBus(IPluginLog? log = null)
    {
        _log = log;
    }

    public void Subscribe<T>(Action<T> handler)
    {
        if (!_subscribers.TryGetValue(typeof(T), out var handlers))
        {
            handlers = new List<object>();
            _subscribers[typeof(T)] = handlers;
        }
        handlers.Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        if (_subscribers.TryGetValue(typeof(T), out var handlers))
        {
            handlers.Remove(handler);
        }
    }

    public void Publish<T>(T eventMessage)
    {
        if (!_subscribers.TryGetValue(typeof(T), out var handlers)) return;

        foreach (var handlerObj in handlers.ToArray())
        {
            if (handlerObj is Action<T> handler)
            {
                try
                {
                    handler(eventMessage);
                }
                catch (Exception ex)
                {
                    _log?.Error(ex, $"Error handling event {typeof(T).Name}");
                }
            }
        }
    }
}
