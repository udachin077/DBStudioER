using System.Windows;

namespace Base
{
    /// <summary>
    /// Представляет класс сообщения об ошибке
    /// </summary>
    public static class ErrorMessage
    {
        /// <summary>
        /// Выводит сообщение об ошибке.
        /// </summary>
        /// <param name="message">Сообщение ошибки</param>
        public static void Show(string message, string caption = "Ошибка")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Представляет класс предупреждения
    /// </summary>
    public static class WarningMessage
    {
        /// <summary>
        /// Выводит предупреждение и возвращает true, если был нажат Yes.
        /// </summary>
        /// <param name="message">Текст предупреждения</param>
        /// <returns></returns>
        public static bool Show(string message, string caption = "Предупреждение")
        {
            MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                return true;

            return false;
        }
    }
}
