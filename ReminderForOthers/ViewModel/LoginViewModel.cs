using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;

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
        if (!CheckValidStr()) {
            await App.Current.MainPage.DisplayAlert("Verification Error", "Username or Password is not filled in. Try again.", "Okay");
            return;
        }

        LoginModel loginModel = new LoginModel();
        int loginValid = await loginModel.ValidateUserLogin(username,password);
        Console.WriteLine($"Login Valid: {loginValid}");
        if (loginValid == 1)
        {
            //store to cache for logged In status true, username of the user. in a new method
            //logged in
            await Shell.Current.GoToAsync(".."); //back to MainPage
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Validation Error", "Username or Password is wrong. Try again.", "Okay");
        }
        
    }

    [RelayCommand]
    void CheckBoxTick() 
    {
        CheckBox = !CheckBox;
    }
    private bool CheckValidStr() 
    {
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }

    [RelayCommand]
    Task SignUpNow() => Shell.Current.GoToAsync(nameof(SignUp));
}