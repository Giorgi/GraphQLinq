﻿using System;
using System.Collections.Generic;
using HSL;

namespace GraphQLinq
{
    public class HslGraphContext : GraphContext
    {
        public HslGraphContext(string baseUrl) : base(baseUrl, "") { }
        public HslGraphContext(string baseUrl, string authorization) : base(baseUrl, authorization) { }

        public GraphItemQuery<Trip> Trip(string id)
        {
            var parameterValues = new object[] { id };

            return BuildItemQuery<Trip>(parameterValues, "trip");
        }

        public GraphCollectionQuery<Stop> Stations()
        {
            var parameterValues = new object[] { };

            return BuildCollectionQuery<Stop>(parameterValues, "stations");
        }

        public GraphItemQuery<Agency> Agency(string id)
        {
            var parameterValues = new object[] { id };

            return BuildItemQuery<Agency>(parameterValues, "agency");
        }


        //      query {
        //trip(id:"HSL:1055_20170501_To_1_1205")
        //      {
        //          id,
        //	serviceId,
        //	gtfsId,
        //	pattern {
        //              id,
        //   name,
        //   headsign,
        //    geometry{
        //                  lat
        //                  lon
        //    }
        //          },
        //  stoptimesForDate(serviceDay: "20170318"){
        //              headsign,
        //    realtimeState
        //    scheduledArrival

        //    }
        //      }
        //  }
    }

    public static class QueryExtensions
    {
        public static List<Stoptime> stoptimesForDate(this Trip trip, string serviceDay)
        {
            throw new NotImplementedException("This method is not implemented. It exists solely for query purposes.");
        }

        public static List<StoptimesInPattern> stoptimesForServiceDate(this Stop stop, string date)
        {
            throw new NotImplementedException("This method is not implemented. It exists solely for query purposes.");
        }
    }
}