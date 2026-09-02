using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;

namespace PixelGacha;

/// <summary>
/// This is the main type for your game.
/// </summary>
public class PixelGachaGame : Game
{
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;
    
    private BitmapFont font;
    
    protected DrawingGrid grid;

    public PixelGachaGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Pixel Gacha";

        graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        if (TitleContainer.Platform == TitlePlatform.iOS)
            graphics.IsFullScreen = true;
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = new SpriteBatch(GraphicsDevice);
        BitmapFont font = Content.Load<BitmapFont>("font");
        this.font = font;

        grid = new DrawingGrid(GraphicsDevice);
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        MouseExtended.Update();
        KeyboardExtended.Update();

        grid.Update(gameTime);

        base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        grid.Draw(spriteBatch, gameTime);
        spriteBatch.End();

        base.Draw(gameTime);
    }
}