# Dungeon of Algorithms 🎮

Um jogo educacional desenvolvido com MonoGame para ensinar conceitos de algoritmos e estruturas de dados.

## Como Jogar (Rápido)

**Windows:**
1. Clone o repositório
2. Dê duplo-clique em `run.bat`
3. O jogo irá compilar e abrir automaticamente!

**Ou via terminal:**
```bash
git clone https://github.com/lucaslopes-ti/DungeonOfAlgorithms-Educacional.git
cd DungeonOfAlgorithms-Educacional
dotnet run --project DungeonOfAlgorithms.csproj
```

## Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022+ (opcional) ou VS Code

## Controles

| Tecla | Ação |
|-------|------|
| WASD / Setas | Movimentar |
| P | Pausar |
| F5 | Salvar jogo |
| F9 | Carregar jogo |
| R | Reiniciar (Game Over/Vitória) |
| Enter | Selecionar no menu |

## Arquitetura

O projeto utiliza diversos padrões de design:

- **Factory Pattern** - `EnemyFactory.cs`, `ItemFactory.cs`
- **Singleton** - `GameManager`, `AudioManager`, `DungeonManager`
- **Strategy** - `IEnemyBehavior` (PatrolBehavior, ChaseBehavior)

## Estrutura

```
DungeonOfAlgorithms/
├── Content/           # Assets (sprites, música, mapas)
│   ├── Enemies/       # Sprites dos inimigos
│   ├── Player/        # Sprites do jogador
│   ├── Music/         # Trilha sonora
│   └── Maps/          # Arquivos CSV dos mapas
├── Source/
│   ├── Core/          # Sistemas principais
│   └── Entities/      # Entidades do jogo
├── run.bat            # Script para rodar o jogo
└── DungeonOfAlgorithms.csproj
```

## Comandos Úteis

```bash
# Compilar
dotnet build DungeonOfAlgorithms.csproj

# Executar
dotnet run --project DungeonOfAlgorithms.csproj

# Limpar build
dotnet clean DungeonOfAlgorithms.csproj
```

## Licença

Projeto educacional.
