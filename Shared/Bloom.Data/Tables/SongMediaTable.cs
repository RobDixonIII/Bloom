﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    public class SongMediaTable : ISqlTable
    {
        /// <summary>
        /// Gets the create song_media table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE song_media (" +
                       "id VARCHAR(36) PRIMARY KEY NOT NULL UNIQUE , " +
                       "song_id VARCHAR(36) NOT NULL , " +
                       "media_type INTEGER NOT NULL , " +
                       "digital_format INTEGER , " +
                       "file_path VARCHAR , " +
                       "is_compressed BOOL NOT NULL DEFAULT FALSE , " +
                       "is_protected BOOL NOT NULL DEFAULT FALSE , " +
                       "is_damaged BOOL NOT NULL DEFAULT FALSE , " +
                       "file_size INTEGER , " +
                       "sample_rate INTEGER , " +
                       "bit_rate INTEGER , " +
                       "volume_offset INTEGER , " +
                       "received_from_person_id VARCHAR(36) , " +
                       "FOREIGN KEY (received_from_person_id) REFERENCES person(id) , " +
                       "FOREIGN KEY (song_id) REFERENCES song(id) )";
            }
        }
    }
}