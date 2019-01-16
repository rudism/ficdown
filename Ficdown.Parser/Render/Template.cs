namespace Ficdown.Parser.Render
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class Template
    {
        public static  string BaseDir => Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);

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
            var path = Path.Combine(Template.BaseDir, "Render", fname);
            return File.ReadAllText(path);
        }
    }
}
