﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    /// <summary>
    /// Represents the song_reference table.
    /// </summary>
    /// <seealso cref="Bloom.Data.Interfaces.ISqlTable" />
    public class SongReferenceTable : ISqlTable
    {
        /// <summary>
        /// Gets the create song_reference table SQL.
        /// </summary>
        public string CreateSql => "CREATE TABLE song_reference (" +
                                   "song_id VARCHAR(36) NOT NULL , " +
                                   "reference_id VARCHAR(36) NOT NULL , " +
                                   "PRIMARY KEY (song_id, reference_id) , " +
                                   "FOREIGN KEY (song_id) REFERENCES song(id) , " +
                                   "FOREIGN KEY (reference_id) REFERENCES reference(id) )";
    }
}