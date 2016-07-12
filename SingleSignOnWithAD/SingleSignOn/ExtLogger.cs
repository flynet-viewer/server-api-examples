using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SingleSignOn;
using Flynet.SCWebControls;
using FSCProLib;

namespace FVExtLogger
{
    public class ExtLogger
    {
        /// <summary>
        /// PreConnect is used to manage details of a new connection.  Especially useful if Windows Security and Active Directory are in play.
        /// </summary>
        /// <param name="context">Active HTTP Request- can use User.Identity to get WindowsIdentity</param>
        /// <param name="hostName">Host name that will be connected to--can change</param>
        /// <param name="luName">LUName or pattern to be used--can change</param>
        /// <param name="locationAddress">The location IP address to be passed--can be changed</param>
        /// <returns>true to continue, false to cancel connection</returns>
        static public bool PreConnect( HttpContext context, ref string hostName, ref string luName, ref string locationAddress )
        {
            // This method is called whenever a user tries to connect to Flynet.
            // This implementation uses the connecting users AD group membership to set the connection's LU Name.

            var identity = HttpContext.Current.User.Identity as WindowsIdentity;

            if ( identity == null || identity.IsAnonymous || !identity.IsAuthenticated )
            {
                luName = "Guest";

                return true;
            }

            try
            {
                using ( var ctx = new PrincipalContext( ContextType.Domain ) )
                {
                    var flynetUser = FlynetUserPrincipal.FindByIdentity( ctx, IdentityType.SamAccountName, identity.Name );

                    luName = flynetUser.GetLUName();
                }
            }
            catch ( Exception ex )
            {
                luName = "Guest";
            }

            return true;
        }

        /// <summary>
        /// Called when screen is received from host when logLevel is ScreenDetails or Higher
        /// </summary>
        /// <param name="logger">Logger Class - must have reference to SCWebControls in FVTerm to work</param>
        /// <param name="oScreen">Flynet HostScreen Object</param>
        /// <returns>null for no logging, oScreen.text to log -- can change logged text, can also change screen text with oScreen object</returns>
        static public string LogScreen( DebugLogs logger, HostScreen oScreen )
        {
            // This method is called whenever a new screen is recieved from the host.
            // This implementation watches for the start screen and then tries to log the user on
            // based on information obtained from AD.

            var screenID = oScreen.getScreenName();

            if ( screenID == "Start" )
            {
                if ( oScreen.getText( 24, 3, 4 ) == "User" )
                {
                    // The presence of the text User starting in column 3 of row 24 indicates that there was a username
                    // or password error. In this case we do nothing and the user will see the screen to correct their error.
                    return null;
                }

                var identity = HttpContext.Current.User.Identity as WindowsIdentity;

                if ( identity == null || identity.IsAnonymous || !identity.IsAuthenticated )
                {
                    return "Windows user not set, anonymous or not authenticated.";
                }

                try
                {
                    using ( var ctx = new PrincipalContext( ContextType.Domain ) )
                    {
                        var flynetUser = FlynetUserPrincipal.FindByIdentity( ctx, IdentityType.SamAccountName, identity.Name );

                        if ( string.IsNullOrWhiteSpace( flynetUser.HostUsername ) ||
                             string.IsNullOrWhiteSpace( flynetUser.HostPassword ) )
                        {
                            // No username/password found in AD, do nothing so that the user will see the screen and can enter thier credentials.
                            return null;
                        }

                        // Use the Viewer API to set the username and password, then send the enter key to log the user on.
                        oScreen.mappedSet( "Username", flynetUser.HostUsername );
                        oScreen.mappedSet( "Password", flynetUser.HostPassword );

                        oScreen.putCommand( "[enter]" );
                    }
                }
                catch ( Exception ex )
                {
                    return string.Format( "Exception accessing AD: {0}", ex.Message );
                }
            }

            return null;
        }

        /// <summary>
        /// Log Screen Entry plug-in
        /// </summary>
        /// <param name="logger">Active logger, can log extra lines using methods</param>
        /// <param name="aidKeys">Aid key or keys triggering screen entry</param>
        /// <param name="fields">null or array of Field objects entered by user</param>
        /// <param name="oScreen">Active FSCProLib HostScreen object</param>
        /// <returns>handled indicator--if true is returned, the logger will not log this screen, 
        /// return false for a normal logging of the screen</returns>
        static public bool LogEnter( DebugLogs logger, string aidKeys, Field[] fields, HostScreen oScreen )
        {
            // This method is called when the user edits one or more fields and presses a key to send the
            // changes to the host.
            // This implementation watches for the Start screen and scrapes the changes into AD.
            // Note: In order to actually update AD you will need to change the identity of the application pool
            //       that FVTerm runs in to one that has permissions to update AD.

            var screenID = oScreen.getScreenName();

            if ( screenID == "Start" )
            {
                var identity = HttpContext.Current.User.Identity as WindowsIdentity;

                if ( identity == null || identity.IsAnonymous || !identity.IsAuthenticated )
                {
                    logger.Log( "Auto Logon", "Windows user not set, anonymous or not authenticated." );

                    return false;
                }

                try
                {
                    string username = null;
                    string password = null;

                    int multiRowIndex;

                    // Itterate through the updated fields looking for the username and password.
                    foreach ( var field in fields )
                    {
                        var fieldName = SCDispatcher.GetFieldName( oScreen, field.offset, out multiRowIndex );

                        switch ( fieldName )
                        {
                            case "Username":

                                username = field.text;

                                break;

                            case "Password":

                                password = field.text;

                                break;
                        }
                    }

                    using ( var ctx = new PrincipalContext( ContextType.Domain ) )
                    {
                        // Look to see if there are changes from what is in AD. If there are, update AD.
                        var flynetUser = FlynetUserPrincipal.FindByIdentity( ctx, IdentityType.SamAccountName, identity.Name );

                        var needSave = false;

                        if ( !string.IsNullOrWhiteSpace( username ) && flynetUser.HostUsername != username )
                        {
                            flynetUser.HostUsername = username;

                            needSave = true;
                        }

                        if ( !string.IsNullOrWhiteSpace( password ) && flynetUser.HostPassword != password )
                        {
                            flynetUser.HostPassword = password;

                            needSave = true;
                        }

                        if ( needSave )
                        {
                            // This is commented out because by default ApplicationPoolIdentity (which is what this code runs as)
                            // doesn't have permission to update AD, so calling Save() will result in an exception.
                            //flynetUser.Save();
                        }
                    }
                }
                catch ( Exception ex )
                {
                    logger.Log( "Auto Logon", string.Format( "Exception accessing AD: {0}", ex.Message ) );

                    return false;
                }
            }

            return true;
        }
    }
}
