using eAgenda.Core.Dominio.ModuloAutenticacao;

namespace eAgenda.WebApi.Identity;

public static class RefreshTokenCookieService
{
    private static readonly string nome = "eAgenda.RefreshToken";

    public static void Set(HttpResponse response, RefreshToken token)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = token.ExpiraEmUtc
        };

        response.Cookies.Append(nome, token.TokenHash, options);
    }

    public static void Clear(HttpResponse response)
    {
        response.Cookies.Delete(nome, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
    }

    public static string? Get(HttpRequest request)
    {
        return request.Cookies[nome];
    }
}
