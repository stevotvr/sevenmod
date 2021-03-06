﻿// <copyright file="QueryCompletedEventArgs.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System;
    using System.Data;

    /// <summary>
    /// Contains arguments for the <see cref="ThreadedQuery.QueryCompleted"/> event.
    /// </summary>
    public class QueryCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="database">The <see cref="Database.Database"/> object used to make the query.</param>
        /// <param name="affectedRows">The number of rows affected by the query.</param>
        /// <param name="results">The <see cref="DataTable"/> object containing the results returned by the query.</param>
        /// <param name="data">The data associated with the query.</param>
        /// <param name="exception">The exception that occurred.</param>
        internal QueryCompletedEventArgs(Database database, int affectedRows, DataTable results, object data, Exception exception)
        {
            this.Success = exception == null;
            this.Database = database;
            this.AffectedRows = affectedRows;
            this.Results = results;
            this.Data = data;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets a value indicating whether the query completed successfully.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the <see cref="Database.Database"/> object used to make the query.
        /// </summary>
        public Database Database { get; }

        /// <summary>
        /// Gets the number of rows affected by the query.
        /// </summary>
        public int AffectedRows { get; }

        /// <summary>
        /// Gets the <see cref="DataTable"/> object containing the results returned by the query.
        /// </summary>
        public DataTable Results { get; }

        /// <summary>
        /// Gets the data associated with the query.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Gets the exception describing the error that occurred if the query failed.
        /// </summary>
        public Exception Exception { get; }
    }
}
