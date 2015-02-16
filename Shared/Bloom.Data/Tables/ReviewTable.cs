﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    public class ReviewTable : ISqlTable
    {
        /// <summary>
        /// Gets the create review table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE \"review\" (" +
                       "\"id\" VARCHAR(36) PRIMARY KEY NOT NULL UNIQUE , " +
                       "\"url\" VARCHAR , " +
                       "\"title\" VARCHAR , " +
                       "\"body\" VARCHAR , " +
                       "\"published_on\" DATETIME ," +
                       "\"publication_id\" VARCHAR(36) , " +
                       "\"author_id\" VARCHAR(36) )";
            }
        }
    }
}
