﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Torum_1._0;

namespace Torum_1._0
{
    class Enemy
    {
        TimeSpan Speedstart;
        TimeSpan speedStart;

 
        // Animation representing the enemy
        public Animation EnemyAnimation;
        // The position of the enemy ship relative to the top left corner of the screen
        public Vector2 Position;
        // The state of the Enemy Ship
        public bool Active;
        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;
        // The amount of damage the enemy inflicts on the player ship
        public int Damage;
        // The amount of score the enemy will give to the player
        public int Value;
        // Get the width of the enemy ship
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }
        // Get the height of the enemy ship
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }
        // The speed at which the enemy moves
        float enemyMoveSpeed;

        public void Initialize(Animation animation, Vector2 position)
        {
            Speedstart = TimeSpan.FromSeconds(13.0f);
            speedStart = TimeSpan.FromSeconds(26.0f);
            // Load the enemy ship texture
            EnemyAnimation = animation;
            // Set the position of the enemy
            Position = position;
            // We initialize the enemy to be active so it will be update in the game
            Active = true;
            // Set the health of the enemy
            Health = 50;
            // Set the amount of damage the enemy can do
            Damage = 10;
            // Set how fast the enemy moves
            enemyMoveSpeed = 21f;
            // Set the score value of the enemy
            Value = 100;

        }

        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement its x position
            Position.X -= enemyMoveSpeed;
            // Update the position of the Animation
            EnemyAnimation.Position = Position;
            // Update Animation
            EnemyAnimation.Update(gameTime);
            if (gameTime.TotalGameTime > Speedstart)
            {
                for (int i = 0; i < 100 && Active; i += 1)
                {                    
                    enemyMoveSpeed += .003f;
                }
            }
            if (gameTime.TotalGameTime > Speedstart && (gameTime.TotalGameTime > speedStart))
            {
                for (int i = 0; i < 125 && Active; i += 1)
                {
                    enemyMoveSpeed += .005f;
                }
            }
            if (gameTime.TotalGameTime > Speedstart && (gameTime.TotalGameTime > speedStart + Speedstart))
            {
                for (int i = 0; i < 150 && Active; i += 1)
                {
                    enemyMoveSpeed += .008f;
                }
            }
            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if (Position.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                Active = false;

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }

    }
}
