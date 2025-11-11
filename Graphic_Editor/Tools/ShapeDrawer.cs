using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Graphic_Editor.Tools
{
    public class ShapeDrawer
    {
        public Shape CreateShape(ToolType type, Point startPoint)
        {
            switch (type)
            {
                case ToolType.Rectangle:
                    return new Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent
                    };

                case ToolType.Ellipse:
                    return new Ellipse
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent
                    };

                case ToolType.Line:
                    return new Line
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 2,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = startPoint.X,
                        Y2 = startPoint.Y
                    };

                default:
                    return null;
            }
        }

        public void UpdateShape(Shape shape, Point startPoint, Point pos)
        {
            if (shape is Rectangle || shape is Ellipse)
            {
                double x = Math.Min(pos.X, startPoint.X);
                double y = Math.Min(pos.Y, startPoint.Y);
                double w = Math.Abs(pos.X - startPoint.X);
                double h = Math.Abs(pos.Y - startPoint.Y);
                Canvas.SetLeft(shape, x);
                Canvas.SetTop(shape, y);
                shape.Width = w;
                shape.Height = h;
            }
            else if (shape is Line line)
            {
                line.X2 = pos.X;
                line.Y2 = pos.Y;
            }
        }
    }
}
