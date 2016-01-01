﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Bloom.Browser.Common;
using Bloom.Browser.PubSubEvents;
using Bloom.Browser.State.Services;
using Bloom.Controls;
using Bloom.PubSubEvents;
using Bloom.Services;
using Bloom.State.Domain.Models;
using Microsoft.Practices.Prism.PubSubEvents;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;

namespace Bloom.Browser
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shell" /> class.
        /// </summary>
        /// <param name="skinningService">The skinning service.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="stateService">The state service.</param>
        public Shell(ISkinningService skinningService, IEventAggregator eventAggregator, IUserService userService, IBrowserStateService stateService)
        {
            InitializeComponent();
            _loading = true;
            _tabs = new Dictionary<Guid, RadPane>();
            _eventAggregator = eventAggregator;
            _stateService = stateService;
            _stateService.ConnectDataSource();
            var user = userService.InitializeUser();
            var state = _stateService.InitializeState(user);
            DataContext = state;

            // Don't open in a minimized state.
            if (state.WindowState == WindowState.Minimized)
                state.WindowState = WindowState.Normal;

            WindowState = state.WindowState;
            TitleBar.SetButtonVisibilties();
            SidebarPane.IsHidden = !state.SidebarVisible;
            skinningService.SetSkin(state.SkinName);

            eventAggregator.GetEvent<AddTabEvent>().Subscribe(AddTab);
            eventAggregator.GetEvent<CloseOtherTabsEvent>().Subscribe(CloseOtherTabs);
            eventAggregator.GetEvent<CloseAllTabsEvent>().Subscribe(CloseAllTabs);
            eventAggregator.GetEvent<HideSidebarEvent>().Subscribe(HideSidebar);
            eventAggregator.GetEvent<ShowSidebarEvent>().Subscribe(ShowSidebar);
            eventAggregator.GetEvent<ConnectionAddedEvent>().Subscribe(ShowSidebar);
            eventAggregator.GetEvent<ConnectionRemovedEvent>().Subscribe(CheckConnections);
        }
        private readonly Dictionary<Guid, RadPane> _tabs;
        private readonly IBrowserStateService _stateService;
        private readonly IEventAggregator _eventAggregator;
        private bool _loading;
        private BrowserState State { get { return (BrowserState) DataContext; } }

        #region Window Events

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.ContentRendered" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            _stateService.RestoreTabs();
            _loading = false;
            if (_tabs.ContainsKey(State.SelectedTabId))
                Dock.ActivePane = _tabs[State.SelectedTabId];
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Activated" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            // TODO: Check state database for new messages.
            State.WindowState = WindowState; // This is here only to avoid a ReSharper warning.
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            State.WindowState = WindowState;
            _stateService.SaveState();
        }

        #endregion

        #region Docking Events

        private void AddTab(TabControl tabControl)
        {
            var titleTemplate = (DataTemplate) FindResource("TitleTemplate");
            var newPane = new RadPane
            {
                Content = tabControl.Content,
                HeaderTemplate = titleTemplate,
                TitleTemplate = titleTemplate,
                Title = tabControl,
                Tag = tabControl.TabId
            };

            tabControl.Tab.UserId = State.UserId;
            _stateService.AddTab(tabControl.Tab);
            if (!_loading || State.SelectedTabId == Guid.Empty)
                State.SelectedTabId = tabControl.TabId;

            _tabs.Add(tabControl.TabId, newPane);
            PaneGroup.Items.Add(newPane);
        }

        private void CloseOtherTabs(object nothing)
        {
            _stateService.RemoveAllTabsExcept(State.SelectedTabId);

            var selectedTab = GetSelectedTab();
            foreach (var tab in _tabs.Values)
            {
                if (selectedTab != null && !Equals(selectedTab, tab)) { }
                    tab.IsHidden = true;
            }
        }

        private void CloseAllTabs(object nothing)
        {
            _stateService.RemoveAllTabs();

            foreach (var tab in _tabs.Values)
                tab.IsHidden = true;
        }

        private void ActivePaneChanged(object sender, ActivePangeChangedEventArgs e)
        {
            foreach (var valuePair in _tabs.Where(valuePair => Equals(valuePair.Value, e.NewPane)))
            {
                State.SelectedTabId = valuePair.Key;
                _eventAggregator.GetEvent<SelectedTabChangedEvent>().Publish(State.SelectedTabId);
                break;
            }
        }

        private void OnClose(object sender, StateChangeEventArgs e)
        {
            var closingTab = e.Panes.SingleOrDefault();

            foreach (var valuePair in _tabs)
            {
                var match = Equals(valuePair.Value, closingTab);
                if (match)
                    _stateService.RemoveTab(valuePair.Key);
            }
        }

        private void PaneStateChanged(object sender, RadRoutedEventArgs e)
        {
            if (_loading)
                return;

            var order = 1;
            foreach (RadPane pane in PaneGroup.Items)
            {
                UpdateTabOrder(pane, order);
                order++;
            }
            foreach (var pane in Dock.Panes.Where(pane => !PaneGroup.Items.Contains(pane)))
            {
                UpdateTabOrder(pane, order);
                order++;
            }
        }

        private void ViewMenuClick(object sender, RadRoutedEventArgs e)
        {
            var menuItem = (RadMenuItem) sender;
            var tabId = (Guid) menuItem.Tag;
            var viewType = (ViewType) Enum.Parse(typeof(ViewType), menuItem.Name);
            _eventAggregator.GetEvent<ChangeLibraryTabViewEvent>().Publish(new Tuple<Guid, ViewType>(tabId, viewType));
        }

        private void UpdateTabOrder(RadPane pane, int order)
        {
            var tabId = _tabs.SingleOrDefault(valuePair => Equals(valuePair.Value, pane)).Key;
            var tab = State.Tabs.SingleOrDefault(t => t.Id == tabId);
            if (tab == null)
                return;

            tab.Order = order;
        }

        private RadPane GetSelectedTab()
        {
            RadPane selectedTab = null;
            foreach (RadPane tab in PaneGroup.Items)
            {
                if (tab.IsSelected)
                    selectedTab = tab;
            }
            return selectedTab;
        }

        private void DockCompassPreview(object sender, PreviewShowCompassEventArgs e)
        {
            if (e.TargetGroup != null && e.TargetGroup.Name == "Sidebar")
            {
                e.Compass.IsLeftIndicatorVisible = false;
                e.Compass.IsTopIndicatorVisible = false;
                e.Compass.IsRightIndicatorVisible = false;
                e.Compass.IsBottomIndicatorVisible = false;
                e.Compass.IsCenterIndicatorVisible = false;
            }
            else
            {
                e.Compass.IsLeftIndicatorVisible = false;
                e.Compass.IsTopIndicatorVisible = false;
                e.Compass.IsRightIndicatorVisible = false;
                e.Compass.IsBottomIndicatorVisible = false;
                e.Compass.IsCenterIndicatorVisible = true;
            }
        }

        #endregion

        #region Sidebar Events

        private void OnSidebarSplitterDoubleClick(object sender, MouseButtonEventArgs e)
        {
            State.ResetSidebarWidth();
        }

        private void ShowSidebar(object nothing)
        {
            State.SidebarVisible = true;
            SidebarPane.IsHidden = false;
            _eventAggregator.GetEvent<SidebarToggledEvent>().Publish(true);
        }

        private void HideSidebar(object nothing)
        {
            State.SidebarVisible = false;
            SidebarPane.IsHidden = true;
            _eventAggregator.GetEvent<SidebarToggledEvent>().Publish(false);
        }

        public void CheckConnections(Guid unused)
        {
            var hasConnections = State != null && State.Connections != null && State.Connections.Count > 0;
            if (!hasConnections)
                HideSidebar(null);
        }

        #endregion
    }
}
