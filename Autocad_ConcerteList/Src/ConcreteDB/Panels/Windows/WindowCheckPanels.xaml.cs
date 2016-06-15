﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowCheckPanels.xaml
    /// </summary>
    public partial class WindowCheckPanels : Window
    {
        public WindowCheckPanels(Panel panel)
        {
            InitializeComponent();
            Title = "Проверка панели";
            var dataModel = new CheckPanelsViewModel(panel);
            DataContext = dataModel;
        }

        public WindowCheckPanels(List<KeyValuePair<Panel, List<Panel>>> checkPanels, string title)
        {            
            InitializeComponent();
            Title = title;
            var dataModel = new CheckPanelsViewModel(checkPanels);
            DataContext = dataModel;            
        }        
    }
}
