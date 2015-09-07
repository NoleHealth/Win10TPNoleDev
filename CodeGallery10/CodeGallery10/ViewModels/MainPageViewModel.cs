using MOS.CodeGallery10.Data.DataSources;
using MOS.CodeGallery10.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace MOS.CodeGallery10.ViewModels
{
    public class MainPageViewModel : Mvvm.ViewModelBase
    {

        bool _busy = false;
        public bool Busy { get { return _busy; } set { Set(ref _busy, value); } }

        //private GroupInfoList _groups;
        //public GroupInfoList Groups
        //{
        //    get { return this._groups; }
        //}

        //private ObservableCollection<GroupInfoList> _groups = new ObservableCollection<GroupInfoList>();
        //public ObservableCollection<GroupInfoList> Groups
        //{
        //    get { return this._groups; }
        //}
        private IEnumerable<GalleryItemGroup> _groups;
        public IEnumerable<GalleryItemGroup> Groups
        {
            get { return this._groups; }
        }

        public MainPageViewModel()
        {
            // designtime data
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
            
        }

        


        private GalleryItem _selectedGalleryItem;
        public GalleryItem SelectedGalleryItem
        {
            get
            {
                return _selectedGalleryItem;
            }
            set
            {
                if(Set(ref _selectedGalleryItem, value))
                    RaisePropertyChanged("SelectedGalleryItemObject");
                if (this.NavigateOnSelectedItemChanged == true)
                    navigateToSelectedItem();
            }
        }

        private void navigateToSelectedItem()
        {

            
            
            if (this.SelectedGalleryItem == null || string.IsNullOrWhiteSpace(this.SelectedGalleryItem.TargetUri))
                return;

            string typeName = this.SelectedGalleryItem.TargetUri.Trim();
            Type navType = Type.GetType(typeName);
            if (navType == null)
                return;

            string parms = this.SelectedGalleryItem.TargetParamaters == null ? "" : "NavHost," + this.SelectedGalleryItem.TargetParamaters.Trim();
            
            this.NavigationService.Navigate(navType, parms);
        }

        public object SelectedGalleryItemObject
        {
            get
            {
                return this.SelectedGalleryItem as object;
            }
            set
            {
                this.SelectedGalleryItem = value as GalleryItem;
                
            }
        }
        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LoadCommand.Execute(null);
            base.OnNavigatedTo(parameter, mode, state);

            

        }

        DelegateCommand _loadCommand = default(DelegateCommand);
        public DelegateCommand LoadCommand { get { return _loadCommand ?? (_loadCommand = new DelegateCommand(ExecuteLoadCommand, CanExecuteLoadCommand)); } }
        private bool CanExecuteLoadCommand() { return !Busy; }
        private async void ExecuteLoadCommand()
        {
            try
            {
                Busy = true;
                await Task.Delay(2000);
                _groups = await GalleryDataSource.GetGroupsAsync();

                //var data = _todoListRepository.Sample(10).Select(x => new ViewModels.TodoListViewModel(x));
                //this.TodoLists.Clear();
                //foreach (var item in data.OrderBy(x => x.TodoList.Title))
                //{
                //    this.TodoLists.Add(item);
                //}
            }
            finally { Busy = false; }
        }

        //public override async void OnNavigatingFrom(NavigatingEventArgs args)
        //{
        //    await ExecuteSaveCommand();
        //}


        //public async override void OnNavigatedTo(string parameter, NavigationMode mode, IDictionary<string, object> state)
        //{
        //    //if (state.Any())
        //    //{
        //    //    // use cache value(s)
        //    //    if (state.ContainsKey(nameof(Value)))
        //    //        Value = state[nameof(Value)]?.ToString();
        //    //    // clear any cache
        //    //    state.Clear();
        //    //}
        //    //else
        //    //{
        //    //    // parameters are not applicable 
        //    //    // use navigation parameter
        //    ////    Value = string.Format("You passed '{0}'", parameter?.ToString());
        //    //}

        //    //_groups = /*await*/ GalleryItem.GetContactsGrouped(250);
        //    _groups = await GalleryDataSource.GetGroupsAsync();
        //    SelectedGalleryItem = null;
        //}

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
            base.OnNavigatingFrom(args);
        }

        private string _Value = string.Empty;
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }


        private bool _navigateOnSelectedItemChanged = true;
        public bool NavigateOnSelectedItemChanged { get { return _navigateOnSelectedItemChanged; } set { Set(ref _navigateOnSelectedItemChanged, value); } }


        

        //public void GotoPage2()
        //{
        //    this.NavigationService.Navigate(typeof(Views.SystemPage), this.Value);
        //}


        public void GotoPage(object sender, RoutedEventArgs e)
        {
            //if ((e.OriginalSource as Windows.UI.Xaml.Controls.Button).Name == "SubmitButton2")
            //    this.NavigationService.Navigate(typeof(Views.DevicesPage), this.Value);
            //else
            //    this.NavigationService.Navigate(typeof(Views.SystemPage), this.Value);
        }
        private bool _canDragItems = false;
        public bool CanDragItems { get { return _canDragItems; } set { Set(ref _canDragItems, value); } }
        //IsItemClickEnabled
        private bool _isItemClickEnabled = true;
        public bool IsItemClickEnabled { get { return _isItemClickEnabled; } set { Set(ref _isItemClickEnabled, value); } }
        //IsSwipeEnabled
        private bool _isSwipeEnabled = false;
        public bool IsSwipeEnabled { get { return _isSwipeEnabled; } set { Set(ref _isSwipeEnabled, value); } }

        //private IEnumerable<ControlInfoDataGroup> _groups;
        //public IEnumerable<ControlInfoDataGroup> Groups
        //{
        //    get { return this._groups; }
        //}

        

       
    }
}
