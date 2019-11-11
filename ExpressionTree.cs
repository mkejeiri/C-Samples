//https://dotnetfiddle.net/C55ERn
/********************************************************************************************************************
       Expression vs reflexion
       Expression are a nice abstraction over reflexion, reflexion is powerful tool emit the IL to run and create methods, 
       reflexion are slow in comparison to Expression, Expression speeds that up, uses reflexion to cache a delegate, 
       compile it and then save it into memory and then reuse it, expression are virtually as fast as methods 
       that we declare inline. Expression captures a lot of what reflexion can do. 

           Shortfall:
           - sugar syntax using '+' operator on string, this would works in lambda 
           because c# call 'Concat' under the hood but since is not ExpressionType.Add and we will get an exception

           - Conversion has to be explicitly handled : if an expression expect an object and we passed an employee 
           if will throw an exception while in lambda this could work because c# magically does the conversion.
 ********************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTree
{
    public static class EnumerableExtensions
    {
        //We get a series of the rules to be translated into binary expressions
        public static IEnumerable<T> WhereRules<T>(this IEnumerable<T> source, IEnumerable<RulesEngineImplementation.Rule> rules)
        {
            //T = "Employee" FullName = "Employee"

            //with the next parameter we will get default Param_0
            //var parameter = Expression.Parameter(typeof(T));

            //we got Employee thanks to "name: typeof(T).Name" instead default Param_0 which is applied on Employee object
            var parameter = Expression.Parameter(typeof(T), name: typeof(T).Name);

            //BinaryExpression represents an operation with a left side, right side and an operator
            //BinaryExpression:
            //Left : expression
            //Right : expression
            //NodeType (The thing in the middle!) : Equal, NotEqual, etc...
            BinaryExpression binaryExpression = null;

            //Create an expression :  (Name == gary) AndAlso (HireDate > #2016/01/01#)
            foreach (var rule in rules)
            {
                //e.g. Extract Name or HireDate property from Employee Object
                //Param_0.Name or Param_0.HireDate
                //Employee.Name or Employee.HireDate
                var prop = Expression.Property(parameter, rule.PropertyName);

                //e.g. Extract  constant of either gary or #2016/01/01#
                var value = Expression.Constant(rule.Value);

                //(Name == gary) or  (HireDate > #2016/01/01#)
                //rule.Operation : '=' or '>' : (Param_0.Name == "gary") or (Param_0.HireDate > 1/1/2016 12:00:00 AM)
                //rule.Operation : '=' or '>' : (Employee.Name == "gary") or (Employee.HireDate > 1/1/2016 12:00:00 AM)
                var newBinary = Expression.MakeBinary(rule.Operation, left: prop, right: value);

                binaryExpression =
                    binaryExpression == null
                        //(Employee.Name == gary) or  (Employee.HireDate > #2016/01/01#)
                        ? newBinary
                        // add (Employee.Name == gary) to (Employee.HireDate > #2016/01/01#) or  (Employee.HireDate > #2016/01/01#) to (Employee.Name == gary) 
                        : Expression.MakeBinary(ExpressionType.AndAlso, left: binaryExpression, right: newBinary);
            }

            //Compile the expression and cache it
            // (Name == gary) AndAlso (HireDate > #2016/01/01#)
            //using default parameter definition - default Param_0, pre-compiled : Param_0 => ((Param_0.Name == "gary") AndAlso (Param_0.HireDate > 1/1/2016 12:00:00 AM))
            // pre-compiled : Employee => ((Employee.Name == "gary") AndAlso (Employee.HireDate > 1/1/2016 12:00:00 AM))
            var appliedExpression = Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);

            //post-compiled appliedExpression
            var appliedExpressionCompiled = appliedExpression.Compile();

            // default Param_0 : Apply the expression (Param_0.Name == gary) AndAlso (Param_0.HireDate > #2016/01/01#)
            //Apply the expression (Employee.Name == gary) AndAlso (Employee.HireDate > #2016/01/01#)
            return source.Where(appliedExpressionCompiled);
        }

        public static IQueryable<T> OrderByPropertyOrField<T>(this IQueryable<T> queryableData, string propertyOrFieldName, bool ascending = true)
        {
            //T = "Employee" FullName = "Employee"
            var elementType = typeof(T);
            string orderByMethodName = ascending ? "OrderBy" : "OrderByDescending";

            //Param_0 is applied on Employee object
            //with the next parameter we will get default Param_0
            //var parameterExpression = Expression.Parameter(elementType);

            //we got Employee thanks to "name: elementType.Name" instead default Param_0 which is applied on Employee object
            var parameterExpression = Expression.Parameter(elementType, name: elementType.Name);

            //Param_0.Name or Employee.Name is we forced the naming into Employee
            var propertyOrFieldExpression = Expression.PropertyOrField(parameterExpression, propertyOrFieldName);

            //Param_0 => Param_0.Name or Employee => Employee.Name
            var lambda = Expression.Lambda(propertyOrFieldExpression, parameterExpression);
            var selector = Expression.Lambda(propertyOrFieldExpression, parameterExpression);

            //Employee[].OrderBy(Param_0 => Param_0.Name) OR Employee[].OrderBy(Employee => Employee.Name)
            var orderByExpression = Expression.Call(
                //the type whose function we want to call
                type: typeof(Queryable),
                //The name of the method (string)
                methodName: orderByMethodName,
                //The generic type signature {Employee, String}
                typeArguments: new[] { elementType, propertyOrFieldExpression.Type },
                //Parameters : queryableData.Expression = {ExpressionTree.RulesEngineImplementation+Employee[]}, selector= {Employee => Employee.Name}
                /*params arguments:*/queryableData.Expression, selector);

            //Create an executable query from the expression tree.
            return queryableData.Provider.CreateQuery<T>(orderByExpression);
        }
    }

    //This two rule will filter engine over the list (Name == gary) AndAlso (HireDate > #2016/01/01#)
    //OR search filtering system over 'Name' and 'HireDate'
    public static class RulesEngineImplementation
    {
        public class Rule
        {
            public string PropertyName { get; set; }
            public ExpressionType Operation { get; set; }
            public object Value { get; set; }
        }
        public class Employee
        {
            public string Name { get; set; }
            public DateTime HireDate { get; set; }
        }
        //https://dotnetfiddle.net/eKyG7j
        public static void Run()
        {
            var employees = new[]
            {
                new Employee
                {
                    Name = "gary",
                    HireDate = new DateTime(year:2017, month:1, day:1)
                },
                new Employee
                {
                    Name = "gary",
                    HireDate = new DateTime(year:2014, month:12, day:31)
                },
                new Employee
                {
                    Name = "michael",
                    HireDate = new DateTime(year: 2017, month:1, day:1)
                },
                new Employee
                {
                    Name = "john",
                    HireDate = new DateTime(year:2017, month:1, day:1)
                },
            };

            //This two rule will filter over the list (Name == gary) AndAlso (HireDate > #2016/01/01#)
            //We get a series of the rules to be translated into binary expressions
            var Rules = new List<Rule> {
                new Rule {
                    PropertyName = "Name",
                    Operation = ExpressionType.Equal,
                    Value = "gary"
                },
                new Rule {
                    PropertyName = "HireDate",
                    Operation = ExpressionType.GreaterThan,
                    Value = new DateTime(year:2016, month:1, day:1)
                }
            };

            Console.WriteLine("----------Filtering by rules----------");
            foreach (var emp in employees.WhereRules(Rules))
            {
                Console.WriteLine("{0} - {1:d}", emp.Name, emp.HireDate);
            }

            Console.WriteLine("----------Sorting by field name----------");
            foreach (var emp in employees.AsQueryable().OrderByPropertyOrField("Name"))
            {
                Console.WriteLine("{0} - {1:d}", emp.Name, emp.HireDate);
            }

            /*
             ----------Filtering by rules----------
            gary - 1/1/2017
            ----------Sorting by field name----------
            gary - 1/1/2017
            gary - 12/31/2014
            john - 1/1/2017
            michael - 1/1/2017
             */

            Console.ReadKey();
        }
    }


    class Program
    {

        static void Main(string[] args)
        {
            SimpleExpressions();
            ExpressionOnString();
            Factorial();
            ChangeAndAlsoToOrElse();
            RulesEngineImplementation.Run();
            BuildDynamicQueries();
            ExceptionsWhenSugarSyntax();

        }

        //Exception : The binary operator Add is not defined for the types 'System.String' and 'System.String'.
        // str + str is not as add operator it's a concatenate under the cover!!!
        // solution with concat function at 'finally'
        /********************************************************************************************************************
        Expression vs reflexion
        Expression are a nice abstraction over reflexion, reflexion is powerful tool emit the IL to run and create methods, 
        reflexion are slow in comparison to Expression, Expression speeds that up, uses reflexion to cache a delegate, 
        compile it and then save it into memory and then reuse it, expression are virtually as fast as methods 
        that we declare inline. Expression captures a lot of what reflexion can do. 

            Shortfall:
            - sugar syntax using '+' operator on string, this would works in lambda 
            because c# call 'Concat' under the hood but since is not ExpressionType.Add and we will get an exception

            - Conversion has to be explicitly handled : if an expression expect an object and we passed an employee 
            if will throw an exception while in lambda this could work because c# magically does the conversion.
         ********************************************************************************************************************/
        private static void ExceptionsWhenSugarSyntax()
        {
            try
            {
                Func<string, string, string> combinedStrings = (str1, str2) =>
                    //this uses string.Concat under the cover!!!
                    str1 + str2;

                Expression<Func<string, string, string>> combinedStringsExp = (str1, str2) => str1 + str2;
                var str1Param = Expression.Parameter(typeof(string));
                var str2Param = Expression.Parameter(typeof(string));
                var combineThem = Expression.MakeBinary(ExpressionType.Add, left: str1Param, right: str2Param);

                var lambda = Expression.Lambda<Func<string, string, string>>(combineThem, str1Param, str2Param);

                Console.WriteLine(lambda.Compile().Invoke("Test1", "Test2"));
            }
            catch (InvalidOperationException e)
            {
                //output: The binary operator Add is not defined for the types 'System.String' and 'System.String'.
                Console.WriteLine(e.Message);
                //throw e;
            }
            
            //Solution
            finally
            {
                // Create the parameter expressions
                var strA = Expression.Parameter(typeof(string), name: "First");
                var strB = Expression.Parameter(typeof(string), name: "Second");

                //{System.String Concat(System.String, System.String)}
                var methodInfo = typeof(string).GetMethod(name: nameof(String.Concat), types:new[] { typeof(string), typeof(string) });

                //{Concat(First, Second)}
                var concatenate = Expression.Call(methodInfo, arg0:strA, arg1:strB);

                //(First, Second) => Concat(First, Second)
                var LambdaExpr = Expression.Lambda<Func<string, string, string>>(concatenate, strA, strB);
                //output:Test1Test2
                //Console.WriteLine(LambdaExpr.Compile().Invoke("Test1", "Test2"));


                //System.Func`3[System.String, System.String, System.String]
                var lambda = LambdaExpr.Compile();

                //output:Test1Test2
                Console.WriteLine(lambda.Invoke("Test1", "Test2"));
                
            }
        }

        //ExpressionVisitor read each piece of the expression and operates on it on the different way,
        //its goal here is to translate a parameter to upper case e.g. "s => (s + " belongs to john").ToUpper()"
        //ExpressionVisitor used to read and operate on expressions ... or even modify them... kind of
        public class ToUpperVisitor : ExpressionVisitor
        {
            public override Expression Visit(Expression node)
            {

                if (node.NodeType == ExpressionType.Parameter)
                {
                    return base.Visit(node);
                }
                //node = s + " belongs to john"
                if (node.Type == typeof(string))
                {
                    //System.String ToUpper()
                    var toUpper = typeof(string).GetMethod(name: nameof(String.ToUpper), Type.EmptyTypes);

                    //s => (s + " belongs to john").ToUpper()
                    var methodCallExpression = Expression.Call(node, toUpper);
                    return methodCallExpression;
                }
                return base.Visit(node);
            }
        }
        private static void ExpressionOnString()
        {

            //default Param_0 as parameters name
            //var prm = Expression.Parameter(typeof(string));

            //"String"  as parameters name
            var prm = Expression.Parameter(typeof(string), name: typeof(string).Name);

            //get the ToUpper method using reflexion
            //var toUpper = typeof(string).GetMethod(name:"ToUpper", Type.EmptyTypes);
            var toUpper = typeof(string).GetMethod(name: nameof(string.ToUpper), Type.EmptyTypes);

            //Creates a MethodCallExpression that represents a call to a method that takes no arguments. 
            //through reflexion we call "toUpper" applied to the instance "prm" (string expression wrapper)
            var body = Expression.Call(prm, toUpper);

            //We create a lambda based on the body and the param 
            //String => String.ToUpper()
            var lambdaExpression = Expression.Lambda(body, prm);


            //We create a lambda based on the body and the param , which a description (expression) of function not yet a function, and it need to be compiled first
            //String => String.ToUpper()
            var stronglyTypedLambdaExpr = Expression.Lambda<Func<string, string>>(body, prm); //Selector = lambda

            //not strongly typed: lack some kind of protection
            //Multiple args since it doesn't know the type
            //var lambda = lambdaExpression.Compile().DynamicInvoke(/*params object[]*/args: "john");
            var lambdaCompiled = lambdaExpression.Compile();
            var notStronglyTyped = lambdaCompiled.DynamicInvoke(/*params object[]*/args: "john");

            var stronglyTypedLambdaCompiled = stronglyTypedLambdaExpr.Compile();
            //One argument!
            var stronglyTyped = stronglyTypedLambdaCompiled.Invoke(arg: "john");

            //output: String => String.ToUpper()
            Console.WriteLine(lambdaExpression);

            //output: String => String.ToUpper()
            Console.WriteLine(stronglyTypedLambdaExpr);

            //After compilation of lambda's expression : 
            //output: System.Func`2[System.String,System.String]
            Console.WriteLine(lambdaCompiled);
            //output: System.Func`2[System.String, System.String]
            Console.WriteLine(stronglyTypedLambdaCompiled);

            //output: JOHN
            Console.WriteLine(notStronglyTyped);
            //output: JOHN
            Console.WriteLine(stronglyTyped);


            //This operates with a function
            //IEnumerable<T>.Where<T>(Func<T,Boolean> predicate)

            //This operates an expression (description of what a function does), can be interpreted by a library at runtime 
            //IQueryable<T>.Where<T>(Expression<Func<T,Boolean>> predicate)

            //This IQueryable : the result not triggered against the DB, **interpreted by a library at runtime**
            //var products = db.Products
            //	.Where(p=> p.Name == "eggs") //<- BinaryExpression
            //	.OrderByDescending(p=>p.Price);

            //BinaryExpression represents an operation with a left side, right side and an operator
            //BinaryExpression:
            //Left : expression
            //Right : expression
            //NodeType (The thing in the middle!) : Equal, NotEqual, etc...

            //ExpressionVisitor read each piece of the expression and operates on it on the different way, its goal is to translate this linked query into SQL
            //ExpressionVisitor used to read and operate on expressions ... or even modify them... kind of
            //p.Name == "eggs"
            //Part of function 			SQL
            //p.Name					Name
            //==						=
            //"eggs"					'eggs'	

            Expression<Func<string, string>> kejString = s => s + " belongs to john";

            var toUpperVisitor = new ToUpperVisitor();

            //s => (s + " belongs to john").ToUpper()
            var expressed = toUpperVisitor.VisitAndConvert(kejString, callerName: null);

            //output : CHEESE BELONGS TO john
            Console.WriteLine(expressed.Compile().Invoke(arg: "cheese"));


            //Example : convert string to a DateTime then back to a specific string format
            var stringParam = Expression.Parameter(typeof(string), name: "x");

            // calls static method "DateTime.Parse" since it is used through a reflexion
            var dateTimeParse = Expression.Call(typeof(DateTime), methodName: nameof(DateTime.Parse), typeArguments: null /* new Type[] { typeof(DateTime) }*/, arguments: stringParam);
            var format = Expression.Constant("MM/dd/yyyy");

            //calls instance method "DateTime.ToString(string)"  since it is used through a reflexion
            var bodyExpression = Expression.Call(dateTimeParse, methodName: nameof(DateTime.ToString), typeArguments: null, arguments: format);


            //x => Parse(x).ToString("MM/dd/yyyy")
            var lambdaExpr = Expression.Lambda<Func<string, string>>(bodyExpression, stringParam);

            //We need to compile it because it is not a func/lambda yet, it just a description of func/lambda, once compiled get cached.
            var lambdaExprCompiled = lambdaExpr.Compile();

            var x = "2016/08/30";


            //We need to compile it because it is not a func/lambda yet, it just a description of func/lambda, once compiled get cached.
            //output : x => Parse(x).ToString("MM/dd/yyyy") :(argument) x = (2016/08/30): result = 08/30/2016
            Console.WriteLine($"{lambdaExpr} :(argument) {nameof(x)} = ({x}): result = {lambdaExpr.Compile()(arg: x)}");


            //output: System.Func`2[System.String,System.String] :(argument) x = (2016/08/30): result = 08/30/2016
            Console.WriteLine($"{lambdaExprCompiled} :(argument) {nameof(x)} = ({x}): result = {lambdaExprCompiled(arg: x)}");
        }


        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/how-to-use-expression-trees-to-build-dynamic-queries
        private static void BuildDynamicQueries()
        {
            // Add a using directive for System.Linq.Expressions.  

            string[] companies =
            {
                "Consolidated Messenger", "Alpine Ski House", "South-ridge Video", "City Power & Light",
                "Coho Winery", "Wide World Importers", "Graphic Design Institute", "Adventure Works",
                "Humongous Insurance", "Wood-grove Bank", "Margie's Travel", "North-wind Traders",
                "Blue Yonder Airlines", "Trey Research", "The Phone Company",
                "Wingtip Toys", "Lucene Publishing", "Fourth Coffee"
            };

            // The IQueryable data to query.  
            IQueryable<String> queryableData = companies.AsQueryable<string>();

            //Compose the expression tree that represents the parameter to the predicate.  
            //Instead of default Param_0 we force it next to Company (name: "Company")
            ParameterExpression companyParam = Expression.Parameter(typeof(string), name: "Company");

            // ***** Where(company => (company.ToLower() == "coho winery" || company.Length > 16)) *****  
            // Create an expression tree that represents the expression 'company.ToLower() == "coho winery"'.  
            //company.ToLower()
            Expression companyToLower = Expression.Call(companyParam, typeof(string).GetMethod(name: "ToLower", System.Type.EmptyTypes));

            //"coho winery"}
            Expression companyConstant = Expression.Constant("coho winery");

            //(company.ToLower() == "coho winery")
            Expression companyToLowerEqualConstantCompanyName = Expression.Equal(left: companyToLower, right: companyConstant);

            // Create an expression tree that represents the expression 'company.Length > 16'.  
            //company.Length
            Expression companyLength = Expression.Property(companyParam, typeof(string).GetProperty(name: "Length"));

            //16
            Expression constantLength = Expression.Constant(value: 16, typeof(int));

            //(company.Length > 16)
            Expression companyLengthGreaterThanConstantLength = Expression.GreaterThan(left: companyLength, right: constantLength);

            // Combine the expression trees to create an expression tree that represents the  
            // expression '(company.ToLower() == "coho winery" || company.Length > 16)'. 
            //predicateBody = conditions
            //((company.ToLower() == "coho winery") OrElse (company.Length > 16))
            Expression conditions = Expression.OrElse(left: companyToLowerEqualConstantCompanyName, right: companyLengthGreaterThanConstantLength);

            // Create an expression tree that represents the expression  
            // 'queryableData.Where(company => (company.ToLower() == "coho winery" || company.Length > 16))'  
            //company => ((company.ToLower() == "coho winery") OrElse (company.Length > 16))
            var wherePredicate = Expression.Lambda<Func<string, bool>>(conditions, parameters: new ParameterExpression[] { companyParam });

            //System.String[].Where(company => ((company.ToLower() == "coho winery") OrElse (company.Length > 16)))
            //We use call to use an expression that use reflexion
            MethodCallExpression whereCallExpression = Expression.Call(
               type: typeof(Queryable),
                methodName: "Where",
                typeArguments: new Type[] { queryableData.ElementType },
                /*params arguments:*/ queryableData.Expression, wherePredicate);
            // ***** End Where *****  

            // ***** OrderBy(company => company) *****  
            // Create an expression tree that represents the expression  
            // 'whereCallExpression.OrderBy(company => company)'

            //company => company
            var orderByPredicate = Expression.Lambda<Func<string, string>>(companyParam, new ParameterExpression[] { companyParam });

            //System.String[].Where(company => ((company.ToLower() == "coho winery") OrElse(company.Length > 16))).OrderBy(company => company)
            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                methodName: "OrderBy",
                typeArguments: new Type[] { queryableData.ElementType, queryableData.ElementType },
                /*params arguments:*/ whereCallExpression, orderByPredicate);
            // ***** End OrderBy *****  

            // Create an executable query from the expression tree.
            //System.String[].Where(company => ((company.ToLower() == "coho winery") OrElse (company.Length > 16))).OrderBy(company => company)
            IQueryable<string> results = queryableData.Provider.CreateQuery<string>(orderByCallExpression);

            // Enumerate the results.  
            foreach (string company in results)
                Console.WriteLine(company);

            /*output:  
                Blue Yonder Airlines  
                City Power & Light  
                Coho Winery  
                Consolidated Messenger  
                Graphic Design Institute  
                Humongous Insurance  
                Lucerne Publishing  
                Northwind Traders  
                The Phone Company  
                Wide World Importers  
            */
            Console.ReadKey();
        }

        private static void ChangeAndAlsoToOrElse()
        {
            Expression<Func<string, bool>> expr = name => name.Length > 10 && name.StartsWith("G");
            //output: name => ((name.Length > 10) && name.StartsWith("G"))  
            Console.WriteLine(expr);

            AndAlsoModifier treeModifier = new AndAlsoModifier();

            //output:name => ((name.Length > 10) || name.StartsWith("G"))  
            Expression modifiedExpr = treeModifier.Modify((Expression)expr);
            Console.WriteLine(modifiedExpr);
        }


        //ExpressionVisitor read each piece of the expression and operates on it on the different way,
        //its goal here is to modify an AndAlso (&&) to OrElse (||)
        public class AndAlsoModifier : ExpressionVisitor
        {
            public Expression Modify(Expression expression)
            {
                //Each this.Visit(expression) 'call', calls VisitBinary Method if the expression is binary 
                //name.StartsWith("G") is not a binary expression
                //(name.Length > 10)  is binary expression!
                return Visit(expression);
            }

            //protected override Expression VisitBinary([NotNull] BinaryExpression node)
            protected override Expression VisitBinary(BinaryExpression node)
            {
                //BinaryExpression represents an operation with a left side, right side and an operator
                //BinaryExpression:
                //Left : expression
                //Right : expression
                //NodeType (The thing in the middle!) : Equal, NotEqual, etc...
                //node = ((name.Length > 10) AndAlso name.StartsWith("G"))
                //node.NodeType = AndAlso
                if (node.NodeType == ExpressionType.AndAlso)
                {
                    // node: name => ((name.Length > 10) && name.StartsWith("G"))
                    Expression left = this.Visit(node.Left);
                    Expression right = this.Visit(node.Right);

                    //Make this binary expression an OrElse operation instead of an AndAlso operation.  
                    //visitorExpr = ((name.Length > 10) OrElse name.StartsWith("G"))
                    var visitorExpr = Expression.MakeBinary(binaryType: ExpressionType.OrElse, left: left, right: right, liftToNull: node.IsLiftedToNull, method: node.Method);
                    return visitorExpr;
                }

                return base.VisitBinary(node);
            }
        }

        private static void Factorial()
        {
            // Creating a parameter expression.  
            ParameterExpression value = Expression.Parameter(typeof(int), name: "value");

            // Creating an expression to hold a local variable.   
            ParameterExpression result = Expression.Parameter(typeof(int), name: "result");

            // Creating a label to jump to from a loop.  
            LabelTarget label = Expression.Label(typeof(int));

            // Creating a method body.  
            BlockExpression block = Expression.Block(
                // Adding a local variable.  
                variables: new[] { result },
                // Assigning a constant to a local variable: result = 1  
                /*"params expression":*/Expression.Assign(left: result, right: Expression.Constant(1)),
                // Adding a loop.  
                Expression.Loop(
                    // Adding a conditional block into the loop.  
                    body: Expression.IfThenElse(
                        // Condition: value > 1  
                        test: Expression.GreaterThan(left: value, right: Expression.Constant(1)),
                        // If true: result *= value --  
                        ifTrue: Expression.MultiplyAssign(left: result, right: Expression.PostDecrementAssign(value)),
                        // If false, exit the loop and go to the label.  
                        ifFalse: Expression.Break(label, result)
                    ),
                    // Label to jump to.  
                    label
                )
            );

            // Compile and execute an expression tree.  
            int factorial = Expression.Lambda<Func<int, int>>(block, value).Compile()(arg: 5);

            Console.WriteLine(factorial);
            // Prints 120. 
        }

        private static void SimpleExpressions()
        {

            //1- function declaration
            //Homoiconicity : the syntax that we use to declare a method can be used to describe a method
            Func<int, int, int> addTwoNumbersLambda = (x, y) => x + y;

            //output: System.Func`3[System.Int32,System.Int32,System.Int32]
            Console.WriteLine(addTwoNumbersLambda);

            //output: addTwoNumbersLambda(1,2) = 3
            Console.WriteLine($"addTwoNumbersLambda(1,2) = {addTwoNumbersLambda(1, 2)}");

            //2- function description
            //Homoiconicity : the syntax that we use to declare a method can be used to describe a method
            // (x, y) are parameters of the expression
            // x + y is the body of the expression
            Expression<Func<int, int, int>> addTwoNumbersExpression = (x, y) => x + y;
            //output: (x, y) => (x + y)
            Console.WriteLine(addTwoNumbersExpression);
            var addTwoNumbersExpressionCompiled = addTwoNumbersExpression.Compile();
            //output: addTwoNumbersExpressionCompiled(1,2) = 3
            Console.WriteLine($"addTwoNumbersExpressionCompiled(1,2) = {addTwoNumbersExpressionCompiled(1, 2)}");

            //BinaryExpression represents an operation with a left side, right side and an operator
            BinaryExpression body = (BinaryExpression)addTwoNumbersExpression.Body;
            //output: (x + y)
            Console.WriteLine(body);

            //output :4 
            //Console.WriteLine(addTwoNumbersExpression.Compile().DynamicInvoke(1, 3));//1+3
            Console.WriteLine(addTwoNumbersExpression.Compile()(arg1: 1, arg2: 3));//1+3


            ParameterExpression numberParameter = Expression.Parameter(typeof(int), name: "Number");
            //output: Number
            Console.WriteLine(numberParameter);

            ConstantExpression five = Expression.Constant(value: 5, typeof(int));
            //Output : 5
            Console.WriteLine(five);

            //BinaryExpression represents an operation with a left side, right side and an operator
            BinaryExpression numLessThanFive = Expression.LessThan(left: numberParameter, right: five);
            //Output : (Number < 5)
            Console.WriteLine(numLessThanFive);

            Expression<Func<int, bool>> lambda1 = Expression.Lambda<Func<int, bool>>(numLessThanFive, parameters: new ParameterExpression[] { numberParameter });
            //output: False
            //Console.WriteLine(lambda1.Compile().DynamicInvoke(6));
            Console.WriteLine(lambda1.Compile()(arg: 6));


            //Creating an expression tree. 
            Expression<Func<int, bool>> expr = num => num < 5;

            // Compiling the expression tree into a delegate.  
            Func<int, bool> result = expr.Compile();

            // Invoking the delegate and writing the result to the console.  
            // Prints True.  
            Console.WriteLine(result(arg: 4));
            // You can also use simplified syntax  
            // to compile and run an expression tree.  
            // The following line can replace two previous statements.
            // Also prints True.
            Console.WriteLine(expr.Compile()(arg: 4));


            // Create an expression tree.  
            Expression<Func<int, bool>> exprTree = number => number < 5;

            // Decompose the expression tree.  
            ParameterExpression param = (ParameterExpression)exprTree.Parameters[0];

            //BinaryExpression represents an operation with a left side, right side and an operator
            BinaryExpression operation = (BinaryExpression)exprTree.Body;
            ParameterExpression left = (ParameterExpression)operation.Left;
            ConstantExpression right = (ConstantExpression)operation.Right;

            //output: Decomposed expression: number => number LessThan 5  
            Console.WriteLine("Decomposed expression: {0} => {1} {2} {3}",
                param.Name, left.Name, operation.NodeType, right.Value);
        }
    }
}
