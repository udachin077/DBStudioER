using Base;
using Microsoft.Win32;
using System;
using System.ComponentModel;

namespace Settings
{
    public enum Hive
    {
        [Description("Journal Managment Studio")]
        JMS,
        [Description("Journal Project Studio")]
        JPS
    }

    public static class AppSettings
    {
        private static RegistryKey currentUserKey = Registry.CurrentUser;

        public struct String
        {
            /// <summary>
            /// Устанавливает значение ключа.
            /// </summary>
            /// <param name="name">Имя ключа</param>
            /// <param name="value">Значение ключа</param>
            public static void SetValue(KeyName name, string value, Hive programName = Hive.JMS)
            {
                RegistryKey baseKey = currentUserKey.CreateSubKey("Software").CreateSubKey("Dream Team").CreateSubKey(programName.GetDescription());
                baseKey.SetValue(name.ToString(), value, RegistryValueKind.String);
                baseKey.Close();
            }

            /// <summary>
            /// Получает значение ключа.
            /// </summary>
            /// <param name="name">Имя ключа</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public static string GetValue(KeyName name, Hive programName = Hive.JMS)
            {
                RegistryKey baseKey = currentUserKey.OpenSubKey("Software").OpenSubKey("Dream Team").OpenSubKey(programName.GetDescription());
                string value = baseKey.GetValue(name.ToString())?.ToString();
                baseKey.Close();
                if (value == null || value == string.Empty)
                {
                    throw new ArgumentNullException(name.ToString(), "Значение не найдено или имеет неверный формат.");
                }
                return value;
            }
        }

        public struct DWord
        {
            /// <summary>
            /// Устанавливает значение ключа.
            /// </summary>
            /// <param name="name">Имя ключа</param>
            /// <param name="value"></param>
            public static void SetValue(KeyName name, dynamic value, Hive programName = Hive.JMS)
            {
                RegistryKey baseKey = currentUserKey.CreateSubKey("Software").CreateSubKey("Dream Team").CreateSubKey(programName.GetDescription());
                baseKey.SetValue(name.ToString(), value, RegistryValueKind.DWord);
                baseKey.Close();
            }

            /// <summary>
            /// Получает значение ключа.
            /// </summary>
            /// <param name="name">Имя ключа</param>
            /// <param name="type"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public static dynamic GetValue(KeyName name, Type type, Hive programName = Hive.JMS)
            {
                RegistryKey baseKey = currentUserKey.OpenSubKey("Software").OpenSubKey("Dream Team").OpenSubKey(programName.GetDescription());
                object _value = baseKey.GetValue(name.ToString());
                baseKey.Close();
                if(_value == null)
                {
                    throw new ArgumentNullException(name.ToString(), "Значение не найдено или имеет неверный формат.");
                }
                dynamic value = null;
                if (type == typeof(byte))
                    value = Convert.ToByte(_value);
                if (type == typeof(bool))
                    value = Convert.ToBoolean(_value);
                return value;
            }
        }
    }
}
