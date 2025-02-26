using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LabBackend.Blocks.Actions;
using LabBackend.Blocks.Conditions;
using LabBackend.Utils;
using LabBackend.Utils.Abstract;
using LabBackend.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WpfApp2.frontend.blocks;
using WpfApp2.frontend.utils;

namespace BlockLinkingApp
{
    public partial class MainWindow : Window
    {
        UIManager uiManager;
        private static int _nextBlockId = 1;
        public MainWindow()
        {
            InitializeComponent();
            uiManager = new UIManager(WorkspaceCanvas);

            InitWorkspace();
            InitComboBox();
            this.KeyDown += MainWindow_DelDown;
            this.KeyDown += MainWindow_InsDown;
            UpdateBlockCounter();
        }

        private void InitWorkspace()
        {
            var startBlock = new Block
            {
                Id = _nextBlockId++,
                Type = "start",
                Text = "Start",
                NextBlockId = null,
                Position = new Point(50, 100)
            };
            var endBlock = new Block
            {
                Id = _nextBlockId++,
                Type = "end",
                Text = "End",
                NextBlockId = null,
                Position = new Point(50, 500)
            };

            uiManager.blocks.Add(startBlock);
            uiManager.blocks.Add(endBlock);
            AddBlockToCanvas(startBlock);
            AddBlockToCanvas(endBlock);
        }

        private void InitComboBox()
        {
            LanguageComboBox.Items.Add("C");
            LanguageComboBox.Items.Add("C++");
            LanguageComboBox.Items.Add("C#");
            LanguageComboBox.Items.Add("Java");
            LanguageComboBox.Items.Add("Python");
            LanguageComboBox.SelectedIndex = 0;
        }

        private void AddBlockButton_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            string blockType = button?.Tag?.ToString();

            var newBlock = new Block
            {
                Id = _nextBlockId++,
                Type = blockType,
                Text = blockType,
                NextBlockId = null,
                Position = new Point(10, 10)
            };

            if (blockType == "if")
            {
                newBlock.Text = "If Block";
                newBlock.TrueBlockId = null;
                newBlock.FalseBlockId = null;
            }

            uiManager.blocks.Add(newBlock);
            AddBlockToCanvas(newBlock);
            UpdateBlockCounter();
            if (uiManager.blocks.Count - 2 == 101)
            {
                var llimit_message = new Llimit_message();
                llimit_message.ShowDialog();
                Application.Current.Shutdown();
            }


        }

        private void AddBlockToCanvas(Block block)
        {
            var border = new Border
            {
                Width = 150,
                Height = 70,
                Background = GetBlockBackground(block.Type),
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Black,
                Tag = block
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var typeTextBlock = new TextBlock
            {
                Text = block.Type,
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = Brushes.Red
            };
            Grid.SetRow(typeTextBlock, 0);

            var textBlock = new TextBlock
            {
                Text = block.Text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(textBlock, 1);

            grid.Children.Add(typeTextBlock);
            grid.Children.Add(textBlock);
            border.Child = grid;

            Canvas.SetLeft(border, block.Position.X);
            Canvas.SetTop(border, block.Position.Y);

            border.MouseMove += Border_MouseMove;
            border.MouseDown += Border_MouseDown;
            border.PreviewMouseLeftButtonDown += Border_Left_2_clck;
            uiManager.WorkspaceCanvas.Children.Add(border);
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Border border)
            {
                DragDrop.DoDragDrop(border, border, DragDropEffects.Move);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Block block)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (block.Type == "if")
                    {
                        HandleIfBlockRightClick(block);
                    }
                    else
                    {
                        HandleOtherBlockRightClick(block);
                    }
                }
                else if (e.LeftButton == MouseButtonState.Pressed && uiManager.sourceBlock != null)
                {
                    HandleLeftClickOnBlock(block);
                }
            }
        }

        private void HandleIfBlockRightClick(Block block)
        {

            var connectionChoiceWindow = new ConnectionChoiceWindow();
            bool? result = connectionChoiceWindow.ShowDialog();

            if (result == true)
            {

                if (result == true)
                {
                    uiManager.sourceBlock = block;
                    uiManager.IsTSelect = connectionChoiceWindow.IsTrueSelected;
                    MessageBox.Show($"{(uiManager.IsTSelect.Value ? "true" : "false")}  {block.Text}");
                }
            }
        }



        private void HandleOtherBlockRightClick(Block block)

        {

            if (block.Type != "if")

            {
                var connectionChoiceWindow = new BlockConnectionChoiceWindow();

                bool? result = connectionChoiceWindow.ShowDialog();
                if (result == true)

                {

                    uiManager.sourceBlock = block;

                    uiManager.IsNextSelect = connectionChoiceWindow.IsNextSelected;
                }
            }
            else

            {
                uiManager.sourceBlock = block;
            }
        }


        private void HandleLeftClickOnBlock(Block block)
        {
            if (uiManager.sourceBlock != null && uiManager.sourceBlock != block)
            {
                if (uiManager.sourceBlock.Type == "if")
                {
                    if (uiManager.IsTSelect.HasValue)
                    {
                        if (uiManager.IsTSelect.Value && uiManager.sourceBlock.TrueBlockId == null)
                        {
                            uiManager.sourceBlock.TrueBlockId = block.Id;
                            uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Green);
                            MessageBox.Show($"connection true: {uiManager.sourceBlock.Text} -> {block.Text}");
                        }
                        else if (!uiManager.IsTSelect.Value && uiManager.sourceBlock.FalseBlockId == null)
                        {
                            uiManager.sourceBlock.FalseBlockId = block.Id;
                            uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Red);
                            MessageBox.Show($"connection false: {uiManager.sourceBlock.Text} -> {block.Text}");
                        }
                        uiManager.IsTSelect = null;
                    }
                }
                else if (uiManager.sourceBlock.Type == "else")
                {
                    if (uiManager.sourceBlock.NextBlockId == null)
                    {
                        uiManager.sourceBlock.NextBlockId = block.Id;
                        uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Orange); //f else 
                        MessageBox.Show($"connection next: {uiManager.sourceBlock.Text} -> {block.Text}");
                    }
                }
                else //def blcks
                {
                    if (uiManager.IsNextSelect.HasValue)
                    {
                        if (uiManager.IsNextSelect.Value && uiManager.sourceBlock.NextBlockId == null)
                        {
                            uiManager.sourceBlock.NextBlockId = block.Id;
                            uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Black);
                            MessageBox.Show($"connection next: {uiManager.sourceBlock.Text} -> {block.Text}");
                        }
                        else if (!uiManager.IsNextSelect.Value && uiManager.sourceBlock.ExitElseBlockId == null)
                        {
                            uiManager.sourceBlock.ExitElseBlockId = block.Id;
                            uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Purple);
                            MessageBox.Show($"connection exit_else: {uiManager.sourceBlock.Text} -> {block.Text}");
                        }
                        uiManager.IsNextSelect = null;
                    }
                }

                uiManager.sourceBlock = null;
            }
        }

        private void WorkspaceCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Border)) is Border droppedBorder)
            {
                Point position = e.GetPosition(uiManager.WorkspaceCanvas);
                Canvas.SetLeft(droppedBorder, position.X - droppedBorder.ActualWidth / 2);
                Canvas.SetTop(droppedBorder, position.Y - droppedBorder.ActualHeight / 2);

                if (droppedBorder.Tag is Block block)
                {
                    block.Position = new Point(Canvas.GetLeft(droppedBorder), Canvas.GetTop(droppedBorder));
                    UpdateArrowsForBlock(block);
                }
            }
        }

        private void SaveAllButton_Click(object sender, RoutedEventArgs e)
        {
            var saveData = uiManager.getBlocks();
            var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, json);
                MessageBox.Show("дані діаграми збережені");
            }
        }

        private void LoadAllButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (dialog.ShowDialog() == true)
            {
                uiManager.WorkspaceCanvas.Children.Clear();
                uiManager.blocks.Clear();

                var json = File.ReadAllText(dialog.FileName);
                var savedBlocks = JsonConvert.DeserializeObject<List<Block>>(json);

                foreach (var savedBlock in savedBlocks)
                {
                    var block = new Block
                    {
                        Id = savedBlock.Id,
                        Type = savedBlock.Type,
                        Text = savedBlock.Text,
                        NextBlockId = savedBlock.NextBlockId,
                        TrueBlockId = savedBlock.TrueBlockId,
                        FalseBlockId = savedBlock.FalseBlockId,
                        ExitElseBlockId = savedBlock.ExitElseBlockId,
                        Position = new Point((double)savedBlock.Position.X, (double)savedBlock.Position.Y)
                    };

                    uiManager.blocks.Add(block);
                    AddBlockToCanvas(block);

                    if (block.Id >= _nextBlockId)
                    {
                        _nextBlockId = block.Id + 1;
                    }
                }

                foreach (var block in uiManager.blocks)
                {
                    if (block.NextBlockId.HasValue)
                    {
                        var toBlock = uiManager.blocks.Find(b => b.Id == block.NextBlockId.Value);
                        if (toBlock != null)
                        {

                            uiManager.DrawArrow(block, toBlock, block.Type == "else" ? Brushes.Orange : Brushes.Black);
                        }
                    }

                    if (block.TrueBlockId.HasValue)
                    {
                        var trueBlock = uiManager.blocks.Find(b => b.Id == block.TrueBlockId.Value);
                        if (trueBlock != null)
                        {
                            uiManager.DrawArrow(block, trueBlock, Brushes.Green);
                        }
                    }

                    if (block.FalseBlockId.HasValue)
                    {
                        var falseBlock = uiManager.blocks.Find(b => b.Id == block.FalseBlockId.Value);
                        if (falseBlock != null)
                        {
                            uiManager.DrawArrow(block, falseBlock, Brushes.Red);
                        }
                    }

                    if (block.ExitElseBlockId.HasValue)
                    {
                        var exitElseBlock = uiManager.blocks.Find(b => b.Id == block.ExitElseBlockId.Value);
                        if (exitElseBlock != null)
                        {
                            uiManager.DrawArrow(block, exitElseBlock, Brushes.Purple);
                        }
                    }
                }

                UpdateBlockCounter();
                MessageBox.Show("data success loaded");
                if (uiManager.blocks.Count - 2 >= 101)
                {
                    var llimit_message = new Llimit_message();
                    llimit_message.ShowDialog();
                    Application.Current.Shutdown();
                }
            }
        }


        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            IManager blockManager = new BlockManager();

            List<Block> blocksRAWFrontend = uiManager.getCloneBlocks();
            List<Block> linkedFrontendBlocks = blockManager.GetLinkedFrontendBlocks(blocksRAWFrontend);
            Dictionary<int, Dictionary<int, bool>> adjacencyMatrix = blockManager.CreateAdjacencyMatrix(linkedFrontendBlocks);
            try
            {
                blockManager.TranslateCode(
                    uiManager.getLanguageCode(),
                    linkedFrontendBlocks,
                    adjacencyMatrix);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show($"[{uiManager.getLanguageCode()}] Code translated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }









        private void Border_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Block block)
            {
                var inputDialog = new InputTextWindow(block.Text);
                bool? result = inputDialog.ShowDialog();
                if (result == true)
                {
                    block.Text = inputDialog.EnteredText;
                    UpdateBlockText(border, block.Text);
                }
            }
        }
        private void UpdateBlockText(Border border, string newText)
        {
            if (border.Child is Grid grid)
            {
                var textBlock = grid.Children.OfType<TextBlock>().FirstOrDefault(tb => Grid.GetRow(tb) == 1);
                if (textBlock != null)
                {
                    textBlock.Text = newText;
                }
            }
        }
        private void Border_Left_2_clck(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is Border border && border.Tag is Block block)
                {
                    var inputDialog = new InputTextWindow(block.Text);
                    bool? result = inputDialog.ShowDialog();
                    if (result == true)
                    {
                        block.Text = inputDialog.EnteredText;
                        UpdateBlockText(border, block.Text);
                    }
                }
            }
        }








        private Block GetBlockUnderCursor()
        {
            var mousePosition = Mouse.GetPosition(WorkspaceCanvas);
            foreach (var child in WorkspaceCanvas.Children)
            {
                if (child is Border border && border.Tag is Block block)
                {
                    var blockPosition = new Point(Canvas.GetLeft(border), Canvas.GetTop(border));
                    var blockRect = new Rect(blockPosition, new Size(border.ActualWidth, border.ActualHeight));

                    if (blockRect.Contains(mousePosition))
                    {
                        return block;
                    }
                }
            }
            return null;
        }





        public void RemovearrForBlock(Block block)
        {

            var arrowsToRemove = WorkspaceCanvas.Children
                .OfType<Line>()
                .Where(line => line.Tag is Tuple<Block, Block> tuple &&
                               tuple.Item1 == block)
                .ToList();

            foreach (var arrow in arrowsToRemove)
            {
                WorkspaceCanvas.Children.Remove(arrow);
            }
        }


        private void Del_block_connection(Block block)
        {
            if (block == null) return;


            RemovearrForBlock(block);
            block.NextBlockId = null;
            block.TrueBlockId = null;
            block.FalseBlockId = null;
            block.ExitElseBlockId = null;
        }

        private void MainWindow_InsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
            {
                var blockUnderCursor = GetBlockUnderCursor();
                if (blockUnderCursor != null)
                {
                    Del_block_connection(blockUnderCursor);
                }
            }
        }



        private void UpdateBlockCounter()
        {
            BlockCounter.Text = $"Blocks: {uiManager.blocks.Count - 2}";
        }



        private void UpdateArrowsForBlock(Block block)
        {
            foreach (var b in uiManager.blocks)
            {
                if (b.NextBlockId == block.Id)
                {
                    uiManager.DrawArrow(b, block, b.Type == "else" ? Brushes.Orange : Brushes.Black);
                }
                if (b.Type == "if")
                {
                    if (b.TrueBlockId == block.Id)
                    {
                        uiManager.DrawArrow(b, block, Brushes.Green);
                    }
                    if (b.FalseBlockId == block.Id)
                    {
                        uiManager.DrawArrow(b, block, Brushes.Red);
                    }
                }
                if (b.ExitElseBlockId == block.Id)
                {
                    uiManager.DrawArrow(b, block, Brushes.Purple);
                }
                if (b.Id == block.NextBlockId ||
                    (block.Type == "if" && (block.TrueBlockId == b.Id || block.FalseBlockId == b.Id)) ||
                    block.ExitElseBlockId == b.Id)
                {
                    uiManager.DrawArrow(block, b,
                        block.Type == "if" ? (b.Id == block.TrueBlockId ? Brushes.Green : Brushes.Red) :
                        block.Type == "else" ? Brushes.Orange :
                        block.ExitElseBlockId == b.Id ? Brushes.Purple : Brushes.Black);
                }
            }
        }



        private void MainWindow_DelDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {

                var blockUnderCursor = GetBlockUnderCursor();
                if (blockUnderCursor != null)
                {
                    DeleteBlock(blockUnderCursor);
                }
            }
        }

        private void DeleteBlock(Block blockToDelete)
        {
            if (blockToDelete.Type == "start" || blockToDelete.Type == "end")
            {
                MessageBox.Show("u cant delete base blocks");
                return;
            }
            RemoveArrowsForBlock(blockToDelete);
            var borderToRemove = uiManager.WorkspaceCanvas.Children
                .OfType<Border>()
                .FirstOrDefault(b => b.Tag == blockToDelete);
            if (borderToRemove != null)
            {
                uiManager.WorkspaceCanvas.Children.Remove(borderToRemove);
            }

            uiManager.blocks.Remove(blockToDelete);
            foreach (var block in uiManager.blocks)
            {
                if (block.NextBlockId == blockToDelete.Id)
                {
                    block.NextBlockId = null;
                }
                if (block.Type == "if")
                {
                    if (block.TrueBlockId == blockToDelete.Id)
                    {
                        block.TrueBlockId = null;
                    }
                    if (block.FalseBlockId == blockToDelete.Id)
                    {
                        block.FalseBlockId = null;
                    }
                }
                if (block.ExitElseBlockId == blockToDelete.Id)
                {
                    block.ExitElseBlockId = null;
                }
            }
            UpdateBlockCounter();
        }

        private void RemoveArrowsForBlock(Block block)
        {
            var arrowsToRemove = uiManager.WorkspaceCanvas.Children
                .OfType<Line>()
                .Where(line => line.Tag is Tuple<Block, Block> tuple &&
                               (tuple.Item1 == block || tuple.Item2 == block))
                .ToList();

            foreach (var arrow in arrowsToRemove)
            {
                uiManager.WorkspaceCanvas.Children.Remove(arrow);
            }
        }







        private Brush GetBlockBackground(string type)
        {
            return type switch
            {
                "AssignmentBlock" => Brushes.LightBlue,
                "ConstantAssignmentBlock" => Brushes.LightGreen,
                "InputBlock" => Brushes.LightYellow,
                "PrintBlock" => Brushes.LightPink,
                "start" => Brushes.LightSkyBlue,
                "end" => Brushes.LightGreen,
                "if" => Brushes.LightCoral,
                _ => Brushes.LightGray
            };
        }
    }
}
