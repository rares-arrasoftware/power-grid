﻿using gui.ViewModel.Resupply;
using System.Windows.Controls;

namespace gui.View.Resupply
{
    /// <summary>
    /// Interaction logic for RessuplyPanel.xaml
    /// </summary>
    public partial class ResupplyPanel : UserControl
    {
        public ResupplyPanel()
        {
            InitializeComponent();
            DataContext = new ResupplyViewModel(); // Bind ViewModel 
        }
    }
}
