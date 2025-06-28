namespace Reports.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string objName, object objKey)
            : base($"Entity {objName} ({objKey}) not found.") { }
    }
}
