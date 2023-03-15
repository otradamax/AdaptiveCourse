using AdaptiveCourseClient.RenderObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveCourseClient.Infrastructure
{
    public static class Helper
    {
        private static double _tolerance = 0.001;

        // Extension for comparing points with tolerance
        public static bool Equals(this Point point1, Point point2, double tolerance)
        {
            if (Math.Abs(point1.X - point2.X) < tolerance && Math.Abs(point1.Y - point2.Y) < tolerance)
                return true;
            else
                return false;
        }

        public static Point FindIntersectionPoint(ConnectionLine connectionLine1, ConnectionLine connectionLine2, bool isBegin)
        {
            PointCollection points1 = connectionLine1.ConnectionLinePolyline!.Points;
            PointCollection points2 = connectionLine2.ConnectionLinePolyline!.Points;

            if (!isBegin)
            {
                points1 = new PointCollection(points1.Reverse());
                points2 = new PointCollection(points2.Reverse());
            }

            int pointsNum = points1.Count < points2.Count ? points1.Count : points2.Count;
            for(int i = 1; i < pointsNum; i++)
            {
                if (Math.Abs(points1[i].X - points2[i].X) > _tolerance) {
                    double direction = (points1[i].X - points1[i - 1].X);
                    if (direction >= 0)
                    {
                        if (points1[i].X > points2[i].X)
                        {
                            return new Point(points2[i].X, points2[i].Y);
                        }
                        else
                        {
                            return new Point(points1[i].X, points1[i].Y);
                        }
                    }
                    else if (direction < 0)
                    {
                        if (points1[i].X > points2[i].X)
                        {
                            return new Point(points1[i].X, points1[i].Y);
                        }
                        else
                        {
                            return new Point(points2[i].X, points2[i].Y);
                        }
                    }
                }
                else if (Math.Abs(points1[i].Y - points2[i].Y) > _tolerance)
                {
                    double direction = (points1[i].Y - points1[i - 1].Y);
                    if ((points1[i].Y > points1[i - 1].Y) && (points2[i].Y < points2[i - 1].Y) || 
                        (points1[i].Y < points1[i - 1].Y) && (points2[i].Y > points2[i - 1].Y))
                    {
                        return new Point(points2[i].X, points2[i - 1].Y);
                    }
                    else if (direction >= 0)
                    {
                        if (points1[i].Y > points2[i].Y)
                        {
                            return new Point(points2[i].X, points2[i].Y);
                        }
                        else
                        {
                            return new Point(points1[i].X, points1[i].Y);
                        }
                    }
                    else if (direction < 0)
                    {
                        if (points1[i].Y > points2[i].Y)
                        {
                            return new Point(points1[i].X, points1[i].Y);
                        }
                        else
                        {
                            return new Point(points2[i].X, points2[i].Y);
                        }
                    }
                }
            }
            return new Point(0, 0);
        }

        public static void TruthTableInitialization(ref List<List<int>> _X, int _InputCount)
        {
            _X = new List<List<int>>();
            for (int i = 0; i < Math.Pow(_InputCount, 2); i++)
            {
                List<int> trueTableRow = new List<int>();
                string iDoubled = Convert.ToString(i, 2);
                for (int j = 0; j < _InputCount - iDoubled.Length; j++)
                {
                    trueTableRow.Add(0);
                }
                for (int j = 0; j < iDoubled.Length; j++)
                {
                    trueTableRow.Add(Convert.ToByte(iDoubled[j]) == 48 ? 0 : 1);
                }
                _X.Add(trueTableRow);
            }
        }

        private static string token = String.Empty;

        public static async Task<HttpResponseMessage> Request(HttpMethod method, string address, StringContent stringContent = null)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                using (HttpRequestMessage request = new HttpRequestMessage(method, address))
                {
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("Authorization", "Bearer " + token);
                    if (stringContent != null)
                    {
                        request.Content = stringContent;
                    }
                    var response = await httpClient.SendAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        AuthorizationWindow authorizationWindow = new AuthorizationWindow();
                        authorizationWindow.ShowDialog();
                        token = authorizationWindow.Token;
                        if (token != String.Empty)
                        {
                            response = await Request(method, address, stringContent);
                        }
                    }
                    else
                    {
                        return response;
                    }
                    return response;
                }
            }
            catch
            {
                throw new NullReferenceException();
            }
        }
    }
}
