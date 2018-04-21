using System.Linq;
using System.Security.Claims;

namespace CosmicBox.Auth {
    public static class ClaimsPrincipalExtensions {
        public static string GetIdentifier(this ClaimsPrincipal user) =>
            user.FindFirst(ClaimTypes.NameIdentifier).Value;
            
        public static bool CanDeleteBoxes(this ClaimsPrincipal user) =>
            user.Claims
                .SingleOrDefault(c => c.Type == "scope")
                .Value.Contains("delete:boxes");
    }
}