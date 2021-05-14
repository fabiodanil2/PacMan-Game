using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RobialacPereiraGarcia.Models
{
    public class Board : DrawableGameComponent
    {
        public char[,] board;

        public int Height, Width;

        public Board(Game1 game, int width, int height) : base(game)
        {
            Width = width;
            Height = height;
            board = new char[width, height];
        }
    }
}