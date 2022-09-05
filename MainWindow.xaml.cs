using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpfMinesweeper.Model;

namespace wpfMinesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MinefieldCell[,] field = new MinefieldCell[16, 16];
        public List<Model.Point> checkedPoints = new List<Model.Point>();

        public MainWindow()
        {
            InitializeComponent();
            SetupField();
        }

        private void SetupField()
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    MinefieldCell m = new MinefieldCell(i, j);
                    field[i, j] = m;
                    Grid.SetColumn(m, j);
                    Grid.SetRow(m, i);
                    panelBody.Children.Add(m);
                }
            }

            foreach (MinefieldCell m in field)
            {
                m._mainWindow = this;
                m.field = field;
                m.checkedNeigbors = checkedPoints;
            }

            int count = 0;
            Random rand = new Random();

            while (count < 50)
            {
                int row = rand.Next(0, 16), col = rand.Next(0, 16);
                if (field[row, col].ActualState == cellState.none)
                {
                    field[row, col].ActualState = cellState.mine;
                    count++;
                }
            }

            foreach (MinefieldCell m in field)
            {
                m.CheckNeighbors();
            }

            headerFlagsLeft.Text = "50";
        }

        public void Reset()
        {
            InitializeComponent();
            SetupField();
        }
    }
}
