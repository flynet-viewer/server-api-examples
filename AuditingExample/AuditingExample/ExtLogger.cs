using System.Web;
using AuditExample;
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
            // This method is called whenever a user tries to connect to Flynet Viewer Server.

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
            // This method is called whenever a new screen is received from the host.

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
            // This method is called whenever enter is pressed on the host

            var currentScreenName = oScreen.getScreenName();
            var currentLUName = logger.oConn.hostLUName;

            logger.Log( LogLevelEnum.ScreenDetails, "Logging Example", string.Format( "Screen Accessed: {0} LUName: {1}", currentScreenName, currentLUName ) );

            return true;
        }
    }
}
