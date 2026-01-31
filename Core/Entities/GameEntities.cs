using System;
using System.Collections.Generic;
using System.Linq;

namespace Aether48.Core.Entities;

public class Tile
{
    public Guid Id { get; } = Guid.NewGuid();
    public int Value { get; set; }
    public Tile[]? MergedFrom { get; set; }
    public (int X, int Y)? PreviousPosition { get; set; }
    public Tile(int value)
    {
        Value = value;
    }
}

public class Grid
{
    public const int Size = 4;
    private readonly Tile?[,] _cells;

    public Grid()
    {
        _cells = new Tile?[Size, Size];
    }

    public void Clear()
    {
        Array.Clear(_cells, 0, _cells.Length);
    }

    public Tile? this[int x, int y]
    {
        get => IsWithinBounds(x, y) ? _cells[x, y] : null;
        set
        {
            if (IsWithinBounds(x, y))
            {
                _cells[x, y] = value;
            }
        }
    }

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size;
    }

    public bool IsCellOccupied(int x, int y)
    {
        return IsWithinBounds(x, y) && _cells[x, y] != null;
    }

    public List<(int X, int Y)> GetEmptyCells()
    {
        var emptyCells = new List<(int X, int Y)>();

        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                if (_cells[x, y] == null)
                {
                    emptyCells.Add((x, y));
                }
            }
        }

        return emptyCells;
    }

    public int[,] GetValues()
    {
        var values = new int[Size, Size];
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                values[x, y] = _cells[x, y]?.Value ?? 0;
            }
        }
        return values;
    }
}
