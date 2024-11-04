using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace Snake_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameState gameState;
        public readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private bool gameRunning;
        private readonly Dictionary<GridValue, ImageSource> gridValueImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }

        }
        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }
        private  void Draw()
        {
            DrawGrid();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        private async Task RunGame()
        {
           Draw();
            await ShowCountDown();
            OverLay.Visibility = Visibility.Hidden;
             await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
                return;
            switch (e.Key)
            {
                case Key.Left:
                    if(gameState.Dir != Direction.Right)
                        gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
            }

        }
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(OverLay.Visibility == Visibility.Visible)
                e.Handled = true;

            if(!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void DrawGrid()
        {
            for(int r = 0; r < rows; r++) 
            {
                for(int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValueImage[gridVal];
                }
            }
        }
        private async Task ShowCountDown()
        {
            for(int i = 3; i >= 1; i--)
            {
                OverLayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }
        private async Task ShowGameOver()
        {
            await Task.Delay(1000);
            OverLay.Visibility = Visibility.Visible;
            OverLayText.Text = "PRESS ANY KEY TO START";
        }
    }
}