﻿using Bloom.Data.Interfaces;

namespace Bloom.State.Data.Tables
{
    /// <summary>
    /// The browser_state table.
    /// </summary>
    public class BrowserStateTable : ISqlTable
    {
        /// <summary>
        /// Gets the create browser_state table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE \"browser_state\" (" +
                       "\"process_name\" VARCHAR NOT NULL , " +
                       "\"person_id\" VARCHAR(36) NOT NULL , " +
                       "\"skin_name\" VARCHAR NOT NULL , " +
                       "\"window_state\" INTEGER NOT NULL , " +
                       "\"sidebar_width\" INTEGER NOT NULL , " +
                       "\"selected_tab_id\" VARCHAR(36) NOT NULL , " +
                       "PRIMARY KEY (process_name, person_id))";
            }
        }
    }
}
