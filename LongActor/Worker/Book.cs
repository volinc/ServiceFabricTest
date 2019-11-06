namespace Worker
{
    using System;
    
    public class Book
    {
        public Book()
        {
            Number = Guid.NewGuid();
        }

        public Guid Number { get; }

        public override string ToString() => $"Book {Number}";
    }
}
