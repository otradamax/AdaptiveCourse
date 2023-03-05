using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AdaptiveCourseClient.Infrastructure;
using AdaptiveCourseClient.Models;
using AdaptiveCourseClient.RenderObjects;
using Newtonsoft.Json;
using static AdaptiveCourseClient.TableWindow;

namespace AdaptiveCourseClient
{
    public partial class SchemeWindow : Window
    {
        public UIElement? SelectedLogicElement { get; set; }
        public ConnectionLine? SelectedLine { get; set; }
        public bool IsConnectionLineSelected 
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
        private Shape? _beginningContact;
        private Point _logicElementOffset;
        private List<LogicElement> _logicElements;
        private List<InputElement> _inputs;
        private OutputElement? _output;
        private List<ConnectionLine> connectionLines;

        private bool _isConnectionLineBuilding = false;
        private bool _isStepEnds = true;
        private bool _isSelected = false;

        private List<string> _renderedBlocks = new List<string>() { "AND", "OR" };

        private static double _mainLeftRightMargin;
        private static double _mainTopBottomMargin;
        private static double _toolboxWidth;

        private static SchemeTask _schemeTask = new SchemeTask();

        public SchemeWindow(SchemeTask schemeTask)
        {
            InitializeComponent();

            _schemeTask = schemeTask;

            Loaded += MainWindow_Loaded;

            bodyCanvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            _logicElements = new List<LogicElement>();
            _inputs = new List<InputElement>();
            connectionLines = new List<ConnectionLine>();
            btnCheckScheme.PreviewMouseLeftButtonUp += BtnCheckScheme_PreviewMouseLeftButtonUp;
        }

        private async void BtnCheckScheme_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var a = Graph.OrientedGraph;
            await GetLogicScheme();
        }

        private async Task GetLogicScheme()
        {
            try
            {
                CheckScheme checkScheme = new CheckScheme(Graph.OrientedGraph, _schemeTask.Id);
                var json = JsonConvert.SerializeObject(checkScheme);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helper.Request(HttpMethod.Post, "https://localhost:7133/Home/LogicScheme", stringContent);
                string result = await response.Content.ReadAsStringAsync();
                MessageBox.Show(result, "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {

            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double mainWidth = bodyCanvas.ActualWidth * 5 / 6;
            _toolboxWidth = bodyCanvas.ActualWidth / 6;
            double toolboxHeight = bodyCanvas.ActualHeight;

            column0.Width = new GridLength(TaskBoxGrid.ActualWidth, GridUnitType.Pixel);
            column1.Width = new GridLength(_toolboxWidth, GridUnitType.Pixel);
            column2.Width = new GridLength(mainWidth, GridUnitType.Pixel);
            row0.Height = new GridLength(btnCheckScheme.ActualHeight, GridUnitType.Pixel);
            row1.Height = new GridLength(toolboxHeight, GridUnitType.Pixel);

            _mainLeftRightMargin = mainWidth / 20;
            _mainTopBottomMargin = toolboxHeight / 10;
            Main.Margin = new Thickness(_mainLeftRightMargin, _mainTopBottomMargin, _mainLeftRightMargin, _mainTopBottomMargin);
            TaskInitialize();
            CreateInputs();
            CreateOutput();
            CreateBlocks();
        }

        private void TaskInitialize()
        {
            textTaskBox.Margin = new Thickness(TaskBoxGrid.ActualWidth / 10);
            textTaskBox.Text = "На основе таблицы истинности составьте корректную электрическую логическую схему";

            List<Data> _datas = new List<Data>();
            List<string> _headerNames = new List<string>() { "X0", "X1", "X2", "X3", "Y" };
            int[,] _X = new int[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 1 },
                { 0, 0, 1, 0 },
                { 0, 0, 1, 1 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 1 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 1 },
                { 1, 0, 0, 0 },
                { 1, 0, 0, 1 },
                { 1, 0, 1, 0 },
                { 1, 0, 1, 1 },
                { 1, 1, 0, 0 },
                { 1, 1, 0, 1 },
                { 1, 1, 1, 0 },
                { 1, 1, 1, 1 }
            };
            tableTaskBox.Width = TaskBoxGrid.ActualWidth - TaskBoxGrid.ActualWidth / 10;
            foreach (string headerName in _headerNames)
            {
                var column = new DataGridTextColumn();
                column.Header = headerName;
                column.Binding = new Binding(headerName);
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                column.IsReadOnly = true;
                tableTaskBox.Columns.Add(column);
            }

            for (int i = 0; i < _X.GetLength(0); i++)
            {
                _datas.Add(new Data { X0 = _X[i, 0], X1 = _X[i, 1], X2 = _X[i, 2], X3 = _X[i, 3], Y = _schemeTask.ExpectedOutput[i] == 49 ? 1 : 0 });
            }
            tableTaskBox.ItemsSource = _datas;
        }

        private void CreateInputs()
        {
            for (int i = 0; i < _schemeTask.InputsNumber; i++)
            {
                InputElement input = new InputElement(bodyCanvas, _schemeTask.InputsNumber);
                input.AddInput(i, _toolboxWidth, bodyCanvas.ActualHeight, _mainLeftRightMargin);
                input.Body!.PreviewMouseLeftButtonDown += BeginningContact_PreviewMouseLeftButtonDown;
                _inputs.Add(input);
            }
        }

        private void CreateOutput()
        {
            OutputElement output = new OutputElement(bodyCanvas);
            output.AddOutput(this.Height, bodyCanvas.ActualWidth, _mainLeftRightMargin);
            _output = output;
        }

        private void CreateBlocks()
        {
            for (int i = 0; i < _renderedBlocks.Count; i++)
            {
                int logicElementNum = _renderedBlocks[i] == "AND" ? _schemeTask.AndNumber : _schemeTask.OrNumber;
                for (int j = 0; j < logicElementNum; j++)
                {
                    LogicElement element = new LogicElement(bodyCanvas, BeginningContact_PreviewMouseLeftButtonDown, _renderedBlocks[i], i, _toolboxWidth);
                    _logicElements.Add(element);
                    element.AddBlock(j);
                    element.MoveLogicBlockEvents(LogicElement_PreviewMouseLeftButtonDown, LogicElement_PreviewMouseMove,
                        LogicElement_PreviewMouseLeftButtonUp);
                }
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
            // Event triggers at the end of any action (flag is for detecting actions on the next action)
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
                if (IsConnectionLineSelected)
                {
                    SelectedLine?.SetColor(Brushes.Black);
                    IsConnectionLineSelected = false;
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
            // Deleting connection line
            if (e.Key == Key.Delete)
            {
                if (connectionLines.Contains(SelectedLine!))
                {
                    connectionLines.Remove(SelectedLine!);
                    SelectedLine!.Remove();
                    IsConnectionLineSelected = false;
                    SelectedLine = null;
                }
            }
        }

        private void ColorAllEndingContacts(bool isEndingContactSelected)
        {
            foreach(LogicElement elementAND in _logicElements)
            {
                foreach(UIElement input in elementAND.InputsSnap!)
                {
                    Shape inputSnap = (Ellipse)input;
                    inputSnap.Stroke = isEndingContactSelected ? Brushes.Red : Brushes.Transparent;
                }
            }
            Shape outnputSnap = (Polygon)_output!.Body!;
            outnputSnap.Stroke = isEndingContactSelected ? Brushes.Red : Brushes.Black;
        }

        private void AddEventsForBeginningContacts()
        {
            foreach (InputElement input in _inputs)
            {
                input.AddColoringEvent();
            }
            foreach (LogicElement uIElement in _logicElements)
            {
                uIElement.AddOutputSnapColoringEvent();
            }
        }

        private void RemoveEventsForBeginningContacts()
        {
            foreach (InputElement input in _inputs)
            {
                input.RemoveColoringEvent();
            }
            foreach (LogicElement uIElement in _logicElements)
            {
                uIElement.RemoveOutputSnapColoringEvent();
            }
        }

        private void AddEventsForEndingContacts()
        {
            _output!.Body!.MouseLeftButtonUp += EndingContact_PreviewMouseLeftButtonUp;
            foreach (LogicElement uIElement in _logicElements)
            {
                foreach(UIElement outputSnap in uIElement.InputsSnap!)
                {
                    outputSnap.MouseLeftButtonUp += EndingContact_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void RemoveEventsForEndingContacts()
        {
            _output!.Body!.MouseLeftButtonUp -= EndingContact_PreviewMouseLeftButtonUp;
            foreach (LogicElement uIElement in _logicElements)
            {
                foreach (UIElement outputSnap in uIElement.InputsSnap!)
                {
                    outputSnap.MouseLeftButtonUp -= EndingContact_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void EndingContact_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point firstPoint = new Point();
            Point lastPoint = new Point();
            Element? firstElement = null;
            Element? lastElement = null;

            // Connection line first and last points determination
            if (_beginningContact is Polygon)
            {
                Polygon polygon = (Polygon)_beginningContact;
                foreach (InputElement input in _inputs)
                {
                    if (input.Body == polygon)
                        firstElement = input;
                }
                firstPoint = polygon.Points.Last();
            }
            else if(_beginningContact is Ellipse)
            {
                firstPoint = new Point(Canvas.GetLeft(_beginningContact) + LogicElement.SnapCircleDiameter / 2,
                    Canvas.GetTop(_beginningContact) + LogicElement.SnapCircleDiameter / 2);
                foreach (LogicElement logicElement in _logicElements)
                {
                    if (logicElement.LogicBlock!.Contains((Ellipse)_beginningContact))
                        firstElement = logicElement;
                }
            }
            
            if (sender is Polygon)
            {
                Polygon polygon = (Polygon)sender;
                lastElement = _output;
                lastPoint = polygon.Points.Last();
            }
            else if (sender is Ellipse)
            {
                lastPoint = new Point(Canvas.GetLeft((Ellipse)sender) + LogicElement.SnapCircleDiameter / 2,
                    Canvas.GetTop((Ellipse)sender) + LogicElement.SnapCircleDiameter / 2);
                foreach (LogicElement logicElement in _logicElements)
                {
                    if (logicElement.LogicBlock!.Contains((Ellipse)sender))
                        lastElement = logicElement;
                }
            }

            ConnectionLine connectionLine = new ConnectionLine(this, bodyCanvas);
            connectionLine.AddConnectionLine(firstPoint, lastPoint, firstElement, lastElement);

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

        private void LogicElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedLogicElement = (UIElement)sender;

            // Selected logic block searching
            foreach(LogicElement logicBlock in _logicElements)
            {
                if (logicBlock.LogicBlock!.Contains(SelectedLogicElement))
                {
                    _logicElement = logicBlock;
                }
            }
            _logicElementOffset = e.GetPosition(bodyCanvas);
            _logicElementOffset.Y -= Canvas.GetTop(SelectedLogicElement);
            _logicElementOffset.X -= Canvas.GetLeft(SelectedLogicElement);

            // Selected logic block layer prioritization
            if (_logicElement != null)
            {
                foreach (UIElement uIElement in _logicElement.LogicBlock!)
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
            Point cursorPosition = e.GetPosition(bodyCanvas);

            _logicElement.MoveLogicElement(cursorPosition, _logicElementOffset);
        }

        private void LogicElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_logicElement == null)
                return;

            // Moving logic element to toolbox case
            if (Canvas.GetLeft(SelectedLogicElement) < Toolbox.ActualWidth)
            {
                int serialNumber = _logicElement.Remove();
                _logicElement.AddBlock(serialNumber);
                _logicElement.OutputSnap!.PreviewMouseLeftButtonDown += BeginningContact_PreviewMouseLeftButtonDown;
                _logicElement.MoveLogicBlockEvents(LogicElement_PreviewMouseLeftButtonDown, LogicElement_PreviewMouseMove,
                    LogicElement_PreviewMouseLeftButtonUp);
            }

            if (_logicElement != null)
            {
                foreach (UIElement uIElement in _logicElement.LogicBlock!)
                {
                    Panel.SetZIndex(uIElement, 0);
                }
            }
            SelectedLogicElement?.ReleaseMouseCapture();
            SelectedLogicElement = null;
            _logicElement = null;
        }
    }
}
