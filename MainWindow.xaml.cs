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

            var startBlock = new Block { Id = 1, Type = "start", Text = "Start", NextBlockId = null, Position = new Point(50, 100) };
            var endBlock = new Block { Id = 2, Type = "end", Text = "End", NextBlockId = null, Position = new Point(50, 500) };

            uiManager.blocks.Add(startBlock);
            uiManager.blocks.Add(endBlock);

            StartBlock.Tag = startBlock;
            EndBlock.Tag = endBlock;

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
            LanguageComboBox.SelectedIndex = 3;
        }

        private void AddBlockButton_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            string blockType = button?.Tag?.ToString();

            var newBlock = new Block
            {
                Id = uiManager.blocks.Count + 1,
                Type = blockType,
                Text = $"{blockType} Block {uiManager.blocks.Count + 1}",
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
            if (uiManager.blocks.Count-2 == 101)
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
                Width = 100,
                Height = 50,
                Background = GetBlockBackground(block.Type),
                Tag = block
            };

            var textBlock = new TextBlock
            {
                Text = block.Text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            border.Child = textBlock;

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
               
                if (connectionChoiceWindow.IsTrueSelected)
                {
                    uiManager.sourceBlock = block;
                    MessageBox.Show($"true link {block.Text}");
                }
               
                else
                {
                    uiManager.sourceBlock = block;
                    MessageBox.Show($"flase link {block.Text}");
                }
            }
        }

        
        private void HandleOtherBlockRightClick(Block block)
        {
            uiManager.sourceBlock = block;
            MessageBox.Show($"block chosen: {block.Text}");
        }

      
        private void HandleLeftClickOnBlock(Block block)
        {
            if (uiManager.sourceBlock != null && uiManager.sourceBlock != block)
            {
                if (uiManager.sourceBlock.Type == "if")
                {



                     if (uiManager.sourceBlock.FalseBlockId == null )
                    {
                        uiManager.sourceBlock.FalseBlockId = block.Id;
                        uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Red);
                        MessageBox.Show($"connection False: {uiManager.sourceBlock.Text} -> {block.Text}");
                    }


                    else if (uiManager.sourceBlock.TrueBlockId == null)
                    {
                        uiManager.sourceBlock.TrueBlockId = block.Id;
                        uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Green); 
                        MessageBox.Show($"connection True: {uiManager.sourceBlock.Text} -> {block.Text}");
                    }






                }
                else
                {
                    uiManager.sourceBlock.NextBlockId = block.Id;
                    uiManager.DrawArrow(uiManager.sourceBlock, block, Brushes.Black);
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
                        Position = new Point((double)savedBlock.Position.X, (double)savedBlock.Position.Y)
                    };

                    uiManager.blocks.Add(block);
                    AddBlockToCanvas(block);
                }

                foreach (var block in uiManager.blocks)
                {
                    if (block.NextBlockId.HasValue)
                    {
                        var toBlock = uiManager.blocks.Find(b => b.Id == block.NextBlockId.Value);
                        if (toBlock != null)
                        {
                            uiManager.DrawArrow(block, toBlock, Brushes.Black); 
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

            List<Block> blocksRAWFrontend = uiManager.getBlocks();
            List<Block> linkedFrontendBlocks = blockManager.GetLinkedFrontendBlocks(blocksRAWFrontend);
            Dictionary<int, Dictionary<int, bool>> adjacencyMatrix = blockManager.CreateAdjacencyMatrix(linkedFrontendBlocks);
            blockManager.TranslateCode(uiManager.getLanguageCode(), linkedFrontendBlocks, adjacencyMatrix);
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
            if (border.Child is TextBlock textBlock)
            {
                textBlock.Text = newText;
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
            BlockCounter.Text = $"Blocks: {uiManager.blocks.Count-2}";
        }



        private void UpdateArrowsForBlock(Block block)
        {
            foreach (var b in uiManager.blocks)
            {
                if (b.NextBlockId == block.Id)
                {
                    uiManager.DrawArrow(b, block, Brushes.Black);
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
                if (b.Id == block.NextBlockId ||
                    (block.Type == "if" && (block.TrueBlockId == b.Id || block.FalseBlockId == b.Id)))
                {
                    uiManager.DrawArrow(block, b, block.Type == "if" ? (b.Id == block.TrueBlockId ? Brushes.Green : Brushes.Red) : Brushes.Black);
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
