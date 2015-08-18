using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Net;
using System.Net.Http;

namespace MyAPI2.Infrastructure
{
    public static class ExtendedClaimProvider
    {
        public static IEnumerable<Claim> GetClaims(ApplicationUser user)
        {
            List<Claim> userClaims = new List<Claim>();

            // TODO : Add Claims based on Roles here 
         //   userClaims.Add(new Claim("ViewPost",false));
          //  userClaims.Add(new Claim("UpdatePost", "0"));
          //  userClaims.Add(new Claim("DeletePost", "0"));
            return userClaims;
        }

        public static Claim CreateClaim(string type, string value)
        {
            return new Claim(type,value, ClaimValueTypes.String);
        }
    }

    public class ClaimsAuthorizationAttribute : AuthorizationFilterAttribute
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (!principal.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Task.FromResult<object>(null);
            }
          
            if (!(principal.HasClaim(x => x.Type.Equals(ClaimType) && x.Value.Equals(ClaimValue))))
            //if (!principal.Claims.Any(x => x.Type == ClaimType && x.Value == ClaimValue))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                return Task.FromResult<object>(null);
            }

            //User is Authorized, complete execution
            return Task.FromResult<object>(null);

        }
    }
}