using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncAwaitExample
{
    public partial class AsyncAwaitExampleForm : Form
    {
        public AsyncAwaitExampleForm()
        {
            InitializeComponent();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            //00- blocking call!!!
             BigLongImportantMethod("John");


            /***************************************************************************
             * 01- Using Task.Factory & TaskScheduler.FromCurrentSynchronizationContext
                Cross-thread operation not valid: Control 'label1' accessed from a 
                thread other than the thread it was created on. 
                to solve this we need : TaskScheduler.FromCurrentSynchronizationContext() 
            ***************************************************************************/           
                Task.Factory.StartNew(()=>BigLongImportantMethod("Sally")) 
                //Access the result property from previous task! 
                .ContinueWith(t => Displaylabel.Text = t.Result, TaskScheduler.FromCurrentSynchronizationContext());
            

            /***************************************************************************
            * 02- using Async much more simple & better! 
            ***************************************************************************/
            
            CallImportantMethodAsync(); 
            Displaylabel.Text = "Waiting...";
        }
        private async void CallImportantMethodAsync(){ 
            var result = await BigLongImportantMethodAsync("Kejeiri"); 
            Displaylabel.Text = result; 
        }

        //execute a TY-a 
        private Task<string> BigLongImportantMethodAsync(string Name){
            //do it in the backgroud 
            return Task.Factory.StartNew(()=> BigLongImportantMethod(Name)); 
        }

        private string BigLongImportantMethod(string Name) {           
            Thread.Sleep(3000); 
            return "hello " + Name; 
        }

       
    }
}
