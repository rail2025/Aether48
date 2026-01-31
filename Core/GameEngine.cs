using System;
using System.Collections.Generic;
using System.Linq;
using Aether48.Core.Entities;
using Aether48.Core.Events;
using Aether48.Foundation;

namespace Aether48.Core;

public class GameEngine : IGridDataSource, IDisposable
{
    private readonly EventBus _eventBus;
    private readonly Configuration _config;
    private readonly Random _random = new();

    public Grid Grid { get; } = new();
    public int Score { get; private set; }
    public int HighScore => _config.HighScore;

    public GameEngine(EventBus eventBus, Dalamud.Plugin.Services.IFramework _, Configuration config)
    {
        _eventBus = eventBus;
        _config = config;

        _eventBus.Subscribe<MoveRequestEvent>(OnMoveRequest);
        _eventBus.Subscribe<GameResetEvent>(OnResetRequest);

        Reset();
    }

    public int GetTileValue(int x, int y) => Grid[x, y]?.Value ?? 0;

    public void Dispose()
    {
        _eventBus.Unsubscribe<MoveRequestEvent>(OnMoveRequest);
        _eventBus.Unsubscribe<GameResetEvent>(OnResetRequest);
    }

    private void OnMoveRequest(MoveRequestEvent e) => Move(e.Direction);
    private void OnResetRequest(GameResetEvent e) => Reset();

    public void Reset()
    {
        Grid.Clear();
        Score = 0;
        SpawnTile();
        SpawnTile();
        PublishUpdate();
    }

    public void Move(MoveDirection direction)
    {
        var vector = GetVector(direction);
        var traversalX = BuildTraversals(vector.X);
        var traversalY = BuildTraversals(vector.Y);

        var moved = false;
        var scoreIncrease = 0;
        var turnHasMerge = false;
        var turnHasCollision = false;

        PrepareTiles();

        foreach (var x in traversalX)
        {
            foreach (var y in traversalY)
            {
                var cell = Grid[x, y];
                if (cell == null) continue;

                var (farthest, next) = FindFarthestPosition(x, y, vector);

                if (next.X != -1 && Grid[next.X, next.Y]?.Value == cell.Value && Grid[next.X, next.Y]?.MergedFrom == null)
                {
                    var merged = new Tile(cell.Value * 2)
                    {
                        MergedFrom = new[] { Grid[next.X, next.Y]!, cell }
                    };

                    if (merged.Value == 1024 && !_config.UnlockedBonusTracks.Contains(1))
                    {
                        _config.UnlockedBonusTracks.Add(1);
                        _config.Save();
                    }

                    Grid[next.X, next.Y] = merged;
                    Grid[x, y] = null;

                    cell.PreviousPosition = (next.X, next.Y);
                    scoreIncrease += merged.Value;
                    moved = true;
                    turnHasMerge = true;
                }
                else
                {
                    if (farthest.X == x && farthest.Y == y) continue;

                    // If we moved, check if we stopped because of a collision with another tile
                    if (Grid.IsWithinBounds(next.X, next.Y))
                    {
                        turnHasCollision = true;
                    }

                    Grid[farthest.X, farthest.Y] = cell;
                    Grid[x, y] = null;
                    moved = true;
                }
            }
        }

        if (moved)
        {
            Score += scoreIncrease;
            if (Score > _config.HighScore)
            {
                _config.HighScore = Score;
                _config.Save();
            }

            _eventBus.Publish(new GameInteractionEvent(turnHasMerge, turnHasCollision));

            SpawnTile();
            PublishUpdate();

            if (!MovesAvailable())
            {
                _eventBus.Publish(new GameOverEvent(false, Score));
            }
        }
    }

    private void SpawnTile()
    {
        var empty = Grid.GetEmptyCells();
        if (empty.Count == 0) return;
        var (x, y) = empty[_random.Next(empty.Count)];
        Grid[x, y] = new Tile(_random.NextDouble() < 0.9 ? 2 : 4);
    }

    private void PrepareTiles()
    {
        for (var x = 0; x < Grid.Size; x++)
        {
            for (var y = 0; y < Grid.Size; y++)
            {
                if (Grid[x, y] is { } tile)
                {
                    tile.MergedFrom = null;
                    tile.PreviousPosition = null;
                }
            }
        }
    }

    private (int X, int Y) GetVector(MoveDirection dir) => dir switch
    {
        MoveDirection.Up => (0, -1),
        MoveDirection.Down => (0, 1),
        MoveDirection.Left => (-1, 0),
        MoveDirection.Right => (1, 0),
        _ => (0, 0)
    };

    private List<int> BuildTraversals(int vector)
    {
        var list = Enumerable.Range(0, Grid.Size).ToList();
        if (vector == 1) list.Reverse();
        return list;
    }

    private ((int X, int Y) Farthest, (int X, int Y) Next) FindFarthestPosition(int x, int y, (int X, int Y) vector)
    {
        int prevX, prevY;
        do
        {
            prevX = x;
            prevY = y;
            x += vector.X;
            y += vector.Y;
        }
        while (Grid.IsWithinBounds(x, y) && !Grid.IsCellOccupied(x, y));

        return ((prevX, prevY), (x, y));
    }

    private bool MovesAvailable() => Grid.GetEmptyCells().Any() || TileMatchesAvailable();

    private bool TileMatchesAvailable()
    {
        for (var x = 0; x < Grid.Size; x++)
        {
            for (var y = 0; y < Grid.Size; y++)
            {
                var tile = Grid[x, y];
                if (tile == null) continue;

                foreach (var dir in new[] { MoveDirection.Down, MoveDirection.Right })
                {
                    var v = GetVector(dir);
                    var tx = x + v.X;
                    var ty = y + v.Y;

                    if (Grid.IsWithinBounds(tx, ty) && Grid[tx, ty]?.Value == tile.Value)
                        return true;
                }
            }
        }
        return false;
    }

    private void PublishUpdate() => _eventBus.Publish(new GridUpdatedEvent(Grid.GetValues(), Score, HighScore));
}
