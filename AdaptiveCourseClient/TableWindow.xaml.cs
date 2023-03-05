using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using AdaptiveCourseClient.RenderObjects;

namespace AdaptiveCourseClient
{
    public partial class TableWindow : Window
    {
        private List<Data> _datas = new List<Data>();
        private List<string> _headerNames = new List<string>() { "X0", "X1", "X2", "X3", "Y" };
        private int[,] _X = new int[,]
        {
            { 0, 0, 0, 0 },
            { 0, 0, 0, 1 },
            { 0, 0, 1, 0 },
            { 0, 0, 1, 1 },
            { 0, 1, 0, 0 },
            { 0, 1, 0, 1 },
            { 0, 1, 1, 0 },
            { 0, 1, 1, 1 },
            { 1, 0, 0, 0 },
            { 1, 0, 0, 1 },
            { 1, 0, 1, 0 },
            { 1, 0, 1, 1 },
            { 1, 1, 0, 0 },
            { 1, 1, 0, 1 },
            { 1, 1, 1, 0 },
            { 1, 1, 1, 1 }
        };


        public TableWindow()
        {
            InitializeComponent();
            Loaded += TableWindow_Loaded;
        }

        public class Data
        {
            public int X0 { get; set; }
            public int X1 { get; set; }
            public int X2 { get; set; }
            public int X3 { get; set; }
            public int Y { get; set; }
        }

        private void TableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            truthTable.Width = btnCheckScheme.ActualWidth;
            double height = (tableWindow.ActualHeight - btnCheckScheme.ActualHeight) / 18;
            foreach (string headerName in _headerNames)
            {
                var column = new DataGridTextColumn();
                column.Header = headerName;
                column.Binding = new Binding(headerName);
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                if (headerName != "Y")
                {
                    column.IsReadOnly = true;
                }
                truthTable.Columns.Add(column);
            }

            for (int i = 0; i < _X.GetLength(0); i++)
            {
                _datas.Add(new Data { X0 = _X[i, 0], X1 = _X[i, 1], X2 = _X[i, 2], X3 = _X[i, 3], Y = 0 });
            }
            truthTable.ItemsSource = _datas;

            truthTable.ColumnHeaderHeight = height;
            truthTable.RowHeight = height;
        }

        private void btnCheckScheme_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
