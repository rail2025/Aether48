using System.Collections.Generic;

namespace Aether48.Core.Events;

public record struct GridUpdatedEvent(int[,] GridValues, int Score, int HighScore);

public record struct GameOverEvent(bool IsWin, int FinalScore);

public record struct GameResetEvent;
