﻿using System.Text.Json.Serialization;

namespace NppesIntake.Core.DTOs;

public class NppesAddressDto
{
    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }

    [JsonPropertyName("address_purpose")]
    public string AddressPurpose { get; set; }

    [JsonPropertyName("address_type")]
    public string AddressType { get; set; }

    [JsonPropertyName("address_1")]
    public string Address1 { get; set; }

    [JsonPropertyName("address_2")]
    public string Address2 { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }

    [JsonPropertyName("telephone_number")]
    public string TelephoneNumber { get; set; }
}