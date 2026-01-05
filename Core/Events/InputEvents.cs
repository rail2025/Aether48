using System.Numerics;

namespace Aether48.Core.Events;

public enum MoveDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

public record MoveRequestEvent(MoveDirection Direction);

public record GameActionCommand(string ActionName);
