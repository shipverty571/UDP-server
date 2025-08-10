using System;

namespace UDPModel.Exceptions
{
    public class NotValidMessageException : Exception
    {
        public NotValidMessageException(string message)
            : base($"Ошибка формата: {message}") { }
    }
}