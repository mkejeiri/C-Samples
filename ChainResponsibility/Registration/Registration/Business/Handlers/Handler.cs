using System;

namespace Registration.Business.Handlers
{
    public abstract class Handler<T> : IHandler<T> where T : class
    {
        private IHandler<T> Next { get; set; }

        public virtual void Handle(T request)
        {

            Console.WriteLine(Next?.GetType().Name);
            Next?.Handle(request);
        }

        public IHandler<T> SetNext(IHandler<T> next)
        {
            Next = next;

            return Next;
        }
    }

    public interface IHandler<T> where T : class
    {
        IHandler<T> SetNext(IHandler<T> next);
        void Handle(T request);
    }
}
