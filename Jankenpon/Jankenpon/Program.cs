using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace Jankenpon
{
    class Program
    {
        //exit 
        public static bool exit = false;

        //variable to  join or create game
        static Mode mode;
        static int choose;

        //enum for player 
        enum Mode
        { Server, Client, Error, }

        enum playerMove
        { Rock, Paper, Scissor, Exit, noInput, }

        struct Player
        {
            public playerMove move;
        }


        //Game
        static string game()
        {
            Player player;
            int choose;
            string input = "0";

            //prevent error in no input
            player.move = playerMove.noInput;

            Console.WriteLine();
            Console.WriteLine("Enter : ");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Rock");
            Console.WriteLine("2. Paper");
            Console.WriteLine("3. Scissor");
            Console.WriteLine("Choose: "); choose = Convert.ToInt32(Console.ReadLine());

            switch (choose)
            {
                case 0:
                    player.move = playerMove.Exit;
                    input = Convert.ToString(player.move);
                    break;

                case 1:
                    player.move = playerMove.Rock;
                    input = Convert.ToString(player.move);
                    break;

                case 2:
                    player.move = playerMove.Paper;
                    input = Convert.ToString(player.move);
                    break;

                case 3:
                    player.move = playerMove.Scissor;
                    input = Convert.ToString(player.move);
                    break;

                default:
                    Console.WriteLine("Error ! check your input and restart the game");
                    player.move = playerMove.noInput;
                    input = Convert.ToString(playerMove.noInput);
                    break;
            }

            return input;
        }


        //create a game server
        static void becameServer()
        {
            int port = 1212;
            string ipAddress = "127.0.0.1";
            Socket serverListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            serverListener.Bind(ep);
            serverListener.Listen(100);
            Console.WriteLine("Wait for 2nd player ");
            Socket clientSocket = default(Socket);
            int num = 0;
            Program p = new Program();

            while (!exit)
            {
                clientSocket = serverListener.Accept();
                Console.WriteLine("Player connected! ");
                Console.WriteLine();

                Thread userThread = new Thread(new ThreadStart(() => Program.User(clientSocket)));
                userThread.Start();
            }
        }

        public static void User(Socket client)
        {
            while (!exit)
            {
                //get data
                byte[] message = new byte[1024];
                int size = client.Receive(message);

                // another player turn
                string opponent = System.Text.Encoding.ASCII.GetString(message, 0, size);

                //check if another player disconnected
                if (opponent == Convert.ToString(playerMove.Exit))
                {
                    Console.WriteLine();
                    Console.WriteLine("Another Player Disconnected");
                    Console.WriteLine();
                    exit = true;
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("Your turn to choose ");
                Console.WriteLine();

                string yourAnswer = null;
                yourAnswer = game();

                //check
                check(opponent, yourAnswer);

                //send input
                client.Send(System.Text.Encoding.ASCII.GetBytes(yourAnswer), 0, yourAnswer.Length, SocketFlags.None);
            }
        }


        //join server
        static void becameClient()
        {
            int port = 1212;
            string ipAddress = "127.0.0.1";
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            clientSocket.Connect(ep);
            Console.WriteLine("Connected to Player...");

            Program p = new Program();

            while (!exit)
            {
                //player turn
                Console.WriteLine();
                Console.WriteLine("Your turn to choose ");
                Console.WriteLine();
                string yourAsnwer = null;
                yourAsnwer = game();

                //send player turn
                clientSocket.Send(System.Text.Encoding.ASCII.GetBytes(yourAsnwer), 0, yourAsnwer.Length, SocketFlags.None);

                //get data
                byte[] message = new byte[1024];
                int size = clientSocket.Receive(message);
                string opponent = null;
                opponent = System.Text.Encoding.ASCII.GetString(message, 0, size);

                //check if player disconnected
                if (opponent == Convert.ToString(playerMove.Exit))
                {
                    Console.WriteLine();
                    Console.WriteLine("Another Player Disconnected...");
                    Console.WriteLine();
                    exit = true;
                    break;
                }

                //check
                check(opponent, yourAsnwer);
            }
        }


        //check the game
        public static void check(string opponentAnswer, string yourAnswer)
        {
            //if opponent choose rock
            if (opponentAnswer == Convert.ToString(playerMove.Rock))
            {
                //if you choose paper
                if (yourAnswer == Convert.ToString(playerMove.Paper))
                {
                    Console.WriteLine();
                    Console.WriteLine("You win, Paper win againts Rock");
                    Console.WriteLine();
                }
                //if you choose scissor
                else if (yourAnswer == Convert.ToString(playerMove.Scissor))
                {
                    Console.WriteLine();
                    Console.WriteLine("You lose, Rock win againts scissor");
                    Console.WriteLine();
                }
                //if you choose rock too
                else if (yourAnswer == Convert.ToString(playerMove.Rock))
                {
                    Console.WriteLine();
                    Console.WriteLine("It's a Tie!");
                    Console.WriteLine();
                }
            }
            //if opponent choose paper
            if (opponentAnswer == Convert.ToString(playerMove.Paper))
            {
                //  if you choose scissor
                if (yourAnswer == Convert.ToString(playerMove.Scissor))
                {
                    Console.WriteLine();                    
                    Console.WriteLine("You win, Scissor win againts Paper");
                    Console.WriteLine();
                }
                //if you choose rock
                else if (yourAnswer == Convert.ToString(playerMove.Rock))
                {
                    Console.WriteLine();                    
                    Console.WriteLine("You lose, Paper win againts Rock");
                    Console.WriteLine();
                }
                //if you choose paper too
                else if (yourAnswer == Convert.ToString(playerMove.Paper))
                {
                    Console.WriteLine();
                    Console.WriteLine("It's a Tie!");
                    Console.WriteLine();
                }
            }
            //if opponent choose scissor
            if (opponentAnswer == Convert.ToString(playerMove.Scissor))
            {
                //  if you choose rock
                if (yourAnswer == Convert.ToString(playerMove.Rock))
                {
                    Console.WriteLine();       
                    Console.WriteLine("You win, Rock win againts Scissor");
                    Console.WriteLine();
                }
                //  if you choose paper
                else if (yourAnswer == Convert.ToString(playerMove.Paper))
                {
                    Console.WriteLine();                    
                    Console.WriteLine("You lose, Scissor win againts Paper");
                    Console.WriteLine();
                }
                //  if you choose scissor too
                else if (yourAnswer == Convert.ToString(playerMove.Scissor))
                {
                    Console.WriteLine();
                    Console.WriteLine("It's a Tie!");
                    Console.WriteLine();
                }
            }

            //  no input from player 
            if (opponentAnswer == Convert.ToString(playerMove.noInput))
            {
                Console.WriteLine();
                Console.WriteLine("False no Player input...");
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("JANKENPON !");
            Console.WriteLine("A ROCK PAPER SCISSOR GAME ! ");
            Console.WriteLine("1. Create server");
            Console.WriteLine("2. Join server");
            Console.Write("Input: "); choose = Convert.ToInt32(Console.ReadLine());

            switch (choose)
            {
                case 1:
                    mode = Mode.Server;
                    break;

                case 2:
                    mode = Mode.Client;
                    break;

                default:
                    mode = Mode.Error;
                    break;
            }

            switch (mode)
            {
                case Mode.Server:
                    Console.WriteLine();
                    Console.WriteLine("You become " + mode);
                    if (!exit) becameServer();
                    break;

                case Mode.Client:
                    Console.WriteLine();
                    Console.WriteLine("You are the " + mode);
                    if (!exit) becameClient();
                    break;

                case Mode.Error:
                    Console.WriteLine();
                    Console.WriteLine("Error, please restart the game! ");
                    break;

                default: break;
            }

            //  preventing program close before user can read all data
            Console.ReadLine();
        }
    }
}
