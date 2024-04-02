# MauiAppPush
This repo contains some boilerplate code on how to make push from azure to work. It is not complete and need correct certificats and all that stuff from Apple and Google.

In my real project I use a dependency injection service to hold my hub so I can change tags from the app. Just call CreateOrUpdateInstallationAsync with the new tags when the changes.
