﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    /// <summary>
    /// Represents the genre table.
    /// </summary>
    /// <seealso cref="Bloom.Data.Interfaces.ISqlTable" />
    public class GenreTable : ISqlTable
    {
        /// <summary>
        /// Gets the create genre table SQL.
        /// </summary>
        public string CreateSql => "CREATE TABLE genre (" +
                                   "id VARCHAR(36) PRIMARY KEY NOT NULL UNIQUE , " +
                                   "name VARCHAR NOT NULL , " +
                                   "description VARCHAR , " +
                                   "parent_genre_id VARCHAR(36) , " +
                                   "FOREIGN KEY (parent_genre_id) REFERENCES genre(id) )";
    }
}