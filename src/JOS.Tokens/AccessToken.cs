using System;

namespace JOS.Tokens
{
    public class AccessToken
    {
        // I usually let my token "expire" 5 minutes before it's actual expiration
        // to avoid using expired tokens and getting 401.
        private static readonly TimeSpan Threshold = new(0, 5, 0);

        public AccessToken(string token, int expiresInSeconds) : this(token, null, expiresInSeconds)
        {

        }

        public AccessToken(
            string token,
            string? refreshToken,
            int expiresInSeconds)
        {
            Token = token;
            RefreshToken = refreshToken;
            ExpiresInSeconds = expiresInSeconds;
            Expires = DateTime.UtcNow.AddSeconds(ExpiresInSeconds);
        }

        public string Token { get; }
        public string? RefreshToken { get; }
        public int ExpiresInSeconds { get; }
        public DateTime Expires { get; }
        public bool Expired => (Expires - DateTime.UtcNow).TotalSeconds <= Threshold.TotalSeconds;
    }
}
