using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AdaptiveCourseClient.Infrastructure;
using AdaptiveCourseClient.RenderObjects;

namespace AdaptiveCourseClient
{
    public partial class MainWindow : Window
    {
        public UIElement? SelectedLogicElement { get; set; }
        public ConnectionLine SelectedLine { get; set; }
        public bool IsSelected 
        { 
            get { return _isSelected; } 
            set 
            {
                _isSelected = value; 
                if (value == true) _isStepEnds = false; 
            } 
        }
        private bool IsConnectionLineBuilding 
        { 
            get { return _isConnectionLineBuilding; }
            set 
            {
                _isConnectionLineBuilding = value;
                if (value == true) _isStepEnds = false; 
            }
        }

        private LogicElement? _logicElement;
        private Shape _beginningContact;
        private Point _logicElementOffset;
        private List<LogicElement> _logicElements;
        private UIElementGroup _inputs;
        private UIElement _output;
        private List<ConnectionLine> connectionLines;

        private bool _isConnectionLineBuilding = false;
        private bool _isStepEnds = true;
        private bool _isSelected = false;

        private static readonly int _logicElementNum = 3;
        private static readonly int _inputsNum = 4;
        private static readonly int _inputWidth = 10; 

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            bodyCanvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            _logicElements = new List<LogicElement>();
            _inputs = new UIElementGroup();
            connectionLines = new List<ConnectionLine>();

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
                input.MouseMove += Input_MouseMove;
                input.MouseLeave += Input_MouseLeave;
                input.PreviewMouseLeftButtonDown += BeginningContact_PreviewMouseLeftButtonDown;

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
                LogicElement element = new LogicElement(bodyCanvas);
                _logicElements.Add(element);
                element.AddBlock();
                element.AddOutputSnap();
                element.OutputSnap.PreviewMouseLeftButtonDown += BeginningContact_PreviewMouseLeftButtonDown;
                element.AddInputsSnap();
                element.MoveLogicBlockEvents(LogicElementAND_PreviewMouseLeftButtonDown, LogicElement_PreviewMouseMove,
                    LogicElement_PreviewMouseLeftButtonUp);
                element.AddOutputSnapColoringEvent();
            }
        }

        private void BeginningContact_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsConnectionLineBuilding)
            {
                IsConnectionLineBuilding = true;
                _beginningContact = (Shape)sender;
                _beginningContact.Stroke = Brushes.DarkGreen;
                ColorAllEndingContacts(true);
                RemoveEventsForBeginningContacts();
                AddEventsForEndingContacts();
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isStepEnds)
            {
                if (IsConnectionLineBuilding)
                {
                    IsConnectionLineBuilding = false;
                    if (_beginningContact is Ellipse)
                        _beginningContact.Stroke = Brushes.Transparent;
                    else if (_beginningContact is Polygon)
                        _beginningContact.Stroke = Brushes.Black;
                    ColorAllEndingContacts(false);
                    RemoveEventsForEndingContacts();
                    AddEventsForBeginningContacts();
                }
                if (IsSelected)
                {
                    SelectedLine.SetColor(Brushes.Black);
                    IsSelected = false;
                    SelectedLine = null;
                }
            }
            else
            {
                _isStepEnds = true;
            }
        }

        private void Canvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (connectionLines.Contains(SelectedLine))
                {
                    connectionLines.Remove(SelectedLine);
                    bodyCanvas.Children.Remove(SelectedLine.Polyline);
                    IsSelected = false;
                    SelectedLine = null;
                }
            }
        }

        private void ColorAllEndingContacts(bool isSelected)
        {
            foreach(LogicElement elementAND in _logicElements)
            {
                foreach(UIElement input in elementAND.InputsSnap)
                {
                    Shape inputSnap = (Ellipse)input;
                    inputSnap.Stroke = isSelected ? Brushes.Red : Brushes.Transparent;
                }
            }
            Shape outnputSnap = (Polygon)_output;
            outnputSnap.Stroke = isSelected ? Brushes.Red : Brushes.Black;
        }

        private void AddEventsForBeginningContacts()
        {
            foreach (UIElement uIElement in _inputs)
            {
                uIElement.MouseMove += Input_MouseMove;
                uIElement.MouseLeave += Input_MouseLeave;
            }
            foreach (LogicElement uIElement in _logicElements)
            {
                uIElement.AddOutputSnapColoringEvent();
            }
        }

        private void RemoveEventsForBeginningContacts()
        {
            foreach (UIElement uIElement in _inputs)
            {
                uIElement.MouseMove -= Input_MouseMove;
                uIElement.MouseLeave -= Input_MouseLeave;
            }
            foreach (LogicElement uIElement in _logicElements)
            {
                uIElement.RemoveOutputSnapColoringEvent();
            }
        }

        private void AddEventsForEndingContacts()
        {
            _output.MouseLeftButtonUp += EndingContact_PreviewMouseLeftButtonUp;
            foreach (LogicElement uIElement in _logicElements)
            {
                foreach(UIElement outputSnap in uIElement.InputsSnap)
                {
                    outputSnap.MouseLeftButtonUp += EndingContact_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void RemoveEventsForEndingContacts()
        {
            _output.MouseLeftButtonUp -= EndingContact_PreviewMouseLeftButtonUp;
            foreach (LogicElement uIElement in _logicElements)
            {
                foreach (UIElement outputSnap in uIElement.InputsSnap)
                {
                    outputSnap.MouseLeftButtonUp -= EndingContact_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void EndingContact_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point firstPoint = new Point();
            Point lastPoint = new Point();

            if (_beginningContact is Polygon)
            {
                Polygon polygon = (Polygon)_beginningContact;
                firstPoint = polygon.Points.Last();
            }
            else if(_beginningContact is Ellipse)
            {
                firstPoint = new Point(Canvas.GetLeft(_beginningContact) + LogicElement.SnapCircleDiameter / 2,
                    Canvas.GetTop(_beginningContact) + LogicElement.SnapCircleDiameter / 2);
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

            ConnectionLine connectionLine = new ConnectionLine(this);
            connectionLine.AddConnectionLine(bodyCanvas, firstPoint, lastPoint);

            connectionLines.Add(connectionLine);
        }

        private void Input_MouseLeave(object sender, MouseEventArgs e)
        {
            Polygon input = (Polygon)sender;
            if (input != null)
            {
                input.Stroke = Brushes.Black;
            }
        }

        private void Input_MouseMove(object sender, MouseEventArgs e)
        {
            Polygon input = (Polygon)sender;
            if (input != null)
            {
                input.Stroke = Brushes.Red;
            }
        }

        private void LogicElementAND_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedLogicElement = (UIElement)sender;
            foreach(LogicElement logicBlock in _logicElements)
            {
                if (logicBlock.LogicBlock.Contains(SelectedLogicElement))
                {
                    _logicElement = logicBlock;
                }
            }
            _logicElementOffset = e.GetPosition(bodyCanvas);
            _logicElementOffset.Y -= Canvas.GetTop(SelectedLogicElement);
            _logicElementOffset.X -= Canvas.GetLeft(SelectedLogicElement);
            if (_logicElement != null)
            {
                foreach (UIElement uIElement in _logicElement.LogicBlock)
                {
                    Panel.SetZIndex(uIElement, 1);
                }
            }
            SelectedLogicElement.CaptureMouse();
        }

        private void LogicElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_logicElement == null)
                return;

            // Logic element movement
            Point cursorPosition = e.GetPosition(sender as Canvas);

            _logicElement.MoveLogicElement(cursorPosition, _logicElementOffset, connectionLines);
        }

        private void LogicElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_logicElement == null)
                return;

            SelectedLogicElement?.ReleaseMouseCapture();
            SelectedLogicElement = null;
            _logicElement = null;
        }
    }
}
