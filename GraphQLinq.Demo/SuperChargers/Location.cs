using System.Collections.Generic;

public partial class Location : Node
{
    public string region { get; set; }
    public int kioskPinY { get; set; }
    public int kioskZoomPinY { get; set; }
    public string path { get; set; }
    public float longitude { get; set; }
    public string provinceState { get; set; }
    public string addressLine2 { get; set; }
    public string address { get; set; }
    public string Country { get; set; }
    public float latitude { get; set; }
    public string locationId { get; set; }
    public List<Phone> salesPhone { get; set; }
    public int kioskZoomPinX { get; set; }
    public string city { get; set; }
    public bool isGallery { get; set; }
    public string commonName { get; set; }
    public string destinationChargerLogo { get; set; }
    public string title { get; set; }
    public string id { get; set; }
    public bool openSoon { get; set; }
    public int nid { get; set; }
    public string hours { get; set; }
    public int kioskPinX { get; set; }
    public string addressNotes { get; set; }
    public string destinationWebsite { get; set; }
    public List<string> locationType { get; set; }
    public string subRegion { get; set; }
    public string addressLine1 { get; set; }
    public bool salesRepresentative { get; set; }
    public string amentities { get; set; }
    public string postalCode { get; set; }
    public string geocode { get; set; }
    public List<Email> emails { get; set; }
    public string chargers { get; set; }
    public string directionsLink { get; set; }
}