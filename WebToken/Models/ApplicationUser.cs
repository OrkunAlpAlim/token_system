using Microsoft.AspNetCore.Identity;

namespace WebToken.Models
{
    public class ApplicationUser : IdentityUser
    {
        // IdentityUser zaten bu özellikleri içeriyor, MongoDB'ye özgü Bson attributeleri kaldýrdýk

        // UserName, NormalizedUserName ve PasswordHash gibi özellikler, IdentityUser'da zaten mevcut.
        // Bu nedenle burada tekrar tanýmlamamýza gerek yok.
    }
}
