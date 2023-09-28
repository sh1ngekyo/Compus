using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Compus.Application.Exceptions;
public class NoСonnectedTerminalAvailableException : Exception
{
    public NoСonnectedTerminalAvailableException(string sessionId, string storageId) 
        : this($"No available terminal is connected for Session Id: {sessionId} and Storage Id: {storageId}.")
    {
    } 

    public NoСonnectedTerminalAvailableException(string? message) : base(message)
    {
    }

    public NoСonnectedTerminalAvailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
