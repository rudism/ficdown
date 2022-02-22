**Note:** Active development has moved to https://code.sitosis.com/rudism/ficdown

# Ficdown

Ficdown is a system for building interactive fiction using MarkDown syntax. See [Ficdown.com](http://www.ficdown.com) for more information.

This project contains the core Ficdown library for parsing Ficdown stories, as well as a console application that can be used to generate HTML or epub ebook formats.

## Dependencies

Ficdown is written using .NET Core and should run on Windows, Linux, and OSX without needing any additional system dependencies installed.

## Obtaining

If you want to use Ficdown to convert your stories into ebooks, download the latest version from the [releases](https://github.com/rudism/Ficdown/releases) page and decompress it somewhere on your hard drive. Ficdown does not include an installer, the application and all of its dependencies are included directly in the zip archive.

## Usage

### Windows

The ficdown.exe utility is a command line program, which means on Windows you must run it from the command prompt (simply double-clicking it will not do anything). The quickest way to get to your command prompt is to open the ficdown directory, hold down your shift key, and then right-click somewhere inside the ficdown window that isn't on a file. A contextual menu will pop up from which you can select "Open command prompt here".

Once in your command prompt, you can run ficdown by typing `ficdown.exe` and then including your command line options as discussed below.

### Linux/Mac OS X

The pre-built releases are self-contained .NET Core deployments, so you should be able to just run the `ficdown` executable after decompressing it.

### Options

Running ficdown.exe without any arguments will produce the following help text:

    Usage: ficdown.exe
        --format (html|epub|lint)
        --in "/path/to/source.md" (lint reads stdin)
        [--out "/path/to/output"]
        [--template "/path/to/template/dir"]
        [--images "/path/to/images/dir"]
        [--author "Author Name"]
        [--debug]

Arguments surrounded by square brackets are optional, everything else is required. It should be noted that while the help text shows Linux-style paths, these will not work on Windows. On Windows you should pass regular paths as you normally would (for example `--in "C:\Users\Me\Documents\MyStory.md`).

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

## Additional Tools

- Ficdown stories can be played interactively in a web browser without even requiring the command line utility here. See [Ficdown.js](https://github.com/rudism/Ficdown.js) for a Javascript Ficdown parser and interpreter that you can include on your own website to present your Ficdown stories.

- [Ficdown-editor](https://byfernanz.github.io/ficdown-editor/) is a web-based GUI for writing Ficdown.

- [Prop](https://github.com/ByFernanz/prop) is a YAML-header style preprocessor for Ficdown

- [vim-ficdown](https://github.com/rudism/vim-ficdown) is an ALE linter for vim that identifies things like links to scenes that don't exist, unused variables, and other errors and warnings in Ficdown files.
