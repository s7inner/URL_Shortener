using URL_Shortener.Data;
using Microsoft.EntityFrameworkCore;

namespace URL_Shortener.Services
{
    public class UrlShorteningService
    {
        public const int NumberOfCharsInShortLink = 5;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
     
        private readonly Random _random = new ();
        private readonly AuthDbContext _dbContext;

        public UrlShorteningService(AuthDbContext authDbContext)
        {
            _dbContext = authDbContext;
        }

        public async Task<string> GenerateUniqueCode() {

            var codeChars = new char[NumberOfCharsInShortLink];

            while (true) 
            {
                for (var i = 0; i < NumberOfCharsInShortLink; i++)
                {
                    int randomIndex = _random.Next(Alphabet.Length - 1);

                    codeChars[i] = Alphabet[randomIndex];
                }

                var code = new string(codeChars);

                if (! await _dbContext.ShortUrls.AnyAsync(s => s.Code == code))
                {
                    return code;
                }
            }
        }
    }
}
