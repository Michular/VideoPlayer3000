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
using System.Windows.Shapes;

namespace VideoPlayer3000
{
    /// <summary>
    /// Interakční logika pro Notebook.xaml
    /// </summary>
    public partial class Notebook : Window
    {
        public string Poznamka {  get; private set; }

        public Notebook()
        {
            InitializeComponent();

            noteTextBox.Focus();
        }

        /// <summary>
        /// Zrusi vytvoreni poznamky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StornoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Dokonci zadavani poznamky a ulozi text do promenne Poznamka
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Poznamka = noteTextBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
