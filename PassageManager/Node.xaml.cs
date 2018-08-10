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

namespace OurGraph
{
    /// <summary>
    /// Interaction logic for Node.xaml
    /// </summary>
    public partial class Node : UserControl
    {
        private double _x;
        private double _y;
        private bool _isPressed;

        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
                //    OnPropertyChanged("Y");
            }
        }

        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                //      OnPropertyChanged("X");

            }
        }

        public bool IsPressed
        {
            get
            {
                return _isPressed;
            }

            set
            {
                _isPressed = value;
            }
        }

        public Node()
        {
            InitializeComponent();
        }
     
    }
}
