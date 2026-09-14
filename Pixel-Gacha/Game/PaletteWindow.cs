using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace PixelGacha;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;

public class PaletteWindow : IPanel
{
    private const int SWATCH_SIZE = 28;
    private const int PADDING = 8;
    
    private BitmapFont font;
    private PlayerInventory inventory;
    private Rectangle bounds;

    private int columns = 1;

    public Color SelectedColor { get; private set; } = Color.Black;
    private string selectedColorName = null;

    private List<Rectangle> swatchRects = new();
    private List<GachaColor> displayedColors = new();

    public PaletteWindow(GraphicsDevice graphicsDevice, BitmapFont font, PlayerInventory inventory)
    {
        this.font = font;
        this.inventory = inventory;
    }

    public void SetBounds(Rectangle newBounds)
    {
        bounds = newBounds;
        columns = Math.Max(1, (bounds.Width - PADDING) / (SWATCH_SIZE + PADDING));
        RebuildLayout();
    }

    public void Update(GameTime gameTime)
    {
        RebuildLayout();

        var mouse = MouseExtended.GetState();
        if (mouse.WasButtonPressed(MouseButton.Left))
        {
            for (int i = 0; i < swatchRects.Count; i++)
            {
                if (swatchRects[i].Contains(mouse.Position))
                {
                    SelectedColor = displayedColors[i].Value;
                    selectedColorName = displayedColors[i].Name;
                    break;
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (displayedColors.Count == 0)
        {
            spriteBatch.DrawString(font, "No colors yet - open a pack!",
                new Vector2(bounds.X + PADDING, bounds.Y + PADDING), Color.Gray);
            return;
        }

        for (int i = 0; i < displayedColors.Count; i++)
        {
            var rect = swatchRects[i];
            var gachaColor = displayedColors[i];
            bool isSelected = gachaColor.Name == selectedColorName;

            spriteBatch.FillRectangle(rect, gachaColor.Value);
            spriteBatch.DrawRectangle(rect, isSelected ? Color.Yellow : Color.Black, isSelected ? 2f : 1f);
        }
    }

    private void RebuildLayout()
    {
        displayedColors = ColorDatabase.AllColors.Where(c => inventory.IsOwned(c.Name)).ToList();

        swatchRects.Clear();
        for (int i = 0; i < displayedColors.Count; i++)
        {
            int col = i % columns;
            int row = i / columns;
            swatchRects.Add(new Rectangle(
                bounds.X + PADDING + col * (SWATCH_SIZE + PADDING),
                bounds.Y + PADDING + row * (SWATCH_SIZE + PADDING),
                SWATCH_SIZE, SWATCH_SIZE));
        }
    }
}