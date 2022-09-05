using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace wpfMinesweeper.Model
{
    public enum cellState
    {
        unknown,
        none,
        mine,
        flag
    }

    public struct Point
    {
        public int row;
        public int col;
    }

    public class MinefieldCell : Button
    {
        // priv Members
        private cellState actualState;
        private cellState publicState;
        private Point point;
        private int neighborMines;

        // public Members
        public MinefieldCell[,] field = null;
        public MainWindow _mainWindow;
        public List<Point> checkedNeigbors;

        // public properties
        public cellState ActualState
        {
            get { return actualState; }
            set { actualState = value; }
        }

        public cellState PublicState
        {
            get { return publicState; }
            set { publicState = value; UpdateLook(); }
        }

        // constructor
        public MinefieldCell(int row, int col)
        {
            this.point.row = row;
            this.point.col = col;
            this.neighborMines = 0;
            this.Click += LeftClick;
            this.MouseRightButtonDown += RightClick;
            this.publicState = cellState.unknown;
            this.actualState = cellState.none;
        }

        // methods
        public void UpdateLook()
        {
            if (this.publicState == cellState.unknown)
            {
                this.Background = Brushes.LightGray;
                this.Content = null;
            }

            else if (this.publicState == cellState.none)
                this.Background = Brushes.White;

            else if (this.publicState == cellState.flag)
                this.Content = new Image
                {
                    Source = new BitmapImage(new Uri("/Resources/flag.png", UriKind.RelativeOrAbsolute))
                };

            else if (this.publicState == cellState.mine)
                this.Content = new Image
                {
                    Source = new BitmapImage(new Uri("/Resources/mine.png", UriKind.RelativeOrAbsolute))
                };
        }

        public void LeftClick(object sender, EventArgs e)
        {
            if (this.ActualState == cellState.none)
            {
                this.PublicState = cellState.none;

                if (neighborMines != 0)
                {
                    this.Content = neighborMines.ToString();
                    return;
                }

                RevealNeighbors();
                checkedNeigbors.Clear();
            }
            else if (this.ActualState == cellState.mine)
            {
                foreach (MinefieldCell cell in field)
                {
                    if (cell.ActualState == cellState.mine)
                    {
                        cell.PublicState = cellState.mine;
                    }
                }

                MessageBox.Show("You lost.. Better luck next time!");

                _mainWindow.Reset();
            }
        }

        public void RightClick(object sender, EventArgs e)
        {
            if (this.publicState == cellState.unknown)
            {
                this.PublicState = cellState.flag;
                _mainWindow.headerFlagsLeft.Text = (Convert.ToInt32(_mainWindow.headerFlagsLeft.Text) - 1).ToString();
            }
            else if (this.publicState == cellState.flag)
            {
                this.PublicState = cellState.unknown;
                _mainWindow.headerFlagsLeft.Text = (Convert.ToInt32(_mainWindow.headerFlagsLeft.Text) + 1).ToString();
            }

            CheckWin();
        }

        public void CheckWin()
        {
            if (_mainWindow.headerFlagsLeft.Text == "0")
            {
                int count = 0;

                foreach (MinefieldCell cell in field)
                {
                    if (cell.ActualState == cellState.mine && cell.PublicState == cellState.flag)
                    {
                        count++;
                    }
                }

                if (count == 50)
                {
                    MessageBox.Show("You win!!!");
                    _mainWindow.Reset();
                }
            }
        }

        public void CheckNeighbors()
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    if (point.row + i < 0 || point.row + i > 15)
                        continue;

                    if (point.col + j < 0 || point.col + j > 15)
                        continue;

                    if (field[point.row + i, point.col + j].actualState == cellState.mine)
                        count++;
                }
            }

            this.neighborMines = count;
        }

        public void RevealNeighbors()
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Point p = new Point { row = point.row + i, col = point.col + j };
                    if (checkedNeigbors.Contains(p))
                        continue;

                    if (i == 0 && j == 0)
                        continue;

                    if (point.row + i < 0 || point.row + i > 15)
                        continue;

                    if (point.col + j < 0 || point.col + j > 15)
                        continue;

                    MinefieldCell currentCell = field[point.row + i, point.col + j];

                    if (currentCell.actualState == cellState.mine)
                    {
                        checkedNeigbors.Add(new Point { row = point.row + i, col = point.col + j });
                        continue;
                    }
                    else if (currentCell.neighborMines > 0)
                    {
                        currentCell.PublicState = cellState.none;
                        currentCell.Content = currentCell.neighborMines.ToString();
                        checkedNeigbors.Add(new Point { row = point.row + i, col = point.col + j });
                        continue;
                    }
                    else if (currentCell.neighborMines == 0)
                    {
                        currentCell.PublicState = cellState.none;
                        checkedNeigbors.Add(new Point { row = point.row + i, col = point.col + j });
                        currentCell.RevealNeighbors();
                    }
                }
            }
        }
    }
}
