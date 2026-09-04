using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace PixelGacha;

public class DrawingGrid : IUpdatable, IDrawable
{
    public MouseStateExtended mouse;
    
    private int GridWidth { get; }
    private int GridHeight { get; }
    
    private const int SCALE = 20;
    private const int OFFSET = 80;

    private Texture2D gridTexture;
    private Color[] pixelData;
    
    private GraphicsDevice graphicsDevice;
    private Color Color => Color.Red;

    public DrawingGrid(GraphicsDevice graphicsDevice)
    {
        mouse = MouseExtended.GetState();
        
        GridWidth = graphicsDevice.Viewport.Width - OFFSET;
        GridHeight = graphicsDevice.Viewport.Height;
        
        gridTexture = new Texture2D(graphicsDevice, GridWidth, GridHeight);
        pixelData = new Color[GridWidth * GridHeight];
        
        this.graphicsDevice = graphicsDevice;
    }
    
    public void Update(GameTime gameTime)
    {
        mouse = MouseExtended.GetState();
        
        int snappedX = (int)MathF.Floor((float)mouse.X / SCALE) * SCALE;
        int snappedY = (int)MathF.Floor((float)mouse.Y / SCALE) * SCALE;

        if (mouse.IsButtonDown(MouseButton.Left)) {
            SetPixel((snappedX - OFFSET) / SCALE, (snappedY) / SCALE, Color);
        }
        else if(mouse.IsButtonDown(MouseButton.Right))
        {
            SetPixel((snappedX - OFFSET) / SCALE, (snappedY) / SCALE, Color.Transparent);
        }

        gridTexture.SetData(pixelData);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        int snappedX = (int)MathF.Floor((float)mouse.X / SCALE) * SCALE;
        int snappedY = (int)MathF.Floor((float)mouse.Y / SCALE) * SCALE;

        if (!mouse.IsButtonDown(MouseButton.Left) && InBounds((snappedX - OFFSET) / SCALE, (snappedY) / SCALE))
        {
            spriteBatch.FillRectangle(new Rectangle(snappedX, snappedY, SCALE, SCALE), Color * 0.3f);
        }

        spriteBatch.Draw(gridTexture, new Rectangle(OFFSET, 0, GridWidth * SCALE, GridHeight * SCALE), Color.White);
        spriteBatch.DrawRectangle(new Rectangle(OFFSET, 0, GridWidth, GridHeight), Color.Black);
    }
    
    public void SetPixel(int x, int y, Color color)
    {
        if (InBounds(x, y))
        {
            pixelData[x + y * GridWidth] = color;
        }
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && x < GridWidth && y >= 0 && y < GridHeight;
    }
}