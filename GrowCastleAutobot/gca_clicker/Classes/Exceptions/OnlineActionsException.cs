using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes.Exceptions
{
    public class OnlineActionsException : Exception
    {
        private string info;
        public string Info => info;
        public OnlineActionsException(string info)
        {
            this.info = info;
        }
    }
}
