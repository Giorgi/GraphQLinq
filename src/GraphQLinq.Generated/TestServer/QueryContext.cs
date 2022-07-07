namespace TestServer
{
    using GraphQLinq;
    using System;
    using System.Collections.Generic;

    public class QueryContext : GraphContext
    {
        public QueryContext() : this("http://localhost:10000/graphql")
        {
        }

        public QueryContext(string baseUrl) : base(baseUrl, "")
        {
        }

        public GraphItemQuery<User> UserTemporaryFixForNullable(int? id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<User>(parameterValues, "userTemporaryFixForNullable");
        }

        public GraphItemQuery<User> User(int id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<User>(parameterValues, "user");
        }

        public GraphItemQuery<User> FailUser()
        {
            var parameterValues = new object[] { };
            return BuildItemQuery<User>(parameterValues, "failUser");
        }
    }
}