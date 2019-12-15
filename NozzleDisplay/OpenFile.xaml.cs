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

namespace NozzleDisplay
{
    /// <summary>
    /// Lógica de interacción para OpenFile.xaml
    /// </summary>
    public partial class OpenFile : Window
    {
        string fichero; //nombre fichero
        Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

        public OpenFile()
        {
            InitializeComponent();

            // escondemos el botónde guardar
            button_openfile.Visibility = Visibility.Hidden;
        }

        private void click_openfile(object sender, RoutedEventArgs e) // botón OPEN FILE
        {
            if (textbox_grande.Text == "" || textbox_peque.Text == "")
                MessageBox.Show("Check the selected file");
            else
                Close();
        }

        private void button_choosefile_Click(object sender, RoutedEventArgs e) // botón CHOOSE FILE
        {
            //miramos que sea un txt
            ofd.Filter = "TXT |*.txt";
            if (ofd.ShowDialog() == true)
            {
                //cogemos el nombre
                textbox_grande.Text = ofd.FileName;
                this.fichero = textbox_grande.Text; //el que queremos
                textbox_peque.Text = ofd.SafeFileName;

                //enseño boton guardar
                button_openfile.Visibility = Visibility.Visible;

                this.Close();
            }
        }

        public string getFichero() //da el nombre del fichero seleccionado
        {
            return this.fichero;
        }
    }
}
