namespace GestioneOrdini.Model.ViewModel
{
    public class LoginResponseModel
    {
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public required UserAuthViewModel UserAuth { get; set; }


    }
}
