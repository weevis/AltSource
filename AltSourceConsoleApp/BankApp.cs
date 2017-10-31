using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AltSourceConsoleApp.Models;
using System.Threading.Tasks;
using System.Globalization;

namespace AltSourceConsoleApp
{
    public class BankApp
    {
        //namespace where we find all of our command definitions
        private const string commandNamespace = "AltSourceConsoleApp.Controllers";

        //a list of command to method mappings
        private static Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>> libraries;

        //we like to do http requests to the api
        private static HttpHandler handler;

        //our user
        public static User user;

        public static void Main(string[] args)
        {
            user = new User();
            user.logged_in = false;

            Console.Title = typeof(BankApp).Name;

            //get our singleton instance, setup library
            handler = getHandler();
            libraries = new Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>>();

            //find our command classes  (Controllers namespace)
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == commandNamespace
                    select t;

            //store a list of available commands so we can get methods and arguments out of it
            var commands = q.ToList();

            var i = 1;
            foreach (var command in commands)
            {
                //Find methods which are labelled public and static within the controller
                var methods = command.GetMethods(BindingFlags.Static | BindingFlags.Public);
                var methodDictionary = new Dictionary<string, IEnumerable<ParameterInfo>>();

                foreach (var method in methods)
                {
                    //store the name of the method and the parameters
                    string commandName = method.Name;
                    methodDictionary.Add(commandName, method.GetParameters());
                }

                //we add the command to the library so we can invoke later
                // Users.Create => Users.Create()
                libraries.Add(command.Name, methodDictionary);
                i++;
            }

            Run();
        }

        /// <summary>
        /// Get a HttpHandler instance to do Get/Post actions
        /// </summary>
        /// <returns>returns a handle of our singleton instance</returns>
        public static HttpHandler getHandler()
        {
            HttpHandler handle = HttpHandler.Instance;
            return handle;
        }

        private static async void Run()
        {
            var AvailableCommands = new string[]
            {
                "0 - Exit",
                "Users.Create <FirstName> <LastName> <Email> <UserName> <Password>",
                "Users.Login <UserName> <Email> <Password>",
                "Users.GetBalance",
                "Users.Transactions",
                "Users.Deposit <amount>",
                "Users.Withdraw <amount>",
                "Users.Logout"
            };
            
            WriteToConsole("Available Commands: ");
            foreach(string str in AvailableCommands )
            {
                WriteToConsole(str);
            }

            //keep it running until we exit
            while(true)
            {
                var input = ReadFromConsole();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                try
                {
                    if (input == "0")
                        Environment.Exit(0);

                    //Create our command from our input
                    var cmd = new Command(input);

                    //if valid command, run it
                    string result = await Execute(cmd);

                    //here we can do things with the command result!
                    switch (cmd.Name)
                    {
                        case "Create":
                            if (IsValidJson(result))
                            {
                                try
                                {
                                    User u = JsonConvert.DeserializeObject<User>(result);
                                    WriteToConsole("Creation Success");
                                }
                                catch (Exception ex)
                                {
                                    WriteToConsole(result);
                                }
                            }
                            else
                                WriteToConsole(result);
                            break;
                        case "GetBalance":
                            if (Double.TryParse(result, out var n))
                                WriteToConsole("Current Balance: " + MoneyFormat(n));
                            else
                                WriteToConsole("Error performing balance check");
                            break;
                        case "Deposit":
                            if (Double.TryParse(result, out var d))
                                WriteToConsole("Current Balance: " + MoneyFormat(d));
                            else
                                WriteToConsole("Error performing deposit");
                            break;
                        case "Login":
                            if (IsValidJson(result))
                            {
                                try
                                {
                                    User u = JsonConvert.DeserializeObject<User>(result);
                                    WriteToConsole("Login Success");

                                }
                                catch(Exception ex )
                                {
                                    WriteToConsole(result);
                                }
                            }
                            else
                                WriteToConsole(result);
                            break;
                        case "Transactions":
                            if (IsValidJson(result))
                            {
                                try
                                {
                                    Transaction[] trans = JsonConvert.DeserializeObject<Transaction[]>(result);

                                    Console.WriteLine("{0,-30}{1,-30}{2,-30}{3,-30}{4,-30}",
                                        "Transaction Date", "Start Balance", "Change Amount", "End Balance", "Description"
                                        );
                                    foreach(Transaction t in trans )
                                    {
                                        Console.WriteLine("{0,-30}{1,-30}{2,-30}{3,-30}{4,-30}",
                                            t.TransactionTime, MoneyFormat(t.StartBalance), MoneyFormat(t.ChangeAmount), MoneyFormat(t.EndBalance), t.Description
                                            );
                                    }

                                }
                                catch (Exception ex)
                                {
                                    WriteToConsole(result);
                                }
                            }
                            else
                                WriteToConsole(result);
                            break;
                        case "Logout":
                            WriteToConsole("Success");
                            break;
                        case "Withdraw":
                            if (Double.TryParse(result, out var w))
                                WriteToConsole("Current Balance: " + MoneyFormat(w));
                            else
                                WriteToConsole("Error performing withdrawal");
                            break;
                        default:
                            WriteToConsole(result);
                            break;
                    }
                }
                catch(Exception ex )
                {
                    WriteToConsole(ex.Message);
                }
            }
        }

        public static string MoneyFormat(double dec)
        {
            return dec.ToString("C", CultureInfo.CurrentCulture);
        }
        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            var trimmed = json.Trim();

            //we only care if it's objects or arrays
            if( (trimmed.StartsWith("{") && trimmed.EndsWith("}")) || 
                (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(trimmed);
                    return true;
                }
                catch( JsonReaderException )
                {
                    return false;
                }
            }
            return false;
        }

        public static async Task<string> Execute(Command command)
        {
            string badCommandMessage = string.Format(""
                + "Unrecognized command \'{0}.{1}\'. "
                + "Please type a valid command.",
                command.ClassName, command.Name);

            //not a valid command!
            if( !libraries.ContainsKey(command.ClassName))
            {
                return badCommandMessage;
            }


            var methodDictionary = libraries[command.ClassName];
            //Command not valid!
            if( !methodDictionary.ContainsKey(command.Name))
            {
                return badCommandMessage;
            }

            //get list of parameters for each function
            var methodParameterList = new List<object>();
            IEnumerable<ParameterInfo> paramList = methodDictionary[command.Name].ToList();

            //see if we supplied enough parameters
            var requiredParams = paramList.Where(p => p.IsOptional == false);
            var optionalParams = paramList.Where(p => p.IsOptional == true);

            int requiredCount = requiredParams.Count();
            int optionalCount = optionalParams.Count();

            int providedCount = command.Arguments.Count();

            if( requiredCount > providedCount )
            {
                return string.Format("Missing required argument. {0} required, {1} optional, {2} provided",
                    requiredCount, optionalCount, providedCount);
            }

            //if we actually have parameters, let's parameterize them
            if( paramList.Count() > 0)
            {
                foreach(var param in paramList)
                {
                    methodParameterList.Add(param.DefaultValue);
                }

                //let's go through parameters in our custom command
                for (int i = 0; i < command.Arguments.Count(); i++)
                {
                    var methodParam = paramList.ElementAt(i);
                    var typeRequired = methodParam.ParameterType;
                    object value = null;

                    try
                    {
                        // see if we can turn the parameter provided to the required type for the function
                        value = stringToType(typeRequired, command.Arguments.ElementAt(i));

                        //remove default parameter, insert custom parameter
                        methodParameterList.RemoveAt(i);
                        methodParameterList.Insert(i, value);
                    }
                    catch( ArgumentException ex )
                    {
                        //We couldn't change the type
                        string argumentName = methodParam.Name;
                        string argumentTypeName = typeRequired.Name;
                        string message =
                            string.Format(""
                            + "The value passed for argument '{0}' cannot be changed to type '{1}'",
                            argumentName, argumentTypeName);

                        throw new ArgumentException(message);
                    }

                }
            }

            //get the assembly so we can get the return type of our command class
            Assembly current = typeof(BankApp).Assembly;

            Type commandClass = current.GetType(commandNamespace + "." + command.ClassName);

            object[] inputArgs = null;
            //if there are parameters, prepare to pass them to the function we invoke
            if( methodParameterList.Count > 0 )
            {
                inputArgs = methodParameterList.ToArray();
            }

            var typeInfo = commandClass;

            try
            {
                //Here we invoke our method using reflection
                var result = await InvokeAsync(typeInfo, command.Name, 
                    BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, inputArgs);

                if (result == null)
                    return "The server did not respond correctly";

                return result.ToString();
            }
            catch( Exception ex )
            {
                throw ex;
            }
        }

        //some fun trickery to invoke with async and await constructs
        public static async Task<string> InvokeAsync(Type typeInfo, string CommandName, BindingFlags flags, object[] input)
        {
            Task<string> result = (Task<string>)typeInfo.InvokeMember(CommandName, flags, null, null, input);
            return await result;
        }

        /// <summary>
        /// Change user input to required type for function argument
        /// </summary>
        /// <param name="requiredType">the type required by the function</param>
        /// <param name="inputValue">the value provided on the console</param>
        /// <returns></returns>
        public static object stringToType(Type requiredType, string inputValue)
        {
            var requiredTypeCode = Type.GetTypeCode(requiredType);
            string exceptionMessage = string.Format("Cannnot coerce the input argument {0} to required type {1}",
                inputValue, requiredType.Name);

            object result = null;

            switch(requiredTypeCode)
            { //if the argument in the function is an int type, let's try and convert
                case TypeCode.Int32:
                    int number32;
                    if( Int32.TryParse(inputValue, out number32))
                    {
                        result = number32;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                    //if it's a string type, that's native from console
                case TypeCode.String:
                    result = inputValue;
                    break;
                    //if a double type is required, let's try and fudge the string to a double
                case TypeCode.Double:
                    Double doubleVal;
                    if (Double.TryParse(inputValue, out doubleVal))
                    {
                        result = doubleVal;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
            }

            return result;
        }

        //write a message to the console
        public static void WriteToConsole(string message = "")
        {
            if (message.Length > 0)
                Console.WriteLine(message);
        }

        //read user input with a prompt
        public static string ReadFromConsole(string outMessage = "")
        {
            Console.Write("> " + outMessage);
            return Console.ReadLine();
        }
    }
}