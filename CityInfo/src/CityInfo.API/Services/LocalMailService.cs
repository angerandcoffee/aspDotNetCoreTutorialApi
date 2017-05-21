﻿using System.Diagnostics;

namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private string _mailTo = "admin@dummycompany.doh";
        private string _mailFrom = "noreplay@dummycompany.doh";

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail sent from {_mailFrom} to {_mailTo}, with LocalMailService");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}
