using System;

namespace CluedIn.ExternalSearch.Providers.DnB.Custom;

internal class BadTokenException : Exception
{
    public BadTokenException() { }

    public BadTokenException(string message)
        : base(message) { }

    public BadTokenException(string message, Exception innerException)
        : base(message, innerException) { }
}