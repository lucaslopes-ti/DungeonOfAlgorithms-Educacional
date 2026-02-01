// ═══════════════════════════════════════════════════════════════════════════════
// 🏠 ROOM - Uma Sala do Dungeon (Nó do Grafo)
// ═══════════════════════════════════════════════════════════════════════════════
// Estrutura de Dados: GRAFO
// Cada Room é um NÓ do grafo, e as conexões são as ARESTAS.
// Room 1 conecta com Room 2 pela "East"? É uma aresta direcionada!
// Isso permite criar dungeons não-lineares, labirintos, backtracking...
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using DungeonOfAlgorithms.Source.Entities;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Uma sala do dungeon. É um nó no grafo do mapa.
/// Contém o tilemap, itens, inimigos e objetos decorativos.
/// </summary>
public class Room
{
    /// <summary>ID único da sala (como se fosse o endereço)</summary>
    public int Id { get; private set; }
    
    /// <summary>O mapa de tiles desta sala</summary>
    public Tilemap Tilemap { get; private set; }
    
    /// <summary>
    /// Conexões com outras salas. Chave = direção ("North", "South", etc)
    /// Valor = ID da sala conectada. É a lista de adjacência do grafo!
    /// </summary>
    public Dictionary<string, int> Connections { get; private set; } = new();
    
    /// <summary>Lista de itens coletáveis na sala</summary>
    public List<Item> Items { get; private set; } = new();
    
    /// <summary>Lista de inimigos na sala (os bad guys)</summary>
    public List<Enemy> Enemies { get; private set; } = new();
    
    /// <summary>Lista de objetos decorativos (caixas, barris, etc)</summary>
    public List<DecorObject> DecorObjects { get; private set; } = new();

    /// <summary>
    /// Cria uma nova sala com ID e tilemap.
    /// </summary>
    public Room(int id, Tilemap tilemap)
    {
        Id = id;
        Tilemap = tilemap;
    }

    /// <summary>
    /// Conecta esta sala a outra (cria uma aresta no grafo).
    /// </summary>
    /// <param name="direction">Direção da conexão ("North", "South", "East", "West")</param>
    /// <param name="roomId">ID da sala de destino</param>
    public void Connect(string direction, int roomId)
    {
        // Não adiciona conexão duplicada
        if (!Connections.ContainsKey(direction))
            Connections.Add(direction, roomId);
    }
    
    /// <summary>Adiciona um item coletável na sala. 💰</summary>
    public void AddItem(Item item)
    {
        Items.Add(item);
    }

    /// <summary>Adiciona um inimigo na sala. 👹</summary>
    public void AddEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);
    }
    
    /// <summary>Adiciona um objeto decorativo na sala. 📦</summary>
    public void AddDecor(DecorObject decor)
    {
        DecorObjects.Add(decor);
    }
    
    /// <summary>
    /// Verifica se um retângulo colide com algum objeto decorativo.
    /// Usado pra impedir o player de atravessar caixas.
    /// </summary>
    public bool IsCollidingWithDecor(Rectangle bounds)
    {
        foreach (var decor in DecorObjects)
        {
            if (bounds.Intersects(decor.Bounds))
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// Atualiza a lógica da sala: coleta de itens, movimento de inimigos, dano.
    /// </summary>
    /// <param name="gameTime">Tempo de jogo</param>
    /// <param name="player">O jogador (pra checar colisões)</param>
    /// <param name="onItemCollected">Callback quando um item é coletado (opcional)</param>
    public void Update(GameTime gameTime, Player player, System.Action<Item> onItemCollected = null)
    {
        // 💰 Verifica coleta de itens
        foreach (var item in Items)
        {
            if (item.IsActive && player.Bounds.Intersects(item.Bounds))
            {
                item.IsActive = false;            // Desativa o item
                player.AddScore(item.Value);      // Dá os pontos pro player
                onItemCollected?.Invoke(item);    // Dispara o callback (se tiver)
            }
        }
        
        // 👹 Atualiza inimigos e verifica dano
        foreach (var enemy in Enemies)
        {
            enemy.Update(gameTime, player, Tilemap);
            
            // Se o inimigo tocar no player, causa dano!
            if (player.Bounds.Intersects(enemy.Bounds))
            {
                player.TakeDamage(enemy.Damage);
            }
        }
    }

    /// <summary>
    /// Desenha a sala: tilemap, decorações, itens e inimigos.
    /// A ordem importa! Primeiro o chão, depois o que fica em cima.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        // 1️⃣ Primeiro o tilemap (o chão)
        Tilemap.Draw(spriteBatch);
        
        // 2️⃣ Depois os objetos decorativos (atrás de tudo)
        foreach (var decor in DecorObjects)
        {
            decor.Draw(spriteBatch);
        }
        
        // 3️⃣ Depois os itens
        foreach (var item in Items)
        {
            item.Draw(spriteBatch);
        }
        
        // 4️⃣ Por último os inimigos (na frente)
        foreach (var enemy in Enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }
}

