namespace Ficdown.Parser.Render
{
    using System.Collections.Generic;
    using System.IO;

    public static class MimeHelper
    {
        private static Dictionary<string, string> _mimeTypes = new Dictionary<string, string>
        {
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".png", "image/png"},
            {".gif", "image/gif"},
            {".bmp", "image/bmp"},
            {".tif", "image/tiff"},
            {".tiff", "image/tiff"}
        };

        public static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return !_mimeTypes.ContainsKey(ext) ? "application/octet-stream" : _mimeTypes[ext];
        }
    }
}
