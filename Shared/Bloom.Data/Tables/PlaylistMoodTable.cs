﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    public class PlaylistMoodTable : ISqlTable
    {
        /// <summary>
        /// Gets the create playlist_mood table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE \"playlist_mood\" (" +
                       "\"playlist_id\" VARCHAR(36) NOT NULL , " +
                       "\"mood_id\" VARCHAR(36) NOT NULL , " +
                       "PRIMARY KEY (\"playlist_id\", \"mood_id\") )";
            }
        }
    }
}