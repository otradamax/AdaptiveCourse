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
        private static SchemeTask _task = new SchemeTask();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        public async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await GetTask();

            this.Visibility = Visibility.Collapsed;

            SchemeWindow schemeWindow = new SchemeWindow(_task);
            schemeWindow.ShowDialog();
        }

        private async Task GetTask()
        {
            try
            {
                HttpResponseMessage response = await Helper.Request(HttpMethod.Get, "https://localhost:7133/Home/GetTask");
                string result = await response.Content.ReadAsStringAsync();
                _task = JsonConvert.DeserializeObject<SchemeTask>(result);
                Element.ContactNumberMax = _task.ContactsNumberMax;
            }
            catch
            {

            }
        }
    }
}
