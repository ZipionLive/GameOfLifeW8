using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace GameOfLifeW8
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Game game;
        List<Cell> cells;

        public MainPage()
        {
            this.InitializeComponent();
            cells = new List<Cell>();
        }

        private void GenerateCells(int rows, int columns)
        {
            for (int row = 0; row < rows; row++)
            {
                StackPanel spRow = new StackPanel();
                spRow.Orientation = Orientation.Horizontal;

                for (int column = 0; column < columns; column++)
                {
                    Cell cell = new Cell(column, row);
                    spRow.Children.Add(cell);
                    cells.Add(cell);
                }

                spGlobal.Children.Add(spRow);
            }
        }

        private void GenerationComputedHandler()
        {
            RefreshCells();
        }

        public void RefreshCells()
        {
            foreach (Cell cell in cells)
            {
                if (cell.alive)
                {
                    if (!game.GameOfLifeGrid[cell.row, cell.column])
                    {
                        cell.Die();
                    }
                }
                else
                {
                    if (game.GameOfLifeGrid[cell.row, cell.column])
                    {
                        cell.Live();
                    }
                }
            }
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page. La propriété Parameter
        /// est généralement utilisée pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            int columns;
            int rows;
            if (int.TryParse(tbxColumns.Text, out columns) && int.TryParse(tbxRows.Text, out rows))
            {
                if (columns <= 50 && rows <= 30 && columns > 0 && rows > 0)
                {
                    game = new Game(columns, rows);
                    GenerateCells(rows, columns);
                    btnGameStart.Visibility = Visibility.Visible;
                    btnGameStop.Visibility = Visibility.Visible;
                    btnReset.Visibility = Visibility.Visible;
                    game.GenerationComputed += GenerationComputedHandler;
                }
                else
                {
                    throw new InvalidCastException("Bad Values !");
                }
            }
            else
            {
                throw new InvalidCastException("Bad Values !");
            }
        }

        private void btnGameStart_Click(object sender, RoutedEventArgs e)
        {
            game.Start();
        }

        private void btnGameStop_Click(object sender, RoutedEventArgs e)
        {
            game.Stop();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #region CellTest
        //private void btnKillCell_Click(object sender, RoutedEventArgs e)
        //{
        //    myCell.Die();
        //}

        //private void btnReanimateCell_Click(object sender, RoutedEventArgs e)
        //{
        //    myCell.Live();
        //}
        #endregion
    }
}
