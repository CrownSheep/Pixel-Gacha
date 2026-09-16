using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using PixelGacha.Layout;
using PixelGacha.Packs;

namespace PixelGacha.Windows;

public class PackOpeningWindow : IPanel
{
    private const float SHAKE_DURATION = 1f;
    private const float REVEAL_CARD_DURATION = 0.5f;
    private const float POST_REVEAL_HOLD = 1.5f;
    private const int PADDING = 14;
    private const int CARD_WIDTH = 90;
    private const int CARD_HEIGHT = 110;
    private const int CARD_SPACING = 14;
    private const int PACK_ICON_WIDTH = 60;
    private const int PACK_ICON_HEIGHT = 90;
    private const int SELECTOR_HEIGHT = 40;
    private const int SELECTOR_SPACING = 6;

    private PlayerInventory inventory;
    private BitmapFont font;
    private Rectangle bounds;
    private Rectangle openButtonRect;
    private Rectangle packIconRect;
    private List<Rectangle> selectorRects = new();
    private int columns = 1;

    private PackType selectedPack = PackCatalog.Basic;

    private enum AnimState { Idle, Shaking, Revealing, Done }
    private AnimState state = AnimState.Idle;

    private List<GachaColor> pendingResults = new();
    private List<bool> pendingIsNew = new();
    private int revealIndex;
    private float stateTimer;
    private float idlePulseTimer;

    public PackOpeningWindow(PlayerInventory inventory, BitmapFont font)
    {
        this.inventory = inventory;
        this.font = font;
    }

    public void SetBounds(Rectangle newBounds)
    {
        bounds = newBounds;
        columns = Math.Max(1, (bounds.Width - PADDING * 2) / (CARD_WIDTH + CARD_SPACING));

        int centerX = bounds.X + bounds.Width / 2;

        selectorRects.Clear();
        int selectorY = bounds.Y + PADDING + 40;
        int selectorWidth = (bounds.Width - PADDING * 2 - SELECTOR_SPACING * (PackCatalog.All.Count - 1)) / PackCatalog.All.Count;
        int x = bounds.X + PADDING;
        foreach (var _ in PackCatalog.All)
        {
            selectorRects.Add(new Rectangle(x, selectorY, selectorWidth, SELECTOR_HEIGHT));
            x += selectorWidth + SELECTOR_SPACING;
        }

        packIconRect = new Rectangle(centerX - PACK_ICON_WIDTH / 2, selectorY + SELECTOR_HEIGHT + 20, PACK_ICON_WIDTH, PACK_ICON_HEIGHT);
        openButtonRect = new Rectangle(centerX - 90, packIconRect.Bottom + 16, 180, 44);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var mouse = MouseExtended.GetState();
        idlePulseTimer += dt;

        switch (state)
        {
            case AnimState.Idle:
                if (mouse.WasButtonPressed(MouseButton.Left))
                {
                    for (int i = 0; i < selectorRects.Count; i++)
                    {
                        if (selectorRects[i].Contains(mouse.Position))
                        {
                            selectedPack = PackCatalog.All[i];
                            break;
                        }
                    }
                }

                if (mouse.WasButtonPressed(MouseButton.Left) && openButtonRect.Contains(mouse.Position))
                {
                    if (inventory.SpendCurrency(selectedPack.Cost))
                    {
                        pendingResults = GachaService.OpenPack(selectedPack);
                        pendingIsNew = pendingResults.Select(c => !inventory.IsOwned(c.Name)).ToList();
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
                    pendingIsNew.Clear();
                    revealIndex = 0;
                }
                break;
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        bool animating = state != AnimState.Idle;

        DrawPackSelector(spriteBatch, animating);

        if (state == AnimState.Done && pendingResults.Count > 0)
        {
            var bestRarity = pendingResults.Max(c => c.Rarity);
            if (bestRarity >= ColorRarity.Epic)
                DrawCelebrationFlash(spriteBatch, bestRarity);
        }

        DrawPackIcon(spriteBatch);

        bool canAfford = inventory.Currency >= selectedPack.Cost;
        var buttonColor = !canAfford && !animating ? Color.Gray : selectedPack.ThemeColor;
        spriteBatch.FillRectangle(openButtonRect, ButtonUtil.GetColor(openButtonRect, buttonColor, disabled: animating || !canAfford));
        spriteBatch.DrawRectangle(openButtonRect, Color.Black);

        string buttonLabel = animating ? "..." : $"Open {selectedPack.Name} ({selectedPack.Cost}$)";
        Vector2 btnTextSize = font.MeasureString(buttonLabel);

        float scale = Math.Min(1f, (openButtonRect.Width - 12) / Math.Max(1f, btnTextSize.X));
        spriteBatch.DrawString(font, buttonLabel,
            new Vector2(openButtonRect.X + (openButtonRect.Width - btnTextSize.X * scale) / 2f,
                        openButtonRect.Y + (openButtonRect.Height - btnTextSize.Y * scale) / 2f),
            Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        if (state == AnimState.Revealing || state == AnimState.Done)
            DrawRevealedCards(spriteBatch);
    }

    private void DrawPackSelector(SpriteBatch spriteBatch, bool disabled)
    {
        for (int i = 0; i < PackCatalog.All.Count; i++)
        {
            var pack = PackCatalog.All[i];
            var rect = selectorRects[i];
            bool selected = pack == selectedPack;

            var baseColor = selected ? pack.ThemeColor : new Color(220, 218, 225);
            spriteBatch.FillRectangle(rect, disabled ? baseColor * 0.6f : ButtonUtil.GetColor(rect, baseColor));
            spriteBatch.DrawRectangle(rect, selected ? Color.Black : Color.Gray, selected ? 2.5f : 1f);

            string label = pack.Name.Replace(" Pack", "");
            Vector2 labelSize = font.MeasureString(label);
            var labelColor = selected ? Color.White : new Color(80, 80, 85);
            spriteBatch.DrawString(font, label,
                new Vector2(rect.X + (rect.Width - labelSize.X) / 2f, rect.Y + 4), labelColor);

            string costText = $"{pack.Cost}$";
            Vector2 costSize = font.MeasureString(costText);
            spriteBatch.DrawString(font, costText,
                new Vector2(rect.X + (rect.Width - costSize.X) / 2f, rect.Y + SELECTOR_HEIGHT - 20), labelColor);
        }
    }

    private void DrawPackIcon(SpriteBatch spriteBatch)
    {
        var rect = packIconRect;

        if (state == AnimState.Shaking)
        {
            float progress = stateTimer / SHAKE_DURATION;
            float intensity = 4f + progress * 10f;
            float wobble = (float)Math.Sin(stateTimer * 40f) * intensity;
            rect.X += (int)wobble;

            var glow = rect;
            glow.Inflate((int)(6 + progress * 5), (int)(6 + progress * 5));
            spriteBatch.FillRectangle(glow, Color.Gold * (0.15f + progress * 0.25f));
        }
        else if (state == AnimState.Idle)
        {
            float pulse = (float)(Math.Sin(idlePulseTimer * 2f) * 0.5f + 0.5f);
            var glow = rect;
            glow.Inflate(4, 4);
            spriteBatch.FillRectangle(glow, selectedPack.ThemeColor * (0.1f + pulse * 0.1f));
        }
        
        spriteBatch.FillRectangle(rect, selectedPack.ThemeColor);
        var ribbon = new Rectangle(rect.X, rect.Y + rect.Height / 2 - 8, rect.Width, 16);
        spriteBatch.FillRectangle(ribbon, new Color(255, 210, 60));
        spriteBatch.DrawRectangle(rect, Color.Black, 2f);

        var shine = new Rectangle(rect.X + 8, rect.Y + 8, 14, 14);
        spriteBatch.FillRectangle(shine, Color.White * 0.5f);

        string mark = state == AnimState.Shaking ? "!" : "?";
        Vector2 textSize = font.MeasureString(mark);
        Vector2 textPos = new Vector2(rect.X + (rect.Width - textSize.X) / 2f, rect.Y + (rect.Height - textSize.Y) / 2f);
        spriteBatch.DrawString(font, mark, textPos, Color.White);
    }

    private void DrawCelebrationFlash(SpriteBatch spriteBatch, ColorRarity rarity)
    {
        float pulse = (float)(Math.Sin(stateTimer * 6f) * 0.5f + 0.5f);
        spriteBatch.FillRectangle(bounds, RarityBorderColor(rarity) * (0.06f + pulse * 0.08f));
    }

    private void DrawRevealedCards(SpriteBatch spriteBatch)
    {
        int startX = bounds.X + PADDING;
        int startY = openButtonRect.Bottom + 24;

        for (int i = 0; i < pendingResults.Count; i++)
        {
            if (i > revealIndex) continue;
            if (i == revealIndex && state != AnimState.Done) continue;

            var pos = CardPosition(startX, startY, i);
            DrawCard(spriteBatch, pos, pendingResults[i], pendingIsNew[i], 1f);
        }

        if (state == AnimState.Revealing && revealIndex < pendingResults.Count)
        {
            float t = MathHelper.Clamp(stateTimer / REVEAL_CARD_DURATION, 0f, 1f);
            float scale = EaseOutBack(t);
            var pos = CardPosition(startX, startY, revealIndex);
            DrawCard(spriteBatch, pos, pendingResults[revealIndex], pendingIsNew[revealIndex], scale);
        }
    }

    private Vector2 CardPosition(int startX, int startY, int index)
    {
        int col = index % columns;
        int row = index / columns;
        return new Vector2(startX + col * (CARD_WIDTH + CARD_SPACING), startY + row * (CARD_HEIGHT + CARD_SPACING));
    }

    private void DrawCard(SpriteBatch spriteBatch, Vector2 topLeft, GachaColor color, bool isNew, float scale)
    {
        int scaledW = (int)(CARD_WIDTH * scale);
        int scaledH = (int)(CARD_HEIGHT * scale);
        int offsetX = (CARD_WIDTH - scaledW) / 2;
        int offsetY = (CARD_HEIGHT - scaledH) / 2;

        var rect = new Rectangle((int)topLeft.X + offsetX, (int)topLeft.Y + offsetY, scaledW, scaledH);
        if (rect.Width <= 0 || rect.Height <= 0) return;

        var borderColor = RarityBorderColor(color.Rarity);

        if (color.Rarity >= ColorRarity.Epic)
        {
            var glowRect = rect;
            glowRect.Inflate(5, 5);
            spriteBatch.FillRectangle(glowRect, borderColor * 0.4f);
        }

        spriteBatch.FillRectangle(rect, Color.White);
        var swatchArea = new Rectangle(rect.X + 4, rect.Y + 4, rect.Width - 8, (int)(rect.Height * 0.6f));
        spriteBatch.FillRectangle(swatchArea, color.Value);
        spriteBatch.DrawRectangle(rect, borderColor, color.Rarity >= ColorRarity.Epic ? 3f : 1.5f);

        if (scale >= 0.9f)
        {
            var nameText = TruncateToFit(color.Name, rect.Width - 8);
            spriteBatch.DrawString(font, nameText, new Vector2(rect.X + 4, swatchArea.Bottom + 4), Color.Black);
            spriteBatch.DrawString(font, color.Rarity.ToString(),
                new Vector2(rect.X + 4, swatchArea.Bottom + 20), borderColor);

            if (isNew)
            {
                var badgeRect = new Rectangle(rect.Right - 34, rect.Y - 8, 34, 18);
                spriteBatch.FillRectangle(badgeRect, Color.OrangeRed);
                spriteBatch.DrawRectangle(badgeRect, Color.Black);
                spriteBatch.DrawString(font, "NEW!", new Vector2(badgeRect.X + 2, badgeRect.Y + 2), Color.White);
            }
        }
    }

    private string TruncateToFit(string text, int maxWidth)
    {
        if (maxWidth <= 0) return text;
        if (font.MeasureString(text).Width <= maxWidth) return text;

        string truncated = text;
        while (truncated.Length > 1 && font.MeasureString(truncated + "...").Width > maxWidth)
            truncated = truncated[..^1];

        return truncated + "...";
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