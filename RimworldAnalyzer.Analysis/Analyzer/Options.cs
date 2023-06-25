using System.Runtime.InteropServices;

namespace RimworldAnalyzer.Analyzer;

public sealed class AnalyzerOptions {

	public AnalyzerOptions() {
		// Configure attribute defaults
		_attribute = AttributeBehaviour.CollectExamples;
		SetBehaviour(AttributeBehaviour.CollectExamples, false, "Name");

		// Configure tag defaults
		_tag = TagBehaviour.Traverse | TagBehaviour.Attributes | TagBehaviour.CollectExamples;
		SetBehaviour(TagBehaviour.Transient, true, "li");
		SetBehaviour(TagBehaviour.CollectExamples | TagBehaviour.Traverse, false, "li", "defName");
	}

	#region Attribute Behaviour Management

	internal AttributeBehaviour _attribute;
	internal readonly Dictionary<string, AttributeBehaviour> _attributes = new();

	public AttributeBehaviour DefaultAttributeBehaviour => _attribute;

	public AttributeBehaviour BehaviourOfAttribute(string tag)
		=> _attributes.TryGetValue(tag, out AttributeBehaviour behaviour) ? behaviour : _attribute;

	public bool BehaviourOfAttribute(string attribute, AttributeBehaviour mask)
		=> BehaviourOfAttribute(attribute).HasFlag(mask);

	public AttributeBehaviour SetDefaultBehaviour(AttributeBehaviour mask, bool enabled) {
		if (enabled)
			_attribute |= mask;
		else
			_attribute &= ~mask;
		return _attribute;
	}

	public AttributeBehaviour SetBehaviour(AttributeBehaviour mask, bool enabled, string attribute) {
		ref AttributeBehaviour behaviour = ref CollectionsMarshal.GetValueRefOrAddDefault(_attributes, attribute, out bool exists);
		if (!exists)
			behaviour = _attribute;
		if (enabled)
			behaviour |= mask;
		else
			behaviour &= ~mask;
		return behaviour;
	}

	public void SetBehaviour(AttributeBehaviour mask, bool enabled, IEnumerable<string> attributes) {
		foreach (string attribute in attributes)
			SetBehaviour(mask, enabled, attribute);
	}

	public void SetBehaviour(AttributeBehaviour mask, bool enabled, params string[] attributes)
		=> SetBehaviour(mask, enabled, attributes.AsEnumerable());

	#endregion

	#region Tag Behaviour Management

	internal TagBehaviour _tag;
	internal readonly Dictionary<string, TagBehaviour> _tags = new();

	public TagBehaviour DefaultTagBehaviour => _tag;

	public TagBehaviour BehaviourOfTag(string tag)
		=> _tags.TryGetValue(tag, out TagBehaviour behaviour) ? behaviour : _tag;

	public bool BehaviourOfTag(string tag, TagBehaviour mask)
		=> BehaviourOfTag(tag).HasFlag(mask);

	public TagBehaviour SetDefaultBehaviour(TagBehaviour mask, bool enabled) {
		if (enabled)
			_tag |= mask;
		else
			_tag &= ~mask;
		return _tag;
	}

	public TagBehaviour SetBehaviour(TagBehaviour mask, bool enabled, string tag) {
		ref TagBehaviour behaviour = ref CollectionsMarshal.GetValueRefOrAddDefault(_tags, tag, out bool exists);
		if (!exists)
			behaviour = _tag;
		if (enabled)
			behaviour |= mask;
		else
			behaviour &= ~mask;
		return behaviour;
	}

	public void SetBehaviour(TagBehaviour mask, bool enabled, IEnumerable<string> tags) {
		foreach (string tag in tags)
			SetBehaviour(mask, enabled, tag);
	}

	public void SetBehaviour(TagBehaviour mask, bool enabled, params string[] tags)
		=> SetBehaviour(mask, enabled, tags.AsEnumerable());

	#endregion

}

[Flags]
public enum TagBehaviour : byte {

	/// <summary>
	/// The tag's children will be analyzed
	/// </summary>
	Traverse = 0x1,

	/// <summary>
	/// The tag's attributes will be analyzed
	/// </summary>
	Attributes = 0x2,

	/// <summary>
	/// Whenever the tag is encountered an example will be recorded
	/// </summary>
	CollectExamples = 0x4,

	/// <summary>
	/// The tag will be considered as introducing an element of a list.
	/// </summary>
	Transient = 0x8,

}

[Flags]
public enum AttributeBehaviour : byte {

	/// <summary>
	/// Whenever the attribute is encountered an example will be recorded
	/// </summary>
	CollectExamples = 0x4,

}
