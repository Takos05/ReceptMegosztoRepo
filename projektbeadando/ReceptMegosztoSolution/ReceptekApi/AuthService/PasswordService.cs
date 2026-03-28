using Microsoft.AspNetCore.Identity;

namespace ReceptekApi.API.AuthService
{
    
    public sealed class PasswordService
    {
        private readonly PasswordHasher<string> _passwordHasher = new();

        
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
