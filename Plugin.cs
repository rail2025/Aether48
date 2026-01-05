using System;
using Aether48.Audio;
using Aether48.Core;
using Aether48.Foundation;
using Aether48.Systems;
using Aether48.UI;
using Aether48.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Aether48;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IKeyState KeyState { get; private set; } = null!;

    private const string CommandName = "/a48";

    public readonly ServiceContainer Services;
    public readonly WindowSystem WindowSystem = new("Aether48");

    public Configuration Configuration { get; init; }
    public AudioManager AudioManager { get; init; }

    private readonly MainWindow _mainWindow;
    private readonly ConfigWindow _configWindow;
    private readonly AboutWindow _aboutWindow;
    public readonly TitleWindow TitleWindow;

    public Plugin()
    {
        Services = new();

        Services.Register(PluginInterface);
        Services.Register(CommandManager);
        Services.Register(ClientState);
        Services.Register(Framework);
        Services.Register(TextureProvider);
        Services.Register(Log);
        Services.Register(KeyState);

        var eventBus = new EventBus(Log);
        Services.Register(eventBus);
        Services.Register(WindowSystem);

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);
        Services.Register(Configuration);

        AudioManager = new(Configuration);
        Services.Register(AudioManager);

        var textureManager = new TextureManager(TextureProvider, Log);
        Services.Register(textureManager);

        var inputService = new InputPollingService(eventBus, Framework, KeyState, WindowSystem);
        Services.Register(inputService);

        var gameEngine = new GameEngine(eventBus, Framework, Configuration);
        Services.Register(gameEngine);
        Services.Register<IGridDataSource>(gameEngine);

        var renderService = new RenderService(eventBus, textureManager, Configuration);
        Services.Register(renderService);

        var audioReactor = new AudioReactor(eventBus, AudioManager);
        Services.Register(audioReactor);

        var themeManager = new ThemeManager();
        Services.Register(themeManager);

        _mainWindow = new(this);
        _configWindow = new(this, AudioManager);
        _aboutWindow = new();
        TitleWindow = new(this);

        WindowSystem.AddWindow(_mainWindow);
        WindowSystem.AddWindow(_configWindow);
        WindowSystem.AddWindow(_aboutWindow);
        WindowSystem.AddWindow(TitleWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the Aether48 game window."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleTitleUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= DrawUI;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleTitleUI;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;

        CommandManager.RemoveHandler(CommandName);
        WindowSystem.RemoveAllWindows();

        _mainWindow.Dispose();
        _configWindow.Dispose();
        _aboutWindow.Dispose();
        TitleWindow.Dispose();
        AudioManager.Dispose();
        Services.Dispose();
    }

    private void OnCommand(string command, string args) => ToggleTitleUI();
    private void DrawUI() => WindowSystem.Draw();
    public void ToggleTitleUI() => TitleWindow.Toggle();
    public void ToggleMainUI() => _mainWindow.IsOpen = !_mainWindow.IsOpen;
    public void ToggleConfigUI() => _configWindow.Toggle();
    public void ToggleAboutUI() => _aboutWindow.Toggle();
}
