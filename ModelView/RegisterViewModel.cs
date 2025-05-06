namespace SuperMarketMangementSystem.ModelView
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Name Is required")]
        [MinLength(2,ErrorMessage ="MinLength Of Letters Should Be Larger Than 1")]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Enter Valid Email")]
        public string Email { get; set; }
        
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Compare("password")]
        [Required(ErrorMessage ="Enter Match Password")]
        [Display(Name="Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage ="Enter Your Avalaible Phone Number")]
        [MaxLength(11 , ErrorMessage =("Your Max Number You Can Enter 11 Number"))]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Enter Your Another Avalaible Phone Number")]
        [MaxLength(11, ErrorMessage = ("Your Max Number You Can Enter 11 Number"))]
        public string AnotherPhoneNumber { get; set; }

        [Required(ErrorMessage = "Enter Your Avalaible Phone Number")]
        [Length(3,50 ,ErrorMessage =("You Should Write Addres Between 3 To 50 Letters"))]
        public string Address {  get; set; }


        
    }
}
