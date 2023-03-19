using System.ComponentModel.DataAnnotations;

namespace JuanProject.ViewModels
{
    public class LoginVM
    {
        [Required, StringLength(100)]
        public string? UsernameorEmail { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
