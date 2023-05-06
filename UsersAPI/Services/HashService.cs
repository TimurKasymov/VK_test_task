using DLL.Abstractions;
using DLL.Entities;
using DLL.Repositories;
using System.Security.Cryptography;
using System.Text;
using UsersAPI.Services.EntityServices;

namespace UsersAPI.Services
{
    public class HashService
    {
        private ILogger<HashService> _logger;

        public HashService(ILogger<HashService> logger)
        {
            _logger = logger;
        }

       /// <summary>
       /// hashes the given string 
       /// </summary>
       /// <param name="toHash"></param>
       /// <returns></returns>
        public string Hash(string toHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var varhashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(toHash));
                string result = Convert.ToBase64String(varhashedBytes);
                return result;
            }
        }

    }
}
