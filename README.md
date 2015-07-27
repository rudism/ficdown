# Ficdown

Ficdown is a system for building interactive fiction using MarkDown syntax. See [Ficdown.com](http://www.ficdown.com) for more information.

This project contains the core Ficdown library for parsing Ficdown stories, as well as a console application that can be used to generate HTML or epub ebook formats.

## Dependencies

Ficdown is written using .NET and should run on recent versions of Windows without requiring any additional downloads. Older versions may need to have .NET 4.5 installed via Windows Software Update.

It has been written and tested on Linux using [Mono](http://www.mono-project.com).

## Obtaining

If you want to use Ficdown to convert your stories into ebooks, download *ficdown.zip* from the latest release on the [releases](https://github.com/rudism/Ficdown/releases) page and decompress it somewhere on your hard drive. Ficdown does not include an installer, the ficdown.exe utility is included directly in the zip archive.

## Usage

### Windows

The ficdown.exe utility is a command line program, which means on Windows you must run it from the command prompt (simply double-clicking it will not do anything). The quickest way to get to your command prompt is to open the ficdown directory, hold down your shift key, and then right-click somewhere inside the ficdown window that isn't on a file. A contextual menu will pop up from which you can select "Open command prompt here".

Once in your command prompt, you can run ficdown by typing `ficdown.exe` and then including your command line options as discussed below.

### Linux/Mac OS X

You must have Mono installed to use ficdown.exe. Assuming it is installed an on your path, from your command line in the ficdown folder you would just need to type `mono ficdown.exe` and include your command line options to pass to ficdown

### Options

Running ficdown.exe without any arguments will produce the following help text:

    Usage: ficdown.exe
        --format (html|epub)
        --in "/path/to/source.md"
        [--out "/path/to/output"]
        [--template "/path/to/template/dir"]
        [--images "/path/to/images/dir"]
        [--author "Author Name"]
        [--debug]

Options surrounded by square brackets are optional, everything else is required. It should be noted that while the help text shows Linux-style paths, these will not work on Windows. On Windows you should pass regular paths as you normally would (for example `--in "C:\Users\Me\Documents\MyStory.md`).

#### --format

Can be either `html` or `epub`.

#### --in

Should be the absolute or relative path to your Ficdown story file.

#### --out

You can specify either the absolute or relative path of the epub file to generate when `format` is `epub`, or the absolute or relative path of the directory to create when `format` is `html`.

#### --template

You can optionally specify a custom HTML template to use when generating the HTML or epub documents. The template directory should contain three files: [index.html](/Ficdown.Parser/Render/Views/index.html), [scene.html](/Ficdown.Parser/Render/Views/scene.html), and [styles.css](/Ficdown.Parser/Render/Assets/styles.css). Those link to the default files used if no custom template is specified (which have been optimized for simple epub formatting).

#### --images

If your story contains images, you must place them all into a directory and then include the absolute or relative path to that directory here. Due to the nature of epub documents, images cannot be linked to from subdirectories in your story--you must always link to them as though they are located in the same directory as the Ficdown file. For example, this is good Markdown syntax for including images in your story:

    ## My Scene

    Here is a lovely image to accompany my scene:

    ![My Image](myimage.png)

The following would not work, even if your images directory contains the `stuff` subdirectory (those images would not get loaded into the epub correctly):

    ## My Scene

    Here is a lovely image to accompany my scene that you will never see:

    ![My Image](stuff/myimage.png)

#### --author

This option is required if your `format` is `epub` for the ebook metadata. It is ignored for `html`.

#### --debug

If you pass this option, all of the pages in your story will include output at the bottom showing you what the current player state looks like (a list of all state variables that have been toggled and are used in scenes that the player can still reach). This can be useful if you discover that your compiled story does not behave the way you expected it to.

## Other Formats

To generate other formats than HTML or epub, you will have to use third party tools. [Calibre](http://www.calibre-ebook.com) is a popular ebook management suite that includes the ability to convert books from almost any format to any other format. Also, Amazon has an official tool called [KindleGen](http://www.amazon.com/gp/feature.html?docId=1000765211) that you can use to convert your epub to a format that can be read on Kindles.

### Interactive Website

Ficdown stories can be played interactively in a web browser without even requiring the command line utility here. See [Ficdown.js](https://github.com/rudism/Ficdown.js) for a Javascript Ficdown parser and interpreter that you can include on your own website to present your Ficdown stories.
