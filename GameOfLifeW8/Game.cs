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
        //private readonly int tabLength { get; }
        //private readonly int tabHeight { get; }
        //private static volatile Game instance;
        //private static object syncRoot = new Object();
        private bool[,] _gameOfLifeGrid;
        private bool[,] _nextGameOfLifeGrid;
        #endregion

        #region Properties
        // Tableau des cellules
        public bool[,] GameOfLifeGrid
        {
            get
            {
                return _gameOfLifeGrid;
            }
            private set
            {
                if (_gameOfLifeGrid != value)
                {
                    _gameOfLifeGrid = value;
                    RaisePropertyChanged("CellTab");
                }
            }
        }

        // TODO: Write a kill method to interrupt process
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the number of rows of the array
        /// </summary>
        private int RowLength
        {
            get
            {
                return GameOfLifeGrid.GetLength(0);
            }
        }

        /// <summary>
        /// Gets the number of columns of the array
        /// </summary>
        private int ColumnLength
        {
            get
            {
                return GameOfLifeGrid.GetLength(1);
            }
        }
        #endregion

        #region Constructor
        public Game(int tabLength, int tabHeight)
        {
            // Instantiation du tableau selon les coordonnées demandées
            GameOfLifeGrid = new bool[tabLength, tabHeight];
            _nextGameOfLifeGrid = new bool[tabLength, tabHeight];

            InitializeToFalse(GameOfLifeGrid);
            InitializeToFalse(_nextGameOfLifeGrid);

            InitializeGlider(GameOfLifeGrid);
            RaisePropertyChanged("CellTab");

            if (GenerationComputed != null)
            {
                GenerationComputed();
            }
        }

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

        /// <summary>
        /// Starts the logic business
        /// </summary>
        public void Start()
        {
            this.IsRunning = true;
            Run();
        }

        public async void Run()
        {
            while (IsRunning)
            {
                for (int row = 0; row < RowLength; row++)
                {
                    for (int column = 0; column < ColumnLength; column++)
                    {
                        CheckCellAndPlay(column, row);
                    }
                }

                await Task.Delay(100);

                if (GenerationComputed != null)
                {
                    GenerationComputed();
                }

                GameOfLifeGrid = _nextGameOfLifeGrid;
                InitializeToFalse(_nextGameOfLifeGrid);
            }
        }

        public void Stop()
        {
            this.IsRunning = false;
        }

        private void CheckCellAndPlay(int column, int row)
        {
            int surroundingPopulation = 0;

            try
            {
                //CheckCellAndPlay(column, row);
                for (int i = row - 1; i <= row + 1; i++)
                {
                    for (int j = column - 1; j <= column + 1; j++)
                    {
                        // SI la cellule (N'EST PAS A LA MÊME COLONNE ET LA MÊME LIGNE) ET
                        // SI la cellule est valide
                        if (!(i == row && j == column) && IsValid(i, j))
                        {
                            if (GameOfLifeGrid[i, j] == true) { surroundingPopulation++; }
                        }
                    }
                }
            }

            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException("Invalid Range Values.\n" + ex.ToString());
            }

            //if (CellTab[column, row])
            //{
            if (surroundingPopulation <= 1 || surroundingPopulation > 3)
            {
                _nextGameOfLifeGrid[column, row] = false;
            }
            else
            {
                //if (surroundingPopulation == 3) // Test nécessaire ???
                _nextGameOfLifeGrid[column, row] = true;
            }
        }

        private bool IsValid(int row, int column)
        {
            return row >= 0 && column >= 0 &&
                row < RowLength && column < ColumnLength;
        }

        #region INotifyPropertyChanged Membres

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}