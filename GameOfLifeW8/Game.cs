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

        #region Attributes
        //private readonly int tabLength { get; }
        //private readonly int tabHeight { get; }
        //private static volatile Game instance;
        //private static object syncRoot = new Object();
        private bool[,] _cellTab;
        #endregion

        #region Properties
        // Tableau des cellules
        public bool[,] CellTab
        {
            get
            {
                return _cellTab;
            }
            private set
            {
                if (_cellTab != value)
                {
                    _cellTab = value;
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
                return CellTab.GetLength(0);
            }
        }

        /// <summary>
        /// Gets the number of columns of the array
        /// </summary>
        private int ColumnLength
        {
            get
            {
                return CellTab.GetLength(1);
            }
        }
        #endregion

        #region Constructor
        public Game(int tabLength, int tabHeight)
        {
            // Instantiation du tableau selon les coordonnées demandées
            CellTab = new bool[tabLength, tabHeight];
            InitializeToFalse(CellTab);
            InitializeGlider(CellTab);
            if (GenerationComputed != null)
            {
                GenerationComputed();
            }
        }

        private void InitializeToFalse(bool[,] CellTab)
        {
            for (int row = 0; row < RowLength; row++)
            {
                for (int column = 0; column < ColumnLength; column++)
                {
                    CellTab[row, column] = false;
                }
            }
        }

        private void InitializeGlider(bool[,] CellTab)
        {
            CellTab[0, 1] = true;
            CellTab[1, 2] = true;
            CellTab[2, 0] = true;
            CellTab[2, 1] = true;
            CellTab[2, 2] = true;
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
        public async void Start()
        {
            this.IsRunning = true;

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
                        // ancien test
                        // SI la cellule n'est PAS à la même colonne ET
                        // SI la cellule n'est PAS à la même ligne ET
                        // Si la cellule est valide
                        //if (i != row && j != column && IsValid(i, j))


                        // nouveau test
                        // SI la cellule (N'EST PAS A LA MÊME COLONNE ET LA MÊME LIGNE) ET
                        // SI la cellule est valide
                        if(!(i == row && j == column) && IsValid(i, j))
                        {
                            if (CellTab[i, j] == true) { surroundingPopulation++; }
                        }
                    }
                }
            }

            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException("Invalid Range Values.\n" + ex.ToString());
            }

            if (CellTab[column, row])
            {
                if (surroundingPopulation <= 1 || surroundingPopulation > 3)
                    CellTab[column, row] = false;
            }
            else
            {
                if (surroundingPopulation == 3) // Test nécessaire ???
                    CellTab[column, row] = true;
            }
        }

        private bool IsValid(int row, int column)
        {
            return row >= 0 && column >= 0 &&
                row < RowLength - 1 && column < ColumnLength - 1;
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