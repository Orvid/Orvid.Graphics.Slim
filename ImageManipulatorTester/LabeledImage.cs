using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageManipulatorTester
{
    public partial class LabeledImage : UserControl
    {
        public LabeledImage()
        {
            InitializeComponent();
        }

        public Image Image
        {
            get
            {
                return label1.Image;
            }
            set
            {
                label1.Image = value;
            }
        }

        new public String Text
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }
    }
}
