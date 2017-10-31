using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AltSourceConsoleApp
{
    public class Command
    {
        //Name of Command (function called via Reflection)
        public string Name { get; set; }

        //Name of Controller class (class where Name is found)
        public string ClassName { get; set; }

        //The list of arguments passed to the command
        private List<string> arguments;

        //Get a list of arguments passed to the command
        public IEnumerable<string> Arguments
        {
            get { return arguments; }
        }

        public Command(string input)
        {
            //split our command up so we can parse the arguments
            var stringArray = Regex.Split(input, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            arguments = new List<string>();

            for(int i = 0; i < stringArray.Length; i++ )
            {
                //first argument is always the command
                if (i == 0)
                {
                    this.Name = stringArray[i];
                    //default class name
                    this.ClassName = "Users";
                    string[] s = stringArray[0].Split('.');
                    if( s.Length == 2 )
                    {
                        //if we are Users.Create then ClassName = Users, and Name = Create
                        this.ClassName = s[0];
                        this.Name = s[1];
                    }
                }
                else
                {
                    var inputArgument = stringArray[i];
                    string argument = inputArgument;

                    //is the argument a quoted string?
                    var regex = new Regex("\"(.*?)\"", RegexOptions.Singleline);
                    var match = regex.Match(inputArgument);

                    if( match.Captures.Count > 0)
                    {
                        //get the text in between quotes
                        var captureQuotedText = new Regex("[^\"]*[^\"]");
                        var quoted = captureQuotedText.Match(match.Captures[0].Value);

                        argument = quoted.Captures[0].Value;
                    }
                    arguments.Add(argument);
                }
            }
        }
    }
}
