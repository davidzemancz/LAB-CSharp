using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class ArgumentParser
    {
        protected Dictionary<string, Argument> Arguments { get; set; }

        /// <summary>
        /// Define arguments
        /// </summary>
        /// <param name="arguments">List of arguments. Argument name must begin with '-' (option) or '--' (long option)</param>
        public void Define(List<Argument> arguments)
        {
            this.Arguments = new Dictionary<string, Argument>();
            foreach (Argument argument in arguments)
            {
                this.Arguments[argument.Name] = argument;
            }
        }

        /// <summary>
        /// Parse arguments from array to dictionary of defined arguments and list of operands
        /// </summary>
        /// <param name="args">Array of arguments</param>
        /// <returns>Tuple, first is dictionary of defined arguments with values, second is list of operands</returns>
        /// <exception cref="UnexpectedArgumentException">Throws if unexpected argument in args array is found</exception>
        /// /// <exception cref="ArgumentsDefinitionException">Throws if not defined</exception>
        public (Dictionary<string, Argument> Arguments, List<string> Operands) Parse(string[] args)
        {
            if (this.Arguments == null) throw new ArgumentsDefinitionException();

            Dictionary<string, Argument> arguments = this.CloneArguments();
            List<string> operands = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (this.IsArgument(arg))
                {
                    if (!arguments.ContainsKey(arg)) throw new UnexpectedArgumentException();

                    Argument argument = arguments[arg];
                    if (argument.ValueType == typeof(bool))
                    {
                        argument.Value = true;
                    }
                    else
                    {
                        argument.Value = ++i < args.Length ? Convert.ChangeType(args[i], argument.ValueType) : argument.DefaultValue;
                    }
                }
                else
                {
                    operands.Add(arg);
                }
            }


            return (arguments, operands);
        }

        /// <summary>
        /// Checks if string is argument
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>true if string starts with '-' otherwise false</returns>
        private bool IsArgument(string arg)
        {
            return arg.StartsWith("-");
        }

        /// <summary>
        /// Clones dictionary of arguments
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Argument> CloneArguments()
        {
            Dictionary<string, Argument> arguments = new Dictionary<string, Argument>(this.Arguments.Count, this.Arguments.Comparer);
            foreach (KeyValuePair<string, Argument> kvp in this.Arguments)
            {
                arguments.Add(kvp.Key, (Argument)kvp.Value.Clone());
            }
            return arguments;
        }
    }

    public class UnexpectedArgumentException : Exception
    {

    }

    public class ArgumentsDefinitionException : Exception
    {

    }

    public class Argument : ICloneable
    {
        public string Name { get; protected set; }

        public object Value { get; set; }

        public Type ValueType { get; protected set; }

        public object DefaultValue { get; protected set; }

        protected Argument()
        {

        }

        public Argument(string name, Type valueType, object defaultValue = null)
        {
            this.Name = name;
            this.ValueType = valueType;
            this.DefaultValue = defaultValue;

            if (this.DefaultValue == null)
            {
                if (this.ValueType == typeof(bool))
                {
                    this.DefaultValue = false;
                }
                else if (this.ValueType.IsValueType)
                {
                    this.Value = 0;
                }
            }
            this.Value = this.DefaultValue;
        }

        public TypeEnum Type
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    if (this.Name.StartsWith("--")) return TypeEnum.OptionLong;
                    else if (this.Name.StartsWith("-")) return TypeEnum.Option;
                    else return TypeEnum.Operand;
                }
                return TypeEnum.None;
            }
        }

        public enum TypeEnum
        {
            None = 0,
            Option = 1,
            OptionLong = 2,
            Operand = 3,
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
