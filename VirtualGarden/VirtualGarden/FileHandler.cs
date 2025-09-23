using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualGarden
{
    public static class FileHandler
    {
        private static readonly string savePath = "saves";
        private static readonly string saveFilename = "save.JSON";
        public static Image GetImageFromFile(string path)
        {
            try
            {
                Image image = Image.FromFile(path);
                return image;
            }
            catch(OutOfMemoryException)
            {
                throw new UnableToLoadImageFromFileException($"Unable to load image from file {path}. Not enought memory to complete action.");
            }
            catch (FileNotFoundException)
            {
                throw new UnableToLoadImageFromFileException($"Unable to load image from file {path}. The file is missing.");
            }
        }

        public static void SaveGardenToFile(Garden garden)
        {
            try
            {
                string serializedGarden = SerializeDictToJSON(SerializeObject(garden));
                string path = Path.Combine(savePath, saveFilename);
                if (!File.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                File.WriteAllText(path, serializedGarden);
            }
            catch (Exception)
            {
                throw new LoadStoreException("Unable to save garden to file.");
            }
        }

        public static Garden LoadGardenFromFile()
        {
            try
            {
                string path = Path.Combine(savePath, saveFilename);
                string json = File.ReadAllText(path);
                Garden garden = DeserializeGarden(json);
                return garden;
            }
            catch(Exception)
            {
                throw new LoadStoreException("Unable to load garden to file.");
            }
        }

        private static Dictionary<string, object?> SerializeObject(object obj)
        {
            Type type = obj.GetType();
            //all properties of obj that are supposed to be saved
            var properties = GetPropertyInfos(type);
            //all fields of obj that are supposed to be saved
            var fields = GetFieldInfos(type);

            Dictionary<string, object?> dictToSerialize = new Dictionary<string, object?>();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                dictToSerialize[property.Name] = GetSerializableValue(value);
            }
            foreach (var field in fields)
            {
                var value = field.GetValue(obj);
                dictToSerialize[field.Name] = GetSerializableValue(value);
            }
            
            return dictToSerialize;
        }

        private static object? GetSerializableValue(object? value)
        {
            if (value == null)
            {
                return null;
            }
            if (IsSimpleType(value.GetType()))
            {
                return value;
            }
            if (value.GetType() == typeof(Dictionary<FlowerType, int>))
            {
                return GetSerializableDict((Dictionary<FlowerType, int>)value);
            }
            return SerializeObject(value);
        }

        private static Dictionary<string, int> GetSerializableDict(Dictionary<FlowerType, int> dict)
        {
            Dictionary<string, int> serializedDict = new Dictionary<string, int>();
            foreach (var key in dict.Keys)
            {
                serializedDict[key.Name] = dict[key];
            }
            return serializedDict;
        }

        private static string SerializeDictToJSON<TKey, TValue>(Dictionary<TKey, TValue> dict) where TKey : notnull
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(dict, options);
        }

        private static bool IsSimpleType(Type type)
        {
            return (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(bool) || type == typeof(char) || type == typeof(string) || type == typeof(Enum));
        }

        private static IEnumerable<PropertyInfo> GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(property => property.GetCustomAttribute(typeof(SaveAttribute)) != null);
        }

        private static IEnumerable<FieldInfo> GetFieldInfos(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute(typeof(SaveAttribute)) != null);
        }

        private static Garden DeserializeGarden(string json)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            Type gardenType = typeof(Garden);
            Garden garden = (Garden)Activator.CreateInstance(gardenType, nonPublic: true);
            var properties = GetPropertyInfos(gardenType);
            var fields = GetFieldInfos(gardenType);
            PropertyInfo eventInfo = null;
            object? eventTypeName = null;
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Event))
                {
                    eventInfo = property;
                }
                else if (property.Name == "EventTypeName")
                {
                    if (dict.TryGetValue(property.Name, out var value))
                    {
                        eventTypeName = ConvertJsonElementToType(value, property.PropertyType);
                    }
                }
                else if (dict.TryGetValue(property.Name, out var value))
                {
                    object? convertedValue = ConvertJsonElementToType(value, property.PropertyType);
                    property.SetValue(garden, convertedValue);
                }
            }

            if (eventTypeName is not null)
            {
                var eventType = Type.GetType((string)eventTypeName);
                if (eventType is null)
                {
                    throw new DeserializationException($"Failed to deserialize event. No event type found.");
                }
                if (!typeof(Event).IsAssignableFrom(eventType))
                {
                    throw new DeserializationException($"Failed to deserialize event of type {eventType.FullName}. {eventType.FullName} is not an event.");
                }
                if (eventInfo is null)
                {
                    throw new DeserializationException($"Failed to deserialize event. No serialized event found.");
                }
                Event gardenEvent = (Event)Activator.CreateInstance(eventType, nonPublic: true);
                eventInfo.SetValue(garden, gardenEvent);
                //add deserialization of event properties
            }
            
            return garden;
        }

        private static object? DeserializeObject(JsonElement element, Type type)
        {
            object obj = Activator.CreateInstance(type, nonPublic: true);
            var properties = GetPropertyInfos(type);
            var fields = GetFieldInfos(type);

            foreach (var property in properties)
            {
                if (element.TryGetProperty(property.Name, out var value))
                {
                    object? convertedValue = ConvertJsonElementToType(value, property.PropertyType);
                    property.SetValue(obj, convertedValue);
                }
            }

            foreach (var field in fields)
            {
                if (element.TryGetProperty(field.Name, out var value))
                {
                    object? convertedValue = ConvertJsonElementToType(value, field.FieldType);
                    field.SetValue(obj, convertedValue);
                }
            }
            return obj;
        }

        private static object? ConvertJsonElementToType(JsonElement element, Type type)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if(type == typeof(int)) { return element.GetInt32(); }
            if (type == typeof(long)) { return element.GetInt64(); }
            if (type == typeof(double)) { return element.GetDouble(); }
            if (type == typeof(bool)) { return element.GetBoolean(); }
            if (type == typeof(char)) { return element.GetString()[0]; }
            if (type == typeof(string)) { return element.GetString(); }
            if (type.IsEnum) { Enum.Parse(type, element.GetString()); }

            if (type == typeof(Dictionary<FlowerType, int>))
            {
                return ConvertJsonElementToFlowerCountDict(element);
            }

            return DeserializeObject(element, type);
        }

        private static Dictionary<FlowerType, int> ConvertJsonElementToFlowerCountDict(JsonElement element)
        {
            var flowerTypesFields = typeof(FlowerTypes).GetFields(BindingFlags.Public | BindingFlags.Static);
            var dict = new Dictionary<FlowerType, int>();
            foreach(var keyValue in element.EnumerateObject())
            {
                foreach (var field in flowerTypesFields)
                {
                    if (field.GetValue(null) is FlowerType fieldFlowerType && fieldFlowerType.Name == keyValue.Name)
                    {
                        FlowerType flowerType = fieldFlowerType;
                        dict.Add(flowerType, keyValue.Value.GetInt32());
                        break;
                    }
                }
            }
            return dict;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class SaveAttribute : System.Attribute { }
}
