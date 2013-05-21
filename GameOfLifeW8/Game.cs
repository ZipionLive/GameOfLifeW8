using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeW8
{
    internal class Game : INotifyPropertyChanged
    {
        #region Events

        public delegate void DelegateGenerationComputed();
        public event DelegateGenerationComputed GenerationComputed;

        #endregion

        #region Fields

        //private static volatile Game instance;
        //private static object syncRoot = new Object();
        private bool[,] _gameOfLifeGrid;
        private bool[,] _nextGameOfLifeGrid;
        private bool _isRunning = false;

        #endregion

        #region Properties

        /// <summary>
        /// Our cell array
        /// </summary>
        public bool[,] GameOfLifeGrid
        {
            get { return _gameOfLifeGrid; }
            private set
            {
                if (_gameOfLifeGrid != value)
                {
                    _gameOfLifeGrid = value;
                    RaisePropertyChanged("CellTab");
                }
            }
        }

        /// <summary>
        /// Allows to check if the game logic is currently running
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            private set { this._isRunning = value; }
        }

        /// <summary>
        /// Gets the number of rows of the array
        /// </summary>
        private int RowLength
        {
            get { return GameOfLifeGrid.GetLength(0); }
        }

        /// <summary>
        /// Gets the number of columns of the array
        /// </summary>
        private int ColumnLength
        {
            get { return GameOfLifeGrid.GetLength(1); }
        }

        public int Generations { get; private set; }

        #endregion

        #region Constructor

        public Game(int tabLength, int tabHeight)
        {
            // Instantiation du tableau selon les coordonnées demandées
            GameOfLifeGrid = new bool[tabLength, tabHeight];
            _nextGameOfLifeGrid = new bool[tabLength, tabHeight];
            Generations = 0;

            InitializeToFalse(GameOfLifeGrid);
            InitializeToFalse(_nextGameOfLifeGrid);

            InitializeGlider(GameOfLifeGrid);
            RaisePropertyChanged("CellTab");

            if (GenerationComputed != null)
            {
                GenerationComputed();
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts the logic business
        /// </summary>
        public void Start()
        {
            this.IsRunning = true;
            Run();
        }

        /// <summary>
        /// Stops the logic business
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;
        }

        public void Reset()
        {
            Stop();
            InitializeToFalse(GameOfLifeGrid);
            InitializeToFalse(_nextGameOfLifeGrid);

            if (GenerationComputed != null)
                GenerationComputed();
        }

        #endregion

        #region Private Methods

        private void InitializeToFalse(bool[,] GameOfLifeGrid)
        {
            for (int row = 0; row < RowLength; row++)
            {
                for (int column = 0; column < ColumnLength; column++)
                {
                    GameOfLifeGrid[row, column] = false;
                }
            }
        }

        private void InitializeGlider(bool[,] GameOfLifeGrid)
        {
            GameOfLifeGrid[0, 1] = true;
            GameOfLifeGrid[1, 2] = true;
            GameOfLifeGrid[2, 0] = true;
            GameOfLifeGrid[2, 1] = true;
            GameOfLifeGrid[2, 2] = true;
        }

        #endregion

        #region Game Logic

        private async void Run()
        {
            while (IsRunning)
            {
                await Task.Delay(100);

                for (int row = 0; row < RowLength; row++)
                {
                    for (int column = 0; column < ColumnLength; column++)
                    {
                        CheckCellAndPlay(column, row);
                    }
                }

                Generations++;

                if (GenerationComputed != null)
                    GenerationComputed();

                GameOfLifeGrid = CopyGrid(_nextGameOfLifeGrid);
            }
        }

        private bool[,] CopyGrid(bool[,] Grid)
        {
            bool[,] NewGrid = new bool[RowLength, ColumnLength];

            for (int row = 0; row < RowLength; row++)
            {
                for (int column = 0; column < ColumnLength; column++)
                    NewGrid[row, column] = Grid[row, column];
            }

            return NewGrid;
        }

        private void CheckCellAndPlay(int column, int row)
        {
            int surroundingPopulation = 0;

            try
            {
                for (int i = row - 1; i <= row + 1; i++)
                {
                    for (int j = column - 1; j <= column + 1; j++)
                    {
                        if (!(i == row && j == column) && IsValid(i, j)) // Checking that we don't count the current cell in or get out of the grid's range
                        {
                            if (GameOfLifeGrid[i, j] == true) // Checking if there is population around
                                surroundingPopulation++;
                        }
                    }
                }
            }

            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException("Invalid Range Values.\n" + ex.ToString());
            }

            // Applying the game rules !
            if (GameOfLifeGrid[column, row]) // Rules for living cells
            {
                if (surroundingPopulation > 1 || surroundingPopulation < 4)
                    _nextGameOfLifeGrid[column, row] = true; // Ah ah ah ah stayin' alive ! Stayin' alive !
                else
                    _nextGameOfLifeGrid[column, row] = false; // Cells hate crowds
            }
            else // Rules for dead cells
            {
                if (surroundingPopulation == 3)
                    _nextGameOfLifeGrid[column, row] = true; // Repopulate ! Orgy time !
                else
                    _nextGameOfLifeGrid[column, row] = false; // No zombies allowed, please stay dead.
            }
        }

        private bool IsValid(int row, int column)
        {
            return row >= 0 && column >= 0 &&
                row < RowLength && column < ColumnLength;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Trash
        //public static Game Instance(int tabLength, int tabHeight) {
        //    get {
        //        if (instance == null) {
        //            lock (syncRoot) {
        //                if (instance == null)
        //                    instance = new Game(tabLength, tabHeight);
        //            }
        //        }
        //    }

        //    return instance;
        //}
        #endregion
    }
}