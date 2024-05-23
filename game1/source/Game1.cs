using Apos.Gui;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled.Serialization;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace game1.source
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D renderTarget;
        private FontSystem fontSystem;


        private static float screenWidth;
        private static float screenHeight;
       
        #region UI
        IMGUI ui;
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

        public static float ScreenWidth { get => screenWidth; set => screenWidth = value; }
        public static float ScreenHeight { get => screenHeight; set => screenHeight = value; }
        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 920;
            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.ApplyChanges();

            ScreenHeight = _graphics.PreferredBackBufferHeight;
            ScreenWidth = _graphics.PreferredBackBufferWidth;

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
                Content.Load<Texture2D>("idle"),
                Content.Load<Texture2D>("run"),
                Content.Load<Texture2D>("jump"),
                Content.Load<Texture2D>("jump"));
            #endregion

            #region Tilemap
            map = new TmxMap("Content\\lvl1.tmx");
            tileset = Content.Load<Texture2D>("for_map\\" + map.Tilesets[0].Name.ToString());
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
                    collisionRectangles.Add(new Rectangle((int)obj.X, (int)obj.Y,(int) obj.Width, (int)obj.Height));

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

            alien = new Enemy(Content.Load<Texture2D>("wake"), enemyPath[0]);
            enemies.Add(alien);
            //alien = new Enemy(Content.Load<Texture2D>("wake"), enemyPath[1]);
            //enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("wake"), enemyPath[2]);
            enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("wake"), enemyPath[3]);
            enemies.Add(alien);
            //alien = new Enemy(Content.Load<Texture2D>("wake"), enemyPath[4]);
            //enemies.Add(alien);
            alien = new Enemy(Content.Load<Texture2D>("wake"), enemyPath[5]);
            enemies.Add(alien);
            #endregion

            renderTarget = new RenderTarget2D(GraphicsDevice, 1080, 920);

            #region Camera
            camera = new Camera();
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region UI
            GuiHelper.UpdateSetup(gameTime);
            ui.UpdateStart(gameTime);

            MenuPanel.Push();

            Label.Put("Points:");

            ui.UpdateEnd(gameTime);
            GuiHelper.UpdateCleanup();
            #endregion

            #region Managers
            if (_gameManager.IsGameEnded(_player.hitBox))
                Console.WriteLine("The end");
            if (gameIsOver)
                Console.WriteLine("Game over");
            #endregion

            #region Enemy
            foreach (var enemy in enemies)
            {
                enemy.Update();
                gameIsOver = gameIsOver || enemy.HasHit(_player.hitBox);
            }
            #endregion

            #region Player Collisions

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
            #endregion

            #region Camera Update
            Rectangle target = new Rectangle((int)_player.position.X, (int)_player.position.Y, 16, 16);
            transformMatrix = camera.Follow(target);
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

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp); //делает изображение четким
            _spriteBatch.Draw(renderTarget, new Vector2(0,0), null, Color.White, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}