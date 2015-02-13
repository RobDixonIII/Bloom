﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bloom.Browser.AlbumModule.ViewModels;
using Bloom.Browser.AlbumModule.Views;
using Bloom.Controls;
using Bloom.Domain.Enums;
using Bloom.PubSubEvents;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Bloom.Browser.AlbumModule.Services
{
    public class AlbumService : IAlbumService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public AlbumService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _tabs = new List<Tab>();

            // Subscribe to events
            _eventAggregator.GetEvent<NewAlbumTabEvent>().Subscribe(NewAlbumTab);
            _eventAggregator.GetEvent<DuplicateTabEvent>().Subscribe(DuplicateAlbumTab);
        }
        private readonly IEventAggregator _eventAggregator;

        public void NewAlbumTab(object nothing)
        {
            NewAlbumTab();
        }

        public void NewAlbumTab()
        {
            var albumViewModel = new AlbumViewModel();
            var albumView = new AlbumView(albumViewModel);
            var albumTab = new Tab
            {
                Id = Guid.NewGuid(),
                EntityType = EntityType.Album,
                Header = "Album",
                Content = albumView
            };

            _tabs.Add(albumTab);
            _eventAggregator.GetEvent<AddTabEvent>().Publish(albumTab);
        }

        public void DuplicateAlbumTab(Guid tabId)
        {
            var existingTab = _tabs.FirstOrDefault(tab => tab.Id == tabId);
            if (existingTab == null)
                return;

            var albumViewModel = new AlbumViewModel();
            var albumView = new AlbumView(albumViewModel);
            var albumTab = new Tab
            {
                Id = Guid.NewGuid(),
                EntityType = EntityType.Album,
                Header = "Album",
                Content = albumView
            };

            _tabs.Add(albumTab);
            _eventAggregator.GetEvent<AddTabEvent>().Publish(albumTab);
        }

        private readonly List<Tab> _tabs;
    }
}