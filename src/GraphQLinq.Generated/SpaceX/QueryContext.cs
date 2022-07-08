namespace SpaceX
{
    using GraphQLinq;
    using System;
    using System.Collections.Generic;

    public partial class QueryContext : GraphContext
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
            return BuildCollectionQuery<Users>(parameterValues, "users");
        }

        public GraphItemQuery<Users_aggregate> Users_aggregate(List<Users_select_column> distinct_on, int? limit, int? offset, List<Users_order_by> order_by, Users_bool_exp where)
        {
            var parameterValues = new object[] { distinct_on, limit, offset, order_by, where };
            return BuildItemQuery<Users_aggregate>(parameterValues, "users_aggregate");
        }

        public GraphItemQuery<Users> Users_by_pk(Guid id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Users>(parameterValues, "users_by_pk");
        }

        public GraphCollectionQuery<Capsule> Capsules(CapsulesFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Capsule>(parameterValues, "capsules");
        }

        public GraphCollectionQuery<Capsule> CapsulesPast(CapsulesFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Capsule>(parameterValues, "capsulesPast");
        }

        public GraphCollectionQuery<Capsule> CapsulesUpcoming(CapsulesFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Capsule>(parameterValues, "capsulesUpcoming");
        }

        public GraphItemQuery<Capsule> Capsule(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Capsule>(parameterValues, "capsule");
        }

        public GraphItemQuery<Info> Company()
        {
            var parameterValues = new object[] { };
            return BuildItemQuery<Info>(parameterValues, "company");
        }

        public GraphCollectionQuery<Core> Cores(CoresFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Core>(parameterValues, "cores");
        }

        public GraphCollectionQuery<Core> CoresPast(CoresFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Core>(parameterValues, "coresPast");
        }

        public GraphCollectionQuery<Core> CoresUpcoming(CoresFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Core>(parameterValues, "coresUpcoming");
        }

        public GraphItemQuery<Core> Core(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Core>(parameterValues, "core");
        }

        public GraphCollectionQuery<Dragon> Dragons(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Dragon>(parameterValues, "dragons");
        }

        public GraphItemQuery<Dragon> Dragon(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Dragon>(parameterValues, "dragon");
        }

        public GraphCollectionQuery<History> Histories(HistoryFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<History>(parameterValues, "histories");
        }

        public GraphItemQuery<HistoriesResult> HistoriesResult(HistoryFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildItemQuery<HistoriesResult>(parameterValues, "historiesResult");
        }

        public GraphItemQuery<History> History(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<History>(parameterValues, "history");
        }

        public GraphCollectionQuery<Landpad> Landpads(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Landpad>(parameterValues, "landpads");
        }

        public GraphItemQuery<Landpad> Landpad(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Landpad>(parameterValues, "landpad");
        }

        public GraphCollectionQuery<Launch> Launches(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Launch>(parameterValues, "launches");
        }

        public GraphCollectionQuery<Launch> LaunchesPast(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Launch>(parameterValues, "launchesPast");
        }

        public GraphItemQuery<LaunchesPastResult> LaunchesPastResult(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildItemQuery<LaunchesPastResult>(parameterValues, "launchesPastResult");
        }

        public GraphCollectionQuery<Launch> LaunchesUpcoming(LaunchFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Launch>(parameterValues, "launchesUpcoming");
        }

        public GraphItemQuery<Launch> Launch(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Launch>(parameterValues, "launch");
        }

        public GraphItemQuery<Launch> LaunchLatest(int? offset)
        {
            var parameterValues = new object[] { offset };
            return BuildItemQuery<Launch>(parameterValues, "launchLatest");
        }

        public GraphItemQuery<Launch> LaunchNext(int? offset)
        {
            var parameterValues = new object[] { offset };
            return BuildItemQuery<Launch>(parameterValues, "launchNext");
        }

        public GraphCollectionQuery<Launchpad> Launchpads(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Launchpad>(parameterValues, "launchpads");
        }

        public GraphItemQuery<Launchpad> Launchpad(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Launchpad>(parameterValues, "launchpad");
        }

        public GraphCollectionQuery<Mission> Missions(MissionsFind find, int? limit, int? offset)
        {
            var parameterValues = new object[] { find, limit, offset };
            return BuildCollectionQuery<Mission>(parameterValues, "missions");
        }

        public GraphItemQuery<MissionResult> MissionsResult(MissionsFind find, int? limit, int? offset)
        {
            var parameterValues = new object[] { find, limit, offset };
            return BuildItemQuery<MissionResult>(parameterValues, "missionsResult");
        }

        public GraphItemQuery<Mission> Mission(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Mission>(parameterValues, "mission");
        }

        public GraphCollectionQuery<Payload> Payloads(PayloadsFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Payload>(parameterValues, "payloads");
        }

        public GraphItemQuery<Payload> Payload(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Payload>(parameterValues, "payload");
        }

        public GraphItemQuery<Roadster> Roadster()
        {
            var parameterValues = new object[] { };
            return BuildItemQuery<Roadster>(parameterValues, "roadster");
        }

        public GraphCollectionQuery<Rocket> Rockets(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildCollectionQuery<Rocket>(parameterValues, "rockets");
        }

        public GraphItemQuery<RocketsResult> RocketsResult(int? limit, int? offset)
        {
            var parameterValues = new object[] { limit, offset };
            return BuildItemQuery<RocketsResult>(parameterValues, "rocketsResult");
        }

        public GraphItemQuery<Rocket> Rocket(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Rocket>(parameterValues, "rocket");
        }

        public GraphCollectionQuery<Ship> Ships(ShipsFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildCollectionQuery<Ship>(parameterValues, "ships");
        }

        public GraphItemQuery<ShipsResult> ShipsResult(ShipsFind find, int? limit, int? offset, string order, string sort)
        {
            var parameterValues = new object[] { find, limit, offset, order, sort };
            return BuildItemQuery<ShipsResult>(parameterValues, "shipsResult");
        }

        public GraphItemQuery<Ship> Ship(string id)
        {
            var parameterValues = new object[] { id };
            return BuildItemQuery<Ship>(parameterValues, "ship");
        }
    }
}