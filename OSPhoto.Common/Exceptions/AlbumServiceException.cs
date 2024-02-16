namespace OSPhoto.Common.Exceptions;

public class AlbumServiceException : Exception
{
    private readonly string _id;
    private readonly string _message;
    public AlbumServiceException(string id, string message, Exception exception) : base(message, exception)
    {
        _id = id;
        _message = message;
    }

    public override string Message
    {
        get
        {
#if DEBUG
                return $"There was a problem reading '{_id}': {_message}";
#else
                return $"There was a problem reading '{_id}'";
#endif
        }
    }
}
