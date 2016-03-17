using System;
using System.DirectoryServices.AccountManagement;

using AuditExample;

namespace TestConsole
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                using ( var ctx = new PrincipalContext( ContextType.Domain ) )
                {
                    var dbsUser = FlynetUserPrincipal.FindByIdentity( ctx, IdentityType.SamAccountName, Environment.UserName );

                    Console.WriteLine( "User: {0}", dbsUser.DisplayName );
                    Console.WriteLine( "Host username: {0}", dbsUser.HostUsername );
                    Console.WriteLine( "Host password: {0}", dbsUser.HostPassword );

                    Console.WriteLine( "LUName: {0}", dbsUser.GetLUName() );
                }
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
