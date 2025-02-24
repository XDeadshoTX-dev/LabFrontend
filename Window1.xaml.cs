using System.Windows;

namespace BlockLinkingApp
{
    public partial class InputTextWindow : Window
    {
        public string EnteredText { get; private set; }
        public InputTextWindow(string currentText)
        {
            InitializeComponent();
            TextInputBox.Text = currentText;
        }





        private void OkBtnClk(object sender, RoutedEventArgs e)
        {
            EnteredText = TextInputBox.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
