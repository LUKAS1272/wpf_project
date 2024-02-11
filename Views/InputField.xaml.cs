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

namespace InventorySystem.Views
{
    public partial class InputField : UserControl
    {
        public InputField()
        {
            InitializeComponent();
        }

        private string placeholder;
        public string Placeholder
        {
            get { return placeholder; }
            set {
                placeholder = value;
                inputPlaceholder.Text = placeholder;
            }
        }

        private string label;
        public string Label
        {
            get { return label; }
            set {
                label = value;
                inputLabel.Content = label;
            }
        }

        private string text;
        public string Text
        {
            get { return inputTxt.Text; }
            set {
                text = value;
                inputTxt.Text = text;
            }
        }




        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            inputTxt.Clear();
            inputTxt.Focus();
        }

        private void inputTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputTxt.Text == string.Empty) {
                inputPlaceholder.Visibility = Visibility.Visible;
            } else {
                inputPlaceholder.Visibility = Visibility.Hidden;
            }
        }
    }
}
