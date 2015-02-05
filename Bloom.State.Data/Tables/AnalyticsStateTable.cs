﻿namespace Bloom.State.Data.Tables
{
    public class AnalyticsStateTable : ISqlTable
    {
        /// <summary>
        /// Gets the create analytics_state table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE \"analytics_state\" (" +
                       "\"process_name\" VARCHAR PRIMARY KEY NOT NULL UNIQUE , " +
                       "\"skin_name\" VARCHAR NOT NULL , " +
                       "\"window_state\" INTEGER NOT NULL , " +
                       "\"sidebar_width\" INTEGER NOT NULL , " +
                       "\"selected_tab_id\" VARCHAR(36) NOT NULL )";
            }
        }
    }
}