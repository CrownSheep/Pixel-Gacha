using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using PixelGacha.Canvas;
using PixelGacha.Layout;

namespace PixelGacha;

public class PixelGachaGame : Game
{
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;
    public static BitmapFont font;

    protected DrawingCanvas Canvas;
    protected Sidebar.Sidebar sidebar;
    protected PlayerInventory inventory;
    protected ArtworkGallery gallery;
    protected CanvasToolbar toolbar;

    private bool layoutDirty = true;

    public PixelGachaGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Pixel Gacha";

        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

        Window.AllowUserResizing = false;
        Window.ClientSizeChanged += (_, _) => layoutDirty = true;

        if (TitleContainer.Platform == TitlePlatform.iOS)
            graphics.IsFullScreen = true;
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<BitmapFont>("font");

        inventory = new PlayerInventory();
        Canvas = new DrawingCanvas(GraphicsDevice, inventory);
        gallery = new ArtworkGallery();
        sidebar = new Sidebar.Sidebar(GraphicsDevice, font, inventory, Canvas, gallery);
        toolbar = new CanvasToolbar(Canvas, gallery);

        ApplyLayout();
    }

    private void ApplyLayout()
    {
        var layout = GameLayout.Compute(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        Canvas.Resize(layout.GridArea);
        toolbar.SetBounds(layout.ToolbarArea);
        sidebar.SetBounds(layout.SidebarArea);
    }

    protected override void Update(GameTime gameTime)
    {
        if (layoutDirty)
        {
            ApplyLayout();
            layoutDirty = false;
        }

        MouseExtended.Update();
        KeyboardExtended.Update();

        toolbar.Update(gameTime);
        Canvas.Update(gameTime);
        Canvas.DrawColor = sidebar.Palette.SelectedColor;
        sidebar.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Canvas.Draw(spriteBatch, gameTime);
        toolbar.Draw(spriteBatch, gameTime);
        sidebar.Draw(spriteBatch, gameTime);
        spriteBatch.End();

        base.Draw(gameTime);
    }
}