// ═══════════════════════════════════════════════════════════════════════════════
// 🗺️ DUNGEON MANAGER - O Mestre das Masmorras
// ═══════════════════════════════════════════════════════════════════════════════
// Design Pattern: Singleton
// Estrutura de Dados: GRAFO (representado pelo Dictionary de Rooms)
// Este cara gerencia TODAS as salas do dungeon. É tipo o Google Maps,
// mas pra masmorras medievais. Ele sabe onde você está, pra onde pode ir,
// e como chegar lá.
// ═══════════════════════════════════════════════════════════════════════════════

using System.Collections.Generic;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Gerenciador do dungeon. Controla todas as salas e transições.
/// É o GPS do calabouço.
/// </summary>
public class DungeonManager
{
    // 🔒 Singleton - só um gerente de dungeon por jogo
    private static DungeonManager _instance;
    public static DungeonManager Instance => _instance ??= new DungeonManager();

    /// <summary>
    /// Dicionário de todas as salas. Chave = ID, Valor = Room.
    /// É a representação do grafo em forma de lista de adjacência!
    /// </summary>
    public Dictionary<int, Room> Rooms { get; private set; } = new();
    
    /// <summary>A sala onde o player está agora</summary>
    public Room CurrentRoom { get; private set; }

    // Construtor privado (Singleton)
    private DungeonManager() { }

    /// <summary>
    /// Adiciona uma sala ao dungeon.
    /// </summary>
    /// <param name="room">A sala a ser adicionada</param>
    public void AddRoom(Room room)
    {
        // Só adiciona se não existir uma sala com esse ID
        if (!Rooms.ContainsKey(room.Id))
            Rooms.Add(room.Id, room);
    }

    /// <summary>
    /// Verifica se o player saiu dos limites da sala atual.
    /// Se sim, tenta transicionar pra sala vizinha.
    /// </summary>
    public void CheckTransition(int playerX, int playerY, int mapWidthPixel, int mapHeightPixel)
    {
        // Lógica simples: se saiu dos limites, procura o vizinho
        string direction = null;
        
        // Determina a direção baseado em qual borda o player cruzou
        if (playerX > mapWidthPixel) direction = "East";        // Saiu pela direita
        else if (playerX < 0) direction = "West";               // Saiu pela esquerda
        else if (playerY > mapHeightPixel) direction = "South"; // Saiu por baixo
        else if (playerY < 0) direction = "North";              // Saiu por cima

        // Se tem uma sala conectada nessa direção, vai pra ela
        if (direction != null && CurrentRoom.Connections.ContainsKey(direction))
        {
            ChangeRoom(CurrentRoom.Connections[direction]);
            // TODO: Teleportar o player pro lado oposto da nova sala
        }
    }

    /// <summary>
    /// Muda a sala atual para a sala com o ID especificado.
    /// </summary>
    /// <param name="roomId">ID da nova sala</param>
    public void ChangeRoom(int roomId)
    {
        if (Rooms.ContainsKey(roomId))
            CurrentRoom = Rooms[roomId];
    }

    /// <summary>
    /// Carrega um mapa de um arquivo CSV (exportado do Tiled).
    /// Cada número no CSV representa um tile do tileset.
    /// </summary>
    /// <param name="filePath">Caminho do arquivo .csv</param>
    /// <returns>Array 2D com os índices dos tiles</returns>
    public int[,] LoadMapFromCSV(string filePath)
    {
        // Se o arquivo não existir no caminho fornecido, tenta no diretório base
        if (!System.IO.File.Exists(filePath))
        { 
             string altPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filePath);
             if (System.IO.File.Exists(altPath)) filePath = altPath;
        }

        // Lê todas as linhas do CSV
        string[] lines = System.IO.File.ReadAllLines(filePath);
        
        // Descobre as dimensões do mapa
        int height = lines.Length;
        int width = lines[0].Split(',').Length;
        
        // Cria o array 2D
        int[,] mapData = new int[height, width];
        
        // Preenche o array com os dados do CSV
        for (int y = 0; y < height; y++)
        {
            string[] values = lines[y].Split(',');
            for (int x = 0; x < width; x++)
            {
                if (int.TryParse(values[x], out int tileIndex))
                {
                    // Tiled exporta -1 pra tiles vazios, tratamos como 0
                    mapData[y, x] = tileIndex < 0 ? 0 : tileIndex;
                }
            }
        }
        
        return mapData;
    }
}
