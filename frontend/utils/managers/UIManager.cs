using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp2.frontend.blocks;


namespace WpfApp2.frontend.utils
{
    public class UIManager
    {
        public List<Block> blocks = new List<Block>();
        public Block sourceBlock;
        public Canvas WorkspaceCanvas;
        Window window;
        public bool? IsTSelect { get; set; }
        public UIManager(Canvas workspace)
        {
            this.WorkspaceCanvas = workspace;
            this.window = Application.Current.MainWindow;
            this.IsTSelect = null;
        }

        private T FindVisualChild<T>(DependencyObject parent, Block block) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T tChild)
                {
                    if (tChild is Border b && b.Tag == block)
                    {
                        return tChild;
                    }
                }

                var result = FindVisualChild<T>(child, block);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        public void DrawArrow(Block fromBlock, Block toBlock, Brush arrowColor)
        {
            var fromElement = FindVisualChild<Border>(WorkspaceCanvas, fromBlock);
            var toElement = FindVisualChild<Border>(WorkspaceCanvas, toBlock);

            if (fromElement != null && toElement != null)
            {
                var existingArrow = WorkspaceCanvas.Children.OfType<Line>().FirstOrDefault(line => line.Tag is Tuple<Block, Block> tuple &&
                            tuple.Item1 == fromBlock && tuple.Item2 == toBlock);

                if (existingArrow != null)
                {
                    existingArrow.X1 = Canvas.GetLeft(fromElement) + fromElement.ActualWidth / 2;
                    existingArrow.Y1 = Canvas.GetTop(fromElement) + fromElement.ActualHeight / 2;
                    existingArrow.X2 = Canvas.GetLeft(toElement) + toElement.ActualWidth / 2;
                    existingArrow.Y2 = Canvas.GetTop(toElement) + toElement.ActualHeight / 2;
                }
                else
                {
                    var line = new Line
                    {
                        Stroke = arrowColor,
                        StrokeThickness = 2,
                        X1 = Canvas.GetLeft(fromElement) + fromElement.ActualWidth / 2,
                        Y1 = Canvas.GetTop(fromElement) + fromElement.ActualHeight / 2,
                        X2 = Canvas.GetLeft(toElement) + toElement.ActualWidth / 2,
                        Y2 = Canvas.GetTop(toElement) + toElement.ActualHeight / 2,
                        Tag = new Tuple<Block, Block>(fromBlock, toBlock)
                    };
                    WorkspaceCanvas.Children.Add(line);
                }
            }
        }
        public List<Block> getBlocks()
        {
            return blocks.Select(block =>
            {
                var element = FindVisualChild<Border>(WorkspaceCanvas, block);

                block.Position = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));

                return block;
            }).ToList();
        }

        public string getLanguageCode()
        {
            var languageComboBox = window.FindName("LanguageComboBox") as ComboBox;
            return languageComboBox.SelectedValue.ToString();
        }
    }
}
