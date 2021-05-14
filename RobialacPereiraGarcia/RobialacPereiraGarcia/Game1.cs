using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RobialacPereiraGarcia.Models;
using Microsoft.Xna.Framework.Media;

namespace RobialacPereiraGarcia
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        /* Especificação das letras:

            E - enemy
            P - pac-man
            W - wall
            F - food
            B - bonus

        */

        // declaração de variáveis
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteBatch spriteBatch2;

        Song backGroundMusic;

        Texture2D spriteSheet;
        Texture2D spriteSheet2;

        int boardWidth, boardHeight;
        Board board;

        public const int outputTileSize = 32;

        // serverá para randomizar a direção dos ghosts
        public static Random RNG = new Random();

        // As nossas entidades

        // esta variavel é criada para guardar uma referência ao player
        public Player player;
        
        // esta lista guarda todos os inimigos
        public List<Enemy> ghosts = new List<Enemy>();

        #region Propriedades

        // utilize está propriedade para obter o board do jogo
        public Board Board { get { return board; } }

        // utilize está propriedade para obter a spriteSheet do jogo
        public Texture2D SpriteSheet { get { return spriteSheet; } }

        /// <summary>
        /// Cosuma está propriedade para obter a spriteBatch do jogo
        /// </summary>
        public SpriteBatch SpriteBatch { get { return spriteBatch; } }

        /// <summary>
        /// Consuma esta propriedade para obter a largura do board
        /// </summary>
        public int BoardWidth { get { return boardWidth; } }

        /// <summary>
        /// Consuma esta propriedade para obter a altura do board
        /// </summary>
        public int BoardHeight { get { return boardHeight; } }

        public GraphicsDeviceManager Graphics
        {
            get
            {
                return graphics;
            }
        }

        #endregion

        #region Metodos
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch2 = new SpriteBatch(GraphicsDevice);
            spriteSheet = Content.Load<Texture2D>("ss");
            spriteSheet2 = Content.Load<Texture2D>("cross");
            backGroundMusic = Content.Load<Song>("music");
            MediaPlayer.Play(backGroundMusic);
            LoadLevel();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Rectangle outRect = new Rectangle(x * outputTileSize, y * outputTileSize,
                                              outputTileSize, outputTileSize);

                    switch (Board.board[x, y])
                    {
                        case 'W':
                            spriteBatch.Draw(
                                spriteSheet, outRect, new Rectangle(0, 2 * 16, 16, 16),
                                Color.White
                            );
                            break;
                        case 'B':
                            spriteBatch.Draw(
                                spriteSheet, outRect, new Rectangle(2 * 16, 2 * 16, 16, 16),
                                Color.White
                            );
                            break;
                        case 'F':
                            spriteBatch.Draw(
                                spriteSheet, outRect, new Rectangle(1 * 16, 2 * 16, 16, 16),
                                Color.White
                            );
                            break;
                        case 'C':
                            spriteBatch.Draw(
                                spriteSheet2, outRect, new Rectangle(0 * 16, 0 * 16, 16, 16),
                                Color.White
                                );
                            break;
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void LoadLevel()
        {
            string[] file = File.ReadAllLines(Content.RootDirectory + "/level0.txt");
            boardWidth = file[0].Length;
            boardHeight = file.Length;

            // Cria o objeto board e adiciona-o a lista de componentes
            board = new Board(this, boardWidth, boardHeight);
            Components.Add(board);

            for (int i = 0; i < boardHeight; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                {
                    if (file[i][j] == 'E')
                    {
                        Enemy ghost = new Enemy(this, j, i); // é criado um novo inimigo
                        ghosts.Add(ghost); // o ghost é adicionado à lista de inimigos
                        Components.Add(ghost); // ghost adicionado aos componentes
                    }
                    else if (file[i][j] == 'P')
                    {
                        player = new Player(this, j, i); // player é criado
                        Components.Add(player); // player adicionado aos componentes
                    }
                    else
                    {
                        // se não for uma entidade, vai ser o board
                        Board.board[j, i] = file[i][j];
                    }
                }
            }

            // definir o tamanho da janela
            graphics.PreferredBackBufferWidth = boardWidth * outputTileSize;
            graphics.PreferredBackBufferHeight = (boardHeight + 1) * outputTileSize;
            graphics.ApplyChanges();
        }

        #endregion
    }
}

