using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TiledSharp;

namespace game1.source
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static float screenWidth;
        public static float screenHeight;

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
        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.PreferredBackBufferWidth = 500;
            _graphics.ApplyChanges();

            screenHeight = _graphics.PreferredBackBufferHeight;
            screenWidth = _graphics.PreferredBackBufferWidth;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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

            tilemapManager = new TilemapManager(map, tileset, tilesetTileWidth, tileWidth, tileHeight);
            #endregion

            #region Collisions 
            collisionRectangles = new List<Rectangle>();

            foreach (var obj in map.ObjectGroups["Collisions"].Objects)
            {
                if (obj.Name == "")
                {
                    collisionRectangles.Add(new Rectangle((int)obj.X, (int)obj.Y,(int) obj.Width, (int)obj.Height));
                }
                if (obj.Name == "Start")
                {
                    startRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                }
                if (obj.Name == "End")
                {
                    endRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                }
            }
            #endregion

            #region Enemy
            enemyPath = new List<Rectangle>();
            foreach (var obj in map.ObjectGroups["EnemyPath"].Objects)
            {
                enemyPath.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));
            }
            enemies = new List<Enemy>();
            alien = new Enemy(
                Content.Load<Texture2D>("wake"),
                enemyPath[0]
                ); 
            enemies.Add( alien );
            #endregion

            #region Camera
            camera = new Camera();
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region Enemy
            foreach(var enemy in enemies)
            {
                enemy.Update();
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: transformMatrix);

            tilemapManager.Draw(_spriteBatch);
            _player.Draw(_spriteBatch, gameTime);
            #region Enemy
            foreach (var enemy in enemies)
            {
                enemy.Draw(_spriteBatch, gameTime);
            }
            #endregion

            _spriteBatch.End();
          
            base.Draw(gameTime);
        }
    }
}