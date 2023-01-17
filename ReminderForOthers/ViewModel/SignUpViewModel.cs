using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;
using System.Net.Mail;
using ReminderForOthers.View;
using System.Diagnostics.Tracing;

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

    public SignUpViewModel()
    {

        //set dates
        birthDate = new DatePicker();
        birthDate.MaximumDate = DateTime.Today;
        birthDate.MinimumDate = new DateTime(DateTime.Today.Year - 100, 1, 1); //support until 100 years old}
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
            ShowError("Field Incomplete", "First Name is not filled in!");
            return false;
        }

        lastName = ValidateStringVariable(LastName);
        if (LastName == null)
        {
            ShowError("Field Incomplete", "Last Name is not filled in!");
            return false;
        }
        //validate the date using Nullable
        DateTime? bDay = birthDate.Date;
        if (!bDay.HasValue)
        {
            ShowError("Field Incomplete", "Date is not filled in.");
            return false;
        }

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
        if (userStored == 1) {
            await Shell.Current.DisplayAlert("User Registered!", "User has been registered, Login Now.", "Okay");
        }
       await HasAccountFromSignUpNext();
    }

    [RelayCommand]
    async Task HasAccountFromSignUpNext()
    {
        await Shell.Current.GoToAsync("../..");
    }


    private async Task<int> SetUser()
    {
        SignUpModel signUpModel = new SignUpModel(lastName, firstName, birthDate.Date.ToString(), username, new MailAddress(email), password);
        int stored = await signUpModel.StoreUserAsync();
        //test json deserilize
        //Task<IDictionary<string,User>> userData= signUpModel.GetLocalUsers();
        return stored;
    }


    bool ValidateAll()
    {
        bool isValidate = false;
        isValidate = IsUserValid();
        isValidate = IsEmailValid();
        isValidate = IsPasswordValid();
        Console.WriteLine($"Alert Displayed: {!isValidate}");
        return isValidate;
    }

    bool IsUserValid()
    {
        username = ValidateStringVariable(username);
        if (username == null)
        {
            ShowError("Field Incomplete", "Username is not filled in!");
            return false;
        }

        //add validation with the database using the model here.

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
            App.Current.MainPage.DisplayAlert("Email not recognized", ex.Message + "\nThe e-mail address should look like example@gmail.com.", "Okay");
            return false;
        }
    }

    bool IsPasswordValid()
    {
        if (string.IsNullOrEmpty(password)
            || password.Length < 8
            || !password.Equals(rePassword))
        {
            App.Current.MainPage.DisplayAlert("Password not recognized", "Either one of the following is the cause: \nPassword is not field filled. \nPassword is not at least 8 characters. \nConfirm Password is not same as Password.", "Okay");
            return false;
        }

        return true;
    }


}