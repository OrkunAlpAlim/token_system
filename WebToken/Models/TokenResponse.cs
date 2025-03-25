namespace WebToken.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public double TokenSuresi { get; set; }
        public double TokenKalanSure { get; set; }
        public bool TokenGecerliMi { get; set; }
        public bool IslemYapabilirMi { get; set; }
        public DateTime TokenIssuedAt { get; set; } 
    }
}
