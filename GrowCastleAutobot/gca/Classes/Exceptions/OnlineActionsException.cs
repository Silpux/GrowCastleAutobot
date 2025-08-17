namespace gca.Classes.Exceptions
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
