# Send Notifications to phones (Android and iOS)
This code domenstrate how to send customized notifications to users' Android and iOS phones according to phone tokens. It combined the usage of Azure Event Grid, Azure Functions, and Azure Noticification Hub to implement sending noticifications to phones instead of specific apps. The noticifications can notice users by texts, emojis, sounds, and light hints. Of course, we must be upfront about the business of sending notifications with the phone owners.

This code is helpful for real practice projects and provide one of the ways for implementing this kind of business needs. Some code is simplified and used placeholders to implement specific business requirments.

## Simple Explanations about used Techniques
Event Grid One Topic
In this code, one Event Grid topic is suggested to create. You can also create multiple topics if needed.

Multiple Azure Functions as event subscribers
Here, several Azure functions are used as subscribers and subscribed to the only one event topic.

RESTful API as Event Publisher
We used web APIs to publish events to the topic instead of using Azure Functions.
