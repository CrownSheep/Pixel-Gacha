using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace PixelGacha.Canvas;

public class DrawingCanvas : IUpdatable, IDrawable
{
    private const int SCALE = 42;
    private const int MAX_UNDO_STEPS = 20;

    private Rectangle bounds;
    public int GridWidth { get; private set; }
    public int GridHeight { get; private set; }

    private Texture2D gridTexture;
    private Color[] pixelData;
    
    private GraphicsDevice graphicsDevice;
    private PlayerInventory inventory;
    
    private Stack<Color[]> undoStack = new();
    private bool strokeDirty = false;
    
    private Point? previousGridPos = null;

    public Color DrawColor { get; set; } = Color.Black;

    public DrawingCanvas(GraphicsDevice graphicsDevice, PlayerInventory inventory)
    {
        this.graphicsDevice = graphicsDevice;
        this.inventory = inventory;
    }
    
    private void BeginStrokeIfNeeded()
    {
        if (strokeDirty) return;
        strokeDirty = true;

        if (undoStack.Count >= MAX_UNDO_STEPS)
        {
            var trimmed = undoStack.Take(MAX_UNDO_STEPS - 1).ToArray();
            undoStack = new Stack<Color[]>(trimmed.Reverse());
        }
        undoStack.Push((Color[])pixelData.Clone());
    }

    public void Resize(Rectangle newBounds)
    {
        bounds = newBounds;

        int newGridWidth = Math.Max(1, bounds.Width / SCALE);
        int newGridHeight = Math.Max(1, bounds.Height / SCALE);
        var newPixelData = new Color[newGridWidth * newGridHeight];

        if (pixelData != null)
        {
            int copyWidth = Math.Min(GridWidth, newGridWidth);
            int copyHeight = Math.Min(GridHeight, newGridHeight);
            for (int y = 0; y < copyHeight; y++)
                for (int x = 0; x < copyWidth; x++)
                    newPixelData[x + y * newGridWidth] = pixelData[x + y * GridWidth];
        }

        GridWidth = newGridWidth;
        GridHeight = newGridHeight;
        pixelData = newPixelData;

        gridTexture?.Dispose();
        gridTexture = new Texture2D(graphicsDevice, GridWidth, GridHeight);
        gridTexture.SetData(pixelData);
    }

    public void Update(GameTime gameTime)
    {
        var mouse = MouseExtended.GetState();
        bool mouseOverGrid = bounds.Contains(mouse.Position);

        int gridX = Math.Clamp((int)MathF.Floor((float)(mouse.X - bounds.X) / SCALE), 0, GridWidth - 1);
        int gridY = Math.Clamp((int)MathF.Floor((float)(mouse.Y - bounds.Y) / SCALE), 0, GridHeight - 1);
        var currentGridPos = new Point(gridX, gridY);

        bool leftDown = mouse.IsButtonDown(MouseButton.Left);
        bool rightDown = mouse.IsButtonDown(MouseButton.Right);

        if ((leftDown || rightDown) && mouseOverGrid)
        {
            BeginStrokeIfNeeded();
            var color = leftDown ? DrawColor : Color.Transparent;

            if (previousGridPos.HasValue) DrawLine(previousGridPos.Value, currentGridPos, color);
            else SetPixel(gridX, gridY, color);

            previousGridPos = currentGridPos;
        }
        else
        {
            previousGridPos = null;
            strokeDirty = false;
        }

        gridTexture.SetData(pixelData);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        var mouse = MouseExtended.GetState();
        int hoverX = (int)MathF.Floor((float)(mouse.X - bounds.X) / SCALE);
        int hoverY = (int)MathF.Floor((float)(mouse.Y - bounds.Y) / SCALE);

        bool leftDown = mouse.IsButtonDown(MouseButton.Left);
        bool rightDown = mouse.IsButtonDown(MouseButton.Right);

        spriteBatch.Draw(gridTexture, new Rectangle(bounds.X, bounds.Y, GridWidth * SCALE, GridHeight * SCALE), Color.White);
        spriteBatch.DrawRectangle(bounds, Color.Black);

        if (!leftDown && !rightDown && InBounds(hoverX, hoverY))
        {
            var hoverRect = new Rectangle(bounds.X + hoverX * SCALE, bounds.Y + hoverY * SCALE, SCALE, SCALE);
            spriteBatch.FillRectangle(hoverRect, DrawColor);
        }
    }

    public void SetPixel(int x, int y, Color color)
    {
        if (!InBounds(x, y)) return;

        bool wasEmpty = pixelData[x + y * GridWidth] == Color.Transparent;
        pixelData[x + y * GridWidth] = color;

        if (wasEmpty)
        {
            var matchedColor = ColorDatabase.AllColors.FirstOrDefault(c => c.Value == DrawColor);
    
            int ownedCount = 1;
            if (matchedColor != null && inventory.Owned.TryGetValue(matchedColor.Name, out int count))
            {
                ownedCount = count;
            }

            int totalReward = PlayerInventory.CURRENCY_PER_PIXEL * ownedCount;

            inventory.AddCurrency(totalReward);
        }
    }

    private bool InBounds(int x, int y) => x >= 0 && x < GridWidth && y >= 0 && y < GridHeight;

    private void DrawLine(Point p0, Point p1, Color color)
    {
        int dx = Math.Abs(p1.X - p0.X), dy = Math.Abs(p1.Y - p0.Y);
        int sx = p0.X < p1.X ? 1 : -1, sy = p0.Y < p1.Y ? 1 : -1;
        int err = dx - dy, x = p0.X, y = p0.Y;

        while (true)
        {
            SetPixel(x, y, color);
            if (x == p1.X && y == p1.Y) break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy; x += sx;
            }

            if (e2 < dx)
            {
                err += dx; y += sy;
            }
        }
    }
    
    public void Undo()
    {
        if (undoStack.Count == 0) return;
        pixelData = undoStack.Pop();
        gridTexture.SetData(pixelData);
    }

    public void Clear()
    {
        BeginStrokeIfNeeded();
        Array.Clear(pixelData, 0, pixelData.Length);
        gridTexture.SetData(pixelData);
        strokeDirty = false;
    }

    public Color[] GetPixelDataSnapshot() => (Color[])pixelData.Clone();
    
    public void LoadPixelData(int savedWidth, int savedHeight, Color[] savedData)
    {
        Array.Clear(pixelData, 0, pixelData.Length);

        int copyWidth = Math.Min(GridWidth, savedWidth);
        int copyHeight = Math.Min(GridHeight, savedHeight);

        for (int y = 0; y < copyHeight; y++)
        for (int x = 0; x < copyWidth; x++)
            pixelData[x + y * GridWidth] = savedData[x + y * savedWidth];

        gridTexture.SetData(pixelData);
        undoStack.Clear();
    }
}