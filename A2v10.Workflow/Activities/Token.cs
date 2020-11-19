using A2v10.Workflow.Interfaces;
using System;

namespace A2v10.Workflow
{
	public struct Token : IToken
	{
		public Guid Id { get; init; }

		private Token(Guid guid)
		{
			Id = guid;
		}

		public static IToken Create()
		{
			return new Token(Guid.NewGuid());
		}

		public override string ToString()
		{
			return Id.ToString();
		}
	}
}
