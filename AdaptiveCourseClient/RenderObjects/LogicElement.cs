using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class LogicElement
    {
        public UIElementGroup? LogicBlock { get; set; }
        public Ellipse? OutputSnap { get; set; }
        public UIElementGroup? InputsSnap { get; set; }

        private Rectangle? _body;
        private Shape? _negativeOutputCircle;
        private Canvas? _canvas;

        public static readonly int OutputCircleDiameter = 16;
        public static readonly int SnapCircleDiameter = 16;

        private static readonly int _bodyWidth = 50;
        private static readonly int _bodyHeight = 70;
        private static readonly int _bodyInitialX = 50;
        private static readonly int _bodyInitialY = 50;
        private static readonly int _contactWidth = 8;
        private static readonly int _inputsNumber = 2;

        public void AddBlock(Canvas canvas)
        {
            LogicBlock = new UIElementGroup();
            InputsSnap = new UIElementGroup();
            _canvas = canvas;

            // Main body creation
            _body = Figures.AddBody(_bodyWidth, _bodyHeight);
            _body.PreviewMouseDown += Body_PreviewMouseDown;
            Canvas.SetLeft(_body, _bodyInitialX);
            Canvas.SetTop(_body, _bodyInitialY);
            LogicBlock.Add(_body);
            canvas.Children.Add(_body);

            // Inputs creation
            for (int i = 0; i < _inputsNumber; i++)
            {
                double relativeInputY = _bodyHeight * ((double)(i + 1) / (_inputsNumber + 1));
                Line leftInputLine = Figures.AddContact();
                leftInputLine.X1 = _bodyInitialX - _contactWidth;
                leftInputLine.Y1 = _bodyInitialY + (relativeInputY);
                leftInputLine.X2 = _bodyInitialX;
                leftInputLine.Y2 = _bodyInitialY + (relativeInputY);
                LogicBlock.Add(leftInputLine);
                canvas.Children.Add(leftInputLine);
            }

            // Output creation
            Line rightLine = Figures.AddContact();
            rightLine.X1 = _bodyInitialX + _bodyWidth;
            rightLine.Y1 = _bodyInitialY + ((double)_bodyHeight / 2);
            rightLine.X2 = _bodyInitialX + _bodyWidth + _contactWidth;
            rightLine.Y2 = _bodyInitialY + ((double)_bodyHeight / 2);
            LogicBlock.Add(rightLine);
            canvas.Children.Add(rightLine);
            _negativeOutputCircle = null;
        }

        public void AddOutputSnap(Canvas canvas)
        {
            Ellipse rightInputSnap = Figures.AddInputSnapCircle(SnapCircleDiameter);
            Canvas.SetLeft(rightInputSnap, _bodyInitialX + _bodyWidth);
            Canvas.SetTop(rightInputSnap, _bodyInitialY + (double)(_bodyHeight / 2) -
                (double)(SnapCircleDiameter / 2));
            LogicBlock?.Add(rightInputSnap);
            canvas.Children.Add(rightInputSnap);
            this.OutputSnap = rightInputSnap;
        }

        public void AddInputsSnap(Canvas canvas)
        {
            for(int i = 0; i < _inputsNumber; i++)
            {
                Ellipse leftInputSnap = Figures.AddInputSnapCircle(SnapCircleDiameter);
                Canvas.SetLeft(leftInputSnap, _bodyInitialX - SnapCircleDiameter);
                Canvas.SetTop(leftInputSnap, _bodyInitialY + _bodyHeight * ((double)(i + 1) / (_inputsNumber + 1)) - 
                    (double)(SnapCircleDiameter / 2));
                LogicBlock.Add(leftInputSnap);
                canvas.Children.Add(leftInputSnap);
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

        public void MoveLogicBlock(MouseButtonEventHandler LogicElementAND_PreviewMouseLeftButtonDown,
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
            double outputX = Canvas.GetLeft(_body) + _bodyWidth;
            double outputY = Canvas.GetTop(_body) + ((double)_bodyHeight / 2);
            Ellipse circle = Figures.AddOutputCircle(OutputCircleDiameter);
            Canvas.SetTop(circle, outputY - OutputCircleDiameter / 2);
            Canvas.SetLeft(circle, outputX - OutputCircleDiameter * 2 / 3);

            LogicBlock?.Add(circle);
            _negativeOutputCircle = circle;
            _canvas?.Children.Add(circle);
        }
    }
}
