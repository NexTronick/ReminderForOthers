
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.AudioRecorder;
using Plugin.SimpleAudioRecorder;
using ReminderForOthers.Model;
using ReminderForOthers.View;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReminderForOthers.ViewModel;
public partial class MainViewModel : ObservableObject, INotifyPropertyChanged
{
    [ObservableProperty]
    int userToIndex;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    DatePicker selectedDate = new DatePicker();

    [ObservableProperty]
    TimePicker selectedTime = new TimePicker();

    //friend 
    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<FriendRequest> ObserveFriendList { get; set; } = new ObservableCollection<FriendRequest>();

    //Recorder
    private RecordModel recordModel;

    public MainViewModel()
    {
        selectedDate.MinimumDate = DateTime.Today;
        selectedDate.MaximumDate = new DateTime(DateTime.Today.Year + 10, DateTime.Today.Month, DateTime.Today.Day);
        selectedTime.Time = DateTime.Now.TimeOfDay;
        recordModel = new RecordModel();
        LoadFriends();

    }
    //to load the friendslist gotten method from friendsVM
    private async void LoadFriends() 
    {
        FriendViewModel friendVM = new FriendViewModel();
        List<FriendRequest> friends = await friendVM.LoadFriendListAsync();

        ObserveFriendList = new ObservableCollection<FriendRequest>(friends);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveFriendList)));
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
        string userTo = ObserveFriendList.ToArray()[userToIndex].FriendUsername;

        //if (string.IsNullOrEmpty(username)) { await Shell.Current.GoToAsync(nameof(Login)); }

        if (!ValidateReminder()) { return; }
        //bool userExists = await UserExists();
        //if (!userExists)
        //{
        //    await App.Current.MainPage.DisplayAlert("Cannot Set Reminder", "Recipent Username does not exists. Please fill in correct Recipent Username.", "Okay");
        //    return;
        //}

        ReminderModel reminderModel = new ReminderModel(username, userTo, title, selectedDate.Date, selectedTime.Time, recordModel.GetRecordPath());
        bool stored = await reminderModel.StoreReminderAsync();
        if (stored)
        {

            await App.Current.MainPage.DisplayAlert("Reminder Set", "Reminder is successfully set.", "Okay");
            userToIndex = 0;
            title = "";
            selectedDate.Date = DateTime.Now;
            selectedTime.Time = DateTime.Now.TimeOfDay;
            await DisposeRecordAudio();
        }
        //Console.WriteLine($"Title: {title} \nDate: {selectedDate.Date} Time: {selectedTime.Time} Time of Day: {DateTime.Now.TimeOfDay}");
    }

    private bool ValidateReminder()
    {

        //validate the username and title
        //if (!CheckStringValue(userTo, "Recipent Username")) { return false; }
        if (!CheckStringValue(title, "Title")) { return false; }


        //validating the date and time
        long selectedDateTime = selectedDate.Date.Ticks + selectedTime.Time.Ticks;
        if (selectedDateTime <= DateTime.Now.Ticks)
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
    public async Task<string> GetUserLoggedInAsync()
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
            await Shell.Current.GoToAsync("//Login");
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
        await Shell.Current.GoToAsync("//Home//"+nameof(PersonalReminders));
    }

    

    //default navigations

    [RelayCommand]
    async void GotoHome()
    {
        await Shell.Current.GoToAsync(nameof(PersonalReminders)); //home set to be Personal Reminders
    }

    [RelayCommand]
    void GotoSetReminder()
    {
        Shell.SetNavBarIsVisible(Shell.Current.CurrentPage, false); //home set to be Personal Reminders
    }

    [RelayCommand]
    async void GotoSettings()
    {
        await Shell.Current.GoToAsync(nameof(Settings));
    }

    [RelayCommand]
    async void GotoFriends()
    {
        await Shell.Current.GoToAsync(nameof(Friend)); //home set to be Personal Reminders
    }

    [RelayCommand]
    async void GotoNotification()
    {
        //await Shell.Current.GoToAsync("..");
        await Shell.Current.DisplayAlert("Notification", "Notification is to be added.", "Okay");
    }
}