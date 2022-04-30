using System.Windows.Controls;
using System.Windows.Media;

namespace WFCUI
{
    public class ImageEx : Image
    {
        protected override void OnRender(DrawingContext dc)
        {
            this.VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor;
            base.OnRender(dc);
        }
    }
}
