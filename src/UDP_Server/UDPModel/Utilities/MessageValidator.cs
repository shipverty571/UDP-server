using System.Text;
using System.Text.RegularExpressions;

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

        public static bool IsValidMessage(string message)
        {
            var messageLength = Encoding.UTF8.GetBytes(message).Length;
            if (messageLength > MaxBytes)
            {
                return false;
            }

            if (!Regex.IsMatch(message, MessageFormatRegex))
            {
                return false;
            }

            return true;
        }
    }
}