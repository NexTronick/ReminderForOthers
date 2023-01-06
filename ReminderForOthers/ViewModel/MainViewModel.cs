
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReminderForOthers.ViewModel;
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    string title;

    [ObservableProperty]
    DatePicker selectedDate = new DatePicker();

    [ObservableProperty]
    TimePicker selectedTime = new TimePicker();

    public MainViewModel() 
    {
        selectedDate.MinimumDate = DateTime.Today;
        selectedDate.MaximumDate = new DateTime(DateTime.Today.Year + 10, DateTime.Today.Month, DateTime.Today.Day);

    }

    [RelayCommand]
    async Task Record() 
    {
        try {
            await Permissions.RequestAsync<Permissions.Microphone>();

        }catch (Exception ex)
        {

        }
    }

    [RelayCommand]
    void SetReminder() 
    {

        if (!ValidateReminder())
        {
            return;
        }
        Console.WriteLine($"Title: {title} \nDate: {selectedDate.Date} Time: {selectedTime.Time} Time of Day: {DateTime.Now.TimeOfDay}");
    }

    private bool ValidateReminder() 
    {
        //validating the Text
        if (title.Trim().Equals("")) 
        {
            App.Current.MainPage.DisplayAlert("Cannot Set Reminder", "Title provided is not filled. Please fill in the field.", "Okay");
            return false;
        }

        //validating the date
        if (selectedDate.Date == DateTime.Today && selectedTime.Time <= DateTime.Now.TimeOfDay)
        {
            App.Current.MainPage.DisplayAlert("Cannot Set Reminder", "Time provided is not valid. Please choose another time.", "Okay");
            return false;
        }
        return true;
    }
}