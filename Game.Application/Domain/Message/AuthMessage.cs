using System;
using System.Collections.Generic;

namespace Game.Application.Domain.Message
{
    public class RegisterReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class LoginReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ConnectionId { get; set; }
    }

    public class ExecutionRes<T>
    {
        public DateTime Timestamp { get; set; }
        public string ErrorCode { get; set; }
        public string Error { get; set; }
        public bool Success { get; set; }
        public List<string> Validates { get; set; }
        public T Result { get; set; }
    }
}
