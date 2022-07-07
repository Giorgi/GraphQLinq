using System;
using System.Collections.Generic;

namespace GraphQLinq
{
    public class GraphQueryExecutionException : Exception
    {
        public GraphQueryExecutionException(string query) : base("Unexpected error response received from server.")
        {
            GraphQLQuery = query;
        }

        public GraphQueryExecutionException(IEnumerable<GraphQueryError> errors, string query)
            : base($"One or more errors occurred during query execution. Check {nameof(Errors)} property for details")
        {
            Errors = errors;
            GraphQLQuery = query;
        }

        public string GraphQLQuery { get; private set; }
        public IEnumerable<GraphQueryError> Errors { get; private set; }
    }
    
    public class GraphQueryError
    {
        public string Message { get; set; }
        public ErrorLocation[] Locations { get; set; }
    }

    public class ErrorLocation
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }
}