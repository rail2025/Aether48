using System;
using Aether48.Audio;
using Aether48.Core.Events;
using Aether48.Foundation;

namespace Aether48.Systems;

public class AudioReactor : IDisposable
{
    private readonly EventBus _eventBus;
    private readonly AudioManager _audioManager;
    private int _lastScore;

    public AudioReactor(EventBus eventBus, AudioManager audioManager)
    {
        _eventBus = eventBus;
        _audioManager = audioManager;

        _eventBus.Subscribe<GridUpdatedEvent>(OnGridUpdated);
        _eventBus.Subscribe<GameOverEvent>(OnGameOver);
        _eventBus.Subscribe<GameResetEvent>(OnReset);
    }

    public void Dispose()
    {
        _eventBus.Unsubscribe<GridUpdatedEvent>(OnGridUpdated);
        _eventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        _eventBus.Unsubscribe<GameResetEvent>(OnReset);
    }

    private void OnGridUpdated(GridUpdatedEvent e)
    {
        if (e.Score > _lastScore)
        {
            _audioManager.PlaySfx("pop");
        }
        else
        {
            _audioManager.PlaySfx("move");
        }

        _lastScore = e.Score;
    }

    private void OnGameOver(GameOverEvent e)
    {
        _audioManager.PlaySfx(e.IsWin ? "win" : "gameover");
    }

    private void OnReset(GameResetEvent e)
    {
        _lastScore = 0;
    }
}
