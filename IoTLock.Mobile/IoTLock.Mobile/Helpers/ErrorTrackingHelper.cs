﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLock.Mobile.Helpers
{
    public static class ErrorTrackingHelper
    {
        // callbacks for exception tracking
        public static Action<Exception, string> TrackException { get; set; }
            = (exception, message) => { };

        // callbacks for blocking UI error message
        public static Func<Exception, string, Task> GenericApiCallExceptionHandler { get; set; }
            = (ex, errorTitle) => Task.FromResult(0);
    }
}
