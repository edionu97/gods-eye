import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/services/models/common/model.dart';

class ActiveWorkerFailedMessage implements IAbstractModel {
  final String status;
  final String details;
  final ActiveSearchRequestModel failedSearchReq;

  static ActiveWorkerFailedMessage createFromJson(final dynamic json) {
    //set the proper details base on status
    String details = "";
    switch (json["Status"]?.toString()) {
      //treat the case when the status is unavailable
      case "Unavailable":
        {
          details =
              "The request could not be processed because the recognition service could not be reached. It may be stopped or unreachable. Please try again later";
          break;
        }
      //treat the case when the status is canceled
      case "Cancelled":
        {
          details =
              "The request was submitted successfully to the service, but it was canceled by user. If there are any results, they should be in the search results menu.";
          break;
        }
      //treat the unknown case
      case "Unknown":
        {
          details =
              "The request was submitted to the recognition service but could not be completed due processing errors. The full details are: ${json["FailureDetails"]}";
        }
    }
    //create a new instance of the worker
    return ActiveWorkerFailedMessage(
        //set the status
        status: json["Status"],
        //set the details
        details: details,
        //set the failed search request info
        failedSearchReq:
            ActiveSearchRequestModel.convertFromJson(json["FailedJobDetails"]));
  }

  const ActiveWorkerFailedMessage(
      {this.failedSearchReq, this.status, this.details});
}
