using AdaptiveCourseClient.RenderObjects;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
            PointCollection points1 = connectionLine1.ConnectionLinePolyline.Points;
            PointCollection points2 = connectionLine2.ConnectionLinePolyline.Points;

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
    }
}
