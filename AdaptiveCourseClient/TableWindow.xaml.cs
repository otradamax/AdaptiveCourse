using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using AdaptiveCourseClient.Infrastructure;
using AdaptiveCourseClient.Models;
using AdaptiveCourseClient.RenderObjects;
using System.Drawing;
using Newtonsoft.Json;
using Image = System.Drawing.Image;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace AdaptiveCourseClient
{
    public partial class TableWindow : Window
    {
        private List<Data> _datas = new List<Data>();
        private List<string> _headerNames;
        private List<List<int>> _X;
        private TableTask _task;

        public TableWindow(TableTask tableTask)
        {
            InitializeComponent();

            _task = tableTask;

            _headerNames = new List<string>();
            for (int i = 0; i < _task.InputsNumber; i++)
            {
                _headerNames.Add("X" + i.ToString());
            }
            _headerNames.Add("Y");

            Helper.TruthTableInitialization(ref _X, _task.InputsNumber);

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
            column0.Width = new GridLength(btnCheckScheme.ActualWidth / 2, GridUnitType.Pixel);
            column1.Width = new GridLength(btnCheckScheme.ActualWidth / 2, GridUnitType.Pixel);
            truthTable.Width = btnCheckScheme.ActualWidth / 2;
            double height = (tableWindow.ActualHeight - btnCheckScheme.ActualHeight) / (Math.Pow(_task.InputsNumber, 2) + 2);
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
                if (_task.InputsNumber == 4)
                {
                    _datas.Add(new Data4( _X[i][0], _X[i][1], _X[i][2], _X[i][3], 0 ));
                }
                else if (_task.InputsNumber == 3)
                {
                    _datas.Add(new Data3(_X[i][0], _X[i][1], _X[i][2], 0));
                }
                else if (_task.InputsNumber == 2)
                {
                    _datas.Add(new Data2(_X[i][0], _X[i][1], 0));
                }
            }
            truthTable.ItemsSource = _datas;

            truthTable.ColumnHeaderHeight = height;
            truthTable.RowHeight = height;

            txtTask.Margin = new Thickness(txtTask.ActualWidth / 50);
            txtTask.Text = "На основе приведенной логической электрической схемы определите выход таблицы истинности";

            using (MemoryStream ms = new MemoryStream(_task.SchemeImage))
            {
                Image image = Image.FromStream(ms);
                image.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                imgScheme.Source = bitmapImage;
            }
        }

        private async void btnCheckScheme_Click(object sender, RoutedEventArgs e)
        {
            string result = String.Empty;
            for(int i = 0; i < Math.Pow(2, _task.InputsNumber); i++)
            {
                TextBlock cellText = truthTable.Columns[_task.InputsNumber].GetCellContent(truthTable.Items[i]) as TextBlock;
                result += cellText.Text.Trim();
            }

            await CheckTable(result);
        }

        private async Task CheckTable(string result)
        {
            try
            {
                CheckTable checkTable = new CheckTable(result, _task.Id);
                var json = JsonConvert.SerializeObject(checkTable);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helper.Request(HttpMethod.Post, "https://localhost:7133/Home/LogicTable", stringContent);
                string responseText = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseText, "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {

            }
        }
    }
}
