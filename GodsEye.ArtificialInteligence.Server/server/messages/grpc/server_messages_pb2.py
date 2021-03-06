# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: server.messages.grpc.server_messages.proto
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='server.messages.grpc.server_messages.proto',
  package='gods.eye.server.artificial.intelligence.messaging',
  syntax='proto3',
  serialized_options=None,
  create_key=_descriptor._internal_create_key,
  serialized_pb=b'\n*server.messages.grpc.server_messages.proto\x12\x31gods.eye.server.artificial.intelligence.messaging\"\xd7\x01\n\x1e\x46\x61\x63ialAttributeAnalysisRequest\x12\x65\n\x11\x66\x61\x63\x65_bounding_box\x18\x01 \x01(\x0b\x32J.gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox\x12.\n&analyzed_image_containing_the_face_b64\x18\x02 \x01(\t\x12\x1e\n\x16is_face_location_known\x18\x03 \x01(\x08\"]\n\x1f\x46\x61\x63ialAttributeAnalysisResponse\x12\x0f\n\x07\x65motion\x18\x01 \x01(\t\x12\x0c\n\x04race\x18\x02 \x01(\t\x12\x0b\n\x03\x61ge\x18\x03 \x01(\x05\x12\x0e\n\x06gender\x18\x04 \x01(\t\"y\n\x16SearchForPersonRequest\x12\x18\n\x10person_image_b64\x18\x01 \x01(\t\x12\x1a\n\x12location_image_b64\x18\x02 \x01(\t\x12)\n!include_cropped_faces_in_response\x18\x03 \x01(\x08\"\x99\x01\n\x17SearchForPersonResponse\x12\x10\n\x08is_found\x18\x01 \x01(\x08\x12l\n\x15\x66\x61\x63\x65_recognition_info\x18\x02 \x03(\x0b\x32M.gods.eye.server.artificial.intelligence.messaging.FaceRecognitionInformation\"\x82\x02\n\x1a\x46\x61\x63\x65RecognitionInformation\x12\x65\n\x11\x66\x61\x63\x65_bounding_box\x18\x01 \x01(\x0b\x32J.gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox\x12\x1e\n\x16\x63ropped_face_image_b64\x18\x02 \x01(\t\x12]\n\x0b\x66\x61\x63\x65_points\x18\x03 \x01(\x0b\x32H.gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation\"[\n\x17\x46\x61\x63\x65LocationBoundingBox\x12\r\n\x05top_x\x18\x01 \x01(\x05\x12\r\n\x05top_y\x18\x02 \x01(\x05\x12\x10\n\x08\x62ottom_x\x18\x03 \x01(\x05\x12\x10\n\x08\x62ottom_y\x18\x04 \x01(\x05\"\xc7\x03\n\x15\x46\x61\x63\x65KeypointsLocation\x12U\n\x0fright_eye_point\x18\x01 \x01(\x0b\x32<.gods.eye.server.artificial.intelligence.messaging.FacePoint\x12T\n\x0eleft_eye_point\x18\x02 \x01(\x0b\x32<.gods.eye.server.artificial.intelligence.messaging.FacePoint\x12P\n\nnose_point\x18\x03 \x01(\x0b\x32<.gods.eye.server.artificial.intelligence.messaging.FacePoint\x12V\n\x10mouth_left_point\x18\x04 \x01(\x0b\x32<.gods.eye.server.artificial.intelligence.messaging.FacePoint\x12W\n\x11mouth_right_point\x18\x05 \x01(\x0b\x32<.gods.eye.server.artificial.intelligence.messaging.FacePoint\"!\n\tFacePoint\x12\t\n\x01x\x18\x01 \x01(\x05\x12\t\n\x01y\x18\x02 \x01(\x05\x32\x96\x03\n\x1c\x46\x61\x63ialRecognitionAndAnalysis\x12\xae\x01\n\x13\x44oFacialRecognition\x12I.gods.eye.server.artificial.intelligence.messaging.SearchForPersonRequest\x1aJ.gods.eye.server.artificial.intelligence.messaging.SearchForPersonResponse\"\x00\x12\xc4\x01\n\x19\x44oFacialAttributeAnalysis\x12Q.gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisRequest\x1aR.gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisResponse\"\x00\x62\x06proto3'
)




_FACIALATTRIBUTEANALYSISREQUEST = _descriptor.Descriptor(
  name='FacialAttributeAnalysisRequest',
  full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='face_bounding_box', full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisRequest.face_bounding_box', index=0,
      number=1, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='analyzed_image_containing_the_face_b64', full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisRequest.analyzed_image_containing_the_face_b64', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='is_face_location_known', full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisRequest.is_face_location_known', index=2,
      number=3, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=98,
  serialized_end=313,
)


_FACIALATTRIBUTEANALYSISRESPONSE = _descriptor.Descriptor(
  name='FacialAttributeAnalysisResponse',
  full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisResponse',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='emotion', full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisResponse.emotion', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='race', full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisResponse.race', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='age', full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisResponse.age', index=2,
      number=3, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='gender', full_name='gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisResponse.gender', index=3,
      number=4, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=315,
  serialized_end=408,
)


_SEARCHFORPERSONREQUEST = _descriptor.Descriptor(
  name='SearchForPersonRequest',
  full_name='gods.eye.server.artificial.intelligence.messaging.SearchForPersonRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='person_image_b64', full_name='gods.eye.server.artificial.intelligence.messaging.SearchForPersonRequest.person_image_b64', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='location_image_b64', full_name='gods.eye.server.artificial.intelligence.messaging.SearchForPersonRequest.location_image_b64', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='include_cropped_faces_in_response', full_name='gods.eye.server.artificial.intelligence.messaging.SearchForPersonRequest.include_cropped_faces_in_response', index=2,
      number=3, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=410,
  serialized_end=531,
)


_SEARCHFORPERSONRESPONSE = _descriptor.Descriptor(
  name='SearchForPersonResponse',
  full_name='gods.eye.server.artificial.intelligence.messaging.SearchForPersonResponse',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='is_found', full_name='gods.eye.server.artificial.intelligence.messaging.SearchForPersonResponse.is_found', index=0,
      number=1, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='face_recognition_info', full_name='gods.eye.server.artificial.intelligence.messaging.SearchForPersonResponse.face_recognition_info', index=1,
      number=2, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=534,
  serialized_end=687,
)


_FACERECOGNITIONINFORMATION = _descriptor.Descriptor(
  name='FaceRecognitionInformation',
  full_name='gods.eye.server.artificial.intelligence.messaging.FaceRecognitionInformation',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='face_bounding_box', full_name='gods.eye.server.artificial.intelligence.messaging.FaceRecognitionInformation.face_bounding_box', index=0,
      number=1, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='cropped_face_image_b64', full_name='gods.eye.server.artificial.intelligence.messaging.FaceRecognitionInformation.cropped_face_image_b64', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='face_points', full_name='gods.eye.server.artificial.intelligence.messaging.FaceRecognitionInformation.face_points', index=2,
      number=3, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=690,
  serialized_end=948,
)


_FACELOCATIONBOUNDINGBOX = _descriptor.Descriptor(
  name='FaceLocationBoundingBox',
  full_name='gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='top_x', full_name='gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox.top_x', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='top_y', full_name='gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox.top_y', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='bottom_x', full_name='gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox.bottom_x', index=2,
      number=3, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='bottom_y', full_name='gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox.bottom_y', index=3,
      number=4, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=950,
  serialized_end=1041,
)


_FACEKEYPOINTSLOCATION = _descriptor.Descriptor(
  name='FaceKeypointsLocation',
  full_name='gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='right_eye_point', full_name='gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation.right_eye_point', index=0,
      number=1, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='left_eye_point', full_name='gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation.left_eye_point', index=1,
      number=2, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='nose_point', full_name='gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation.nose_point', index=2,
      number=3, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='mouth_left_point', full_name='gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation.mouth_left_point', index=3,
      number=4, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='mouth_right_point', full_name='gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation.mouth_right_point', index=4,
      number=5, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=1044,
  serialized_end=1499,
)


_FACEPOINT = _descriptor.Descriptor(
  name='FacePoint',
  full_name='gods.eye.server.artificial.intelligence.messaging.FacePoint',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='x', full_name='gods.eye.server.artificial.intelligence.messaging.FacePoint.x', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='y', full_name='gods.eye.server.artificial.intelligence.messaging.FacePoint.y', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=1501,
  serialized_end=1534,
)

_FACIALATTRIBUTEANALYSISREQUEST.fields_by_name['face_bounding_box'].message_type = _FACELOCATIONBOUNDINGBOX
_SEARCHFORPERSONRESPONSE.fields_by_name['face_recognition_info'].message_type = _FACERECOGNITIONINFORMATION
_FACERECOGNITIONINFORMATION.fields_by_name['face_bounding_box'].message_type = _FACELOCATIONBOUNDINGBOX
_FACERECOGNITIONINFORMATION.fields_by_name['face_points'].message_type = _FACEKEYPOINTSLOCATION
_FACEKEYPOINTSLOCATION.fields_by_name['right_eye_point'].message_type = _FACEPOINT
_FACEKEYPOINTSLOCATION.fields_by_name['left_eye_point'].message_type = _FACEPOINT
_FACEKEYPOINTSLOCATION.fields_by_name['nose_point'].message_type = _FACEPOINT
_FACEKEYPOINTSLOCATION.fields_by_name['mouth_left_point'].message_type = _FACEPOINT
_FACEKEYPOINTSLOCATION.fields_by_name['mouth_right_point'].message_type = _FACEPOINT
DESCRIPTOR.message_types_by_name['FacialAttributeAnalysisRequest'] = _FACIALATTRIBUTEANALYSISREQUEST
DESCRIPTOR.message_types_by_name['FacialAttributeAnalysisResponse'] = _FACIALATTRIBUTEANALYSISRESPONSE
DESCRIPTOR.message_types_by_name['SearchForPersonRequest'] = _SEARCHFORPERSONREQUEST
DESCRIPTOR.message_types_by_name['SearchForPersonResponse'] = _SEARCHFORPERSONRESPONSE
DESCRIPTOR.message_types_by_name['FaceRecognitionInformation'] = _FACERECOGNITIONINFORMATION
DESCRIPTOR.message_types_by_name['FaceLocationBoundingBox'] = _FACELOCATIONBOUNDINGBOX
DESCRIPTOR.message_types_by_name['FaceKeypointsLocation'] = _FACEKEYPOINTSLOCATION
DESCRIPTOR.message_types_by_name['FacePoint'] = _FACEPOINT
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

FacialAttributeAnalysisRequest = _reflection.GeneratedProtocolMessageType('FacialAttributeAnalysisRequest', (_message.Message,), {
  'DESCRIPTOR' : _FACIALATTRIBUTEANALYSISREQUEST,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisRequest)
  })
_sym_db.RegisterMessage(FacialAttributeAnalysisRequest)

FacialAttributeAnalysisResponse = _reflection.GeneratedProtocolMessageType('FacialAttributeAnalysisResponse', (_message.Message,), {
  'DESCRIPTOR' : _FACIALATTRIBUTEANALYSISRESPONSE,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.FacialAttributeAnalysisResponse)
  })
_sym_db.RegisterMessage(FacialAttributeAnalysisResponse)

SearchForPersonRequest = _reflection.GeneratedProtocolMessageType('SearchForPersonRequest', (_message.Message,), {
  'DESCRIPTOR' : _SEARCHFORPERSONREQUEST,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.SearchForPersonRequest)
  })
_sym_db.RegisterMessage(SearchForPersonRequest)

SearchForPersonResponse = _reflection.GeneratedProtocolMessageType('SearchForPersonResponse', (_message.Message,), {
  'DESCRIPTOR' : _SEARCHFORPERSONRESPONSE,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.SearchForPersonResponse)
  })
_sym_db.RegisterMessage(SearchForPersonResponse)

FaceRecognitionInformation = _reflection.GeneratedProtocolMessageType('FaceRecognitionInformation', (_message.Message,), {
  'DESCRIPTOR' : _FACERECOGNITIONINFORMATION,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.FaceRecognitionInformation)
  })
_sym_db.RegisterMessage(FaceRecognitionInformation)

FaceLocationBoundingBox = _reflection.GeneratedProtocolMessageType('FaceLocationBoundingBox', (_message.Message,), {
  'DESCRIPTOR' : _FACELOCATIONBOUNDINGBOX,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.FaceLocationBoundingBox)
  })
_sym_db.RegisterMessage(FaceLocationBoundingBox)

FaceKeypointsLocation = _reflection.GeneratedProtocolMessageType('FaceKeypointsLocation', (_message.Message,), {
  'DESCRIPTOR' : _FACEKEYPOINTSLOCATION,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.FaceKeypointsLocation)
  })
_sym_db.RegisterMessage(FaceKeypointsLocation)

FacePoint = _reflection.GeneratedProtocolMessageType('FacePoint', (_message.Message,), {
  'DESCRIPTOR' : _FACEPOINT,
  '__module__' : 'server.messages.grpc.server_messages_pb2'
  # @@protoc_insertion_point(class_scope:gods.eye.server.artificial.intelligence.messaging.FacePoint)
  })
_sym_db.RegisterMessage(FacePoint)



_FACIALRECOGNITIONANDANALYSIS = _descriptor.ServiceDescriptor(
  name='FacialRecognitionAndAnalysis',
  full_name='gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis',
  file=DESCRIPTOR,
  index=0,
  serialized_options=None,
  create_key=_descriptor._internal_create_key,
  serialized_start=1537,
  serialized_end=1943,
  methods=[
  _descriptor.MethodDescriptor(
    name='DoFacialRecognition',
    full_name='gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis.DoFacialRecognition',
    index=0,
    containing_service=None,
    input_type=_SEARCHFORPERSONREQUEST,
    output_type=_SEARCHFORPERSONRESPONSE,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
  _descriptor.MethodDescriptor(
    name='DoFacialAttributeAnalysis',
    full_name='gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis.DoFacialAttributeAnalysis',
    index=1,
    containing_service=None,
    input_type=_FACIALATTRIBUTEANALYSISREQUEST,
    output_type=_FACIALATTRIBUTEANALYSISRESPONSE,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
])
_sym_db.RegisterServiceDescriptor(_FACIALRECOGNITIONANDANALYSIS)

DESCRIPTOR.services_by_name['FacialRecognitionAndAnalysis'] = _FACIALRECOGNITIONANDANALYSIS

# @@protoc_insertion_point(module_scope)
