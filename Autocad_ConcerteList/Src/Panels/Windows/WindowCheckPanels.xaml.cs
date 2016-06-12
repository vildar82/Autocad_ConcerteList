﻿using System;
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
using System.Windows.Shapes;

namespace Autocad_ConcerteList.Src.RegystryPanel.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowCheckPanels.xaml
    /// </summary>
    public partial class WindowCheckPanels : Window
    {
        public WindowCheckPanels(List<Panel> panels)
        {
            InitializeComponent();

            DataContext = new CheckPanelsViewModel(panels);
        }
    }
}