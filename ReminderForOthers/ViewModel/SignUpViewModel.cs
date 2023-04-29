using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;
using System.Net.Mail;
using ReminderForOthers.View;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace ReminderForOthers.ViewModel;

public partial class SignUpViewModel : ObservableObject
{
    [ObservableProperty]
    string firstName;

    [ObservableProperty]
    string lastName;

    [ObservableProperty]
    DatePicker birthDate;

    [ObservableProperty]
    string username;

    [ObservableProperty]
    string email;

    [ObservableProperty]
    string password;

    [ObservableProperty]
    string rePassword;

    private SignUpSingleton signUpSingleton;
    public SignUpViewModel()
    {

        //set dates
        birthDate = new DatePicker();
        birthDate.MaximumDate = new DateTime(DateTime.Today.Year - 3, DateTime.Today.Month, DateTime.Today.Day); //minimum 3 years old.
        birthDate.MinimumDate = new DateTime(DateTime.Today.Year - 100, DateTime.Today.Month, DateTime.Today.Day); //support until 100 years old}
        signUpSingleton = new SignUpSingleton();
    }

    //sign up page
    [RelayCommand]
    async Task NextPage()
    {
        //show error
        if (!ValidateFirstPage()) { return; }
        await Shell.Current.GoToAsync(nameof(SignUpNext));

    }

    //to validate the first page
    private bool ValidateFirstPage()
    {

        //validate variables
        firstName = ValidateStringVariable(firstName);
        if (firstName == null)
        {
            ShowError("Field Incomplete", "One of the followings: \nFirst Name is not filled in! \nFirst Name is not between 1-32 characters (no digit or speical character is allowed).");
            return false;
        }

        lastName = ValidateStringVariable(LastName);
        if (LastName == null)
        {
            ShowError("Field Incomplete", "One of the followings: \nLast Name is not filled in! \nLast Name is not between 1-32 characters (no digit or speical character is allowed).");
            return false;
        }
        //validate the date using Nullable
        DateTime? bDay = birthDate.Date;
        if (!bDay.HasValue)
        {
            ShowError("Field Incomplete", "Date is not filled in.");
            return false;
        }
        signUpSingleton.birthDate = bDay.ToString();
        signUpSingleton.firstName = firstName;
        signUpSingleton.lastName = lastName;
        Console.WriteLine("signUpSingleton.lastName : "+ signUpSingleton.lastName);
        return true; 
    }

    //to show the error for the phone 
    private void ShowError(string title, string msg)
    {
        Shell.Current.DisplayAlert(title, msg, "Okay");
    }

    //-1 ->failed , 1->passed all checks, 0-> value has to be trimed.
    string ValidateStringVariable(string val)
    {
        if (string.IsNullOrEmpty(val)) { return null; }
        string temp = val.Trim();
        if (temp.Equals("")) { return null; }
        if (temp.Length > 32) { return null; }
        string regex = @"^[A-Za-z\s]*$";
        Match match = Regex.Match(temp, regex);
        if (!match.Success) { return null; } //checks for only letters
        return temp;

    }

    [RelayCommand]
    async Task HasAccountFromSignUp()
    {
        await Shell.Current.GoToAsync("..");
    }

    //Sign up next page

    //for registering the user
    [RelayCommand]
    async Task RegisterUser()
    {

        //validate 
        if (!ValidateAll()) return;

        //set to database
        int userStored = await SetUser();
        if (userStored == -1)
        {
            await Shell.Current.DisplayAlert("User Registration Failed!", "Email already exists, please use new email.", "Okay");
            return;
        }
        else if (userStored == 0)
        {
            await Shell.Current.DisplayAlert("User Registration Failed!", "Username already exists, please use new username.", "Okay");
            return;
        }

        //move to login page
        if (userStored == 1)
        {
            await Shell.Current.DisplayAlert("User Registered!", "User has been registered, Login Now.", "Okay");
        }
        ClearSignUpData();
        await HasAccountFromSignUpNext();
    }

    //helper method to empty the data
    private void ClearSignUpData() {
        firstName = "";
        lastName = "";
        birthDate.Date = birthDate.MaximumDate;
        username = "";
        email = "";
        password = "";
        rePassword = "";
        signUpSingleton.ClearAllData();
    }

    [RelayCommand]
    async Task HasAccountFromSignUpNext()
    {
        await Shell.Current.GoToAsync("../..");
    }


    private async Task<int> SetUser()
    {
        SignUpModel signUpModel = new SignUpModel(signUpSingleton.lastName, signUpSingleton.firstName, signUpSingleton.birthDate, username, new MailAddress(email), password);
        int stored = await signUpModel.StoreUserAsync();
        //test json deserilize
        //Task<IDictionary<string,User>> userData= signUpModel.GetLocalUsers();
        ClearSignUpData();
        return stored;
    }


    bool ValidateAll()
    {

        if (!IsUserValid()) { return false; }
        if (!IsEmailValid()) { return false; }
        if (!IsPasswordValid()) { return false; }
        return true;
    }

    bool CheckForSpecialChar(string val)
    {
        return val.Any(ch => !char.IsLetterOrDigit(ch));
    }

    bool IsUserValid()
    {
        if (string.IsNullOrEmpty(username))
        {
            ShowError("Field Incorrect", "Username is not filled in!");
            return false;
        }
        username = username.Trim();
        if (username.Length < 4 || username.Length > 16)
        {
            ShowError("Field Incorrect", "Username is not in the range of 4-16 characters!");
            return false;
        }
        if (CheckForSpecialChar(username)) 
        {
            ShowError("Field Incorrect", "Username should have Letters and Numbers only.");
            return false;
        }
        return true;
    }

    bool IsEmailValid()
    {
        try
        {
            MailAddress mail = new MailAddress(email);
            return true;
        }
        catch (Exception ex)
        {
            ShowError("Email not recognized", $"{ex.Message}\nThe e-mail address should look like example@gmail.com.");
            return false;
        }
    }

    bool IsPasswordValid()
    {
        if (string.IsNullOrEmpty(password)
            || password.Length < 8
            || !password.Equals(rePassword))
        {
            ShowError("Password not recognized", "Either one of the following is the cause: \nPassword is not filled in. \nPassword is not at least 8 characters. \nConfirm Password is not same as Password.");
            return false;
        }
        string regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,16}$";
        Match match = Regex.Match(password, regex);
        if (!match.Success) {
            ShowError("Password not recognized", "Password is not at least 8-16 Characters, 1 Upper case and 1 Lower case Letter, and 1 Number");
            return false;
        }

        return true;
    }

    //method used by settings VM to validate
    public bool ValidateUsersDetails(User user) 
    {
        firstName = user.FirstName;
        lastName = user.LastName;
        birthDate.Date = DateTime.Parse(user.BirthDate);
        email = user.Email;
        username = user.Username;
        if (!ValidateFirstPage()) { return false; }
        if (!IsUserValid()) { return false; }
        if (!IsEmailValid()) { return false; }
        return true;
    }


    //default navigations


    [RelayCommand]
    async void GoBack() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    async void GotoNotification()
    {
        await Shell.Current.DisplayAlert("Login Required", "Login is required for accessing Notifications.", "Okay");
    }
}