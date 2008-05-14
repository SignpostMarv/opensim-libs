namespace TestApp.Models
{
    public class User
    {
        private string _id;
        private string _userName;
        private string _password;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}
