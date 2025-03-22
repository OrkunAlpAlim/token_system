using Microsoft.AspNetCore.Identity;

namespace WebToken.Models
{
    public class ApplicationUser : IdentityUser
    {
        // IdentityUser zaten bu �zellikleri i�eriyor, MongoDB'ye �zg� Bson attributeleri kald�rd�k

        // UserName, NormalizedUserName ve PasswordHash gibi �zellikler, IdentityUser'da zaten mevcut.
        // Bu nedenle burada tekrar tan�mlamam�za gerek yok.
    }
}
