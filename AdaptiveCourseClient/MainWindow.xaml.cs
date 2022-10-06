using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AdaptiveCourseClient.RenderObjects;

namespace AdaptiveCourseClient
{
    public partial class MainWindow : Window
    {
        private UIElement? _currentElement;
        private UIElementGroup? logicElement;
        private Shape coloredElement;
        private Point logicElementOffset;
        private List<LogicElement> logicElements;
        private UIElementGroup _inputs;
        private UIElement _output;
        private UIElementGroup ConnectionLines;

        private bool isConnectionLineBuilding = false;

        private static readonly int _logicElementNum = 3;
        private static readonly int _inputsNum = 4;
        private static readonly int _inputWidth = 10; 

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            bodyCanvas.PreviewMouseLeftButtonDown += Canvas_PreviewMouseLeftButtonDown;

            logicElements = new List<LogicElement>();
            _inputs = new UIElementGroup();
            ConnectionLines = new UIElementGroup();

            CreateBlocks();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateInputs();
            CreateOutput();
        }

        private void CreateInputs()
        {
            for (int i = 0; i < _inputsNum; i++)
            {
                Polygon input = new Polygon();
                input.Fill = Brushes.White;
                input.Stroke = Brushes.Black;
                input.StrokeThickness = 3;
                PointCollection inputPoints = new PointCollection();
                inputPoints.Add(new Point(Toolbox.ActualWidth,
                    this.Height * ((double)(i + 1) / (_inputsNum + 1)) - _inputWidth));
                inputPoints.Add(new Point(Toolbox.ActualWidth,
                    this.Height * ((double)(i + 1) / (_inputsNum + 1)) + _inputWidth));
                inputPoints.Add(new Point(Toolbox.ActualWidth + 40,
                    this.Height * ((double)(i + 1) / (_inputsNum + 1))));

                input.Points = inputPoints;
                input.MouseMove += LeftInput_MouseMove;
                input.MouseLeave += LeftInput_MouseLeave;
                input.PreviewMouseLeftButtonDown += Input_PreviewMouseLeftButtonDown;

                bodyCanvas.Children.Add(input);
                _inputs.Add(input);
            }
        }

        private void CreateOutput()
        {
            Polygon output = new Polygon();
            output.Fill = Brushes.White;
            output.Stroke = Brushes.Black;
            output.StrokeThickness = 3;

            PointCollection outputPoints = new PointCollection();
            outputPoints.Add(new Point(bodyCanvas.ActualWidth,
                this.Height / 2 - _inputWidth));
            outputPoints.Add(new Point(bodyCanvas.ActualWidth,
                this.Height / 2 + _inputWidth));
            outputPoints.Add(new Point(bodyCanvas.ActualWidth - 40,
                this.Height / 2));

            output.Points = outputPoints;
            _output = output;
            bodyCanvas.Children.Add(output);
        }

        private void CreateBlocks()
        {
            for (int i = 0; i < _logicElementNum; i++)
            {
                LogicElement element = new LogicElement();
                logicElements.Add(element);
                element.AddBlock(bodyCanvas);
                element.AddOutputSnap(bodyCanvas);
                element.OutputSnap.PreviewMouseLeftButtonDown += Input_PreviewMouseLeftButtonDown;
                element.AddInputsSnap(bodyCanvas);
                element.MoveLogicBlock(LogicElementAND_PreviewMouseLeftButtonDown, LogicElement_PreviewMouseMove,
                    LogicElement_PreviewMouseLeftButtonUp);
                element.AddOutputSnapColoringEvent();
            }
        }

        private void Input_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isConnectionLineBuilding)
            {
                isConnectionLineBuilding = true;
                coloredElement = (Shape)sender;
                    coloredElement.Stroke = Brushes.DarkGreen;
                    ColorAllEndingContacts(true);
                    RemoveEventsForRightInputs();
                    AddEventsForLeftInputs();
            }
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isConnectionLineBuilding)
            {
                isConnectionLineBuilding = false;
                    if (coloredElement is Ellipse)
                        coloredElement.Stroke = Brushes.Transparent;
                    else if (coloredElement is Polygon)
                        coloredElement.Stroke = Brushes.Black;
                    ColorAllEndingContacts(false);
                    RemoveEventsForLeftInputs();
                    AddEventsForRightInputs();
            }
        }

        private void BodyCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (ConnectionLines.Contains(_currentElement))
                {
                    ConnectionLines.Remove(_currentElement);
                    bodyCanvas.Children.Remove(_currentElement);
                    _currentElement = null;
                }
            }
        }

        private void ColorAllEndingContacts(bool isSelected)
        {
            foreach(LogicElement elementAND in logicElements)
            {
                foreach(UIElement leftInput in elementAND.InputsSnap)
                {
                    Shape leftInputAround = (Ellipse)leftInput;
                    leftInputAround.Stroke = isSelected ? Brushes.Red : Brushes.Transparent;
                }
            }
            Shape rightInputAround = (Polygon)_output;
            rightInputAround.Stroke = isSelected ? Brushes.Red : Brushes.Black;
        }

        private void AddEventsForRightInputs()
        {
            foreach (UIElement uIElement in _inputs)
            {
                uIElement.MouseMove += LeftInput_MouseMove;
                uIElement.MouseLeave += LeftInput_MouseLeave;
            }
            foreach (LogicElement uIElement in logicElements)
            {
                uIElement.AddOutputSnapColoringEvent();
            }
        }

        private void RemoveEventsForRightInputs()
        {
            foreach (UIElement uIElement in _inputs)
            {
                uIElement.MouseMove -= LeftInput_MouseMove;
                uIElement.MouseLeave -= LeftInput_MouseLeave;
            }
            foreach (LogicElement uIElement in logicElements)
            {
                uIElement.RemoveOutputSnapColoringEvent();
            }
        }

        private void AddEventsForLeftInputs()
        {
            _output.MouseLeftButtonUp += LeftInput_PreviewMouseLeftButtonUp;
            foreach (LogicElement uIElement in logicElements)
            {
                foreach(UIElement leftInputAround in uIElement.InputsSnap)
                {
                    leftInputAround.MouseLeftButtonUp += LeftInput_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void RemoveEventsForLeftInputs()
        {
            _output.PreviewMouseLeftButtonUp -= LeftInput_PreviewMouseLeftButtonUp;
            foreach (LogicElement uIElement in logicElements)
            {
                foreach (UIElement leftInputAround in uIElement.InputsSnap)
                {
                    leftInputAround.PreviewMouseLeftButtonUp -= LeftInput_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void LeftInput_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point firstPoint = new Point();
            Point lastPoint = new Point();

            if (coloredElement is Polygon)
            {
                Polygon polygon = (Polygon)coloredElement;
                firstPoint = polygon.Points.Last();
            }
            else if(coloredElement is Ellipse)
            {
                firstPoint = new Point(Canvas.GetLeft(coloredElement) + LogicElement.SnapCircleDiameter / 2,
                    Canvas.GetTop(coloredElement) + LogicElement.SnapCircleDiameter / 2);
            }
            
            if (sender is Polygon)
            {
                Polygon polygon = (Polygon)sender;
                lastPoint = polygon.Points.Last();
            }
            else if (sender is Ellipse)
            {
                lastPoint = new Point(Canvas.GetLeft((Ellipse)sender) + LogicElement.SnapCircleDiameter / 2,
                    Canvas.GetTop((Ellipse)sender) + LogicElement.SnapCircleDiameter / 2);
            }

            ConnectionLine connectionLine = new ConnectionLine();
            Polyline connectionLinePoly = connectionLine.AddConnectionLine(bodyCanvas, firstPoint, lastPoint);

            ConnectionLines.Add(connectionLinePoly);
        }

        private void LeftInput_MouseLeave(object sender, MouseEventArgs e)
        {
            Polygon input = (Polygon)sender;
            if (input != null)
            {
                input.Stroke = Brushes.Black;
            }
        }

        private void LeftInput_MouseMove(object sender, MouseEventArgs e)
        {
            Polygon input = (Polygon)sender;
            if (input != null)
            {
                input.Stroke = Brushes.Red;
            }
        }

        private void LogicElementAND_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _currentElement = (UIElement)sender;
            foreach(LogicElement logicBlock in logicElements)
            {
                if (logicBlock.LogicBlock.Contains(_currentElement))
                {
                    logicElement = logicBlock.LogicBlock;
                }
            }
            logicElementOffset = e.GetPosition(bodyCanvas);
            logicElementOffset.Y -= Canvas.GetTop(_currentElement);
            logicElementOffset.X -= Canvas.GetLeft(_currentElement);
            if (logicElement != null)
            {
                foreach (UIElement uIElement in logicElement)
                {
                    Panel.SetZIndex(uIElement, 1);
                }
            }
            _currentElement.CaptureMouse();
        }

        private void LogicElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (logicElement == null)
                return;

            // Logic element movement
            Point cursorPosition = e.GetPosition(sender as Canvas);

            double positionXMain = Canvas.GetLeft(logicElement[0]);
            double positionYMain = Canvas.GetTop(logicElement[0]);

            foreach (UIElement uIElement in logicElement)
            {
                double Y = cursorPosition.Y - logicElementOffset.Y - positionYMain;
                double X = cursorPosition.X - logicElementOffset.X - positionXMain;
                if (uIElement is Line)
                {
                    Line line = (Line)uIElement;
                    double positionX = line.X1;
                    double positionY = line.Y1;
                    Y += positionY;
                    X += positionX;
                    MoveConnectionLines(line, X, Y);
                    line.X1 = X;
                    line.Y1 = Y;
                    line.X2 = X + LogicElement.OutputCircleDiameter / 2;
                    line.Y2 = Y;
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

        private void MoveConnectionLines(Line input, double newX, double newY)
        {
            foreach(UIElement line in ConnectionLines)
            {
                Polyline connectionLine = (Polyline)line;
                Point firstConnectionPoint = connectionLine.Points[0];
                Point lastConnectionPoint = connectionLine.Points.Last();
                // Left input
                if ((firstConnectionPoint.X == input.X1) && (firstConnectionPoint.Y == input.Y1))
                {
                    connectionLine.Points = MoveConnectionLinePoints(connectionLine.Points, newX, newY, (lastConnectionPoint.X + newX) / 2);
                }
                else if((lastConnectionPoint.X == input.X1) && (lastConnectionPoint.Y == input.Y1))
                {
                    connectionLine.Points = MoveConnectionLinePoints(new PointCollection(connectionLine.Points.Reverse()), newX, newY, (firstConnectionPoint.X + newX) / 2);
                }
                // Right input
                else if ((firstConnectionPoint.X == input.X2) && (firstConnectionPoint.Y == input.Y2))
                {
                    connectionLine.Points =  MoveConnectionLinePoints(connectionLine.Points, newX, newY, (lastConnectionPoint.X + newX) / 2);
                }
                else if ((lastConnectionPoint.X == input.X2) && (lastConnectionPoint.Y == input.Y2))
                {
                    connectionLine.Points =  MoveConnectionLinePoints(new PointCollection(connectionLine.Points.Reverse()), newX, newY, (firstConnectionPoint.X + newX) / 2);
                    
                }
            }
        }

        private PointCollection MoveConnectionLinePoints(PointCollection connectionLine, double newX, double newY, double connectionLineX)
        {
            PointCollection points = new PointCollection();
            points.Add(new Point(newX, newY));
            points.Add(new Point(connectionLineX, newY));
            points.Add(new Point(connectionLineX, connectionLine[2].Y));
            points.Add(new Point(connectionLine[3].X, connectionLine[3].Y));
            return points;
        }

        private void LogicElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (logicElement == null)
                return;

            Point position = e.GetPosition(sender as Canvas);
            foreach (UIElement uIElement in logicElement)
            {
                if (position.X < Toolbox.ActualWidth)
                {
                    Canvas.SetTop(uIElement, 50);
                    Canvas.SetLeft(uIElement, 50);
                }
                Panel.SetZIndex(uIElement, 0);
            }
            _currentElement?.ReleaseMouseCapture();
            _currentElement = null;
            logicElement = null;
        }
    }
}
