namespace BackEnd.Model
{
    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string GrantType { get; set; }
    }
}
