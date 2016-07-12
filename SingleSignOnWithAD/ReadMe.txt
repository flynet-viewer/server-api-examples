Notes for using the Single Sign On example code.
================================================

This example was written in VS2013 against .NET 4.5

As delivered, this code works against the Insure example host running in the Flynet Simulated Host. To use this example without changes you must have the simulator running
with the Insure script compiled in it. The Insure script can be found here:

C:\Program Files\flynet\viewer\SimHostScripts\Insure Script.xml

In order to compile this example you will need to check the references of the SingleSignOn project to ensure that the SCWebControls and ViewerLib4 dlls can be found.

SCWebControls is found in the bin directory of the FVTerm app, usually C:\inetpub\wwwroot\FVTerm\bin you will need to browse to it in the Add Reference... dialog

ViewerLib4 is in the GAC. In the Add Reference... dialog it can be found in Assemblies > Extensions > Flynet Viewer .NET Library. You will need the 4.1.0.x version.
Be sure to select the x64 version unless you need the x86 one, in which case it's the one in the \Win32 directory.

The TestConsole project will allow you to check that everything is working correctly.

Once you have compiled the example you will need to copy SingleSignOn.dll from the project's output directory into a folder which can be accessed by FVterm when it is running,
C:\SingleSignOn should work fine. This directory should also contain a sub-directory for the log files the code produces.

To run this example you will need to make some changes to AD, IIS and to the web.config file in the FVTerm directory.

AD ATTRIBUTE SETUP
==================

The extension attributes used by this example are created by Exchange.

This example requires that the current AD user has the necessary attributes to store the username and password

Additional attributes can also be added to AD users via a schema. 
See http://www.sharepointpals.com/post/How-to-Custom-Attributes-in-Active-Directory-in-Windows-Server-2012-ADDS for information on this process
If the new attributes have different names to the extension attributes used in the example the code should be updated to reflect this.

AD CONFIGURATION
================

In AD you will need to enable View > Advanced Features to be able to edit the attributes. Once you have enabled this, locate the user who will be
running the example in AD and right-click > Properties. Click on the Attribute Editor tab and locate the following extension attributes.

extensionAttribute1 - this holds the username, which must be set to simmy for the logon to work. Any other value will cause the login to fail.
extensionAttribute2 - this holds the password, which you can set to any value you choose since the Insure script does not check the password.

IIS
===

In the IIS Manager change the Authentication of FVTerm so that Windows Authentication is Enabled and all other authentication methods are disabled.

web.config
==========

In the web.config file:

Make a backup of the web.config file in case of errors.

Change <authentication mode="None" /> to <authentication mode="Windows" />

Uncomment and change the value of the logExtension setting so that it points to the SingleSignOn dll e.g. <add key="logExtension" value="c:\SingleSignOn\SingleSignOn.dll"/>

Uncomment and change the value of the loggingFolder setting so that it points to the logging sub-directory e.g. <add key="loggingFolder" value="c:\SingleSignOn\Logs"/>

Uncomment and change the value of the logLevel setting so that the level is ScreenDetails e.g. <add key="logLevel" value="ScreenDetails"/>

Uncomment the logNameFormat setting, its default value should be fine.

Uncomment and change the value of the loggingDefinition setting so that it references the definition file for the Insure example e.g. <add key="loggingDefinition" value="insure.xml"/>

You should now be able to open FVTerm in your browser, click on First Flynet Viewer Host and be automatically logged in to the Insure application.

Note: if you modify the code and have difficulty updating the dll on disk because it is in use then you will need to restart IIS so that it releases the dll.

If you have any problems or questions please email support@flynetviewer.com


