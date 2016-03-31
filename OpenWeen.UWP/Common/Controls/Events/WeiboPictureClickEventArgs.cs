using System;
using OpenWeen.Core.Model.Status;

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