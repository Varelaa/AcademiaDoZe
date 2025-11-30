using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class ColaboradorPage : ContentPage
    {
        private readonly ColaboradorViewModel _viewModel;

        public ColaboradorPage(ColaboradorViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        
        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            if (BindingContext is ColaboradorViewModel vm)
                await vm.SaveAsync();
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            if (BindingContext is ColaboradorViewModel vm)
                await vm.CancelAsync();
        }
    }
}
