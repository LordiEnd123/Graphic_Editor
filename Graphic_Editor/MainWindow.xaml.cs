using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Graphic_Editor
{
    public partial class MainWindow : Window
    {
        private enum ToolType { None, Rectangle, Ellipse, Line }
        private ToolType currentTool = ToolType.None;
        private Shape currentShape;
        private Point startPoint;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Выбор инструмента
        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Rectangle;
        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Ellipse;
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Line;
        }

        // Начало рисования
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentTool == ToolType.None)
                return;
            startPoint = e.GetPosition(DrawCanvas);
            switch (currentTool)
            {
                case ToolType.Rectangle:
                    currentShape = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent
                    };
                    break;

                case ToolType.Ellipse:
                    currentShape = new Ellipse
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent
                    };
                    break;

                case ToolType.Line:
                    currentShape = new Line
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 2,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = startPoint.X,
                        Y2 = startPoint.Y
                    };
                    break;
            }

            if (currentShape != null)
            {
                DrawCanvas.Children.Add(currentShape);
            }
        }

        // Рисуем
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentShape == null || e.LeftButton != MouseButtonState.Pressed)
                return;
            Point pos = e.GetPosition(DrawCanvas);
            if (currentShape is Rectangle || currentShape is Ellipse)
            {
                double x = Math.Min(pos.X, startPoint.X);
                double y = Math.Min(pos.Y, startPoint.Y);
                double w = Math.Abs(pos.X - startPoint.X);
                double h = Math.Abs(pos.Y - startPoint.Y);
                Canvas.SetLeft(currentShape, x);
                Canvas.SetTop(currentShape, y);
                currentShape.Width = w;
                currentShape.Height = h;
            }
            else if (currentShape is Line line)
            {
                line.X2 = pos.X;
                line.Y2 = pos.Y;
            }
        }

        // Завершение рисования
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            currentShape = null;
        }
    }
}