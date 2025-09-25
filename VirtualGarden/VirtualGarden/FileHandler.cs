using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

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
                string serializedGarden = SerializeDictToJSON(SerializeObjectToDict(garden));
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
            catch(LoadStoreException ex)
            {
                throw;
            }
            catch(Exception)
            {
                throw new LoadStoreException("Unable to load garden from file.");
            }
            
        }

        private static Dictionary<string, object?> SerializeObjectToDict(object obj)
        {
            Type type = obj.GetType();
            //all properties of obj that are supposed to be saved
            var properties = GetSavePropertyInfos(type);
            //all fields of obj that are supposed to be saved
            var fields = GetSaveFieldInfos(type);

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
            if (value.GetType().IsEnum)
            {
                return value.ToString();
            }
            if (value.GetType() == typeof(Dictionary<FlowerType, int>))
            {
                return GetSerializableDict((Dictionary<FlowerType, int>)value);
            }
            if (value.GetType() == typeof(Tile[,]))
            {
                return GetSerializedTileGrid((Tile[,])value);
            }
            return SerializeObjectToDict(value);
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

        private static Dictionary<string, object?> GetSerializedTileGrid(Tile[,] tileGrid)
        {
            Dictionary<string, object?> serializedTileGrid = new Dictionary<string, object?>();
            int rows = tileGrid.GetLength(0);
            int columns = tileGrid.GetLength(1);
            serializedTileGrid["rows"] = rows;
            serializedTileGrid["columns"] = columns;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    serializedTileGrid[GetTileKeyString(i, j)] = SerializeObjectToDict(tileGrid[i, j]);
                }
            }
            return serializedTileGrid;
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
            return (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(bool) || type == typeof(char) || type == typeof(string));
        }

        private static IEnumerable<PropertyInfo> GetSavePropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(property => property.GetCustomAttribute(typeof(SaveAttribute)) != null);
        }

        private static IEnumerable<FieldInfo> GetSaveFieldInfos(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute(typeof(SaveAttribute)) != null);
        }

        private static IEnumerable<PropertyInfo> GetLoadPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(property => property.GetCustomAttribute(typeof(SaveAttribute)) != null &&
                property.GetCustomAttribute(typeof(NotLoadAttribute)) == null);
        }

        private static IEnumerable<FieldInfo> GetLoadFieldInfos(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute(typeof(SaveAttribute)) != null &&
                field.GetCustomAttribute(typeof(NotLoadAttribute)) == null);
        }

        private static Garden DeserializeGarden(string json)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            Type gardenType = typeof(Garden);
            Garden garden = (Garden)Activator.CreateInstance(gardenType, nonPublic: true);
            var properties = GetLoadPropertyInfos(gardenType);
            var fields = GetLoadFieldInfos(gardenType);
            PropertyInfo eventInfo = null;
            object? eventTypeName = null;
            JsonElement? eventElement = null;
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Event))
                {
                    eventInfo = property;
                    if (dict.TryGetValue(property.Name, out var value))
                    {
                        eventElement = value;
                    }
                }
                else if (property.Name == "EventTypeName")
                {
                    if (dict.TryGetValue(property.Name, out var value))
                    {
                        eventTypeName = ConvertJsonElementToType(value, property.PropertyType);
                    }
                }
                else if (property.PropertyType == typeof(Tile[,]))
                {
                    if (dict.TryGetValue(property.Name, out var value))
                    {
                        Tile[,] tileGrid = ConvertJsonElementToTileGrid(value);
                        property.SetValue(garden, tileGrid);
                    }
                }
                else if (dict.TryGetValue(property.Name, out var value))
                {
                    object? convertedValue = ConvertJsonElementToType(value, property.PropertyType);
                    property.SetValue(garden, convertedValue);
                }
            }

            foreach (var field in fields)
            {
                if (dict.TryGetValue(field.Name, out var value))
                {
                    object? convertedValue = ConvertJsonElementToType(value, field.FieldType);
                    field.SetValue(garden, convertedValue);
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
                if (eventElement is null)
                {
                    throw new DeserializationException($"Failed to deserialize event. No serialized event found.");
                }
                Event gardenEvent = (Event)DeserializeObject((JsonElement)eventElement, eventType);
                eventInfo.SetValue(garden, gardenEvent);
            }
            
            return garden;
        }

        private static object? DeserializeObject(JsonElement element, Type type)
        {
            object obj = Activator.CreateInstance(type, nonPublic: true);
            var properties = GetLoadPropertyInfos(type);
            var fields = GetLoadFieldInfos(type);

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

            if (type.IsEnum) { return Enum.Parse(type, element.GetString()); }

            if (type == typeof(Dictionary<FlowerType, int>))
            {
                return ConvertJsonElementToFlowerCountDict(element);
            }

            if (type == typeof(PlantedFlower))
            {
                PlantedFlower plantedFlower = ConvertJsonElementToPlantedFlower(element);
                return plantedFlower;
            }

            if (type == typeof(BugInfestation))
            {
                BugInfestation bugType = ConvertJsonElementToBugInfestation(element);
                return bugType;
            }

            return DeserializeObject(element, type);
        }

        private static Dictionary<FlowerType, int> ConvertJsonElementToFlowerCountDict(JsonElement element)
        {
            var flowerTypesFields = typeof(FlowerTypes).GetFields(BindingFlags.Public | BindingFlags.Static);
            var dict = new Dictionary<FlowerType, int>();
            foreach(var keyValue in element.EnumerateObject())
            {
                FlowerType flowerType = GetFlowerType(keyValue.Name);
                dict.Add(flowerType, keyValue.Value.GetInt32());
            }
            return dict;
        }

        private static PlantedFlower ConvertJsonElementToPlantedFlower(JsonElement plantedFlowerElement)
        {
            string flowerTypeNameProperty = "FlowerTypeName";
            if (plantedFlowerElement.TryGetProperty(flowerTypeNameProperty, out var value))
            {
                string flowerTypeName = (string)ConvertJsonElementToType(value, typeof(string));
                FlowerType flowerType = GetFlowerType(flowerTypeName);
                PlantedFlower plantedFlower = new PlantedFlower(flowerType);
                LoadJsonPropertiesToObj(plantedFlower, typeof(PlantedFlower), plantedFlowerElement);
                return plantedFlower;
            }
            else
            {
                throw new DeserializationException($"Cannot deserialize planted flower. {flowerTypeNameProperty} not found.");
            }
        }

        private static BugInfestation ConvertJsonElementToBugInfestation(JsonElement bugElement)
        {
            string bugTypeNameProperty = "BugsTypeName";
            if (bugElement.TryGetProperty(bugTypeNameProperty, out var value))
            {
                string bugTypeName = (string)ConvertJsonElementToType(value, typeof(string));
                BugType bugType = GetBugType(bugTypeName);
                BugInfestation bugInfestation = new BugInfestation(bugType);
                LoadJsonPropertiesToObj(bugInfestation, typeof(BugInfestation), bugElement);
                return bugInfestation;
            }
            else
            {
                throw new DeserializationException($"Cannot deserialize bug infestation. {bugTypeNameProperty} not found.");
            }

        }

        private static void LoadJsonPropertiesToObj(Object obj, Type objType, JsonElement element)
        {
            var properties = GetLoadPropertyInfos(objType);
            var fields = GetLoadFieldInfos(objType);

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
        }

        private static FlowerType GetFlowerType(string flowerTypeName)
        {
            var flowerTypesFields = typeof(FlowerTypes).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in flowerTypesFields)
            {
                if (field.GetValue(null) is FlowerType fieldFlowerType && fieldFlowerType.Name == flowerTypeName)
                {
                    return fieldFlowerType;
                }
            }
            throw new LoadStoreException($"Loading/storing failed. Flower type with name {flowerTypeName} was not found.");
        }

        private static BugType GetBugType(string bugTypeName)
        {
            var bugTypesFields = typeof(BugTypes).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in bugTypesFields)
            {
                if (field.GetValue(null) is BugType bugType && bugType.Name == bugTypeName)
                {
                    return bugType;
                }
            }
            throw new LoadStoreException($"Loading/storing failed. Bug type with name {bugTypeName} was not found.");
        }

        private static Tile[,] ConvertJsonElementToTileGrid(JsonElement element)
        {
            int? rows = null;
            int? columns = null;
            if(element.TryGetProperty("rows", out var rowsElement))
            {
                rows = rowsElement.GetInt32();
            }
            if (element.TryGetProperty("columns", out var columnsElement))
            {
                columns = columnsElement.GetInt32();
            }
            if (rows is null || columns is null)
            {
                throw new DeserializationException($"Failed to deserialize tile grid. The dimensions were not found.");
            }
            Tile[,] tileGrid = new Tile[(int)rows, (int)columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    JsonElement? tileElement = GetTileJsonElement(i, j, element);
                    if (tileElement is null)
                    {
                        throw new DeserializationException($"Failed to deserialize tile grid. Tile [{i}, {j}] not found.");
                    }
                    Object obj = ConvertJsonElementToType((JsonElement)tileElement, typeof(Tile));
                    tileGrid[i, j] = (Tile)obj;
                }
            }
            return tileGrid;
        }

        private static JsonElement? GetTileJsonElement(int row, int column, JsonElement gridElement)
        {
            if (gridElement.TryGetProperty(GetTileKeyString(row, column), out var tileElement))
            {
                return tileElement;
            }
            return null;
        }

        private static string GetTileKeyString(int row, int column)
        {
            return $"({row},{column})";
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class SaveAttribute : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class NotLoadAttribute : System.Attribute { }
}
