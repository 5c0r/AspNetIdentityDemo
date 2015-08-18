using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using MyAPI2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MyAPI2.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); // TODO: Validate Client ID
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = "http://localhost:8080";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            //var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            using (AuthRepository _repo = new AuthRepository()) {
                ApplicationUser user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }



                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(_repo._userManager, "JWT");

                //oAuthIdentity.AddClaims(ExtendedClaimProvider.GetClaims(user));

                var ticket = new AuthenticationTicket(oAuthIdentity,null);

                context.Validated(ticket);
            }

        }
    }
}