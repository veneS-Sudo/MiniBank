namespace Minibank.Web.Infrastructure.Authentication
{
    public class AuthPayload
    {
        public string iss { get; set; }
        public int nbf { get; set; }
        public int iat { get; set; }
        public int exp { get; set; }
    }
}