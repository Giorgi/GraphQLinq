namespace HSL
{
    using System.Collections.Generic;

    public partial class fare
    {
        public string type { get; set; }
        public string currency { get; set; }
        public int cents { get; set; }
        public List<fareComponent> components { get; set; }
    }
}