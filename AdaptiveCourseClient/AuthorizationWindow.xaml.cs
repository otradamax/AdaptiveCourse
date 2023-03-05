using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using AdaptiveCourseClient.RenderObjects;

namespace AdaptiveCourseClient
{
    public partial class AuthorizationWindow : Window
    {
        public string Token = String.Empty;

        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private async void btnLogin_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7133/Authorize/Login"))
            {
                string authorization = txtLogin.Text + ":" + txtPassword.Password.ToString();
                byte[] encodedAuthorization = Encoding.GetEncoding("ISO-8859-1").GetBytes(authorization);
                string a = Convert.ToBase64String(encodedAuthorization);
                request.Headers.Add("Authorization", "Authorization Basic " + a);
                var response = await httpClient.SendAsync(request);
                Token = await response.Content.ReadAsStringAsync();
            }
            this.Close();
        }

        private void btnExit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_PreviewMouseLeftButtonDown(sender, null);
            }
        }
    }
}
