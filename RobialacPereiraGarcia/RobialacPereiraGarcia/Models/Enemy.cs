using System.IO;
using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RobialacPereiraGarcia.Enums;
using Microsoft.Xna.Framework.Audio;

namespace RobialacPereiraGarcia.Models
{
    public class Enemy : DrawableGameComponent
    {
        // declaração de variáveis

        public Point position; // posição atual do inimigo
        Point targetPosition; // posição para a qual o inimigo quer ir quando se move
        Point origin; // initial position of the ghost

        Orientation orientation; // variavel que guarda a orientação
        int direction = 1, patrolPosition = 0; // quando o jogo começa, o inimigo a andar numa certa direção

        float timer = 0f;

        // referências
        Game1 game;
        Board board; 

        Texture2D texture;
        SpriteBatch spriteBatch;

        SoundEffect ghostDeath;

        public static bool enemyDefenseless = false;

        public Enemy(Game1 game, int x, int y) : base(game)
        {
            position.Y = y * Game1.outputTileSize;
            position.X = x * Game1.outputTileSize;

            // inicialmente, o inimigo começa parado
            origin = targetPosition = position;

            // referência ao game
            this.game = game;

            //patrolSize = 2 + Game1.RNG.Next(4);

            orientation = Game1.RNG.Next(2) > 0 ?
                          Orientation.Horizontal : Orientation.Vertical;

            texture = game.SpriteSheet;
            spriteBatch = game.SpriteBatch;
            ghostDeath = game.Content.Load<SoundEffect>("pacman_eatghost");
        }

        public override void Update(GameTime gameTime)
        {
            //declaração de variáveis

            Rectangle playerArea = new Rectangle(
                    game.player.position, new Point(Game1.outputTileSize));
            Rectangle enemyArea = new Rectangle(
                    position, new Point(Game1.outputTileSize));

            if (enemyDefenseless == true && playerArea.Intersects(enemyArea))
            {
                // o inimigo vai morrer se o pacman lhe tocar, enquanto o beast mode esta ativo
                Die();
                enemyDefenseless = false;
            }
            else if (enemyDefenseless == false && playerArea.Intersects(enemyArea))
                   game.player.Die();

            // fantasma chega a um limite do mapa e tem de decidir para onde ir
            if (position == targetPosition)
            {
                // move horizontally or vertically one unit
                targetPosition +=
                        orientation == Orientation.Horizontal
                    ? new Point(direction * Game1.outputTileSize, 0)
                    : new Point(0, direction * Game1.outputTileSize);

                if (game.Board.board[targetPosition.X / Game1.outputTileSize,
                        targetPosition.Y / Game1.outputTileSize] == ' ')
                {
                    // increment patrol Position
                    patrolPosition += direction;
                }
                else if (game.Board.board[targetPosition.X / Game1.outputTileSize,targetPosition.Y / Game1.outputTileSize] == 'W')
                {
                    targetPosition = position;
                    direction = - direction; // change direction
                }
            }
            else
            {
                // I like to move it move it...
                Vector2 dir = (targetPosition - position).ToVector2();
                dir.Normalize();
                position += dir.ToPoint();
            }
        }

        public void Die()
        {
            // se o ghost morrer, o player ganha 400 pontos
            game.player.totalPoints += 400;

            Rectangle playerArea = new Rectangle(
                    game.player.position, new Point(Game1.outputTileSize));

            if (timer <= 4)
            {
                foreach (var enemy in game.ghosts.ToArray())
                {
                    Rectangle eArea = new Rectangle(
                        enemy.position, new Point(Game1.outputTileSize)
                    );
                    if (playerArea.Intersects(eArea))
                    {
                        //  volta à sua posição inicial
                        position = targetPosition = origin;
                        //game.ghosts.Remove(enemy);
                        //game.Components.Remove(enemy);
                    }
                }
                timer++;
            }
            ghostDeath.Play();
        }
            
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // desenhar o sprite do nosso inimigo
            spriteBatch.Draw(
                texture,
                new Rectangle(position.X, position.Y, Game1.outputTileSize, Game1.outputTileSize),
                new Rectangle(0 * 16, 1 * 16, 16, 16),
                Color.White
            );

            if (enemyDefenseless == false)
            {
                spriteBatch.Draw(
                texture,
                new Rectangle(position.X, position.Y, Game1.outputTileSize, Game1.outputTileSize),
                new Rectangle(0 * 16, 1 * 16, 16, 16),
                Color.White);
            }
            else
            {
                spriteBatch.Draw(
                texture,
                new Rectangle(position.X, position.Y, Game1.outputTileSize, Game1.outputTileSize),
                new Rectangle(0 * 16, 1 * 16, 16, 16),
                Color.DeepSkyBlue);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

