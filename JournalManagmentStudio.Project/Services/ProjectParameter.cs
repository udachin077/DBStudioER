using Base;

namespace JournalManagmentStudio.Project.Services
{
    /// <summary>
    /// Представляет параметры проекта
    /// </summary>
    public struct ProjectParameter
    {
        /// <summary>
        /// Имя программы
        /// </summary>
        public ProgramName Program;
        /// <summary>
        /// Имя проекта
        /// </summary>
        public string Name;
        /// <summary>
        /// Путь к датасет
        /// </summary>
        public string PathDataSet;
        /// <summary>
        /// Путь к проектируемой таблице
        /// </summary>
        public string PathTable;
    }
}
