﻿using AdaptiveCourseClient.Infrastructure;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class InputElement : IOElement
    {
        public Polygon Input;
        public int Xn;
        private int _inputsNum;
        private TextBlock _textBlock;

        public InputElement(Canvas canvas, int inputsNum) 
            : base(canvas) 
        {
            _inputsNum = inputsNum;
        }
        
        public void AddInput(int i, double elementInitialX, double elementInitialY, double elementInitialWidth)
        {
            // Add text
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "X" + i;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.FontSize = _textSize;
            textBlock.FontStyle = FontStyles.Italic;
            Canvas.SetLeft(textBlock, elementInitialX + 2);
            Canvas.SetTop(textBlock, elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) - _contactWidth / 2);
            _textBlock = textBlock;
            _canvas.Children.Add(textBlock);

            Input = new Polygon();
            Input.Fill = Brushes.Transparent;
            Input.Stroke = Brushes.Black;
            Input.StrokeThickness = 3;

            // Creating a triangle input
            PointCollection inputPoints = new PointCollection();
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) - _contactWidth));
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) + _contactWidth));
            inputPoints.Add(new Point(elementInitialX + elementInitialWidth,
                elementInitialY * ((double)(i + 1) / (_inputsNum + 1))));

            Input.Points = inputPoints;
            Input.MouseMove += Input_MouseMove;
            Input.MouseLeave += Input_MouseLeave;

            // Index number
            Xn = i;

            _canvas.Children.Add(Input);
        }

        public void ChangeLocation(double elementInitialX, double elementInitialY, double elementInitialWidth)
        {
            Canvas.SetLeft(_textBlock, elementInitialX + 2);
            Canvas.SetTop(_textBlock, elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1)) - _contactWidth / 2);

            Input.Points.Clear();
            PointCollection inputPoints = new PointCollection();
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1)) - _contactWidth));
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1)) + _contactWidth));
            inputPoints.Add(new Point(elementInitialX + elementInitialWidth,
                elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1))));
            Input.Points = inputPoints;
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
            _connectionLines.Add(connectionLine);
        }

        public override void CreateNodes(ConnectionLine connectionLine)
        {
            foreach (ConnectionLine _connectionLine in _connectionLines)
            {
                if (_connectionLine.BeginElement == this && _connectionLine != connectionLine)
                {
                    Point intersectPoint = Helper.FindIntersectionPoint(connectionLine, _connectionLine, true);
                    if (intersectPoint.X != 0 && intersectPoint.Y != 0)
                    {
                        Node node = new Node(_canvas);
                        node.AddNode(intersectPoint);
                        connectionLine.AddNode(node);
                        _connectionLine.AddNode(node);
                    }
                }
            }
        }

        public void AddColoringEvent()
        {
            Input.MouseMove += Input_MouseMove;
            Input.MouseLeave += Input_MouseLeave;
        }

        public void RemoveColoringEvent()
        {
            Input.MouseMove -= Input_MouseMove;
            Input.MouseLeave -= Input_MouseLeave;
        }
    }
}
