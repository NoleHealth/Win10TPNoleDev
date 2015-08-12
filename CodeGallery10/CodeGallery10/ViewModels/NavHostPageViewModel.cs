using MOS.CodeGallery10.Core;
using MOS.CodeGallery10.Data.DataSources;
using MOS.CodeGallery10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MOS.CodeGallery10.ViewModels
{
    
    public class NavHostPageViewModel : Mvvm.ViewModelBase
    {

        private NavHostPageModel _model;

        private IEnumerable<NavigationGroup> _navGroups;
        public IEnumerable<NavigationGroup> NavGroups
        {
            get { return this._navGroups; }
        }

        public NavHostPageViewModel()
        {
            // designtime data
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.Value = "Designtime value";
            }
        }

        public async override void OnNavigatedTo(string parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.Any())
            //{
            //    // use cache value(s)
            //    if (state.ContainsKey(nameof(Value)))
            //        Value = state[nameof(Value)]?.ToString();
            //    // clear any cache
                state.Clear();
            //}
            //else
            //{
            //    // use navigation parameter
            //    Value = string.Format("You passed '{0}'", parameter?.ToString());
            //}

            var ps = parameter?.Split(new char[] { ',' });

            if(ps.Length >=2)
            {
                if (ps[0] == "NavHost")
                {
                    Value = ps[1];
                    //_model = await NavigationDataSource.GetNavigationGroupsAsync(ps[1]);
                    _navGroups = await NavigationDataSource.GetNavigationGroupsAsync(ps[1]);
                    
                    return;
                }
            }
            Value = "Unknown";


        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                // persist into cache
                state[nameof(Value)] = Value;
            }
            return base.OnNavigatedFromAsync(state, suspending);
        }

        public override void OnNavigatingFrom(Template10.Services.NavigationService.NavigatingEventArgs args)
        {
            args.Cancel = false;
        }

        private string _Value = "Loading...";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }
    }
}
