using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RimworldExtractor.Templates;

public interface ITemplateEvaluator {

	public abstract string? Evaluate(ref ReadOnlySpan<char> token);

}

public delegate string? TemplateEvaluationHandler(ref ReadOnlySpan<char> token);
public delegate string? TemplateObjectContextEvaluationHandler<TContext>(TContext context, ref ReadOnlySpan<char> token) where TContext : class;

public readonly struct TemplateEvaluator {
	public TemplateEvaluator() { }

	private readonly List<TemplateEvaluationHandler> _handlers = new();

	public readonly string Evaluate(string template)
		=> RegexLibrary.DetectTemplateToken().Replace(template, Evaluate);

	public readonly string Evaluate(ReadOnlySpan<char> token) {
		foreach (TemplateEvaluationHandler handler in _handlers)
			if (handler.Invoke(ref token) is string result)
				return result;
		Debug.WriteLine($"Unknown token '{token}'");
		return string.Empty;
	}

	private readonly string Evaluate(Match match) {
		if (match.Groups[1].ValueSpan is "\\")
			return match.Value;
		string result = Evaluate(match.Groups[2].ValueSpan);
		return $"{match.Groups[1]}{result}";
	}

	public readonly void Add(TemplateEvaluationHandler handler)
		=> _handlers.Add(handler);

	public readonly void Add<TEvaluator>(TEvaluator evaluator) where TEvaluator : ITemplateEvaluator
		=> _handlers.Add(evaluator.Evaluate);

	public readonly void Add<TContext>(TContext context, TemplateObjectContextEvaluationHandler<TContext> handler) where TContext : class
		=> Add(new ObjectContextEvaluator<TContext>(context, handler));

	private readonly struct ObjectContextEvaluator<TContext> : ITemplateEvaluator where TContext : class {

		public ObjectContextEvaluator(TContext context, TemplateObjectContextEvaluationHandler<TContext> handler) {
			_context = context;
			_handler = handler;
		}

		private readonly TContext _context;
		private readonly TemplateObjectContextEvaluationHandler<TContext> _handler;

		public string? Evaluate(ref ReadOnlySpan<char> token)
			=> _handler.Invoke(_context, ref token);

	}
}
