using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SpaceX;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace GraphQLinq.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await SpaceXQueryExamples();
        }

        private static async Task SpaceXQueryExamples()
        {
            var spaceXContext = new QueryContext();
            //Launch_date_unix and Static_fire_date_unix need custom converter
            spaceXContext.JsonSerializerOptions.Converters.Add(new UnixEpochDateTimeConverter());

            #region Company details
            var company = await spaceXContext.Company().ToItem();

            RenderCompanyDetails(company);
            #endregion

            #region Specific properties of company
            //Use an anonymous type to select specific properties
            var companySummaryAnonymous = spaceXContext.Company().Select(c => new { c.Ceo, c.Name, c.Headquarters }).ToItem();

            //Use data class to select specific properties
            var companySummary = await spaceXContext.Company().Select(c => new CompanySummary
            {
                Ceo = c.Ceo,
                Name = c.Name,
                Headquarters = c.Headquarters
            }).ToItem();

            RenderCompanySummary(companySummary);
            #endregion

            #region Include navigation properties
            var companyWithHeadquartersAndLinks = await spaceXContext.Company()
                                                                .Include(info => info.Headquarters)
                                                                .Include(info => info.Links).ToItem();

            RenderCompanyDetailsAndLinks(companyWithHeadquartersAndLinks);
            #endregion

            #region Filter missions, compose queries
            var missionsQuery = spaceXContext.Missions(new MissionsFind { Manufacturer = "Orbital ATK" }, null, null)
                                                 .Include(mission => mission.Manufacturers);
            var missions = await missionsQuery.ToEnumerable();

            RenderMissions(missions);

            var missionsWithPayloads = await missionsQuery.Include(mission => mission.Payloads).ToEnumerable();

            RenderMissions(missionsWithPayloads, true);
            #endregion

            #region Multiple levels of Includes

            var launches = await spaceXContext.Launches(null, 10, 0, null, null)
                                        .Include(launch => launch.Links)
                                        .Include(launch => launch.Rocket)
                                        .Include(launch => launch.Rocket.Second_stage.Payloads.Select(payload => payload.Manufacturer))
                                        .ToEnumerable();

            RenderLaunches(launches);
            #endregion
        }

        private static void RenderLaunches(IEnumerable<Launch> launches)
        {
            var table = new Table().Title("Launches");
            table.AddColumn(nameof(Launch.Mission_name), column => column.Width = 12).AddColumn(nameof(Launch.Launch_date_utc), column => column.Width = 15)
                 .AddColumn(nameof(Launch.Rocket.Rocket_name)).AddColumn(nameof(Launch.Links)).AddColumn(
                     $"{nameof(Launch.Rocket.Second_stage.Payloads)}  {nameof(Payload.Manufacturer)}", column => column.Width = 12);

            foreach (var launch in launches)
            {
                var linksTable = new Table().AddColumn(nameof(launch.Links));
                linksTable.AddRow(new Markup($"[link={launch.Links.Article_link}]Article_link - {launch.Links.Article_link}[/]"));
                linksTable.AddRow(new Markup($"[link={launch.Links.Video_link}]Video_link - {launch.Links.Video_link}[/]"));
                linksTable.AddRow(new Markup($"[link={launch.Links.Presskit}]Presskit - {launch.Links.Presskit}[/]"));
                linksTable.AddRow(new Markup($"[link={launch.Links.Reddit_launch}]Reddit_launch - {launch.Links.Reddit_launch}[/]"));
                linksTable.AddRow(new Markup($"[link={launch.Links.Wikipedia}]Wikipedia - {launch.Links.Wikipedia}[/]"));

                var payloadManufacturers = string.Join(Environment.NewLine, launch.Rocket.Second_stage.Payloads.Where(payload => payload != null)
                                                 .Select(p => p.Manufacturer));

                table.AddRow(new Markup(launch.Mission_name),
                             new Markup(launch.Launch_date_utc.ToString()),
                             new Markup(launch.Rocket.Rocket_name),
                             linksTable,
                             new Markup(payloadManufacturers));
            }

            AnsiConsole.Render(table);
            AnsiConsole.MarkupLine("");
        }

        private static void RenderMissions(IEnumerable<Mission> missions, bool showPayload = false)
        {
            var table = new Table().Title(showPayload ? "Missions with Payload" : "Missions");

            table.AddColumn(nameof(Mission.Name)).AddColumn(nameof(Mission.Description)).AddColumn(nameof(Mission.Manufacturers));

            if (showPayload)
            {
                table.AddColumn("Payloads");
            }

            foreach (var mission in missions)
            {
                var manufacturers = string.Join(Environment.NewLine, mission.Manufacturers);

                var rowItems = new List<IRenderable> { new Markup(mission.Name), new Markup(mission.Description), new Markup(manufacturers) };

                if (showPayload)
                {
                    var payloadsTable = new Table().AddColumn("Property").AddColumn("Value").Centered();

                    //For some reason the server returns null items in the list
                    foreach (var payload in mission.Payloads.Where(payload => payload != null))
                    {
                        payloadsTable.AddRow(nameof(payload.Nationality), payload.Nationality);
                        payloadsTable.AddRow(nameof(payload.Payload_mass_kg), payload.Payload_mass_kg.ToString());
                        payloadsTable.AddRow(nameof(payload.Orbit), payload.Orbit);
                        payloadsTable.AddRow(nameof(payload.Reused), payload.Reused.ToString());

                        payloadsTable.AddEmptyRow();
                    }

                    rowItems.Add(payloadsTable);
                }

                table.AddRow(rowItems);
            }

            AnsiConsole.Render(table);
            AnsiConsole.MarkupLine("");
        }

        private static void RenderCompanyDetails(Info company)
        {
            var table = new Table().Title("Company Details");

            table.AddColumn("Property").AddColumn("Value").Centered();

            table.AddRow(nameof(company.Name), company.Name);
            table.AddRow(nameof(company.Ceo), company.Ceo);
            table.AddRow(nameof(company.Summary), company.Summary);
            table.AddRow(nameof(company.Founded), company.Founded.ToString());
            table.AddRow(nameof(company.Founder), company.Founder);
            table.AddRow(nameof(company.Employees), company.Employees.ToString());

            AnsiConsole.Render(table);
            AnsiConsole.MarkupLine("");
        }

        private static void RenderCompanyDetailsAndLinks(Info company)
        {
            var table = new Table().Title("Company Details and Headquarters");

            table.AddColumn("Property").AddColumn("Value").Centered();

            table.AddRow(nameof(company.Name), company.Name);
            table.AddRow(nameof(company.Ceo), company.Ceo);
            table.AddRow(nameof(company.Summary), company.Summary);
            table.AddRow(nameof(company.Founded), company.Founded.ToString());
            table.AddRow(nameof(company.Founder), company.Founder);
            table.AddRow(nameof(company.Employees), company.Employees.ToString());

            table.AddRow(new Markup(nameof(company.Headquarters)),
                new Panel(string.Join(Environment.NewLine, company.Headquarters.State, company.Headquarters.City, company.Headquarters.Address)));

            AnsiConsole.Render(table);

            AnsiConsole.MarkupLine($"[link={company.Links.Elon_twitter}]Elon Twitter - {company.Links.Elon_twitter}[/]");
            AnsiConsole.MarkupLine($"[link={company.Links.Flickr}]Flickr - {company.Links.Flickr}[/]");
            AnsiConsole.MarkupLine($"[link={company.Links.Twitter}]Twitter - {company.Links.Twitter}[/]");
            AnsiConsole.MarkupLine($"[link={company.Links.Website}]Website - {company.Links.Website}[/]");
            AnsiConsole.MarkupLine("");
        }

        private static void RenderCompanySummary(CompanySummary companyInfo)
        {
            var table = new Table().Title("Company Selected Details and Headquarters");

            table.AddColumn("Property").AddColumn("Value");

            table.AddRow(nameof(companyInfo.Name), companyInfo.Name);
            table.AddRow(nameof(companyInfo.Ceo), companyInfo.Ceo);
            table.AddRow(new Markup(nameof(companyInfo.Headquarters)),
                         new Panel(string.Join(Environment.NewLine, companyInfo.Headquarters.State, companyInfo.Headquarters.City, companyInfo.Headquarters.Address)));

            AnsiConsole.Render(table);
            AnsiConsole.MarkupLine("");
        }
    }

    class CompanySummary
    {
        public string Ceo { get; set; }
        public string Name { get; set; }
        public AddressType Headquarters { get; set; }
    }
    
    class UnixEpochDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return DateTime.UnixEpoch.AddSeconds(reader.GetInt64());
            }

            return JsonSerializer.Deserialize<DateTime>(ref reader, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
