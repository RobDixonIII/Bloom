﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Bloom.Browser.PubSubEvents;
using Bloom.PubSubEvents;
using Bloom.Services;
using Bloom.State.Domain.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;

namespace Bloom.Browser.MenuModule.ViewModels
{
    /// <summary>
    /// View model for MenuView.xaml.
    /// </summary>
    public class MenuViewModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuViewModel" /> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="skinningService">The skinning service.</param>
        /// <param name="processService">The process service.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public MenuViewModel(IRegionManager regionManager, ISkinningService skinningService, IProcessService processService, IEventAggregator eventAggregator)
        {
            _skinningService = skinningService;
            _processService = processService;
            _eventAggregator = eventAggregator;
            State = (BrowserState) regionManager.Regions["MenuRegion"].Context;
            CheckConnections(null);
            SetUser(null);
            SetLibraryContext(State.SelectedTabId);

            _eventAggregator.GetEvent<ConnectionAddedEvent>().Subscribe(CheckConnections);
            _eventAggregator.GetEvent<ConnectionRemovedEvent>().Subscribe(CheckConnections);
            _eventAggregator.GetEvent<UserChangedEvent>().Subscribe(SetUser);
            _eventAggregator.GetEvent<SidebarToggledEvent>().Subscribe(SetToggleSidebarVisibilityOption);
            _eventAggregator.GetEvent<SelectedTabChangedEvent>().Subscribe(SetLibraryContext);
            
            // File Menu
            CreateNewLibraryCommand = new DelegateCommand<object>(CreateNewLibrary, CanCreateNewLibrary);
            ManageConnectedLibrariesCommand = new DelegateCommand<object>(ManageConnectedLibraries, CanManageConnectedLibraries);
            ExitApplicationCommand = new DelegateCommand<object>(ExitApplication, CanExitApplication);
            // Edit Menu
            EditLibraryPropertiesCommand = new DelegateCommand<object>(EditLibraryProperties, CanEditLibraryProperties);
            // Browser Menu
            DuplicateTabCommand = new DelegateCommand<object>(DuplicateTab, CanDuplicateTab);
            CloseOtherTabsCommand = new DelegateCommand<object>(CloseOtherTabs, CanCloseOtherTabs);
            CloseAllTabsCommand = new DelegateCommand<object>(CloseAllTabs, CanCloseAllTabs);
            // Player Menu
            GoToPlayerCommand = new DelegateCommand<object>(GoToPlayer, CanGoToPlayer);
            // Analytics Menu
            GoToAnalyticsCommand = new DelegateCommand<object>(GoToAnalytics, CanGoToAnalytics);
            // View Menu
            OpenHomeTabCommand = new DelegateCommand<object>(OpenHomeTab, CanOpenHomeTab);
            SetToggleSidebarVisibilityOption(State.SidebarVisible);
            ToggleSidebarVisibilityCommand = new DelegateCommand<object>(ToggleSidebarVisibility, CanToggleSidebarVisibility);
            SetSkinCommand = new DelegateCommand<string>(SetSkin, CanSetSkin);
            // Help Menu
            OpenGettingStartedTabCommand = new DelegateCommand<object>(OpenGettingStartedTab, CanOpenGettingStartedTab);
        }
        private readonly ISkinningService _skinningService;
        private readonly IProcessService _processService;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Gets the state.
        /// </summary>
        public BrowserState State { get; private set; }

        public bool HasConnections
        {
            get { return _hasConnections; }
            set { SetProperty(ref _hasConnections, value); }
        }
        private bool _hasConnections;

        public void CheckConnections(object unused)
        {
            HasConnections = State != null && State.Connections != null && State.Connections.Count > 0;
            if (!HasConnections)
            {
                SetToggleSidebarVisibilityOption(false);
                _eventAggregator.GetEvent<HideSidebarEvent>().Publish(null);
            }
        }

        public void CheckConnections(Guid unused)
        {
            CheckConnections(null);
        }

        public bool HasUser
        {
            get { return _hasUser; }
            set { SetProperty(ref _hasUser, value); }
        }
        private bool _hasUser;

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }
        private string _userName;

        public void SetUser(object nothing)
        {
            if (State == null || State.User == null || State.User.Name == null)
            {
                UserName = "Login";
                HasUser = false;
            }
            else
            {
                UserName = State.User.Name;
                HasUser = true;
            }
        }

        public bool HasLibraryContext
        {
            get { return _hasLibraryContext; }
            set { SetProperty(ref _hasLibraryContext, value); }
        }
        private bool _hasLibraryContext;
        private Guid _libraryContext;

        public void SetLibraryContext(Guid tabId)
        {
            var selectedTab = State.Tabs.SingleOrDefault(tab => tab.Id == tabId);
            if (selectedTab == null || selectedTab.LibraryId == Guid.Empty)
            {
                _libraryContext = Guid.Empty;
                HasLibraryContext = false;
            }
            else
            {
                _libraryContext = selectedTab.LibraryId;
                HasLibraryContext = true;
            }
        }

        #region File Menu

        /// <summary>
        /// Gets or sets the create new library command.
        /// </summary>
        public ICommand CreateNewLibraryCommand { get; set; }

        private bool CanCreateNewLibrary(object nothing)
        {
            return true;
        }

        private void CreateNewLibrary(object nothing)
        {
            _eventAggregator.GetEvent<ShowCreateNewLibraryModalEvent>().Publish(null);
        }

        /// <summary>
        /// Gets or sets the manage connected libraries command.
        /// </summary>
        public ICommand ManageConnectedLibrariesCommand { get; set; }

        private bool CanManageConnectedLibraries(object nothing)
        {
            return true;
        }

        private void ManageConnectedLibraries(object nothing)
        {
            _eventAggregator.GetEvent<ShowConnectedLibrariesModalEvent>().Publish(null);
        }

        /// <summary>
        /// Gets or sets the exit application command.
        /// </summary>
        public ICommand ExitApplicationCommand { get; set; }

        private bool CanExitApplication(object nothing)
        {
            return true;
        }

        private void ExitApplication(object nothing)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Edit Menu

        public ICommand EditLibraryPropertiesCommand { get; set; }

        private bool CanEditLibraryProperties(object nothing)
        {
            return true;
        }

        private void EditLibraryProperties(object nothing)
        {
            _eventAggregator.GetEvent<ShowLibraryPropertiesModalEvent>().Publish(_libraryContext);
        }

        #endregion

        #region Browser Menu

        /// <summary>
        /// Gets or sets the duplicate tab command.
        /// </summary>
        public ICommand DuplicateTabCommand { get; set; }

        private bool CanDuplicateTab(object nothing)
        {
            return true;
        }

        private void DuplicateTab(object nothing)
        {
            _eventAggregator.GetEvent<DuplicateTabEvent>().Publish(State.SelectedTabId);
        }

        /// <summary>
        /// Gets or sets the close other tabs command.
        /// </summary>
        public ICommand CloseOtherTabsCommand { get; set; }

        private bool CanCloseOtherTabs(object nothing)
        {
            return true;
        }

        private void CloseOtherTabs(object nothing)
        {
            _eventAggregator.GetEvent<CloseOtherTabsEvent>().Publish(null);
        }

        /// <summary>
        /// Gets or sets the close all tabs command.
        /// </summary>
        public ICommand CloseAllTabsCommand { get; set; }

        private bool CanCloseAllTabs(object nothing)
        {
            return true;
        }

        private void CloseAllTabs(object nothing)
        {
            _eventAggregator.GetEvent<CloseAllTabsEvent>().Publish(null);
        }

        #endregion

        #region Player Menu

        /// <summary>
        /// Gets or sets the go to player command.
        /// </summary>
        public ICommand GoToPlayerCommand { get; set; }

        private bool CanGoToPlayer(object nothing)
        {
            return true;
        }

        private void GoToPlayer(object nothing)
        {
            _processService.GoToPlayerProcess();
        }

        #endregion

        #region Analytics Menu

        /// <summary>
        /// Gets or sets the go to analytics command.
        /// </summary>
        public ICommand GoToAnalyticsCommand { get; set; }

        private bool CanGoToAnalytics(object nothing)
        {
            return true;
        }

        private void GoToAnalytics(object nothing)
        {
            _processService.GoToAnalyticsProcess();
        }

        #endregion

        #region View Menu

        public ICommand OpenHomeTabCommand { get; set; }

        private bool CanOpenHomeTab(object nothing)
        {
            return true;
        }

        private void OpenHomeTab(object nothing)
        {
            _eventAggregator.GetEvent<NewHomeTabEvent>().Publish(null);
        }

        public string ToggleSidebarVisibilityOption
        {
            get { return _toggleSidebarVisibilityOption; }
            set { SetProperty(ref _toggleSidebarVisibilityOption, value); }
        }
        private string _toggleSidebarVisibilityOption;

        private void SetToggleSidebarVisibilityOption(bool isVisible)
        {
            if (isVisible)
                ToggleSidebarVisibilityOption = "Hide Sidebar";
            else
                ToggleSidebarVisibilityOption = "Show Sidebar";
        }

        public ICommand ToggleSidebarVisibilityCommand { get; set; }

        private bool CanToggleSidebarVisibility(object nothing)
        {
            return true;
        }

        private void ToggleSidebarVisibility(object nothing)
        {
            SetToggleSidebarVisibilityOption(!State.SidebarVisible);
            if (State.SidebarVisible)
                _eventAggregator.GetEvent<HideSidebarEvent>().Publish(null);
            else
                _eventAggregator.GetEvent<ShowSidebarEvent>().Publish(null);
        }

        /// <summary>
        /// Gets or sets the set skin command.
        /// </summary>
        public ICommand SetSkinCommand { get; set; }

        private bool CanSetSkin(string skinName)
        {
            return true;
        }

        private void SetSkin(string skinName)
        {
            if (State.SkinName == skinName)
                return;

            State.SkinName = skinName;
            _skinningService.SetSkin(skinName);
        }

        #endregion

        #region Help Menu

        public ICommand OpenGettingStartedTabCommand { get; set; }

        private bool CanOpenGettingStartedTab(object nothing)
        {
            return true;
        }

        private void OpenGettingStartedTab(object nothing)
        {
            _eventAggregator.GetEvent<NewGettingStartedTabEvent>().Publish(null);
        }

        #endregion
    }
}
