using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.Common.Controls.Events
{
    public class WeiboPictureClickEventArgs : EventArgs
    {
        public WeiboPictureClickEventArgs(PictureModel clickedPicture)
        {
            ClickedPicture = clickedPicture;
        }
        public PictureModel ClickedPicture { get; set; }
        public MessageModel DataContext { get; set; }
    }
}
