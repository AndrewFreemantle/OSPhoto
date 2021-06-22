using System;

namespace OSPhoto.Services.Exceptions
{
    public class AlbumServiceException : Exception
    {
        private readonly string _path;
        private readonly string _message;
        public AlbumServiceException(string path, string message, Exception exception) : base(message, exception)
        {
            _path = path;
            _message = message;
        }

        public override string Message
        {
            get
            {
#if DEBUG
                    return $"There was a problem reading '{_path}': {_message}";
#else
                    return $"There was a problem reading '{_path}'";
#endif
            }
        }
    }
}