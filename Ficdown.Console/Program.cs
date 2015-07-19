namespace Ficdown.Console
{
    using System;
    using System.IO;
    using Microsoft.SqlServer.Server;
    using Parser;
    using Parser.Render;

    internal class Program
    {
        private static int Main(string[] args)
        {
            string infile = null;
            string output = null;
            string tempdir = null;
            string format = null;
            string author = null;
            string images = null;
            var debug = false;

            if (args.Length == 1)
            {
                if (args[0] == "/?" || args[0] == "/help" || args[0] == "-help" || args[0] == "--help")
                {
                    ShowHelp();
                    return 0;
                }
            }
            else if (args.Length > 1)
            {
                for (var i = 0; i < args.Length; i += 2)
                {
                    switch (args[i])
                    {
                        case "--format":
                            format = args[i + 1];
                            break;
                        case "--in":
                            infile = args[i + 1];
                            break;
                        case "--out":
                            output = args[i + 1];
                            break;
                        case "--template":
                            tempdir = args[i + 1];
                            break;
                        case "--author":
                            author = args[i + 1];
                            break;
                        case "--images":
                            images = args[i + 1];
                            break;
                        case "--debug":
                            i--;
                            debug = true;
                            break;
                        default:
                            Console.WriteLine(@"Unknown option: {0}", args[i]);
                            return 1;
                    }
                }
            }
            else
            {
                ShowHelp();
                return 0;
            }
            if (string.IsNullOrWhiteSpace(format) || string.IsNullOrWhiteSpace(infile))
            {
                ShowHelp();
                return 1;
            }
            if (!File.Exists(infile))
            {
                Console.WriteLine(@"Source file {0} not found.", infile);
                return 2;
            }
            if (string.IsNullOrWhiteSpace(output))
                if (format == "html")
                    output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"html");
                else if (format == "epub")
                    output = "output.epub";

            if (!string.IsNullOrWhiteSpace(output) && (Directory.Exists(output) || File.Exists(output)))
            {
                Console.WriteLine(@"Specified output {0} already exists.", output);
                return 2;
            }
            if (!string.IsNullOrWhiteSpace(tempdir))
            {
                if (!Directory.Exists(tempdir))
                {
                    Console.WriteLine(@"Template directory {0} does not exist.", tempdir);
                    return 2;
                }
                if (!File.Exists(Path.Combine(tempdir, "index.html")) ||
                    !File.Exists(Path.Combine(tempdir, "scene.html")) ||
                    !File.Exists(Path.Combine(tempdir, "styles.css")))
                {
                    Console.WriteLine(
                        @"Template directory must contain ""index.html"", ""scene.html"", and ""style.css"" files.");
                }
            }

            if (!string.IsNullOrWhiteSpace(images) && !Directory.Exists(images))
            {
                Console.WriteLine(@"Images directory {0} does not exist.", images);
                return 2;
            }

            var parser = new FicdownParser();
            var storyText = File.ReadAllText(infile);

            Console.WriteLine(@"Parsing story...");

            var story = parser.ParseStory(storyText);

            IRenderer rend;
            switch (format)
            {
                case "html":
                    Directory.CreateDirectory(output);
                    rend = new HtmlRenderer();
                    break;
                case "epub":
                    if (string.IsNullOrWhiteSpace(author))
                    {
                        Console.WriteLine(@"Epub format requires the --author argument.");
                        return 1;
                    }
                    rend = new EpubRenderer(author);
                    break;
                default:
                    ShowHelp();
                    return 1;
            }

            if (!string.IsNullOrWhiteSpace(tempdir))
            {
                rend.IndexTemplate = File.ReadAllText(Path.Combine(tempdir, "index.html"));
                rend.SceneTemplate = File.ReadAllText(Path.Combine(tempdir, "scene.html"));
                rend.StylesTemplate = File.ReadAllText(Path.Combine(tempdir, "styles.css"));
            };

            if (!string.IsNullOrWhiteSpace(images)) rend.ImageDir = images;

            Console.WriteLine(@"Rendering story...");

            rend.Render(story, output, debug);

            Console.WriteLine(@"Done.");
            return 0;
        }


        private static void ShowHelp()
        {
            Console.WriteLine(
                @"Usage: ficdown.exe
    --format (html|epub)
    --in ""/path/to/source.md""
    [--out ""/path/to/output""]
    [--template ""/path/to/template/dir""]
    [--images ""/path/to/images/dir""]
    [--author ""Author Name""]
    [--debug]");
        }
    }
}
