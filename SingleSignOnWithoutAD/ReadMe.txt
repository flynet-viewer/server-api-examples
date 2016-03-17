================================================
Server Single Sign On example readme
================================================

This example project was made using VS2015 and compiled using .NET 4.5

It is intended for use with the Flynet Simulated Host, which should be configured to run the Insure script.

The default location of the Insure script is: C:\Program Files\flynet\viewer\SimHostScripts\Insure Script.xml

It stores credentials in a key file located at: C:\inetpub\wwwroot\FVTerm\auth.key

================================================
Initial Setup
================================================

References to the SCWebControls and ViewerLib4 dlls are required to compile the project.

SCWebControls is located in the bin directory of the FVTerm app. 
By default this is C:\inetpub\wwwroot\FVTerm\bin. 
It is also possible to use the 'Add Reference...' dialog to change this.

ViewerLib4 is in the GAC. In the 'Add Reference...' dialog it can be found in 'Assemblies > Extensions > Flynet Viewer .NET Library'. 
Select version 4.1.0.x - On a 64-bit version of Windows select the x64 dll, on a 32-bit version select the dll in the \Win32 directory.

The web.config file in the FVTerm directory must also be modified to reference the dll produced by this example.
For details on this process see the section 'web.config modifications' below.

================================================
Testing, Compilation and Deployment
================================================

The TestConsole project can be used to check that everything is working correctly.

Compile the example and copy SingleSignOn.dll from the output directory to a folder which can be accessed by FVterm, for example: C:\SingleSignOn.
This directory should also contain a sub-directory for the log files produced.

Note: if Windows cannot overwrite an existing dll it may still be in use. Restarting IIS will release the file so that it can be overwritten. 
This can be done by opening a command prompt with Adminstrator privileges and entering the command 'iisreset'.

================================================
web.config modifications
================================================

! Make a backup of the web.config file in case of errors.

In the web.config file:

Uncomment and change the value of the logExtension setting so that it points to the SingleSignOn dll e.g. <add key="logExtension" value="c:\SingleSignOn\SingleSignOn.dll"/>
Uncomment and change the value of the loggingFolder setting so that it points to the logging sub-directory e.g. <add key="loggingFolder" value="c:\SingleSignOn\Logs"/>
Uncomment and change the value of the logLevel setting so that the level is ScreenDetails e.g. <add key="logLevel" value="ScreenDetails"/>
Uncomment the logNameFormat setting, its default value should be fine.
Uncomment and change the value of the loggingDefinition setting so that it references the definition file for the Insure example e.g. <add key="loggingDefinition" value="insure.xml"/>

================================================
Usage
================================================

The first time a terminal session is established the system will behave as normal, allowing the user to input credentials.

These credentials are saved, encrypted, and stored on the server machine for use later. 

After credentials have been stored choose 'Flynet Viewer Terminal Emulation' and select the 'First Flynet Viewer Host' tile to be automatically logged in to the application.

If you have any problems or questions please email support@flynetviewer.com