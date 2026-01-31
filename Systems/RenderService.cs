using System;
using Aether48.Core.Events;
using Aether48.Foundation;
using Aether48.UI;

namespace Aether48.Systems;

public class RenderService : IDisposable
{
    private readonly EventBus _eventBus;
    private readonly TextureManager _textureManager;
    private readonly Configuration _config;

    public RenderService(EventBus eventBus, TextureManager textureManager, Configuration config)
    {
        _eventBus = eventBus;
        _textureManager = textureManager;
        _config = config;

    }

    public void Dispose()
    {}
}
