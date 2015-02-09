﻿namespace Ficdown.Parser.Render
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Epub4Net;

    public class EpubRenderer : HtmlRenderer
    {
        private readonly string _author;

        public EpubRenderer(string author) : base()
        {
            _author = author;
        }

        public EpubRenderer(string author, string index, string scene, string styles) : base(index, scene, styles)
        {
            _author = author;
        }

        public override void Render(Model.Parser.ResolvedStory story, string outPath, bool debug = false)
        {
            var temppath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temppath);
            base.Render(story, temppath, debug);

            var chapters = new List<Chapter>
            {
                new Chapter(Path.Combine(temppath, "index.html"), "index.html", "Title Page")
            };
            chapters.AddRange(from file in Directory.GetFiles(temppath)
                select Path.GetFileName(file)
                into fname
                where fname != "index.html" && fname != "styles.css"
                select new Chapter(Path.Combine(temppath, fname), fname, fname.Replace(".html", string.Empty)));

            var epub = new Epub(Story.Name, _author, chapters);
            epub.AddResourceFile(new ResourceFile("styles.css", Path.Combine(temppath, "styles.css"), "text/css"));

            var builder = new EPubBuilder();
            var built = builder.Build(epub);

            File.Move(built, outPath);
            Directory.Delete(temppath, true);
        }
    }
}