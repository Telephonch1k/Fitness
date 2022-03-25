namespace Fitness.ViewModels.Trainers
{
    public class SortTrainersViewModel
    {
        public TrainersSortState LastNameSort { get; private set; }
        public TrainersSortState FirstNameSort { get; private set; }
        public TrainersSortState PatronymicSort { get; private set; }
        public TrainersSortState Current { get; private set; }     // текущее значение сортировки

        public SortTrainersViewModel(TrainersSortState sortOrder)
        {
            LastNameSort = sortOrder == TrainersSortState.LastNameAsc ?
                TrainersSortState.LastNameDesc : TrainersSortState.LastNameAsc;

            FirstNameSort = sortOrder == TrainersSortState.FirstNameAsc ?
                TrainersSortState.FirstNameDesc : TrainersSortState.FirstNameAsc;

            PatronymicSort = sortOrder == TrainersSortState.PatronymicAsc ?
                TrainersSortState.PatronymicDesc : TrainersSortState.PatronymicAsc;
            Current = sortOrder;
        }
    }
}