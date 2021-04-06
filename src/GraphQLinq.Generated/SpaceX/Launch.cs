namespace SpaceX
{
    using System;
    using System.Collections.Generic;

    public partial class Launch
    {
        public string Details { get; set; }
        public string Id { get; set; }
        public bool? Is_tentative { get; set; }
        public DateTime? Launch_date_local { get; set; }
        public DateTime? Launch_date_unix { get; set; }
        public DateTime? Launch_date_utc { get; set; }
        public LaunchSite Launch_site { get; set; }
        public bool? Launch_success { get; set; }
        public string Launch_year { get; set; }
        public LaunchLinks Links { get; set; }
        public List<string> Mission_id { get; set; }
        public string Mission_name { get; set; }
        public LaunchRocket Rocket { get; set; }
        public DateTime? Static_fire_date_unix { get; set; }
        public DateTime? Static_fire_date_utc { get; set; }
        public LaunchTelemetry Telemetry { get; set; }
        public string Tentative_max_precision { get; set; }
        public bool? Upcoming { get; set; }
        public List<Ship> Ships { get; set; }
    }
}