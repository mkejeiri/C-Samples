using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 
  Structural Patterns are about making objects work together and structuring your code accordingly
  The Decorator Pattern can be used to add functionality to an object dynamically
 */
namespace DecoratorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            IMessage msg= new Message();
            msg.Msg = "testing the service ...";
            //IMessage emailDecorator = new EmailDecorator(new FaxDecorator(new ExternalSystemDecorator(msg)));
            //emailDecorator.Process();

            IMessage decorator = new EmailDecorator(new FaxDecorator(new ExternalSystemDecorator(msg))); 
            decorator.Process();

            Console.WriteLine();

            decorator = new EmailDecorator(msg);
            decorator.Msg = "Bye"; 
            decorator.Process();
            Console.ReadKey();

        }
    }

    public interface IMessage
    {
        string Msg { set; get; }
        void Process();
    }

    public class Message : IMessage
    {

        public string Msg { set; get; }

        public void Process()
        {           
            Console.WriteLine(String.Format("Saved '{0}' to database.",Msg));
        }
    }

    public abstract class BaseMessageDecorator: IMessage
    {
        private IMessage innerMessage;
        public BaseMessageDecorator(IMessage decorator)
        {
            innerMessage = decorator;        
        }
              
        public virtual void Process()
        {
           innerMessage.Process();
        }

        public string Msg {
            get {
                return this.innerMessage.Msg;
            }
            set {
                this.innerMessage.Msg = value;
            } 
        }
        
    }
    public class EmailDecorator: BaseMessageDecorator
    {
        public EmailDecorator(IMessage decorator) : base(decorator) {}

        public override void Process()
        {
            base.Process();
            Console.WriteLine(String.Format("Sent '{0}' as email", Msg));
        }        
    }

    public class FaxDecorator: BaseMessageDecorator
    {
        public FaxDecorator(IMessage decorator) : base(decorator) { }
        public override void Process()
        {
            base.Process();
            Console.WriteLine(String.Format("Sent '{0}' as Fax.", Msg));
        }
        
    }

    public class ExternalSystemDecorator: BaseMessageDecorator
    {
        public ExternalSystemDecorator(IMessage decorator) :base(decorator){}
        public override void Process()
        {
            base.Process();
            Console.WriteLine(String.Format("Sent '{0}' to external system", Msg));
        }        
    }
}
