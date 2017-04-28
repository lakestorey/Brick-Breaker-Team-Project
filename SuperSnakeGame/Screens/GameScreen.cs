/*  Created by: Steven HL
 *  Project: Brick Breaker
 *  Date: Tuesday, April 4th
 */ 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Xml;

namespace BrickBreaker.Screens
{
    public partial class GameScreen : UserControl
    {
        #region global values

        //player1 button control keys - DO NOT CHANGE
        Boolean leftArrowDown, downArrowDown, rightArrowDown, upArrowDown, spaceDown;

        // Game values
        int lives;
        int currentLevel = 1;
        string levelToLoad;

        // Paddle and Ball objects
        Paddle paddle;
        Ball ball;

        // list of all blocks
        List<Block> blocks = new List<Block>();

        // Brushes
        SolidBrush paddleBrush = new SolidBrush(Color.White);
        SolidBrush ballBrush = new SolidBrush(Color.White);
        SolidBrush blockBrush = new SolidBrush(Color.Red);

        #endregion

        public GameScreen()
        {
            InitializeComponent();
            OnStart();
        }


        public void OnStart()
        {
            //set life counter
            lives = 3;

            //set all button presses to false.
            leftArrowDown = downArrowDown = rightArrowDown = upArrowDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2));
            int paddleY = (this.Height - paddleHeight) - 60;
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.White);

            // setup starting ball values
            int ballX = ((this.Width / 2) - 10);
            int ballY = (this.Height - paddle.height) - 80;

            // Creates a new ball
            int xSpeed = 6;
            int ySpeed = 6;
            int ballSize = 20;
            ball = new Ball(ballX, ballY, xSpeed, ySpeed, ballSize);

            //also added by Lake
            //// Creates blocks for generic level
            //blocks.Clear();
            //int x = 10;

            //while (blocks.Count < 12)
            //{
            //    x += 57;
            //    Block b1 = new Block(x, 10, 1, Color.White);
            //    blocks.Add(b1);
            //}
            loadLevel("level1.xml");

            // start the game engine loop
            gameTimer.Enabled = true;
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Space:
                    spaceDown = true;
                    break;
                default:
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
                case Keys.Space:
                    spaceDown = false;
                    break;
                default:
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Move the paddle
            if (leftArrowDown && paddle.x > 0)
            {
                paddle.Move("left");
            }
            if (rightArrowDown && paddle.x < (this.Width - paddle.width))
            {
                paddle.Move("right");
            }

            // Moves ball
            ball.Move();

            // Check for collision with top and side walls
            ball.WallCollision(this);

            // Check for collision of ball with paddle, (incl. paddle movement)
            ball.PaddleCollision(paddle, leftArrowDown, rightArrowDown);

            // Check if ball has collided with any blocks
            foreach (Block b in blocks)
            {
                if (ball.BlockCollision(b))
                {
                    blocks.Remove(b);

                    if (blocks.Count == 0)
                    {
                        //added by Lake
                        #region Decide Wich Level To Load
                        currentLevel++;

                        switch (currentLevel)
                        {
                            case 2:
                                levelToLoad = "level2.xml";
                                break;
                            case 3:
                                levelToLoad = "level3.xml";
                                break;
                            case 4:
                                levelToLoad = "level4.xml";
                                break;
                            case 5:
                                levelToLoad = "level5.xml";
                                break;
                            case 6:
                                levelToLoad = "level6.xml";
                                break;
                            case 7:
                                levelToLoad = "level7.xml";
                                break;
                            case 8:
                                levelToLoad = "level8.xml";
                                break;
                            case 9:
                                levelToLoad = "level9.xml";
                                break;
                            case 10:
                                levelToLoad = "level10.xml";
                                break;
                            case 11:
                                levelToLoad = "level11.xml";
                                break;
                            case 12:
                                levelToLoad = "level12.xml";
                                break;
                            case 13:
                                levelToLoad = "level13.xml";
                                break;
                            case 14:
                                OnEnd();
                                break;
                        }

                        loadLevel(levelToLoad);
                        ball.x = ((paddle.x - (ball.size / 2)) + (paddle.width / 2));
                        ball.y = (this.Height - paddle.height) - 85;
                        paddle.x = ((this.Width / 2) - (80 / 2));
                        paddle.y = (this.Height - 20) - 60;
                        #endregion

                        //gameTimer.Enabled = false;

                        //OnEnd();
                    }

                    break;
                }
            }

            // Check for ball hitting bottom of screen
            if (ball.BottomCollision(this))
            {
                lives--;

                // Moves the ball back to origin
                ball.x = ((paddle.x - (ball.size / 2)) + (paddle.width / 2));
                ball.y = (this.Height - paddle.height) - 85;

                //if (lives == 0)
                //{
                //    gameTimer.Enabled = false;

                //    OnEnd();
                //}
            }

            //redraw the screen
            Refresh();
        }

        //method to load the levels
        //Added by Lake
        public void loadLevel(string Level)
        {
            //clear list of blocks
            blocks.Clear();

            //create temp variables to hold strings
            string newX = "1";
            string newY = "1";
            string newHp = "1";
            string newColour = "Black";

            //make more temp variables to hold info
            int blockX;
            int blockY;
            int blockHp;
            Color blockColour;

            int items = 1;

            //extract info
            XmlTextReader reader = new XmlTextReader(Level);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    switch (items)
                    {
                        case 1:
                            newX = reader.Value;
                            break;
                        case 2:
                            newY = reader.Value;
                            break;
                        case 3:
                            newHp = reader.Value;
                            break;
                        case 4:
                            newColour = reader.Value;
                            blockX = Convert.ToInt16(newX);
                            blockY = Convert.ToInt16(newY);
                            blockHp = Convert.ToInt16(newHp);
                            blockColour = Color.FromName(newColour);
                            
                            Block newBlock = new Block(blockX, blockY, blockHp, blockColour);
                            blocks.Add(newBlock);
                            items = 0;
                            break;
                    }
                    items++;
                }
            }
            reader.Close();
        }

        public void OnEnd()
        {
            // Goes to the game over screen
            Form form = this.FindForm();
            MenuScreen ps = new MenuScreen();

            ps.Location = new Point((form.Width - ps.Width) / 2, (form.Height - ps.Height) / 2);

            form.Controls.Add(ps);
            form.Controls.Remove(this);
        }

        public void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Draws paddle
            e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);

            // Draws blocks
            foreach (Block b in blocks)
            {
                e.Graphics.FillRectangle(blockBrush, b.x, b.y, b.width, b.height);
            }
            
            // Draws balls
            e.Graphics.FillRectangle(ballBrush, ball.x, ball.y, ball.size, ball.size);
        }
    }
}
