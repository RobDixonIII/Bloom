﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    public class LabelPersonelTable : ISqlTable
    {
        /// <summary>
        /// Gets the create label_personel table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE label_personel (" +
                       "id VARCHAR(36) PRIMARY KEY NOT NULL UNIQUE , " +
                       "label_id VARCHAR(36) NOT NULL , " +
                       "person_id VARCHAR(36) NOT NULL , " +
                       "FOREIGN KEY (label_id) REFERENCES label(id) , " +
                       "FOREIGN KEY (person_id) REFERENCES person(id) )";
            }
        }
    }
}
