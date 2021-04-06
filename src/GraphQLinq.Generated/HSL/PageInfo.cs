namespace HSL
{
    public partial class PageInfo
    {
        public bool hasNextPage { get; set; }
        public bool hasPreviousPage { get; set; }
        public string startCursor { get; set; }
        public string endCursor { get; set; }
    }
}