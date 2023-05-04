using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderForOthers.Model
{
    public class PermissionsModel 
    {

        public enum PermissionsType { MIC, S_READ, S_WRITE};

        public async Task<bool> AskRequiredPermissionsAsync() 
        {
            bool pMic = await AskForPermissionsAsync(PermissionsType.MIC);
            bool pRead = await AskForPermissionsAsync(PermissionsType.S_READ);
            bool pWrite = await AskForPermissionsAsync(PermissionsType.S_WRITE);
            return (pMic == true? (pRead == true? (pWrite == true? true : false): false) : false);
        }

        private async Task<bool> AskForPermissionsAsync(PermissionsType type)
        {
            //permission requirement description (why need permision?)
            string pDesc = "";
            //default status for permission
            var status = PermissionStatus.Unknown;

            switch (type) 
            {
                case PermissionsType.MIC:
                    status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
                    if (status == PermissionStatus.Granted) { return true; }
                    pDesc = "Microphone is required to record the voice memo in this application. Please go to device settings to allow Microphone permission.";
                    status = await Permissions.RequestAsync<Permissions.Microphone>(); 
                    break;

                case PermissionsType.S_READ:
                    status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    if (status == PermissionStatus.Granted) { return true; }
                    pDesc = "Storage or File is required to store the voice memo. Please go to device settings to allow Storage or File Permissions.";
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    break;

                case PermissionsType.S_WRITE:
                    status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                    if (status == PermissionStatus.Granted) { return true; }
                    pDesc = "Storage or File is required to store the voice memo. Please go to device settings to allow Storage or File Permissions.";
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    break;
            }
            //if its not granted then return false
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Permission Required", pDesc, "Okay");
                return false;
            }

            return true;
        }
        
    }
}
