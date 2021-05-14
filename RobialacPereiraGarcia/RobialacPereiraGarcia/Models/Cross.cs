using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RobialacPereiraGarcia.Models
{
    class Cross : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D spriteSheet;
        Point position;
        Game1 game;
        public static int numberCrosses = 0;

        public Cross(Game1 game, int x, int y) : base(game)
        {
            position = new Point(x, y);
            spriteBatch = game.SpriteBatch;
            spriteSheet = game.SpriteSheet;
            numberCrosses++;
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {

            Rectangle crossArea = new Rectangle(
                    position,
                    new Point(Game1.outputTileSize));

            foreach (Enemy ghost in game.ghosts.ToArray())
            {
                Rectangle ghostArea = new Rectangle(
                    ghost.position, new Point(Game1.outputTileSize)
                );
                if (ghostArea.Intersects(crossArea))
                {
                    game.ghosts.Remove(ghost);
                    game.Components.Remove(ghost);
                }
            }

            game.Components.Remove(this);
            Cross.numberCrosses--;
        }


        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                spriteSheet,
                // target position
                new Rectangle(
                    (position.ToVector2() * Game1.outputTileSize).ToPoint(),
                    new Point(Game1.outputTileSize)
                ),
                // source position
                new Rectangle(2 * 16, 2 * 16, 16, 16),
                Color.White
            );
            spriteBatch.End();
        }
    }
}
