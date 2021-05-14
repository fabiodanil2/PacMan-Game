using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RobialacPereiraGarcia.Enums;
using Microsoft.Xna.Framework.Audio;
using System;

namespace RobialacPereiraGarcia.Models
{
    public class Player : DrawableGameComponent
    {
        // variaveis de estado
        Dictionary<Direction, Vector2> spritePositions;
        Direction direction = Direction.North;

        int frame = 0;
        int lives = 3;
        float timer = 0;
        public int totalPoints = 0;
        int highScore = 0;
        string High_Score;
        string A;
        bool win = false;
        

        public Point position;
        Point targetPosition;
        Point origin;

        Texture2D texture;
        SpriteBatch spriteBatch;
        SpriteFont arial;
        Game1 game;
        Board board;
        SoundEffect munch;
        SoundEffect death;

        public Player(Game1 game, int x, int y) : base(game)
        {
            DrawOrder = 100; // definir pacman como ultimo elemento a ser desenhado
            position.Y = y * Game1.outputTileSize;
            position.X = x * Game1.outputTileSize;
            origin = targetPosition = position;

            texture     = game.SpriteSheet;
            spriteBatch = game.SpriteBatch;
            board       = game.Board;
            this.game   = game;
            munch = game.Content.Load<SoundEffect>("pacman_chomp");
            death = game.Content.Load<SoundEffect>("pacman_death");
            arial = game.Content.Load<SpriteFont>("Arial");

            spritePositions = new Dictionary<Direction, Vector2>();
            spritePositions[Direction.East] = new Vector2(1, 0);
            spritePositions[Direction.West] = new Vector2(1, 3);
            spritePositions[Direction.North] = new Vector2(1, 6);
            spritePositions[Direction.South] = new Vector2(1, 7);
        }

        #region Propriedades

        public int Lives 
        { 
            get 
            { 
                return lives; 
            }
            
            set
            {
                lives = value;
            }
        }

        #endregion

        #region Métodos

        public override void Update(GameTime gameTime)
        {
            // declaração de variaveis
            bool keyPressed = false;
            KeyboardState state = Keyboard.GetState();

            if (targetPosition == position)
            {
                frame = 0;

                if (state.IsKeyDown(Keys.C))
                {
                    // coloca uma cruz
                    if (Cross.numberCrosses == 0)
                    {
                        Cross cross = new Cross(
                            game,
                            position.X / Game1.outputTileSize,
                            position.Y / Game1.outputTileSize);
                        game.Components.Add(cross);
                    }
                }

                if (state.IsKeyDown(Keys.Up)) 
                {
                    direction = Direction.North;
                    keyPressed = true;
                }
                if (state.IsKeyDown(Keys.Down))
                {
                    direction = Direction.South;
                    keyPressed = true;
                }
                if (state.IsKeyDown(Keys.Right))
                {
                    direction = Direction.East;
                    keyPressed = true;
                }
                if (state.IsKeyDown(Keys.Left))
                {
                    direction = Direction.West;
                    keyPressed = true;
                }

                if (keyPressed)
                {
                    switch (direction)
                    {
                        case Direction.North:
                            targetPosition.Y -= Game1.outputTileSize;
                            break;
                        case Direction.South:
                            targetPosition.Y += Game1.outputTileSize;
                            break;
                        case Direction.East:
                            targetPosition.X += Game1.outputTileSize;
                            break;
                        case Direction.West:
                            targetPosition.X -= Game1.outputTileSize;
                            break;
                    }

                    if (board.board[targetPosition.X / Game1.outputTileSize, targetPosition.Y / Game1.outputTileSize] == 'W')
                    {
                        targetPosition = position;
                    }
                }
            }
            else
            {
                // Posição não é a mesma, mover player
                Vector2 vec = targetPosition.ToVector2() - position.ToVector2();
                vec.Normalize();
                position = (position.ToVector2() + vec).ToPoint();

                if ((position.X + position.Y) % 6 == 0)
                    frame++;
                if (frame > 1)
                    frame = -1;
            }

            // retirar as bolas do campo e efectuar o som
            if (game.Board.board[position.X / Game1.outputTileSize, position.Y / Game1.outputTileSize] == 'F')
            {
                game.Board.board[position.X / Game1.outputTileSize, position.Y / Game1.outputTileSize] = ' ';
                munch.Play();
                totalPoints += 200;
            }
            else if(game.Board.board[position.X / Game1.outputTileSize, position.Y / Game1.outputTileSize] == 'B')
            {
                game.Board.board[position.X / Game1.outputTileSize, position.Y / Game1.outputTileSize] = ' ';
                munch.Play();

                if (timer < 2)
                {
                    Enemy.enemyDefenseless = true;
                    timer++;
                }
                else
                {
                    Enemy.enemyDefenseless = false;
                    timer = 0f;
                }
                
            }

            // Cria um ficheiro onde guarda o numero total de pontos do jogo passado se este for maior que o valor que la ta
            if (totalPoints > highScore)
            {
                StreamWriter sw = new StreamWriter("Content" + "/HighScore.txt");
                sw.WriteLine(totalPoints);
                sw.Close();

            }

            // HighScore
            StreamReader sr = new StreamReader("Content" + "/HighScore.txt");
            A = sr.ReadLine();
            sr.Close();
            highScore = Int32.Parse(A);

            // verificar se ganhou o jogo
            if (totalPoints == 38400)
                Win();

        }

        public void Die() {
            lives--;
            death.Play();
            position = targetPosition = origin;
            if (lives <= 0) {
                
                foreach (GameComponent comp in game.Components)
                {
                    comp.Enabled = false;
                }
            }
        }


        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                texture,
                // Target Position
                new Rectangle(position, new Point(Game1.outputTileSize)),
                // Source on SpriteSheet
                new Rectangle(
                    ((spritePositions[direction] + Vector2.UnitX * frame) * 16).ToPoint(),
                    new Point(16, 16)
                ),
                Color.White
                );

            //pontuação
            string Points = string.Format("Score: {0} Points", totalPoints);
            Vector2 dim = arial.MeasureString(Points);
            spriteBatch.DrawString(arial, Points, new Vector2((game.BoardHeight * Game1.outputTileSize - dim.X) / (float)30, 620), Color.Red);


            //pontuacao maxima
            High_Score = string.Format("HighScore: {0} Points", highScore);
            dim = arial.MeasureString(High_Score);
            spriteBatch.DrawString(arial, High_Score, new Vector2((game.BoardHeight * Game1.outputTileSize - dim.X), 620), Color.Red);

            if (lives <= 0 || win)
            {
                // Draw Game Over / Win
                string message = win ? "YOU WIN" : "GAME OVER!";
                GraphicsDevice.Clear(Color.Black);
                Vector2 stringSize = arial.MeasureString(message);
                Vector2 screenSize =
                    new Vector2(game.Graphics.PreferredBackBufferWidth,
                                game.Graphics.PreferredBackBufferHeight);
                Vector2 textPos = (screenSize - stringSize) / 2f;
                spriteBatch.DrawString(arial, message, textPos, Color.BlueViolet);
            }

            spriteBatch.End();
        }
            

        public void Win()
        {
            win = true;
            foreach (GameComponent comp in game.Components)
            {
                comp.Enabled = false;
            }
        }
        #endregion
    }
}
