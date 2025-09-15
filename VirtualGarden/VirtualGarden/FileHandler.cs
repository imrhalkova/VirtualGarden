using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public static class FileHandler
    {
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
    }
}
