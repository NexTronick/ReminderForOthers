using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderForOthers.Services
{
    public interface IForegroundService
    {
        void Start();
        void Stop();
        bool IsForegroundServiceRunning();
    }
}
