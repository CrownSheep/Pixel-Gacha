using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;

namespace PixelGacha;

public class PackOpeningWindow : IPanel
{
    private const float SHAKE_DURATION = 1f;
    private const float REVEAL_CARD_DURATION = 0.5f;
    private const float POST_REVEAL_HOLD = 1.5f;
    private const int PADDING = 8;
    private const int CARD_SIZE = 40;
    private const int CARD_SPACING = 10;
    private const int ROW_HEIGHT = 44;
    private const int SWATCH_SIZE = 32;
    private const int ROW_SPACING = 6;

    private PlayerInventory inventory;
    private BitmapFont font;
    private Rectangle bounds;
    private Rectangle openButtonRect;
    private Rectangle packIconRect;

    private enum AnimState
    {
        Idle,
        Shaking,
        Revealing,
        Done
    }

    private AnimState state = AnimState.Idle;

    private List<GachaColor> pendingResults = new();
    private int revealIndex;
    private float stateTimer;


    public PackOpeningWindow(PlayerInventory inventory, BitmapFont font)
    {
        this.inventory = inventory;
        this.font = font;
    }

    public void SetBounds(Rectangle newBounds)
    {
        bounds = newBounds;
        openButtonRect = new Rectangle(bounds.X + PADDING, bounds.Y + PADDING, 160, 40);
        packIconRect = new Rectangle(bounds.X + PADDING, openButtonRect.Bottom + 20, 60, 60);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var mouse = MouseExtended.GetState();

        switch (state)
        {
            case AnimState.Idle:
                if (mouse.WasButtonPressed(MouseButton.Left) && openButtonRect.Contains(mouse.Position))
                {
                    if (inventory.SpendCurrency(PlayerInventory.PACK_COST))
                    {
                        pendingResults = GachaService.OpenPack(3);
                        revealIndex = 0;
                        stateTimer = 0f;
                        state = AnimState.Shaking;
                    }
                }

                break;

            case AnimState.Shaking:
                stateTimer += dt;
                if (stateTimer >= SHAKE_DURATION)
                {
                    stateTimer = 0f;
                    state = AnimState.Revealing;
                }

                break;

            case AnimState.Revealing:
                stateTimer += dt;
                if (stateTimer >= REVEAL_CARD_DURATION)
                {
                    stateTimer = 0f;
                    inventory.AddColor(pendingResults[revealIndex]);
                    revealIndex++;
                    if (revealIndex >= pendingResults.Count)
                        state = AnimState.Done;
                }

                break;

            case AnimState.Done:
                stateTimer += dt;
                if (stateTimer >= POST_REVEAL_HOLD || mouse.WasButtonPressed(MouseButton.Left))
                {
                    state = AnimState.Idle;
                    pendingResults.Clear();
                    revealIndex = 0;
                }

                break;
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        bool animating = state != AnimState.Idle;

        spriteBatch.FillRectangle(openButtonRect,
            ButtonFeel.GetColor(openButtonRect, Color.DarkSlateBlue, disabled: animating));
        spriteBatch.DrawRectangle(openButtonRect, Color.Black);
        spriteBatch.DrawString(font, $"Open Pack ({PlayerInventory.PACK_COST})",
            new Vector2(openButtonRect.X + 6, openButtonRect.Y + 10), Color.White);

        if (state == AnimState.Shaking)
            DrawShakingPack(spriteBatch);
        else if (state == AnimState.Revealing || state == AnimState.Done)
            DrawRevealedCards(spriteBatch);
    }

    private void DrawShakingPack(SpriteBatch spriteBatch)
    {
        float progress = stateTimer / SHAKE_DURATION;
        float intensity = 4f + progress * 8f;
        float wobble = (float)Math.Sin(stateTimer * 40f) * intensity;

        var rect = packIconRect;
        rect.X += (int)wobble;

        spriteBatch.FillRectangle(rect, Color.SaddleBrown);
        spriteBatch.DrawRectangle(rect, Color.Black, 2f);

        string questionMark = "?";
        Vector2 textSize = font.MeasureString(questionMark);
        Vector2 textPos = new Vector2(
            rect.X + (rect.Width - textSize.X) / 2f,
            rect.Y + (rect.Height - textSize.Y) / 2f);

        spriteBatch.DrawString(font, questionMark, textPos, Color.White);
    }

    private void DrawRevealedCards(SpriteBatch spriteBatch)
    {
        int x = openButtonRect.X;
        int y = openButtonRect.Bottom + 20;

        for (int i = 0; i < pendingResults.Count; i++)
        {
            if (i > revealIndex) continue;
            if (i == revealIndex && state != AnimState.Done) continue;

            var rowPos = new Vector2(x, y + i * (ROW_HEIGHT + ROW_SPACING));
            DrawCardRow(spriteBatch, rowPos, pendingResults[i], 1f);
        }

        if (state == AnimState.Revealing && revealIndex < pendingResults.Count)
        {
            float t = MathHelper.Clamp(stateTimer / REVEAL_CARD_DURATION, 0f, 1f);
            float scale = EaseOutBack(t);

            var rowPos = new Vector2(x, y + revealIndex * (ROW_HEIGHT + ROW_SPACING));
            DrawCardRow(spriteBatch, rowPos, pendingResults[revealIndex], scale);
        }
    }

    private void DrawCardRow(SpriteBatch spriteBatch, Vector2 rowTopLeft, GachaColor color, float scale)
    {
        int scaledSize = (int)(SWATCH_SIZE * scale);
        int offset = (SWATCH_SIZE - scaledSize) / 2;

        var swatchRect = new Rectangle(
            (int)rowTopLeft.X + offset,
            (int)rowTopLeft.Y + offset,
            scaledSize, scaledSize);

        if (color.Rarity >= ColorRarity.Epic)
        {
            var glowRect = new Rectangle((int)rowTopLeft.X - 3, (int)rowTopLeft.Y - 3, SWATCH_SIZE + 6,
                SWATCH_SIZE + 6);
            spriteBatch.FillRectangle(glowRect, RarityBorderColor(color.Rarity) * 0.4f);
        }

        if (swatchRect.Width > 0 && swatchRect.Height > 0)
        {
            spriteBatch.FillRectangle(swatchRect, color.Value);
            spriteBatch.DrawRectangle(swatchRect, RarityBorderColor(color.Rarity),
                color.Rarity >= ColorRarity.Epic ? 3f : 1.5f);
        }

        if (scale >= 0.9f)
        {
            var textPos = new Vector2(rowTopLeft.X + SWATCH_SIZE + 10, rowTopLeft.Y + 4);
            spriteBatch.DrawString(font, color.Name, textPos, Color.Black);

            var rarityTextPos = new Vector2(rowTopLeft.X + SWATCH_SIZE + 10, rowTopLeft.Y + 22);
            spriteBatch.DrawString(font, color.Rarity.ToString(), rarityTextPos, RarityBorderColor(color.Rarity));
        }
    }

    private Vector2 CardPosition(int startX, int startY, int index, int cardsPerRow)
    {
        int col = index % cardsPerRow;
        int row = index / cardsPerRow;
        return new Vector2(startX + col * (CARD_SIZE + CARD_SPACING), startY + row * (CARD_SIZE + 20));
    }

    private void DrawCard(SpriteBatch spriteBatch, Vector2 topLeft, GachaColor color, float scale)
    {
        int scaledSize = (int)(CARD_SIZE * scale);
        int offset = (CARD_SIZE - scaledSize) / 2;
        var rect = new Rectangle((int)topLeft.X + offset, (int)topLeft.Y + offset, scaledSize, scaledSize);
        if (rect.Width <= 0 || rect.Height <= 0) return;

        if (color.Rarity >= ColorRarity.Epic)
        {
            var glowRect = rect;
            glowRect.Inflate(4, 4);
            spriteBatch.FillRectangle(glowRect, RarityBorderColor(color.Rarity) * 0.5f);
        }

        spriteBatch.FillRectangle(rect, color.Value);
        spriteBatch.DrawRectangle(rect, RarityBorderColor(color.Rarity), color.Rarity >= ColorRarity.Epic ? 3f : 1.5f);

        if (scale >= 0.95f)
            spriteBatch.DrawString(font, color.Name, new Vector2(topLeft.X, topLeft.Y + CARD_SIZE + 2), Color.Black);
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        t -= 1f;
        return 1f + c3 * t * t * t + c1 * t * t;
    }

    private Color RarityBorderColor(ColorRarity rarity) => rarity switch
    {
        ColorRarity.Common => Color.Gray,
        ColorRarity.Rare => Color.RoyalBlue,
        ColorRarity.Epic => Color.MediumPurple,
        ColorRarity.Legendary => Color.Gold,
        _ => Color.Black
    };
}