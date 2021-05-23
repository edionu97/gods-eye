import 'package:flutter/material.dart';
import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/geolocation/model.dart';
import 'package:gods_eye_app/services/models/person_found/search_result/model.dart';
import 'package:gods_eye_app/utils/helpers/conversion/image/convertor.dart';
import 'package:gods_eye_app/utils/helpers/conversion/primitives/convertor.dart';

class PersonFoundMessageModel extends IAbstractModel {
  //date when the searching started
  String _searchStartedAt;

  //date when the person was potentially identified
  String _personFoundAt;

  //the image of the searched person
  Image searchedPersonImage;

  //the response id (the same as job search id)
  final String responseId;

  //the image of the searched person
  final String searchedPersonImageString;

  //the geolocation of the person
  final GeolocationModel geoLocation;

  //the search result model
  final SearchResultModel searchResult;

  // convert the object from json
  static PersonFoundMessageModel convertFromJson(final dynamic json) {
    //return the new instance of the object
    return PersonFoundMessageModel(
        responseId: json['ResponseId'],
        searchStartedAt:
            PrimitivesConvertor.convertResponseDateToExpectedFormat(
                json['SearchStartedAt']),
        personFoundAt: PrimitivesConvertor.convertResponseDateToExpectedFormat(
            json['FoundAt']),
        geoLocation: GeolocationModel.convertFromJson(json['GeoLocation']),
        searchResult: SearchResultModel.convertFromJson(json['SearchResult']),
        searchedPersonImageString: json['SearchedPersonImage']);
  }

  PersonFoundMessageModel(
      {this.responseId,
      String searchStartedAt,
      String personFoundAt,
      this.geoLocation,
      this.searchResult,
      this.searchedPersonImageString}) {

    //convert the image from base 64 into img
    searchedPersonImage = null;
    if (searchedPersonImageString != null &&
        searchedPersonImageString.isNotEmpty) {
      //create the image from base64 string
      searchedPersonImage =
          ImageConvertor.imageFromBase64String(searchedPersonImageString);
    }

    _searchStartedAt = searchStartedAt;
    _personFoundAt = personFoundAt;
  }

  //get the date
  DateTime get startedAt =>
      PrimitivesConvertor.convertUtcStringDateToLocal(_searchStartedAt);

  //get the found at date
  DateTime get foundAt =>
      PrimitivesConvertor.convertUtcStringDateToLocal(_personFoundAt);
}
