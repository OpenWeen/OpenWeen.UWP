using OpenWeen.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.Common
{
    internal static class StaticResource
    {
        public static List<EmotionModel> Emotions { get; set; }
        public static string EmotionPattern { get; set; }
        public static long Uid { get; set; }
        //public static event EventHandler UpdateUnreadCountTaskComplete;

    }
}
