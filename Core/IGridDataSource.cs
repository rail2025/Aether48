namespace Aether48.Core;

public interface IGridDataSource
{
    int Score { get; }
    int HighScore { get; }
    int GetTileValue(int x, int y);
}
