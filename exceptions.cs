using System;

public class DivisionLogicException : Exception
{
	public DivisionLogicException()
	{
        public DivisionLogicException() : base("Division logic error.") { }
        public DivisionLogicException(string message) : base(message) { }
        public DivisionLogicException(string message, Exception inner) : base(message, inner) { }
    }
}
