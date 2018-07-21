using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 Behavioral Patterns are about changing runtime behaviors of algorithms and classes.
 With the Observer Pattern it’s possible for a class, a so-called observable, to notify other classes, observers, of changes.
 */
namespace ObserverPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Stock stock = new Stock("Cheese"); 
            stock.NoOfItemsInStock = 10; // Register observers. 
            Seller seller = new Seller(); 
            stock.Attach(seller); 
            Buyer buyer = new Buyer(); 
            stock.Attach(buyer); 
            stock.NoOfItemsInStock = 4;
            //stock.NoOfItemsInStock = 1;
            Console.ReadKey();
        }
    }

    public interface IListerner<T>
	{
        void Update(T observable);
	}

    public interface INotifier<T>
    {
        void Attach(IListerner<T> observer);
        void Detach(IListerner<T> observer);
        void Notify();        
    }

    public class Stock : INotifier<Stock> 
    {
        private List<IListerner<Stock>> observers = new List<IListerner<Stock>>();
        private string productName; 
        private int noOfItemsInStock; 
        public Stock(string name) { 
            productName = name; 
        }
        public void Attach(IListerner<Stock> observer) 
        {
            observers.Add(observer); 
        }
        public void Detach(IListerner<Stock> observer) 
        { 
            observers.Remove(observer); 
        } 
        public void Notify() {
            foreach (IListerner<Stock> observer in observers) 
            { 
                observer.Update(this); 
            }
        } 
        public string ProductName {
            get { 
                return productName;
            } 
        } 
        public int NoOfItemsInStock { 
            get { 
                return noOfItemsInStock;
            } 
            set { 
                noOfItemsInStock = value;
                Notify(); 
            } 
        } 
    }

    public class Seller : IListerner<Stock>
    {
        public void Update(Stock observable) 
        {
            Console.WriteLine(String.Format("Seller was notified about the stock change of {0} to {1} items.", observable.ProductName, observable.NoOfItemsInStock));
        } 
    }
    public class Buyer : IListerner<Stock>
    {
        public void Update(Stock observable)
        {
            Console.WriteLine(String.Format("Buyer was notified about the stock change of {0} to {1} items.", observable.ProductName, observable.NoOfItemsInStock));
        } 
    }
}
