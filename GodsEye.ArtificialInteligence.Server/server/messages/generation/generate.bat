
echo off

"C:\Users\Eduard\AppData\Local\Programs\Python\Python39\python.exe" -m grpc_tools.protoc --python_out=. --proto_path=. --grpc_python_out=. server.messages.grpc.server_messages.proto

