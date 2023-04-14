using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;
using ReminderForOthers.View;
using System.ComponentModel;

namespace ReminderForOthers.ViewModel;

public partial class LoginViewModel : ObservableObject, INotifyPropertyChanged
{
    [ObservableProperty]
    string username;

    [ObservableProperty]
    string password;

    public event PropertyChangedEventHandler PropertyChanged;

    private bool _checkBox;
    public bool CheckBox
    {
        get => _checkBox;
        set
        {
            if (_checkBox != value)
            {
                CheckBoxToggled(value);
            }
        }
    }


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
        int loginValid = await loginModel.ValidateUserLogin(username, password, _checkBox);
        Console.WriteLine($"Login Valid: {loginValid}");
        if (loginValid == 1)
        {
            //store to cache for logged In status true, username of the user. in a new method
            await Shell.Current.GoToAsync("//Home//" + nameof(PersonalReminders));
            await Shell.Current.DisplayAlert("Welcome "+username, "Please make sure to turn on background notificaiton in the settings to be able to play reminders in background.", "Okay");
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Validation Error", "Username or Password is wrong. Try again.", "Okay");
        }

    }

    [RelayCommand]
    void CheckBoxTick()
    {
        CheckBoxToggled(!_checkBox);
    }

    private void CheckBoxToggled(bool check) 
    {
        _checkBox = check;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckBox)));

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