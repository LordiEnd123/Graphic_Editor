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

        private readonly ShapeEditor shapeEditor = new ShapeEditor();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Выбор инструментов
        private void RectangleButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Rectangle;
        private void EllipseButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Ellipse;
        private void LineButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Line;
        private void PolygonButton_Click(object sender, RoutedEventArgs e) => currentTool = ToolType.Polygon;

        private (Brush stroke, Brush fill) GetSelectedColors()
        {
            var strokeItem = StrokeColorPicker.SelectedItem as ComboBoxItem;
            var fillItem = FillColorPicker.SelectedItem as ComboBoxItem;

            string strokeColor = strokeItem?.Tag?.ToString() ?? "Black";
            string fillColor = fillItem?.Tag?.ToString() ?? "Transparent";

            Brush stroke = (Brush)new BrushConverter().ConvertFromString(strokeColor);
            Brush fill = (Brush)new BrushConverter().ConvertFromString(fillColor);

            return (stroke, fill);
        }

        // Рисование
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(DrawCanvas);

            if (currentTool == ToolType.None)
            {
                shapeEditor.SelectShape(DrawCanvas, e);
                return;
            }
            shapeEditor.Deselect();

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
            var (stroke, fill) = GetSelectedColors();
            currentShape = shapeDrawer.CreateShape(currentTool, startPoint);

            if (currentShape != null)
            {
                currentShape.Stroke = stroke;
                currentShape.Fill = currentShape is Line ? Brushes.Transparent : fill;
                DrawCanvas.Children.Add(currentShape);
            }

        }


        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(DrawCanvas);

            if (currentTool == ToolType.None)
            {
                shapeEditor.MoveShape(e, DrawCanvas);
                return;
            }

            if (currentTool == ToolType.Polygon)
            {
                polygonDrawer.ShowPreview(pos, DrawCanvas);
                return;
            }

            if (currentShape == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            shapeDrawer.UpdateShape(currentShape, startPoint, pos);
        }

        private void ApplyColor_Click(object sender, RoutedEventArgs e)
        {
            if (shapeEditor.SelectedShape == null)
            {
                MessageBox.Show("Выберите фигуру, чтобы изменить цвет.", "Нет выделения", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Получаем выбранные цвета
            string strokeColor = ((ComboBoxItem)StrokeColorPicker.SelectedItem).Tag.ToString();
            string fillColor = ((ComboBoxItem)FillColorPicker.SelectedItem).Tag.ToString();

            Brush stroke = (Brush)new BrushConverter().ConvertFromString(strokeColor);
            Brush fill = (Brush)new BrushConverter().ConvertFromString(fillColor);

            shapeEditor.ApplyColor(stroke, fill);
        }


        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentTool == ToolType.Polygon)
                polygonDrawer.Finish(DrawCanvas);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            shapeEditor.StopMoving();
            currentShape = null;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.None;
        }

        private void DrawCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (currentTool == ToolType.None)
            {
                shapeEditor.ScaleShape(e);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            shapeEditor.DeleteSelected(DrawCanvas);
        }


    }
}
