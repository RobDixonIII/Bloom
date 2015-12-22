﻿using System;
using Bloom.Data.Interfaces;
using Bloom.Services;
using Bloom.State.Data.Respositories;
using Bloom.State.Domain.Models;

namespace Bloom.Player.State.Services
{
    /// <summary>
    /// Service for managing the player application state.
    /// </summary>
    public class PlayerStateService : StateBaseService, IPlayerStateService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStateService"/> class.
        /// </summary>
        /// <param name="stateDataSource">The state data source.</param>
        /// <param name="playerStateRepository">The player state repository.</param>
        public PlayerStateService(IDataSource stateDataSource, IPlayerStateRepository playerStateRepository)
        {
            StateDataSource = stateDataSource;
            _playerStateRepository = playerStateRepository;
        }
        private readonly IPlayerStateRepository _playerStateRepository;

        /// <summary>
        /// Initializes the player application state.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The file path to the state database file has not been specified.</exception>
        public PlayerState InitializeState(User user)
        {
            State = _playerStateRepository.GetPlayerState(user) ?? AddNewState(user);

            if (State.User == null)
                return (PlayerState) State;

            State.User.LastLogin = DateTime.Now;
            if (State.Connections == null || State.Connections.Count <= 0)
                return (PlayerState) State;

            foreach (var libraryConnection in State.Connections)
                libraryConnection.Connect(State.User);

            return (PlayerState) State;
        }

        private PlayerState AddNewState(User user)
        {
            var playerState = new PlayerState();
            playerState.SetUser(user);
            _playerStateRepository.AddPlayerState(playerState);
            return playerState;
        }
    }
}