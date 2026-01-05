namespace Aether48.Core;

public class MockGridDataSource : IGridDataSource
{
    private readonly int[,] _grid = new int[4, 4];

    public int Score => 12345;
    public int HighScore => 99999;

    public MockGridDataSource()
    {
        // Hardcode a pattern to test rendering colors
        _grid[0, 0] = 2;
        _grid[1, 0] = 4;
        _grid[2, 0] = 8;
        _grid[3, 0] = 16;

        _grid[0, 1] = 32;
        _grid[1, 1] = 64;
        _grid[2, 1] = 128;
        _grid[3, 1] = 256;

        _grid[0, 2] = 512;
        _grid[1, 2] = 1024;
        _grid[2, 2] = 2048;
        _grid[3, 2] = 0; // Empty
    }

    public int GetTileValue(int x, int y)
    {
        if (x < 0 || x > 3 || y < 0 || y > 3) return 0;
        return _grid[x, y];
    }
}
