using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;

namespace PixelGacha;

public class CanvasToolbar : IUpdatable, IDrawable
{
    private const int BUTTON_WIDTH = 70;
    private const int BUTTON_HEIGHT = 28;
    private const int SPACING = 8;
    
    private DrawingGrid grid;
    private ArtworkGallery gallery;
    private Rectangle bounds;

    private Rectangle clearButtonRect, saveButtonRect, undoButtonRect;
    
    public CanvasToolbar(DrawingGrid grid, ArtworkGallery gallery)
    {
        this.grid = grid;
        this.gallery = gallery;
    }

    public void SetBounds(Rectangle newBounds)
    {
        bounds = newBounds;
        int x = bounds.X + SPACING;
        int y = bounds.Y + (bounds.Height - BUTTON_HEIGHT) / 2;

        clearButtonRect   = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT); x += BUTTON_WIDTH + SPACING;
        saveButtonRect  = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT); x += BUTTON_WIDTH + SPACING;
        undoButtonRect  = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT); x += BUTTON_WIDTH + SPACING;
    }

    public void Update(GameTime gameTime)
    {
        var mouse = MouseExtended.GetState();
        if (!mouse.WasButtonPressed(MouseButton.Left)) return;

        if (clearButtonRect.Contains(mouse.Position))
        {
            grid.Clear();
        }
        else if (saveButtonRect.Contains(mouse.Position))
        {
            gallery.Save(grid.GridWidth, grid.GridHeight, grid.GetPixelDataSnapshot());
        }
        else if (undoButtonRect.Contains(mouse.Position))
        {
            grid.Undo();
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.FillRectangle(bounds, new Color(230, 230, 235));
        spriteBatch.DrawRectangle(bounds, Color.Black);

        DrawButton(spriteBatch, clearButtonRect, "Clear", Color.IndianRed);
        DrawButton(spriteBatch, saveButtonRect, "Save", Color.MediumSeaGreen);
        DrawButton(spriteBatch, undoButtonRect, "Undo", Color.Goldenrod);
    }

    private void DrawButton(SpriteBatch spriteBatch, Rectangle rect, string label, Color baseColor)
    {
        spriteBatch.FillRectangle(rect, ButtonFeel.GetColor(rect, baseColor));
        spriteBatch.DrawRectangle(rect, Color.Black);

        Vector2 textSize = PixelGachaGame.font.MeasureString(label);
        Vector2 textPos = new Vector2(
            rect.X + (rect.Width - textSize.X) / 2f,
            rect.Y + (rect.Height - textSize.Y) / 2f);

        spriteBatch.DrawString(PixelGachaGame.font, label, textPos, Color.White);
    }
}