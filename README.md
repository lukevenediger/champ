# champ
A sensible static site generator that runs on Windows and uses Razor templates. I built it so that I could focus more time on writing content and less on fiddling with markup.

Features include:
* Intuitive pathing system
* Uses Razor template syntax for layouts
* Supports reusable components
* Callbacks to resolve URLS for subdirectories

## Download champ
The tool is packaged as a single .exe file. Download the latest release of champ.exe from here: [champ v1.0](https://github.com/lukevenediger/champ/releases/tag/v1.0)

## Getting Started
Getting started with champ is easy. Copy champ.exe into the root folder of your project, open command prompt 
and then create your site using the --bootstrap option:

```
c:\mysite\champ.exe --bootstrap
```

champ will start creating all required folders and add a few example pages. Now build your site:

```
c:\mysite\champ.exe .   <-- don't forget the period!
```

You won't see anything appear in the console, unless there's an error.

> Hint: use the --verbose option to get (lots of) output.

Great! You're ready to view your site. Open index.html in the output directory with your favourite browser:

```
start c:\mysite\output\index.html
```

Now you can get editing by adding new files into the pages/ directory and running champ.exe again.

## Finding Your Way Around
champ uses a combination of [razor templates](http://weblogs.asp.net/scottgu/archive/2010/07/02/introducing-razor.aspx) and [markdown files](http://daringfireball.net/projects/markdown/syntax) 
to produce static .html files. It combines these files from various directories when building your site. There are three main directories in use:

* `pages\` - to store content 
* `static\` - to store assets like images, javascript files and stylesheets
* `templates\` - to store layout templates and reusable components

### pages\
The `pages\` directory contains your content markdown (.md) files that will be converted to HTML and combined with a template, 
before being saved as a `.html` file of the same name. So, `about-me.md` will become `about-me.html`.

Directory paths are also mirrored, making it easy to organise your content. Given this file, `pages\projects\foo\instructions.md`, 
champ will create a file called `projects\foo\instructions.html`. Your subdirectories are mirrored when creating your content.
The output of pages\ will also become the rooted content of your website. In other words, `pages\index.md` will become `yoursite.com/index.html`
and `pages\company\founders.md` will become `yoursite.com/company/founders.html`.

**What's in a Page file?**
A page file contains a section at the top which specifies the page title and template. `template` maps to a `.cshtml` file found in `templates\`. 
`title` is the page title and used by the template.

```html
<!--
template = page
title = About Me
-->
```

> Note: The `template` is optional. If you don't specify one then the template from index.md will be used. 
If that doesn't exist, the parent index.md will be used.

**What about non-markdown files?**
Any file that isn't a markdown (.md) file will be copied over. This is useful when you want to send an .html file to the output directory, or need a [CNAME](https://help.github.com/articles/setting-up-a-custom-domain-with-pages#setting-the-domain-in-your-repo) file to enable github pages.

### static\
All site assets like stylesheets, javascript files and images should reside here. They're copied over as-is to the output folder, and no special processing is done to these files.

### templates\
All layout templates and reusable components are stored here. champ doesn't support subdirectories, so all templates must reside here. 

Templates are `.cshtml` files that can be included in a layout template, include a reusable component and also provide a hook point for injecting the markdown output.

## Using Champ to Generate Content
Use `champ.exe` to regenerate your content after an edit:

```
champ.exe SOURCE-DIRECTORY OUTPUT-DIRECTORY
```

> Note: champ will delete everything from the output directory before building your site. Only files and directories
that start with a period (.) will be ignored.

### Errors
Compilation errors are shown in the page that was trying to be created. The error page shows you the compilation
error message, the offending line number and a short snippet of the broken code.

## More Options

### Watch for changes and automatically generate the output
The --watch option will scan the source directory for any file changes and automatically generate the
new output. Use this when you're busy developing your site content and don't want to have to keep
starting champ after every change.

```
champ.exe --watch C:\src\lukevenediger.github.io C:\src\lukevenediger.github.io-live
```

### Downloading champ-bootstrap from a file share
You can specify a file or UNC path for the location of champ-bootstrap by using the --bootstrap-source option:

```
champ.exe --bootstrap --bootstrap-source \\myserver\share\champ\champ-bootstrap.zip
```

If you don't specify --bootstrap-source champ will try download the latest zip file from github.com.

## Project Info

### Getting Help
There are a few ways to get help:
* Open an issue on the project
* Contact me on twitter or via email (see below)

I'd also love your suggestions and welcome any improvements you may have.

### Why Champ? (plus some alternatives)
I really love the idea of pre-generated content pages as a way to make building a site much simpler, as well as freeing up cycles
to be able to write more content. I looked around and saw that the really good generators such as [Jekyll](http://github.com/mojombo/jekyll) 
and [Punch](http://laktek.github.com/punch) were unix/macos/linux focused, while windows-only options like [pretzel](https://github.com/Code52/pretzel) 
didn't give me a good first-run experience. So, champ was born, and it suits my needs just fine. I hope it suits yours too!

### What's Next?
These features would be great to have:
* Better support for blog articles, including RSS feed generation
* A reusable component for DISQUS comments
* Automatic minification/LESSification of javascript and LESS files
* A --watch option that auto-generates files as they are updated

# About the Codebase
Licence: MIT

Maintainers:
* [Luke Venediger](http://lukevenediger.github.io/): [lukev@lukev.net](mailto:lukev@lukev.net)