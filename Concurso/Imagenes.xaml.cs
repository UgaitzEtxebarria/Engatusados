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

namespace Concurso
{
    /// <summary>
    /// Lógica de interacción para Imagenes.xaml
    /// </summary>
    public partial class Imagenes : Window
    {
        public Imagenes()
        {
            InitializeComponent();
        }

        private bool okButton = false;

        public bool OKButtonClicked
        {
            get { return okButton; }
        }
        public int Puntos
        {
            get { return Convert.ToInt32(txtPuntos.Text); }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            okButton = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            okButton = false;
            this.Close();
        }

    }
}
