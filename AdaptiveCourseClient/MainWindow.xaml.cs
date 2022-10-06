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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement? currentUIElement { get; set; }
        private UIElementGroup? logicElement { get; set; }
        private Shape coloredElement { get; set; }
        private Point logicElementOffset;
        private List<ElementAND> logicElements { get; set; }
        private UIElementGroup leftInputs { get; set; }
        private UIElement rightInput { get; set; }
        private UIElementGroup ConnectionLines { get; set; }

        private bool isConnectionLineBuilding = false;

        private int InputsNum = 4;
        private int InputHeight = 10; 

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            bodyCanvas.PreviewMouseLeftButtonDown += BodyCanvas_PreviewMouseLeftButtonDown;

            logicElements = new List<ElementAND>();
            leftInputs = new UIElementGroup();
            ConnectionLines = new UIElementGroup();

            CreateBlocks();
        }

        private void BodyCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (ConnectionLines.Contains(currentUIElement)){
                    ConnectionLines.Remove(currentUIElement);
                    bodyCanvas.Children.Remove(currentUIElement);
                    currentUIElement = null;
                }
            }
        }

        private void CreateBlocks()
        {
            for (int i = 0; i < 3; i++)
            {
                ElementAND elementAND = new ElementAND();
                logicElements.Add(elementAND);
                elementAND.AddAND(bodyCanvas);
                elementAND.AddRightInputAround(bodyCanvas);
                elementAND.rightInputAround.PreviewMouseLeftButtonDown += Input_PreviewMouseLeftButtonDown;
                elementAND.AddLeftInputAround(bodyCanvas);
                elementAND.MoveLogicBlock(LogicElementAND_PreviewMouseLeftButtonDown, LogicElement_PreviewMouseMove,
                    LogicElement_PreviewMouseLeftButtonUp);
                elementAND.AddRightInputsAroundColoring();
            }
        }

        private void CreateLeftInputs()
        {
            for (int i = 0; i < InputsNum; i++)
            {
                Polygon leftInput = new Polygon();
                leftInput.Fill = Brushes.White;
                leftInput.Stroke = Brushes.Black;
                leftInput.StrokeThickness = 3;
                PointCollection leftPoints = new PointCollection();
                leftPoints.Add(new Point(Toolbox.ActualWidth,
                    this.Height * ((double)(i + 1) / (InputsNum + 1)) - InputHeight));
                leftPoints.Add(new Point(Toolbox.ActualWidth,
                    this.Height * ((double)(i + 1) / (InputsNum + 1)) + InputHeight));
                leftPoints.Add(new Point(Toolbox.ActualWidth + 40,
                    this.Height * ((double)(i + 1) / (InputsNum + 1))));

                leftInput.Points = leftPoints;
                leftInput.MouseMove += LeftInput_MouseMove;
                leftInput.MouseLeave += LeftInput_MouseLeave;
                leftInput.PreviewMouseLeftButtonDown += Input_PreviewMouseLeftButtonDown;

                bodyCanvas.Children.Add(leftInput);
                leftInputs.Add(leftInput);
            }
        }

        private void CreateRightInput()
        {
            Polygon rightInput = new Polygon();
            rightInput.Fill = Brushes.White;
            rightInput.Stroke = Brushes.Black;
            rightInput.StrokeThickness = 3;

            PointCollection points = new PointCollection();
            points.Add(new Point(bodyCanvas.ActualWidth,
                this.Height / 2 - InputHeight));
            points.Add(new Point(bodyCanvas.ActualWidth,
                this.Height / 2 + InputHeight));
            points.Add(new Point(bodyCanvas.ActualWidth - 40,
                this.Height / 2));

            rightInput.Points = points;
            this.rightInput = rightInput;
            bodyCanvas.Children.Add(rightInput);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateLeftInputs();
            CreateRightInput();
        }

        private void BodyCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isConnectionLineBuilding)
            {
                isConnectionLineBuilding = false;
                if (coloredElement.Stroke != Brushes.Black && coloredElement.Stroke != Brushes.Transparent)
                {
                    if (coloredElement is Ellipse)
                        coloredElement.Stroke = Brushes.Transparent;
                    else if (coloredElement is Polygon)
                        coloredElement.Stroke = Brushes.Black;
                    ColorAllLeftInputs(false);
                    RemoveEventsForLeftInputs();
                    AddEventsForRightInputs();
                }
            }
        }

        private void Input_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isConnectionLineBuilding)
            {
                isConnectionLineBuilding = true;
                coloredElement = (Shape)sender;
                if (coloredElement.Stroke != Brushes.DarkGreen)
                {
                    coloredElement.Stroke = Brushes.DarkGreen;
                    ColorAllLeftInputs(true);
                    RemoveEventsForRightInputs();
                    AddEventsForLeftInputs();
                }
            }
        }

        private void ColorAllLeftInputs(bool isSelected)
        {
            foreach(ElementAND elementAND in logicElements)
            {
                foreach(UIElement leftInput in elementAND.leftInputsAround)
                {
                    Shape leftInputAround = (Ellipse)leftInput;
                    leftInputAround.Stroke = isSelected ? Brushes.Red : Brushes.Transparent;
                }
            }
            Shape rightInputAround = (Polygon)rightInput;
            rightInputAround.Stroke = isSelected ? Brushes.Red : Brushes.Black;
        }

        private void AddEventsForRightInputs()
        {
            foreach (UIElement uIElement in leftInputs)
            {
                uIElement.MouseMove += LeftInput_MouseMove;
                uIElement.MouseLeave += LeftInput_MouseLeave;
            }
            foreach (ElementAND uIElement in logicElements)
            {
                uIElement.AddRightInputsAroundColoring();
            }
        }

        private void RemoveEventsForRightInputs()
        {
            foreach (UIElement uIElement in leftInputs)
            {
                uIElement.MouseMove -= LeftInput_MouseMove;
                uIElement.MouseLeave -= LeftInput_MouseLeave;
            }
            foreach (ElementAND uIElement in logicElements)
            {
                uIElement.RemoveRightInputsAroundColoring();
            }
        }

        private void AddEventsForLeftInputs()
        {
            rightInput.PreviewMouseLeftButtonUp += LeftInput_PreviewMouseLeftButtonUp;
            foreach (ElementAND uIElement in logicElements)
            {
                foreach(UIElement leftInputAround in uIElement.leftInputsAround)
                {
                    leftInputAround.PreviewMouseLeftButtonUp += LeftInput_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void RemoveEventsForLeftInputs()
        {
            rightInput.PreviewMouseLeftButtonUp -= LeftInput_PreviewMouseLeftButtonUp;
            foreach (ElementAND uIElement in logicElements)
            {
                foreach (UIElement leftInputAround in uIElement.leftInputsAround)
                {
                    leftInputAround.PreviewMouseLeftButtonUp -= LeftInput_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void LeftInput_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Polyline ConnectionLine = new Polyline();

            PointCollection points = new PointCollection();
            Point firstPoint = new Point();
            if (coloredElement is Polygon)
            {
                Polygon polygon = (Polygon)coloredElement;
                firstPoint = polygon.Points.Last();
            }
            else if(coloredElement is Ellipse)
            {
                firstPoint = new Point(Canvas.GetLeft(coloredElement) + ElementAND.circleAroundDiameter / 2,
                    Canvas.GetTop(coloredElement) + ElementAND.circleAroundDiameter / 2);
            }
            Point lastPoint = new Point();
            if (sender is Polygon)
            {
                Polygon polygon = (Polygon)sender;
                lastPoint = polygon.Points.Last();
            }
            else if (sender is Ellipse)
            {
                lastPoint = new Point(Canvas.GetLeft((Ellipse)sender) + ElementAND.circleAroundDiameter / 2,
                    Canvas.GetTop((Ellipse)sender) + ElementAND.circleAroundDiameter / 2);
            }
            double fractureX = (firstPoint.X + lastPoint.X) / 2;
            points.Add(firstPoint);
            points.Add(new Point(fractureX, firstPoint.Y));
            points.Add(new Point(fractureX, lastPoint.Y));
            points.Add(lastPoint);

            ConnectionLine.Stroke = Brushes.Black;
            ConnectionLine.StrokeThickness = 2;
            ConnectionLine.Points = points;
            ConnectionLine.MouseMove += ConnectionLine_MouseMove;
            ConnectionLine.MouseLeave += ConnectionLine_MouseLeave;
            ConnectionLine.KeyDown += BodyCanvas_PreviewKeyDown;
            ConnectionLine.PreviewMouseLeftButtonDown += ConnectionLine_PreviewMouseLeftButtonDown;

            bodyCanvas.Children.Add(ConnectionLine);
            ConnectionLines.Add(ConnectionLine);
        }

        private void ConnectionLine_MouseLeave(object sender, MouseEventArgs e)
        {
            Polyline selectedLine = (Polyline)sender;
            selectedLine.StrokeThickness = 2;
        }

        private void ConnectionLine_MouseMove(object sender, MouseEventArgs e)
        {
            Polyline selectedLine = (Polyline)sender;
            selectedLine.StrokeThickness = 5;
        }

        private void ConnectionLine_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentUIElement = (UIElement)sender;
            Polyline selectedLine = (Polyline)sender;
            selectedLine.Stroke = Brushes.BlueViolet;
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
            currentUIElement = (UIElement)sender;
            foreach(ElementAND logicBlock in logicElements)
            {
                if (logicBlock.logicBlock.Contains(currentUIElement))
                {
                    logicElement = logicBlock.logicBlock;
                }
            }
            logicElementOffset = e.GetPosition(bodyCanvas);
            logicElementOffset.Y -= Canvas.GetTop(currentUIElement);
            logicElementOffset.X -= Canvas.GetLeft(currentUIElement);
            if (logicElement != null)
            {
                foreach (UIElement uIElement in logicElement)
                {
                    Panel.SetZIndex(uIElement, 1);
                }
            }
            currentUIElement.CaptureMouse();
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
                    line.X2 = X + ElementAND.circleDiameter / 2;
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
            currentUIElement?.ReleaseMouseCapture();
            currentUIElement = null;
            logicElement = null;
        }
    }
}
