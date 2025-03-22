public class TokenResponse
{
    public string Token { get; set; } // Oluşturulan JWT token'ı
    public double TokenSuresi { get; set; } // Token'ın geçerlilik süresi (saniye cinsinden)
    public double TokenKalanSure { get; set; } // Token'ın kalan geçerlilik süresi (saniye cinsinden)
    public bool TokenGecerliMi { get; set; } // Token geçerli mi?
    public bool IslemYapabilirMi { get; set; } // Token ile işlem yapılabilir mi?
}
