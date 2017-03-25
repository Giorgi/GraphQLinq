namespace HSL
{
    using System.Collections.Generic;

    public partial class Alert : Node
    {
        public string id { get; set; }
        public Agency agency { get; set; }
        public Route route { get; set; }
        public Trip trip { get; set; }
        public Stop stop { get; set; }
        public List<Pattern> patterns { get; set; }
        public string alertHeaderText { get; set; }
        public List<TranslatedString> alertHeaderTextTranslations { get; set; }
        public string alertDescriptionText { get; set; }
        public List<TranslatedString> alertDescriptionTextTranslations { get; set; }
        public string alertUrl { get; set; }
        public long effectiveStartDate { get; set; }
        public long effectiveEndDate { get; set; }
    }
}