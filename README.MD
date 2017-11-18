Orleans Messenger
====================================

An example of using Orleans for a simple messaging application

### Uses the following Orleans features
* Streams (to send messages to clients)
* Journalled Grains (event sourcing of conversation history)


Run server

```
src/OrleansMessenger $ dotnet run
```

Run one or more clients
```
src/OrleansMessenger.Client $ dotnet run
```

To send a message "hello" to bill:
```
s:bill:hello
``` 

