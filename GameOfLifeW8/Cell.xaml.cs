﻿using System;
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
        public bool alive { get; private set; }
        public int row { get; private set; }
        public int column { get; private set; }

        public Cell(int posX, int posY)
        {
            this.row = posX;
            this.column = posY;
            this.alive = false;
            this.InitializeComponent();
        }

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
    }
}