using System;

namespace GettingInput
{
  class Program
  {
    static void Main()
    {
      Console.WriteLine("How old are you?");
      string input = Console.ReadLine();
      // This displays your answer with the statement
      Console.WriteLine($"You are {input} years old!");
    }
  }
}
