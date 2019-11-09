//https://dotnetfiddle.net/C55ERn
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTree
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereRules<T>(this IEnumerable<T> source, IEnumerable<RulesEngineImplementation.Rule> rules)
        {
            var parameter = Expression.Parameter(typeof(T));
            BinaryExpression binaryExpression = null;

            //Create an expression :  (Name == gary) AndAlso (HireDate > #2016/01/01#)
            foreach (var rule in rules)
            {
                var prop = Expression.Property(parameter, rule.PropertyName);
                var value = Expression.Constant(rule.Value);
                var newBinary = Expression.MakeBinary(rule.Operation, left: prop, right: value);

                binaryExpression =
                    binaryExpression == null
                        ? newBinary
                        : Expression.MakeBinary(ExpressionType.AndAlso, left: binaryExpression, right: newBinary);
            }

            //Compile the expression and cache it
            var cookedExpression = Expression.Lambda<Func<T, bool>>(binaryExpression, parameter).Compile();

            //Apply the expression (Name == gary) AndAlso (HireDate > #2016/01/01#)
            return source.Where(cookedExpression);
        }

        public static IQueryable<T> OrderByPropertyOrField<T>(this IQueryable<T> queryable, string propertyOrFieldName, bool ascending = true)
        {
            //Name = "Employee" FullName = "Employee"}
            var elementType = typeof(T);
            var orderByMethodName = ascending ? "OrderBy" : "OrderByDescending";
            
            //Param_0
            var parameterExpression = Expression.Parameter(elementType);
            
            //Param_0.Name
            var propertyOrFieldExpression = Expression.PropertyOrField(parameterExpression, propertyOrFieldName);
            
            ////Param_0 => Param_0.Name
            var selector = Expression.Lambda(propertyOrFieldExpression, parameterExpression);
            
            //Employee[].OrderBy(Param_0 => Param_0.Name)
            var orderByExpression = Expression.Call(
                type: typeof(Queryable),
                methodName: orderByMethodName,
                typeArguments: new[] { elementType, propertyOrFieldExpression.Type },
                /*params arguments:*/queryable.Expression, selector);

            return queryable.Provider.CreateQuery<T>(orderByExpression);
        }
    }

    //This two rule will filter over the list (Name == gary) AndAlso (HireDate > #2016/01/01#)
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
            Factorial();
            ChangeAndAlsoToOrElse();
            RulesEngineImplementation.Run();
            BuildDynamicQueries();
           
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

            // Compose the expression tree that represents the parameter to the predicate.  
            ParameterExpression companyParam = Expression.Parameter(typeof(string), name: "company");

            // ***** Where(company => (company.ToLower() == "coho winery" || company.Length > 16)) *****  
            // Create an expression tree that represents the expression 'company.ToLower() == "coho winery"'.  
            Expression companyToLower = Expression.Call(companyParam, typeof(string).GetMethod(name: "ToLower", System.Type.EmptyTypes));
            Expression companyConstant = Expression.Constant("coho winery");
            Expression companyToLowerEqualConstantCompanyName = Expression.Equal(left: companyToLower, right: companyConstant);

            // Create an expression tree that represents the expression 'company.Length > 16'.  
            Expression companyLength = Expression.Property(companyParam, typeof(string).GetProperty(name: "Length"));
            Expression constantLength = Expression.Constant(value: 16, typeof(int));
            Expression companyLengthGreaterThanConstantLength = Expression.GreaterThan(left: companyLength, right: constantLength);

            // Combine the expression trees to create an expression tree that represents the  
            // expression '(company.ToLower() == "coho winery" || company.Length > 16)'. 
            //predicateBody = conditions
            Expression conditions = Expression.OrElse(left: companyToLowerEqualConstantCompanyName, right: companyLengthGreaterThanConstantLength);

            // Create an expression tree that represents the expression  
            // 'queryableData.Where(company => (company.ToLower() == "coho winery" || company.Length > 16))'  
            var wherePredicate = Expression.Lambda<Func<string, bool>>(conditions, parameters: new ParameterExpression[] { companyParam });
            MethodCallExpression whereCallExpression = Expression.Call(
               type: typeof(Queryable),
                methodName: "Where",
                typeArguments: new Type[] { queryableData.ElementType },
                /*params arguments:*/ queryableData.Expression, wherePredicate);
            // ***** End Where *****  

            // ***** OrderBy(company => company) *****  
            // Create an expression tree that represents the expression  
            // 'whereCallExpression.OrderBy(company => company)'  
            var orderByPredicate = Expression.Lambda<Func<string, string>>(companyParam, new ParameterExpression[] { companyParam });
            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                methodName: "OrderBy",
                typeArguments: new Type[] { queryableData.ElementType, queryableData.ElementType },
                /*params arguments:*/ whereCallExpression, orderByPredicate);
            // ***** End OrderBy *****  

            // Create an executable query from the expression tree.  
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
            Expression modifiedExpr = treeModifier.Modify((Expression)expr);
            //output:name => ((name.Length > 10) || name.StartsWith("G"))  
            Console.WriteLine(modifiedExpr);
        }

        public class AndAlsoModifier : ExpressionVisitor
        {
            public Expression Modify(Expression expression)
            {
                return Visit(expression);
            }

            protected override Expression VisitBinary(BinaryExpression b)
            {
                if (b.NodeType == ExpressionType.AndAlso)
                {
                    Expression left = this.Visit(b.Left);
                    Expression right = this.Visit(b.Right);

                    // Make this binary expression an OrElse operation instead of an AndAlso operation.  
                    return Expression.MakeBinary(binaryType: ExpressionType.OrElse, left: left, right: right, liftToNull: b.IsLiftedToNull, method: b.Method);
                }

                return base.VisitBinary(b);
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
                        ifTrue: Expression.MultiplyAssign(left: result,
                            right: Expression.PostDecrementAssign(value)),
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
            Expression<Func<int, int, int>> addTwoNumbersExpression = (x, y) => x + y;
            //output: (x, y) => (x + y)
            Console.WriteLine(addTwoNumbersExpression);

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

            BinaryExpression numLessThanFive = Expression.LessThan(left: numberParameter, right: five);
            //Output : (Number < 5)
            Console.WriteLine(numLessThanFive);

            Expression<Func<int, bool>> lambda1 = Expression.Lambda<Func<int, bool>>(numLessThanFive, parameters: new ParameterExpression[] { numberParameter });
            //False
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
            BinaryExpression operation = (BinaryExpression)exprTree.Body;
            ParameterExpression left = (ParameterExpression)operation.Left;
            ConstantExpression right = (ConstantExpression)operation.Right;

            //output: Decomposed expression: number => number LessThan 5  
            Console.WriteLine("Decomposed expression: {0} => {1} {2} {3}",
                param.Name, left.Name, operation.NodeType, right.Value);
        }
    }
}



