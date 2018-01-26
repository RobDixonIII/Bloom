﻿using Bloom.Data.Interfaces;

namespace Bloom.State.Data.Tables
{
    /// <summary>
    /// Represents the tab table.
    /// </summary>
    public class TabTable : ISqlTable
    {
        /// <summary>
        /// Gets the create tab table SQL.
        /// </summary>
        public string CreateSql => "CREATE TABLE tab (" +
                                   "id VARCHAR(36) PRIMARY KEY NOT NULL UNIQUE , " +
                                   "process INTEGER NOT NULL , " +
                                   "person_id VARCHAR(36) NOT NULL , " +
                                   "library_id VARCHAR(36) , " +
                                   "entity_id VARCHAR(36) , " +
                                   "type INTEGER NOT NULL , " +
                                   "view VARCHAR , " +
                                   "header VARCHAR , " +
                                   "\"order\" INTEGER NOT NULL )";
    }
}