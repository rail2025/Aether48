using Aether48.Core;
using Aether48.Core.Events;
using Aether48.Foundation;
using Aether48.UI;
using Aether48.Audio;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;

namespace Aether48.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly IGridDataSource _dataSource;
    private readonly ThemeManager _themeManager;
    private readonly AudioManager _audioManager;
    private readonly EventBus _eventBus;
    private readonly Configuration _configuration;

    private const float GridPadding = 10f;
    private const float CellSpacing = 8f;
    private const int GridSize = 4;

    public MainWindow(Plugin plugin) : base("Aether48")
    {
        _dataSource = plugin.Services.Get<IGridDataSource>();
        _themeManager = plugin.Services.Get<ThemeManager>();
        _audioManager = plugin.Services.Get<AudioManager>();
        _eventBus = plugin.Services.Get<EventBus>();
        _configuration = plugin.Services.Get<Configuration>();

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 400),
            MaximumSize = new Vector2(600, 800)
        };
    }

    public void Dispose()
    {
    }
    public override void OnOpen()
    {
        _audioManager.StartBgmPlaylist();
        base.OnOpen();
    }
    public override void OnClose()
    {
        _audioManager.EndPlaylist();
        base.OnClose();
    }

    public override void Draw()
    {
        DrawScoreBoard();
        DrawControls();
        DrawGrid();
    }

    private void DrawScoreBoard()
    {
        var scoreText = $"Score: {_dataSource.Score}";
        var highText = $"Best: {_dataSource.HighScore}";

        ImGui.TextColored(_themeManager.BoardText, "");

        var availWidth = ImGui.GetContentRegionAvail().X;
        var scoreWidth = ImGui.CalcTextSize(scoreText).X;
        var highWidth = ImGui.CalcTextSize(highText).X;

        ImGui.SameLine(availWidth - scoreWidth - highWidth - 20);
        ImGui.TextColored(_themeManager.BoardText, scoreText);
        ImGui.SameLine(availWidth - highWidth);
        ImGui.TextDisabled(highText);

        ImGui.Separator();
        ImGui.Spacing();
    }

    private void DrawControls()
    {
        if (ImGui.Button("New Game"))
        {
            _eventBus.Publish(new GameResetEvent());
        }

        ImGui.SameLine();

        if (ImGui.Button(_themeManager.IsDarkMode ? "Light Mode" : "Dark Mode"))
        {
            _themeManager.ToggleTheme();
        }

        ImGui.Spacing();
        ImGui.Separator();

        var muteBgm = _configuration.IsBgmMuted;
        if (ImGui.Checkbox("BGM", ref muteBgm))
        {
            _configuration.IsBgmMuted = muteBgm;
            _audioManager.UpdateBgmState();
            _configuration.Save();
        }

        ImGui.SameLine();
        var muteSfx = _configuration.IsSfxMuted;
        if (ImGui.Checkbox("SFX", ref muteSfx))
        {
            _configuration.IsSfxMuted = muteSfx;
            _configuration.Save();
        }

        ImGui.SameLine();
        if (ImGui.ArrowButton("##prev", ImGuiDir.Left)) _audioManager.PlayPreviousTrack();

        ImGui.SameLine();
        if (ImGui.ArrowButton("##next", ImGuiDir.Right)) _audioManager.PlayNextTrack();

        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);

        var vol = _configuration.MusicVolume;
        if (ImGui.SliderFloat("##vol", ref vol, 0.0f, 1.0f, "Vol %.2f"))
        {
            _configuration.MusicVolume = vol;
            _audioManager.SetMusicVolume(vol);
            _configuration.Save();
        }
    }

    private void DrawGrid()
    {
        var drawList = ImGui.GetWindowDrawList();
        var p = ImGui.GetCursorScreenPos();
        var avail = ImGui.GetContentRegionAvail();

        var totalSpacing = (GridSize - 1) * CellSpacing;
        var boardWidth = avail.X;
        var tileSize = (boardWidth - totalSpacing) / GridSize;

        var boardSize = new Vector2(boardWidth, boardWidth);
        drawList.AddRectFilled(p, p + boardSize, ImGui.ColorConvertFloat4ToU32(_themeManager.GridBackground), 6f);

        for (var y = 0; y < GridSize; y++)
        {
            for (var x = 0; x < GridSize; x++)
            {
                var value = _dataSource.GetTileValue(x, y);

                var xPos = p.X + (x * (tileSize + CellSpacing));
                var yPos = p.Y + (y * (tileSize + CellSpacing));
                var start = new Vector2(xPos, yPos);
                var end = start + new Vector2(tileSize, tileSize);

                var tileColor = value == 0 ? _themeManager.EmptySlotColor : _themeManager.GetTileColor(value);
                var textColor = _themeManager.GetTextColor(value);

                drawList.AddRectFilled(start, end, ImGui.ColorConvertFloat4ToU32(tileColor), 4f);

                if (value > 0)
                {
                    var text = value.ToString();
                    var textSize = ImGui.CalcTextSize(text);
                    var textPos = start + (new Vector2(tileSize, tileSize) - textSize) * 0.5f;

                    drawList.AddText(textPos, ImGui.ColorConvertFloat4ToU32(textColor), text);
                }
            }
        }
    }
}
