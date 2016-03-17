using System;
using System.IO;
using Flynet.SCWebControls;
using FSCProLib;
using EncryptString;

namespace FVSingleSignOn
{
    public static class SingleSignOnExample
    {
        static private readonly string authPath = @"C:\inetpub\wwwroot\FVTerm\auth.key";

        static private string savedUsername = null;
        static private string savedPassword = null;

        static private void ResetCredentials()
        {
            savedUsername = null;
            savedPassword = null;
        }

        static private void SaveCredentialsToFile( string argConstUsername, string argConstPassword )
        {
            string concatenated = argConstUsername + "|" + argConstPassword;
            File.WriteAllText( authPath, Encrypt.EncryptString( concatenated ) );
        }

        static private bool PopulateCredentialsFromFile( string argConstUsername, string argConstPassword )
        {
            try
            {
                string rawText = File.ReadAllText( authPath );

                string decryptedText = Encrypt.DecryptString( rawText );
                string[] splitText = decryptedText.Split( '|' );

                savedUsername = splitText[0];
                savedPassword = splitText[1];
            }
            catch( Exception ex )
            {
            }

            bool changed = false;
            if( !string.IsNullOrWhiteSpace( argConstUsername ) && savedUsername != argConstUsername )
            {
                savedUsername = argConstUsername;

                changed = true;
            }

            if( !string.IsNullOrWhiteSpace( argConstPassword ) && savedPassword != argConstPassword )
            {
                savedPassword = argConstPassword;

                changed = true;
            }

            return changed;
        }

        static private void SubmitCredentialsForSysSelect( HostScreen oScreen, string user )
        {
            oScreen.setText( 22, 23, user );
            oScreen.putCommand( "[enter]" );
        }

        static private void SubmitCredentialsForSignOn( HostScreen oScreen, string user, string pass )
        {
            oScreen.mappedSet( "Userid", user );
            oScreen.mappedSet( "Password", pass );

            ResetCredentials();

            oScreen.putCommand( "[enter]" );
        }

        static private void CompleteSignOn( HostScreen oScreen )
        {
            // SignonComplete
            oScreen.putCommand( "[clear]" );

            // MyClearScreen
            oScreen.setText( 1, 1, "info" );
        }

        static public bool AttemptAutomaticSignOnForStart( HostScreen oScreen )
        {
            bool authExists = File.Exists( authPath );
            if( !authExists )
                return false;

            PopulateCredentialsFromFile( savedUsername, savedPassword );

            if( string.IsNullOrWhiteSpace( savedUsername ) )
                return false;

            SubmitCredentialsForSysSelect( oScreen, savedUsername );

            return true;
        }

        static public bool AttemptAutomaticSignOnForSysSelect( HostScreen oScreen, DebugLogs logger )
        {
            bool authExists = File.Exists( authPath );
            if( !authExists )
                return false;

            PopulateCredentialsFromFile( savedUsername, savedPassword );

            if( string.IsNullOrWhiteSpace( savedUsername ) || string.IsNullOrWhiteSpace( savedPassword ) )
                return false;

            oScreen.putCommand( "[enter]" );
            oScreen.waitForScreen( "Signon", 100 );

            bool complete = false;
            while( !complete )
            {
                switch( oScreen.getScreenName() )
                {
                    case "Signon":
                        SubmitCredentialsForSignOn( oScreen, savedUsername, savedPassword );
                        CompleteSignOn( oScreen );

                        complete = true;
                        break;
                    default:
                        break;
                }
            }

            return complete;
        }

        static public void DoSignOnWithCapture( HostScreen oScreen, Field[] fields )
        {
            string capturedUsername = null;
            string capturedPassword = null;

            int multiRowIndex;

            // Iterate through the updated fields looking for the username and password.
            foreach( var field in fields )
            {
                var fieldName = SCDispatcher.GetFieldName( oScreen, field.offset, out multiRowIndex );

                if( fieldName == "Signon.Userid" )
                {
                    capturedUsername = field.text;
                }
                else if( fieldName == "!Signon.Password" )
                {
                    capturedPassword = field.text;
                }
            }

            bool needFileSave = PopulateCredentialsFromFile( capturedUsername, capturedPassword );
            if( needFileSave )
            {
                SaveCredentialsToFile( capturedUsername, capturedPassword );
            }

            SubmitCredentialsForSignOn( oScreen, capturedUsername, capturedPassword );
            CompleteSignOn( oScreen );
        }
    }
}
