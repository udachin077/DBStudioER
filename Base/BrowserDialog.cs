using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Base
{
    public class BrowserDialog
    {
        private static OpenFileDialog openFileDialog;
        private static CommonOpenFileDialog commonOpenFileDialog;

        /// <summary>
        /// Заголовок окна.
        /// </summary>
        public string OpenFileTitle { get; set; }
        /// <summary>
        /// Строка фильтра файлов.
        /// </summary>
        public string OpenFileFilter { get; set; }

        public string OpenFileDirectory { get; set; }

        /// <summary>
        /// Открывает окно выбора файлов. Возвращает null, если файл не был выбран.
        /// </summary>
        /// <returns></returns>
        public string OpenFile()
        {
            openFileDialog = openFileDialog ?? new OpenFileDialog
            {
                CheckFileExists = true,
                FilterIndex = 1,
                Title = OpenFileTitle,
                Filter = OpenFileFilter,
                InitialDirectory = OpenFileDirectory
            };
            if (openFileDialog.ShowDialog() == false)
                return null;
            return openFileDialog.FileName;
        }

        /// <summary>
        /// Открывает окно выбора папки. Возвращает null, если папка не была выбрана.
        /// </summary>
        /// <returns></returns>
        public string OpenFolder()
        {
            commonOpenFileDialog = commonOpenFileDialog ?? new CommonOpenFileDialog();
            commonOpenFileDialog.Title = "Расположение проектов";
            commonOpenFileDialog.IsFolderPicker = true;
            commonOpenFileDialog.AddToMostRecentlyUsedList = false;
            commonOpenFileDialog.AllowNonFileSystemItems = false;
            commonOpenFileDialog.EnsureFileExists = true;
            commonOpenFileDialog.EnsurePathExists = true;
            commonOpenFileDialog.EnsureReadOnly = false;
            commonOpenFileDialog.EnsureValidNames = true;
            commonOpenFileDialog.Multiselect = false;
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return commonOpenFileDialog.FileName;
            }
            return null;
        }
    }
}