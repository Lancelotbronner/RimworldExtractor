//using System.Xml;

//namespace RimworldExtractor.Analysis;

//public struct HtmlAnalysisReportWriter {

//	public HtmlAnalysisReportWriter(AnalysisReport report, Stream stream, XmlWriterSettings? setting = null) {
//		_report = report;
//		_document = XmlWriter.Create(stream, setting);
//	}

//	private readonly AnalysisReport _report;
//	private readonly XmlWriter _document;

//	public void Produce() {
//		_document.WriteDocType("html", null, null, null);

//		_document.WriteStartElement("html");
//		_document.WriteAttributeString("lang", "en");
//		_document.WriteAttributeString("dir", "ltr");

//		_document.WriteStartElement("head");
//		WriteHead();
//		_document.WriteEndElement();

//		_document.WriteStartElement("body");
//		WriteBody();
//		_document.WriteEndElement();

//		_document.WriteEndDocument();
//	}

//	#region Section Methods

//	private void WriteHead() {
//		// Title tag
//		_document.WriteStartElement("title");
//		_document.WriteValue($"{_report.Title} v{_report.Version}");
//		_document.WriteEndElement();

//		// Keywords tag
//		_document.WriteStartElement("meta");
//		_document.WriteAttributeString("name", "keywords");
//		_document.WriteAttributeString("content", "RimWorld");
//		_document.WriteEndElement();

//		// Description tag
//		_document.WriteStartElement("meta");
//		_document.WriteAttributeString("name", "description");
//		_document.WriteAttributeString("content", $"Auto-generated {_report.Title} v{_report.Version} tag documentation");
//		_document.WriteEndElement();
//	}

//	private void WriteBody() {
//		_document.WriteStartElement("h1");
//		_document.WriteValue($"{_report.Title} Auto Documentation for {_report.Version}");
//		_document.WriteEndElement();

//		_document.WriteStartElement("h5");
//		_document.WriteValue($"Made by Lancelotbronner, based on Epicguru, itself based on milon's work.");
//		_document.WriteEndElement();

//		_document.WriteStartElement("nav");
//		WriteNavigation();
//		_document.WriteEndElement();

//		_document.WriteStartElement("main");
//		WriteContent();
//		_document.WriteEndElement();
//	}

//	#endregion

//	#region Navigation Methods

//	private void WriteNavigation() {
//		_document.WriteStartElement("section");
//		WriteModuleNavigation();
//		_document.WriteEndElement();

//		_document.WriteStartElement("section");
//		WriteClassNavigation();
//		_document.WriteEndElement();

//		_document.WriteStartElement("section");
//		WriteTagNavigation();
//		_document.WriteEndElement();

//		_document.WriteStartElement("section");
//		WriteDefinitionNavigation();
//		_document.WriteEndElement();
//	}

//	private void WriteModuleNavigation() {
//		_document.WriteStartElement("h2");
//		_document.WriteValue("Modules");
//		_document.WriteEndElement();

//		foreach (AnalysisModule module in _report.Modules) {
//			_document.WriteStartElement("a");
//			_document.WriteAttributeString("href", $"#module-{module.Identifier}");
//			_document.WriteValue(module.Identifier);
//			_document.WriteEndElement();
//		}
//	}

//	private void WriteClassNavigation() {
//		_document.WriteStartElement("h2");
//		_document.WriteValue("Classes");
//		_document.WriteEndElement();

//		foreach (AnalysisClass @class in _report.Classes) {
//			_document.WriteStartElement("a");
//			_document.WriteAttributeString("href", $"#class-{@class.Name}");
//			_document.WriteValue(@class.Name);
//			_document.WriteEndElement();
//		}
//	}

//	private void WriteTagNavigation() {
//		_document.WriteStartElement("h2");
//		_document.WriteValue("Tags");
//		_document.WriteEndElement();

//		foreach (AnalysisTag tag in _report.Tags) {
//			_document.WriteStartElement("a");
//			LinkToTag(tag);
//			_document.WriteValue(tag.Name);
//			_document.WriteEndElement();
//		}
//	}

//	private void WriteDefinitionNavigation() {
//		_document.WriteStartElement("h2");
//		_document.WriteValue("Definitions");
//		_document.WriteEndElement();

//		foreach (AnalysisDefinition definition in _report.Definitions) {
//			_document.WriteStartElement("a");
//			_document.WriteAttributeString("href", $"#definition-{definition.TypedIdentifier}");
//			_document.WriteValue(definition.Declaration);
//			_document.WriteEndElement();
//		}
//	}

//	#endregion

//	#region Content Methods

//	private void WriteContent() {
//		_document.WriteStartElement("section");
//		WriteModuleContent();
//		_document.WriteEndElement();

//		_document.WriteStartElement("section");
//		WriteClassContent();
//		_document.WriteEndElement();

//		_document.WriteStartElement("section");
//		WriteTagContent();
//		_document.WriteEndElement();

//		_document.WriteStartElement("section");
//		WriteDefinitionContent();
//		_document.WriteEndElement();
//	}

//	private void WriteModuleContent() {
//		foreach (AnalysisModule module in _report.Modules) {
//			_document.WriteStartElement("article");
//			_document.WriteAttributeString("id", $"#module-{module.Identifier}");
//			_document.WriteAttributeString("class", "module");

//			_document.WriteStartElement("h3");
//			_document.WriteValue(module.Identifier);
//			_document.WriteEndElement();

//			_document.WriteEndElement();
//			_document.WriteStartElement("hr");
//		}
//	}

//	private void WriteClassContent() {
//		foreach (AnalysisClass @class in _report.Classes) {
//			_document.WriteStartElement("article");
//			_document.WriteAttributeString("id", $"#class-{@class.Name}");
//			_document.WriteAttributeString("class", "class");

//			_document.WriteStartElement("h3");
//			_document.WriteValue(@class.Name);
//			_document.WriteEndElement();

//			_document.WriteEndElement();
//			_document.WriteStartElement("hr");
//		}
//	}

//	private void WriteTagContent() {
//		foreach (AnalysisTag tag in _report.Tags) {
//			_document.WriteStartElement("article");
//			_document.WriteAttributeString("id", GetTagAnchor(tag));
//			_document.WriteAttributeString("class", "tag");

//			_document.WriteStartElement("h3");
//			_document.WriteValue(tag.Name);
//			_document.WriteEndElement();

//			if (tag.Parents.Count > 0)
//				WriteTagArticleParents(tag);

//			if (tag.Children.Count > 0)
//				WriteTagArticleChildren(tag);

//			WriteTagArticleExamples(tag);

//			_document.WriteStartElement("hr");
//		}
//	}

//	private void WriteTagArticleParents(AnalysisTag tag) {
//		_document.WriteStartElement("section");
//		_document.WriteAttributeString("class", "parents");

//		_document.WriteStartElement("h4");
//		_document.WriteValue($"Parents ({tag.Parents.Count})");
//		_document.WriteEndElement();

//		_document.WriteStartElement("ul");
//		foreach (AnalysisTag parent in tag.Parents) {
//			_document.WriteStartElement("li");
//			_document.WriteStartElement("a");
//			LinkToTag(parent);
//			_document.WriteValue(parent.Name);
//			_document.WriteEndElement();
//			_document.WriteEndElement();
//		}

//		_document.WriteEndElement();
//		_document.WriteEndElement();
//	}

//	private void WriteTagArticleChildren(AnalysisTag tag) {
//		_document.WriteStartElement("section");
//		_document.WriteAttributeString("class", "children");

//		_document.WriteStartElement("h4");
//		_document.WriteValue($"Children ({tag.Children.Count})");
//		_document.WriteEndElement();

//		_document.WriteStartElement("ul");
//		foreach (AnalysisTag child in tag.Children) {
//			_document.WriteStartElement("li");
//			_document.WriteStartElement("a");
//			LinkToTag(child);
//			_document.WriteValue(child.Name);
//			_document.WriteEndElement();
//			_document.WriteEndElement();
//		}

//		_document.WriteEndElement();
//		_document.WriteEndElement();
//	}

//	private void WriteTagArticleExamples(AnalysisTag tag) {
//		_document.WriteStartElement("section");
//		_document.WriteAttributeString("class", "examples");

//		_document.WriteStartElement("h4");
//		_document.WriteValue($"Examples ({tag.Uses.Count})");
//		_document.WriteEndElement();

//		//TODO: Add tooltips
//		_document.WriteStartElement("dl");
//		foreach (var group in tag.UsageByParent) {
//			_document.WriteStartElement("dt");
//			_document.WriteValue("when used");
//			_document.WriteStartElement("dfn");
//			_document.WriteValue(group.Key?.Name ?? "as root");
//			_document.WriteEndElement();
//			_document.WriteEndElement();

//			foreach (PropertyUsage usage in group) {
//				_document.WriteStartElement("dd");

//				_document.WriteStartElement("span");
//				_document.WriteValue(usage.Value);
//				_document.WriteEndElement();

//				_document.WriteStartElement("a");
//				LinkToDefinition(usage.Definition);
//				_document.WriteValue(usage.Definition.ToString());
//				_document.WriteEndElement();

//				_document.WriteEndElement();
//			}
//		}

//		_document.WriteEndElement();
//		_document.WriteEndElement();
//	}

//	private void WriteDefinitionContent() {
//		foreach (AnalysisDefinition definition in _report.Definitions) {
//			_document.WriteStartElement("article");
//			_document.WriteAttributeString("id", GetDefinitionAnchor(definition));
//			_document.WriteAttributeString("class", "definition");

//			_document.WriteStartElement("h3");
//			_document.WriteValue(definition.Declaration);
//			_document.WriteEndElement();

//			_document.WriteEndElement();
//			_document.WriteStartElement("hr");
//		}
//	}

//	#endregion



//	//private string MakeTooltip(IEnumerable<Source> sources) {
//	//	StringBuilder str = new StringBuilder();
//	//	str.AppendLine("Examples:").AppendLine();
//	//	foreach (var src in sources) {
//	//		string filePath = src.File;
//	//		filePath = filePath.Substring(filePath.IndexOf("Defs") + 5);
//	//		str.Append("Def: ").Append(src.DefName ?? "???").Append(", File: ").AppendLine(filePath);
//	//	}
//	//	return str.ToString().TrimEnd();
//	//}

//	//private HashSet<string> alreadyLinked = new HashSet<string>();

//	//private string MakeSee(IEnumerable<Source> sources, bool links) {
//	//	alreadyLinked.Clear();
//	//	StringBuilder str = new StringBuilder();
//	//	str.Append("(see: ");

//	//	bool first = true;
//	//	int i = 0;
//	//	foreach (var src in sources) {
//	//		if (!alreadyLinked.Add(src.File))
//	//			continue;

//	//		if (links)
//	//			str.Append("<a href=\"file:///").Append(src.File.Replace('/', '\\')).Append("\" target=\"popup\">");
//	//		else
//	//			str.Append("<a>");
//	//		if (!first)
//	//			str.Append(", ");
//	//		str.Append(new FileInfo(src.File).Name).Append("</a>");

//	//		first = false;

//	//		if (i++ > 4) {
//	//			str.Append(" ...");
//	//			break;
//	//		}
//	//	}
//	//	str.Append(")");
//	//	return str.ToString();
//	//}

//	#region Fragment Methods

//	public void LinkToTag(AnalysisTag tag)
//		=> _document.WriteAttributeString("href", GetTagAnchor(tag));

//	public void LinkToDefinition(AnalysisDefinition definition)
//		=> _document.WriteAttributeString("href", GetDefinitionAnchor(definition));

//	#endregion

//	#region Utility Methods

//	public static string GetTagAnchor(AnalysisTag tag)
//		=> $"#tag-{tag.Name}";

//	public static string GetDefinitionAnchor(AnalysisDefinition definition)
//		=> $"#definition-{definition.TypedIdentifier}";

//	#endregion

//}
