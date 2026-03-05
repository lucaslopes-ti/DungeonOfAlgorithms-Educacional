// =============================================================================
// GAME1 - O Cerebro Central do Dungeon of Algorithms
// =============================================================================
// Esta e a classe principal do jogo. Herda de Game (MonoGame).
// Game Loop:
//   Initialize() -> LoadContent() -> [ Update() -> Draw() ] <- repete 60x/segundo
// =============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using DungeonOfAlgorithms.Source.Entities;
using Microsoft.Xna.Framework.Media;

namespace DungeonOfAlgorithms.Source.Core;

public class Game1 : Game
{
    // Resolucao do jogo
    private const int SCREEN_WIDTH = 1280;
    private const int SCREEN_HEIGHT = 720;

    // Graficos
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Entidades e sistemas
    private Player _player;
    private Camera _camera;
    private HUD _hud;
    private GameState _gameState = GameState.MainMenu;
    private SpriteFont _font;
    private KeyboardState _lastKeyboardState;
    private System.Collections.Generic.Dictionary<string, Texture2D> _playerTextures;
    private Texture2D _vignetteTexture;
    private Song _ambientMusic;

    // Sistema de fade
    private Texture2D _fadeTexture;
    private float _fadeAlpha = 0f;
    private bool _isFading = false;
    private bool _fadeOut = true;
    private const float FADE_SPEED = 3f;
    private int _pendingRoom = -1;
    private Vector2 _pendingSpawnPosition;

    // Sistema de menu
    private int _menuSelectedIndex = 0;
    private string[] _menuOptions = { "JOGAR", "CONTINUAR", "CREDITOS", "SAIR" };
    private float _menuPulseTimer = 0f;
    private Texture2D _pixelTexture;
    private bool _hasSaveData = false;
    private float _titleScale = 1f;

    // Pause menu
    private int _pauseSelectedIndex = 0;
    private string[] _pauseOptions = { "CONTINUAR", "MENU PRINCIPAL" };

    // Credits scroll
    private float _creditsScrollY = 0f;

    // Game Over / Victory timers para animacao
    private float _endScreenTimer = 0f;

    // Sistema de mensagens/tutorial
    private string _currentMessage = "";
    private float _messageTimer = 0f;
    private const float MESSAGE_DURATION = 4f;
    private float _messageAlpha = 0f;
    private bool _showIntro = true;

    // Intro overlay (tela de objetivo)
    private float _introTimer = 0f;
    private const float INTRO_DURATION = 6f;

    // Texto de transicao de sala
    private string _roomTransitionText = "";
    private float _roomTransitionTimer = 0f;
    private const float ROOM_TRANSITION_DURATION = 2.5f;

    // Notificacao de moedas completas
    private bool _allCoinsNotified = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
    }

    protected override void Initialize()
    {
        GameManager.Instance.Initialize();
        _camera = new Camera();

        Window.Title = "Dungeon of Algorithms";

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Criacao das salas (Graph Structure)
        var tilesetTexture = Content.Load<Texture2D>("Tiles/Tileset");

        int[,] map1 = DungeonManager.Instance.LoadMapFromCSV("Content/Maps/Room_01.csv");
        var room1 = new Room(1, new Tilemap(tilesetTexture, map1, 16, 16));

        int[,] map2 = DungeonManager.Instance.LoadMapFromCSV("Content/Maps/Room_02.csv");
        var room2 = new Room(2, new Tilemap(tilesetTexture, map2, 16, 16));

        int[,] map3 = DungeonManager.Instance.LoadMapFromCSV("Content/Maps/Room_03.csv");
        var room3 = new Room(3, new Tilemap(tilesetTexture, map3, 16, 16));

        // Conexoes (Arestas do Grafo)
        room1.Connect("East", 2);
        room2.Connect("West", 1);
        room2.Connect("East", 3);
        room3.Connect("West", 2);

        // Itens
        ItemFactory.Initialize(Content);

        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(80, 80)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(40, 80)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(150, 200)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(300, 100)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(100, 320)));
        room1.AddItem(ItemFactory.CreateItem("Coin", new Vector2(400, 150)));

        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(100, 100)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(200, 150)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(350, 200)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(150, 250)));
        room2.AddItem(ItemFactory.CreateItem("Coin", new Vector2(400, 100)));

        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(100, 150)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(150, 200)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(160, 120)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(250, 80)));
        room3.AddItem(ItemFactory.CreateItem("Coin", new Vector2(300, 100)));

        // Inimigos
        EnemyFactory.Initialize(Content);

        room1.AddEnemy(EnemyFactory.CreateEnemy("Slime", new Vector2(350, 270)));
        room1.AddEnemy(EnemyFactory.CreateEnemy("Slime", new Vector2(400, 200)));
        room1.AddEnemy(EnemyFactory.CreateEnemy("Boss", new Vector2(200, 100)));
        room1.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(80, 230)));
        room1.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(80, 200)));

        room2.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(100, 200)));
        room2.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(200, 200)));
        room2.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(350, 150)));

        room3.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(150, 200)));
        room3.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(200, 200)));
        room3.AddEnemy(EnemyFactory.CreateEnemy("Ghost", new Vector2(250, 200)));

        // Bau na sala 3 - no final do cenario (area walkable: Y 48-224)
        room3.AddItem(ItemFactory.CreateItem("Chest", new Vector2(420, 130)));

        // Objetos decorativos
        var boxTextures = new Texture2D[16];
        for (int i = 0; i < 16; i++)
        {
            boxTextures[i] = Content.Load<Texture2D>($"Items/Boxes/{i + 1}");
        }

        room1.AddDecor(new DecorObject(boxTextures[0], new Vector2(320, 180)));
        room1.AddDecor(new DecorObject(boxTextures[1], new Vector2(340, 180)));
        room1.AddDecor(new DecorObject(boxTextures[6], new Vector2(420, 170)));
        room1.AddDecor(new DecorObject(boxTextures[7], new Vector2(440, 170)));
        room1.AddDecor(new DecorObject(boxTextures[10], new Vector2(380, 120)));
        room1.AddDecor(new DecorObject(boxTextures[12], new Vector2(360, 100)));

        room2.AddDecor(new DecorObject(boxTextures[2], new Vector2(100, 80)));
        room2.AddDecor(new DecorObject(boxTextures[3], new Vector2(100, 200)));
        room2.AddDecor(new DecorObject(boxTextures[8], new Vector2(400, 200)));
        room2.AddDecor(new DecorObject(boxTextures[11], new Vector2(200, 120)));
        room2.AddDecor(new DecorObject(boxTextures[13], new Vector2(350, 150)));
        room2.AddDecor(new DecorObject(boxTextures[14], new Vector2(50, 150)));

        room3.AddDecor(new DecorObject(boxTextures[4], new Vector2(200, 80)));
        room3.AddDecor(new DecorObject(boxTextures[5], new Vector2(300, 180)));
        room3.AddDecor(new DecorObject(boxTextures[9], new Vector2(350, 100)));
        room3.AddDecor(new DecorObject(boxTextures[6], new Vector2(250, 150)));
        room3.AddDecor(new DecorObject(boxTextures[15], new Vector2(380, 180)));
        room3.AddDecor(new DecorObject(boxTextures[10], new Vector2(150, 100)));

        // Registra salas
        DungeonManager.Instance.AddRoom(room1);
        DungeonManager.Instance.AddRoom(room2);
        DungeonManager.Instance.AddRoom(room3);
        DungeonManager.Instance.ChangeRoom(1);

        // Player
        _playerTextures = new System.Collections.Generic.Dictionary<string, Texture2D>
        {
            { "Down", Content.Load<Texture2D>("Player/Player_Down") },
            { "Side", Content.Load<Texture2D>("Player/Player_Side") },
            { "Up", Content.Load<Texture2D>("Player/Player_Up") },
            { "Down_Idle", Content.Load<Texture2D>("Player/Player_Down_Idle") },
            { "Side_Idle", Content.Load<Texture2D>("Player/Player_Side_Idle") },
            { "Up_Idle", Content.Load<Texture2D>("Player/Player_Up_Idle") }
        };

        _player = new Player(_playerTextures, new Vector2(50, 80));

        // UI e efeitos visuais
        _font = Content.Load<SpriteFont>("Fonts/GameFont");
        _hud = new HUD(_font);

        // Musica de fundo
        try
        {
            _ambientMusic = Content.Load<Song>("Music/OnFlip");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.7f;
            MediaPlayer.Play(_ambientMusic);
            System.Console.WriteLine("[Music] BGM started successfully");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("[Music] Error loading music: " + ex.Message);
            System.Console.WriteLine("[Music] Stack: " + ex.StackTrace);
        }

        // Vinheta para 1280x720
        _vignetteTexture = new Texture2D(GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
        Color[] data = new Color[SCREEN_WIDTH * SCREEN_HEIGHT];
        Vector2 center = new Vector2(SCREEN_WIDTH / 2f, SCREEN_HEIGHT / 2f);
        float maxDist = Vector2.Distance(center, Vector2.Zero);

        for (int i = 0; i < data.Length; i++)
        {
            int x = i % SCREEN_WIDTH;
            int y = i / SCREEN_WIDTH;
            float dist = Vector2.Distance(new Vector2(x, y), center);
            float factor = dist / maxDist;
            factor = (float)Math.Pow(factor, 2.0);
            data[i] = Color.Black * Math.Min(factor * 1.8f, 0.85f);
        }
        _vignetteTexture.SetData(data);

        // Texturas auxiliares
        _fadeTexture = new Texture2D(GraphicsDevice, 1, 1);
        _fadeTexture.SetData(new[] { Color.Black });

        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        // Verifica se tem save
        try
        {
            var saveData = DatabaseManager.Instance.LoadGame();
            _hasSaveData = saveData != null;
        }
        catch
        {
            _hasSaveData = false;
        }

        _gameState = GameState.MainMenu;
    }

    // =========================================================================
    // UPDATE
    // =========================================================================

    protected override void Update(GameTime gameTime)
    {
        InputManager.Instance.Update();
        var currentKeyboard = Keyboard.GetState();
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // F11 para fullscreen
        if (currentKeyboard.IsKeyDown(Keys.F11) && _lastKeyboardState.IsKeyUp(Keys.F11))
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges();
        }

        // =============================================
        // MENU PRINCIPAL
        // =============================================

        if (_gameState == GameState.MainMenu)
        {
            _menuPulseTimer += deltaTime;
            _titleScale = 1f + 0.03f * (float)Math.Sin(_menuPulseTimer * 2);

            // Navegacao
            if ((currentKeyboard.IsKeyDown(Keys.Up) && _lastKeyboardState.IsKeyUp(Keys.Up)) ||
                (currentKeyboard.IsKeyDown(Keys.W) && _lastKeyboardState.IsKeyUp(Keys.W)))
            {
                _menuSelectedIndex--;
                if (_menuSelectedIndex < 0) _menuSelectedIndex = _menuOptions.Length - 1;
            }
            if ((currentKeyboard.IsKeyDown(Keys.Down) && _lastKeyboardState.IsKeyUp(Keys.Down)) ||
                (currentKeyboard.IsKeyDown(Keys.S) && _lastKeyboardState.IsKeyUp(Keys.S)))
            {
                _menuSelectedIndex++;
                if (_menuSelectedIndex >= _menuOptions.Length) _menuSelectedIndex = 0;
            }

            // Skip CONTINUAR se nao tem save
            if (_menuSelectedIndex == 1 && !_hasSaveData)
            {
                if (currentKeyboard.IsKeyDown(Keys.Down) || currentKeyboard.IsKeyDown(Keys.S))
                    _menuSelectedIndex = 2;
                else
                    _menuSelectedIndex = 0;
            }

            // Selecao
            if (currentKeyboard.IsKeyDown(Keys.Enter) && _lastKeyboardState.IsKeyUp(Keys.Enter))
            {
                switch (_menuSelectedIndex)
                {
                    case 0: // JOGAR
                        ResetGame();
                        _gameState = GameState.Playing;
                        _showIntro = true;
                        _introTimer = INTRO_DURATION;
                        ShowRoomTransition(1);
                        break;
                    case 1: // CONTINUAR
                        if (_hasSaveData)
                        {
                            LoadSavedGame();
                            _gameState = GameState.Playing;
                            _showIntro = false;
                        }
                        break;
                    case 2: // CREDITOS
                        _creditsScrollY = 0f;
                        _gameState = GameState.Credits;
                        break;
                    case 3: // SAIR
                        Exit();
                        break;
                }
            }

            _lastKeyboardState = currentKeyboard;
            base.Update(gameTime);
            return;
        }

        // =============================================
        // CREDITOS
        // =============================================

        if (_gameState == GameState.Credits)
        {
            _creditsScrollY += 40f * deltaTime;

            if (currentKeyboard.IsKeyDown(Keys.Escape) && _lastKeyboardState.IsKeyUp(Keys.Escape))
            {
                _gameState = GameState.MainMenu;
            }

            _lastKeyboardState = currentKeyboard;
            base.Update(gameTime);
            return;
        }

        // =============================================
        // GAME OVER
        // =============================================

        if (_gameState == GameState.GameOver)
        {
            _endScreenTimer += deltaTime;

            if (currentKeyboard.IsKeyDown(Keys.R) && _lastKeyboardState.IsKeyUp(Keys.R))
            {
                ResetGame();
                _gameState = GameState.Playing;
                _showIntro = true;
                _introTimer = INTRO_DURATION;
                ShowRoomTransition(1);
            }
            if (currentKeyboard.IsKeyDown(Keys.M) && _lastKeyboardState.IsKeyUp(Keys.M))
            {
                _gameState = GameState.MainMenu;
                _menuSelectedIndex = 0;
            }

            _lastKeyboardState = currentKeyboard;
            base.Update(gameTime);
            return;
        }

        // =============================================
        // VITORIA
        // =============================================

        if (_gameState == GameState.Victory)
        {
            _endScreenTimer += deltaTime;

            if (currentKeyboard.IsKeyDown(Keys.R) && _lastKeyboardState.IsKeyUp(Keys.R))
            {
                ResetGame();
                _gameState = GameState.Playing;
                _showIntro = true;
                _introTimer = INTRO_DURATION;
                ShowRoomTransition(1);
            }
            if (currentKeyboard.IsKeyDown(Keys.M) && _lastKeyboardState.IsKeyUp(Keys.M))
            {
                _gameState = GameState.MainMenu;
                _menuSelectedIndex = 0;
            }

            _lastKeyboardState = currentKeyboard;
            base.Update(gameTime);
            return;
        }

        // =============================================
        // PAUSA (P ou ESC para pausar/despausar)
        // =============================================

        bool pausePressed = (currentKeyboard.IsKeyDown(Keys.P) && _lastKeyboardState.IsKeyUp(Keys.P)) ||
                            (currentKeyboard.IsKeyDown(Keys.Escape) && _lastKeyboardState.IsKeyUp(Keys.Escape));

        if (pausePressed && _gameState == GameState.Playing)
        {
            _gameState = GameState.Paused;
            _pauseSelectedIndex = 0;
            AudioManager.Instance.PauseAmbientMusic();
        }

        if (_gameState == GameState.Paused)
        {
            _menuPulseTimer += deltaTime;

            // Navegacao do menu de pausa
            if ((currentKeyboard.IsKeyDown(Keys.Up) && _lastKeyboardState.IsKeyUp(Keys.Up)) ||
                (currentKeyboard.IsKeyDown(Keys.W) && _lastKeyboardState.IsKeyUp(Keys.W)))
            {
                _pauseSelectedIndex--;
                if (_pauseSelectedIndex < 0) _pauseSelectedIndex = _pauseOptions.Length - 1;
            }
            if ((currentKeyboard.IsKeyDown(Keys.Down) && _lastKeyboardState.IsKeyUp(Keys.Down)) ||
                (currentKeyboard.IsKeyDown(Keys.S) && _lastKeyboardState.IsKeyUp(Keys.S)))
            {
                _pauseSelectedIndex++;
                if (_pauseSelectedIndex >= _pauseOptions.Length) _pauseSelectedIndex = 0;
            }

            if (currentKeyboard.IsKeyDown(Keys.Enter) && _lastKeyboardState.IsKeyUp(Keys.Enter))
            {
                if (_pauseSelectedIndex == 0) // CONTINUAR
                {
                    _gameState = GameState.Playing;
                    AudioManager.Instance.ResumeAmbientMusic();
                }
                else if (_pauseSelectedIndex == 1) // MENU PRINCIPAL
                {
                    _gameState = GameState.MainMenu;
                    _menuSelectedIndex = 0;
                    AudioManager.Instance.StopAmbientMusic();
                    StartBGM();
                }
            }

            // P ou ESC para despausar rapido
            if (pausePressed)
            {
                _gameState = GameState.Playing;
                AudioManager.Instance.ResumeAmbientMusic();
            }

            _lastKeyboardState = currentKeyboard;
            base.Update(gameTime);
            return;
        }

        // =============================================
        // JOGANDO
        // =============================================

        _lastKeyboardState = currentKeyboard;

        // Atualiza timers de mensagem
        if (_messageTimer > 0)
        {
            _messageTimer -= deltaTime;
            _messageAlpha = _messageTimer > 1f ? 1f : Math.Max(0, _messageTimer);
        }
        if (_roomTransitionTimer > 0)
        {
            _roomTransitionTimer -= deltaTime;
        }
        if (_introTimer > 0)
        {
            _introTimer -= deltaTime;
            if (_introTimer <= 0)
                _showIntro = false;
        }

        var tilemap = DungeonManager.Instance.CurrentRoom.Tilemap;
        var currentRoom = DungeonManager.Instance.CurrentRoom;

        Vector2 prevPosition = _player.Position;
        _player.Update(gameTime, tilemap);

        // Colisao com decor usando sliding (X e Y separados)
        Vector2 afterMove = _player.Position;
        if (currentRoom.IsCollidingWithDecor(_player.Bounds))
        {
            // Tenta manter X, reverte Y
            _player.SetPosition(new Vector2(afterMove.X, prevPosition.Y));
            if (currentRoom.IsCollidingWithDecor(_player.Bounds))
            {
                // Tenta manter Y, reverte X
                _player.SetPosition(new Vector2(prevPosition.X, afterMove.Y));
                if (currentRoom.IsCollidingWithDecor(_player.Bounds))
                {
                    // Ambos falham, reverte tudo
                    _player.SetPosition(prevPosition);
                }
            }
        }

        if (!_player.IsAlive)
        {
            _gameState = GameState.GameOver;
            _endScreenTimer = 0f;
            AudioManager.Instance.StopAmbientMusic();
            return;
        }

        // Atualiza estado do bau (trancado/destrancado)
        bool allCoins = DungeonManager.Instance.AllCoinsCollected;
        foreach (var room in DungeonManager.Instance.Rooms.Values)
        {
            foreach (var item in room.Items)
            {
                if (item is ChestItem chestItem)
                    chestItem.IsUnlocked = allCoins;
            }
        }

        // Notificacao quando todas as moedas sao coletadas
        if (allCoins && !_allCoinsNotified)
        {
            _allCoinsNotified = true;
            ShowMessage("Todas as moedas coletadas! Encontre o bau!");
        }

        DungeonManager.Instance.CurrentRoom.Update(gameTime, _player, (item) =>
        {
            if (item is ChestItem)
            {
                _gameState = GameState.Victory;
                _endScreenTimer = 0f;
            }
            else
            {
                // Feedback ao coletar moeda
                int collected = DungeonManager.Instance.CollectedCoins;
                int total = DungeonManager.Instance.TotalCoins;
                if (collected < total)
                    ShowMessage($"Moeda coletada! ({collected}/{total})");
            }
        });

        // Sistema de fade para transicao de sala
        if (_isFading)
        {
            if (_fadeOut)
            {
                _fadeAlpha += FADE_SPEED * deltaTime;
                if (_fadeAlpha >= 1f)
                {
                    _fadeAlpha = 1f;
                    if (_pendingRoom >= 0)
                    {
                        DungeonManager.Instance.ChangeRoom(_pendingRoom);
                        _player.SetPosition(_pendingSpawnPosition);
                        ShowRoomTransition(_pendingRoom);
                        _pendingRoom = -1;
                    }
                    _fadeOut = false;
                }
            }
            else
            {
                _fadeAlpha -= FADE_SPEED * deltaTime;
                if (_fadeAlpha <= 0f)
                {
                    _fadeAlpha = 0f;
                    _isFading = false;
                    _fadeOut = true;
                }
            }
            base.Update(gameTime);
            return;
        }

        // Deteccao de portas
        int mapWidth = currentRoom.Tilemap.MapWidth;
        int mapHeight = currentRoom.Tilemap.MapHeight;
        int playerCenterX = (int)(_player.Position.X + 8);
        int playerCenterY = (int)(_player.Position.Y + 8);
        int tileUnderPlayer = currentRoom.Tilemap.GetTileAt(playerCenterX, playerCenterY);

        bool isOnEmptyTile = tileUnderPlayer == -1;
        bool nearEastEdge = _player.Position.X >= mapWidth - 48;
        bool nearWestEdge = _player.Position.X <= 32;
        bool nearNorthEdge = _player.Position.Y <= 32;
        bool nearSouthEdge = _player.Position.Y >= mapHeight - 48;

        if (isOnEmptyTile && nearEastEdge && currentRoom.Connections.ContainsKey("East") && !_isFading)
        {
            _pendingRoom = currentRoom.Connections["East"];
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

        // Save/Load
        try
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F5) && _lastKeyboardState.IsKeyUp(Keys.F5))
            {
                DatabaseManager.Instance.SaveGame(DungeonManager.Instance.CurrentRoom.Id, _player.Position, _player.Score);
                _hasSaveData = true;
                ShowMessage("Jogo salvo!");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F9) && _lastKeyboardState.IsKeyUp(Keys.F9))
            {
                LoadSavedGame();
                ShowMessage("Jogo carregado!");
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Save/Load error: " + ex.Message);
        }

        _camera.Follow(_player.Position, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        base.Update(gameTime);
    }

    // =========================================================================
    // DRAW
    // =========================================================================

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(15, 15, 25));

        int sw = SCREEN_WIDTH;
        int sh = SCREEN_HEIGHT;
        int cx = sw / 2;

        // =============================================
        // MENU PRINCIPAL
        // =============================================

        if (_gameState == GameState.MainMenu)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Background com gradiente escuro
            _spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, sw, sh), new Color(10, 10, 20));

            // Particulas decorativas (estrelas estaticas baseadas no timer)
            DrawMenuParticles();

            // Titulo com escala pulsante
            string title = "DUNGEON OF ALGORITHMS";
            Vector2 titleSize = _font.MeasureString(title);
            Vector2 titlePos = new Vector2(cx - titleSize.X / 2, 120);
            DrawShadowString(_font, title, titlePos, Color.Gold, 2);

            // Subtitulo
            string subtitle = "The Memory Leak Chronicle";
            Vector2 subtitleSize = _font.MeasureString(subtitle);
            DrawShadowString(_font, subtitle, new Vector2(cx - subtitleSize.X / 2, 165), new Color(100, 220, 100));

            // Linha decorativa superior
            int lineWidth = 400;
            _spriteBatch.Draw(_pixelTexture, new Rectangle(cx - lineWidth / 2, 200, lineWidth, 2), Color.Gold * 0.4f);
            _spriteBatch.Draw(_pixelTexture, new Rectangle(cx - lineWidth / 2, 203, lineWidth, 1), Color.Gold * 0.2f);

            // Instrucoes do jogo
            string[] instructions = {
                "Objetivo: Colete todas as moedas",
                "em todas as salas para desbloquear",
                "o bau do tesouro e vencer!"
            };
            for (int idx = 0; idx < instructions.Length; idx++)
            {
                Vector2 instrSize = _font.MeasureString(instructions[idx]);
                DrawShadowString(_font, instructions[idx],
                    new Vector2(cx - instrSize.X / 2, 218 + idx * 18),
                    Color.LightGray * 0.7f);
            }

            // Opcoes do menu
            int menuStartY = 300;
            int menuSpacing = 50;

            for (int i = 0; i < _menuOptions.Length; i++)
            {
                bool isSelected = (i == _menuSelectedIndex);
                bool isDisabled = (i == 1 && !_hasSaveData);

                Vector2 textSize = _font.MeasureString(_menuOptions[i]);
                Vector2 optionPos = new Vector2(cx - textSize.X / 2, menuStartY + (i * menuSpacing));

                if (isDisabled)
                {
                    DrawShadowString(_font, _menuOptions[i], optionPos, Color.Gray * 0.3f);
                    continue;
                }

                float pulse = isSelected ? (float)(0.8f + 0.2f * Math.Sin(_menuPulseTimer * 5)) : 0.5f;
                Color optionColor = isSelected ? Color.Gold * pulse : Color.White * 0.5f;

                if (isSelected)
                {
                    float arrowOffset = (float)(Math.Sin(_menuPulseTimer * 8) * 4);

                    // Caixa de destaque
                    int boxPadding = 12;
                    _spriteBatch.Draw(_pixelTexture,
                        new Rectangle((int)(optionPos.X - boxPadding - 20), (int)(optionPos.Y - 4),
                                      (int)(textSize.X + boxPadding * 2 + 40), (int)(textSize.Y + 8)),
                        Color.Gold * 0.08f);

                    // Borda da caixa
                    DrawHorizontalLine((int)(optionPos.X - boxPadding - 20), (int)(optionPos.Y - 4),
                        (int)(textSize.X + boxPadding * 2 + 40), Color.Gold * 0.3f);
                    DrawHorizontalLine((int)(optionPos.X - boxPadding - 20), (int)(optionPos.Y + textSize.Y + 3),
                        (int)(textSize.X + boxPadding * 2 + 40), Color.Gold * 0.3f);

                    // Setas animadas
                    DrawShadowString(_font, ">>", new Vector2(optionPos.X - 35 + arrowOffset, optionPos.Y), Color.Gold);
                    DrawShadowString(_font, "<<", new Vector2(optionPos.X + textSize.X + 10 - arrowOffset, optionPos.Y), Color.Gold);
                }

                DrawShadowString(_font, _menuOptions[i], optionPos, optionColor);
            }

            // Linha decorativa inferior
            _spriteBatch.Draw(_pixelTexture, new Rectangle(cx - lineWidth / 2, 520, lineWidth, 2), Color.Gold * 0.4f);

            // Dicas de controle
            string hint1 = "[W/S] Navegar   [ENTER] Selecionar   [F11] Tela Cheia";
            Vector2 hint1Size = _font.MeasureString(hint1);
            DrawShadowString(_font, hint1, new Vector2(cx - hint1Size.X / 2, 540), Color.Gray * 0.6f);

            string hint2 = "[WASD] Mover   [P/ESC] Pausar   [F5] Salvar   [F9] Carregar";
            Vector2 hint2Size = _font.MeasureString(hint2);
            DrawShadowString(_font, hint2, new Vector2(cx - hint2Size.X / 2, 560), Color.Gray * 0.5f);

            // Versao
            string version = "v2.0 - Dungeon of Algorithms";
            Vector2 versionSize = _font.MeasureString(version);
            DrawShadowString(_font, version, new Vector2(cx - versionSize.X / 2, sh - 35), Color.Gray * 0.4f);

            _spriteBatch.End();
            base.Draw(gameTime);
            return;
        }

        // =============================================
        // CREDITOS
        // =============================================

        if (_gameState == GameState.Credits)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, sw, sh), new Color(10, 10, 20));

            string[] creditLines = {
                "DUNGEON OF ALGORITHMS",
                "",
                "The Memory Leak Chronicle",
                "",
                "------------------------",
                "",
                "Desenvolvido por:",
                "Lucas Lopes",
                "",
                "------------------------",
                "",
                "Engine: MonoGame Framework",
                "Linguagem: C# (.NET 9.0)",
                "",
                "------------------------",
                "",
                "Design Patterns Utilizados:",
                "",
                "Singleton - GameManager",
                "Factory - ItemFactory, EnemyFactory",
                "Strategy - Enemy Behaviors",
                "Observer - HUD System",
                "Graph - Dungeon Structure",
                "",
                "------------------------",
                "",
                "Estruturas de Dados:",
                "",
                "Grafo - Conexao entre salas",
                "Dicionario - Texturas e salas",
                "Lista - Itens e inimigos",
                "Array 2D - Tilemap",
                "",
                "------------------------",
                "",
                "Obrigado por jogar!",
                "",
                "[ESC] Voltar ao Menu"
            };

            float y = sh - _creditsScrollY;
            for (int i = 0; i < creditLines.Length; i++)
            {
                float lineY = y + i * 30;
                if (lineY < -30 || lineY > sh + 30) continue;

                Vector2 lineSize = _font.MeasureString(creditLines[i].Length > 0 ? creditLines[i] : " ");
                Color lineColor = i == 0 ? Color.Gold : (creditLines[i].StartsWith("-") ? Color.Gold * 0.5f : Color.White * 0.8f);

                DrawShadowString(_font, creditLines[i], new Vector2(cx - lineSize.X / 2, lineY), lineColor);
            }

            // Reset scroll quando chega ao fim
            if (y + creditLines.Length * 30 < 0)
                _creditsScrollY = 0f;

            _spriteBatch.End();
            base.Draw(gameTime);
            return;
        }

        // =============================================
        // WORLD SPACE - Gameplay
        // =============================================

        _spriteBatch.Begin(transformMatrix: _camera.Transform, samplerState: SamplerState.PointClamp);
        DungeonManager.Instance.CurrentRoom.Draw(_spriteBatch);
        _player.Draw(_spriteBatch);
        _spriteBatch.End();

        // =============================================
        // SCREEN SPACE - UI
        // =============================================

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Vinheta
        if (_vignetteTexture != null)
            _spriteBatch.Draw(_vignetteTexture, Vector2.Zero, Color.White);

        // =============================================
        // GAME OVER
        // =============================================

        if (_gameState == GameState.GameOver)
        {
            _spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, sw, sh), Color.Black * 0.75f);

            float fadeIn = Math.Min(_endScreenTimer * 2f, 1f);

            string goTitle = "KERNEL PANIC";
            Vector2 goTitleSize = _font.MeasureString(goTitle);
            float glitch = (float)(Math.Sin(_endScreenTimer * 15) * 2);
            DrawShadowString(_font, goTitle, new Vector2(cx - goTitleSize.X / 2 + glitch, 200), Color.Red * fadeIn, 2);

            string goSub = "SYSTEM CRASH - MEMORY CORRUPTED";
            Vector2 goSubSize = _font.MeasureString(goSub);
            DrawShadowString(_font, goSub, new Vector2(cx - goSubSize.X / 2, 250), Color.OrangeRed * fadeIn * 0.8f);

            _spriteBatch.Draw(_pixelTexture, new Rectangle(cx - 200, 285, 400, 2), Color.Red * fadeIn * 0.5f);

            if (_endScreenTimer > 0.5f)
            {
                float fadeIn2 = Math.Min((_endScreenTimer - 0.5f) * 2f, 1f);

                int collected = DungeonManager.Instance.CollectedCoins;
                int total = DungeonManager.Instance.TotalCoins;
                string scoreText = $"Moedas: {collected}/{total}  |  Score: {_player.Score}";
                Vector2 scoreSize = _font.MeasureString(scoreText);
                DrawShadowString(_font, scoreText, new Vector2(cx - scoreSize.X / 2, 310), Color.White * fadeIn2);

                string hint1go = "[R] Reboot System";
                Vector2 h1Size = _font.MeasureString(hint1go);
                DrawShadowString(_font, hint1go, new Vector2(cx - h1Size.X / 2, 380), Color.Gray * fadeIn2 * 0.8f);

                string hint2go = "[M] Return to BIOS (Menu)";
                Vector2 h2Size = _font.MeasureString(hint2go);
                DrawShadowString(_font, hint2go, new Vector2(cx - h2Size.X / 2, 410), Color.Gray * fadeIn2 * 0.8f);
            }
        }

        // =============================================
        // VITORIA
        // =============================================

        else if (_gameState == GameState.Victory)
        {
            _spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, sw, sh), Color.Black * 0.7f);

            float fadeIn = Math.Min(_endScreenTimer * 2f, 1f);

            string vicTitle = "SYSTEM RESTORED";
            Vector2 vicTitleSize = _font.MeasureString(vicTitle);
            DrawShadowString(_font, vicTitle, new Vector2(cx - vicTitleSize.X / 2, 180), Color.Gold * fadeIn, 2);

            string vicSub = "All memory leaks have been patched!";
            Vector2 vicSubSize = _font.MeasureString(vicSub);
            DrawShadowString(_font, vicSub, new Vector2(cx - vicSubSize.X / 2, 230), Color.LightGreen * fadeIn);

            _spriteBatch.Draw(_pixelTexture, new Rectangle(cx - 200, 265, 400, 2), Color.Gold * fadeIn * 0.5f);

            if (_endScreenTimer > 0.5f)
            {
                float fadeIn2 = Math.Min((_endScreenTimer - 0.5f) * 2f, 1f);

                string scoreText = $"Final Score: {_player.Score}";
                Vector2 scoreSize = _font.MeasureString(scoreText);
                DrawShadowString(_font, scoreText, new Vector2(cx - scoreSize.X / 2, 290), Color.White * fadeIn2);

                // Estrelas baseadas no score
                int stars = _player.Score >= 15 ? 3 : (_player.Score >= 10 ? 2 : 1);
                string starsText = new string('*', stars) + new string('.', 3 - stars);
                Vector2 starsSize = _font.MeasureString(starsText);
                DrawShadowString(_font, starsText, new Vector2(cx - starsSize.X / 2, 330), Color.Gold * fadeIn2);

                string hint1vic = "[R] Play Again";
                Vector2 h1Size = _font.MeasureString(hint1vic);
                DrawShadowString(_font, hint1vic, new Vector2(cx - h1Size.X / 2, 390), Color.Gray * fadeIn2 * 0.8f);

                string hint2vic = "[M] Return to Menu";
                Vector2 h2Size = _font.MeasureString(hint2vic);
                DrawShadowString(_font, hint2vic, new Vector2(cx - h2Size.X / 2, 420), Color.Gray * fadeIn2 * 0.8f);
            }
        }

        // =============================================
        // PAUSA
        // =============================================

        else if (_gameState == GameState.Paused)
        {
            _spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, sw, sh), Color.Black * 0.7f);

            string pauseTitle = "PAUSED";
            Vector2 pauseTitleSize = _font.MeasureString(pauseTitle);
            DrawShadowString(_font, pauseTitle, new Vector2(cx - pauseTitleSize.X / 2, 220), Color.White, 2);

            _spriteBatch.Draw(_pixelTexture, new Rectangle(cx - 150, 260, 300, 2), Color.White * 0.3f);

            int pauseStartY = 300;
            int pauseSpacing = 45;

            for (int i = 0; i < _pauseOptions.Length; i++)
            {
                bool isSelected = (i == _pauseSelectedIndex);
                Vector2 textSize = _font.MeasureString(_pauseOptions[i]);
                Vector2 optionPos = new Vector2(cx - textSize.X / 2, pauseStartY + (i * pauseSpacing));

                float pulse = isSelected ? (float)(0.8f + 0.2f * Math.Sin(_menuPulseTimer * 5)) : 0.5f;
                Color optionColor = isSelected ? Color.Gold * pulse : Color.White * 0.5f;

                if (isSelected)
                {
                    int boxPadding = 10;
                    _spriteBatch.Draw(_pixelTexture,
                        new Rectangle((int)(optionPos.X - boxPadding), (int)(optionPos.Y - 2),
                                      (int)(textSize.X + boxPadding * 2), (int)(textSize.Y + 4)),
                        Color.Gold * 0.1f);
                }

                DrawShadowString(_font, _pauseOptions[i], optionPos, optionColor);
            }

            string pauseHint = "[P] ou [ESC] para voltar";
            Vector2 pauseHintSize = _font.MeasureString(pauseHint);
            DrawShadowString(_font, pauseHint, new Vector2(cx - pauseHintSize.X / 2, 440), Color.Gray * 0.5f);
        }

        // =============================================
        // HUD DURANTE GAMEPLAY
        // =============================================

        else if (_gameState == GameState.Playing)
        {
            _hud.Draw(_spriteBatch, _player, _pixelTexture, DungeonManager.Instance.CurrentRoom);

            // Mensagem de tutorial/feedback (canto inferior central)
            if (_messageTimer > 0 && _messageAlpha > 0)
            {
                Vector2 msgSize = _font.MeasureString(_currentMessage);
                float msgX = cx - msgSize.X / 2;
                float msgY = sh - 80;

                // Background da mensagem
                _spriteBatch.Draw(_pixelTexture,
                    new Rectangle((int)(msgX - 10), (int)(msgY - 5),
                                  (int)(msgSize.X + 20), (int)(msgSize.Y + 10)),
                    Color.Black * 0.7f * _messageAlpha);

                DrawShadowString(_font, _currentMessage,
                    new Vector2(msgX, msgY), Color.White * _messageAlpha);
            }

            // Texto de transicao de sala (centro da tela, fade)
            if (_roomTransitionTimer > 0)
            {
                float rtAlpha = _roomTransitionTimer > 1.5f ? 1f : _roomTransitionTimer / 1.5f;
                Vector2 rtSize = _font.MeasureString(_roomTransitionText);

                _spriteBatch.Draw(_pixelTexture,
                    new Rectangle((int)(cx - rtSize.X / 2 - 15), (int)(sh / 3 - 5),
                                  (int)(rtSize.X + 30), (int)(rtSize.Y + 10)),
                    Color.Black * 0.6f * rtAlpha);

                DrawShadowString(_font, _roomTransitionText,
                    new Vector2(cx - rtSize.X / 2, sh / 3), Color.Gold * rtAlpha, 2);
            }

            // Intro overlay - objetivo do jogo (aparece ao iniciar)
            if (_showIntro && _introTimer > 0)
            {
                float introAlpha = _introTimer > 1f ? 1f : _introTimer;

                // Fundo escuro semi-transparente
                _spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, sw, sh), Color.Black * 0.6f * introAlpha);

                // Caixa central
                int boxW = 500;
                int boxH = 200;
                int boxX = cx - boxW / 2;
                int boxY = sh / 2 - boxH / 2 - 30;

                _spriteBatch.Draw(_pixelTexture, new Rectangle(boxX, boxY, boxW, boxH), Color.Black * 0.85f * introAlpha);
                // Bordas
                _spriteBatch.Draw(_pixelTexture, new Rectangle(boxX, boxY, boxW, 2), Color.Gold * 0.6f * introAlpha);
                _spriteBatch.Draw(_pixelTexture, new Rectangle(boxX, boxY + boxH - 2, boxW, 2), Color.Gold * 0.6f * introAlpha);
                _spriteBatch.Draw(_pixelTexture, new Rectangle(boxX, boxY, 2, boxH), Color.Gold * 0.6f * introAlpha);
                _spriteBatch.Draw(_pixelTexture, new Rectangle(boxX + boxW - 2, boxY, 2, boxH), Color.Gold * 0.6f * introAlpha);

                // Titulo
                string introTitle = "MISSAO";
                Vector2 itSize = _font.MeasureString(introTitle);
                DrawShadowString(_font, introTitle, new Vector2(cx - itSize.X / 2, boxY + 15), Color.Gold * introAlpha, 2);

                // Linha separadora
                _spriteBatch.Draw(_pixelTexture, new Rectangle(boxX + 20, boxY + 45, boxW - 40, 1), Color.Gold * 0.4f * introAlpha);

                // Instrucoes
                string[] introLines = {
                    "Explore todas as 3 salas da dungeon.",
                    "Colete todas as moedas espalhadas.",
                    "Encontre e abra o bau do tesouro!",
                    "",
                    "O bau so abre quando TODAS as",
                    "moedas forem coletadas."
                };

                for (int i = 0; i < introLines.Length; i++)
                {
                    if (introLines[i].Length == 0) continue;
                    Vector2 lineSize = _font.MeasureString(introLines[i]);
                    Color lineColor = i <= 2 ? Color.White : Color.Yellow;
                    DrawShadowString(_font, introLines[i],
                        new Vector2(cx - lineSize.X / 2, boxY + 55 + i * 20), lineColor * introAlpha * 0.9f);
                }

                // Controles
                string ctrlText = "[WASD] Mover   [P/ESC] Pausar   [F5] Salvar";
                Vector2 ctrlSize = _font.MeasureString(ctrlText);
                DrawShadowString(_font, ctrlText,
                    new Vector2(cx - ctrlSize.X / 2, boxY + boxH + 15), Color.Gray * 0.6f * introAlpha);
            }

            // Indicador de bau trancado/destrancado (canto inferior direito)
            if (!DungeonManager.Instance.AllCoinsCollected)
            {
                string lockText = "Bau: TRANCADO";
                Vector2 lockSize = _font.MeasureString(lockText);
                float lockX = sw - lockSize.X - 15;
                float lockY = sh - 30;
                _spriteBatch.Draw(_pixelTexture,
                    new Rectangle((int)(lockX - 5), (int)(lockY - 3),
                                  (int)(lockSize.X + 10), (int)(lockSize.Y + 6)),
                    Color.Black * 0.5f);
                DrawShadowString(_font, lockText, new Vector2(lockX, lockY), Color.Red * 0.7f);
            }
            else
            {
                string unlockText = "Bau: DESBLOQUEADO!";
                Vector2 unlockSize = _font.MeasureString(unlockText);
                float unlockX = sw - unlockSize.X - 15;
                float unlockY = sh - 30;
                _spriteBatch.Draw(_pixelTexture,
                    new Rectangle((int)(unlockX - 5), (int)(unlockY - 3),
                                  (int)(unlockSize.X + 10), (int)(unlockSize.Y + 6)),
                    Color.Black * 0.5f);
                DrawShadowString(_font, unlockText, new Vector2(unlockX, unlockY), Color.LimeGreen * 0.9f);
            }
        }

        _spriteBatch.End();

        // Fade overlay
        if (_fadeAlpha > 0)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, sw, sh), Color.White * _fadeAlpha);
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    private void DrawShadowString(SpriteFont font, string text, Vector2 position, Color color, int shadowOffset = 1)
    {
        _spriteBatch.DrawString(font, text, position + new Vector2(shadowOffset, shadowOffset), Color.Black * 0.8f);
        _spriteBatch.DrawString(font, text, position, color);
    }

    private void DrawHorizontalLine(int x, int y, int width, Color color)
    {
        _spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, width, 1), color);
    }

    private void DrawMenuParticles()
    {
        int[] starX = { 100, 250, 400, 550, 700, 850, 1000, 1150, 180, 330, 480, 630, 780, 930, 1080, 60, 210, 360, 510, 660 };
        int[] starY = { 50, 120, 80, 200, 150, 90, 170, 60, 250, 180, 100, 220, 140, 70, 190, 300, 350, 400, 450, 500 };

        for (int i = 0; i < starX.Length; i++)
        {
            float twinkle = (float)(0.2f + 0.3f * Math.Sin(_menuPulseTimer * (1.5f + i * 0.3f) + i * 1.7f));
            _spriteBatch.Draw(_pixelTexture, new Rectangle(starX[i], starY[i], 2, 2), Color.White * twinkle);
        }
    }

    private void ShowMessage(string message)
    {
        _currentMessage = message;
        _messageTimer = MESSAGE_DURATION;
        _messageAlpha = 1f;
    }

    private void ShowRoomTransition(int roomId)
    {
        string[] roomNames = { "", "The Stack", "The Heap", "Kernel Panic" };
        string name = roomId >= 1 && roomId <= 3 ? roomNames[roomId] : $"Room {roomId}";
        _roomTransitionText = $"-- {name} --";
        _roomTransitionTimer = ROOM_TRANSITION_DURATION;
    }

    private void StartBGM()
    {
        if (_ambientMusic != null)
        {
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.7f;
                MediaPlayer.Play(_ambientMusic);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("[Music] BGM error: " + ex.Message);
            }
        }
    }

    private void ResetGame()
    {
        _player = new Player(_playerTextures, new Vector2(50, 80));
        DungeonManager.Instance.ChangeRoom(1);
        _endScreenTimer = 0f;
        _allCoinsNotified = false;
        _messageTimer = 0f;
        _roomTransitionTimer = 0f;

        // Reativa todos os itens
        foreach (var room in DungeonManager.Instance.Rooms.Values)
        {
            foreach (var item in room.Items)
            {
                item.IsActive = true;
                if (item is ChestItem chestItem)
                    chestItem.IsUnlocked = false;
            }
        }

        AudioManager.Instance.StopAmbientMusic();
        StartBGM();
    }

    private void LoadSavedGame()
    {
        try
        {
            var data = DatabaseManager.Instance.LoadGame();
            if (data != null)
            {
                DungeonManager.Instance.ChangeRoom(data.Value.Level);
                _player.SetPosition(data.Value.Position);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Load error: " + ex.Message);
        }
    }
}
