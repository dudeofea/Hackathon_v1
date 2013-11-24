using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Hackathon_v1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        VertexPositionColor[] vertices;
        Effect effect;
        Texture2D background, tiles, walkingTex;
        Rectangle screenBounds;
        int[,] level;
        bool charFlip = false;
        Color charColor = Color.White;
        float charStep = 0.0f;               //which step the character is in when walking
        Vector2 charPos;
        Vector2 charSpd = new Vector2(0, 0);
        Vector2 worldOffset;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
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
            device = graphics.GraphicsDevice;
            effect = Content.Load<Effect>("effects");
            background = Content.Load<Texture2D>("bg");
            walkingTex = Content.Load<Texture2D>("p1_walk");
            tiles = Content.Load<Texture2D>("tiles_spritesheet");
            screenBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            SetUpVertices();

            level = new int[,]{
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,49 ,0  ,165},
                {165,0  ,-2 ,0  ,0  ,135,0  ,0  ,0  ,0  ,0  ,0  ,112,112,112,112,112,112,111,112,112,112,111,112,112,28 ,112,0  ,62 ,0  ,165},
                {165,112,21 ,112,112,111,112,112,0  ,112,112,112,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,112,112,112,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,144,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,112,112,112,112,112,112,112,112,112,112,112,112,112,112,112,28 ,112,112,112,112,112,112,0  ,0  ,0  ,0  ,-1 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,112,112,112,34 ,112,112,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,144,0  ,0  ,0  ,0  ,0  ,112,112,112,112,0  ,0  ,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,112,112,112,112,0  ,0  ,0  ,112,112,112,0  ,0  ,0  ,112,165,165,165,165,112,112,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,112,112,112,165,165,165,165,112,112,112,165,165,165,112,112,112,165,165,165,165,165,165,165,112,112,112,112,112,112,112}
            };

            worldOffset = new Vector2(1, -screenBounds.Height / 70f + 16.8f);
            charPos = new Vector2(100, level.GetLength(0) * 70 - 200);
        }

        private void SetUpVertices()
        {
            vertices = new VertexPositionColor[3];

            vertices[0].Position = new Vector3(-0.5f, -0.5f, 0f);
            vertices[0].Color = Color.Red;
            vertices[1].Position = new Vector3(0, 0.5f, 0f);
            vertices[1].Color = Color.Green;
            vertices[2].Position = new Vector3(0.5f, -0.5f, 0f);
            vertices[2].Color = Color.Yellow;
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboard = Keyboard.GetState();

            Vector2 old_pos = charPos;

            charPos += charSpd;

            int bottom_index_y = (int)((charPos.Y + 102) / 70);
            int top_index_y = (int)((charPos.Y) / 70);
            int right_index_x = (int)((charPos.X + 65) / 70);
            int left_index_x = (int)((charPos.X) / 70);
            int right_edge = level.GetLength(1);
            int bottom_edge = level.GetLength(0);
            int collide = 0;
            charColor = Color.White;

            //for moving camera
            if (left_index_x > 4 && left_index_x < right_edge - 10)
            {
                worldOffset.X = charPos.X / 70 - 4;
            }
            if (top_index_y < bottom_edge - 4)
            {
                worldOffset.Y = (charPos.Y - screenBounds.Height) / 70 + 3.8f;
            }

            if (left_index_x >= 0 && right_index_x < right_edge - 1)
            {
                //ladder right
                if (level[top_index_y, left_index_x] == 34 || level[bottom_index_y, left_index_x] == 34)
                {
                    charSpd = new Vector2(0, -2f);
                }//ladder right
                else if (level[top_index_y, right_index_x] == 21 || level[bottom_index_y, right_index_x] == 21)
                {
                    charSpd = new Vector2(0, -2f);
                }//ladder right
                else if (level[top_index_y, left_index_x] == 111 || level[bottom_index_y,  left_index_x] == 111)
                {
                    charSpd = new Vector2(0, 2f);
                }//top of ladder for throwing left
                else if (level[bottom_index_y, left_index_x] == -1)
                {
                    charSpd = new Vector2(-2f, -2f);
                }//top of ladder for throwing right
                else if (level[bottom_index_y, left_index_x] == -2)
                {
                    charSpd = new Vector2(2f, -2f);
                }
                else
                {
                    //burn trap
                    if (level[top_index_y, left_index_x] == 28 || level[bottom_index_y, left_index_x] == 28)
                    {
                        charColor = Color.Red;
                    }
                    //check right wall
                    if ((isSolid(top_index_y, right_index_x) || isSolid(top_index_y + 1, right_index_x)))
                    {
                        collide++;
                        charSpd.X = -charSpd.X * 0.2f;
                    }
                    //check left wall
                    if ((isSolid(top_index_y, left_index_x) || isSolid(top_index_y + 1, left_index_x)))
                    {
                        collide++;
                        charSpd.X = -charSpd.X * 0.2f;
                    }
                    //check ceiling
                    if ((isSolid(top_index_y, left_index_x) || isSolid(top_index_y, left_index_x + 1)))
                    {
                        collide++;
                        charSpd.Y = -charSpd.Y * 0.5f;
                    }
                    //check floor
                    if ((isSolid(bottom_index_y, left_index_x) || isSolid(bottom_index_y, left_index_x + 1)))
                    {
                        collide++;
                        charSpd.Y = 0f;
                        charPos.Y = bottom_index_y * 70 - 102;
                        charSpd.X *= 0.99f;

                        if (keyboard.IsKeyDown(Keys.W))
                        {
                            charSpd += new Vector2(0, -7f);
                        }
                        if (keyboard.IsKeyDown(Keys.A))
                        {
                            charSpd.X -= 0.1f;
                            charFlip = true;
                        }
                        if (keyboard.IsKeyDown(Keys.D))
                        {
                            charSpd.X += 0.1f;
                            charFlip = false;
                        }

                        charStep += Math.Abs(charSpd.X / 10f);
                        if (charStep > 5.0f)
                        {
                            charStep = 0.0f;
                        }
                    }
                }
            }
            else
            {
                charSpd.X = -charSpd.X * 0.8f;
            }

            if (collide == 0)
            {
                charSpd += new Vector2(0.01f, 0.2f);
                charStep = 5;
            }
            else if (collide > 1)
            {
                charPos = old_pos;
            }

            base.Update(gameTime);
        }

        //check if you can fall through the selected tile
        protected bool isSolid(int y, int x)
        {
            if (x < 0 || x > level.GetLength(1))
            {
                return false;
            }//for utility blocks
            if (level[y, x] == 34 || level[y, x] == 21 || level[y, x] == 49 || level[y, x] == 62 || level[y, x] == 111)
            {
                return false;
            }
            if (level[y, x] > 0)
            {
                return true;
            }
            return false;
        }

        //function to draw 70x70 pixel tiles
        //tiles are offset from the top-left corner
        protected void DrawTile(int x, int y, int tile)
        {
            spriteBatch.Draw(tiles, new Rectangle((int)((x - worldOffset.X) * 70), (int)((y - worldOffset.Y) * 70) - 10, 70, 70), new Rectangle((tile%13) * 72, (int)(tile/13) * 72, 70, 70), Color.White);
        }

        protected void DrawWalk(int step)
        {
            if (!charFlip)
            {
                spriteBatch.Draw(walkingTex, new Rectangle((int)(charPos.X - worldOffset.X * 70), (int)(charPos.Y - worldOffset.Y * 70), 65, 92), new Rectangle((step % 3) * 67, (int)(step / 3) * 92, 65, 92), charColor);
            }
            else
            {
                spriteBatch.Draw(walkingTex, new Rectangle((int)(charPos.X - worldOffset.X * 70), (int)(charPos.Y - worldOffset.Y * 70), 65, 92), new Rectangle((step % 3) * 67 + 66, (int)(step / 3) * 92, -65, 92), charColor);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, screenBounds.Width, screenBounds.Height), Color.White);
            for (int i = 0; i < level.GetLength(0); i++)
            {
                for (int j = 0; j < level.GetLength(1); j++)
                {
                    if (level[i, j] > 0)
                    {
                        DrawTile(j, i, level[i, j] - 1);
                    }
                }
            }
            DrawWalk((int)charStep);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
