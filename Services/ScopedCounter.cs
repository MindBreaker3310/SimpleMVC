using System;
namespace SimpleMVC.Services
{
    public class ScopedCounter : IScopedCounter
    {

        private int count { get; set; }

        public int GetCounter()
        {
            count++;
            return count;
        }
    }
}

