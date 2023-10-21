using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using LiveGameApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Duende.IdentityServer.AspNetIdentity;

namespace LiveGameApp.Services
{

    public class MyProfileService : ProfileService<Appuser>
    {

        public MyProfileService(UserManager<Appuser> userManager, IUserClaimsPrincipalFactory<Appuser> claimsFactory): base(userManager, claimsFactory)
        {
        }

        override public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);

            Appuser user = await UserManager.GetUserAsync(context.Subject);

            IList<string> roles = await UserManager.GetRolesAsync(user);

            IList<Claim> claims = new List<Claim>();
            foreach (string role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }
            //claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));

            context.IssuedClaims.AddRange(claims);

        }
    }
}
