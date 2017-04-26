using System;
using System.Web;
using System.Web.UI;
using FSCProLib;

namespace WebSSO
{
    public partial class Default : Page
    {
        protected void BtnLogin_Click( object sender, EventArgs e )
        {
            if ( string.IsNullOrWhiteSpace( TxtBoxUsername.Text ) )
            {
                LblErrorMessage.Text = "Username not set";

                return;
            }

            if ( string.IsNullOrWhiteSpace( TxtBoxPassword.Text ) )
            {
                LblErrorMessage.Text = "Password not set";

                return;
            }

            string sessionKey;

            if ( !Login( TxtBoxUsername.Text, TxtBoxPassword.Text, out sessionKey ) )
            {
                LblErrorMessage.Text = "Login failed.";
            }
            else
            {
                Redirect( sessionKey );
            }
        }

        private bool Login( string username, string password, out string sessionKey )
        {
            sessionKey = null;

            var connection = new HostConnection();

            // This should be the name of the host as defined in the Flynet Viewer Admin Console.
            connection.hostName = "YOUR HOSTNAME HERE";

            // This is the name of the application as defined in the definition file loaded below.
            connection.application = "SSO";

            // Example of how to set the user location session option.
            //connection.sessionOptions = string.Format( "userLocation:[{0}]", GetIPAddress() );

            // Need to load the screen recognition.
            var screenDef = new ScreenDefControl();

            // Load the file that defines the recognition for the screens used below.
            screenDef.load( "SSO.xml" );

            connection.connect();

            if ( connection.connected )
            {
                var screen = connection.getScreen();

                // Screen names come from the definition file loaded above.
                if ( screen.name == "Login" )
                {
                    screen.putCommand( username + "[enter]" );

                    screen.waitForScreen( "Password" );

                    if ( screen.name == "Password" )
                    {
                        screen.putCommand( password + "[enter]" );

                        screen.waitForScreen( "SignOnComplete" );

                        if ( screen.name == "SignOnComplete" )
                        {
                            sessionKey = connection.sessionKey;

                            // Disconnect but reseve the session so that FVTerm can pick it up.
                            connection.disconnect( "reserve" );

                            return true;
                        }
                    }
                    else
                    {
                        LblErrorMessage.Text = "Password Screen Not Shown";
                    }
                }
                else
                {
                    LblErrorMessage.Text = "Login Screen Not Shown";
                }
            }
            else
            {
                // Connection failed.
                LblErrorMessage.Text = "Could Not Connect To Host";
            }

            if ( connection != null && connection.connected )
            {
                // Disconnect the session.
                connection.disconnect( "stop" );
            }

            return false;
        }

        private void Redirect( string sessionKey )
        {
            // Redirect to FVTerm with session key.
            var url = string.Format( "FVParent.html?sessionKey={0}", sessionKey );

            Response.Redirect( url );
        }

        /// <summary>
        /// Attempts to find user's IP address, even if they are going via a proxy.
        /// </summary>
        /// <returns>String IP address.</returns>
        private string GetIPAddress()
        {
            var context = HttpContext.Current;

            var ipAddress = context.Request.ServerVariables [ "HTTP_X_FORWARDED_FOR" ];

            if ( !string.IsNullOrEmpty( ipAddress ) )
            {
                var addresses = ipAddress.Split( ',' );

                if ( addresses.Length != 0 )
                {
                    return addresses [ 0 ];
                }
            }

            return context.Request.ServerVariables [ "REMOTE_ADDR" ];
        }
    }
}