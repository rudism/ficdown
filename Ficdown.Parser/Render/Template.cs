namespace Ficdown.Parser.Render
{
    using System;
    using System.IO;

    public static class Template
    {
        public static string Index
        {
            get { return GetFileContents("Views/index.html"); }
        }

        public static string Scene
        {
            get { return GetFileContents("Views/scene.html"); }
        }

        public static string Styles
        {
            get { return GetFileContents("Assets/styles.css"); }
        }

        private static string GetFileContents(string fname)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("Render/{0}", fname));
            return File.ReadAllText(path);
        }
    }
}
