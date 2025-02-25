using System.Windows;

namespace BlockLinkingApp
{
    public partial class BlockConnectionChoiceWindow : Window
    {
        public bool IsNextSelected { get; private set; }

        public BlockConnectionChoiceWindow()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            IsNextSelected = true;
            this.DialogResult = true;
            this.Close();
        }

        private void ExitElseButton_Click(object sender, RoutedEventArgs e)
        {
            IsNextSelected = false;
            this.DialogResult = true;
            this.Close();
        }
    }
}