// ═══════════════════════════════════════════════════════════════════════════════
// TILEMAP - O Chão que Você Pisa (ou Não)
// ═══════════════════════════════════════════════════════════════════════════════
// Estrutura de Dados: ARRAY 2D (int[,])
// Cada número representa um tile diferente. É tipo Minecraft, mas 2D.
// Alguns tiles são sólidos (paredes), outros são passáveis (chão).
// Este sistema permite criar mapas ENORMES com memória mínima!
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Sistema de mapa baseado em tiles. Usa um array 2D pra representar o chão.
/// Cada número é um índice no tileset (spritesheet de tiles).
/// </summary>
public class Tilemap
{
    // 🖼️ A textura do tileset (spritesheet com todos os tiles)
    private readonly Texture2D _tilesetTexture;
    public Texture2D Texture => _tilesetTexture;
    
    // 📊 Array 2D com os índices dos tiles - O MAPA EM SI!
    private readonly int[,] _mapData;
    
    // 📏 Dimensões de cada tile em pixels
    private readonly int _tileWidth;
    private readonly int _tileHeight;
    
    // 🔢 Quantos tiles cabem em uma linha do tileset
    private readonly int _tilesPerRow;
    
    /// <summary>
    /// Tiles sólidos - índices que representam paredes/obstáculos.
    /// Se o tile está nessa lista, você NÃO PASSA!
    /// HashSet pra busca O(1) - performance importa!
    /// </summary>
    private static readonly HashSet<int> SolidTiles = new() 
    { 
        96, 97, 98, 99, 100,  // Paredes principais
        77, 58, 39,           // Cantos
        20, 21, 22, 23, 24,   // Topo das paredes
        43, 62, 81,           // Laterais
        26, 45,               // Decorações de parede
        172, 173, 174         // Objetos sólidos
    };
    
    // Índices das portas - tiles que ativam transição de sala
    public static readonly int DoorEast = 99;
    public static readonly int DoorWest = 98;
    public static readonly int DoorNorth = 97;
    public static readonly int DoorSouth = 96;

    /// <summary>Largura de cada tile em pixels</summary>
    public int TileWidth => _tileWidth;
    
    /// <summary>Altura de cada tile em pixels</summary>
    public int TileHeight => _tileHeight;
    
    /// <summary>Largura total do mapa em pixels</summary>
    public int MapWidth => _mapData.GetLength(1) * _tileWidth;
    
    /// <summary>Altura total do mapa em pixels</summary>
    public int MapHeight => _mapData.GetLength(0) * _tileHeight;

    /// <summary>
    /// Cria um novo tilemap.
    /// </summary>
    /// <param name="tileset">Textura do tileset (spritesheet)</param>
    /// <param name="mapData">Array 2D com os índices dos tiles</param>
    /// <param name="tileWidth">Largura de cada tile</param>
    /// <param name="tileHeight">Altura de cada tile</param>
    public Tilemap(Texture2D tileset, int[,] mapData, int tileWidth, int tileHeight)
    {
        _tilesetTexture = tileset;
        _mapData = mapData;
        _tileWidth = tileWidth;
        _tileHeight = tileHeight;
        _tilesPerRow = tileset.Width / tileWidth; // Calcula quantos tiles por linha
    }

    /// <summary>
    /// Retorna o índice do tile em uma posição do mundo.
    /// Converte coordenadas de pixel pra coordenadas de tile.
    /// </summary>
    /// <param name="worldX">Posição X em pixels</param>
    /// <param name="worldY">Posição Y em pixels</param>
    /// <returns>Índice do tile ou -1 se fora dos limites</returns>
    public int GetTileAt(int worldX, int worldY)
    {
        // Converte pixels pra índice de tile
        int tileX = worldX / _tileWidth;
        int tileY = worldY / _tileHeight;
        
        // Verifica limites (não deixa sair do mapa!)
        if (tileX < 0 || tileY < 0 || tileY >= _mapData.GetLength(0) || tileX >= _mapData.GetLength(1))
            return -1; // Fora do mapa
            
        return _mapData[tileY, tileX];
    }

    /// <summary>
    /// Verifica se uma posição do mundo é sólida (parede/obstáculo).
    /// </summary>
    public bool IsSolid(int worldX, int worldY)
    {
        int tile = GetTileAt(worldX, worldY);
        // Fora do mapa = sólido (parede invisível)
        // Tile no HashSet = sólido
        return tile < 0 || SolidTiles.Contains(tile);
    }

    /// <summary>
    /// Verifica se um retângulo colide com algum tile sólido.
    /// Checa os 4 cantos do retângulo.
    /// </summary>
    /// <param name="bounds">O retângulo de colisão</param>
    /// <returns>true se colidiu com parede</returns>
    public bool IsColliding(Rectangle bounds)
    {
        // Verifica os 4 cantos do bounding box
        return IsSolid(bounds.Left, bounds.Top) ||           // Canto superior esquerdo
               IsSolid(bounds.Right - 1, bounds.Top) ||      // Canto superior direito
               IsSolid(bounds.Left, bounds.Bottom - 1) ||    // Canto inferior esquerdo
               IsSolid(bounds.Right - 1, bounds.Bottom - 1); // Canto inferior direito
    }

    /// <summary>
    /// Desenha o tilemap inteiro na tela.
    /// Percorre o array 2D e desenha cada tile.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        int rows = _mapData.GetLength(0); // Quantas linhas
        int cols = _mapData.GetLength(1); // Quantas colunas

        // Percorre todas as células do array 2D
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int tileIndex = _mapData[y, x];
                if (tileIndex < 0) continue; // -1 = vazio, pula

                // Calcula onde está o tile no tileset (spritesheet)
                // É tipo achar uma casa numa planilha: linha e coluna
                int tileX = (tileIndex % _tilesPerRow) * _tileWidth;
                int tileY = (tileIndex / _tilesPerRow) * _tileHeight;
                Rectangle sourceRect = new Rectangle(tileX, tileY, _tileWidth, _tileHeight);

                // Calcula onde desenhar no mundo
                Vector2 position = new Vector2(x * _tileWidth, y * _tileHeight);

                // Desenha o tile!
                spriteBatch.Draw(_tilesetTexture, position, sourceRect, Color.White);
            }
        }
    }
}

