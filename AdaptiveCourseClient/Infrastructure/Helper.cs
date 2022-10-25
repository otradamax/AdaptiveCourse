using AdaptiveCourseClient.RenderObjects;
using System;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveCourseClient.Infrastructure
{
    public static class Helper
    {
        private static double _tolerance = 0.001;

        public static Point FindIntersectionPoint(ConnectionLine connectionLine1, ConnectionLine connectionLine2)
        {
            PointCollection points1 = connectionLine1.ConnectionLinePolyline.Points;
            PointCollection points2 = connectionLine2.ConnectionLinePolyline.Points;
            int pointsNum = points1.Count < points2.Count ? points1.Count : points2.Count;
            for(int i = 1; i < pointsNum; i++)
            {
                if (Math.Abs(points1[i].X - points2[i].X) > _tolerance) {
                    double direction = (points1[i].X - points1[i - 1].X);
                    if (direction > 0)
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
                    if (direction > 0)
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
    }
}
