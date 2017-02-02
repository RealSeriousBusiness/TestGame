using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TestGame
{
    /// <summary>
    /// This is the main type for your game. 
    /// TODO:
    /// -cleanup
    /// -Rebuild game surface if window is resized
    /// -add messages when game ends, allow to restart
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager g;
        SpriteBatch sb;


        Texture2D sprBackground, sprSegment, sprFood;

        Player snake;
        
        bool sizeHasChanged = false;
        bool gameRunning = true;

        public Game()
        {
            g = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Window.AllowUserResizing = true;
            this.IsMouseVisible = true;

            this.Window.ClientSizeChanged += Window_ClientSizeChanged;

            //this.IsFixedTimeStep = false;
            //this.graphics.SynchronizeWithVerticalRetrace = false;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(20); //50 fps
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e) //gets called while resizing
        {
            sizeHasChanged = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameSurface.gridWidth = 31;
            GameSurface.gridHeight = 31;
            var size = GraphicsDevice.PresentationParameters;

            GameSurface.Build(size.BackBufferWidth, size.BackBufferHeight);

            float x = (float)GameSurface.gridWidth / 2.0f, y = (float)GameSurface.gridHeight / 2.0f; //init pos

            snake = new Player((int)x, (int)y, 5);

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            sb = new SpriteBatch(GraphicsDevice);

            CreatePlainTexture(ref sprBackground, Color.CornflowerBlue);
            CreatePlainTexture(ref sprSegment, Color.GreenYellow);
            CreatePlainTexture(ref sprFood, Color.Red);
        }

        void CreatePlainTexture(ref Texture2D target, Color color)
        {
            target = new Texture2D(this.GraphicsDevice, GameSurface.squareSize, GameSurface.squareSize);

            Color[] data = new Color[target.Width * target.Height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;

            target.SetData<Color>(data);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) //gametime provides how long a frame needed to render
        {
            if (!gameRunning)
                return;

            if ((snake.state & Player.State.MOVING_MASK) == Player.State.MOVING) //only update when direction is specified
            {
                float movedUnits = (float)(gameTime.ElapsedGameTime.TotalSeconds * snake.Speed);
                snake.partStep += movedUnits;
                snake.Update(); //movement
            }
            else if(snake.state == Player.State.CRASHED)
            {
                Console.WriteLine("you crashed with {0} Points", snake.collected);
                gameRunning = false;
            }
            else if (snake.state == Player.State.CLEARED)
            {
                Console.WriteLine("you won, gz.");
                gameRunning = false;
            }


            var s = Keyboard.GetState();

            if (s.IsKeyDown(Keys.Up))
                snake.state = Player.State.UP;
            else if (s.IsKeyDown(Keys.Down))
                snake.state = Player.State.DOWN;
            else if (s.IsKeyDown(Keys.Right))
                snake.state = Player.State.RIGHT;
            else if (s.IsKeyDown(Keys.Left))
                snake.state = Player.State.LEFT;
            else if (s.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        protected override void Draw(GameTime gameTime)
        {
            Window.Title = "Draw Time: " + gameTime.ElapsedGameTime.TotalMilliseconds;

            GraphicsDevice.Clear(Color.Black);

            sb.Begin();

            sb.Draw(sprBackground, GameSurface.playArea, Color.White);

            var segs = snake.segs;
            for (int i = 0; i < segs.Count; i++) //draw segments
                sb.Draw(sprSegment, segs[i].screenPosition);

            sb.Draw(sprFood, GameSurface.foodPos);

            sb.End();

            base.Draw(gameTime);
        }
    }
}
