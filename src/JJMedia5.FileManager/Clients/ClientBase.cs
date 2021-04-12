using JJMedia5.Core.Models;
using System.Net.Http;

namespace JJMedia5.FileManager.Clients {

    public abstract class ClientBase {
        protected readonly string _userName;
        protected readonly string _address;
        protected readonly string _password;
        protected readonly HttpClient _client;

        public ClientBase(HttpClient client, BasicAuthEndPoint auth) {
            _client = client;
            _userName = auth.Username;
            _password = auth.Password;
            _address = auth.Address;
        }
    }
}