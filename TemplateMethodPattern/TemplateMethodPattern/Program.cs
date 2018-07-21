using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 
 */
namespace TemplateMethodPattern
{
    class Program
    {
        static void Main(string[] args)
        {

            CustomerRetriever retriever = new CustomerRetriever("connString");
            DataTable table = retriever.RetrieveData();
        }
    }


    public abstract class BaseDataRetriever { 
        public DataTable RetrieveData() { 
            Connect(); 
            string query = GetQuery(); 
            DataTable result = ExecQuery(query); 
            Close(); return result; 
        } 
        protected abstract void Connect(); 
        protected abstract string GetQuery(); 
        protected abstract DataTable ExecQuery(string query); 
        protected abstract void Close(); 
    }


    public abstract class SqlDataRetriever : BaseDataRetriever  { 
        private string connString; 
        private SqlConnection connection; 
        public SqlDataRetriever(string connString) { 
            this.connString = connString; } 
        protected override void Connect() { 
            connection = new SqlConnection(connString); 
        } 
        //protected override string GetQuery() { 
        //    return "SELECT Id, Name, Price FROM Product"; 
        //} 
        protected override DataTable ExecQuery(string query) { 
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection)) { 
                DataTable result = new DataTable(); 
                adapter.Fill(result); return result; 
            } 
        } 
        protected override void Close() { 
            connection.Dispose(); 
            connection = null; 
        } 
    }


    public class ProductRetriever : SqlDataRetriever {
        public ProductRetriever(string connString) : base(connString) { } 
        protected override string GetQuery() {return "SELECT Id, Name, Price FROM Product"; } 
    }

    public class CustomerRetriever : SqlDataRetriever { 
        public CustomerRetriever(string connString) : base(connString) { } 
        protected override string GetQuery() { 
            return "SELECT Id, FirstName, LastName, Address FROM Customer"; 
        } 
    }

    /*
     Separate example : template for inserting a genetic type into a collection!
     */
    public class UniqueCollection<T> : Collection<T> { // Only insert or set items if // the item is not yet in the collection. 
        protected override void InsertItem(int index, T item) { 
            if (!this.Contains(item)) { base.InsertItem(index, item); 
            } 
        } 
        protected override void SetItem(int index, T item) { 
            if (!this.Contains(item)) { 
                base.SetItem(index, item); 
            } 
        } 
    }}
