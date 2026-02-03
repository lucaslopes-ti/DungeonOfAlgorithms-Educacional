// ═══════════════════════════════════════════════════════════════════════════════
// 👑 GAME MANAGER - O Chefão, O Patrão, O Big Boss
// ═══════════════════════════════════════════════════════════════════════════════
// Design Pattern: Singleton (porque só pode ter UM chefe, né?)
// Este cara é responsável por gerenciar... bem... o jogo todo.
// É tipo o gerente de TI: ninguém sabe exatamente o que faz,
// mas quando some, tudo para de funcionar.
// ═══════════════════════════════════════════════════════════════════════════════

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// O gerente supremo do jogo. Singleton porque dois chefes = caos total.
/// </summary>
public class GameManager
{
    // 🔒 Instância única - tipo eu nas festas, sempre sozinho
    private static GameManager _instance;
    
    /// <summary>
    /// Pega a instância do GameManager. Se não existir, cria uma.
    /// É tipo magia, mas com código.
    /// </summary>
    public static GameManager Instance => _instance ??= new GameManager(); // ??= significa se for nulo, criar novo

    // _instance == null? _instance = new GameManager() : _instance;
    // _instance é nulo? Cria novo GameManager, senão retorna o existente

    // Construtor privado - ninguém cria GameManager na mão!
    private GameManager() { }

    /// <summary>
    /// Inicializa o gerenciador. Por enquanto não faz nada,
    /// mas um dia... um dia ele vai fazer algo grandioso!
    /// </summary>
    public void Initialize()
    {
        // TODO: Colocar lógica de inicialização aqui
        // Por enquanto, só existe para parecer importante
    }
}
