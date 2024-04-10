# MauiAppPush
This repo contains some boilerplate code on how to make push from azure to work.

For iOS you don´t need anything else in your app except selecting the correct certificate when you build.

Android you need the google-service.json file you download from you firebase console and add it directly under Android folder.

Here are a tutorial on iOS. https://learn.microsoft.com/en-us/azure/notification-hubs/ios-sdk-get-started

Setting up Android FCM V1 https://learn.microsoft.com/en-us/azure/notification-hubs/firebase-migration-rest

If GoogleServieJson doesn´t show up as build action https://github.com/dotnet/maui/issues/14486

In my real project I use a dependency injection service to hold my hub so I can change tags from the app. Just call CreateOrUpdateInstallationAsync with the new tags when they changes.

I had an issue with android release which stopped working. My solution was to remove trim and AOT from release build and it started to work again.
