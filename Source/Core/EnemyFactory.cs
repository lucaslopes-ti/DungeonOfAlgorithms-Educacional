using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using DungeonOfAlgorithms.Source.Entities;

namespace DungeonOfAlgorithms.Source.Core;

public static class EnemyFactory
{
    private static ContentManager _content;
    private static Dictionary<string, Dictionary<string, Texture2D>> _enemyTextures = new();
    private static int _nextId = 0;

    public static void Initialize(ContentManager content)
    {
        _content = content;
    }

    public static Enemy CreateEnemy(string type, Vector2 position)
    {
        if (!_enemyTextures.ContainsKey(type))
            _enemyTextures[type] = LoadEnemyTextures(type);

        IEnemyBehavior behavior = type switch
        {
            "Ghost" => new ChaseBehavior(),
            "Alien" => new WanderBehavior(),
            _ => new PatrolBehavior()
        };

        return new Enemy(_nextId++, _enemyTextures[type], position, behavior);
    }

    private static Dictionary<string, Texture2D> LoadEnemyTextures(string type)
    {
        var dict = new Dictionary<string, Texture2D>();

        string[] keys = { "Down", "Up", "Side", "Down_Idle", "Up_Idle", "Side_Idle" };
        string[] folderNames = { "D_Walk", "U_Walk", "S_Walk", "D_Idle", "U_Idle", "S_Idle" };

        for (int i = 0; i < keys.Length; i++)
        {
            string path = $"Enemies/{type}/{folderNames[i]}";
            try
            {
                dict[keys[i]] = _content.Load<Texture2D>(path);
            }
            catch
            {
                System.Console.WriteLine($"Asset nao encontrado: {path}");
            }
        }

        return dict;
    }
}
