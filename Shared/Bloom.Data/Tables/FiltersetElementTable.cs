﻿using Bloom.Data.Interfaces;

namespace Bloom.Data.Tables
{
    public class FiltersetElementTable : ISqlTable
    {
        /// <summary>
        /// Gets the create filterset_element table SQL.
        /// </summary>
        public string CreateSql
        {
            get
            {
                return "CREATE TABLE filterset_element (" +
                       "filterset_id VARCHAR(36) NOT NULL , " +
                       "element_number INTEGER NOT NULL , " +
                       "element_type INTEGER NOT NULL , " +
                       "filter_id VARCHAR(36) NOT NULL , " +
                       "filter_comparison INTEGER NOT NULL , " +
                       "filter_against VARCHAR , " +
                       "PRIMARY KEY (filterset_id, element_number) , " +
                       "FOREIGN KEY (filterset_id) REFERENCES filterset(id) )";
            }
        }
    }
}