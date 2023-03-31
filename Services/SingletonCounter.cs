using System;
namespace SimpleMVC.Services
{
    public class SingletonCounter : ISingletonCounter
    {

        private int count { get; set; }

        public int GetCounter()
        {
            count++;
            return count;
        }
    }
}

