Server API example readme
================================================

This is a collection of example projects made using VS2015 and compiled using .NET 4.5

These are intended for use with the Flynet Simulated Host, which should be configured to run the Insure script.

The default location of the Insure script is: C:\Program Files\flynet\viewer\SimHostScripts\Insure Script.xml

Initial Setup
================================================

References to the SCWebControls and ViewerLib4 dlls are required to compile these projects.

SCWebControls is located in the bin directory of the FVTerm app. 
By default this is C:\inetpub\wwwroot\FVTerm\bin. 
It is also possible to use the 'Add Reference...' dialog to change this.

ViewerLib4 is in the GAC. In the 'Add Reference...' dialog it can be found in 'Assemblies > Extensions > Flynet Viewer .NET Library'. 
Select version 4.1.0.x - On a 64-bit version of Windows select the x64 dll, on a 32-bit version select the dll in the \Win32 directory.

The web.config file in the FVTerm directory must also be modified to reference the dlls produced by these examples.
See individual readme files for more detailed instructions.

List of examples
================================================

This repository contains the following example projects:

Auditing example
================================================

An example of how to implement auditing

Single sign on with AD
================================================

A single sign on example using Active Directory

Single sign on without AD
================================================

A single sign on example using the local file system

Web Single Sign On
================================================

A single sign on example using a web page to collect the username and password

Support
================================================

If you have any problems or questions please email support@flynetviewer.com