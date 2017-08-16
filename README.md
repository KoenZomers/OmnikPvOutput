# Omnik PvOutput

Allows reading of live statistics from an Omnik Solar Inverter with WiFi module and posting the statistics to PVOutput.org.

## Download

[Download the latest version](../../releases/latest)

## Setup steps

I'm assuming here you already have your Omnik Solar Inverter with WiFi module configured to be able to access your WiFi network and you know it's IP address.

1. Create an account at [PVOutput.org](http://www.pvoutput.org) if you haven't done so already
2. Under [Settings](http://pvoutput.org/account.jsp) at the PVOutput website, at the bottom in the Registered Systems section, add one or more Solar Panel sets, if you haven't done so already
3. Take note of the System Id assigned to your registered systems. We'll need these later on.
4. Right above the Registered Systems section you'll find the API Settings section. Make sure API Access is set to Enabled and you've generated an API Key. Take note of this API Key as we'll need it later on.
5. [Download the console application](https://github.com/KoenZomers/OmnikPvOutput/raw/master/KoenZomers.Omnik.PVOutput.zip)
6. Extract the files to some location on your machine
7. Open the KoenZomers.Omnik.PVOutput.ConsoleApp.exe.config file in your favorite text editor (i.e. notepad)
8. You'll see XML inside this file. Fill out each of the value arguments within the appSettings section, so OmnikAddress, OmnikSerial, PVOutputApiKey, PvOutputSystemId. The comments in the XML describe where you can get these values from. PvOutputSystemId is the value from step 3 above, PVOutputApiKey is the value from step 4 above.
9. Run the Console Application by starting KoenZomers.Omnik.PVOutput.ConsoleApp.exe. It will connect to your Omnik Solar Inverter with WiFi module, retrieve its statistics and post them to PVOutput
10. On the PVOutput.org website under Live (middle of the page at the left) you should see the entries coming in. Note that it only accepts a post once every 5 minutes at most, so if you run it several times after each other, you won't see the result show up on the website even though the API responded with OK.

You can schedule this tool to run every five minutes using Windows Scheduler. If you have multiple Omnik Solar Inverters, just create a copy of the tool in separate folders, adjust the KoenZomers.Omnik.PVOutput.ConsoleApp.exe.config files of each of them with the correct configuration and schedule them to run at an interval.

In case you don't want any 24/7 Windows machine to be running to execute this code, another option could be to have it run in Microsoft Azure. For free you can have it run there as an Azure Website Webjob once every hour. If you want it more often than that (i.e. every five minutes), you need to scale up the Azure Scheduler component in your Azure subscription from free to at least standard level which does result in costs being charged to you.

## Version History

[Version 1.0.1.0](../../releases/tag/1.0.1.0) - August 17, 2017 

- Added showing details on the pull failing to the console output
- Replaced get properties with lambda expressions

## Feedback

Questions/comments/bug reports? Feel free to share them with me.

Koen Zomers
mail@koenzomers.nl
