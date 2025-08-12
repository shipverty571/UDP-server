using System.Text;
using System.Text.RegularExpressions;
using UDPModel.Exceptions;

namespace UDPModel.Utilities
{
    public static class MessageValidator
    {
        private const int MaxBytes = 512;

        /// <summary>
        /// [^\s:]+ — одна или более символов, кроме пробела (\s) и двоеточия (:)
        /// : - двоеточие-разделитель
        /// \-? - необязательный минус (для отрицательных чисел)
        /// \d+ - одна или более цифр (целая часть числа)
        /// (\.\d+)? - необязательная дробная часть: точка и одна или более цифр
        /// </summary>
        private const string MessageFormatRegex = @"^[^\s:]+:\-?\d+(\.\d+)?$";

        public static void IsValidMessage(string message)
        {
            var utf8Message = Encoding.UTF8.GetBytes(message);
            if (utf8Message.Length > MaxBytes)
            {
                throw new NotValidMessageException(message);
            }

            if (!Regex.IsMatch(message, MessageFormatRegex))
            {
                throw new NotValidMessageException(message);
            }
        }
    }
}