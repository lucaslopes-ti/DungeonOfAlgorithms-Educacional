// ═══════════════════════════════════════════════════════════════════════════════
// 🏭 ITEM FACTORY - A Loja de Itens Mágicos
// ═══════════════════════════════════════════════════════════════════════════════
// Design Pattern: Factory
// Precisa de uma moeda? Uma poção? Um baú do tesouro?
// A ItemFactory cria tudo isso pra você. É tipo um NPC mercador,
// mas que não cobra 10x o preço de tudo.
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using DungeonOfAlgorithms.Source.Entities;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Fábrica de itens. Cria moedas, baús e outros colecionáveis.
/// Design Pattern: Factory - porque new Item(...) toda hora é feio.
/// </summary>
public static class ItemFactory
{
    // 📦 Referência ao ContentManager
    private static ContentManager _content;
    
    // 🖼️ Cache de texturas (carrega uma vez, usa sempre)
    private static Dictionary<string, Texture2D> _textures = new();

    /// <summary>
    /// Inicializa a fábrica. Chame isso antes de criar itens!
    /// </summary>
    /// <param name="content">O gerenciador de conteúdo do MonoGame</param>
    public static void Initialize(ContentManager content)
    {
        _content = content;
    }

    /// <summary>
    /// Cria um item do tipo especificado.
    /// "Coin" = moedinha dourada (todo mundo ama)
    /// "Chest" = baú do tesouro (JACKPOT!)
    /// </summary>
    /// <param name="type">Tipo do item ("Coin", "Chest")</param>
    /// <param name="position">Onde o item vai aparecer no mundo</param>
    /// <returns>Um item novinho, esperando ser coletado</returns>
    public static Item CreateItem(string type, Vector2 position)
    {
        // Carrega texturas sob demanda (lazy loading)
        if (!_textures.ContainsKey("Coin"))
            _textures["Coin"] = _content.Load<Texture2D>("Items/Coin");
            
        if (!_textures.ContainsKey("Chest"))
            _textures["Chest"] = _content.Load<Texture2D>("Items/Chest");

        // Cria o item baseado no tipo
        switch (type)
        {
            case "Coin":
                // 💰 Moeda de ouro - vale 10 pontos de felicidade
                return new Item(1, "Gold Coin", _textures["Coin"], position);
                
            case "Chest":
                // 📦 Baú do tesouro - o objetivo final!
                return new ChestItem(_textures["Chest"], position);
                
            default:
                // Tipo desconhecido? Retorna null e torce pra não dar NullReferenceException
                return null;
        }
    }
}
