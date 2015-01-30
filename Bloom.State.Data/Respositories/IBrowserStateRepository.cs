﻿using Bloom.State.Domain.Models;

namespace Bloom.State.Data.Respositories
{
    /// <summary>
    /// Repository for the browser state.
    /// </summary>
    public interface IBrowserStateRepository
    {
        /// <summary>
        /// Determines whether the browser state exists.
        /// </summary>
        bool BrowserStateExists();

        /// <summary>
        /// Gets the browser state.
        /// </summary>
        BrowserState GetBrowserState();

        /// <summary>
        /// Adds the state of the browser.
        /// </summary>
        /// <param name="browserState">State of the browser.</param>
        void AddBrowserState(BrowserState browserState);
    }
}
