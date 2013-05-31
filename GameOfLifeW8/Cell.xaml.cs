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

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page http://go.microsoft.com/fwlink/?LinkId=234236

namespace GameOfLifeW8
{
    public sealed partial class Cell : UserControl
    {
        #region Events

        public delegate void DelegateCellTouched(int row, int column);
        public event DelegateCellTouched CellTouched;

        #endregion

        #region Properties

        public bool alive { get; private set; }
        public int row { get; private set; }
        public int column { get; private set; }
        private bool isPointerPressed { get; set; }

        #endregion

        #region Constructor

        public Cell(int row, int column)
        {
            this.row = row;
            this.column = column;
            this.alive = false;
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        public void Live()
        {
            this.alive = true;
            VisualStateManager.GoToState(this, "Alive", true);
        }

        public void Die()
        {
            this.alive = false;
            VisualStateManager.GoToState(this, "Dead", true);
        }

        public void Toggle()
        {
            if (this.alive)
                this.Die();
            else
                this.Live();
        }

        #endregion

        #region EventHandlers

        private void CellBase_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (isPointerPressed)
            {
                if (CellTouched != null)
                    CellTouched(this.row, this.column);

                this.Toggle();
            }
        }

        private void CellBase_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (CellTouched != null)
                CellTouched(this.row, this.column);

            this.Toggle();
        }

        public void GridPointerPressedHandler()
        {
            isPointerPressed = true;
        }

        public void GridPointerReleasedOrLostHandler()
        {
            isPointerPressed = false;
        }

        #endregion
    }
}
