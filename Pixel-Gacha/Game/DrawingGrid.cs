using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace PixelGacha;

public class DrawingGrid : IUpdatable, IDrawable
{
    private const int SCALE = 20;
    private const int OFFSET = 80;
    
    private int GridWidth { get; }
    private int GridHeight { get; }
    
    public MouseStateExtended mouse;

    private Texture2D gridTexture;
    private Color[] pixelData;
    
    private GraphicsDevice graphicsDevice;
    
    private Point? previousGridPos = null;
    
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

        int gridX = (snappedX - OFFSET) / SCALE;
        int gridY = snappedY / SCALE;

        Point currentGridPos = new Point(gridX, gridY);

        if (mouse.IsButtonDown(MouseButton.Left))
        {
            if (previousGridPos.HasValue)
            {
                DrawLine(previousGridPos.Value, currentGridPos, Color);
            } else {
                SetPixel(gridX, gridY, Color);
            }
            previousGridPos = currentGridPos;
        } else if (mouse.IsButtonDown(MouseButton.Right)) {
            if (previousGridPos.HasValue)
            {
                DrawLine(previousGridPos.Value, currentGridPos, Color.Transparent);
            } else {
                SetPixel(gridX, gridY, Color.Transparent);
            }
            previousGridPos = currentGridPos;
        } else {
            previousGridPos = null;
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
    
    private void DrawLine(Point p0, Point p1, Color color)
    {
        int dx = Math.Abs(p1.X - p0.X);
        int dy = Math.Abs(p1.Y - p0.Y);
        int sx = p0.X < p1.X ? 1 : -1;
        int sy = p0.Y < p1.Y ? 1 : -1;
        int err = dx - dy;

        int x = p0.X;
        int y = p0.Y;

        while (true)
        {
            SetPixel(x, y, color);

            if (x == p1.X && y == p1.Y) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }
}