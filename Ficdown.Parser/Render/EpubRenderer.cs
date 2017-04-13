namespace Ficdown.Parser.Render
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Epub4Net;
    using Ionic.Zip;
    using Ionic.Zlib;

    #region fix for bug in epub4net
    // https://bitbucket.org/dalager/epub4net/issues/2/windows-path-separator-hard-coded-in

    public class FixedFileSystemManager : IFileSystemManager
    {
        private FileSystemManager _fsm;

        public FixedFileSystemManager()
        {
            _fsm = new FileSystemManager(Guid.NewGuid().ToString());
        }

        public void SetupOutputDir()
        {
            _fsm.SetupOutputDir();
        }

        public string ContentDir
        {
            get { return _fsm.ContentDir; }
            set { _fsm.ContentDir = value; }
        }

        public string BuildDirectory
        {
            get { return _fsm.BuildDirectory; }
            set { _fsm.BuildDirectory = value; }
        }

        public void CreateTocFile(Epub epub)
        {
            _fsm.CreateTocFile(epub);
        }

        public void CreateContentOpfFile(Epub epub)
        {
            _fsm.CreateContentOpfFile(epub);
        }

        public void CopyChapterFilesToContentFolder(Epub epub)
        {
            _fsm.CopyChapterFilesToContentFolder(epub);
        }

        public string ZipEpub(Epub epub)
        {
            var epubFilename = epub.Title + ".epub";
            if(File.Exists(epubFilename)) File.Delete(epubFilename);
            using(var zip = new ZipFile(epubFilename, Encoding.UTF8))
            {
                zip.EmitTimesInWindowsFormatWhenSaving = false;
                zip.CompressionLevel = CompressionLevel.None;
                zip.AddFile(Path.Combine(_fsm.BuildDirectory, "mimetype"), "\\");
                zip.Save();
                File.Delete(Path.Combine(_fsm.BuildDirectory, "mimetype"));
                zip.AddDirectory(_fsm.BuildDirectory);
                zip.Save();
            }
            return epubFilename;
        }

        public void CopyResourceFilesToContentFolder(Epub epub)
        {
            _fsm.CopyResourceFilesToContentFolder(epub);
        }

        public void ValidatePathsExists(IEnumerable<IPathed> fileList)
        {
            _fsm.ValidatePathsExists(fileList);
        }

        public void DeleteBuildDir()
        {
            _fsm.DeleteBuildDir();
        }
    }
    #endregion

    public class EpubRenderer : HtmlRenderer
    {
        private readonly string _author;
        private readonly string _bookId;
        private readonly string _language;

        public EpubRenderer(string author, string bookId, string language) : base(language)
        {
            _author = author;
            _bookId = bookId ?? Guid.NewGuid().ToString("D");
            _language = language ?? "en";
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
            epub.BookId = _bookId;
            epub.Language = _language;
            epub.AddResourceFile(new ResourceFile("styles.css", Path.Combine(temppath, "styles.css"), "text/css"));

            if (!string.IsNullOrWhiteSpace(ImageDir))
            {
                var dirname = ImageDir.Substring(ImageDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                var tempimgdir = Path.Combine(temppath, dirname);
                foreach (var img in Directory.GetFiles(tempimgdir))
                {
                    var fname = Path.GetFileName(img);
                    epub.AddResourceFile(new ResourceFile(fname,
                        Path.Combine(tempimgdir, fname), MimeHelper.GetMimeType(img)));
                }
            }

            var builder = new EPubBuilder(new FixedFileSystemManager(), Guid.NewGuid().ToString());
            var built = builder.Build(epub);

            File.Move(built, outPath);
            Directory.Delete(temppath, true);
        }
    }
}
