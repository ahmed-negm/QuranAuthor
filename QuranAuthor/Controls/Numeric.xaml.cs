using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuranAuthor.Controls
{
    public class NumericEventArgs : EventArgs
    {
        public int Value { get; set; }

        public NumericEventArgs(int value)
        {
            this.Value = value;
        }
    }

    public partial class Numeric : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(int), typeof(Numeric));

        public event EventHandler ValueChanged;

        public int Value
        {
            get { return (int)base.GetValue(ValueProperty); }
            set
            {
                txtPoint.Text = value.ToString();
                base.SetValue(ValueProperty, value);
            }
        }

        public Numeric()
        {
            InitializeComponent();
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            txtPoint.Text = (int.Parse(txtPoint.Text) + 5).ToString();
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            txtPoint.Text = (int.Parse(txtPoint.Text) - 5).ToString();
        }

        private void txtPoint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                cmdUp_Click(null, null);
            }
            else if (e.Key == Key.Down)
            {
                cmdDown_Click(null, null);
            }
        }

        private void txtPoint_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void txtPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Value = int.Parse(txtPoint.Text);
            if (ValueChanged != null)
            {
                ValueChanged(this, new NumericEventArgs(this.Value));
            }
        }
    }
}