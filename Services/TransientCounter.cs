using System;
namespace SimpleMVC.Services
{
    public class TransientCounter : ITransientCounter
    {

        private int count { get; set; }

        public int GetCounter()
        {
            count++;
            return count;
        }
    }
}

