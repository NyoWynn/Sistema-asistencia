using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string Name { get; set; }

        [Display(Name = "¿Es Administrador?")]
        public bool IsAdmin { get; set; }
    }
}