using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReminderForOthers.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    string username;


    [ObservableProperty]
    string password;

    [ObservableProperty]
    bool checkBox;

    [RelayCommand]
    async void LoginUser()
    {
        //validate here
        if (username == "natraj" && password == "password1") {
           await Shell.Current.GoToAsync(".."); //back to MainPage
        }else
        {
            App.Current.MainPage.DisplayAlert("Verification Error", "Username or Password is wrong. Try again.", "Okay");
        }
        
    } 

    [RelayCommand]
    Task SignUpNow() => Shell.Current.GoToAsync(nameof(SignUp));
}