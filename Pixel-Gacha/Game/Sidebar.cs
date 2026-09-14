using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;

namespace PixelGacha;

public class Sidebar : IUpdatable, IDrawable
{
    private const int TAB_BAR_HEIGHT = 36;
    private const int HEADER_PADDING_TOP = 6;
    private const int HEADER_PADDING_BOTTOM = 8;

    private BitmapFont font;
    private PlayerInventory inventory;
    private Dictionary<SidebarTab, IPanel> panels;
    private SidebarTab activeTab = SidebarTab.Palette;
    private Rectangle bounds;
    private List<(SidebarTab tab, Rectangle rect)> tabButtons = new();

    private readonly int headerHeight;

    public PaletteWindow Palette => (PaletteWindow)panels[SidebarTab.Palette];

    public Sidebar(GraphicsDevice gd, BitmapFont font, PlayerInventory inventory, DrawingGrid grid, ArtworkGallery gallery)
    {
        this.font = font;
        this.inventory = inventory;
        
        headerHeight = HEADER_PADDING_TOP + (int)font.MeasureString("Currency: 0").Height + HEADER_PADDING_BOTTOM;

        panels = new Dictionary<SidebarTab, IPanel>
        {
            { SidebarTab.Palette,    new PaletteWindow(gd, font, inventory) },
            { SidebarTab.Packs,      new PackOpeningWindow(inventory, font) },
            { SidebarTab.Collection, new CollectionWindow(inventory, font) },
            { SidebarTab.Gallery,    new GalleryWindow(gd, gallery, grid, font) },
        };
    }

    public void SetBounds(Rectangle newBounds)
    {
        bounds = newBounds;
        LayoutTabButtons();

        var contentRect = new Rectangle(
            bounds.X,
            bounds.Y + TAB_BAR_HEIGHT + headerHeight,
            bounds.Width,
            bounds.Height - TAB_BAR_HEIGHT - headerHeight);

        foreach (var panel in panels.Values)
            panel.SetBounds(contentRect);
    }

    private void LayoutTabButtons()
    {
        tabButtons.Clear();
        var tabs = Enum.GetValues<SidebarTab>();
        int tabWidth = bounds.Width / tabs.Length;

        for (int i = 0; i < tabs.Length; i++)
            tabButtons.Add((tabs[i], new Rectangle(bounds.X + i * tabWidth, bounds.Y, tabWidth, TAB_BAR_HEIGHT)));
    }

    public void Update(GameTime gameTime)
    {
        var mouse = MouseExtended.GetState();
        if (mouse.WasButtonPressed(MouseButton.Left))
        {
            foreach (var (tab, rect) in tabButtons)
            {
                if (rect.Contains(mouse.Position))
                {
                    activeTab = tab;
                    break;
                }
            }
        }

        panels[activeTab].Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        string currencyString = $"Currency: {inventory.Currency}";

        spriteBatch.FillRectangle(bounds, new Color(240, 240, 245));

        int separatorY = bounds.Y + TAB_BAR_HEIGHT + headerHeight;
        spriteBatch.DrawLine(bounds.X, separatorY, bounds.X + bounds.Width, separatorY, Color.Gray, 1);
        spriteBatch.DrawLine(new Vector2(bounds.X + bounds.Width, bounds.Y),
            new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height), Color.Black, 4);

        foreach (var (tab, rect) in tabButtons)
        {
            bool active = tab == activeTab;
            spriteBatch.FillRectangle(rect, ButtonFeel.GetColor(rect, active ? Color.White : new Color(210, 210, 220)));
            spriteBatch.DrawRectangle(rect, Color.Black);
            spriteBatch.DrawString(font, tab.ToString(), new Vector2(rect.X + 8, rect.Y + 9), Color.Black);
        }

        spriteBatch.DrawString(font, currencyString,
            new Vector2(bounds.X + 8, bounds.Y + TAB_BAR_HEIGHT + HEADER_PADDING_TOP), Color.Black);

        panels[activeTab].Draw(spriteBatch, gameTime);
    }
}