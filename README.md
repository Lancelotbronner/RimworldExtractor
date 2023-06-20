# Rimworld Extractor

This is a very simple tool to extract and analyze XML tag usage of Rimworld mods.

This is currently in development as I'm still working on improving the analysis and the report JSON format.

## Roadmap

I'd like to make the analyzer use an SQLite database as its report and have the other
report derivative use that database. This should also improve performance, memory usage
and size in both the analyzer and the tools which will consume its reports.

The analysis is the core of this tool, here's what I'm working on:

- Type inference from usage
- Inferring polymorphic relationships on tags
- Inferring tag collections

The JSON format is one of the best way to share those analysis results. I'd like to reduce its physical size to be more portable. Here's what I'm working on:

- Format version
- Reducing duplicate information
- Indexing collections

I'm also working on a very basic templated static site to display the analysis results on the web. This could be hosted for the whole community to use.

Futur directions include associating markdown documentation to modules, classes and tags in order to improve the available information by sharing knowledge. I'm considering exploring the idea of producing a Symbol Graph to use the already existing documentation infrastructure [DocC](https://developer.apple.com/documentation/docc).
