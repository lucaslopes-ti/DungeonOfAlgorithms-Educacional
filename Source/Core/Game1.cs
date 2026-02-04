// ═══════════════════════════════════════════════════════════════════════════════
// GAME1 - O Cérebro Central do Dungeon of Algorithms
// ═══════════════════════════════════════════════════════════════════════════════
// Esta é a classe principal do jogo. Herda de Game (MonoGame).
// Aqui acontece TUDO: Initialize → LoadContent → Update → Draw (loop infinito)
// É tipo a main() do jogo, mas com esteroides.
// 
// Game Loop:
//   Initialize() → LoadContent() → [ Update() → Draw() ] ← repete 60x/segundo
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using DungeonOfAlgorithms.Source.Entities;
using Microsoft.Xna.Framework.Media;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Classe principal do jogo. Onde a mágica acontece.
/// MonoGame chama Update() e Draw() 60 vezes por segundo.
/// </summary>
public class Game1 : Game
{
    // ═══════════════════════════════════════════════════════════════════════════
    // 🖥️ GRÁFICOS E RENDERIZAÇÃO
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>Gerencia a placa de vídeo, resolução, fullscreen, etc.</summary>
    private GraphicsDeviceManager _graphics;

    /// <summary>Batcha sprites pra desenhar tudo de uma vez (otimização)</summary>
    private SpriteBatch _spriteBatch;

    // ═══════════════════════════════════════════════════════════════════════════
    // 🎭 ENTIDADES E SISTEMAS
    // ═══════════════════════════════════════════════════════════════════════════

    private Player _player;                    // O herói da história
    private Camera _camera;                    // A câmera que segue o player
    private HUD _hud;                          // Interface (HP, Score)
    private GameState _gameState = GameState.MainMenu;  // Estado atual do jogo
    private SpriteFont _font;                  // Fonte pra textos
    private KeyboardState _lastKeyboardState;  // Estado anterior do teclado
    private System.Collections.Generic.Dictionary<string, Texture2D> _playerTextures;  // Sprites do player
    private Texture2D _vignetteTexture;        // Efeito de vinheta (escurece bordas)
    private Song _ambientMusic;               // Música de fundo

    // ═══════════════════════════════════════════════════════════════════════════
    // 🌓 SISTEMA DE TRANSIÇÃO DE SALA (FADE)
    // ═══════════════════════════════════════════════════════════════════════════
    // Quando troca de sala, faz um fade: escurece → muda sala → clareia

    private Texture2D _fadeTexture;            // Textura preta 1x1 esticada
    private float _fadeAlpha = 0f;             // Opacidade (0=invisível, 1=preto total)
    private bool _isFading = false;            // Está em transição?
    private bool _fadeOut = true;              // true=escurecendo, false=clareando
    private const float FADE_SPEED = 3f;       // Velocidade da transição
    private int _pendingRoom = -1;             // Sala pra onde vai (enquanto escurece)
    private Vector2 _pendingSpawnPosition;     // Onde spawnar na nova sala

    // ═══════════════════════════════════════════════════════════════════════════
    // 📋 SISTEMA DE MENU
    // ═══════════════════════════════════════════════════════════════════════════

    private int _menuSelectedIndex = 0;                    // Opção selecionada (0=Jogar, 1=Sair)
    private string[] _menuOptions = { "JOGAR", "SAIR" };   // Texto das opções
    private float _menuPulseTimer = 0f;                    // Timer pra efeito de pulsar
    private Texture2D _pixelTexture;                       // Pixel branco (pra desenhar retângulos)

    // ═══════════════════════════════════════════════════════════════════════════
    // 🏗️ CONSTRUTOR
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Construtor do jogo. Define configurações básicas.
    /// Aqui a janela ainda não existe - só configuramos.
    /// </summary>
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);  // Inicia o gerenciador gráfico
        Content.RootDirectory = "Content";             // Pasta onde ficam os assets
        IsMouseVisible = true;                         // Mostra cursor (útil pra debug)
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // INITIALIZE - Primeira coisa a rodar
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Inicializa sistemas antes de carregar conteúdo.
    /// Roda UMA VEZ quando o jogo inicia.
    /// </summary>
    protected override void Initialize()
    {
        GameManager.Instance.Initialize();  // Inicializa o Singleton do GameManager
        _camera = new Camera();             // Cria a câmera
        base.Initialize();                  // Chama Initialize() da classe base (Game)
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 📦 LOADCONTENT - Carrega TODOS os assets
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Carrega texturas, fontes, sons, etc.
    /// Roda UMA VEZ depois do Initialize().
    /// Aqui criamos as salas, itens, inimigos e tudo mais.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);  // Cria o SpriteBatch pra desenhar

        // ═════════════════════════════════════════════════════════════════════
        // 🗺️ CRIAÇÃO DAS SALAS (Graph Structure)
        // ═════════════════════════════════════════════════════════════════════

        // Carrega tileset (imagem com todos os tiles)
        var tilesetTexture = Content.Load<Texture2D>("Tiles/Tileset");

        // --- Sala 1: The Stack (sala inicial) ---
        int[,] map1 = DungeonManager.Instance.LoadMapFromCSV("Content/Maps/Room_01.csv");
        var room1 = new Room(1, new Tilemap(tilesetTexture, map1, 16, 16));

        // --- Sala 2: The Heap (sala do meio) ---
        int[,] map2 = DungeonManager.Instance.LoadMapFromCSV("Content/Maps/Room_02.csv");
        var room2 = new Room(2, new Tilemap(tilesetTexture, map2, 16, 16));

        // --- Sala 3: Kernel Panic (sala final com baú) ---
        int[,] map3 = DungeonManager.Instance.LoadMapFromCSV("Content/Maps/Room_03.csv");
        var room3 = new Room(3, new Tilemap(tilesetTexture, map3, 16, 16));

        // ═════════════════════════════════════════════════════════════════════
        // 🔗 CONEXÕES (Arestas do Grafo)
        // ═════════════════════════════════════════════════════════════════════
        // Room1 ←→ Room2 ←→ Room3

        room1.Connect("East", 2);    // Sala 1 → Leste → Sala 2
        room2.Connect("West", 1);    // Sala 2 → Oeste → Sala 1
        room2.Connect("East", 3);    // Sala 2 → Leste → Sala 3
        room3.Connect("West", 2);    // Sala 3 → Oeste → Sala 2

        // ═════════════════════════════════════════════════════════════════════
        // 💰 ITENS - Moedas espalhadas pelo dungeon
        // ═════════════════════════════════════════════════════════════════════

        ItemFactory.Initialize(Content);  // Inicializa a Factory

        // Sala 1 - 5 moedas de boas vindas
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(80, 80)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(40, 80)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(150, 150)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(300, 100)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(200, 200)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(400, 150)));

        // Sala 2 - 5 moedas mais
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(100, 100)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(200, 150)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(350, 200)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(150, 250)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(400, 100)));

        // Sala 3 - 5 moedas na sala do tesouro
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(100, 150)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(150, 200)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(200, 250)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(250, 150)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(300, 200)));

        // ═════════════════════════════════════════════════════════════════════
        // 👹 INIMIGOS - Os Memory Leaks personificados
        // ═════════════════════════════════════════════════════════════════════

        EnemyFactory.Initialize(Content);  // Inicializa a Factory

        // Sala 1: Slimes (patrulham - movimento previsível)
        room1.AddEnemy(EnemyFactory.CreateEnemy("Slime", new Vector2(350, 270)));
        room1.AddEnemy(EnemyFactory.CreateEnemy("Slime", new Vector2(400, 200)));
        room1.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(80, 160)));
        room1.AddEnemy(EnemyFactory.CreateEnemy("Alien", new Vector2(80, 200)));


        // Sala 2: Ghosts (mais rápidos, te perseguem!)
        room2.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(100, 200)));
        room2.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(200, 200)));
        room2.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(350, 150)));

        // Sala 3: Ghosts guardando o baú - boa sorte!
        room3.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(150, 200)));
        room3.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(200, 200)));
        room3.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(250, 200)));

        // O baú da vitória - objetivo final
        room3.AddItem(ItemFactory.CreateItem("Chest", new Vector2(200, 250)));

        // ═════════════════════════════════════════════════════════════════════
        // 📦 OBJETOS DECORATIVOS - Caixas, Barris, Potes, Sacos
        // ═════════════════════════════════════════════════════════════════════

        // Carrega todas as 16 variações de caixas
        // (porque variedade é o tempero da dungeon)
        var boxTextures = new Texture2D[16];
        for (int i = 0; i < 16; i++)
        {
            boxTextures[i] = Content.Load<Texture2D>($"Items/Boxes/{i + 1}");
        }

        // Guia das texturas:
        // 1-2: Caixas de madeira simples
        // 3-4: Caixas de madeira com detalhes
        // 5-6: Caixas grandes/altas
        // 7-10: Barris variados (cerveja?)
        // 11-12: Potes/jarros
        // 13-16: Sacos/bags

        // Sala 1 - Caixas e barris na área caminhável
        room1.AddDecor(new DecorObject(boxTextures[0], new Vector2(320, 280)));   // Caixa madeira 1
        room1.AddDecor(new DecorObject(boxTextures[1], new Vector2(340, 280)));   // Caixa madeira 2
        room1.AddDecor(new DecorObject(boxTextures[6], new Vector2(420, 270)));   // Barril 1
        room1.AddDecor(new DecorObject(boxTextures[7], new Vector2(440, 270)));   // Barril 2
        room1.AddDecor(new DecorObject(boxTextures[10], new Vector2(380, 200)));  // Pote
        room1.AddDecor(new DecorObject(boxTextures[12], new Vector2(360, 150)));  // Saco

        // Sala 2 - Variedade de objetos
        room2.AddDecor(new DecorObject(boxTextures[2], new Vector2(100, 100)));   // Caixa detalhada 1
        room2.AddDecor(new DecorObject(boxTextures[3], new Vector2(100, 280)));   // Caixa detalhada 2
        room2.AddDecor(new DecorObject(boxTextures[8], new Vector2(400, 280)));   // Barril 3
        room2.AddDecor(new DecorObject(boxTextures[11], new Vector2(200, 150)));  // Jarro
        room2.AddDecor(new DecorObject(boxTextures[13], new Vector2(350, 180)));  // Saco 2
        room2.AddDecor(new DecorObject(boxTextures[14], new Vector2(50, 180)));   // Saco 3

        // Sala 3 - Sala do tesouro com muitos objetos (pra dificultar)
        room3.AddDecor(new DecorObject(boxTextures[4], new Vector2(100, 100)));   // Caixa grande 1
        room3.AddDecor(new DecorObject(boxTextures[5], new Vector2(120, 180)));   // Caixa grande 2
        room3.AddDecor(new DecorObject(boxTextures[9], new Vector2(130, 220)));   // Barril 4
        room3.AddDecor(new DecorObject(boxTextures[6], new Vector2(120, 210)));   // Barril 1
        room3.AddDecor(new DecorObject(boxTextures[15], new Vector2(150, 250)));  // Saco 4
        room3.AddDecor(new DecorObject(boxTextures[10], new Vector2(80, 200)));   // Pote

        // ═════════════════════════════════════════════════════════════════════
        // 📊 REGISTRA SALAS NO DUNGEON MANAGER
        // ═════════════════════════════════════════════════════════════════════

        DungeonManager.Instance.AddRoom(room1);
        DungeonManager.Instance.AddRoom(room2);
        DungeonManager.Instance.AddRoom(room3);
        DungeonManager.Instance.ChangeRoom(1);  // Começa na sala 1


        // ═════════════════════════════════════════════════════════════════════
        // PLAYER - O Herói!
        // ═════════════════════════════════════════════════════════════════════

        // Carrega todas as texturas de animação do player
        _playerTextures = new System.Collections.Generic.Dictionary<string, Texture2D>
        {
            { "Down", Content.Load<Texture2D>("Player/Player_Down") },        // Andando pra baixo
            { "Side", Content.Load<Texture2D>("Player/Player_Side") },        // Andando pro lado
            { "Up", Content.Load<Texture2D>("Player/Player_Up") },            // Andando pra cima
            { "Down_Idle", Content.Load<Texture2D>("Player/Player_Down_Idle") },  // Parado olhando baixo
            { "Side_Idle", Content.Load<Texture2D>("Player/Player_Side_Idle") },  // Parado olhando lado
            { "Up_Idle", Content.Load<Texture2D>("Player/Player_Up_Idle") }       // Parado olhando cima
        };

        // Cria o player na posição inicial (canto da sala 1)
        _player = new Player(_playerTextures, new Vector2(50, 80));

        // ═════════════════════════════════════════════════════════════════════
        // 🖼️ UI E EFEITOS VISUAIS
        // ═════════════════════════════════════════════════════════════════════

        // Carrega fonte e cria HUD
        _font = Content.Load<SpriteFont>("Fonts/GameFont");
        _hud = new HUD(_font);


        // ═════════════════════════════════════════════════════════════════════
        // MUSICA DE FUNDO
        try
        {
            System.Console.WriteLine("[DEBUG] Tentando carregar música...");
            _ambientMusic = Content.Load<Song>("Music/OnFlip");
            System.Console.WriteLine("[DEBUG] Música carregada com sucesso!");
            AudioManager.Instance.PlayAmbientMusic(_ambientMusic, 1.0f);
            System.Console.WriteLine("Ambient music loaded and playing.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Error loading or playing music: " + ex.Message);
            System.Console.WriteLine("Stack Trace: " + ex.StackTrace);
            if (ex.InnerException != null)
            {
                System.Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
        }

        // --- Vinheta: Efeito cinematográfico que escurece as bordas ---
        // Isso faz o centro da tela ficar mais iluminado
        // Parece profissional e ajuda o foco
        _vignetteTexture = new Texture2D(GraphicsDevice, 800, 600);
        Color[] data = new Color[800 * 600];  // Array com 480.000 pixels!
        Vector2 center = new Vector2(400, 300);  // Centro da tela
        float maxDist = Vector2.Distance(center, Vector2.Zero);  // Distância máxima

        // Preenche cada pixel com preto baseado na distância do centro
        for (int i = 0; i < data.Length; i++)
        {
            int x = i % 800;
            int y = i / 800;
            float dist = Vector2.Distance(new Vector2(x, y), center);
            float factor = dist / maxDist;
            factor = (float)Math.Pow(factor, 2.0); // Curva não-linear (mais suave)

            // Aplica preto com opacidade baseada na distância
            data[i] = Color.Black * Math.Min(factor * 1.8f, 0.85f);
        }
        _vignetteTexture.SetData(data);  // Aplica o array na textura

        // Textura de fade (1x1 pixel preto, esticado pra cobrir tela)
        _fadeTexture = new Texture2D(GraphicsDevice, 1, 1);
        _fadeTexture.SetData(new[] { Color.Black });

        // Textura de pixel branco (útil pra desenhar retângulos coloridos)
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        // Começa no menu principal
        _gameState = GameState.MainMenu;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 🔄 UPDATE - O Coração do Game Loop (60x por segundo)
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Atualiza toda a lógica do jogo.
    /// Roda 60 vezes por segundo (60 FPS).
    /// Ordem: Input → Física → IA → Colisão → Estado
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        InputManager.Instance.Update();  // Atualiza estado do input
        var currentKeyboard = Keyboard.GetState();  // Pega teclado atual

        // ═════════════════════════════════════════════════════════════════════
        // 📋 ESTADO: MENU PRINCIPAL
        // ═════════════════════════════════════════════════════════════════════

        if (_gameState == GameState.MainMenu)
        {
            Window.Title = "Dungeon of Algorithms";
            _menuPulseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;  // Timer do efeito pulsar

            // Navegação com setas ou WASD
            if (currentKeyboard.IsKeyDown(Keys.Up) && _lastKeyboardState.IsKeyUp(Keys.Up))
            {
                _menuSelectedIndex--;
                if (_menuSelectedIndex < 0) _menuSelectedIndex = _menuOptions.Length - 1;  // Loop!
            }
            if (currentKeyboard.IsKeyDown(Keys.Down) && _lastKeyboardState.IsKeyUp(Keys.Down))
            {
                _menuSelectedIndex++;
                if (_menuSelectedIndex >= _menuOptions.Length) _menuSelectedIndex = 0;  // Loop!
            }
            if (currentKeyboard.IsKeyDown(Keys.W) && _lastKeyboardState.IsKeyUp(Keys.W))
            {
                _menuSelectedIndex--;
                if (_menuSelectedIndex < 0) _menuSelectedIndex = _menuOptions.Length - 1;
            }
            if (currentKeyboard.IsKeyDown(Keys.S) && _lastKeyboardState.IsKeyUp(Keys.S))
            {
                _menuSelectedIndex++;
                if (_menuSelectedIndex >= _menuOptions.Length) _menuSelectedIndex = 0;
            }

            // Seleção com ENTER
            if (currentKeyboard.IsKeyDown(Keys.Enter) && _lastKeyboardState.IsKeyUp(Keys.Enter))
            {
                if (_menuSelectedIndex == 0) // JOGAR
                {
                    _gameState = GameState.Playing;  // Bora jogar!
                }
                else if (_menuSelectedIndex == 1) // SAIR
                {
                    Exit();  // Tchau!
                }
            }

            _lastKeyboardState = currentKeyboard;
            return;  // Não processa mais nada no menu
        }

        // ESC pra sair durante gameplay (se usar controle)
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            Exit();

        // ═════════════════════════════════════════════════════════════════════
        // 💀 ESTADO: GAME OVER (Kernel Panic)
        // ═════════════════════════════════════════════════════════════════════

        if (_gameState == GameState.GameOver)
        {
            Window.Title = "GAME OVER - Press R to Restart";
            if (currentKeyboard.IsKeyDown(Keys.R))
            {
                // Reset completo - novo player, volta pra sala 1
                _player = new Player(_playerTextures, new Vector2(40, 20));
                DungeonManager.Instance.ChangeRoom(1);
                if (_ambientMusic != null)
                {
                    AudioManager.Instance.PlayAmbientMusic(_ambientMusic, 1.0f);
                }
                _gameState = GameState.Playing;
            }
            _lastKeyboardState = currentKeyboard;
            return;
        }

        // ═════════════════════════════════════════════════════════════════════
        // ⏸️ PAUSA - P pra pausar/despausar
        // ═════════════════════════════════════════════════════════════════════

        if (Keyboard.GetState().IsKeyDown(Keys.P) && _lastKeyboardState.IsKeyUp(Keys.P))
        {
            _gameState = _gameState == GameState.Paused ? GameState.Playing : GameState.Paused;

            if (_gameState == GameState.Paused)
            {
                AudioManager.Instance.PauseAmbientMusic();
            }
            else
            {
                AudioManager.Instance.ResumeAmbientMusic();
            }
        }
        _lastKeyboardState = Keyboard.GetState();

        if (_gameState == GameState.Paused)
        {
            Window.Title = "PAUSED - Press P to Resume";
            return;  // Congela tudo!
        }



        // ═════════════════════════════════════════════════════════════════════
        // 🏆 ESTADO: VITÓRIA!
        // ═════════════════════════════════════════════════════════════════════

        if (_gameState == GameState.Victory)
        {
            Window.Title = "VICTORY! - Press R to Restart";
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                // Zerou? Joga de novo!
                _player = new Player(_playerTextures, new Vector2(100, 100));
                DungeonManager.Instance.ChangeRoom(1);
                _gameState = GameState.Playing;
            }
            return;
        }

        // ═════════════════════════════════════════════════════════════════════
        // 🎮 ESTADO: JOGANDO
        // ═════════════════════════════════════════════════════════════════════

        var tilemap = DungeonManager.Instance.CurrentRoom.Tilemap;
        var currentRoom = DungeonManager.Instance.CurrentRoom;

        // Guarda posição antes de mover (pra reverter se bater em algo)
        Vector2 prevPosition = _player.Position;
        _player.Update(gameTime, tilemap);  // Atualiza player (movimento, animação)

        // Verifica colisão com objetos decorativos
        if (currentRoom.IsCollidingWithDecor(_player.Bounds))
        {
            _player.SetPosition(prevPosition);  // Bateu numa caixa? Volta!
        }

        // Player morreu?
        if (!_player.IsAlive)
        {
            _gameState = GameState.GameOver;
            return;
        }

        // Atualiza sala atual (inimigos, itens, coleta)
        // O lambda é chamado quando pega um item
        DungeonManager.Instance.CurrentRoom.Update(gameTime, _player, (item) =>
        {
            // Se pegou o baú, ganhou!
            if (item is DungeonOfAlgorithms.Source.Entities.ChestItem)
            {
                _gameState = GameState.Victory;
            }
        });

        // Mostra info de debug na barra de título
        Window.Title = $"HP: {_player.Health} | Score: {_player.Score} | Room: {DungeonManager.Instance.CurrentRoom.Id}";

        // ═════════════════════════════════════════════════════════════════════
        // 🌓 TRANSIÇÃO DE SALA COM FADE
        // ═════════════════════════════════════════════════════════════════════
        // Funciona assim:
        // 1. Detecta porta → _isFading = true
        // 2. Escurece tela (_fadeOut = true, _fadeAlpha aumenta)
        // 3. Quando fica preto total → muda de sala
        // 4. Clareia tela (_fadeOut = false, _fadeAlpha diminui)
        // 5. _isFading = false → jogador volta a ter controle

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_isFading)
        {
            if (_fadeOut)
            {
                // Escurecendo...
                _fadeAlpha += FADE_SPEED * deltaTime;
                if (_fadeAlpha >= 1f)
                {
                    _fadeAlpha = 1f;
                    // Tá totalmente preto - hora de mudar de sala!
                    if (_pendingRoom >= 0)
                    {
                        DungeonManager.Instance.ChangeRoom(_pendingRoom);
                        _player.SetPosition(_pendingSpawnPosition);
                        System.Console.WriteLine($"Transição para Room {_pendingRoom}");
                        _pendingRoom = -1;
                    }
                    _fadeOut = false; // Agora começa a clarear
                }
            }
            else
            {
                // Clareando...
                _fadeAlpha -= FADE_SPEED * deltaTime;
                if (_fadeAlpha <= 0f)
                {
                    _fadeAlpha = 0f;
                    _isFading = false;
                    _fadeOut = true; // Reseta pra próxima transição
                }
            }
            // Durante fade, não processa movimento
            base.Update(gameTime);
            return;
        }

        // ═════════════════════════════════════════════════════════════════════
        // DETECÇÃO DE PORTAS (Transição de Sala)
        // ═════════════════════════════════════════════════════════════════════
        // Portas são aberturas nas paredes (tile vazio perto da borda)

        int mapWidth = currentRoom.Tilemap.MapWidth;
        int mapHeight = currentRoom.Tilemap.MapHeight;

        // Pega o tile embaixo do centro do player
        int playerCenterX = (int)(_player.Position.X + 8);
        int playerCenterY = (int)(_player.Position.Y + 8);
        int tileUnderPlayer = currentRoom.Tilemap.GetTileAt(playerCenterX, playerCenterY);

        // Verifica condições pra transição
        bool isOnEmptyTile = tileUnderPlayer == -1;  // Tile vazio (abertura)
        bool nearEastEdge = _player.Position.X >= mapWidth - 48;    // Perto da borda leste
        bool nearWestEdge = _player.Position.X <= 32;               // Perto da borda oeste
        bool nearNorthEdge = _player.Position.Y <= 32;              // Perto da borda norte
        bool nearSouthEdge = _player.Position.Y >= mapHeight - 48;  // Perto da borda sul
        // Checa cada direção e inicia transição se tiver conexão
        if (isOnEmptyTile && nearEastEdge && currentRoom.Connections.ContainsKey("East") && !_isFading)
        {
            _pendingRoom = currentRoom.Connections["East"];
            // Sala 3 precisa de Y diferente (mapa mais alto)
            float spawnY = _pendingRoom == 3 ? 200 : _player.Position.Y;
            _pendingSpawnPosition = new Vector2(80, spawnY);
            _isFading = true;
            _fadeOut = true;
        }
        else if (isOnEmptyTile && nearWestEdge && currentRoom.Connections.ContainsKey("West") && !_isFading)
        {
            _pendingRoom = currentRoom.Connections["West"];
            _pendingSpawnPosition = new Vector2(mapWidth - 100, _player.Position.Y);
            _isFading = true;
            _fadeOut = true;
        }
        else if (isOnEmptyTile && nearNorthEdge && currentRoom.Connections.ContainsKey("North") && !_isFading)
        {
            _pendingRoom = currentRoom.Connections["North"];
            _pendingSpawnPosition = new Vector2(_player.Position.X, mapHeight - 100);
            _isFading = true;
            _fadeOut = true;
        }
        else if (isOnEmptyTile && nearSouthEdge && currentRoom.Connections.ContainsKey("South") && !_isFading)
        {
            _pendingRoom = currentRoom.Connections["South"];
            _pendingSpawnPosition = new Vector2(_player.Position.X, 80);
            _isFading = true;
            _fadeOut = true;
        }

        // ═════════════════════════════════════════════════════════════════════
        // 💾 SAVE/LOAD - F5 Salva, F9 Carrega
        // ═════════════════════════════════════════════════════════════════════

        try
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                // Salva no banco SQLite
                DatabaseManager.Instance.SaveGame(DungeonOfAlgorithms.Source.Core.DungeonManager.Instance.CurrentRoom.Id, _player.Position, 0);
                Window.Title = "Game Saved!";  // Feedback visual
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F9))
            {
                // Carrega do banco SQLite
                var data = DatabaseManager.Instance.LoadGame();
                if (data != null)
                {
                    DungeonOfAlgorithms.Source.Core.DungeonManager.Instance.ChangeRoom(data.Value.Level);
                    _player.SetPosition(data.Value.Position);
                    Window.Title = "Game Loaded!";
                }
            }
        }
        catch (System.Exception ex)
        {
            // Deu ruim no banco? Log e mostra erro
            System.Console.WriteLine("CRITICAL ERROR: " + ex.Message);
            Window.Title = "Error: " + ex.Message;
        }

        // Atualiza câmera pra seguir o player
        _camera.Follow(_player.Position, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        base.Update(gameTime);  // Chama Update() da classe base
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 🎨 DRAW - Desenha TUDO na Tela (60x por segundo)
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Renderiza o jogo na tela.
    /// Ordem importa: fundo → objetos → player → UI → fade
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);  // Limpa tela (cor de fundo)

        // ═════════════════════════════════════════════════════════════════════
        // 🎮 WORLD SPACE - Desenha com transformação da câmera
        // ═════════════════════════════════════════════════════════════════════
        // Tudo que se move com a câmera vai aqui

        _spriteBatch.Begin(transformMatrix: _camera.Transform, samplerState: SamplerState.PointClamp);

        DungeonManager.Instance.CurrentRoom.Draw(_spriteBatch);  // Mapa, inimigos, itens
        _player.Draw(_spriteBatch);                               // Player

        _spriteBatch.End();

        // ═════════════════════════════════════════════════════════════════════
        // 🖼️ SCREEN SPACE - Desenha fixo na tela (sem câmera)
        // ═════════════════════════════════════════════════════════════════════
        // UI, menus, efeitos - não afetados pela câmera

        _spriteBatch.Begin();

        // Efeito de vinheta (bordas escuras)
        if (_vignetteTexture != null)
            _spriteBatch.Draw(_vignetteTexture, Vector2.Zero, Color.White);

        // ═════════════════════════════════════════════════════════════════════
        // 📋 DESENHO DO MENU PRINCIPAL
        // ═════════════════════════════════════════════════════════════════════

        if (_gameState == GameState.MainMenu)
        {
            // Overlay escuro de fundo
            _spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, 800, 600), Color.Black * 0.7f);

            // Centralizar tudo baseado na largura da tela (800px)
            int screenCenterX = 400;

            // ═══════ TÍTULO ═══════
            string title = "DUNGEON OF ALGORITHMS";
            Vector2 titleSize = _font.MeasureString(title);  // Mede o texto pra centralizar
            DrawShadowString(_font, title, new Vector2(screenCenterX - titleSize.X / 2, 100), Color.Gold);

            string subtitle = "The Memory Leak Chronicle";
            Vector2 subtitleSize = _font.MeasureString(subtitle);
            DrawShadowString(_font, subtitle, new Vector2(screenCenterX - subtitleSize.X / 2, 135), Color.LightGreen);

            // Linha decorativa
            _spriteBatch.Draw(_pixelTexture, new Rectangle(screenCenterX - 150, 170, 300, 2), Color.Gold * 0.5f);

            // ═══════ OPÇÕES DO MENU ═══════
            int menuStartY = 280;
            int menuSpacing = 45;

            for (int i = 0; i < _menuOptions.Length; i++)
            {
                bool isSelected = (i == _menuSelectedIndex);

                // Mede o texto pra centralizar perfeitamente
                Vector2 textSize = _font.MeasureString(_menuOptions[i]);
                Vector2 optionPos = new Vector2(screenCenterX - textSize.X / 2, menuStartY + (i * menuSpacing));

                // Efeito de pulsar na opção selecionada (usa seno pro efeito)
                float pulse = isSelected ? (float)(0.8f + 0.2f * Math.Sin(_menuPulseTimer * 5)) : 0.6f;
                Color optionColor = isSelected ? Color.Gold * pulse : Color.White * 0.6f;

                // Se selecionado, desenha setas e caixa de destaque
                if (isSelected)
                {
                    float arrowOffset = (float)(Math.Sin(_menuPulseTimer * 8) * 3);  // Setas balançam

                    // Caixa de highlight
                    int boxPadding = 8;
                    _spriteBatch.Draw(_pixelTexture,
                        new Rectangle((int)(optionPos.X - boxPadding), (int)(optionPos.Y - 2),
                                      (int)(textSize.X + boxPadding * 2), (int)(textSize.Y + 4)),
                        Color.Gold * 0.15f);

                    // Setas animadas
                    DrawShadowString(_font, ">", new Vector2(optionPos.X - 20 + arrowOffset, optionPos.Y), Color.Gold);
                    DrawShadowString(_font, "<", new Vector2(optionPos.X + textSize.X + 8 - arrowOffset, optionPos.Y), Color.Gold);
                }

                DrawShadowString(_font, _menuOptions[i], optionPos, optionColor);
            }

            // Linha decorativa inferior
            _spriteBatch.Draw(_pixelTexture, new Rectangle(screenCenterX - 150, 400, 300, 2), Color.Gold * 0.5f);

            // ═══════ DICAS DE CONTROLE ═══════
            string hint1 = "W/S ou Setas para navegar";
            Vector2 hint1Size = _font.MeasureString(hint1);
            DrawShadowString(_font, hint1, new Vector2(screenCenterX - hint1Size.X / 2, 430), Color.Gray * 0.8f);

            string hint2 = "ENTER para selecionar";
            Vector2 hint2Size = _font.MeasureString(hint2);
            DrawShadowString(_font, hint2, new Vector2(screenCenterX - hint2Size.X / 2, 455), Color.Gray * 0.8f);

            // Versão no rodapé
            string version = "v1.0 - Dungeon of Algorithms";
            Vector2 versionSize = _font.MeasureString(version);
            DrawShadowString(_font, version, new Vector2(screenCenterX - versionSize.X / 2, 550), Color.Gray * 0.5f);
        }

        // ═════════════════════════════════════════════════════════════════════
        // 💀 TELA DE GAME OVER
        // ═════════════════════════════════════════════════════════════════════

        else if (_gameState == GameState.GameOver)
        {
            DrawShadowString(_font, "KERNEL PANIC (GAME OVER)", new Vector2(260, 180), Color.Red);
            DrawShadowString(_font, $"Leaked Objects Cleaned: {_player.Score}", new Vector2(260, 220), Color.White);
            DrawShadowString(_font, "Press R to Reboot System", new Vector2(250, 260), Color.Gray);
        }

        // ═════════════════════════════════════════════════════════════════════
        // 🏆 TELA DE VITÓRIA
        // ═════════════════════════════════════════════════════════════════════

        else if (_gameState == GameState.Victory)
        {
            DrawShadowString(_font, "SYSTEM RESTORED (VICTORY!)", new Vector2(260, 150), Color.Gold);
            DrawShadowString(_font, "You patched the memory leak!", new Vector2(240, 200), Color.White);
            DrawShadowString(_font, $"Final Score: {_player.Score}", new Vector2(260, 250), Color.White);
            DrawShadowString(_font, "Press R to Reboot", new Vector2(320, 300), Color.Gray);
        }

        // ═════════════════════════════════════════════════════════════════════
        // 🎮 HUD DURANTE GAMEPLAY
        // ═════════════════════════════════════════════════════════════════════

        else
        {
            _hud.Draw(_spriteBatch, _player);  // HP e Score
        }

        _spriteBatch.End();

        // ═════════════════════════════════════════════════════════════════════
        // 🌑 OVERLAY DE FADE (por cima de TUDO)
        // ═════════════════════════════════════════════════════════════════════

        if (_fadeAlpha > 0)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, 800, 600), Color.White * _fadeAlpha);
            _spriteBatch.End();
        }

        base.Draw(gameTime);  // Chama Draw() da classe base
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 🔤 HELPER: Texto com Sombra
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Desenha texto com sombra pra melhor legibilidade.
    /// Desenha primeiro a sombra (preto deslocado) depois o texto.
    /// </summary>
    private void DrawShadowString(SpriteFont font, string text, Vector2 position, Color color)
    {
        _spriteBatch.DrawString(font, text, position + new Vector2(1, 1), Color.Black);  // Sombra
        _spriteBatch.DrawString(font, text, position, color);                             // Texto
    }
}
