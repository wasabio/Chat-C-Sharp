# Chat-C-Sharp
## Introduction


A chat application written in c#. This application is supposed to work using a Client side, and a Server side, connected to each other by
a socket. Multi-threading was used for this chat, and users have the possibility to create different chat rooms.

## A.	Features
In our project, we realized all the asked functionalities and way more. We divided the application in two different parts, as it was suggested in the subject: client side and server side. You can take a look at the different features we implemented:  
Firstly, we managed to make an authentication system. The user has to log in or to register, if he's new, to be able to use the application. We handle data persistence by using SQLite which is a kind of portable database, which works without SQL server.
Secondly, our application uses sockets, which communicates with each other thanks to TCP-IP protocol.
Moreover, the software is multi-thread. The client side is using 2 different threads that allows to handle the communication with the server, without freezing the interface. The server side is working with multiple threads : there is one thread that handle the incoming connections, and one thread that is dedicated to broadcast messages : from a client to a room of clients, for instance. Furthermore, each client has his own thread, only dedicated to himself on the server. 
We also developed an important feature for the rooms. A user can chat in multiple rooms, and create rooms with the name he wants. Our multi-room system is great because the other rooms are still updated with the messages, even if the user is on an other room. Users can see the room list and choose which one they want to interact with, if they're not interested in using all rooms. 
In the multi-room system, we keep track of the conversation by saving all the messages in the RAM, and when we have to recover it, we just load it where it is needed. For example, when the user select an other room, the conversation is stored in RAM, and the conversation of the main room is loaded in the interface. 
Finally, we had enough time to create a GUI, which is way better for any user. 
