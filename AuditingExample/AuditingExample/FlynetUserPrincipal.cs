using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace AuditExample
{
    /// <summary>
    /// A class that extends UserPrincipal to add properties to support single sign-on.
    /// The additional properties are all stored in AD extension properties on the UserPrincipal.
    /// 
    /// Adapted from an example in this MSDN blog:
    /// http://blogs.msdn.com/b/kaevans/archive/2012/04/11/querying-active-directory-using-principal-extensions-in-system-directoryservices-accountmanagement.aspx
    /// </summary>
    [DirectoryRdnPrefix( "CN" )]
    [DirectoryObjectClass( "person" )]
    public sealed class FlynetUserPrincipal : UserPrincipal
    {
        /// <summary>
        /// Ctor, passes through to the base class method.
        /// </summary>
        /// <param name="context"></param>
        public FlynetUserPrincipal( PrincipalContext context )
            : base( context ) {}

        /// <summary>
        /// Ctor, passes through to the base class method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="samAccountName"></param>
        /// <param name="password"></param>
        /// <param name="enabled"></param>
        public FlynetUserPrincipal( PrincipalContext context, string samAccountName, string password, bool enabled )
            : base( context, samAccountName, password, enabled ) {}

        /// <summary>
        /// Additional property to get and set the user's username for the host.
        /// </summary>
        [DirectoryProperty( "extensionAttribute1" )]
        public string HostUsername
        {
            get
            {
                return GetExtensionAttrValue( "extensionAttribute1" );
            }

            set
            {
                // To persist changes you need to call Save() on the UserPrincipal.
                ExtensionSet( "extensionAttribute1", value );
            }
        }

        /// <summary>
        /// Additional property to get and set the user's password for the host.
        /// </summary>
        [DirectoryProperty( "extensionAttribute2" )]
        public string HostPassword
        {
            get
            {
                return GetExtensionAttrValue( "extensionAttribute2" );
            }

            set
            {
                // To persist changes you need to call Save() on the UserPrincipal.
                ExtensionSet( "extensionAttribute2", value );
            }
        }

        /// <summary>
        /// Uses the user's AD groups to determine the LU Name to
        /// Use when connecting to the host.
        /// </summary>
        /// <returns></returns>
        public string GetLUName()
        {
            using ( var groups = this.GetGroups() )
            {
                if ( groups.Any( g => g.Name == "Domain Admins" ) )
                {
                    return "AdminUser";
                }

                if ( groups.Any( g => g.Name == "Domain Users") )
                {
                    return "NormalUser";
                }

                return "Guest";
            }
        }

        /// <summary>
        /// Gets the value of the attribute with the given name or null if no such attribute is
        /// defined on the current principal.
        /// </summary>
        /// <param name="extensionAttrName"></param>
        /// <returns></returns>
        private string GetExtensionAttrValue( string extensionAttrName )
        {
            var attr = ExtensionGet( extensionAttrName );

            if ( attr.Length != 1 )
            {
                return null;
            }

            return (string) attr[ 0 ];
        }

        /// <summary>
        /// Locates a principal with the given identity type and value within the given context.
        /// 
        /// Marked as new because it hides a method with the same signature on the base to return the more derived type.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="identityType"></param>
        /// <param name="identityValue"></param>
        /// <returns></returns>
        public static new FlynetUserPrincipal FindByIdentity( PrincipalContext ctx, IdentityType identityType, string identityValue )
        {
            return (FlynetUserPrincipal) FindByIdentityWithType( ctx, typeof( FlynetUserPrincipal ), identityType, identityValue );
        }
    }
}
