import 'dart:convert';
import 'dart:io';
import 'dart:ui';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:folder_file_saver/folder_file_saver.dart';
import 'package:gods_eye_app/services/drawing/service.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/services/models/person_found/search_result/model.dart';
import 'package:gods_eye_app/utils/components/modal/component.dart';
import 'package:gods_eye_app/utils/components/rounded_card/component.dart';
import 'package:ext_storage/ext_storage.dart';
import 'package:path/path.dart' as p;

class FaceMatchDetails extends StatefulWidget {
  final Object heroKey;
  final String userToken;
  final PersonFoundMessageModel foundPersonDetails;

  const FaceMatchDetails(
      {Key key,
      @required this.foundPersonDetails,
      this.userToken,
      this.heroKey})
      : super(key: key);

  @override
  _FaceMatchDetailsState createState() => _FaceMatchDetailsState();
}

class _FaceMatchDetailsState extends State<FaceMatchDetails>
    with SingleTickerProviderStateMixin {
  Image _displayedImage;
  int _currentSelectedItemIdx = 3;

  //define the animation controller
  AnimationController _animationController;
  Animation<double> _animation;

  //this is true only when nothing is displayed
  bool _isNothingDisplayed = false;

  final Map<int, Image> _cachedImage = {};

  @override
  void initState() {
    //execute the super logic
    super.initState();

    //create the animation controller
    _animationController = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 400));

    //create the opacity controller
    _animation = Tween<double>(begin: 0, end: 1).animate(_animationController);

    //call the set image method
    _setImageAsync(_currentSelectedItemIdx);

    _animationController.addListener(_checkIfSomethingIsDisplayed);
  }

  @override
  void dispose() {
    _animationController.removeListener(_checkIfSomethingIsDisplayed);
    //dispose the animation controller
    _animationController.dispose();
    //execute the base dispose logic
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    //create get the image value
    return Scaffold(
        bottomNavigationBar: _createNavigationBar(context),
        //in the body place all the children
        body: Column(children: [
          //create the image part
          _createImagePart(),
          //create the details part
          _createDetailsPart()
        ]));
  }

  /// Create the image part with all the logic
  Widget _createImagePart() {
    //set the border radius
    final borderRadius = BorderRadius.all(Radius.circular(30));

    //create the hero
    return Hero(
        tag: widget.heroKey,
        //add a padding
        child: Padding(
            padding: const EdgeInsets.all(8.0),
            //wrap everything in a material widget
            child: Material(
                //establish the shadow color
                shadowColor: Colors.blueGrey[700],
                //establish the color
                color: Colors.grey[50],
                //create the border
                shape: RoundedRectangleBorder(borderRadius: borderRadius),
                //set the elevation
                elevation: 5,
                //add the padding
                child: Padding(
                    padding: const EdgeInsets.all(.2),
                    //create container
                    child: Container(
                        height: 400,
                        color: Colors.transparent,
                        width: MediaQuery.of(context).size.width,
                        child: _createImage(borderRadius, context))))));
  }

  Widget _createImage(
      final BorderRadius borderRadius, final BuildContext context) {
    //if the image is null or there is nothing to display
    return _displayedImage == null || _isNothingDisplayed
        //display the progress bar
        ? Center(
            child: CircularProgressIndicator(
            backgroundColor: Colors.transparent,
            valueColor: AlwaysStoppedAnimation<Color>(Colors.blueGrey[600]),
          ))
        //otherwise display the image clipped
        : ClipRRect(
            borderRadius: borderRadius,
            //put a new transition
            child: FadeTransition(
                opacity: _animation,
                //wrap in a gesture detector
                child: GestureDetector(
                    //download the image
                    onLongPress: () =>
                        _saveImageToDownloadsFolderAsync(context),
                    //display the image
                    child: _displayedImage)));
  }

  Widget _createDetailsPart() {
    var locationInfo = "unknown";
    final geo = widget.foundPersonDetails?.geoLocation;
    if (geo != null) {
      locationInfo = "${geo.countryName} (${geo.countryCode}), "
          "${geo.regionName} (${geo.regionCode}), "
          "${geo.city}, ${geo.zipCode}";
    }

    var latitudeInfo = "unknown";
    if (geo != null) {
      latitudeInfo = geo.latitude?.toString() ?? latitudeInfo;
    }

    var longitude = "unknown";
    if (geo != null) {
      longitude = geo.longitude?.toString() ?? longitude;
    }

    //get the attribute analysis
    var attributeAnalysis =
        widget.foundPersonDetails?.searchResult?.attributeAnalysis;

    var age = "unknown";
    if (attributeAnalysis != null) {
      age = attributeAnalysis.age?.toString() ?? age;
    }

    var race = "unknown";
    if (attributeAnalysis != null) {
      race = attributeAnalysis.race?.toString() ?? race;
    }

    var emotion = "unknown";
    if (attributeAnalysis != null) {
      emotion = attributeAnalysis.emotion?.toString() ?? emotion;
    }

    var gender = "unknown";
    if (attributeAnalysis != null) {
      gender = attributeAnalysis.gender?.toString() ?? gender;
      gender = gender.toLowerCase();
    }

    //put in an expanded to use all the available space
    return Expanded(
      //create a column
      child: Center(
        //put the items in a row
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: [
            //create the first card for the geolocation
            _createRoundedCard(
                width: 160,
                height: 180,
                cardTitle: "Geolocation",
                cardSubtitle: "Person location info",
                children: [
                  //create card row (location)
                  _createCardRow(
                      image: ImageIcon(AssetImage("assets/location.png"),
                          color: Colors.blueGrey[600], size: 25),
                      padding: const EdgeInsets.only(
                          left: 8.0, top: 8.0, right: 8.0),
                      displayedValue: locationInfo),
                  //latitude
                  _createCardRow(
                      image: ImageIcon(AssetImage("assets/latitude.png"),
                          color: Colors.blueGrey[300], size: 25),
                      padding: const EdgeInsets.only(
                          left: 8.0, top: 8.0, right: 8.0),
                      displayedValue:
                          "The value for latitude is $latitudeInfo degrees"),
                  //longitude
                  _createCardRow(
                      image: ImageIcon(AssetImage("assets/longitude.png"),
                          color: Colors.blueGrey[400], size: 25),
                      padding: const EdgeInsets.only(
                          left: 8.0, top: 8.0, right: 8.0),
                      displayedValue:
                          "The value for longitude is $longitude degrees")
                ]),
            _createRoundedCard(
                cardTitle: "Facial recognition",
                cardSubtitle: "Facial attribute analysis",
                width: 160,
                height: 180,
                children: [
                  _createCardRow(
                      image: ImageIcon(AssetImage("assets/age.png"),
                          color: Colors.blueGrey[500], size: 25),
                      padding: const EdgeInsets.only(left: 10, top: 8.0),
                      displayedValue: "Predicted age is $age"),
                  _createCardRow(
                      image: Icon(Icons.person_outlined,
                          color: Colors.blueGrey[200], size: 24),
                      displayedValue: "Predicted race is $race"),
                  _createCardRow(
                      image: ImageIcon(AssetImage("assets/emotion.png"),
                          color: Colors.blueGrey[500], size: 25),
                      displayedValue: "Predicted emotion is $emotion"),
                  _createCardRow(
                      image: ImageIcon(AssetImage("assets/sex.png"),
                          color: Colors.blueGrey[300], size: 24),
                      displayedValue: "Predicted gender is $gender")
                ]),
            //
          ],
        ),
      ),
    );
  }

  Widget _createRoundedCard(
      {@required final List<Widget> children,
      final String cardTitle,
      final String cardSubtitle,
      final double height = 150,
      final double width = 150}) {
    //create the text style
    final TextStyle textStyleHigh = TextStyle(
        fontWeight: FontWeight.w400,
        fontStyle: FontStyle.normal,
        fontSize: 11,
        color: Colors.white);

    //create the text style
    final TextStyle textStyleMid = TextStyle(
        fontWeight: FontWeight.w400,
        fontStyle: FontStyle.normal,
        fontSize: 10,
        color: Colors.white);

    //define the box decoration
    final BoxDecoration decoration = BoxDecoration(
        color: Colors.transparent,
        borderRadius: const BorderRadius.only(
            bottomLeft: Radius.circular(30), bottomRight: Radius.circular(30)));

    //create the first card for the geolocation
    return RoundedCorneredCard(
      titleBackgroundColor: Colors.blueGrey[500],
      //set the elevation
      elevation: 4,
      //specify the height
      height: height,
      //specify the width
      width: width,
      //put the widgets that are defining the card title
      cardTitleChildren: [
        //put the title of the card
        Padding(
            padding: const EdgeInsets.only(top: 5),
            child: Text(cardTitle ?? "", style: textStyleHigh)),
        //put the other information
        Padding(
            padding: const EdgeInsets.all(2),
            child: Text(cardSubtitle ?? "", style: textStyleMid))
      ],
      children: [
        //ensure we distribute the space evenly
        Expanded(
            //create a container
            child: Container(
                //put the decoration
                decoration: decoration,
                child: Column(children: children)))
      ],
    );
  }

  Widget _createCardRow(
      {@required final Widget image,
      @required final String displayedValue,
      final EdgeInsetsGeometry padding}) {
    //set the text style
    final TextStyle labelTextStyle = TextStyle(
        fontWeight: FontWeight.w400,
        fontStyle: FontStyle.normal,
        fontSize: 12,
        color: Colors.blueGrey[300]);

    //create padding and the row elements
    return Padding(
        padding: padding ?? const EdgeInsets.only(left: 8.0, top: 8.0),
        //each icon should be placed in a row
        child: Row(children: [
          image,
          Expanded(
              child: Padding(
                  padding: const EdgeInsets.only(left: 3),
                  child: Text(displayedValue,
                      overflow: TextOverflow.ellipsis,
                      style: labelTextStyle,
                      maxLines: 3)))
        ]));
  }

  ///This function it is used for creating the navbar
  Widget _createNavigationBar(BuildContext context) {
    //wrap in container
    return Container(
        //set the margin of the nav bar
        margin: EdgeInsets.only(left: 1, right: 1),
        //set it's border radius
        decoration: BoxDecoration(
            borderRadius: BorderRadius.only(
                topLeft: Radius.circular(80.0),
                topRight: Radius.circular(80.0)),
            color: Colors.white,
            //create the box shadow
            boxShadow: [
              BoxShadow(
                color: Colors.blueGrey[700],
                blurRadius: .5,
                spreadRadius: 2.0,
                offset: Offset(-1.0, -1.0), // shadow direction: bottom right
              )
            ]),
        //create the nav bar
        child: ClipRRect(
            //set the radius
            borderRadius: BorderRadius.only(
                topLeft: Radius.circular(80.0),
                topRight: Radius.circular(80.0)),
            //define the navbar structure
            child: BottomNavigationBar(
                type: BottomNavigationBarType.fixed,
                currentIndex: _currentSelectedItemIdx,
                //items from navigation bar
                items: [
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon: ImageIcon(AssetImage("assets/search_results.png"),
                          size: 27),
                      label: 'Go back'),
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon: ImageIcon(AssetImage("assets/face-recognition.png"),
                          size: 27),
                      label: 'Facial points'),
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon: ImageIcon(
                          AssetImage("assets/face-detection_on.png"),
                          size: 27),
                      label: 'Reveal person'),
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon: Icon(Icons.photo_library_outlined, size: 28),
                      label: 'Show original'),
                ],
                selectedItemColor: Colors.white,
                elevation: 10.0,
                unselectedItemColor: Colors.white70,
                backgroundColor: Colors.blueGrey[700],
                onTap: (i) => _onMenuButtonClicked(i, context))));
  }

  /// This callback will be used to check if is something displayed on  the screen
  void _checkIfSomethingIsDisplayed() {
    setState(() {
      _isNothingDisplayed = _animation.value <= .3;
    });
  }

  /// This method it is called when the user clicks on one item
  /// from the bottom menu
  void _onMenuButtonClicked(int index, BuildContext context) {
    //do not take any action because we on the required screen
    if (_currentSelectedItemIdx == index) {
      return;
    }

    //reflect the selection on the ui
    setState(() {
      _currentSelectedItemIdx = index;
    });

    //navigate back if index == 0
    if (index == 0) {
      Navigator.of(context).pop();
      return;
    }

    //set the image async
    _setImageAsync(index);
  }

  /// This function it is used for setting the image based on the user's navigation
  /// the image will be set accordingly with the value of the [index]
  void _setImageAsync(int index) async {
    //await reverse
    await _animationController.reverse();

    //if the image is not cached
    if (!_cachedImage.containsKey(index)) {
      //cache it
      _cachedImage[index] = await _alterImageUsingServiceAsync(index);
    }

    //set the state after the response
    setState(() {
      _displayedImage = _cachedImage[index];
    });

    //forward the controller
    _animationController.forward();
  }

  /// This method it is used for altering the image
  /// it will modify the image (resize or draw over it)
  Future<Image> _alterImageUsingServiceAsync(int index) async {
    //get the search result model
    final SearchResultModel searchResultModel =
        widget.foundPersonDetails?.searchResult;

    //define the constants
    const int imageWidth = 400;
    const int imageHeight = 400;

    try {
      //get the image string
      final String base64Img = searchResultModel?.foundImageString;

      //switch based on index
      switch (index) {
        //these two cases are the same(the same server call is used)
        case 1:
        case 2:
          {
            //put bnd box over the image and resize it
            return await DrawingService().drawFaceBoundingBoxAsync(
                userToken: widget.userToken,
                base64Image: base64Img,
                width: imageWidth,
                resize: true,
                height: imageHeight,
                facialKeyPointsModel:
                    index == 1 ? searchResultModel?.faceKeyPoints : null,
                bndBox: searchResultModel?.boundingBox);
          }
        //treat the show original selection
        case 3:
          {
            //draw the original image resized
            return await DrawingService().resizeImageAsync(
              userToken: widget.userToken,
              base64Image: base64Img,
              width: imageWidth,
              height: imageHeight,
            );
          }
      }

      //on exception return the found image
    } on Exception catch (e) {
      //get message
      final message = Modal.extractMessageFromException(e);

      //get the message and report it
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context, "Drawing failed", message);

      //return the image from the model
      return searchResultModel.foundImage;
    }

    //otherwise return null
    return null;
  }

  /// This method will be called when top image is long pressed
  void _saveImageToDownloadsFolderAsync(final BuildContext context) async {
    //ensure that we have an image displayed
    if (_displayedImage == null) {
      return;
    }

    //try to execute the saving of the file
    try {
      //if the app does not have the required permissions do nothing
      if (!(await _hasWriteStoragePermissionsAsync(context))) {
        return;
      }

      //create the image file
      var imageFile = await _createImageFileAsync(widget.foundPersonDetails);

      //get the found image string
      var foundImageBase64 = widget
          ?.foundPersonDetails?.searchResult?.foundImageString
          ?.split(',')
          ?.last;

      //treat the message
      if (foundImageBase64 == null || foundImageBase64.isEmpty) {
        throw Exception(
            "The image may be broken, since the value for it is null");
      }

      //encode the image and save it into the file
      await imageFile.writeAsBytes(base64Decode(foundImageBase64));

      //display the success message
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context,
          "Success",
          "Image saved successfully."
              " The file can be found into the "
              "downloads/gods_eye/${widget.foundPersonDetails?.responseId} folder");

      //treat any exception that occurs
    } on Exception catch (e) {
      //get the exception message
      final String message = Modal.extractMessageFromException(e);
      //display the message
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context, "Error encountered", message);
    }
  }

  /// This function ensures that the application has the storage permissions enabled
  /// Otherwise it will open the settings and a dialog will be displayed on [context]
  static Future<bool> _hasWriteStoragePermissionsAsync(
      final BuildContext context) async {
    // if return 0 permission is PERMISSION_GRANTED
    // if return 1 permission is PERMISSION_IS_DENIED
    // if return 2 permission is PERMISSION_IS_DENIED with click don't ask again
    final hasRequiredPermissions = await FolderFileSaver.requestPermission();

    //check if result
    if (hasRequiredPermissions != 0) {
      //display the modal
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context,
          "Storage permission required",
          "The file could not "
              "be saved because the app does not have storage permission enabled."
              "Open settings and enable the permission, and after that try again");

      //get the settings
      await FolderFileSaver.openSetting;
    }
    //return the value of the permissions
    return await FolderFileSaver.requestPermission() == 0;
  }

  /// This function it is used for creating the file in which the image will be saved
  /// The image is obtained by looking at the value of [foundPersonDetails].searchResult.foundImageString
  static Future<File> _createImageFileAsync(
      final PersonFoundMessageModel foundPersonDetails) async {
    //get the regex
    final regex = RegExp(r"^data:image/(?<imageType>.*);base64$");

    //get the image streaming format
    final imageStreamingType = foundPersonDetails?.imageFormat;

    //check if the
    if (!regex.hasMatch(imageStreamingType)) {
      throw Exception("The image format is unknown");
    }

    //get the with that name
    var imageType =
        regex.firstMatch(imageStreamingType).namedGroup("imageType");

    //get the documents directory
    final documentsPath = await ExtStorage.getExternalStoragePublicDirectory(
        ExtStorage.DIRECTORY_DOWNLOADS);

    //get the file path
    final filePath = p.join(
        documentsPath,
        "gods_eye",
        foundPersonDetails?.responseId ?? "",
        "${DateTime.now().ticks}.$imageType");

    //create the file and all the directories to it
    return await File(filePath).create(recursive: true);
  }
}

/// This extension method it is used for computing the ticks
const _epochTicks = 621355968000000000;
extension TicksOnDateTime on DateTime {
  int get ticks => this.microsecondsSinceEpoch * 10 + _epochTicks;
}
