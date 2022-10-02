using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class ElementAND
    {
        private Rectangle bodyElement;
        private Shape rightInput;
        public Ellipse rightInputAround;
        private Canvas Canvas;
        public UIElementGroup logicBlock;

        public static int circleDiameter = 16;
        private static int circleAroundDiameter = 15;
        private static int mainBodyWidth = 50;
        private static int mainBodyHeight = 70;
        private static int mainDodyInitialX = 50;
        private static int mainDodyInitialY = 50;
        private static int inputLineWidth = 8;
        private static int leftInputsNumber = 2;

        public void AddAND(Canvas canvas)
        {
            logicBlock = new UIElementGroup();

            this.Canvas = canvas;
            //Main body
            bodyElement = AddRectangle();
            Canvas.SetLeft(bodyElement, mainDodyInitialX);
            Canvas.SetTop(bodyElement, mainDodyInitialY);
            logicBlock.Add(bodyElement);
            canvas.Children.Add(bodyElement);

            //Inputs
            for (int i = 0; i < leftInputsNumber; i++)
            {
                double relativeHeight = mainBodyHeight * ((double)(i + 1) / (leftInputsNumber + 1));
                //Inputs
                Line leftInputLine = AddLine();
                leftInputLine.X1 = mainDodyInitialX - inputLineWidth;
                leftInputLine.Y1 = mainDodyInitialY + (relativeHeight);
                leftInputLine.X2 = mainDodyInitialX;
                leftInputLine.Y2 = mainDodyInitialY + (relativeHeight);
                logicBlock.Add(leftInputLine);
                canvas.Children.Add(leftInputLine);
            }

            Line rightLine = AddLine();
            rightLine.X1 = mainDodyInitialX + mainBodyWidth;
            rightLine.Y1 = mainDodyInitialY + ((double)mainBodyHeight / 2);
            rightLine.X2 = mainDodyInitialX + mainBodyWidth + inputLineWidth;
            rightLine.Y2 = mainDodyInitialY + ((double)mainBodyHeight / 2);
            logicBlock.Add(rightLine);
            canvas.Children.Add(rightLine);
            rightInput = rightLine;
        }

        public UIElement AddRightInputAround(Canvas canvas)
        {
            Ellipse rightInputAround = AddInputAroundCircle();
            Canvas.SetLeft(rightInputAround, mainDodyInitialX + mainBodyWidth);
            Canvas.SetTop(rightInputAround, mainDodyInitialY + (double)(mainBodyHeight / 2) -
                (double)(circleAroundDiameter / 2));
            logicBlock.Add(rightInputAround);
            canvas.Children.Add(rightInputAround);
            this.rightInputAround = rightInputAround;
            return rightInputAround;
        }

        public void MoveLogicBlock(MouseButtonEventHandler LogicElementAND_PreviewMouseLeftButtonDown,
            MouseEventHandler LogicElement_PreviewMouseMove, MouseButtonEventHandler LogicElement_PreviewMouseLeftButtonUp)
        {
            bodyElement.PreviewMouseLeftButtonDown += LogicElementAND_PreviewMouseLeftButtonDown;
            bodyElement.PreviewMouseMove += LogicElement_PreviewMouseMove;
            bodyElement.PreviewMouseLeftButtonUp += LogicElement_PreviewMouseLeftButtonUp;
            bodyElement.PreviewMouseDown += Body_PreviewMouseDown;
        }

        public void AddInputsAroundColoring()
        {
            rightInputAround.MouseLeave += RightInput_MouseLeave;
            rightInputAround.MouseMove += RightInput_MouseMove;
        }

        public void RemoveInputsAroundColoring()
        {
            rightInputAround.MouseLeave -= RightInput_MouseLeave;
            rightInputAround.MouseMove -= RightInput_MouseMove;
        }

        private void RightInput_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender == rightInputAround)
            {
                rightInputAround.Stroke = Brushes.Transparent;
            }
        }

        private void RightInput_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender == rightInputAround)
            {
                rightInputAround.Stroke = Brushes.Red;
            }
        }

        private void Body_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                if (rightInput is Ellipse)
                {
                    ChangeEllipseToLine((Ellipse)rightInput);
                }
                else if (rightInput is Line)
                {
                    ChangeLineToEllipse((Line)rightInput);
                }
            }
        }

        private void ChangeEllipseToLine(Ellipse rightEllipse)
        {
            double elementPositionX = Canvas.GetLeft(rightEllipse);
            double elementPositionY = Canvas.GetTop(rightEllipse);
            Line line = AddLine();
            line.Y1 = elementPositionY + circleDiameter / 2;
            line.Y2 = elementPositionY + circleDiameter / 2;
            line.X1 = elementPositionX + circleDiameter / 2;
            line.X2 = elementPositionX + circleDiameter;

            int index = Canvas.Children.IndexOf(rightEllipse);
            Canvas.Children.Remove(rightEllipse);
            logicBlock.Replace(rightEllipse, line);
            rightInput = line;
            Canvas.Children.Insert(index, line);
        }

        private void ChangeLineToEllipse(Line rightLine)
        {
            double elementPositionX = rightLine.X1;
            double elementPositionY = rightLine.Y1;
            Ellipse circle = AddInputCircle();
            Canvas.SetTop(circle, elementPositionY - circleDiameter / 2);
            Canvas.SetLeft(circle, elementPositionX - circleDiameter / 2);

            int index = Canvas.Children.IndexOf(rightLine);
            Canvas.Children.Remove(rightLine);
            logicBlock.Replace(rightLine, circle);
            rightInput = circle;
            Canvas.Children.Insert(index, circle);
        }

        private Ellipse AddInputCircle()
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = circleDiameter;
            Circle.Height = circleDiameter;
            Circle.Fill = Brushes.White;
            Circle.Stroke = Brushes.Black;
            Circle.StrokeThickness = 3;
            return Circle;
        }

        private Ellipse AddInputAroundCircle()
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = circleAroundDiameter;
            Circle.Height = circleAroundDiameter;
            Circle.Fill = Brushes.Transparent;
            Circle.Stroke = Brushes.Transparent;
            Circle.StrokeThickness = 1;
            return Circle;
        }

        private Line AddLine()
        {
            Line line = new Line();
            line.Fill = Brushes.Black;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3;
            return line;
        }

        private Rectangle AddRectangle()
        {
            Rectangle rectangle = new Rectangle();
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri(@"../../../../images/and.png", UriKind.Relative));
            rectangle.Width = mainBodyWidth;
            rectangle.Height = mainBodyHeight;
            rectangle.RadiusX = 3;
            rectangle.RadiusY = 3;
            rectangle.Fill = imgBrush;
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 5;
            return rectangle;
        }
    }
}
