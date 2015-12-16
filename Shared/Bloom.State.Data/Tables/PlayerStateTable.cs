﻿using Bloom.Data.Interfaces;

namespace Bloom.State.Data.Tables
{
    /// <summary>
    /// The player_state table.
    /// </summary>
    public class PlayerStateTable : ISqlTable
    {
        /// <summary>
        /// Gets the create player_state table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE \"player_state\" (" +
                       "\"process_name\" VARCHAR NOT NULL , " +
                       "\"person_id\" VARCHAR(36) NOT NULL , " +
                       "\"skin_name\" VARCHAR NOT NULL , " +
                       "\"window_state\" INTEGER NOT NULL , " +
                       "\"recent_width\" INTEGER NOT NULL , " +
                       "\"upcoming_width\" INTEGER NOT NULL , " +
                       "PRIMARY KEY (process_name, person_id))";
            }
        }
    }
}
