using AdaptiveCourseClient.Infrastructure;
using AdaptiveCourseClient.Models;
using AdaptiveCourseClient.RenderObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace AdaptiveCourseClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static SchemeTask _schemeTask = new SchemeTask();
        private static TableTask _tableTask = new TableTask();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        public async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool isScheme = true;
            if (isScheme)
            {
                await GetSchemeTask();

                this.Visibility = Visibility.Collapsed;

                SchemeWindow schemeWindow = new SchemeWindow(_schemeTask);
                schemeWindow.ShowDialog();
            }
            else
            {
                await GetTableTask();

                this.Visibility = Visibility.Collapsed;

                TableWindow tableWindow = new TableWindow(_tableTask);
                tableWindow.ShowDialog();
            }
        }

        private async Task GetSchemeTask()
        {
            try
            {
                HttpResponseMessage response = await Helper.Request(HttpMethod.Get, "https://localhost:7133/Home/GetSchemeTask");
                string result = await response.Content.ReadAsStringAsync();
                _schemeTask = JsonConvert.DeserializeObject<SchemeTask>(result);
                Element.ContactNumberMax = _schemeTask.ContactsNumberMax;
            }
            catch
            {

            }
        }

        private async Task GetTableTask()
        {
            try
            {
                HttpResponseMessage response = await Helper.Request(HttpMethod.Get, "https://localhost:7133/Home/GetTableTask");
                string result = await response.Content.ReadAsStringAsync();
                _tableTask = JsonConvert.DeserializeObject<TableTask>(result);
            }
            catch
            {

            }
        }
    }
}
