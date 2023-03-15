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
using AdaptiveCourseClient.Infrastructure;
using AdaptiveCourseClient.RenderObjects;

namespace AdaptiveCourseClient
{
    public partial class TableWindow : Window
    {
        private int _InputCount = 2;
        private List<Data> _datas = new List<Data>();
        private List<string> _headerNames;
        private List<List<int>> _X;


        public TableWindow()
        {
            InitializeComponent();

            _headerNames = new List<string>();
            for (int i = 0; i < _InputCount; i++)
            {
                _headerNames.Add("X" + i.ToString());
            }
            _headerNames.Add("Y");

            Helper.TruthTableInitialization(ref _X, _InputCount);

            Loaded += TableWindow_Loaded;
        }

        public class Data
        {
        }

        public class Data4 : Data
        {
            public int X0 { get; set; }
            public int X1 { get; set; }
            public int X2 { get; set; }
            public int X3 { get; set; }
            public int Y { get; set; }
            public Data4(int x0, int x1, int x2, int x3, int y)
            {
                X0 = x0;
                X1 = x1;
                X2 = x2;
                X3 = x3;
                Y = y;
            }
        }

        public class Data3 : Data
        {
            public int X0 { get; set; }
            public int X1 { get; set; }
            public int X2 { get; set; }
            public int Y { get; set; }
            public Data3(int x0, int x1, int x2, int y)
            {
                X0 = x0;
                X1 = x1;
                X2 = x2;
                Y = y;
            }
        }

        public class Data2 : Data
        {
            public int X0 { get; set; }
            public int X1 { get; set; }
            public int Y { get; set; }
            public Data2(int x0, int x1, int y)
            {
                X0 = x0;
                X1 = x1;
                Y = y;
            }
        }

        private void TableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            truthTable.Width = btnCheckScheme.ActualWidth;
            double height = (tableWindow.ActualHeight - btnCheckScheme.ActualHeight) / (Math.Pow(_InputCount, 2) + 2);
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

            for (int i = 0; i < _X.Count; i++)
            {
                if (_InputCount == 4)
                {
                    _datas.Add(new Data4( _X[i][0], _X[i][1], _X[i][2], _X[i][3], 0 ));
                }
                else if (_InputCount == 3)
                {
                    _datas.Add(new Data3(_X[i][0], _X[i][1], _X[i][2], 0));
                }
                else if (_InputCount == 2)
                {
                    _datas.Add(new Data2(_X[i][0], _X[i][1], 0));
                }
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
