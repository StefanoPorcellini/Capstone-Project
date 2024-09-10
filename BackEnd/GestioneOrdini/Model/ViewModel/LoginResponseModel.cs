namespace GestioneOrdini.Model.ViewModel
{
    public class LoginResponseModel
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Token { get; set; }
        public DateTime Expires { get; set; }

    }
}
