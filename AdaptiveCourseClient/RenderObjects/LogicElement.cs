using System;
using System.Collections.Generic;
using System.Linq;
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

        public UIElement Output { get; set; }
        public UIElementGroup? Inputs { get; set; }

        private Rectangle? _body;
        private Shape? _negativeOutputCircle;        private List<Shape> _negativeInputsCircle;

        public static readonly int NegativeCircleDiameter = 24;
        public static readonly int SnapCircleDiameter = 24;
        public static readonly int ContactWidth = 12;
        public readonly int BodyWidth = 70;
        public int BodyHeight = 100;

        private readonly int _bodyInitialX = 100;
        private readonly int _bodyInitialY = 50;
        private int _inputsNumber = 2;

        public LogicElement(Canvas canvas) : base(canvas)
        {
            _canvas = canvas;
        }

        public void AddBlock(int serialNumber)
        {
            LogicBlock = new UIElementGroup();
            InputsSnap = new UIElementGroup();
            Inputs = new UIElementGroup();

            // Main body creation
            _body = Figures.AddBody(BodyWidth, BodyHeight);
            Canvas.SetLeft(_body, _bodyInitialX);
            Canvas.SetTop(_body, _bodyInitialY);
            LogicBlock.Add(_body);
            _canvas.Children.Add(_body);
            _body.PreviewMouseLeftButtonDown += _body_PreviewMouseLeftButtonDown;
            _body.PreviewMouseRightButtonDown += _body_PreviewMouseRightButtonDown;

            AddInputs();
            AddOutput();
            
            AddOutputSnap();
            AddInputsSnap();
            AddOutputSnapColoringEvent();

            Name = "AND" + serialNumber;
            Graph.AddNode(Name);
        }

        public int Remove()
        {
            if (LogicBlock != null)
            {
                foreach (UIElement logicElementPart in LogicBlock)
                {
                    _canvas.Children.Remove(logicElementPart);
                }
                for (int i = _connectionLines.Count - 1; i >= 0; i--)
                {
                    _connectionLines[i].Remove();
                }
                _connectionLines.Clear();
            }
            int serialNumber = 0;
            int.TryParse(Name.Substring("AND".Length), out serialNumber);
            Graph.RemoveNode(Name);
            return serialNumber;
        }

        private void RemoveContactsAndSnaps()
        {
            foreach (UIElement contact in Inputs)
            {
                _canvas.Children.Remove(contact);
                LogicBlock?.Remove(contact);
            }
            Inputs.Clear();
            foreach (UIElement contactSnap in InputsSnap)
            {
                _canvas.Children.Remove(contactSnap);
                LogicBlock?.Remove(contactSnap);
            }
            InputsSnap.Clear();

            _canvas.Children.Remove(Output);
            LogicBlock?.Remove(Output);
            Output = null;

            _canvas.Children.Remove(OutputSnap);
            LogicBlock?.Remove(OutputSnap);
            OutputSnap = null;
        }

        private void AddOutput()
        {
            double bodyX = Canvas.GetLeft(_body);
            double bodyY = Canvas.GetTop(_body);
            Line rightLine = Figures.AddContact();
            rightLine.X1 = bodyX + BodyWidth;
            rightLine.Y1 = bodyY + ((double)BodyHeight / 2);
            rightLine.X2 = bodyX + BodyWidth + ContactWidth;
            rightLine.Y2 = bodyY + ((double)BodyHeight / 2);
            LogicBlock.Add(rightLine);
            _canvas.Children.Add(rightLine);
            Output = rightLine;
            _negativeOutputCircle = null;
        }

        private void AddInputs()
        {
            double bodyX = Canvas.GetLeft(_body);
            double bodyY = Canvas.GetTop(_body);
            for (int i = 0; i < _inputsNumber; i++)
            {
                double relativeInputY = BodyHeight * ((double)(i + 1) / (_inputsNumber + 1));
                Line leftInputLine = Figures.AddContact();
                leftInputLine.X1 = bodyX - ContactWidth;
                leftInputLine.Y1 = bodyY + (relativeInputY);
                leftInputLine.X2 = bodyX;
                leftInputLine.Y2 = bodyY + (relativeInputY);
                LogicBlock.Add(leftInputLine);
                _canvas.Children.Add(leftInputLine);
                Inputs.Add(leftInputLine);
            }
            _negativeInputsCircle = new List<Shape>(new Shape[_inputsNumber]);
        }

        private void AddOutputSnap()
        {
            double bodyX = Canvas.GetLeft(_body);
            double bodyY = Canvas.GetTop(_body);
            Ellipse rightInputSnap = Figures.AddSnapCircle(SnapCircleDiameter);
            Canvas.SetLeft(rightInputSnap, bodyX + BodyWidth);
            Canvas.SetTop(rightInputSnap, bodyY + (double)(BodyHeight / 2) -
                (double)(SnapCircleDiameter / 2));
            LogicBlock?.Add(rightInputSnap);
            _canvas.Children.Add(rightInputSnap);
            rightInputSnap.PreviewMouseLeftButtonDown += OutputSnap_PreviewMouseLeftButtonDown;
            OutputSnap = rightInputSnap;
        }

        private void AddInputsSnap()
        {
            double bodyX = Canvas.GetLeft(_body);
            double bodyY = Canvas.GetTop(_body);
            for (int i = 0; i < _inputsNumber; i++)
            {
                Ellipse leftInputSnap = Figures.AddSnapCircle(SnapCircleDiameter);
                Canvas.SetLeft(leftInputSnap, bodyX - SnapCircleDiameter);
                Canvas.SetTop(leftInputSnap, bodyY + BodyHeight * ((double)(i + 1) / (_inputsNumber + 1)) - 
                    (double)(SnapCircleDiameter / 2));
                LogicBlock?.Add(leftInputSnap);
                _canvas.Children.Add(leftInputSnap);
                leftInputSnap.Name = "input" + i.ToString();
                leftInputSnap.PreviewMouseLeftButtonDown += InputSnap_PreviewMouseLeftButtonDown;
                InputsSnap?.Add(leftInputSnap);
            }
        }

        public void AddOutputSnapColoringEvent()
        {
            if (OutputSnap != null)
            {
                OutputSnap.MouseLeave += Output_MouseLeave;
                OutputSnap.MouseMove += Output_MouseMove;
            }
        }

        public void RemoveOutputSnapColoringEvent()
        {
            if (OutputSnap != null)
            {
                OutputSnap.MouseLeave -= Output_MouseLeave;
                OutputSnap.MouseMove -= Output_MouseMove;
            }
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
            if (_body != null)
            {
                _body.PreviewMouseLeftButtonDown += LogicElementAND_PreviewMouseLeftButtonDown;
                _body.PreviewMouseMove += LogicElement_PreviewMouseMove;
                _body.PreviewMouseLeftButtonUp += LogicElement_PreviewMouseLeftButtonUp;
            }
        }

        private void _body_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                IncreaseInputsNumber();
            }
        }

        private void IncreaseInputsNumber()
        {
            if (_inputsNumber < 10)
            {
                _inputsNumber++;
                RemoveContactsAndSnaps();
                _body.Height += 10;
                BodyHeight += 10;
                AddInputs();
                AddOutput();
                AddInputsSnap();
                AddOutputSnap();
            }
        }

        private void _body_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                DecreaseInputsNumber();
            }
        }

        private void DecreaseInputsNumber()
        {
            if (_inputsNumber > 2)
            {
                _inputsNumber--;
                RemoveContactsAndSnaps();
                _body.Height -= 10;
                BodyHeight -= 10;
                AddInputs();
                AddOutput();
                AddInputsSnap();
                AddOutputSnap();
            }
        }

        private void InputSnap_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                Ellipse chosenEllipse = (Ellipse)sender;
                int serialNumber = Convert.ToInt32(chosenEllipse.Name.Replace("input", ""));
                if (_negativeInputsCircle[serialNumber] != null)
                {
                    AddPositiveInput((Ellipse)_negativeInputsCircle[serialNumber], serialNumber);
                }
                else
                {
                    AddNegativeInput(serialNumber);
                }
            }
        }

        private void AddPositiveInput(Ellipse rightEllipse, int serialNumber)
        {
            _canvas?.Children.Remove(rightEllipse);
            LogicBlock?.Remove(rightEllipse);
            _negativeInputsCircle[serialNumber] = null;
        }

        private void AddNegativeInput(int serialNumber)
        {
            double outputX = Canvas.GetLeft(_body);
            double outputY = Canvas.GetTop(_body) + (Convert.ToDouble(BodyHeight * (serialNumber + 1)) / (_inputsNumber + 1));
            Ellipse circle = Figures.AddNegativeCircle(NegativeCircleDiameter);
            Canvas.SetTop(circle, outputY - NegativeCircleDiameter / 2);
            Canvas.SetLeft(circle, outputX - NegativeCircleDiameter * 1 / 3);

            LogicBlock?.Add(circle);
            circle.PreviewMouseLeftButtonDown += InputSnap_PreviewMouseLeftButtonDown;
            _negativeInputsCircle[(int)serialNumber] = circle;
            _canvas?.Children.Add(circle);
            circle.Name = "input" + serialNumber.ToString();
            if (LogicBlock != null)
            {
                foreach (UIElement uIElement in LogicBlock)
                {
                    Panel.SetZIndex(uIElement, 0);
                }
            }
        }

        private void OutputSnap_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
            Ellipse circle = Figures.AddNegativeCircle(NegativeCircleDiameter);
            Canvas.SetTop(circle, outputY - NegativeCircleDiameter / 2);
            Canvas.SetLeft(circle, outputX - NegativeCircleDiameter * 2 / 3);

            LogicBlock?.Add(circle);
            circle.PreviewMouseLeftButtonDown +=OutputSnap_PreviewMouseLeftButtonDown;
            _negativeOutputCircle = circle;
            _canvas?.Children.Add(circle);
            if (LogicBlock != null)
            {
                foreach (UIElement uIElement in LogicBlock)
                {
                    Panel.SetZIndex(uIElement, 0);
                }
            }
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
            _connectionLines.Add(connectionLine);
        }

        public override void CreateNodes(ConnectionLine connectionLine)
        {
            if (connectionLine.BeginElement == this)
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
            else if (connectionLine.EndElement == this)
            {
                foreach (ConnectionLine _connectionLine in _connectionLines)
                {
                    if (_connectionLine.EndElement == this && _connectionLine != connectionLine
                        && _connectionLine.ConnectionLinePolyline!.Points.Last().Equals(connectionLine.ConnectionLinePolyline!.Points.Last(), 0.001))
                    {
                        Point intersectPoint = Helper.FindIntersectionPoint(connectionLine, _connectionLine, false);
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
        }

        public void MoveLogicElement(Point cursorPosition, Point _logicElementOffset)
        {
            if (LogicBlock != null)
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
                        contact.X2 = X + LogicElement.NegativeCircleDiameter / 2;
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
}
