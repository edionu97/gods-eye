class GeolocationModel {
  final String countryCode;
  final String countryName;
  final String regionCode;
  final String regionName;
  final String city;
  final String zipCode;
  final double latitude;
  final String longitude;

  GeolocationModel(
      {this.countryCode,
      this.countryName,
      this.regionCode,
      this.regionName,
      this.city,
      this.zipCode,
      this.latitude,
      this.longitude});
}
