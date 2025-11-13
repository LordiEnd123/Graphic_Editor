using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Graphic_Editor.Tools
{
    public class PolygonDrawer
    {
        private readonly List<Point> points = new List<Point>();
        private Line previewLine;
        public bool IsDrawing => points.Count > 0;
        public Brush Stroke { get; set; } = Brushes.Purple;
        public Brush Fill { get; set; } = Brushes.Transparent;
        public void Start(Point startPoint)
        {
            points.Clear();
            points.Add(startPoint);
        }

        public void AddPoint(Point nextPoint, Canvas canvas)
        {
            if (points.Count > 0)
            {
                Point last = points.Last();
                Line edge = new Line
                {
                    X1 = last.X,
                    Y1 = last.Y,
                    X2 = nextPoint.X,
                    Y2 = nextPoint.Y,
                    Stroke = Stroke,
                    StrokeThickness = 2,
                    IsHitTestVisible = false
                };
                canvas.Children.Add(edge);
            }
            points.Add(nextPoint);
        }

        public void ShowPreview(Point mousePos, Canvas canvas)
        {
            if (points.Count == 0) return;
            Point last = points.Last();

            if (previewLine == null)
            {
                previewLine = new Line
                {
                    Stroke = Stroke,
                    StrokeDashArray = new DoubleCollection { 2 },
                    StrokeThickness = 1,
                    IsHitTestVisible = false
                };
                canvas.Children.Add(previewLine);
            }
            previewLine.X1 = last.X;
            previewLine.Y1 = last.Y;
            previewLine.X2 = mousePos.X;
            previewLine.Y2 = mousePos.Y;
        }

        public void Finish(Canvas canvas)
        {
            if (points.Count < 3)
            {
                points.Clear();
                return;
            }

            if (previewLine != null)
            {
                canvas.Children.Remove(previewLine);
                previewLine = null;
            }

            var tempLines = canvas.Children.OfType<Line>().Where(l => !l.IsHitTestVisible).ToList();

            foreach (var line in tempLines)
                canvas.Children.Remove(line);

            // Создаём финальную фигуру
            Polygon polygon = new Polygon
            {
                Stroke = Stroke,
                StrokeThickness = 2,
                Fill = Fill,
                IsHitTestVisible = true
            };

            foreach (var p in points)
                polygon.Points.Add(p);

            canvas.Children.Add(polygon);

            if (canvas.Tag is ActionHistory history)
                history.SaveState(canvas);

            points.Clear();

        }

        public void Cancel(Canvas canvas)
        {
            points.Clear();

            if (previewLine != null)
            {
                canvas.Children.Remove(previewLine);
                previewLine = null;
            }

            var tempLines = canvas.Children.OfType<Line>().Where(l => !l.IsHitTestVisible).ToList();
            foreach (var line in tempLines)
                canvas.Children.Remove(line);
        }

    }
}
