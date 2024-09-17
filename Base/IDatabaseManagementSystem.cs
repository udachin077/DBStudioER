using System;
using System.Data;

namespace Base
{
    public interface IDatabaseManagementSystem
    {
        event Action<int> CounterChanged;
        /// <summary>
        /// Выполняет подключение к базе данных.
        /// </summary>
        /// <param name="ds">DataSet для заполнения</param>
        void Connect(ref DataSet ds);
        /// <summary>
        /// Создает набор данных DataSet для проектов.
        /// </summary>
        /// <param name="ds">DataSet для заполнения</param>
        void CreateDataSet();
        /// <summary>
        /// Выполняет добавление записей в журнал.
        /// </summary>
        /// <param name="ds">DataSet для заполнения</param>
        void Merge(ref DataTable journal);
        /// <summary>
        /// Не реализовано
        /// </summary>
        /// <param name="ds">DataSet для заполнения</param>
        void QueryExecute(string query);
        /// <summary>
        /// Выполняет обновление записей.
        /// </summary>
        /// <param name="ds">DataSet для заполнения</param>
        void Update(ref DataSet ds);
    }
}
