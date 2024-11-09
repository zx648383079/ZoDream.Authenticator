using System;
using System.Linq;
using System.Reflection;

namespace ZoDream.Shared.Database
{
    internal static class TypeMapper
    {

        public static bool HasAccountProperty(EntryType type) => type != EntryType.File && type != EntryType.Note;

        public static bool IsLargeLength(EntryType type) => type != EntryType.File;


        public static PropertyInfo? GetPropertyInfo(object obj, string name)
        {
            var type = obj.GetType();
            var field = type.GetProperty(name);
            if (field is not null)
            {
                return field;
            }
            foreach (var item in type.GetProperties())
            {
                var attr = item.GetCustomAttributes<ColumnAttribute>().Where(it => it.Name == name);
                if (attr.Any())
                {
                    return item;
                }
            }
            return null;
        }
        public static PropertyInfo? GetPropertyInfo(object obj, string name, int index)
        {
            var type = obj.GetType();
            var field = type.GetProperty(name);
            if (field is not null)
            {
                return field;
            }
            foreach (var item in type.GetProperties())
            {
                var attr = item.GetCustomAttributes<ColumnAttribute>().Where(it => it.Name == name);
                if (attr.Any())
                {
                    return item;
                }
                var items = item.GetCustomAttributes<ExtraColumnAttribute>().Where(it => it.Index == index);
                if (items.Any())
                {
                    return item;
                }
            }
            return null;
        }

        public static T GetProperty<T>(object obj, string name)
        {
            var field = GetPropertyInfo(obj, name);
            if (field is not null)
            {
                return (T)field.GetValue(obj);
            }
            return default;
        }
        public static T GetProperty<T>(object obj, string name, int index)
        {
            var field = GetPropertyInfo(obj, name, index);
            if (field is not null)
            {
                return (T)System.Convert.ChangeType(field.GetValue(obj), typeof(T));
            }
            return default;
        }

        public static void SetProperty(object obj, string name, object val)
        {
            var field = GetPropertyInfo(obj, name);
            field?.SetValue(obj, val);
        }
        public static void SetProperty(object obj, string name, int index, object val)
        {
            var field = GetPropertyInfo(obj, name, index);
            field?.SetValue(obj, System.Convert.ChangeType(val, field.DeclaringType));
        }
        public static EntryType Convert(object entity)
        {
            var type = entity.GetType();
            var attr = type.GetCustomAttributes<EntryAttribute>();
            if (attr is not null && attr.Any())
            {
                return attr.First().Type;
            }
            if (entity is IAuthEntryEntity)
            {
                return EntryType.Authentication;
            }
            if (entity is ITOTPEntryEntity)
            {
                return EntryType.ToTp;
            }
            if (entity is IWirelessEntryEntity)
            {
                return EntryType.Wireless;
            }
            if (entity is IPasswordEntryEntity)
            {
                return EntryType.Password;
            }
            if (entity is IFileEntryEntity)
            {
                return EntryType.File;
            }
            if (entity is INoteEntryEntity)
            {
                return EntryType.Note;
            }
            
            return EntryType.None;
        }

        public static string[] EntryPropertyNames(EntryType type)
        {
            return type switch
            {
                EntryType.Wireless => ["Title", "Account", "Password", "Security"],
                EntryType.ToTp => ["Title", "Account", "Secret", "Url", "Algorithm", "Period", "Digits"],
                EntryType.Authentication => ["Title", "Account", "Email", "Mobile", "Password", "Url"],
                EntryType.Password => ["Title", "Account", "Password", "Url"],
                EntryType.File => ["Title", "FileName"],
                EntryType.Note => ["Title", "Content"],
                _ => ["Title", "Account", "Password", "Url"]
            };
        }


    }
}
