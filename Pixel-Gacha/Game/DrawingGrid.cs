using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Pixel_Gacha;
using IDrawable = Pixel_Gacha.IDrawable;

namespace PixelGacha;

public class DrawingGrid : IUpdatable, IDrawable
{
    public MouseStateExtended mouse;
    
    private const int GRID_WIDTH = 150;
    private const int GRID_HEIGHT = 100;
    
    private const int SCALE = 3;

    private Texture2D gridTexture;
    private Color[] pixelData;
    
    private GraphicsDevice graphicsDevice;

    public DrawingGrid(GraphicsDevice graphicsDevice)
    {
        mouse = MouseExtended.GetState();
        
        gridTexture = new Texture2D(graphicsDevice, GRID_WIDTH, GRID_HEIGHT);
        pixelData = new Color[GRID_WIDTH * GRID_HEIGHT];
        
        this.graphicsDevice = graphicsDevice;
    }
    
    public void Update(GameTime gameTime)
    {
        mouse = MouseExtended.GetState();
        
        if (mouse.IsButtonDown(MouseButton.Left)) {
            SetPixel(mouse.X / SCALE, mouse.Y / SCALE, Color.Black);
        }

        gridTexture.SetData(pixelData);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Draw(gridTexture, new Rectangle(0, 0, GRID_WIDTH * SCALE, GRID_HEIGHT * SCALE), Color.White);
    }
    
    public void SetPixel(int x, int y, Color color)
    {
        if (x is >= 0 and < GRID_WIDTH && y is >= 0 and < GRID_HEIGHT)
        {
            pixelData[x + y * GRID_WIDTH] = color;
        }
    }
}