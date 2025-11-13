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

        private readonly ActionHistory history = new ActionHistory();

        public MainWindow()
        {
            InitializeComponent();

            DrawCanvas.Tag = history;
            this.KeyDown += (s, e) =>
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
                {
                    history.Undo(DrawCanvas);
                }
            };
        }

        // Выбор инструментов
        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            currentTool = ToolType.Rectangle;
        }
        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            currentTool = ToolType.Ellipse;
        }
        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            currentTool = ToolType.Line;
        }
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
            var (stroke, fill) = GetSelectedColors();

            if (currentTool == ToolType.None)
            {
                shapeEditor.SelectShape(DrawCanvas, e);
                return;
            }
            shapeEditor.Deselect();

            if (currentTool == ToolType.Polygon)
            {
                polygonDrawer.Stroke = stroke;
                polygonDrawer.Fill = fill;

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
            {
                currentShape.Stroke = stroke;
                currentShape.Fill = currentShape is Line ? Brushes.Transparent : fill;
                DrawCanvas.Children.Add(currentShape);
                history.SaveState(DrawCanvas);
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

            history.SaveState(DrawCanvas);

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
            {
                polygonDrawer.Finish(DrawCanvas);
                history.SaveState(DrawCanvas);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            shapeEditor.StopMoving();
            currentShape = null;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            currentTool = ToolType.None;
        }

        private double zoom = 1.0;
        private const double MinZoom = 0.3;
        private const double MaxZoom = 5.0;

        private void DrawCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Масштаб фигуры
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                shapeEditor.ScaleShape(e);
                e.Handled = true;
                return;
            }

            // Масштабируем весь холст
            double scale = e.Delta > 0 ? 1.1 : 0.9;
            double newZoom = zoom * scale;
            if (newZoom < MinZoom || newZoom > MaxZoom)
                return;
    
            zoom = newZoom;
            DrawCanvas.LayoutTransform = new ScaleTransform(zoom, zoom);

            e.Handled = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            history.SaveState(DrawCanvas);
            shapeEditor.DeleteSelected(DrawCanvas);
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            history.Undo(DrawCanvas);
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            SaveLoadFile.Save(DrawCanvas);
        }

        private void LoadProject_Click(object sender, RoutedEventArgs e)
        {
            polygonDrawer.Cancel(DrawCanvas);
            SaveLoadFile.Load(DrawCanvas);
        }

    }
}
