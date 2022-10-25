using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AdaptiveCourseClient.Infrastructure;

namespace AdaptiveCourseClient.RenderObjects
{
    public class LogicElement : Element
    {
        public UIElementGroup? LogicBlock { get; set; }
        public Ellipse? OutputSnap { get; set; }
        public UIElementGroup? InputsSnap { get; set; }

        private Rectangle? _body;
        private Shape? _negativeOutputCircle;
        private List<ConnectionLine> _connectionLines = new List<ConnectionLine>();

        public static readonly int OutputCircleDiameter = 16;
        public static readonly int SnapCircleDiameter = 16;
        public static readonly int ContactWidth = 8;
        public readonly int BodyWidth = 50;
        public readonly int BodyHeight = 70;

        private static readonly int _bodyInitialX = 50;
        private static readonly int _bodyInitialY = 50;
        private static readonly int _inputsNumber = 2;

        public LogicElement(Canvas canvas) : base(canvas)
        {
            _canvas = canvas;
        }

        public void AddBlock()
        {
            LogicBlock = new UIElementGroup();
            InputsSnap = new UIElementGroup();

            // Main body creation
            _body = Figures.AddBody(BodyWidth, BodyHeight);
            _body.PreviewMouseDown += Body_PreviewMouseDown;
            Canvas.SetLeft(_body, _bodyInitialX);
            Canvas.SetTop(_body, _bodyInitialY);
            LogicBlock.Add(_body);
            _canvas.Children.Add(_body);

            // Inputs creation
            for (int i = 0; i < _inputsNumber; i++)
            {
                double relativeInputY = BodyHeight * ((double)(i + 1) / (_inputsNumber + 1));
                Line leftInputLine = Figures.AddContact();
                leftInputLine.X1 = _bodyInitialX - ContactWidth;
                leftInputLine.Y1 = _bodyInitialY + (relativeInputY);
                leftInputLine.X2 = _bodyInitialX;
                leftInputLine.Y2 = _bodyInitialY + (relativeInputY);
                LogicBlock.Add(leftInputLine);
                _canvas.Children.Add(leftInputLine);
            }

            // Output creation
            Line rightLine = Figures.AddContact();
            rightLine.X1 = _bodyInitialX + BodyWidth;
            rightLine.Y1 = _bodyInitialY + ((double)BodyHeight / 2);
            rightLine.X2 = _bodyInitialX + BodyWidth + ContactWidth;
            rightLine.Y2 = _bodyInitialY + ((double)BodyHeight / 2);
            LogicBlock.Add(rightLine);
            _canvas.Children.Add(rightLine);
            _negativeOutputCircle = null;

            AddOutputSnap();
            AddInputsSnap();
            AddOutputSnapColoringEvent();
        }

        public void RemoveBlock()
        {
            foreach(UIElement logicElementPart in LogicBlock)
            {
                _canvas.Children.Remove(logicElementPart);
            }
        }

        public void AddOutputSnap()
        {
            Ellipse rightInputSnap = Figures.AddInputSnapCircle(SnapCircleDiameter);
            Canvas.SetLeft(rightInputSnap, _bodyInitialX + BodyWidth);
            Canvas.SetTop(rightInputSnap, _bodyInitialY + (double)(BodyHeight / 2) -
                (double)(SnapCircleDiameter / 2));
            LogicBlock?.Add(rightInputSnap);
            _canvas.Children.Add(rightInputSnap);
            this.OutputSnap = rightInputSnap;
        }

        public void AddInputsSnap()
        {
            for(int i = 0; i < _inputsNumber; i++)
            {
                Ellipse leftInputSnap = Figures.AddInputSnapCircle(SnapCircleDiameter);
                Canvas.SetLeft(leftInputSnap, _bodyInitialX - SnapCircleDiameter);
                Canvas.SetTop(leftInputSnap, _bodyInitialY + BodyHeight * ((double)(i + 1) / (_inputsNumber + 1)) - 
                    (double)(SnapCircleDiameter / 2));
                LogicBlock.Add(leftInputSnap);
                _canvas.Children.Add(leftInputSnap);
                this.InputsSnap.Add(leftInputSnap);
            }
        }

        private void Body_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                if (_negativeOutputCircle != null)
                {
                    AddPositiveOutput((Ellipse)_negativeOutputCircle);
                }
                else
                {
                    AddNegativeOutput();
                }
            }
        }

        public void AddOutputSnapColoringEvent()
        {
            OutputSnap.MouseLeave += Output_MouseLeave;
            OutputSnap.MouseMove += Output_MouseMove;
        }

        public void RemoveOutputSnapColoringEvent()
        {
            OutputSnap.MouseLeave -= Output_MouseLeave;
            OutputSnap.MouseMove -= Output_MouseMove;
        }

        private void Output_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender == OutputSnap)
            {
                OutputSnap.Stroke = Brushes.Transparent;
            }
        }

        private void Output_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender == OutputSnap)
            {
                OutputSnap.Stroke = Brushes.Red;
            }
        }

        public void MoveLogicBlockEvents(MouseButtonEventHandler LogicElementAND_PreviewMouseLeftButtonDown,
            MouseEventHandler LogicElement_PreviewMouseMove, MouseButtonEventHandler LogicElement_PreviewMouseLeftButtonUp)
        {
            _body.PreviewMouseLeftButtonDown += LogicElementAND_PreviewMouseLeftButtonDown;
            _body.PreviewMouseMove += LogicElement_PreviewMouseMove;
            _body.PreviewMouseLeftButtonUp += LogicElement_PreviewMouseLeftButtonUp;
        }

        private void AddPositiveOutput(Ellipse rightEllipse)
        {
            _canvas?.Children.Remove(rightEllipse);
            LogicBlock?.Remove(rightEllipse);
            _negativeOutputCircle = null;
        }

        private void AddNegativeOutput()
        {
            double outputX = Canvas.GetLeft(_body) + BodyWidth;
            double outputY = Canvas.GetTop(_body) + ((double)BodyHeight / 2);
            Ellipse circle = Figures.AddOutputCircle(OutputCircleDiameter);
            Canvas.SetTop(circle, outputY - OutputCircleDiameter / 2);
            Canvas.SetLeft(circle, outputX - OutputCircleDiameter * 2 / 3);

            LogicBlock?.Add(circle);
            _negativeOutputCircle = circle;
            _canvas?.Children.Add(circle);
            foreach (UIElement uIElement in LogicBlock)
            {
                Panel.SetZIndex(uIElement, 0);
            }
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
            _connectionLines.Add(connectionLine);
        }

        public override void FindIntersections(ConnectionLine connectionLine)
        {
            if (connectionLine.BeginElement == this)
            {
                foreach (ConnectionLine _connectionLine in _connectionLines)
                {
                    if (_connectionLine.BeginElement == this)
                    {
                        Point intersectPoint = Helper.FindIntersectionPoint(connectionLine, _connectionLine);
                        if (intersectPoint.X != 0 && intersectPoint.Y != 0)
                        {
                            Node node = new Node(_canvas);
                            node.AddNode(intersectPoint);
                        }
                    }
                }
            }
            else if (connectionLine.EndElement == this)
            {
                foreach (ConnectionLine _connectionLine in _connectionLines)
                {
                    if (_connectionLine.EndElement == this)
                    {
                        Point intersectPoint = Helper.FindIntersectionPoint(connectionLine, _connectionLine);
                        if (intersectPoint.X != 0 && intersectPoint.Y != 0)
                        {
                            Node node = new Node(_canvas);
                            node.AddNode(intersectPoint);
                        }
                    }
                }
            }
        }

        public void MoveLogicElement(Point cursorPosition, Point _logicElementOffset)
        {
            double bodyX = Canvas.GetLeft(LogicBlock[0]);
            double bodyY = Canvas.GetTop(LogicBlock[0]);
            foreach (UIElement uIElement in LogicBlock)
            {
                double Y = cursorPosition.Y - _logicElementOffset.Y - bodyY;
                double X = cursorPosition.X - _logicElementOffset.X - bodyX;

                // it is an another way of line coordinates determination
                if (uIElement is Line)
                {
                    Line contact = (Line)uIElement;
                    double contactX = contact.X1;
                    double contactY = contact.Y1;
                    Y += contactY;
                    X += contactX;
                    foreach (ConnectionLine connectionLine in _connectionLines)
                        connectionLine.MoveConnectionLine(contact, X, Y);
                    contact.X1 = X;
                    contact.Y1 = Y;
                    contact.X2 = X + LogicElement.OutputCircleDiameter / 2;
                    contact.Y2 = Y;
                }
                else
                {
                    double positionX = Canvas.GetLeft(uIElement);
                    double positionY = Canvas.GetTop(uIElement);
                    Y += positionY;
                    X += positionX;
                    Canvas.SetTop(uIElement, Y);
                    Canvas.SetLeft(uIElement, X);
                }
            }
        }
    }
}
