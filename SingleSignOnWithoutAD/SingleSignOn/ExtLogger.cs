using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FVSingleSignOn;
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
            return true;
        }

        /// <summary>
        /// PreConnectEx is used to manage full details of a new connection.  Especially useful if Windows Security and Active Directory are in play.
        /// </summary>
        /// <param name="context">Active HTTP Request- can use User.Identity to get WindowsIdentity</param>
        /// <param name="hostName">Host name that will be connected to--can change</param>
        /// <param name="luName">LUName or pattern to be used</param>
        /// <param name="locationAddress">The location IP address to be passed--can be changed</param>
        /// <param name="userName">User name passed-in with initialization--use context for Windows or AzureAD signins</param>
        /// <param name="IPAddress">IP Address will be null--change to override the host name's configured address</param>
        /// <param name="port">Telnet PORT -- passed as zero, change if different than default for host</param>
        /// <param name="sessionType">Leave null for host default, options include ssh, tnvt, tn3270 and tn5250</param>
        /// <param name="termType">Optional terminal type (like vt220 or viewpoint)</param>
        /// <param name="SSHKey">If an SSH connection, set to the key otherwise the configured key will be used</param>
        /// <param name="SSHPW">If and SSH connection and password is used set to the SSH password</param>
        /// <param name="errorMessage">Set to error for if returning false</param>
        /// <param name="connectInfo">Dictionary of values for use in-between the PreConnectEx and PostConnectEx methods</param>
        /// <returns>true to continue, false to cancel connection</returns>
        static public bool PreConnectEx( HttpContext context, ref string hostName, ref string luName, ref string locationAddress,
                                                    ref string userName, ref string IPAddress, ref int port, ref string sessionType, ref string termType,
                                                    ref string SSHKey, ref string SSHPW, ref string errorMessage, ref Dictionary<string, object> connectInfo )
        {
            return true;
        }

        /// <summary>
        /// PostConnectEx is called after PreConnectEx
        /// </summary>
        /// <param name="oScreen">Active HostScreen object for the new connection</param>
        /// <param name="connectInfo">Connection info dictionary filled-in by the PreConnectEx method</param>
        /// <param name="errorMessage">Set to an error message if false is returned</param>
        /// <returns>bool - set false to cancel and close the connection</returns>
        static public bool PostConnectEx( HostScreen oScreen, Dictionary<string, object> connectInfo, ref string errorMessage )
        {
            var finished = false;

            while( !finished )
            {
                switch( oScreen.getScreenName() )
                {
                    case "Start":

                        // The presence of the text User starting in column 3 of row 24 indicates that there was a username
                        // or password error. In this case we do nothing and the user will see the screen to correct their error.
                        if( oScreen.getText( 24, 3, 4 ) != "User" )
                        {
                            SingleSignOnExample.AttemptAutomaticSignOnForStart( oScreen );
                        }

                        finished = true;

                        break;
                }
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
            return null;
        }

        /// <summary>
        /// Called when the user edits one or more fields and presses a key to send changes to the host.
        /// </summary>
        /// <param name="logger">Active logger, can log extra lines using methods</param>
        /// <param name="aidKeys">Aid key or keys triggering screen entry</param>
        /// <param name="fields">null or array of Field objects entered by user</param>
        /// <param name="oScreen">Active FSCProLib HostScreen object</param>
        /// <returns>handled indicator--if true is returned, the logger will not log this screen, 
        /// return false for a normal logging of the screen</returns>
        static public bool LogEnter( DebugLogs logger, string aidKeys, Field[] fields, HostScreen oScreen )
        {
            var screenName = oScreen.getScreenName();

            if ( screenName == "SysSelect" )
            {
                if ( fields != null && fields.Length > 0 )
                {
                    foreach ( var field in fields )
                    {
                        oScreen.setText( field.offset, field.text );
                    }

                    SingleSignOnExample.AttemptAutomaticSignOnForSysSelect( oScreen, logger );
                }
            }
            else if ( screenName == "Signon" )
            {
                SingleSignOnExample.DoSignOnWithCapture( oScreen, fields );
            }

            return true;
        }
    }
}
