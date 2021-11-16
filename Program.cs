
using System;

namespace BullAndCow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"---------------------------------
---- Welcome to Bull and Cow ----
---------------------------------");
            Console.WriteLine(@" 
    ,           ,
   /             \              
  ((__-^^-,-^^-__))
   `-_---' `---_-'
    <__|o` 'o|__>
       \  `  /
        ): :(
        :o_o:");
            Console.WriteLine(@"  
                   _(__)_        V
                   '-e e -'__,--.__)
                   (o_o)        ) 
                       \. /___.  |
                       ||| _)/_)/
                       //_(/_(/_(");
            Console.WriteLine("========================================");

            Console.WriteLine(@"The Objective: One player
tries to guess a secret number 
in as few tries as possible.");

            Console.WriteLine("========================================\n");

            Game game = new Game();
            game.CreateSecret();
            game.SetupCheat();
            game.Play();
        }
    }
}

