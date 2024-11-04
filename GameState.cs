using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Collections.Generic;

namespace Snake_Game
{
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver {  get; private set; }
        private readonly Dictionary<Direction, bool> keys = new Dictionary<Direction, bool>();
        private readonly LinkedList<Position> snakePosition = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }
        public void ChangeDirection(Direction dir)
        {
            Dir= dir;
        }

        private void AddSnake()
        {
            int r = Rows / 2;

            for(int c = 1; c<= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePosition.AddFirst(new Position(r, c));

            }
        }

        public IEnumerable<Position> EmptyPositions()
        {
            for(int r = 0; r < Rows; r++)
            {
                for(int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                        yield return new Position(r, c);
                }
            }
        }

        public void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if (empty.Count == 0)
             return; 

            Position pos = empty[random.Next(empty.Count)];
           Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Position HeadPoisition()
        {
            return snakePosition.First.Value;
        }

        public Position TailPosition()
        {
            return snakePosition.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePosition;
        }

        private void AddHead(Position pos)
        {
            snakePosition.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail() 
        {
            Position tail = snakePosition.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePosition.RemoveLast();
        }
        

        private bool OutSideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols; 
        }

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutSideGrid(newHeadPos))
                return GridValue.Outside;

            if (newHeadPos == TailPosition())
                return GridValue.Empty;

            return Grid[newHeadPos.Row, newHeadPos.Col];    
        }

        public void Move()
        {
            Position newHeadPos = HeadPoisition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
                GameOver = true;
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
