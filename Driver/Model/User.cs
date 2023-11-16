using Newtonsoft.Json;

namespace Driver.Model;

public class User
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("address")]
    public Address Address { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("website")]
    public string Website { get; set; }

    [JsonProperty("company")]
    public Company Company { get; set; }
    
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Username: {Username}, Email: {Email}, Phone: {Phone}, Website: {Website}, Address: {Address}, Company: {Company}";
    }

}

public class Address
{
    [JsonProperty("street")]
    public string Street { get; set; }

    [JsonProperty("suite")]
    public string Suite { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("zipcode")]
    public string Zipcode { get; set; }

    [JsonProperty("geo")]
    public Geo Geo { get; set; }
    public override string ToString()
    {
        return $"Street: {Street}, Suite: {Suite}, City: {City}, Zipcode: {Zipcode}, Geo: {Geo}";
    }
}

public class Geo
{
    [JsonProperty("lat")]
    public string Lat { get; set; }

    [JsonProperty("lng")]
    public string Lng { get; set; }
    // In Geo class:
    public override string ToString()
    {
        return $"Lat: {Lat}, Lng: {Lng}";
    }
}

public class Company
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("catchPhrase")]
    public string CatchPhrase { get; set; }

    [JsonProperty("bs")]
    public string Bs { get; set; }
    // In Company class:
    public override string ToString()
    {
        return $"Name: {Name}, CatchPhrase: {CatchPhrase}, Bs: {Bs}";
    }
}
