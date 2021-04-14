namespace SpaceX
{
    using GraphQLinq;
    using System;
    using System.Collections.Generic;

    public class QueryContext : GraphContext
    {
        public QueryContext() : this("https://api.spacex.land/graphql")
        {
        }

        public QueryContext(string baseUrl) : base(baseUrl, "")
        {
        }

        public GraphCollectionQuery<Users> Users(List<Users_select_column> distinct_on, int? limit, int? offset, List<Users_order_by> order_by, Users_bool_exp where)
        {
            var parameterValues = new object[] { distinct_on, limit, offset, order_by, where };
            return BuildCollectionQuery<Users>(parameterValues);
        }

        public GraphItemQuery<Users_aggregate> Users_aggregate(List<Users_select_column> distinct_on, int? limit, int? offset, List<Users_order_by> order_by, Users_bool_exp where)
        {
            var parameterValues = new object[] { distinct_on, limit, offset, order_by, where };
            return BuildItemQuery<Users_aggregate>(parameterValues);
        }

        public GraphItemQuery<Users> Users_by_pk(Guid id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Users>(parameterValues);
        }

        public GraphCollectionQuery<Capsule> Capsules(CapsulesFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Capsule>(parameterValues);
        }

        public GraphCollectionQuery<Capsule> CapsulesPast(CapsulesFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Capsule>(parameterValues);
        }

        public GraphCollectionQuery<Capsule> CapsulesUpcoming(CapsulesFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Capsule>(parameterValues);
        }

        public GraphItemQuery<Capsule> Capsule(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Capsule>(parameterValues);
        }

        public GraphItemQuery<Info> Company()
        {
            var parameterValues = new object[] { };
            return BuildItemQuery<Info>(parameterValues);
        }

        public GraphCollectionQuery<Core> Cores(CoresFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Core>(parameterValues);
        }

        public GraphCollectionQuery<Core> CoresPast(CoresFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Core>(parameterValues);
        }

        public GraphCollectionQuery<Core> CoresUpcoming(CoresFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Core>(parameterValues);
        }

        public GraphItemQuery<Core> Core(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Core>(parameterValues);
        }

        public GraphCollectionQuery<Dragon> Dragons(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Dragon>(parameterValues);
        }

        public GraphItemQuery<Dragon> Dragon(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Dragon>(parameterValues);
        }

        public GraphCollectionQuery<History> Histories(HistoryFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<History>(parameterValues);
        }

        public GraphItemQuery<HistoriesResult> HistoriesResult(HistoryFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildItemQuery<HistoriesResult>(parameterValues);
        }

        public GraphItemQuery<History> History(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<History>(parameterValues);
        }

        public GraphCollectionQuery<Landpad> Landpads(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Landpad>(parameterValues);
        }

        public GraphItemQuery<Landpad> Landpad(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Landpad>(parameterValues);
        }

        public GraphCollectionQuery<Launch> Launches(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Launch>(parameterValues);
        }

        public GraphCollectionQuery<Launch> LaunchesPast(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Launch>(parameterValues);
        }

        public GraphItemQuery<LaunchesPastResult> LaunchesPastResult(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildItemQuery<LaunchesPastResult>(parameterValues);
        }

        public GraphCollectionQuery<Launch> LaunchesUpcoming(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Launch>(parameterValues);
        }

        public GraphItemQuery<Launch> Launch(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Launch>(parameterValues);
        }

        public GraphItemQuery<Launch> LaunchLatest(int? offset)
        {
            var parameterValues = new object[] { offset };
            return BuildItemQuery<Launch>(parameterValues);
        }

        public GraphItemQuery<Launch> LaunchNext(int? offset)
        {
            var parameterValues = new object[] { offset };
            return BuildItemQuery<Launch>(parameterValues);
        }

        public GraphCollectionQuery<Launchpad> Launchpads(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Launchpad>(parameterValues);
        }

        public GraphItemQuery<Launchpad> Launchpad(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Launchpad>(parameterValues);
        }

        public GraphCollectionQuery<Mission> Missions(MissionsFind find, int? limit, int? offset)
        {
            var parameterValues = new object[] { find, limit, offset };
            return BuildCollectionQuery<Mission>(parameterValues);
        }

        public GraphItemQuery<MissionResult> MissionsResult(MissionsFind find, int? limit, int? offset)
        {
            var parameterValues = new object[] { find, limit, offset };
            return BuildItemQuery<MissionResult>(parameterValues);
        }

        public GraphItemQuery<Mission> Mission(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Mission>(parameterValues);
        }

        public GraphCollectionQuery<Payload> Payloads(PayloadsFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Payload>(parameterValues);
        }

        public GraphItemQuery<Payload> Payload(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Payload>(parameterValues);
        }

        public GraphItemQuery<Roadster> Roadster()
        {
            var parameterValues = new object[] { };
            return BuildItemQuery<Roadster>(parameterValues);
        }

        public GraphCollectionQuery<Rocket> Rockets(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Rocket>(parameterValues);
        }

        public GraphItemQuery<RocketsResult> RocketsResult(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildItemQuery<RocketsResult>(parameterValues);
        }

        public GraphItemQuery<Rocket> Rocket(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Rocket>(parameterValues);
        }

        public GraphCollectionQuery<Ship> Ships(ShipsFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Ship>(parameterValues);
        }

        public GraphItemQuery<ShipsResult> ShipsResult(ShipsFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildItemQuery<ShipsResult>(parameterValues);
        }

        public GraphItemQuery<Ship> Ship(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Ship>(parameterValues);
        }
    }
}