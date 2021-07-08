using System;
using Shouldly;
using Xunit;

namespace JOS.Tokens.Tests
{
    public class AccessTokenTests
    {
        [Fact]
        public void ExpiredShouldBeFalseIfTokenExpiresLaterThanThreshold()
        {
            var accessToken = new AccessToken(Guid.NewGuid().ToString(), (int) TimeSpan.FromMinutes(30).TotalSeconds);

            accessToken.Expired.ShouldBeFalse();
        }

        [Fact]
        public void ExpiredShouldBeTrueIfTokenExpiresSoonerThanThreshold()
        {
            var accessToken = new AccessToken(Guid.NewGuid().ToString(), (int)TimeSpan.FromMinutes(1).TotalSeconds);

            accessToken.Expired.ShouldBeTrue();
        }

        [Fact]
        public void ExpiredShouldBeTrueIfTokenExpiresAtThreshold()
        {
            var accessToken = new AccessToken(Guid.NewGuid().ToString(), (int)TimeSpan.FromMinutes(5).TotalSeconds);

            accessToken.Expired.ShouldBeTrue();
        }
    }
}
