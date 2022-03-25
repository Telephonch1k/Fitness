using Fitness.Models.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Fitness.ViewModels.Trainers
{
    public class FilterTrainersViewModel
    {
        public string SelectedLastName { get; private set; }    // введенная Фамилия
        public string SelectedFirstName { get; private set; }    // введенное Имя
        public string SelectedPatronymic { get; private set; } // введенное Отчество




        public FilterTrainersViewModel(string LastName, string FirstName, string Patronymic)
        {
            SelectedLastName = LastName;
            SelectedFirstName = FirstName;
            SelectedPatronymic = Patronymic;
        }
    }
}