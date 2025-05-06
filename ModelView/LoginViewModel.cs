namespace SuperMarketMangementSystem.ModelView
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email {  get; set; }
        [DataType(DataType.Password)]
        public string password { get; set; }
        public bool RemeberMe { get; set; }

    }
}
