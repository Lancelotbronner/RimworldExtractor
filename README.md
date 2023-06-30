# Rimworld Extractor

This is a very simple tool to extract and analyze XML tag usage of Rimworld mods.

This is currently in development. It can produce reports and still needs transformation commands.

```sh
# Analyze official modules, like Core and expansions
rimworld-analyzer --include-officials
```

## Features

- Collects XML tag and attribute examples
- Includes tag context, so you will know the parent of tags under `li`
- Includes modules, definitions, and filepaths for a better overview
- Consume those reports as either SQLite or JSON

## Roadmap

I have some functionality to configure the behaviour per tag and attributes.
I'd like to add them to the commands and possibly expand on them.

Here's what I'd like to improve on the analysis:

- Type inference from examples
- Inferring polymorphic relationships on tags
- Inferring tag collections (arrays with `li`)

I'd like to add the possibility to ignore usage or resources while analyzing to reduce the size of the report.
In the meantime a transformation command to strip them should be sufficient.

Futur directions include associating markdown documentation to modules, classes, tags and attributes in order to improve the available information by sharing knowledge.
I'm considering exploring the idea of producing a Symbol Graph to use the already existing documentation infrastructure of  [DocC](https://developer.apple.com/documentation/docc).
