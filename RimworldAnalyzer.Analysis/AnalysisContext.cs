using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

public sealed class AnalysisDatabase : DbContext {

	public AnalysisDatabase(DbContextOptions<AnalysisDatabase> options) : base(options) {

	}

	public async Task Apply<TTransformation>(TTransformation transformation) where TTransformation : IArchiveTransformation {
		Database.BeginTransaction();
		try {
			await transformation.Transform(this);
		} catch {
			Database.RollbackTransaction();
		}
		Database.CommitTransaction();
	}

	#region Attribute Management

	public DbSet<AttributeTable> Attributes { get; private set; }

	private readonly Dictionary<string, AttributeTable> _attributes = new();

	public Task<AttributeTable?> GetAttribute(string identifier)
		=> Get(_attributes, identifier, row => row.Identifier == identifier);

	public Task<AttributeTable> GetOrCreateAttribute(string identifier)
		=> GetOrCreate(_attributes, identifier, GetAttribute, () => new(identifier));

	#endregion

	#region Attribute Example Management

	public DbSet<AttributeExampleTable> AttributeExamples { get; private set; }

	private readonly Dictionary<(AttributeTable, TagTable, ExampleTable), AttributeExampleTable> _attributeExamples = new();

	public async Task<AttributeExampleTable?> GetAttributeExample(AttributeTable attribute, TagTable tag, ExampleTable example) {
		AttributeExampleTable? result = await Get(_attributeExamples, (attribute, tag, example), row => row.AttributeId == attribute.Id && row.TagId == tag.Id && row.ExampleId == example.Id);
		if (result is not null) {
			result.Attribute = attribute;
			result.Tag = tag;
			result.Example = example;
		}
		return result;
	}

	public Task<AttributeExampleTable> GetOrCreateAttributeExample(AttributeTable attribute, TagTable tag, ExampleTable example)
		=> GetOrCreate(_attributeExamples, (attribute, tag, example), key => GetAttributeExample(key.Item1, key.Item2, key.Item3), () => new(attribute, tag, example));

	#endregion

	#region Attribute Usage Management

	public DbSet<AttributeUsageTable> AttributeUsage { get; private set; }

	private readonly Dictionary<(DefinitionTable, AttributeExampleTable), AttributeUsageTable> _attributeUsage = new();

	public async Task<AttributeUsageTable?> GetAttributeUsage(DefinitionTable definition, AttributeExampleTable example) {
		AttributeUsageTable? result = await Get(_attributeUsage, (definition, example), row => row.DefinitionId == definition.Id && row.ExampleId == example.Id);
		if (result is not null) {
			result.Definition = definition;
			result.Example = example;
		}
		return result;
	}

	public Task<AttributeUsageTable> GetOrCreateAttributeUsage(DefinitionTable definition, AttributeExampleTable example)
		=> GetOrCreate(_attributeUsage, (definition, example), key => GetAttributeUsage(key.Item1, key.Item2), () => new(definition, example));

	#endregion

	#region Class Management

	public DbSet<ClassTable> Classes { get; private set; }

	private readonly Dictionary<string, ClassTable> _classes = new();

	public Task<ClassTable?> GetClass(string identifier)
		=> Get(_classes, identifier, row => row.Identifier == identifier);

	public Task<ClassTable> GetOrCreateClass(string identifier)
		=> GetOrCreate(_classes, identifier, GetClass, () => new(identifier));

	#endregion

	#region Definition Management

	public DbSet<DefinitionTable> Definitions { get; private set; }

	private readonly Dictionary<string, DefinitionTable> _definitions = new();

	public Task<DefinitionTable?> GetDefinition(string identifier)
		=> Get(_definitions, identifier, row => row.Identifier == identifier);

	public Task<DefinitionTable> GetOrCreateDefinition(string identifier)
		=> GetOrCreate(_definitions, identifier, GetDefinition, () => new(identifier));

	#endregion

	#region Example Management

	public DbSet<ExampleTable> Examples { get; private set; }

	private readonly Dictionary<string, ExampleTable> _examples = new();

	public Task<ExampleTable?> GetExample(string value)
		=> Get(_examples, value, row => row.Value == value);

	public Task<ExampleTable> GetOrCreateExample(string value)
		=> GetOrCreate(_examples, value, GetExample, () => new(value));

	#endregion

	#region Issue Management

	public DbSet<IssueTable> Issues { get; private set; }

	public IQueryable<IssueTable> Warnings => Issues
		.AsNoTracking()
		.Where(issue => issue.Severity == IssueSeverity.Warning);

	public IQueryable<IssueTable> Errors => Issues
		.AsNoTracking()
		.Where(issue => issue.Severity == IssueSeverity.Error);

	public IssueTable Warning(string message) {
		IssueTable issue = new() {
			Severity = IssueSeverity.Warning,
			Message = message,
		};
		Issues.Add(issue);
		Debug.WriteLine($"[WARNING] {message}");
		return issue;
	}

	public IssueTable Warning(string filepath, string message)
		=> Warning($"[{filepath}] {message}");

	public IssueTable Error(string message) {
		IssueTable issue = new() {
			Severity = IssueSeverity.Error,
			Message = message,
		};
		Issues.Add(issue);
		Debug.WriteLine($"[ERROR] {message}");
		return issue;
	}

	public IssueTable Error(string filepath, string message)
		=> Error($"[{filepath}] {message}");

	#endregion

	#region Module Management

	public DbSet<ModuleTable> Modules { get; private set; }

	private readonly Dictionary<string, ModuleTable> _modules = new();

	public Task<ModuleTable?> GetModule(string identifier)
		=> Get(_modules, identifier, row => row.Identifier == identifier);

	public Task<ModuleTable> GetOrCreateModule(string identifier)
		=> GetOrCreate(_modules, identifier, GetModule, () => new(identifier));

	#endregion

	#region Relationship Management

	public DbSet<RelationshipTable> Relationships { get; private set; }

	private readonly Dictionary<(TagTable, TagTable, TagTable?), RelationshipTable> _relationships = new();

	public async Task<RelationshipTable?> GetRelationship(TagTable parent, TagTable child, TagTable? context) {
		int? contextId = context?.Id;
		RelationshipTable? relationship = await Get(_relationships, (parent, child, context), row => row.ParentId == parent.Id && row.ChildId == child.Id && row.ContextId == contextId);
		if (relationship is not null) {
			relationship.Parent = parent;
			relationship.Child = child;
			relationship.Context = context;
		}
		return relationship;
	}

	public Task<RelationshipTable> GetOrCreateRelationship(TagTable parent, TagTable child, TagTable? context)
		=> GetOrCreate(_relationships, (parent, child, context), key => GetRelationship(key.Item1, key.Item2, key.Item3), () => new(parent, child, context));

	#endregion

	#region Resource Management

	public DbSet<ResourceTable> Resources { get; private set; }

	private readonly Dictionary<(string, ModuleTable), ResourceTable> _resources = new();

	public async Task<ResourceTable?> GetResource(string path, ModuleTable module) {
		ResourceTable? resource = await Get(_resources, (path, module), row => row.Path == path && row.ModuleId == module.Id);
		if (resource is not null)
			resource.Module = module;
		return resource;
	}

	public Task<ResourceTable> GetOrCreateResource(string path, ModuleTable module)
		=> GetOrCreate(_resources, (path, module), key => GetResource(key.Item1, key.Item2), () => new() {
			Path = path,
			ModuleId = module.Id,
			Module = module,
		});

	#endregion

	#region Tag Management

	public DbSet<TagTable> Tags { get; private set; }

	private readonly Dictionary<string, TagTable> _tags = new();

	public Task<TagTable?> GetTag(string identifier)
		=> Get(_tags, identifier, row => row.Identifier == identifier);

	public Task<TagTable> GetOrCreateTag(string identifier)
		=> GetOrCreate(_tags, identifier, GetTag, () => new(identifier));

	#endregion

	#region Tag Example Management

	public DbSet<TagExampleTable> TagExamples { get; private set; }

	private readonly Dictionary<(RelationshipTable, ExampleTable), TagExampleTable> _tagExamples = new();

	public async Task<TagExampleTable?> GetTagExample(RelationshipTable relationship, ExampleTable example) {
		TagExampleTable? result = await Get(_tagExamples, (relationship, example), row => row.RelationshipId == relationship.Id && row.ExampleId == example.Id);
		if (result is not null) {
			result.Relationship = relationship;
			result.Example = example;
		}
		return result;
	}

	public Task<TagExampleTable> GetOrCreateTagExample(RelationshipTable relationship, ExampleTable example)
		=> GetOrCreate(_tagExamples, (relationship, example), key => GetTagExample(key.Item1, key.Item2), () => new(relationship, example));

	#endregion

	#region Tag Usage Management

	public DbSet<TagUsageTable> TagUsage { get; private set; }

	private readonly Dictionary<(DefinitionTable, TagExampleTable), TagUsageTable> _tagUsage = new();

	public async Task<TagUsageTable?> GetTagUsage(DefinitionTable definition, TagExampleTable example) {
		TagUsageTable? result = await Get(_tagUsage, (definition, example), row => row.DefinitionId == definition.Id && row.ExampleId == example.Id);
		if (result is not null) {
			result.Definition = definition;
			result.Example = example;
		}
		return result;
	}

	public Task<TagUsageTable> GetOrCreateTagUsage(DefinitionTable definition, TagExampleTable example)
		=> GetOrCreate(_tagUsage, (definition, example), key => GetTagUsage(key.Item1, key.Item2), () => new(definition, example));

	#endregion

	#region Cache Management

	private async Task<TValue?> Get<TKey, TValue>(Dictionary<TKey, TValue> cache, TKey key, Expression<Func<TValue, bool>> filter) where TKey : notnull where TValue : class {
		if (cache.TryGetValue(key, out TValue? table))
			return table;
		table = await Set<TValue>()
			.Where(filter)
			.SingleOrDefaultAsync();
		return table;
	}

	private async Task<TValue> GetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> cache, TKey key, Func<TKey, Task<TValue?>> get, Func<TValue> create) where TKey : notnull where TValue : class {
		if (await get.Invoke(key) is TValue value)
			return value;
		value = create.Invoke();
		Add(value);
		cache[key] = value;
		return value;
	}

	#endregion

}
