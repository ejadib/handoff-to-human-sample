# Hand-off to Humans - Bot Sample 

A sample bot showing how to hand-off to humans using Microsoft Bot Framework

### Prerequisites
The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* Register your bot with the Microsoft Bot Framework. Please refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering) for the instructions.
* [Optional] The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

> Note: Documentation coming soon....

You need at least two active conversations from different "users" (a conversation in two different channels for example):

* In one of the conversation, send the "register" message to register that user as available agent. 
* In the other conversation, chat with the bot as usual. Sending a phrase containing the word "leg" when replying to the symptoms, will trigger the question to the user asking if he wants to talk with a real person. 


### TODO Items

* Extract agent storage out of MessageRouter to allow extensibility
* Enable "Waiting mode" for users when there is no Agent available
* Add ability to unregister agents
* Create 2 scorables with logic from MessageRouter:
 * One should register/unregister agents
 * The other one should route messages