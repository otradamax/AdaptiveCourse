using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private Shape? _negativeOutputCircle;        
        private List<Shape> _negativeInputsCircle;

        private MouseButtonEventHandler _outputEventFromMain;

        public static readonly int SnapCircleDiameter = 24;
        public static readonly int ContactWidth = 12;
        public readonly int BodyWidth = 70;
        public int BodyHeight;

        private readonly int _negativeCircleDiameterOut = 24;

        private int _negativeCircleDiameter = 24;
        private int _bodyHeightDelta = 10;
        private readonly int _bodyInitialX = 100;
        private int _bodyInitialY = 50;
        private int _inputsNumber;
        private string _logicBlockName;

        public LogicElement(Canvas canvas, MouseButtonEventHandler BeginningContact_PreviewMouseLeftButtonDown, string logicBlockName, int verticalSerialNumber) : base(canvas)
        {
            _canvas = canvas;
            _outputEventFromMain = BeginningContact_PreviewMouseLeftButtonDown;
            _logicBlockName = logicBlockName;
            _bodyInitialY += verticalSerialNumber * 150;
        }

        public void AddBlock(int serialNumber)
        {
            LogicBlock = new UIElementGroup();
            InputsSnap = new UIElementGroup();
            Inputs = new UIElementGroup();
            _inputsNumber = 2;
            _negativeInputsCircle = new List<Shape>(new Shape[_inputsNumber]);
            BodyHeight = 100;

            // Main body creation
            _body = Figures.AddBody(BodyWidth, BodyHeight);
            Canvas.SetLeft(_body, _bodyInitialX);
            Canvas.SetTop(_body, _bodyInitialY);
            LogicBlock.Add(_body);
            _canvas.Children.Add(_body);
            _body.PreviewMouseLeftButtonDown += _body_PreviewMouseLeftButtonDown;
            _body.PreviewMouseRightButtonDown += _body_PreviewMouseRightButtonDown;

            // Signature creation
            string signature;
            double signatureOffset = 0;
            switch (_logicBlockName)
            {
                case "OR":
                    signature = "1";
                    signatureOffset = 0.1;
                    break;
                case "AND":
                    signature = "&";
                    break;
                default:
                    signature = "";
                    break;
            }
            TextBlock sign = Figures.AddSignature(signature);
            Canvas.SetLeft(sign, _bodyInitialX + (0.6 + signatureOffset) * BodyWidth);
            Canvas.SetTop(sign, _bodyInitialY + 0.05 * BodyHeight);
            LogicBlock.Add(sign);
            _canvas.Children.Add(sign);

            AddInputs();
            AddOutput();
            
            AddOutputSnap();
            AddInputsSnap();
            AddOutputSnapColoringEvent();

            Name = _logicBlockName + serialNumber;
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
            int.TryParse(Name.Substring(_logicBlockName.Length), out serialNumber);
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
                OutputSnap.PreviewMouseLeftButtonDown += _outputEventFromMain;
            }
        }

        private void MoveNegativeCirclesAbove()
        {
            foreach (var logicElement in LogicBlock)
            {
                if (logicElement is Ellipse)
                {
                    Ellipse negativeCircle = logicElement as Ellipse;
                    if (negativeCircle.Stroke == Brushes.Black)
                    {
                        _canvas.Children.Remove(negativeCircle);
                        _canvas.Children.Add(negativeCircle);
                    }
                }
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
                _negativeCircleDiameter -= 2;

                RemoveContactsAndSnaps();
                RestoreConnectionLines(true);
                RestoreNegativeCircles(true);

                _inputsNumber++;
                _body.Height += _bodyHeightDelta;
                BodyHeight += _bodyHeightDelta;

                AddInputs();
                _negativeInputsCircle.Add(null);
                AddOutput();
                AddInputsSnap();
                AddOutputSnap();
                AddOutputSnapColoringEvent();
                MoveNegativeCirclesAbove();
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
                _negativeCircleDiameter += 2;

                RemoveContactsAndSnaps();
                RestoreConnectionLines(false);
                RestoreNegativeCircles(false);

                _inputsNumber--;
                _body.Height -= _bodyHeightDelta;
                BodyHeight -= _bodyHeightDelta;

                AddInputs();
                _negativeInputsCircle.RemoveAt(_negativeInputsCircle.Count - 1);
                AddOutput();
                AddInputsSnap();
                AddOutputSnap();
                AddOutputSnapColoringEvent();
                MoveNegativeCirclesAbove();
            }
        }

        private void RestoreConnectionLines(bool increase)
        {
            for(int i = _connectionLines.Count - 1; i >= 0; i--)
            {
                double bodyY = Canvas.GetTop(_body);
                if (_connectionLines[i].BeginElement == this)
                {
                    double beginPointY = bodyY + ((double)(increase ? (BodyHeight + _bodyHeightDelta) : (BodyHeight - _bodyHeightDelta)) / 2);
                    Point beginPoint = new Point(_connectionLines[i].ConnectionLinePolyline.Points[0].X, beginPointY);
                    _connectionLines[i].SetConnectionLinePoints(_connectionLines[i].ConnectionLinePolyline, beginPoint, _connectionLines[i].ConnectionLinePolyline.Points.Last());
                }
                else if (_connectionLines[i].EndElement == this)
                {
                    double transformCoef;
                    double endPointY;
                    if (increase)
                    {
                        transformCoef = (double)((BodyHeight + _bodyHeightDelta) * (_inputsNumber + 1)) / ((_inputsNumber + 2) * (BodyHeight));
                        endPointY = (_connectionLines[i].ConnectionLinePolyline.Points.Last().Y - bodyY) * transformCoef + bodyY;
                        Point endPoint = new Point(_connectionLines[i].ConnectionLinePolyline.Points.Last().X, endPointY);
                        _connectionLines[i].SetConnectionLinePoints(_connectionLines[i].ConnectionLinePolyline, _connectionLines[i].ConnectionLinePolyline.Points[0], endPoint);
                    }
                    else
                    {
                        transformCoef = (double)((_inputsNumber + 1) * (BodyHeight - _bodyHeightDelta)) / ((BodyHeight) * (_inputsNumber));
                        double lastContactY = bodyY + (double)BodyHeight * (_inputsNumber) / (_inputsNumber + 1);
                        endPointY = (_connectionLines[i].ConnectionLinePolyline.Points.Last().Y - bodyY) * transformCoef + bodyY;
                        if (_connectionLines[i].ConnectionLinePolyline.Points.Last().Y >= lastContactY - 0.001)
                        {
                            _connectionLines[i].Remove();
                        }
                        else
                        {
                            Point endPoint = new Point(_connectionLines[i].ConnectionLinePolyline.Points.Last().X, endPointY);
                            _connectionLines[i].SetConnectionLinePoints(_connectionLines[i].ConnectionLinePolyline, _connectionLines[i].ConnectionLinePolyline.Points[0], endPoint);
                        }
                    }
                }
            }
        }

        private void RestoreNegativeCircles(bool increase)
        {
            for (int i = LogicBlock.Count - 1; i >= 0; i--)
            {
                if (LogicBlock[i] is Ellipse)
                {
                    Ellipse negativeCircle = LogicBlock[i] as Ellipse;
                    if (negativeCircle.Stroke == Brushes.Black)
                    {
                        double bodyY = Canvas.GetTop(_body);
                        double circleY = Canvas.GetTop(negativeCircle) + negativeCircle.Height / 2;
                        double endPointY = 0;
                        // Left negative input
                        if (Canvas.GetLeft(negativeCircle) < (Canvas.GetLeft(_body) + _body.Width / 2))
                        {
                            double transformCoef;
                            if (increase)
                            {
                                negativeCircle.Height = _negativeCircleDiameter;
                                negativeCircle.Width = _negativeCircleDiameter;
                                transformCoef = (double)((BodyHeight + _bodyHeightDelta) * (_inputsNumber + 1)) / ((_inputsNumber + 2) * (BodyHeight));
                                endPointY = (circleY - bodyY) * transformCoef + bodyY;
                            }
                            else
                            {
                                negativeCircle.Height = _negativeCircleDiameter;
                                negativeCircle.Width = _negativeCircleDiameter;
                                transformCoef = (double)((_inputsNumber + 1) * (BodyHeight - _bodyHeightDelta)) / ((BodyHeight) * (_inputsNumber));
                                endPointY = (circleY - bodyY) * transformCoef + bodyY;

                                if (_negativeInputsCircle.Last() != null)
                                {
                                    AddPositiveInput((Ellipse)_negativeInputsCircle[_inputsNumber - 1], _inputsNumber - 1);
                                }
                            }
                            Canvas.SetTop(negativeCircle, endPointY - _negativeCircleDiameter / 2);
                            Canvas.SetLeft(negativeCircle, Canvas.GetLeft(_body) - _negativeCircleDiameter * 1 / 3);
                        }
                        // Right negative output
                        else
                        {
                            endPointY = bodyY + ((double)(increase ? (BodyHeight + _bodyHeightDelta) : (BodyHeight - _bodyHeightDelta)) / 2);
                            Canvas.SetTop(negativeCircle, endPointY - _negativeCircleDiameterOut / 2);
                        }
                    }
                }
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
            double outputX = Canvas.GetLeft(_body);
            double outputY = Canvas.GetTop(_body) + (Convert.ToDouble(BodyHeight * (serialNumber + 1)) / (_inputsNumber + 1));
            NegationToGraph(true, false, outputX, outputY);
            _canvas?.Children.Remove(rightEllipse);
            LogicBlock?.Remove(rightEllipse);
            _negativeInputsCircle[serialNumber] = null;
        }

        private void AddNegativeInput(int serialNumber)
        {
            double outputX = Canvas.GetLeft(_body);
            double outputY = Canvas.GetTop(_body) + (Convert.ToDouble(BodyHeight * (serialNumber + 1)) / (_inputsNumber + 1));
            Ellipse circle = Figures.AddNegativeCircle(_negativeCircleDiameter);
            Canvas.SetTop(circle, outputY - _negativeCircleDiameter / 2);
            Canvas.SetLeft(circle, outputX - _negativeCircleDiameter * 1 / 3);

            NegationToGraph(true, true, outputX, outputY);
            LogicBlock?.Add(circle);
            circle.PreviewMouseLeftButtonDown += InputSnap_PreviewMouseLeftButtonDown;
            _negativeInputsCircle[(int)serialNumber] = circle;
            _canvas?.Children.Add(circle);
            circle.Name = "input" + serialNumber.ToString();
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
            double outputX = Canvas.GetLeft(_body) + BodyWidth;
            double outputY = Canvas.GetTop(_body) + ((double)BodyHeight / 2);
            NegationToGraph(false, false, outputX, outputY);
            _canvas?.Children.Remove(rightEllipse);
            LogicBlock?.Remove(rightEllipse);
            _negativeOutputCircle = null;
        }

        private void AddNegativeOutput()
        {
            double outputX = Canvas.GetLeft(_body) + BodyWidth;
            double outputY = Canvas.GetTop(_body) + ((double)BodyHeight / 2);
            Ellipse circle = Figures.AddNegativeCircle(_negativeCircleDiameterOut);
            Canvas.SetTop(circle, outputY - _negativeCircleDiameter / 2);
            Canvas.SetLeft(circle, outputX - _negativeCircleDiameter * 2 / 3);

            NegationToGraph(false, true, outputX, outputY);
            LogicBlock?.Add(circle);
            circle.PreviewMouseLeftButtonDown +=OutputSnap_PreviewMouseLeftButtonDown;
            _negativeOutputCircle = circle;
            _canvas?.Children.Add(circle);
        }

        private void NegationToGraph(bool isInputContact, bool isAdd, double contactX, double contactY)
        {
            foreach(ConnectionLine connectionLine in _connectionLines)
            {
                if (isInputContact)
                {
                    if ((connectionLine.BeginElement == this && Math.Abs(connectionLine.ConnectionLinePolyline.Points[0].Y - contactY) <= 0.001 && connectionLine.ConnectionLinePolyline.Points[0].X < contactX) ||
                    (connectionLine.EndElement == this && Math.Abs(connectionLine.ConnectionLinePolyline.Points.Last().Y - contactY) <= 0.001 && connectionLine.ConnectionLinePolyline.Points.Last().X < contactX))
                    {
                        if (isAdd)
                        {
                            Graph.AddNegationNode(connectionLine.BeginElement.Name, connectionLine.EndElement.Name, connectionLine.NegativeCount);
                            connectionLine.NegativeCount++;
                        }
                        else
                        {
                            Graph.RemoveNegationNode(connectionLine.BeginElement.Name, connectionLine.EndElement.Name, connectionLine.NegativeCount);
                            connectionLine.NegativeCount--;
                        }
                    }
                }
                else
                {
                    if ((connectionLine.BeginElement == this && Math.Abs(connectionLine.ConnectionLinePolyline.Points[0].Y - contactY) <= 0.001 && connectionLine.ConnectionLinePolyline.Points[0].X > contactX) ||
                    (connectionLine.EndElement == this && Math.Abs(connectionLine.ConnectionLinePolyline.Points.Last().Y - contactY) <= 0.001 && connectionLine.ConnectionLinePolyline.Points.Last().X > contactX))
                    {
                        if (isAdd)
                        {
                            Graph.AddNegationNode(connectionLine.BeginElement.Name, connectionLine.EndElement.Name, connectionLine.NegativeCount);
                            connectionLine.NegativeCount++;
                        }
                        else
                        {
                            Graph.RemoveNegationNode(connectionLine.BeginElement.Name, connectionLine.EndElement.Name, connectionLine.NegativeCount);
                            connectionLine.NegativeCount--;
                        }
                    }
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
                        contact.X2 = X + SnapCircleDiameter / 2;
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

        public override bool HasNegationOnContact(Point point)
        {
            foreach(Shape negativeInput in _negativeInputsCircle)
            {
                if (negativeInput != null)
                {
                    double negativeInputX = Canvas.GetLeft(negativeInput);
                    double negativeInputY = Canvas.GetTop(negativeInput) + _negativeCircleDiameter / 2;
                    if (Math.Abs(negativeInputY - point.Y) <= 0.001 && point.X < negativeInputX)
                        return true;
                }
            }

            if (_negativeOutputCircle != null)
            {
                double negativeOutputX = Canvas.GetLeft(_negativeOutputCircle);
                double negativeOutputY = Canvas.GetTop(_negativeOutputCircle) + _negativeCircleDiameterOut / 2;
                if (Math.Abs(negativeOutputY - point.Y) <= 0.001 && point.X > negativeOutputX)
                    return true;
            }

            return false;
        }
    }
}
