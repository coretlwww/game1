using Apos.Gui;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TiledSharp;

namespace game1
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D renderTarget;

        #region UI
        IMGUI ui;
        bool isTextShown = false;
        private FontSystem fontSystem;
        #endregion

        #region Manager
        private GameManager _gameManager;
        private bool gameIsOver = false;
        #endregion

        #region Player
        private Player _player;
        #endregion

        #region Enemy
        private Enemy alien;
        private List<Enemy> enemies;
        private List<Rectangle> enemyPath;
        #endregion

        #region Oxygen
        Texture2D oxygenTankTexture;
        List<Vector2> oxygenTanksPositions;
        int score = 1;
        readonly float scoreDecreaseInterval = 10f;
        float timeSinceLastDecrease = 0f;
        #endregion

        #region Tilemap
        private TmxMap map;
        private TilemapManager tilemapManager;
        private Texture2D tileset;
        private List<Rectangle> collisionRectangles;
        private Rectangle startRect;
        private Rectangle endRect;
        #endregion

        #region Camera
        private Camera camera;
        private Matrix transformMatrix;

        public static float ScreenWidth => 1080f;
        public static float ScreenHeight => 990f;
        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = (int)ScreenHeight;
            _graphics.PreferredBackBufferWidth = (int)ScreenWidth;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region UI
            fontSystem = new FontSystem();
            fontSystem.AddFont(TitleContainer.OpenStream($"{Content.RootDirectory}/Jersey10-Regular.ttf"));

            GuiHelper.Setup(this, fontSystem);

            ui = new IMGUI();
            #endregion

            #region Player
            _player = new Player(
                new Vector2(startRect.X, startRect.Y),
                Content.Load<Texture2D>("Sprites\\idle"),
                Content.Load<Texture2D>("Sprites\\run"),
                Content.Load<Texture2D>("Sprites\\jump"),
                Content.Load<Texture2D>("Sprites\\jump"));
            #endregion

            #region Tilemap
            map = new TmxMap("Content\\Map\\mainLevel.tmx");
            tileset = Content.Load<Texture2D>("Map\\" + map.Tilesets[0].Name.ToString());
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;
            int tilesetTileWidth = tileset.Width / tileWidth;

            tilemapManager = new TilemapManager(map, tileset, tilesetTileWidth, tileWidth, tileHeight, GraphicsDevice, _spriteBatch);
            #endregion

            #region Collisions 
            collisionRectangles = new List<Rectangle>();

            foreach (var obj in map.ObjectGroups["Collisions"].Objects)
            {
                if (obj.Name == "")
                    collisionRectangles.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));

                if (obj.Name == "Start")
                    startRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);

                if (obj.Name == "End")
                    endRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
            }
            #endregion

            _gameManager = new GameManager(endRect);

            #region Enemy
            enemyPath = new List<Rectangle>();

            foreach (var obj in map.ObjectGroups["EnemyPath"].Objects)
                enemyPath.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));

            enemies = new List<Enemy>();

            //alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[0]);
            //enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[1]);
            enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[2]);
            enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[3]);
            enemies.Add(alien);
            //alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[4]);
            //enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[5]);
            enemies.Add(alien);
            //alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[6]);
            //enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("Sprites\\wake"), enemyPath[7]);
            enemies.Add(alien);
            #endregion  

            #region Camera
            camera = new Camera();
            #endregion

            renderTarget = new RenderTarget2D(GraphicsDevice, 1080, 990);

            #region Oxygen
            oxygenTanksPositions = new List<Vector2>();
            oxygenTankTexture = Content.Load<Texture2D>("Sprites\\oxygen");
            foreach (var obj in map.ObjectGroups["Oxygen"].Objects)
            {
                Vector2 position = new((float)obj.X, (float)obj.Y);
                oxygenTanksPositions.Add(position);
            }
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            #region Enemy
            foreach (var enemy in enemies)
            {
                enemy.Update();
                gameIsOver = gameIsOver || enemy.HasHit(_player.hitBox);
            }
            #endregion

            #region Camera Update
            Rectangle target = new((int)_player.position.X, (int)_player.position.Y, 16, 16);
            transformMatrix = camera.Follow(target);
            #endregion

            #region UI & Player Collision
            GuiHelper.UpdateSetup(gameTime);
            ui.UpdateStart(gameTime);
            
            Label.Put($"Oxygen: {score}", fontSize: 50, Color.DarkSlateBlue);

            MenuPanel.Push().XY = new Vector2();
            if (_gameManager.IsGameEnded(_player.hitBox))
            {
                Label.Put("You Won!", fontSize: 80, Color.DarkSlateBlue);
                isTextShown = true;
            }

            else if (gameIsOver || score == 0)
            {
                Label.Put("Game over", fontSize: 80, Color.DarkSlateBlue);
                isTextShown = true;
            }
            MenuPanel.Pop();

            if (!isTextShown)
            {
                var initialPosition = _player.position;

                _player.Update();

                //y-axis
                foreach (var rect in collisionRectangles)
                {
                    if (!_player.isJumping)
                        _player.isFalling = true;

                    if (rect.Intersects(_player.playerFallRect))
                    {
                        _player.isFalling = false;
                        break;
                    }
                }

                //x-axis
                foreach (var rect in collisionRectangles)
                {
                    if (rect.Intersects(_player.hitBox))
                    {
                        _player.position.X = initialPosition.X;
                        _player.velocity.X = initialPosition.X;
                        break;
                    }
                }
            }

            ui.UpdateEnd(gameTime);
            GuiHelper.UpdateCleanup();
            #endregion

            #region Collecting Oxygen
            static bool IsPlayerPickingOxygenTank(Vector2 playerPosition, Vector2 tankPosition)
            {
                float distance = Vector2.Distance(playerPosition, tankPosition);
                return distance < 32; // Примерный радиус подбора
            }

            timeSinceLastDecrease += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastDecrease >= scoreDecreaseInterval)
            {
                if (score > 0)
                    score--;

                timeSinceLastDecrease = 0f;
            }

            foreach (var position in oxygenTanksPositions.ToList())
            {
                if (IsPlayerPickingOxygenTank(_player.position, position))
                {
                    oxygenTanksPositions.Remove(position);
                    score++;
                }
            }

            if (score < 0)
                Exit();
            #endregion 

            DrawLevel(gameTime);
            base.Update(gameTime);
        }

        public void DrawLevel(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: transformMatrix);

            tilemapManager.Draw(_spriteBatch);

            _player.Draw(_spriteBatch, gameTime);

            #region Enemy
            foreach (var enemy in enemies)
                enemy.Draw(_spriteBatch, gameTime);
            #endregion

            #region Oxygen
            foreach (var position in oxygenTanksPositions)
                _spriteBatch.Draw(oxygenTankTexture, new Rectangle((int)position.X, (int)position.Y, 16, 16), Color.White);

            #endregion

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(renderTarget, new Vector2(0, 0), null, Color.White, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            _spriteBatch.End();
            ui.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}