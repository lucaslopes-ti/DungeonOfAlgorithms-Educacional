using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using DungeonOfAlgorithms.Source.Entities;

namespace DungeonOfAlgorithms.Source.Core;

public static class ItemFactory
{
    private static ContentManager _content;
    private static Dictionary<string, Texture2D> _textures = new();

    public static void Initialize(ContentManager content)
    {
        _content = content;
    }

    public static Item CreateItem(string type, Vector2 position)
    {
        if (!_textures.ContainsKey("Coin"))
            _textures["Coin"] = _content.Load<Texture2D>("Items/Coin");

        if (!_textures.ContainsKey("Chest"))
            _textures["Chest"] = _content.Load<Texture2D>("Items/Chest");

        return type switch
        {
            "Coin" => new Item(1, "Gold Coin", _textures["Coin"], position),
            "Chest" => new ChestItem(_textures["Chest"], position),
            _ => null
        };
    }
}
