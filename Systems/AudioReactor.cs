using System;
using Aether48.Audio;
using Aether48.Core.Events;
using Aether48.Foundation;

namespace Aether48.Systems;

public class AudioReactor : IDisposable
{
    private readonly EventBus _eventBus;
    private readonly AudioManager _audioManager;

    public AudioReactor(EventBus eventBus, AudioManager audioManager)
    {
        _eventBus = eventBus;
        _audioManager = audioManager;

        _eventBus.Subscribe<GameInteractionEvent>(OnInteraction);
        _eventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    public void Dispose()
    {
        _eventBus.Unsubscribe<GameInteractionEvent>(OnInteraction);
        _eventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }

    private void OnInteraction(GameInteractionEvent e)
    {
        if (e.HasMerge)
        {
            _audioManager.PlaySfx("pop.wav");
        }

        if (e.HasCollision)
        {
            _audioManager.PlaySfx("advance.wav");
        }
    }

    private void OnGameOver(GameOverEvent e)
    {
        _audioManager.PlaySfx(e.IsWin ? "win" : "gameover");
    }
}
