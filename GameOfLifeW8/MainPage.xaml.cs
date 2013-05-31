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
        #region Events

        public delegate void DelegateGridPointerPressed();
        public delegate void DelegateGridPointerReleasedOrLost();
        public event DelegateGridPointerPressed GridPointerPressed;
        public event DelegateGridPointerReleasedOrLost GridPointerReleasedOrLost;
        #endregion

        #region Fields

        Game game;
        List<Cell> cells;
        int maxRows;
        int maxColumns;
        
        #endregion

        #region Constructor

        public MainPage()
        {
            this.InitializeComponent();
            cells = new List<Cell>();
        }

        #endregion

        #region Methods

        private void Generate(int rows, int columns)
        {
            // Clear any preexisting grid
            game = null;
            spGlobal.Children.Clear();

            // Generate new grid
            game = new Game(rows, columns);
            GenerateCells(rows, columns);
            RefreshCells();

            // Register to the game events
            game.GenerationComputed += GenerationComputedHandler;

            // Display control buttons
            btnGameStart.Visibility = Visibility.Visible;
            btnReset.Visibility = Visibility.Visible;
            tbGenerationsLabel.Visibility = Visibility.Visible;
            tbGenerations.Visibility = Visibility.Visible;

        }

        private void GenerateCells(int rows, int columns)
        {
            for (int row = 0; row < rows; row++)
            {
                StackPanel spRow = new StackPanel();
                spRow.Orientation = Orientation.Horizontal;

                for (int column = 0; column < columns; column++)
                {
                    Cell cell = new Cell(row, column);
                    spRow.Children.Add(cell);
                    cells.Add(cell);
                    cell.Die();

                    cell.CellTouched += game.CellTouchedHandler;
                    GridPointerPressed += cell.GridPointerPressedHandler;
                    GridPointerReleasedOrLost += cell.GridPointerReleasedOrLostHandler;
                }

                spGlobal.Children.Add(spRow);
            }
        }

        public void RefreshCells()
        {
            foreach (Cell cell in cells)
            {
                bool isAlive = game.GameOfLifeGrid[cell.row, cell.column];

                if (cell.alive)
                {
                    if (!isAlive)
                    {
                        cell.Die();
                    }
                }
                else if (!cell.alive)
                {
                    if (isAlive)
                    {
                        cell.Live();
                    }
                }
            }
        }

        #endregion

        #region User Interaction Handlers

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            maxRows = (int)UsableSpace.ActualHeight / 29;
            maxColumns = (int)UsableSpace.ActualWidth / 30;

            int columns;
            int rows;
            bool canGenerate = true;

            if (tbxRows.Text == String.Empty)
                rows = maxRows;
            else
            {
                if (int.TryParse(tbxRows.Text, out rows))
                {
                    if (rows > 0 && rows <= maxRows)
                    { /* Nothing to do here*/ }
                    else
                    {
                        tbxRows.Text = "Bad Value";
                        canGenerate = false;
                    }
                }
                else
                {
                    tbxRows.Text = "Bad Value";
                    canGenerate = false;
                }
            }

            if (tbxColumns.Text == String.Empty)
                columns = maxColumns;
            else
            {
                if (int.TryParse(tbxColumns.Text, out columns))
                {
                    if (columns > 0 && columns <= maxColumns)
                    { /* Nothing to do here*/ }
                    else
                    {
                        tbxRows.Text = "Bad Value";
                        canGenerate = false;
                    }
                }
                else
                {
                    tbxRows.Text = "Bad Value";
                    canGenerate = false;
                }
            }

            if (canGenerate)
            {
                Generate(rows, columns);
            }
        }

        private void btnGameStart_Click(object sender, RoutedEventArgs e)
        {
            game.Start();
            btnGameStart.Visibility = Visibility.Collapsed;
            btnGamePause.Visibility = Visibility.Visible;
        }

        private void btnGamePause_Click(object sender, RoutedEventArgs e)
        {
            game.Pause();
            btnGamePause.Visibility = Visibility.Collapsed;
            btnGameStart.Visibility = Visibility.Visible;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            game.Reset();
        }

        private void UsableSpace_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (GridPointerPressed != null)
                GridPointerPressed();
        }

        private void UsableSpace_PointerReleasedOrLost(object sender, PointerRoutedEventArgs e)
        {
            if (GridPointerReleasedOrLost != null)
                GridPointerReleasedOrLost();
        }

        #endregion

        #region Event Handlers

        public void GenerationComputedHandler()
        {
            RefreshCells();
            tbGenerations.Text = game.Generations.ToString();
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page. La propriété Parameter
        /// est généralement utilisée pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #endregion
    }
}
