namespace TestServer
{
    using GraphQLinq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public class QueryContext : GraphContext
    {
        public QueryContext(HttpClient httpClient) : base(httpClient)
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