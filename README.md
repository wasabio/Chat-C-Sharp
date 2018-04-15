# Chat-C-Sharp
## Introduction


A chat application written in c#. This application is supposed to work using a Client side, and a Server side, connected to each other by
a socket. Multi-threading was used for this chat, and users have the possibility to create different chat rooms.

## Characteristics
In our project, we realized all the asked functionalities (By the school, Efrei) and way more. We divided the application into two different parts : client side and server side. You can take a look at some of the major characteristics :

* **Authentication system**. The user has to log in or to register, if he's new, to be able to use the application. We handle data persistence by using SQLite which is a kind of portable database, which works without SQL server.

* **Sockets**. They allow communication between client(s) and server thanks to TCP-IP protocol.

* **Multi-thread**. The client side is using 2 different threads : One to communicate with the server, One for the GUI. The server side is working with multiple threads : One is used to handle the incoming connections, and another one is dedicated to broadcast messages : from a client to a room of clients, for instance. Furthermore, each client has his own single thread dedicated to himself on the server.

* **Multi-rooms**. A user can chat in many rooms, and create rooms with the name he wants. This multi-room system is great because the other rooms are still updated with the messages, even if the user is on an other room. Users can see the room list and choose which one they want to interact with, if they're not interested in using all rooms. The history of the messages is stored in the RAM, and a user can retrive old conversations.

* **Winforms**. For the GUI



## Software design

The chat server has 5 main classes:
*	Server class
*	Session class
*	Message class
*	Room class
*	Auth class


Please, check the report in "Documentation" folder to get more details about the project !

## Screenshots 

![alt text](https://github.com/wasabio/Chat-C-Sharp/blob/master/Documentation/Screenshots/Chat.png)
![alt text](https://github.com/wasabio/Chat-C-Sharp/blob/master/Documentation/Screenshots/Login.png)
![alt text](https://github.com/wasabio/Chat-C-Sharp/blob/master/Documentation/Screenshots/Server.png)
![alt text](https://github.com/wasabio/Chat-C-Sharp/blob/master/Documentation/Screenshots/Uml.png)

