using Microsoft.AspNetCore.Identity;

namespace ReceptekApi.API.AuthService
{
    /// <summary>
    /// Egyszerű szolgáltatás a jelszavak kezelésére, 
    /// például hash-elésre és ellenőrzésre. 
    /// Ez a szolgáltatás felelős lehet a jelszavak biztonságos tárolásáért és ellenőrzéséért a felhasználói hitelesítés során.
    /// </summary>
    public sealed class PasswordService
    {
        private readonly PasswordHasher<string> _passwordHasher = new();

        /// <summary>
        /// Elkészíti a jelszó hash-ét a megadott felhasználónév és jelszó alapján.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string userName, string password)
        {
            return _passwordHasher.HashPassword(userName, password);
        }

        public bool VerifyPassword(string userName, string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(userName, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
