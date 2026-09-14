using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;

namespace PixelGacha;

public class CollectionWindow : IPanel
{
    private PlayerInventory inventory;
    private BitmapFont font;
    private Rectangle bounds;

    private const int CARD_WIDTH = 100;
    private const int CARD_HEIGHT = 55;
    private const int CARD_PADDING = 10;
    private const int SECTION_HEADER_HEIGHT = 26;
    private const int HEADER_HEIGHT = 34;
    private const int OUTER_PADDING = 10;
    private const int SCROLLBAR_WIDTH = 6;
    private const float SCROLL_SPEED = 0.35f;

    private static readonly ColorRarity[] RarityOrder = [ColorRarity.Base, ColorRarity.Common, ColorRarity.Rare, ColorRarity.Epic, ColorRarity.Legendary];

    private static readonly RasterizerState ScissorRasterizer = new RasterizerState { ScissorTestEnable = true };

    private int columns = 1;
    private int scrollOffset = 0;
    private int contentHeight = 0;
    private int previousScrollWheelValue;

    public CollectionWindow(PlayerInventory inventory, BitmapFont font)
    {
        this.inventory = inventory;
        this.font = font;
        previousScrollWheelValue = Mouse.GetState().ScrollWheelValue;
    }

    public void SetBounds(Rectangle newBounds)
    {
        bounds = newBounds;
        columns = Math.Max(1, (bounds.Width - OUTER_PADDING * 2 - SCROLLBAR_WIDTH) / (CARD_WIDTH + CARD_PADDING));
        contentHeight = ComputeContentHeight();
        ClampScroll();
    }

    public void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        int scrollDelta = mouseState.ScrollWheelValue - previousScrollWheelValue;
        previousScrollWheelValue = mouseState.ScrollWheelValue;

        if (scrollDelta != 0 && bounds.Contains(new Point(mouseState.X, mouseState.Y)))
        {
            scrollOffset -= (int)(scrollDelta * SCROLL_SPEED);
            ClampScroll();
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        int ownedCount = ColorDatabase.AllColors.Count(c => inventory.IsOwned(c.Name));
        int totalCount = ColorDatabase.AllColors.Count;
        
        spriteBatch.DrawString(font, $"Collection  {ownedCount}/{totalCount}",
            new Vector2(bounds.X + OUTER_PADDING, bounds.Y + OUTER_PADDING), Color.Black);

        var progressBarRect = new Rectangle(bounds.X + OUTER_PADDING, bounds.Y + OUTER_PADDING + 20,
            bounds.Width - OUTER_PADDING * 2, 8);
        spriteBatch.FillRectangle(progressBarRect, new Color(220, 220, 225));
        float pct = totalCount == 0 ? 0 : (float)ownedCount / totalCount;
        var filledRect = progressBarRect;
        filledRect.Width = (int)(progressBarRect.Width * pct);
        spriteBatch.FillRectangle(filledRect, Color.MediumSeaGreen);
        spriteBatch.DrawRectangle(progressBarRect, Color.Black);

        var viewport = new Rectangle(bounds.X, bounds.Y + HEADER_HEIGHT, bounds.Width, bounds.Height - HEADER_HEIGHT);
        DrawScrollableContent(spriteBatch, viewport);
        DrawScrollbar(spriteBatch, viewport);
    }

    private void DrawScrollableContent(SpriteBatch spriteBatch, Rectangle viewport)
    {
        var device = spriteBatch.GraphicsDevice;
        var previousScissor = device.ScissorRectangle;

        spriteBatch.End();
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: ScissorRasterizer);
        device.ScissorRectangle = Rectangle.Intersect(viewport, device.Viewport.Bounds);

        int y = viewport.Y - scrollOffset;
        foreach (var rarity in RarityOrder)
        {
            y = DrawRaritySection(spriteBatch, rarity, y);
            y += CARD_PADDING;
        }

        spriteBatch.End();
        device.ScissorRectangle = previousScissor;
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    }

    private void DrawScrollbar(SpriteBatch spriteBatch, Rectangle viewport)
    {
        int maxScroll = MaxScroll(viewport.Height);
        if (maxScroll <= 0) return;

        var trackRect = new Rectangle(viewport.Right - SCROLLBAR_WIDTH, viewport.Y, SCROLLBAR_WIDTH, viewport.Height);
        spriteBatch.FillRectangle(trackRect, new Color(225, 225, 230));

        float thumbHeightPct = MathHelper.Clamp((float)viewport.Height / contentHeight, 0.08f, 1f);
        int thumbHeight = Math.Max(20, (int)(viewport.Height * thumbHeightPct));

        float scrollPct = maxScroll == 0 ? 0 : (float)scrollOffset / maxScroll;
        int thumbY = viewport.Y + (int)((viewport.Height - thumbHeight) * scrollPct);

        var thumbRect = new Rectangle(trackRect.X, thumbY, SCROLLBAR_WIDTH, thumbHeight);
        spriteBatch.FillRectangle(thumbRect, new Color(150, 150, 158));
    }

    private int DrawRaritySection(SpriteBatch spriteBatch, ColorRarity rarity, int startY)
    {
        var colorsInRarity = ColorDatabase.ByRarity(rarity).ToList();
        int ownedInRarity = colorsInRarity.Count(c => inventory.IsOwned(c.Name));

        spriteBatch.DrawString(font, $"{rarity}  ({ownedInRarity}/{colorsInRarity.Count})",
            new Vector2(bounds.X + OUTER_PADDING, startY), RarityTextColor(rarity));

        int cardsY = startY + SECTION_HEADER_HEIGHT;

        for (int i = 0; i < colorsInRarity.Count; i++)
        {
            int col = i % columns;
            int row = i / columns;

            var cardRect = new Rectangle(
                bounds.X + OUTER_PADDING + col * (CARD_WIDTH + CARD_PADDING),
                cardsY + row * (CARD_HEIGHT + CARD_PADDING),
                CARD_WIDTH, CARD_HEIGHT);
            
            if (cardRect.Bottom < bounds.Y || cardRect.Y > bounds.Bottom) continue;

            DrawCard(spriteBatch, cardRect, colorsInRarity[i], rarity);
        }

        int rows = (int)Math.Ceiling(colorsInRarity.Count / (float)columns);
        return cardsY + rows * (CARD_HEIGHT + CARD_PADDING);
    }

    private void DrawCard(SpriteBatch spriteBatch, Rectangle rect, GachaColor gachaColor, ColorRarity rarity)
    {
        bool owned = inventory.IsOwned(gachaColor.Name);
        var borderColor = RarityBorderColor(rarity);

        spriteBatch.FillRectangle(rect, owned ? Color.White : new Color(235, 235, 238));
        spriteBatch.DrawRectangle(rect, owned ? borderColor : Color.Gray, owned && rarity >= ColorRarity.Epic ? 3f : 1.5f);

        if (owned && rarity >= ColorRarity.Epic)
        {
            var glowRect = rect;
            glowRect.Inflate(3, 3);
            spriteBatch.FillRectangle(glowRect, borderColor * 0.25f);
            spriteBatch.FillRectangle(rect, Color.White);
            spriteBatch.DrawRectangle(rect, borderColor, 3f);
        }

        var swatchRect = new Rectangle(rect.X + 8, rect.Y + 8, 24, 24);

        if (owned)
        {
            spriteBatch.FillRectangle(swatchRect, gachaColor.Value);
            spriteBatch.DrawRectangle(swatchRect, Color.Black);

            spriteBatch.DrawString(font, gachaColor.Name, new Vector2(swatchRect.Right + 8, rect.Y + 8), Color.Black);

            int count = inventory.Owned[gachaColor.Name];
            if (count > 1)
                spriteBatch.DrawString(font, $"x{count}", new Vector2(swatchRect.Right + 8, rect.Y + 26), Color.Gray);
        }
        else
        {
            spriteBatch.FillRectangle(swatchRect, new Color(180, 180, 185));
            spriteBatch.DrawRectangle(swatchRect, Color.Gray);
            spriteBatch.DrawString(font, "?", new Vector2(swatchRect.X + 9, swatchRect.Y + 4), Color.White);
            spriteBatch.DrawString(font, "???", new Vector2(swatchRect.Right + 8, rect.Y + 8), Color.Gray);
        }
    }

    private int ComputeContentHeight()
    {
        int y = 0;
        foreach (var rarity in RarityOrder)
        {
            int count = ColorDatabase.ByRarity(rarity).Count();
            int rows = (int)Math.Ceiling(count / (float)columns);
            y += SECTION_HEADER_HEIGHT + rows * (CARD_HEIGHT + CARD_PADDING) + CARD_PADDING;
        }
        return y;
    }

    private int MaxScroll(int viewportHeight) => Math.Max(0, contentHeight - viewportHeight);

    private void ClampScroll()
    {
        int viewportHeight = bounds.Height - HEADER_HEIGHT;
        scrollOffset = MathHelper.Clamp(scrollOffset, 0, MaxScroll(viewportHeight));
    }

    private Color RarityBorderColor(ColorRarity rarity) => rarity switch
    {
        ColorRarity.Base => new Color(160, 160, 165),
        ColorRarity.Common => Color.Gray,
        ColorRarity.Rare => Color.RoyalBlue,
        ColorRarity.Epic => Color.MediumPurple,
        ColorRarity.Legendary => Color.Gold,
        _ => Color.Black
    };

    private Color RarityTextColor(ColorRarity rarity) => rarity switch
    {
        ColorRarity.Base => new Color(120, 120, 125),
        ColorRarity.Common => new Color(90, 90, 90),
        ColorRarity.Rare => new Color(40, 70, 160),
        ColorRarity.Epic => new Color(110, 40, 160),
        ColorRarity.Legendary => new Color(180, 130, 0),
        _ => Color.Black
    };
}