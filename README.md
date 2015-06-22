# OmnikPvOutput

Allows reading of live statistics from an Omnik Solar Inverter with WiFi module and posting the statistics to PVOutput.org.

## Setup steps

1. Create an account at [PVOutput.org](http://www.pvoutput.org) if you haven't done so already
2. Under [Settings](http://pvoutput.org/account.jsp) at the PVOutput website, at the bottom in the Registered Systems section, add one or more Solar Panel sets, if you haven't done so already
3. Take note of the System Id assigned to your registered systems. We'll need these later on.
4. Right above the Registered Systems section you'll find the API Settings section. Make sure API Access is set to Enabled and you've generated an API Key. Take note of this API Key as we'll need it later on.
5. [Download the console application](/KoenZomers/OmnikPvOutput/raw/master/KoenZomers.Omnik.PVOutput.zip)
6. Extract the files to some location on your machine
7. Open the KoenZomers.Omnik.PVOutput.ConsoleApp.exe.config file in your favorite text editor (i.e. notepad)
8. You'll see XML inside this file. Fill out each of the value arguments within the appSettings section, so OmnikAddress, OmnikSerial, PVOutputApiKey, PvOutputSystemId. The comments in the XML describe where you can get these values from. PvOutputSystemId is the value from step 3 above, PVOutputApiKey is the value from step 4 above.
9. Run the Console Application by starting KoenZomers.Omnik.PVOutput.ConsoleApp.exe. It will connect to your Omnik Solar Inverter with WiFi module, retrieve its statistics and post them to PVOutput
10. On the PVOutput.org website under Live (middle of the page at the left) you should see the entries coming in. Note that it only accepts a post once every 5 minutes at most, so if you run it several times after each other, you won't see the result show up on the website even though the API responded with OK.

## Feedback

Questions/comments/bug reports? Feel free to share them with me.

Koen Zomers
mail@koenzomers.nl
