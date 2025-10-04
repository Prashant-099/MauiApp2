using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Service
{
    public class ToastService
    {
        public event Action<string, string>? OnShow;

        public void Show(string message, string type = "success")
        {
            OnShow?.Invoke(message, type);
        }
    }
}
