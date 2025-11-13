using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Graphic_Editor.Tools
{
    public class ActionHistory
    {
        private readonly Stack<List<Shape>> undoStack = new Stack<List<Shape>>();
        private const int MaxSteps = 5;

        public void SaveState(Canvas canvas)
        {
            var snapshot = new List<Shape>();
            foreach (var shape in canvas.Children)
            {
                if (shape is Shape s)
                    snapshot.Add(CloneShape(s));
            }
            undoStack.Push(snapshot);
            if (undoStack.Count > MaxSteps)
                undoStack.TrimExcess();
        }

        public void Undo(Canvas canvas)
        {
            if (undoStack.Count == 0)
                return;

            var lastState = undoStack.Pop();
            canvas.Children.Clear();

            foreach (var s in lastState)
                canvas.Children.Add(s);
        }

        // Вспомогательный метод для копирования фигур
        private Shape CloneShape(Shape shape)
        {
            Shape clone = null;

            switch (shape)
            {
                case Rectangle r:
                    clone = new Rectangle
                    {
                        Width = r.Width,
                        Height = r.Height,
                        Stroke = r.Stroke,
                        StrokeThickness = r.StrokeThickness,
                        Fill = r.Fill
                    };
                    break;

                case Ellipse e:
                    clone = new Ellipse
                    {
                        Width = e.Width,
                        Height = e.Height,
                        Stroke = e.Stroke,
                        StrokeThickness = e.StrokeThickness,
                        Fill = e.Fill
                    };
                    break;

                case Line l:
                    clone = new Line
                    {
                        X1 = l.X1,
                        Y1 = l.Y1,
                        X2 = l.X2,
                        Y2 = l.Y2,
                        Stroke = l.Stroke,
                        StrokeThickness = l.StrokeThickness
                    };
                    break;

                case Polygon p:
                    clone = new Polygon
                    {
                        Stroke = p.Stroke,
                        StrokeThickness = p.StrokeThickness,
                        Fill = p.Fill
                    };
                    foreach (var point in p.Points)
                        ((Polygon)clone).Points.Add(point);
                    break;
            }

            if (clone != null)
            {
                Canvas.SetLeft(clone, Canvas.GetLeft(shape));
                Canvas.SetTop(clone, Canvas.GetTop(shape));
            }

            return clone;
        }
    }
}
