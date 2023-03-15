﻿namespace ExcelToCSharpModelConverter.Core.Exceptions;

public class FileContentIsEmptyException : Exception
{
    public FileContentIsEmptyException() : base("File content is empty")
    {
        
    }
}
