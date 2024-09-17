using Base;
using Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace JournalManagmentStudio.Project.Services
{
    internal static class ProjectFileParser
    {
        /// <summary>
        /// Создает проект.
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        /// <param name="programName">Название программы</param>
        /// <param name="pathDataSet">Путь к dataset</param>
        /// <returns></returns>
        public static ProjectParameter Save(string projectName, ProgramName programName, string pathDataSet = null)
        {
            // Установка пути к новому проекту
            string _value0 = Path.Combine(AppSettings.String.GetValue(KeyName.ProjectFolder, Hive.JPS), projectName);
            // Путь к таблице
            string _value1 = Path.Combine(_value0, "Release", $"{projectName}.dt");
            // Путь к датасет
            string _value2 = string.Empty;
            // Создание папки проекта
            Directory.CreateDirectory(_value0);
            Directory.CreateDirectory(Path.Combine(_value0, "Release"));
            // Если датасет задан, то копируем его в папку проекта
            if (string.IsNullOrEmpty(pathDataSet) == false)
            {
                _value2 = Path.Combine(_value0, "DataSet.ds");
                File.Copy(pathDataSet, _value2);
            }
            // Копирование схемы таблицы в папку проекта
            switch (programName)
            {
                case ProgramName.WinAl:
                    File.Copy("Shemas\\winal.xml", _value1);
                    break;
                case ProgramName.AsuOdsMsde:
                    File.Copy("Shemas\\asuods.xml", _value1);
                    break;
                case ProgramName.Horizon:
                    File.Copy("Shemas\\horizon.xml", _value1);
                    break;
                case ProgramName.AsuOdsSqlServer:
                    break;
                case ProgramName.ASUDScada:
                    File.Copy("Shemas\\scada.xml", _value1);
                    break;
                default:
                    break;
            }

            // Создание файла проекта
            XDocument xdoc = new XDocument(new XElement("Project", new XAttribute("Name", projectName), new XAttribute("Program", programName),
            new XElement("ItemGroup",
                new XElement("Reference", new XAttribute("Include", _value1)),
                new XElement("Reference", new XAttribute("Include", _value2)))
                        ));
            xdoc.Save(Path.Combine(_value0, $"{projectName}.jproj"));

            return new ProjectParameter()
            {
                Name = projectName,
                Program = programName,
                PathTable = _value1,
                PathDataSet = _value2
            };
        }

        /// <summary>
        /// Загружает проект.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ProjectParameter Load(string path)
        {
            XDocument xdoc = XDocument.Load(path);
            List<string> vs = new List<string>();
            foreach (XElement refElement in xdoc.Element("Project").Element("ItemGroup").Elements("Reference"))
            {
                vs.Add(refElement.Attribute("Include").Value);
            }

            return new ProjectParameter()
            {
                Name = xdoc.Element("Project").Attribute("Name").Value,
                Program = (ProgramName)Enum.Parse(typeof(ProgramName), xdoc.Element("Project").Attribute("Program").Value),
                PathTable = vs[0],
                PathDataSet = vs[1]
            };
        }
    }
}
