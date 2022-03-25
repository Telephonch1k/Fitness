using System.ComponentModel.DataAnnotations;

namespace Fitness.ViewModels.Trainers
{
    public class CreateTrainersViewModel
    {
        [Required(ErrorMessage = "Введите Фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введите Имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите Отчество")]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }   
    }
}