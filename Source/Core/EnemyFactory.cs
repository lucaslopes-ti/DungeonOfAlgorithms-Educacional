// ═══════════════════════════════════════════════════════════════════════════════
// ENEMY FACTORY - A Fábrica de Monstros
// ═══════════════════════════════════════════════════════════════════════════════
// Design Pattern: Factory
// Quer um Slime? Pede pra fábrica. Quer um Ghost? Fábrica.
// É tipo a Amazon de monstros: você pede, a gente entrega.
// Cuidado: não aceitamos devoluções de monstros usados.
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using DungeonOfAlgorithms.Source.Entities;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Fábrica de inimigos. Cria monstros sob demanda.
/// Design Pattern: Factory - centraliza a criação de objetos complexos.
/// </summary>
public static class EnemyFactory
{
    // Referência ao ContentManager pra carregar texturas
    private static ContentManager _content;
    
    // Cache de texturas já carregadas (pra não carregar a mesma coisa 500 vezes)
    private static Dictionary<string, Dictionary<string, Texture2D>> _enemyTextures = new();
    
    // Contador de IDs (cada monstro tem um ID único, tipo CPF)
    private static int _nextId = 0;

    /// <summary>
    /// Inicializa a fábrica com o ContentManager.
    /// Chame isso ANTES de criar qualquer monstro!
    /// </summary>
    /// <param name="content">O gerenciador de conteúdo do MonoGame</param>
    public static void Initialize(ContentManager content)
    {
        _content = content;
    }

    /// <summary>
    /// Cria um novo inimigo do tipo especificado.
    /// "Slime" = gosma verde que patrulha
    /// "Ghost" = fantasma assustador que te persegue
    /// </summary>
    /// <param name="type">Tipo do inimigo ("Slime", "Ghost", etc)</param>
    /// <param name="position">Onde o monstro vai nascer</param>
    /// <returns>Um inimigo novinho em folha, pronto pra te matar</returns>
    public static Enemy CreateEnemy(string type, Vector2 position)
    {
        // Carrega as texturas se ainda não foram carregadas
        if (!_enemyTextures.ContainsKey(type))
        {
            _enemyTextures[type] = LoadEnemyTextures(type);
        }

        // Define o comportamento baseado no tipo
        IEnemyBehavior behavior = type switch
        {
            "Ghost" => new ChaseBehavior(),    // Persegue quando perto, vagueia quando longe
            "Alien" => new WanderBehavior(),   // Movimento organico aleatorio
            _ => new PatrolBehavior()          // Patrulha em 4 direcoes (Slime, Boss)
        };

        // Cria e retorna o monstro com ID único
        return new Enemy(_nextId++, _enemyTextures[type], position, behavior);
    }
    
    /// <summary>
    /// Carrega todas as texturas de animação de um tipo de inimigo.
    /// Cada inimigo tem 6 sprites: andar e idle para cima, baixo e lado.
    /// </summary>
    /// <param name="type">Tipo do inimigo (nome da pasta)</param>
    /// <returns>Dicionário com todas as texturas de animação</returns>
    private static Dictionary<string, Texture2D> LoadEnemyTextures(string type)
    {
        var dict = new Dictionary<string, Texture2D>();
        
        // Mapeamento: chave no código -> nome do arquivo na pasta
        string[] keys = { "Down", "Up", "Side", "Down_Idle", "Up_Idle", "Side_Idle" };
        string[] folderNames = { "D_Walk", "U_Walk", "S_Walk", "D_Idle", "U_Idle", "S_Idle" };
        
        for (int i = 0; i < keys.Length; i++)
        {
             // Monta o caminho: ex: "Enemies/Slime/D_Walk"
             string path = $"Enemies/{type}/{folderNames[i]}";
             try
             {
                dict[keys[i]] = _content.Load<Texture2D>(path);
             }
             catch
             {
                // Se não achar o arquivo, só avisa e continua
                // Melhor um monstro sem sprite do que um crash
                System.Console.WriteLine($"Asset não encontrado: {path}");
             }
        }
        
        return dict;
    }
}
