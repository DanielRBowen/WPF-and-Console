using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;

namespace The_Arithmetician
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static SoundPlayer LoadAudio(string relativeUrl)
        {
            var resourceStreamInfo = App.GetResourceStream(new Uri(relativeUrl, UriKind.Relative));
            return new SoundPlayer(resourceStreamInfo.Stream);
        }
    }
}