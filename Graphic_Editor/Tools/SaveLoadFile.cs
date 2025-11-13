using Microsoft.Win32;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Graphic_Editor.Tools
{
    public static class SaveLoadFile
    {
        // Сохранение проекта
        public static void Save(Canvas canvas)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Graphic Editor Project (*.geproj)|*.geproj",
                FileName = "project.geproj"
            };

            if (dialog.ShowDialog() != true)
                return;

            XElement root = new XElement("Canvas");
            foreach (var shape in canvas.Children.OfType<Shape>().Where(s => s.IsHitTestVisible))
            {
                XElement element = null;

                if (shape is Rectangle r)
                {
                    double left = Canvas.GetLeft(r);
                    double top = Canvas.GetTop(r);
                    if (double.IsNaN(left)) left = 0;
                    if (double.IsNaN(top)) top = 0;

                    double width = double.IsNaN(r.Width) ? r.RenderSize.Width : r.Width;
                    double height = double.IsNaN(r.Height) ? r.RenderSize.Height : r.Height;

                    if (width < 1 || height < 1)
                        continue;

                    element = new XElement("Rectangle",
                        new XAttribute("X", left.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Y", top.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Width", r.Width.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Height", r.Height.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Stroke", r.Stroke.ToString()),
                        new XAttribute("StrokeThickness", r.StrokeThickness.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Fill", r.Fill.ToString()));
                }
                else if (shape is Ellipse e)
                {
                    double left = Canvas.GetLeft(e);
                    double top = Canvas.GetTop(e);
                    if (double.IsNaN(left)) left = 0;
                    if (double.IsNaN(top)) top = 0;

                    double width = double.IsNaN(e.Width) ? e.RenderSize.Width : e.Width;
                    double height = double.IsNaN(e.Height) ? e.RenderSize.Height : e.Height;

                    if (width < 1 || height < 1)
                        continue;

                    element = new XElement("Ellipse",
                        new XAttribute("X", left.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Y", top.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Width", e.Width.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Height", e.Height.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Stroke", e.Stroke.ToString()),
                        new XAttribute("StrokeThickness", e.StrokeThickness.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Fill", e.Fill.ToString()));
                }
                else if (shape is Line l)
                {
                    element = new XElement("Line",
                        new XAttribute("X1", l.X1.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Y1", l.Y1.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("X2", l.X2.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Y2", l.Y2.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("Stroke", l.Stroke.ToString()),
                        new XAttribute("StrokeThickness", l.StrokeThickness.ToString(CultureInfo.InvariantCulture))
                    );
                }

                else if (shape is Polygon p)
                {
                    double left = Canvas.GetLeft(p);
                    double top = Canvas.GetTop(p);
                    if (double.IsNaN(left)) left = 0;
                    if (double.IsNaN(top)) top = 0;

                    var pointsStr = string.Join(" ",
                        p.Points.Select(pt =>
                            string.Format(CultureInfo.InvariantCulture, "{0},{1}", pt.X + left, pt.Y + top)));

                    if (p.Points.Count >= 3)
                    {
                        element = new XElement("Polygon",
                            new XAttribute("Points", pointsStr),
                            new XAttribute("Stroke", p.Stroke.ToString()),
                            new XAttribute("StrokeThickness", p.StrokeThickness.ToString(CultureInfo.InvariantCulture)),
                            new XAttribute("Fill", p.Fill.ToString()));
                    }
                }

                if (element != null)
                    root.Add(element);
            }

            root.Save(dialog.FileName);
            MessageBox.Show("Проект успешно сохранён", "Сохранение завершено",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Загрузка проекта
        public static void Load(Canvas canvas)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Graphic Editor Project (*.geproj)|*.geproj"
            };

            if (dialog.ShowDialog() != true)
                return;

            canvas.Children.Clear();
            XElement root = XElement.Load(dialog.FileName);

            foreach (var element in root.Elements())
            {
                Shape shape = null;

                switch (element.Name.LocalName)
                {
                    case "Rectangle":
                        shape = new Rectangle
                        {
                            Width = SafeSize(element, "Width"),
                            Height = SafeSize(element, "Height"),
                            Stroke = (Brush)new BrushConverter().ConvertFromString(element.Attribute("Stroke").Value),
                            StrokeThickness = SafeDouble(element, "StrokeThickness"),
                            Fill = (Brush)new BrushConverter().ConvertFromString(element.Attribute("Fill").Value)
                        };
                        Canvas.SetLeft(shape, SafeDouble(element, "X"));
                        Canvas.SetTop(shape, SafeDouble(element, "Y"));
                        break;

                    case "Ellipse":
                        shape = new Ellipse
                        {
                            Width = SafeSize(element, "Width"),
                            Height = SafeSize(element, "Height"),
                            Stroke = (Brush)new BrushConverter().ConvertFromString(element.Attribute("Stroke").Value),
                            StrokeThickness = SafeDouble(element, "StrokeThickness"),
                            Fill = (Brush)new BrushConverter().ConvertFromString(element.Attribute("Fill").Value)
                        };
                        Canvas.SetLeft(shape, SafeDouble(element, "X"));
                        Canvas.SetTop(shape, SafeDouble(element, "Y"));
                        break;

                    case "Line":
                        shape = new Line
                        {
                            X1 = SafeDouble(element, "X1"),
                            Y1 = SafeDouble(element, "Y1"),
                            X2 = SafeDouble(element, "X2"),
                            Y2 = SafeDouble(element, "Y2"),
                            Stroke = (Brush)new BrushConverter().ConvertFromString(element.Attribute("Stroke").Value),
                            StrokeThickness = SafeDouble(element, "StrokeThickness")
                        };
                        break;

                    case "Polygon":
                        {
                            var poly = new Polygon
                            {
                                Stroke = (Brush)new BrushConverter().ConvertFromString(element.Attribute("Stroke").Value),
                                StrokeThickness = SafeDouble(element, "StrokeThickness"),
                                Fill = (Brush)new BrushConverter().ConvertFromString(element.Attribute("Fill").Value)
                            };

                            var raw = element.Attribute("Points")?.Value ?? string.Empty;
                            var pairs = raw.Split(new[] { ' ', '\t', '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

                            foreach (var pair in pairs)
                            {
                                var parts = pair.Split(',');
                                if (parts.Length != 2) continue;

                                if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) && double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                                {
                                    poly.Points.Add(new Point(x, y));
                                }
                            }

                            if (poly.Points.Count >= 3)
                            {
                                Canvas.SetLeft(poly, 0);
                                Canvas.SetTop(poly, 0);
                                shape = poly;
                            }
                            break;
                        }
                }

                if (shape != null)
                    canvas.Children.Add(shape);
            }

            MessageBox.Show("Проект успешно загружен", "Загрузка завершена",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Вспомогательные
        private static double SafeDouble(XElement el, string attr)
        {
            var a = el.Attribute(attr);
            if (a == null) return 0;
            return double.TryParse(a.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double v) ? v : 0;
        }

        private static double SafeSize(XElement el, string attr)
        {
            double v = SafeDouble(el, attr);
            return v > 0 ? v : 10;
        }
    }
}
