import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/common/active_search_request_component/active_search_request_component.dart';
import 'package:gods_eye_app/services/facial_recognition/service.dart';
import 'package:gods_eye_app/services/messages/service.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/failure/model.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';
import 'package:gods_eye_app/services/notifications/service.dart';
import 'package:gods_eye_app/utils/components/bottom_right_button/component.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';
import 'package:gods_eye_app/utils/components/modal/component.dart';
import 'package:gods_eye_app/utils/components/top_corner_button/component.dart';
import 'package:image_picker/image_picker.dart';

/// This component is displayed on the HomeScreen => Person search
class PersonSearchScreenWidget extends StatefulWidget {
  final String userToken;

  const PersonSearchScreenWidget({this.userToken});

  @override
  State<StatefulWidget> createState() => _PersonSearchScreenWidgetState();
}

class _PersonSearchScreenWidgetState extends State<PersonSearchScreenWidget> {
  //get the image picker
  final ImagePicker imagePicker = ImagePicker();

  final Map<String, ActiveSearchRequestModel> _displayedWorkerRequests = {};

  @override
  void initState() {
    //call the super init state logic
    super.initState();

    //register the observer
    NotificationService().registerObserver(_onNotification);

    //do the initial interface update
    FacialRecognitionService().pingAllWorkersAsync(widget.userToken);
  }

  @override
  void dispose() {
    //unregister the observer
    NotificationService().unregisterObserver(_onNotification);

    //execute the dispose logic
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    //get the values from the active search request
    final List<ActiveSearchRequestModel> activeSearchRequests =
        _displayedWorkerRequests.values.toList();

    //create the stack
    return Stack(clipBehavior: Clip.none, children: [
      //define the column with grid
      Column(children: [
        SizedBox(height: 10),
        //expand the widget
        Expanded(
            child: Padding(
                //set the padding
                padding: const EdgeInsets.only(left: 25, right: 25),
                //create the centered item
                child: Center(
                    //if display the loader only if the list is empty
                    child: CircularSpinningLoader(
                        //if the list is empty, display the spinner
                        displayLoaderIf: () => activeSearchRequests.isEmpty,
                        //otherwise display the grid
                        elseDisplay:
                            _buildGridWidget(context, activeSearchRequests)))))
      ]),
      //create the bottom button
      BottomRightButton(onPressed: () => _addButtonClicked(context))
    ]);
  }

  /// Build the grid view
  Widget _buildGridWidget(BuildContext context,
      final List<ActiveSearchRequestModel> activeSearchRequests) {
    //return the grid
    return GridView.builder(
        clipBehavior: Clip.none,
        // the number of items from grid is equal with the number of items from list
        itemCount: activeSearchRequests.length,
        //the items are instances of remote workers
        itemBuilder: (BuildContext context, int index) =>
            //create the active search request instance
            ActiveSearchRequestWidget(
                extraText: "Running since: ",
                //set the data model
                activeSearchRequestModel: activeSearchRequests[index],
                //set the font size
                fontSize: 9.5,
                //set the max value of the opacity
                opacityValue: 1,
                //set the widget that will appear on top
                onTopWidget: TopCornerButton(
                  onTap: () => _deleteButtonClicked(
                      activeSearchRequests[index], context),
                )),
        //specifies the grid alignment
        gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 2, crossAxisSpacing: 10, mainAxisSpacing: 10));
  }

  /// In this region all the handlers are defined
  //#region Handlers

  /// This handler will be called when the [message] it is send
  /// Keep only the oldest requests
  void _onNotification(final String message) {
    //convert the json into an object
    IAbstractModel convertedObject =
        MessageParsingService().parseModelFromJson(message);

    //treat the failure case
    if (convertedObject is ActiveWorkerFailedMessage) {
      //do the interface update
      //set the state
      setState(() {
        //remove the item from the list
        _displayedWorkerRequests
            .remove(convertedObject.failedSearchReq?.searchRequestHashId);
      });
      return;
    }

    //check if the object is the right instance
    if (!(convertedObject is RemoteWorkerModel)) {
      return;
    }

    //get the active search requests for the model
    final List<ActiveSearchRequestModel> requestsFromWorker =
        (convertedObject as RemoteWorkerModel).activeSearchRequests;

    //iterate the active requests
    //and each time keep the older request from all the responses
    for (var workerRequest in requestsFromWorker) {
      //get the request id
      final String reqId = workerRequest.searchRequestHashId;

      //if the value is not in history
      if (!_displayedWorkerRequests.containsKey(reqId)) {
        //add it in history
        _displayedWorkerRequests[reqId] = workerRequest;
        //continue the flow
        continue;
      }

      //get the request items
      var requestItem = _displayedWorkerRequests[reqId];

      //if the item from the list is newer than the item from message
      if (requestItem.startedAt.isAfter(workerRequest.startedAt)) {
        requestItem = workerRequest;
      }

      //set the item
      _displayedWorkerRequests[reqId] = requestItem;
    }

    //set the state
    setState(() {});
  }

  //#endregion

  /// In this region all the actions are defined
  //#region Actions

  /// Create the handler for item deletion
  void _deleteButtonClicked(
      ActiveSearchRequestModel clickedRequest, BuildContext context) async {
    //try to execute the stopping request
    try {
      //
      await FacialRecognitionService().stopActiveSearchRequestAsync(
          widget.userToken, clickedRequest.imageBase64);

      //set the state
      setState(() {
        //remove the item from the list
        _displayedWorkerRequests.remove(clickedRequest.searchRequestHashId);
      });

      //do the interface update
      await FacialRecognitionService().pingAllWorkersAsync(widget.userToken);

      //handle the exception
    } on Exception catch (e) {
      //get message
      final message = Modal.extractMessageFromException(e);

      //get the message and report it
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context, "Stopping person searching failed", message);
    }
  }

  /// Define the handler for adding a new search request
  void _addButtonClicked(BuildContext context) async {
    //show the modal with actions
    await Modal.showBottomWithActionsDialogAsync(context, actions: [
      //create the first option (upload the image from gallery)
      ListTile(
          visualDensity: VisualDensity.compact,
          leading: Icon(Icons.photo_library_outlined, color: Colors.white70),
          title: Text('Upload person of interest from gallery',
              style: TextStyle(
                  fontWeight: FontWeight.w300,
                  fontSize: 15,
                  color: Colors.white70)),
          onTap: () => _pickAndProcessTheImage(context, ImageSource.gallery)),

      //upload the person using camera
      ListTile(
          visualDensity: VisualDensity.compact,
          leading: Icon(Icons.photo_camera_outlined, color: Colors.white70),
          title: Text('Upload person of interest from camera',
              style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w300,
                  color: Colors.white70)),
          onTap: () => _pickAndProcessTheImage(context, ImageSource.camera))
    ]);
  }

  /// This method it is used for image picking. Based on the [imageSource] the image will
  /// be loaded accordingly and sent to server for starting a new search request
  /// The ui will be updated
  void _pickAndProcessTheImage(
      BuildContext context, ImageSource imageSource) async {
    //try to pick an image
    try {
      //get the image from gallery
      final PickedFile imagePath =
          await imagePicker.getImage(source: imageSource, imageQuality: 100);

      //if the image is null then do nothing
      if (imagePath == null) {
        return;
      }

      //await searching for a new person
      await FacialRecognitionService()
          .startSearchingForANewPersonAsync(widget.userToken, imagePath.path);

      //go back in activity stack
      Navigator.of(context).pop();

      //ping all the workers for refreshing the displayed data
      await FacialRecognitionService().pingAllWorkersAsync(widget.userToken);

      //notify the exceptions changes
    } on Exception catch (e) {
      //get message
      final message = Modal.extractMessageFromException(e);
      //get the message and report it
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context, "Image upload failed", message);
    }
  }

//#endregion
}
