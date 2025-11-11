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
        private List<Point> points = new List<Point>();
        private Line previewLine;

        public bool IsDrawing => points.Count > 0;

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
                    Stroke = Brushes.Purple,
                    StrokeThickness = 2
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
                    Stroke = Brushes.Gray,
                    StrokeDashArray = new DoubleCollection { 2 },
                    StrokeThickness = 1
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
            if (points.Count < 3) return;
            if (previewLine != null)
            {
                canvas.Children.Remove(previewLine);
                previewLine = null;
            }

            Polygon polygon = new Polygon
            {
                Stroke = Brushes.Purple,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            foreach (var p in points)
                polygon.Points.Add(p);

            var linesToRemove = canvas.Children.OfType<Line>().Where(l => l.Stroke == Brushes.Purple && l.StrokeThickness == 2).ToList();

            foreach (var l in linesToRemove)
                canvas.Children.Remove(l);
            canvas.Children.Add(polygon);
            points.Clear();
        }
    }
}
