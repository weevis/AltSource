using Microsoft.AspNetCore.Identity;

namespace AltSourceBankAppAPI.Entity
{
    /* We can map an Identity to a User for the UserManager and SigninManager to use */
    public class UserEntity : IdentityUser
    {}
}
