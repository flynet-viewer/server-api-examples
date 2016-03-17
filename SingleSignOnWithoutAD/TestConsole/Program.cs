using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SingleSignOn;
using EncryptString;

namespace TestConsole
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                string rawText = "";
                string savedUsername = "";
                string savedPassword = "";

                rawText = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\FVTerm\auth.key");

                string decryptedText = Encrypt.DecryptString(rawText);
                string[] splitText = decryptedText.Split('|');

                savedUsername = splitText[0];
                savedPassword = splitText[1];

                Console.WriteLine("Host username: {0}", savedUsername);
                Console.WriteLine("Host password: {0}", savedPassword);
                //Console.WriteLine( "LUName: {0}", dbsUser.GetLUName() );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "Exception: {0}", ex.Message );
            }

            Console.WriteLine( "" );
            Console.WriteLine( "Press any key to exit" );
            Console.ReadKey();
        }
    }
}
