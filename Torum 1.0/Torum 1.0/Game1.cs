#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input.Touch;
using TorumPlayer;
using Torum_1._0;

#endregion

namespace Torum_1._0
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        //Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

        // A movement speed for the player
        float playerMoveSpeed;

        // Image used to display the static background
        Texture2D mainBackground;
        Rectangle rectBackground;
        ParallaxingBackground bgLayer1, bgLayer2, mbg;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        Explosions xplosion;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        TimeSpan annihilationTime;
        TimeSpan annihilationTimeEnd;

        // A random number generator
        Random random;

        float scale = 1f;

        
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player();        
    
            //Background
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            mbg = new ParallaxingBackground();

            playerMoveSpeed = 5.0f;

            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            // Initialize the enemies list
            enemies = new List<Enemy>();
            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;
            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(0.5f);
            // Initialize our random number generator
            annihilationTime = TimeSpan.FromSeconds(10.0f);
            annihilationTimeEnd = TimeSpan.FromSeconds(15.0f);
            random = new Random();
            xplosion = new Explosions();
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 768;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here// Load the player resources
            
            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
            GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerPosition);

            // Load the parallaxing background
            mainBackground = Content.Load<Texture2D>("Graphics/mainbackground");
            mbg.Initialize(Content, "Graphics/mainbackground", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -5);
            enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation");
            //mainBackground = Content.Load<Texture2D>("Graphics/mainbackground");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            //Update the player
            UpdatePlayer(gameTime);

            // Update the parallaxing background
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            // Update the enemies
            UpdateEnemies(gameTime);

            // Update the collision
            UpdateCollision(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

          
            // Get Thumbstick Controls
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }

            if(currentKeyboardState.IsKeyDown(Keys.F ))
            {
                playerMoveSpeed++;
                
            }
            if (currentKeyboardState.IsKeyDown(Keys.G))
            {
                playerMoveSpeed--;
            }

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height / 2);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Start drawing
            spriteBatch.Begin();

            //Draw the Main Background Texture
            spriteBatch.Draw(mainBackground, rectBackground, Color.DarkMagenta);

            // Draw the moving background
            mbg.Draw(spriteBatch);
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            // Draw the Player
            player.Draw(spriteBatch);

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();
            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2,
            random.Next(100, GraphicsDevice.Viewport.Height - 100));
            // Create an enemy
            Enemy enemy = new Enemy();
            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);
            // Add the enemy to the active enemies list
            enemies.Add(enemy);

        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if ((gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) && (gameTime.TotalGameTime < annihilationTime))
            {
                previousSpawnTime = gameTime.TotalGameTime;
                // Add an Enemy
                AddEnemy();
            }
            else if ((gameTime.TotalGameTime > annihilationTime) && gameTime.TotalGameTime < annihilationTimeEnd)
            {
                if (gameTime.TotalGameTime - previousSpawnTime > (enemySpawnTime - previousSpawnTime) + enemySpawnTime)
                {
                    previousSpawnTime = gameTime.TotalGameTime;
                    
                    // Add an Enemy
                    AddEnemy();
                }

            }
            else if ((gameTime.TotalGameTime > annihilationTime) && gameTime.TotalGameTime > annihilationTimeEnd)
            {
                if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
                {
                    previousSpawnTime = gameTime.TotalGameTime;
                    // Add an Enemy
                    AddEnemy();
                }
            }
            else if ((gameTime.TotalGameTime > annihilationTime) && (gameTime.TotalGameTime < annihilationTime + annihilationTimeEnd))
            {
                if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime - previousSpawnTime)
                {
                    previousSpawnTime = gameTime.TotalGameTime;
                    // Add an Enemy
                    AddEnemy();
                }

            }
            // Update the Enemies
            for (int i = enemies.Count-1; i >= 0; i--)
            {   
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision(GameTime gametime)
	    {
		    // Use the Rectangle’s built-in intersect function to
		    // determine if two objects are overlapping
		    Rectangle rectangle1;
		    Rectangle rectangle2;
		    // Only create the rectangle once for the player
		    rectangle1 = new Rectangle((int)player.Position.X,
		    (int)player.Position.Y,
		    player.Width,
		    player.Height);
		    // Do the collision between the player and the enemies
		    for (int i = 0; i <enemies.Count; i++)
		    {
			    rectangle2 = new Rectangle((int)enemies[i].Position.X,
			    (int)enemies[i].Position.Y,
			    enemies[i].Width,
			    enemies[i].Height);
			    // Determine if the two objects collided with each
			    // other

			    if (rectangle1.Intersects(rectangle2))
			    {
				    // Subtract the health from the player based on
				    // the enemy damage
                    
				    player.Health -= enemies[i].Damage;
				    // Since the enemy collided with the player
				    // destroy it
                    for (int g = player.Damage; g < 200 && enemies[i].Active ; g += 1)
                    {
                        enemies[i].Health -= player.Damage;
                        playerMoveSpeed += .0002f;
                        if (gametime.TotalGameTime > annihilationTimeEnd)
                        {
                            playerMoveSpeed += .001f;
                        }
                        else if (gametime.TotalGameTime < annihilationTimeEnd)
                        {
                            playerMoveSpeed += .0002f;
                        }
                    }
				    // If the player health is less than zero we died
				    if (player.Health <= 0)
					    player.Active = false;
			    }
		    }
	    }

    }
}
