using System.Collections.Generic;
using OpenWeen.Core.Model;

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