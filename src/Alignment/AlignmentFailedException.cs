using System;

namespace ImageStackerConsole.Alignment
{
    class AlignmentFailedException : Exception
    {

        public AlignmentFailedException()
        {
        }

        public AlignmentFailedException(string message)
            : base(message)
        {
        }

        public AlignmentFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
