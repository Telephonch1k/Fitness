using Fitness.Models.Data;
using Fitness.ViewModels.Trainers;
using System.Collections.Generic;

namespace Fitness.ViewModels.Trainers
{
    public class IndexTrainersViewModel
    {
        public IEnumerable<Trainer> Trainers { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterTrainersViewModel FilterTrainersViewModel { get; set; }
        public SortTrainersViewModel SortTrainersViewModel { get; set; }
    }
}