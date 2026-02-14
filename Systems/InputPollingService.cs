using Aether48.Core.Events;
using Aether48.Foundation;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System;
using System.Linq;

namespace Aether48.Systems;

public class InputPollingService : IDisposable
{
    private readonly EventBus _eventBus;
    private readonly IFramework _framework;
    private readonly IKeyState _keyState;
    private readonly WindowSystem _windowSystem;

    private bool _wasUp;
    private bool _wasDown;
    private bool _wasLeft;
    private bool _wasRight;

    public InputPollingService(EventBus eventBus, IFramework framework, IKeyState keyState, WindowSystem windowSystem)
    {
        _eventBus = eventBus;
        _framework = framework;
        _keyState = keyState;
        _windowSystem = windowSystem;

        _framework.Update += OnUpdate;
    }

    public void Dispose()
    {
        _framework.Update -= OnUpdate;
    }

    private void OnUpdate(IFramework framework)
    {
        var window = _windowSystem.Windows.FirstOrDefault(w => w.WindowName == "Aether48");
        if (window is not { IsOpen: true, IsFocused: true }) return;

        bool isUp = IsKeyPressed(VirtualKey.UP) || IsKeyPressed(VirtualKey.W);
        bool isDown = IsKeyPressed(VirtualKey.DOWN) || IsKeyPressed(VirtualKey.S);
        bool isLeft = IsKeyPressed(VirtualKey.LEFT) || IsKeyPressed(VirtualKey.A);
        bool isRight = IsKeyPressed(VirtualKey.RIGHT) || IsKeyPressed(VirtualKey.D);

        if (isUp || isDown || isLeft || isRight)
        {
            _keyState.ClearAll();
        }

        if (isUp && !_wasUp) RaiseMove(MoveDirection.Up);
        else if (isDown && !_wasDown) RaiseMove(MoveDirection.Down);
        else if (isLeft && !_wasLeft) RaiseMove(MoveDirection.Left);
        else if (isRight && !_wasRight) RaiseMove(MoveDirection.Right);

        _wasUp = isUp;
        _wasDown = isDown;
        _wasLeft = isLeft;
        _wasRight = isRight;
    }

    private bool IsKeyPressed(VirtualKey key) => _keyState[key];

    private void RaiseMove(MoveDirection dir)
    {
        _eventBus.Publish(new MoveRequestEvent(dir));
    }
}
