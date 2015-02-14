﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    public class AlbumTagTable : ISqlTable
    {
        /// <summary>
        /// Gets the create album_tag table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE \"album_tag\" (" +
                       "\"album_id\" VARCHAR(36) NOT NULL , " +
                       "\"tag_id\" VARCHAR(36) NOT NULL , " +
                       "PRIMARY KEY (\"album_id\", \"tag_id\") )";
            }
        }
    }
}