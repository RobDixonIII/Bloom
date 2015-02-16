﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    public class PlaylistTrackTable : ISqlTable
    {
        /// <summary>
        /// Gets the create album_track table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE \"playlist_track\" (" +
                       "\"playlist_id\" VARCHAR(36) NOT NULL , " +
                       "\"song_id\" VARCHAR(36) NOT NULL ," +
                       "\"track_number\" INTEGER NOT NULL , " +
                       "PRIMARY KEY (\"playlist_id\", \"song_id\", \"track_number\") )";
            }
        }
    }
}