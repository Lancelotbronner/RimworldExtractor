using System.Xml;

namespace RimworldExplorer.Analysis;

public struct WebsiteAnalysisReportWriter {

	public WebsiteAnalysisReportWriter(AnalysisReport report) {
		_report = report;
	}

	private readonly AnalysisReport _report;

	public void Produce() {

	}

	public void ProcessClassTemplate(Span<char> file) {
		//TODO: Token replacements
	}

	#region Templating Management

	public ReadOnlySpan<char> Template(string filename) {
		//TODO: Retrieve template from assembly
		return "";
	}

	private readonly XmlWriterSettings? _settings;

	public void HTML(string filename, Action<XmlWriter> head, Action<XmlWriter> body) {
		XmlWriter document = XmlWriter.Create(File.OpenWrite(filename), _settings);
		document.WriteDocType("html", null, null, null);

		document.WriteStartElement("html");
		document.WriteAttributeString("lang", "en");
		document.WriteAttributeString("dir", "ltr");

		document.WriteStartElement("head");
		WriteHead(document, _report.Title, _report.Version);
		head.Invoke(document);
		document.WriteEndElement();

		document.WriteStartElement("body");
		body.Invoke(document);
		document.WriteEndElement();

		document.WriteEndDocument();
	}

	#endregion

	#region Section Methods

	public void WriteHead(XmlWriter document, string title, string version) {
		// Title tag
		document.WriteStartElement("title");
		document.WriteValue($"{title} v{version}");
		document.WriteEndElement();

		// Keywords tag
		document.WriteStartElement("meta");
		document.WriteAttributeString("name", "keywords");
		document.WriteAttributeString("content", "RimWorld");
		document.WriteEndElement();

		// Description tag
		document.WriteStartElement("meta");
		document.WriteAttributeString("name", "description");
		document.WriteAttributeString("content", $"Auto-generated {title} v{version} documentation");
		document.WriteEndElement();
	}

	#endregion

	#region Fragment Methods

	public void LinkToTag(XmlWriter document, AnalysisTag tag)
		=> document.WriteAttributeString("href", GetTagAnchor(tag));

	public void LinkToDefinition(XmlWriter document, AnalysisDefinition definition)
		=> document.WriteAttributeString("href", GetDefinitionAnchor(definition));

	#endregion

	#region Utility Methods

	public static string GetTagAnchor(AnalysisTag tag)
		=> $"#tag-{tag.Name}";

	public static string GetDefinitionAnchor(AnalysisDefinition definition)
		=> $"#definition-{definition.TypedIdentifier}";

	#endregion

}
