using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;
using ReminderForOthers.View;

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

        //if filed is filled in
        if (!CheckValidStr())
        {
            await App.Current.MainPage.DisplayAlert("Verification Error", "Username or Password is not filled in. Try again.", "Okay");
            return;
        }

        LoginModel loginModel = new LoginModel();
        int loginValid = await loginModel.ValidateUserLogin(username, password, checkBox);
        Console.WriteLine($"Login Valid: {loginValid}");
        if (loginValid == 1)
        {
            //store to cache for logged In status true, username of the user. in a new method
            await Shell.Current.GoToAsync("//Home");
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Validation Error", "Username or Password is wrong. Try again.", "Okay");
        }

    }

    [RelayCommand]
    void CheckBoxTick()
    {
        checkBox = !checkBox;
    }
    private bool CheckValidStr()
    {
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }

    [RelayCommand]
    async void SignUpNow() => await Shell.Current.GoToAsync(nameof(SignUp));

    [RelayCommand]
    async void GotoNotification()
    {
        await Shell.Current.DisplayAlert("Login Required", "Login is required for accessing Notifications.", "Okay");
    }
}