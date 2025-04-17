using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Pokemon.Components.Account
{
    internal sealed class IdentityUserAccessor(
        UserManager<ApplicationUserModel> userManager,
        IdentityRedirectManager redirectManager
    )
    {
        public async Task<ApplicationUserModel> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus(
                    "Account/InvalidUser",
                    $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.",
                    context
                );
            }

            return user;
        }
    }
}
