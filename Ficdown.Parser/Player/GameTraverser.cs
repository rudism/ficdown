namespace Ficdown.Parser.Player
{
    using System.IO;
    using System.Threading;
    using Model.Story;

    internal class GameTraverser
    {
        private IRenderer _renderer;

        public IRenderer Renderer
        {
            get
            {
                return _renderer ??
                       (_renderer =
                           new HtmlRenderer {Template = File.ReadAllText(@"C:\Users\Rudis\Desktop\template.html")});
            }
            set { _renderer = value; }
        }

        private volatile int _page = 0;

        private string _template;

        public void ExportStaticStory(Story story, string templateFile, string outputDirectory)
        {
            _template = File.ReadAllText(templateFile);
            var dir = new DirectoryInfo(outputDirectory);
            if (!dir.Exists) dir.Create();
            else
            {
                foreach (var finfo in dir.GetFileSystemInfos())
                {
                    finfo.Delete();
                }
            }

            var index = string.Format("# {0}\n\n{1}\n\n[Start the game.](page{2}.html)", story.Name,
                story.Description, _page++);
            Renderer.Render(index, Path.Combine(outputDirectory, "index.html"));
        }
    }
}
