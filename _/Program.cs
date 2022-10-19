using System;

class Program
{

    public class Objekt
    {
        private string jmeno;
        public string JmenoGetSet
        {
            get
            {
                return jmeno;
            }
            set
            {
                if (jmeno == "Dita") throw new ArgumentException();
                jmeno = value;
            }
        }



        private string Jmeno;

        public string GetJmeno()
        {
            return Jmeno;
        }

        public void SetJmeno(string jmeno)
        {
            Jmeno = jmeno;
        }
    }

    unsafe static void Main(string[] args)
    {
        Objekt obj = new Objekt();
        string jmenoGetSet = obj.JmenoGetSet;

    }

   
}