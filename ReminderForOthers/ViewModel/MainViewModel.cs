
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.AudioRecorder;
using Plugin.SimpleAudioRecorder;
using ReminderForOthers.Model;
using ReminderForOthers.View;

namespace ReminderForOthers.ViewModel;
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    string userTo;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    DatePicker selectedDate = new DatePicker();

    [ObservableProperty]
    TimePicker selectedTime = new TimePicker();

    //Recorder
    private RecordModel recordModel;

    public MainViewModel()
    {
        selectedDate.MinimumDate = DateTime.Today;
        selectedDate.MaximumDate = new DateTime(DateTime.Today.Year + 10, DateTime.Today.Month, DateTime.Today.Day);
        selectedTime.Time = DateTime.Now.TimeOfDay;
        recordModel = new RecordModel();
    }

    [RelayCommand]
    async Task Record()
    {
        //first ask for permissions
        PermissionsModel permissionsModel = new PermissionsModel();

        bool status = await permissionsModel.AskRequiredPermissionsAsync();

        if (!status)
        {
            await Shell.Current.DisplayAlert("Record Error", "Record cannot be done because Permissions are missing, please allow the permissions.", "Okay");
            return;
        }

        //record audio
        //record the voice
        bool record = await recordModel.RecordAudioAsync();
        if (!record)
        {
            await Shell.Current.DisplayAlert("Media Record Failed!", "The media may Have already started the recording. Try hitting the Reset button, before clicking on Record.", "Okay");
        }
    }

    [RelayCommand]
    async Task StopRecord()
    {
        bool record = await recordModel.StopRecordAudioAsync();
        if (!record)
        {
            await Shell.Current.DisplayAlert("Media Record Failed!", "The media may Not Have started the recording. Try hitting the Reset button, before clicking on Record.", "Okay");
        }
    }

    [RelayCommand]
    async Task DisposeRecordAudio()
    {
        await recordModel.DisposeAudioAsync();
    }

    [RelayCommand]
    void PlayRecordedAudio()
    {
        bool isPlaying = recordModel.PlayAudio();
        if (!isPlaying)
        {
            DisplayMediaErrorAlert();
        }
    }

    [RelayCommand]
    void PauseRecordedAudio()
    {
        bool isPaused = recordModel.PauseAudio();
        if (!isPaused)
        {
            DisplayMediaErrorAlert();
        }
    }

    private void DisplayMediaErrorAlert()
    {
        Shell.Current.DisplayAlert("Media Error", "Voice Memo has not been recorded. Please record voice memo.", "Okay");
    }

    [RelayCommand]
    async Task SetReminder()
    {
        //if this person is logged
        string username = await GetUserLoggedInAsync();
        if (string.IsNullOrEmpty(username)) { await Shell.Current.GoToAsync(nameof(Login)); }

        if (!ValidateReminder()) { return; }
        bool userExists = await UserExists();
        if (!userExists)
        {
            await App.Current.MainPage.DisplayAlert("Cannot Set Reminder", "Recipent Username does not exists. Please fill in correct Recipent Username.", "Okay");
            return;
        }

        ReminderModel reminderModel = new ReminderModel(username, userTo, title, selectedDate.Date, selectedTime.Time, recordModel.GetRecordPath());
        bool stored = await reminderModel.StoreReminderAsync();
        if (stored)
        {

            await App.Current.MainPage.DisplayAlert("Reminder Set", "Reminder is successfully set.", "Okay");
            userTo = "";
            title = "";
            selectedDate.Date = DateTime.Now;
            selectedTime.Time = DateTime.Now.TimeOfDay;
            await DisposeRecordAudio();
        }
        Console.WriteLine($"Title: {title} \nDate: {selectedDate.Date} Time: {selectedTime.Time} Time of Day: {DateTime.Now.TimeOfDay}");
    }
    private async Task<bool> UserExists()
    {
        SignUpModel signUpModel = new SignUpModel();
        return await signUpModel.DoesUserNameExits(userTo);
    }

    private bool ValidateReminder()
    {

        //validate the username and title
        if (!CheckStringValue(userTo, "Recipent Username")) { return false; }
        if (!CheckStringValue(title, "Title")) { return false; }


        //validating the date and time
        if (selectedTime.Time <= DateTime.Now.TimeOfDay)
        {
            App.Current.MainPage.DisplayAlert("Cannot Set Reminder", "Time provided is not valid. Please choose another time.", "Okay");
            return false;
        }

        if (!recordModel.HasRecordedAudio())
        {
            App.Current.MainPage.DisplayAlert("Cannot Set Reminder", "Voice Memo has not been recorded. Please record voice memo.", "Okay");
            return false;
        }

        return true;
    }
    //helper method for checking string
    private bool CheckStringValue(string val, string valName)
    {
        if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
        {
            App.Current.MainPage.DisplayAlert("Cannot Set Reminder", $"{valName} is not provided. Please fill in the field.", "Okay");
            return false;
        }
        return true;
    }

    //to check if user is logged in 
    private async Task<string> GetUserLoggedInAsync()
    {
        LoginModel loginModel = new LoginModel();
        string cache = await loginModel.GetLogInCacheAsync();
        return cache;
    }
    [RelayCommand]
    public async Task GotoLoginPageAsync()
    {
        //move to login page
        if (string.IsNullOrEmpty(await GetUserLoggedInAsync()))
        {
            await Shell.Current.GoToAsync(nameof(Login));
        }
        //await Shell.Current.GoToAsync(nameof(Login));
    }

    [RelayCommand]
    public async Task LogoutUserAsync()
    {
        LoginModel loginModel = new LoginModel();
        loginModel.Logout();
        await GotoLoginPageAsync();
    }
    [RelayCommand]
    public async Task CheckMyReminders()
    {
        await Shell.Current.GoToAsync(nameof(PersonalReminders));
    }

    public async Task<List<Reminder>> GetRemindersAsync()
    {
        ReminderModel reminderModel = new ReminderModel();
        IDictionary<string, Reminder> currentUserReminders = await reminderModel.GetRemindersForUserAsync(await GetUserLoggedInAsync());
        if (currentUserReminders == null) { return new List<Reminder>(); }

        Reminder[] arrangedReminders = RearrangeDictionary(currentUserReminders);

        return arrangedReminders.ToList();
    }
    //helper method to re arrange according to date and time
    private Reminder[] RearrangeDictionary(IDictionary<string, Reminder> reminders)
    {
        Reminder[] reminderArr = ConvertToReminderArr(reminders);

        for (int i = 0; i < reminderArr.Length; i++)
        {
            for (int j = reminderArr.Length - 1; j > i; j--)
            {
                Console.WriteLine($"Before switch, Reminder i:{reminderArr[i].Id}, Reminder j: {reminderArr[j].Id}");
                if (reminderArr[i].PlayDateTime >= reminderArr[j].PlayDateTime)
                {

                    Reminder temp = reminderArr[i];
                    reminderArr[i] = reminderArr[j];
                    reminderArr[j] = temp;
                    Console.WriteLine($"After switch, Reminder i:{reminderArr[i].Id}, Reminder j: {reminderArr[j].Id}");

                }
            }
        }


        return reminderArr;
    }

    private Reminder[] ConvertToReminderArr(IDictionary<string, Reminder> reminders)
    {
        Reminder[] reminderArr = new Reminder[reminders.Count];
        int i = 0;
        foreach (var reminder in reminders)
        {
            reminderArr[i++] = reminder.Value;
        }
        return reminderArr;
    }
}