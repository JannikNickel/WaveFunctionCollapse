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

namespace WFCUI
{
    /// <summary>
    /// Interaction logic for TilePreview.xaml
    /// </summary>
    public partial class TilePreview : Window
    {
        public TilePreview()
        {
            InitializeComponent();
        }

        public void Init(BitmapImage image, float rotation, string[] connector)
        {
            previewImage.Source = image;
            previewImageRotation.Angle = rotation;
            topConnector.Text = connector[0];
            rightConnector.Text = connector[1];
            bottomConnector.Text = connector[2];
            leftConnector.Text = connector[3];
        }
    }
}
