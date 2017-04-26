Notes for using the Web Single Sign On example code
===================================================

This example shows how to use an ASP.NET web form to collect login information and use that information to connect
to a host via FVTerm.

This example was written in VS2015 against .NET 4.5.1

Required Packages
=================

This example uses the following packages, mangage them from NuGet into the WebSSO project in the usual fashion:

* arg.js
* jQuery
* Microsoft.CodeDom.DotNetCompilerPlatform
* Microsoft.Net.Compilers

See packages.config in the WebSSO project for details on version numbers.

FVTermParent.js
===============

This file can be found in C:\inetpub\wwwroot\FVTerm\Scripts, copy it into the Scripts folder of the WebSSO project.

Hostname
========

You will need to edit the Login method of Default.apsx.cs in the WebSSO project and change the text "YOUR HOSTNAME HERE" to be the name of the host you wish to
conect to as defined in the Flynet Viewer Admin Console.

Compilation
===========

In order to compile this example you will need to check the references of the WebSSO project to ensure that the ViewerLib4 dlls can be found.

ViewerLib4 is in the GAC. In the Add Reference... dialog it can be found in Assemblies > Extensions > Flynet Viewer .NET Library. You will need the 4.1.0.x version.
Be sure to select the x64 version unless you need the x86 one, in which case it's the one in the \Win32 directory.

Definition File
===============

The definition file supplied with the project (SSO.xml) gives an example of how to define recognition for the host screens used in the login sequence. You will
need to edit this file so that the login screen(s) specific to your host can be recognised by the example. Each screen to be recognised appears as an element
in the file like this:

~~~~
<Screen name="Login" treatAsDefault="true" paddingMode="default" spaceType="blank" cursorHomeRow="3" cursorHomeColumn="8">
    <Recognize row="3" column="1" text="Login:" type="include" spaceType="blank" />
</Screen>
~~~~

Here the Login screen is recognised because it has the text Login: on it begining at row 3, column 1. In addition, the cursor will be at row 3, column 8. You will
need to add or modify Screen elements in the file to cover all the screens used to login to your host. Note that the file include recognition for the first screen
shown after a successful login (called SignOnComplete) so that the code can know that the login worked.

You may find that enabling tracing in Flynet Viewer and using the Trace Viewer is useful when modifying this file.

Once the file is complete, copy it to C:\Program Files\flynet\viewer\Definitions so that it can be loaded by the example code.

How It Works
============

Default.aspx shows textboxes for username and password along with login button. When the login button is pressed, its callback checks that a username and password
has been entered and then calls the Login method. The Login method creates and configures a new HostConnection, loads the definition file (see above) and then
connects to the host.

Once the connection has connected, the code gets the current screen from the connection and, if the screen is Login, puts the username and an [enter] into the screen
and waits for the next screen in the sequence (Password).

If the next screen is Password, then the code enters the password into the screen and waits for the next screen (SignOnComplete).

If the next screen is SignOnComplete then the code gets the connection's session key and then disconnects the session, reserving it so that FVTerm can pick it up
again later. The Login method then returns true, passing the session key back as an out param.

If the wrong sequence of screens is encountered, the Login method sets an error message, disconnects the session and returns false.

A successful return from the Login method causes the button callback to redirect the browser to FVParent.html, passing the session key as a URL param.

FVParent.html is a simple web page which contains an iframe in which FVTerm will be displayed. When the page is loaded it calls the showSession() function which,
assuming there is a session key, calls ConnectFVTerm() from FVTermParent.js passing the session key to connect to the existing session which will be displayed
in the iframe. In addition, an onClose() function is registered so that, when the FVTerm session ends, the user is sent back to Default.aspx.

If you have any problems or questions please email support@flynetviewer.com

