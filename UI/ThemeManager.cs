using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace Aether48.UI;

public class ThemeManager
{
    public bool IsDarkMode { get; private set; }

    public void ToggleTheme() => IsDarkMode = !IsDarkMode;

    public Vector4 GridBackground => IsDarkMode
        ? new(0.12f, 0.12f, 0.12f, 1.0f)
        : new(0.73f, 0.68f, 0.63f, 1.0f);

    public Vector4 EmptySlotColor => IsDarkMode
        ? new(0.24f, 0.24f, 0.24f, 1.0f)
        : new(0.80f, 0.75f, 0.71f, 1.0f);

    public Vector4 BoardText => IsDarkMode
        ? new(0.90f, 0.90f, 0.90f, 1.0f)
        : new(0.47f, 0.43f, 0.39f, 1.0f);

    public Vector4 TextColorLight => new(0.47f, 0.43f, 0.39f, 1.0f);
    public Vector4 TextColorDark => new(0.97f, 0.96f, 0.94f, 1.0f);

    private readonly Dictionary<int, Vector4> _tileColors = new()
    {
        { 2,    new(0.93f, 0.89f, 0.85f, 1.0f) },
        { 4,    new(0.93f, 0.88f, 0.78f, 1.0f) },
        { 8,    new(0.95f, 0.69f, 0.47f, 1.0f) },
        { 16,   new(0.96f, 0.58f, 0.39f, 1.0f) },
        { 32,   new(0.96f, 0.48f, 0.37f, 1.0f) },
        { 64,   new(0.96f, 0.37f, 0.23f, 1.0f) },
        { 128,  new(0.93f, 0.81f, 0.45f, 1.0f) },
        { 256,  new(0.93f, 0.80f, 0.38f, 1.0f) },
        { 512,  new(0.93f, 0.78f, 0.31f, 1.0f) },
        { 1024, new(0.93f, 0.77f, 0.25f, 1.0f) },
        { 2048, new(0.93f, 0.76f, 0.18f, 1.0f) },
    };

    public Vector4 GetTileColor(int value)
    {
        return _tileColors.TryGetValue(value, out var color) ? color : new Vector4(0.24f, 0.23f, 0.20f, 1.0f);
    }

    public Vector4 GetTextColor(int value)
    {
        return value <= 4 ? TextColorLight : TextColorDark;
    }
}
