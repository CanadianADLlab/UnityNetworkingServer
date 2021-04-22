# UnityNetworkingServer
The unity networking server project for the unity networking example, based off the tutorial from https://www.youtube.com/watch?v=uh8XaC0Y5MA&amp;t=4s

<h1>Requirements</h1>
This is meant to be used with the Unity project https://github.com/CanadianADLlab/UnityNetworkingClient but can be modified to work with anything.
If you don't already have the .NET SDK installed make sure you do that https://docs.microsoft.com/en-us/dotnet/core/sdk.
<h1>Overview</h1>
This is the server side of the UnityNetworking project here, basically this handles packets sent from different unity clients and sends different ones back. I will will be going over
how to run the project and some of the settings. Also if you want to know how to add a new function to the server or the client that is covered in the documentation on https://github.com/CanadianADLlab/UnityNetworkingClient.
<h1>Running the server</h1>
To run the server just open up the command line in the root of the project and use the command dotnet run.
<br>
The server will ask for the port you want to run on, make sure it's the same port configured in the Unity client project under NetworkManager.
<br>
It will then ask for tick rate, tick rate is just how often the server refreshes, the smaller the tick rate the less often it will send things to clients, the higher the tick rate
more network data will be used and potentially more stress on the server but data will be sent more often causing things to be a bit more accurate.
<br>
Now the server should be listening and you should now be able to connect to it from the Unity Client Project.
<br> 
Also the server should just be running on the ip of the device connected so if you wanted to run this over a public connection you should just be able to host it
on something then hit that ip from the Unity project. Right now the Unity project just tries to hit 127.0.0.1.

<h1>Adding new functionality to the server</h1>

If you want to modify the server and add some new packet handling stuff it is written in detail on how to do that on the readme of the https://github.com/CanadianADLlab/UnityNetworkingClient.
<br>
