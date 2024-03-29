using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Layouts;
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
    private FriendModel friendModel;

    //recording started
    private int autoStopSec;
    private bool recordStart;
    private DateTime startedTime;
    public MainViewModel()
    {
        selectedDate.MinimumDate = DateTime.Today;
        selectedDate.MaximumDate = new DateTime(DateTime.Today.Year + 10, DateTime.Today.Month, DateTime.Today.Day);
        selectedTime.Time = DateTime.Now.TimeOfDay;
        recordModel = new RecordModel();
        friendModel = new FriendModel();
        autoStopSec = 40;
        LoadFriends();

    }
    //to load the friendslist gotten method from friendsVM
    private async void LoadFriends()
    {
        LoginModel loginModel = new LoginModel();
        string currentUser = await loginModel.GetLogInCacheAsync();
        IDictionary<string, FriendRequest> friendsDic = await friendModel.GetFriendDictionaryAsync(currentUser);
        List<FriendRequest> friends = friendModel.ConvertToListFriendRequestObj(currentUser, friendsDic);
        ObserveFriendList = new ObservableCollection<FriendRequest>(friends);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveFriendList)));
    }

    [RelayCommand]
    async Task Record()
    {
        if (recordStart)
        {
            await Shell.Current.DisplayAlert("Media Record Failed!", "The media may Have already started the recording. Try hitting the Reset button, before clicking on Record.", "Okay");
            return;
        }
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

        recordStart = true;
        startedTime = DateTime.Now;
        //Task.Run(() => { AutoStopRecord(); });
        RunTaskRecord();
        await recordModel.RecordAudioAsync();

    }

    private async void RunTaskRecord()
    {
        Page currentPage = Shell.Current.CurrentPage;
        await Task.Run(() =>
        {
            while (recordStart)
            {
                //Thread.Sleep(1000);
                //Console.WriteLine("Running Task");
                if (!recordStart) { continue; }

                if (startedTime.AddSeconds(autoStopSec) < DateTime.Now)
                {
                    
                    try
                    {
                        StopRecord();
                        MainThread.BeginInvokeOnMainThread(async() =>
                        {
                            await AutoStopRecord();
                        });
                    }
                    catch (Exception ex)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlert("Error Has Occured", "Error occured in system, Please reopen Application!", "Okay");
                        });
                        Console.WriteLine(ex.Message);
                    }
                    
                    Console.WriteLine("Running This");
                    
                }
            }
        });

    }
    private async Task AutoStopRecord()
    {
        bool response = await Shell.Current.DisplayAlert("Recording Automatically Stopped!", "Recording exceeds " + autoStopSec + " seconds!\nWould you like to Keep or Discard Recording?", "Keep", "Discard");
        Console.WriteLine("Result: " + response);
        if (!response)
        {
            DisposeRecordAudio();
        }
    }

    [RelayCommand]
    async void StopRecord()
    {
        bool record = await recordModel.StopRecordAudioAsync();
        if (!record)
        {
            await Shell.Current.DisplayAlert("Media Record Failed!", "The media may Not Have started the recording. Try hitting the Reset button, before clicking on Record.", "Okay");
        }
        recordStart = false;
    }

    [RelayCommand]
    async void DisposeRecordAudio()
    {
        await recordModel.DisposeAudioAsync();
        recordStart = false;
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

        if (!ValidateReminder()) { return; }

        //set all the values for reminders
        Reminder reminder = new Reminder();
        reminder.UsernameFrom = username;
        reminder.UsernameTo = userTo;
        reminder.Title = title;
        reminder.PlayDateTime = selectedDate.Date.AddTicks(selectedTime.Time.Ticks);
        reminder.RecordPath = recordModel.GetRecordPath();
        reminder.ReminderCreationTime = DateTime.Now;
        reminder.HasPlayed = false;

        //store reminders
        ReminderModel reminderModel = new ReminderModel(reminder);
        bool stored = await reminderModel.StoreReminderAsync();
        if (stored)
        {


            userToIndex = 0;
            title = "";
            selectedDate.Date = DateTime.Now;
            selectedTime.Time = DateTime.Now.TimeOfDay;
            DisposeRecordAudio();
            await Shell.Current.GoToAsync("//Home//" + nameof(PersonalReminders));
            await App.Current.MainPage.DisplayAlert("Reminder Set", "Reminder is successfully set.", "Okay");
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
    public async Task CheckMyReminders()
    {
        await Shell.Current.GoToAsync("//Home//" + nameof(PersonalReminders));
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