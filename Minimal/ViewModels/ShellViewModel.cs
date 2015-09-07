using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal.ViewModels
{
    public class ShellViewModel : Minimal.Mvvm.ViewModelBase
    {
        private bool _showShellBackButton = false;
        public bool ShowShellBackButton { get { return _showShellBackButton; } set { Set(ref _showShellBackButton, value); } }

        private bool _showSplashScreen = true;
        public bool ShowSplashScreen { get { return _showSplashScreen; } set { Set(ref _showSplashScreen, value); } }

        private int _cacheMaxDurationDays = 2;
        public int CacheMaxDurationDays { get { return _cacheMaxDurationDays; } set { Set(ref _cacheMaxDurationDays, value); } }
    }
}
