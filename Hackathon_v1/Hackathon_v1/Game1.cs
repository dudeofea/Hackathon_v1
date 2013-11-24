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
        Texture2D background, tiles, walkingTex, coin;
        Rectangle screenBounds;
        int[,] level, collisions;
        bool charFlip = false;
        Color charColor = Color.White;
        float charStep = 0.0f;               //which step the character is in when walking
        Vector2 charPos;
        Vector2 charSpd = new Vector2(0, 0);
        Vector2 worldOffset;
        Texture2D[] hud_nums = new Texture2D[10];
        int coinCount = 0;
        float burnTimeout = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 700;
            graphics.IsFullScreen = false;
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
            coin = Content.Load<Texture2D>("Coin");
            for (int i = 0; i < 10; i++)
            {
                hud_nums[i] = Content.Load<Texture2D>("hud_" + i.ToString());
            }
            tiles = Content.Load<Texture2D>("tiles_spritesheet");
            screenBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            SetUpVertices();

            level = new int[,]{
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,-3 ,0  ,-3 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,-3 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,49 ,0  ,165},
                {165,0  ,21 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,112,112,112,112,112,112,111,112,112,112,111,112,112,28 ,112,0  ,62 ,0  ,165},
                {165,112,34 ,112,112,112,112,111,111,112,112,112,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,112,112,112,165},
                {165,0  ,34 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,34 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,34 ,0  ,0  ,-3 ,0  ,-3 ,0  ,0  ,0  ,0  ,0  ,-3 ,0  ,0  ,0  ,0  ,0  ,0  ,-3 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,0  ,34 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,-3 ,0  ,0  ,0  ,0  ,165},
                {165,0  ,34 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,144,0  ,0  ,0  ,0  ,0  ,0  ,0  ,35 ,112,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,165},
                {165,112,112,112,112,112,112,112,112,112,112,112,112,112,28 ,112,112,112,22 ,165,112,112,112,9  ,0  ,0  ,0  ,21 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,164,112,112,112,34 ,112,112,165},
                {165,0  ,0  ,0  ,0  ,-3 ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,-3 ,0  ,0  ,144,0  ,0  ,-3 ,0  ,0  ,112,112,112,112,0  ,0  ,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,0  ,0  ,0  ,35 ,112,112,9  ,0  ,0  ,0  ,112,112,112,0  ,0  ,0  ,112,165,165,165,165,112,112,0  ,0  ,0  ,34 ,0  ,0  ,165},
                {165,112,112,112,22 ,165,165,164,112,112,112,165,165,165,112,112,112,165,165,165,165,165,165,165,112,112,112,112,112,112,112}
            };

            //0: Nothing
            //1: Collide
            //2: Ladder
            //3: Thrust Down
            //4: Thrust Left
            //5: Thrust Right
            //6: Exit
            //7: Upwards Slope /
            //8: Downwards Slope \
            //9: Second Collide (Items and whatnot)
            collisions = new int[,]{
                {1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0 , 0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  },
                {1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  },
                {1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,6  ,0  ,1  },
                {1  ,5  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  ,1  ,1  ,1  ,1  ,1  ,3  ,1  ,1  ,1  ,3  ,1  ,1  ,1  ,1  ,0  ,6  ,0  ,1  },
                {1  ,2  ,0  ,1  ,1  ,1  ,1  ,3  ,3  ,1  ,1  ,1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  ,1  ,1  ,1  },
                {1  ,2  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  },
                {1  ,2  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  },
                {1  ,2  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  },
                {1  ,2  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  },
                {1  ,2  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,9  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,7  ,9  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  },
                {1  ,2  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,1  ,8  ,0  ,0  ,0  ,4  ,0  ,0  ,1  },
                {1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  ,1  ,1  ,1  ,2  ,1  ,1  ,1  },
                {1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,2  ,0  ,0  ,1  },
                {1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,2  ,0  ,0  ,1  },
                {1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,9  ,0  ,0  ,0  ,0  ,0  ,1  ,1  ,1  ,1  ,0  ,0  ,0  ,0  ,0  ,2  ,0  ,0  ,1  },
                {1  ,0  ,0  ,0  ,7  ,1  ,1  ,8  ,0  ,0  ,0  ,1  ,1  ,1  ,0  ,0  ,0  ,1  ,0  ,0  ,0  ,0  ,1  ,1  ,0  ,0  ,0  ,2  ,0  ,0  ,1  },
                {1  ,1  ,1  ,1  ,1  ,0  ,0  ,1  ,1  ,1  ,1  ,0  ,0  ,0  ,1  ,1  ,1  ,0  ,0  ,0  ,0  ,0  ,0  ,0  ,1  ,1  ,1  ,2  ,1  ,1  ,1  }
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
            int above_ground_index_y = (int)((charPos.Y + 90) / 70);
            int top_index_y = (int)((charPos.Y) / 70);
            int right_index_x = (int)((charPos.X + 65) / 70);
            int left_index_x = (int)((charPos.X) / 70);

            int right_edge = level.GetLength(1);
            int bottom_edge = level.GetLength(0);
            int collide = 0;

            burnTimeout -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (burnTimeout < 0)
            {
                burnTimeout = 0;
                charColor = Color.White;
            }

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
                if (collisions[bottom_index_y, left_index_x] == 2)
                {
                    charSpd = new Vector2(0, -3f);
                }//water trap
                else if (level[top_index_y, left_index_x] == 111 || level[bottom_index_y,  left_index_x] == 111)
                {
                    charSpd = new Vector2(0, 2f);
                }//top of ladder for throwing right
                else if (collisions[bottom_index_y, left_index_x] == 5)
                {
                    charFlip = true;
                    charSpd = new Vector2(1.5f, -2f);
                }//top of ladder for throwing left
                else if (collisions[bottom_index_y, left_index_x] == 4)
                {
                    charFlip = false;
                    charSpd = new Vector2(-2f, -2f);
                }//coin capture
                else if (level[top_index_y, left_index_x] == -3)
                {
                    coinCount++;
                    level[top_index_y, left_index_x] = 0;
                }//this is the exit
                else if (collisions[bottom_index_y, left_index_x] == 6)
                {
                    //exity stuff
                }
                else
                {
                    //burn trap
                    if (burnTimeout <= 0 && (level[top_index_y, left_index_x] == 28 || level[bottom_index_y, left_index_x] == 28))
                    {
                        burnTimeout = 2000;
                        charColor = Color.Red;
                        coinCount--;
                        if (coinCount < 0)
                        {
                            coinCount = 0;
                        }
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
                        charPos.Y = bottom_index_y*70 - 102;
                        charSpd.X *= 0.99f;

                        //check direction
                        charFlip = true;
                        for (int i = bottom_index_y; i < bottom_edge; i++)
                        {
                            if(collisions[i, left_index_x] == 1)
                            {
                                charFlip = !charFlip;
                            }
                        }

                        checkInput(keyboard);

                        charStep += Math.Abs(charSpd.X / 10f);
                        if (charStep > 5.0f)
                        {
                            charStep = 0.0f;
                        }
                    }
                    //check up slope
                    if (isSlope(above_ground_index_y, right_index_x) || isSlope(bottom_index_y, left_index_x) || 
                        isSlope(above_ground_index_y, left_index_x) || isSlope(bottom_index_y, right_index_x))
                    {
                        //for going up the slope
                        if (collisions[above_ground_index_y, right_index_x] == 7)
                        {
                            float diff = -charPos.X + right_index_x*70 - 40;
                            charPos.Y = above_ground_index_y * 70 - 50 +diff;
                            //charSpd.X -= 0.01f;
                        }//for going down the slope
                        else if (collisions[bottom_index_y, left_index_x] == 8)
                        {
                            float diff = charPos.X - left_index_x * 70 + 10;
                            charPos.Y = bottom_index_y * 70 - 102 + diff;
                            //charSpd.X += 0.01f;
                        }//for going up the slope
                        else if (collisions[above_ground_index_y, left_index_x] == 8)
                        {
                            float diff = charPos.X - left_index_x * 70 - 2;
                            charPos.Y = above_ground_index_y * 70 - 102 + diff;
                            //charSpd.X += 0.01f;
                        }//for going down the slope
                        else if (collisions[bottom_index_y, left_index_x] == 7)
                        {
                            float diff = -charPos.X + left_index_x * 70 + 10;
                            charPos.Y = bottom_index_y * 70 - 102 + diff;
                            //charSpd.X += 0.01f;
                        }

                        checkInput(keyboard);
                    }
                }
            }
            else
            {
                charSpd.X = -charSpd.X * 0.8f;
            }

            if (collide == 0)
            {
                charSpd += new Vector2(0.0f, 0.2f);
                charStep = 5;
            }
            else if (collide > 1)
            {
                charPos = old_pos;
            }

            base.Update(gameTime);
        }

        protected void checkInput(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.W))
            {
                charSpd += new Vector2(0, -7f);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                if (charFlip)
                {
                    charSpd.X -= 0.1f;
                }
                else
                {
                    charSpd.X += 0.1f;
                }
            }
        }

        //check if you can fall through the selected tile
        protected bool isSolid(int y, int x)
        {
            if (x < 0 || x > level.GetLength(1))
            {
                return false;
            }
            if (collisions[y, x] == 1 || collisions[y, x] == 9)
            {
                return true;
            }
            return false;
        }

        protected bool isSlope(int y, int x)
        {
            if (x < 0 || x > level.GetLength(1))
            {
                return false;
            }
            if(collisions[y, x] == 7 || collisions[y, x] == 8)
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

        //works for numbers from 0 to 99
        protected void DrawNumber(int x, int y, int num)
        {
            if (num > 9)
            {
                spriteBatch.Draw(hud_nums[num % 10], new Rectangle(x + 30, y, 30, 38), new Rectangle(0, 0, 30, 38), Color.White);
                spriteBatch.Draw(hud_nums[num / 10], new Rectangle(x, y, 30, 38), new Rectangle(0, 0, 30, 38), Color.White);
            }
            else
            {
                spriteBatch.Draw(hud_nums[num], new Rectangle(x + 30, y, 30, 38), new Rectangle(0, 0, 30, 38), Color.White);
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
                    else if (level[i, j] == -3)
                    {
                        spriteBatch.Draw(coin, new Rectangle((int)((j - worldOffset.X) * 70), (int)((i - worldOffset.Y) * 70) - 10, 70, 70), new Rectangle(0, 0, 70, 70), Color.White);
                    }
                }
            }
            DrawWalk((int)charStep);
            DrawNumber(40, screenBounds.Height - 40, coinCount);
            spriteBatch.Draw(coin, new Rectangle(0, screenBounds.Height - 50, 50, 50), new Rectangle(0, 0, 70, 70), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
