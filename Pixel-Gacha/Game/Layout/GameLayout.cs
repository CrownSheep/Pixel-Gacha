using System;
using Microsoft.Xna.Framework;

namespace PixelGacha.Layout;

public struct GameLayout
{
    public const int SIDEBAR_WIDTH = 340;
    public const int TOOLBAR_HEIGHT = 44;

    public Rectangle SidebarArea;
    public Rectangle ToolbarArea;
    public Rectangle GridArea;

    public static GameLayout Compute(int screenWidth, int screenHeight)
    {
        var remaining = new Rectangle(SIDEBAR_WIDTH, 0, Math.Max(0, screenWidth - SIDEBAR_WIDTH), screenHeight);
        return new GameLayout
        {
            SidebarArea = new Rectangle(0, 0, SIDEBAR_WIDTH, screenHeight),
            ToolbarArea = new Rectangle(remaining.X, 0, remaining.Width, TOOLBAR_HEIGHT),
            GridArea = new Rectangle(remaining.X, TOOLBAR_HEIGHT, remaining.Width, Math.Max(0, remaining.Height - TOOLBAR_HEIGHT))
        };
    }
}