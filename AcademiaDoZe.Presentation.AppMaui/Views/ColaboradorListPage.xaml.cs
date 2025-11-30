using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class ColaboradorListPage : ContentPage
    {
        private readonly ColaboradorListViewModel _viewModel;

        public ColaboradorListPage(ColaboradorListViewModel vm)
        {
            InitializeComponent();
            BindingContext = _viewModel = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        
        private async void OnNovoClicked(object sender, EventArgs e)
        {
            
            await Shell.Current.GoToAsync(nameof(ColaboradorPage));
        }
    }
}
