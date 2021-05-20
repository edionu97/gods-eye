class GeolocationModel {
  final String countryCode;
  final String countryName;
  final String regionCode;
  final String regionName;
  final String city;
  final String zipCode;
  final double latitude;
  final String longitude;

  GeolocationModel({this.countryCode,
    this.countryName,
    this.regionCode,
    this.regionName,
    this.city,
    this.zipCode,
    this.latitude,
    this.longitude});

  /// Convert the json dart object
  static convertFromJson(final dynamic jsonObject) {
    return GeolocationModel(
        countryCode: jsonObject["country_code"],
        countryName: jsonObject["country_name"],
        regionCode: jsonObject["region_code"],
        regionName: jsonObject["region_name"],
        city: jsonObject["city"],
        zipCode: jsonObject["zip_code"],
        latitude: jsonObject["latitude"],
        longitude: jsonObject["longitude"]);
  }
}
