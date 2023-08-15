using System.ComponentModel.DataAnnotations;

namespace Yamaha.ESB.Authentication.Models
{
    public class ValidateModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Uid { get; set; }
        public int StatusCode { get; set; }
    }
}