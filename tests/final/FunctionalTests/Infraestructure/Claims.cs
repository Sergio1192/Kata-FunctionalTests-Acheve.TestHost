using System.Security.Claims;

namespace FunctionalTests.Infraestructure
{
    internal static class Claims
    {
        public static readonly IEnumerable<Claim> UserWithPolicy = new[]
        {
            new Claim(
                type: ClaimTypes.NameIdentifier,
                value: "1",
                valueType: ClaimValueTypes.Integer32,
                issuer: "TestIssuer",
                originalIssuer: "OriginalTestIssuer"),
            new Claim(type: ClaimTypes.Name, value: "User")
        };

        public static readonly IEnumerable<Claim> User = new[]
        {
            new Claim(
                type: ClaimTypes.NameIdentifier,
                value: "2",
                valueType: ClaimValueTypes.Integer32,
                issuer: "TestIssuer2",
                originalIssuer: "OriginalTestIssuer2"),
            new Claim(type: ClaimTypes.Name, value: "User2")
        };
    }
}
