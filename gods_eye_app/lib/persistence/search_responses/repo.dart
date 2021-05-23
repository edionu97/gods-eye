import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/utils/helpers/objects/observer/object.dart';
import 'package:mutex/mutex.dart';

class PersonSearchResponseRepository extends FunctionalObservable {
  //implement the singleton pattern
  static PersonSearchResponseRepository _singletonInstance =
      PersonSearchResponseRepository._internal();

  //create the sync primitive
  final Mutex _syncPrimitive = Mutex();

  //declare a list of
  final List<PersonFoundMessageModel> _foundPersons = [];

  //declare a private constructor
  PersonSearchResponseRepository._internal();

  //implement the factory method
  factory PersonSearchResponseRepository() {
    return _singletonInstance;
  }

  /// This method it is used for adding a new instance into the list
  /// The instance is the [itemToBeAdded]
  Future addItemAsync(final PersonFoundMessageModel itemToBeAdded) async {
    //critical section (add data into the list)
    await _syncPrimitive.protect(() async => _foundPersons.add(itemToBeAdded));

    //notify all observers that a new item was added
    notifyAllObservers(itemToBeAdded);
  }

  /// This method it is used for getting all the items async
  Future<List<PersonFoundMessageModel>> allItemsAsync() async {
    //critical section (lock the list when get all)
    return await _syncPrimitive.protect(() async => _foundPersons);
  }

  //clear the repository
  void clearRepository() {
    _foundPersons.clear();
  }
}
