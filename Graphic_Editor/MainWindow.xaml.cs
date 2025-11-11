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
using Graphic_Editor.Tools;

namespace Graphic_Editor
{
    public partial class MainWindow : Window
    {
        private ToolType currentTool = ToolType.None;
        private Shape currentShape;
        private Point startPoint;

        private readonly ShapeDrawer shapeDrawer = new ShapeDrawer();
        private readonly PolygonDrawer polygonDrawer = new PolygonDrawer();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Выбор инструментов
        private void RectangleButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Rectangle;
        private void EllipseButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Ellipse;
        private void LineButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Line;
        private void PolygonButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Polygon;

        // Рисование
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(DrawCanvas);

            if (currentTool == ToolType.Polygon)
            {
                if (!polygonDrawer.IsDrawing)
                    polygonDrawer.Start(pos);
                else
                    polygonDrawer.AddPoint(pos, DrawCanvas);
                return;
            }

            if (currentTool == ToolType.None)
                return;

            startPoint = pos;
            currentShape = shapeDrawer.CreateShape(currentTool, startPoint);
            if (currentShape != null)
                DrawCanvas.Children.Add(currentShape);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(DrawCanvas);

            if (currentTool == ToolType.Polygon)
            {
                polygonDrawer.ShowPreview(pos, DrawCanvas);
                return;
            }

            if (currentShape == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            shapeDrawer.UpdateShape(currentShape, startPoint, pos);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentTool == ToolType.Polygon)
                polygonDrawer.Finish(DrawCanvas);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            currentShape = null;
        }
    }
}
