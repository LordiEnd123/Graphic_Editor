using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Graphic_Editor.Tools
{
    public class ShapeEditor
    {
        private Shape selectedShape;
        private bool isDragging = false;
        private Point dragStart;

        public Shape SelectedShape => selectedShape;

        public void ApplyColor(Brush stroke, Brush fill)
        {
            if (selectedShape == null) return;

            selectedShape.Stroke = stroke;
            if (!(selectedShape is Line))
                selectedShape.Fill = fill;
        }


        // Выбор фигуры
        public void SelectShape(Canvas canvas, MouseButtonEventArgs e)
        {
            var shape = e.OriginalSource as Shape;
            if (shape != null && canvas.Children.Contains(shape))
            {
                if (selectedShape != null)
                {
                    selectedShape.StrokeThickness = 2;
                    selectedShape.StrokeDashArray = null;
                }

                selectedShape = shape;
                selectedShape.StrokeThickness = 3;
                selectedShape.StrokeDashArray = new DoubleCollection { 2, 2 };
                dragStart = e.GetPosition(canvas);
                isDragging = true;
            }
            else
            {
                Deselect();
            }
        }

        public void Deselect()
        {
            if (selectedShape != null)
            {
                selectedShape.StrokeThickness = 2;
                selectedShape.StrokeDashArray = null;
                selectedShape = null;
            }
            isDragging = false;
        }

        // Перемещение
        public void MoveShape(MouseEventArgs e, Canvas canvas)
        {
            if (!isDragging || selectedShape == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            Point pos = e.GetPosition(canvas);
            double dx = pos.X - dragStart.X;
            double dy = pos.Y - dragStart.Y;

            if (selectedShape is Line line)
            {
                line.X1 += dx;
                line.Y1 += dy;
                line.X2 += dx;
                line.Y2 += dy;
            }
            else if (selectedShape is Polygon polygon)
            {
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    polygon.Points[i] = new Point(polygon.Points[i].X + dx,polygon.Points[i].Y + dy);
                }
            }
            else
            {
                double left = Canvas.GetLeft(selectedShape);
                double top = Canvas.GetTop(selectedShape);
                Canvas.SetLeft(selectedShape, left + dx);
                Canvas.SetTop(selectedShape, top + dy);
            }
            dragStart = pos;
        }

        public void StopMoving()
        {
            isDragging = false;
        }


        public void ScaleShape(MouseWheelEventArgs e)
        {
            if (selectedShape == null)
                return;

            double scale = e.Delta > 0 ? 1.1 : 0.9;

            if (selectedShape is Line line)
            {
                // Масштабируем линию относительно её центра
                double centerX = (line.X1 + line.X2) / 2;
                double centerY = (line.Y1 + line.Y2) / 2;

                line.X1 = centerX + (line.X1 - centerX) * scale;
                line.Y1 = centerY + (line.Y1 - centerY) * scale;
                line.X2 = centerX + (line.X2 - centerX) * scale;
                line.Y2 = centerY + (line.Y2 - centerY) * scale;
            }
            else if (selectedShape is Polygon polygon)
            {
                // Масштабируем многоугольник относительно центра
                double centerX = polygon.Points.Average(p => p.X);
                double centerY = polygon.Points.Average(p => p.Y);

                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    double newX = centerX + (polygon.Points[i].X - centerX) * scale;
                    double newY = centerY + (polygon.Points[i].Y - centerY) * scale;
                    polygon.Points[i] = new Point(newX, newY);
                }
            }
            else
            {
                // Масштабируем эллипс и прямоугольник относительно центра
                double left = Canvas.GetLeft(selectedShape);
                double top = Canvas.GetTop(selectedShape);
                double width = selectedShape.Width;
                double height = selectedShape.Height;

                double centerX = left + width / 2;
                double centerY = top + height / 2;

                width *= scale;
                height *= scale;

                double newLeft = centerX - width / 2;
                double newTop = centerY - height / 2;

                selectedShape.Width = width;
                selectedShape.Height = height;

                Canvas.SetLeft(selectedShape, newLeft);
                Canvas.SetTop(selectedShape, newTop);
            }
        }


        public void DeleteSelected(Canvas canvas)
        {
            if (selectedShape != null)
            {
                canvas.Children.Remove(selectedShape);
                selectedShape = null;
                isDragging = false;
            }
        }



    }
}
