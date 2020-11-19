
using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A2v10.Workflow
{
	public class ActivityScriptBuilder : IScriptBuilder
	{
		public const String FMAP = "__fmap__";

		private String _declaratons;
		private readonly Dictionary<String, List<String>> _methods = new Dictionary<String, List<String>>();

		private readonly IActivity _activity;

		public ActivityScriptBuilder(IActivity activity)
		{
			_activity = activity;
		}

		public void AddVariables(IEnumerable<IVariable> variables)
		{
			if (variables == null)
				return;
			// declare
			var sb = new StringBuilder();
			foreach (var v in variables)
				sb.Append(
					v.Dir switch
					{
						VariableDirection.Const => $"const {v.Name}; ",
						_ => $"let {v.Name}; "
					}
				);

			_declaratons = sb.ToString();

			var mtds = new List<String>();
			// arguments - In, InOut
			{
				var args = variables.Where(v => v.IsArgument).ToList();
				if (args.Count != 0)
					mtds.Add($"Arguments: (_arg_) => {{ {String.Join("; ", args.Select(x => $"{x.Name} = _arg_.{x.Name}"))}; }}");
			}
			// result - Out, InOut
			{
				var res = variables.Where(v => v.IsResult).ToList();
				if (res.Count != 0)
					mtds.Add($"Result: () => {{return {{ {String.Join(", ", res.Select(x => $"{x.Name} : {x.Name}"))} }}; }}");
			}
			// store, restore - In, Out, Local, not constant!
			{
				var strest = variables.Where(v => v.Dir != VariableDirection.Const).ToList();
				if (strest.Count != 0)
				{
					mtds.Add($"Store: () => {{return {{ {String.Join(", ", strest.Select(x => $"{x.Name} : {x.Name}"))} }}; }}");
					mtds.Add($"Restore: (_arg_) => {{ {String.Join("; ", strest.Select(x => $"{x.Name} = _arg_.{x.Name} "))}; }}");
				}
			}

			if (mtds.Count > 0)
			{
				AddMethods(_activity.Id, mtds);
			}
		}

		void AddMethods(String refer, List<String> methods)
		{
			if (!_methods.ContainsKey(refer))
				_methods.Add(refer, methods);
			else
				_methods[refer].AddRange(methods);
		}

		void AddMethod(String refer, String method)
		{
			if (!_methods.ContainsKey(refer))
				_methods.Add(refer, new List<String>() { method });
			else
				_methods[refer].Add(method);
		}

		public void BuildExecute(String name, String expression)
		{
			AddMethod(_activity.Id, $"{name}: () => {{{expression};}}");
		}

		public void BuildEvaluate(String name, String expression)
		{
			AddMethod(_activity.Id, $"{name}: () => {{ return {expression};}}");
		}

		public String Methods
		{
			get
			{
				var sb = new StringBuilder();
				static String _methodsText(List<String> methods)
				{
					if (methods == null || methods.Count == 0)
						return "null";
					return $"{{\n{String.Join(",\n", methods)}\n}};";
				}

				foreach (var (k, v) in _methods)
					sb.Append($"{FMAP}['{k}'] = {_methodsText(v)}");
				return sb.ToString();
			}
		}

		public String Declarations => _declaratons;
	}

	public class ActivityScript
	{
		private readonly ScriptBuilder _builder;
		private readonly IActivity _activity;

		public ActivityScript(ScriptBuilder builder, IActivity activity)
		{
			_activity = activity;
			_builder = builder;
		}

		public String Ref => _activity.Id;

		public void Build(IActivity activity)
		{
			if (activity is IScriptable scriptable)
			{
				var scriptBuilder = new ActivityScriptBuilder(activity);
				scriptable.BuildScript(scriptBuilder);

				_builder.Append(scriptBuilder.Declarations);
				_builder.Append(scriptBuilder.Methods);
			}
		}
	}

	public class ScriptBuilder
	{
		private readonly Stack<ActivityScript> _stack;
		private readonly StringBuilder _textBuilder;

		public ScriptBuilder()
		{
			_stack = new Stack<ActivityScript>();
			_textBuilder = new StringBuilder();
			_textBuilder.AppendLine("\"use strict\";");
			_textBuilder.AppendLine("() => {");
			_textBuilder.AppendLine($"let {ActivityScriptBuilder.FMAP} = {{}};");
		}

		public void EndScript()
		{
			_textBuilder.AppendLine($"return {ActivityScriptBuilder.FMAP};");
			_textBuilder.AppendLine("};");
		}

		public String Script => _textBuilder.ToString();

		public void Start(IActivity activity)
		{
			if (activity is IScoped)
			{
				var ascript = new ActivityScript(this, activity);
				_stack.Push(ascript);
				_textBuilder.AppendLine("(function() {");
			}
		}

		public void Append(String text)
		{
			if (!String.IsNullOrEmpty(text))
				_textBuilder.AppendLine(text);
		}

		public void End(IActivity activity)
		{
			var ascript = _stack.Peek();
			if (ascript.Ref == activity.Id)
			{
				_textBuilder.AppendLine("})();");
				_stack.Pop();
			}
		}

		public void Build(IActivity activity)
		{
			var ascript = _stack.Peek();
			ascript.Build(activity);
		}
	}
}
