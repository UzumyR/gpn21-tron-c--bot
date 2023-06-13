using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Windows;
using System.Drawing;

public class Program
{

    // Links - rechts X
    // hoch - runter y
    // links oben 0,0
    private const int portNum = 4000;
    private const string hostName = "gpn-tron.duckdns.org";
    private const string password = "XXX";
    private const string username = "-#-";

    private static Random random = new Random();


    private static bool Alive = false;
    private static int CurrentId = -1;
    private static Point CurrentPosition = new Point(0, 0);

    private static int MaxX, MaxY;

    private static int[,] Board = new int[999, 999];

    private static int StartZick = 10;


    //private const string hostName = "gpn-mazing.v6.rocks";

    public static int Main(String[] args)
    {
        Console.WriteLine("Moin");
        try
        {
            TcpClient client = new TcpClient();
            //   TcpClient client = new TcpClient( AddressFamily.InterNetworkV6 );
            client.Connect(hostName, portNum);

            NetworkStream ns = client.GetStream();

            ReciveMessages(ns);

            IMessage message = new JoinMessage(username, password);
            SendMessage(ns, message);

            while (true) // Gameloop
            {
                ReciveMessages(ns);
            }


            ReciveMessages(ns);
            ReciveMessages(ns);
            ReciveMessages(ns);
            ReciveMessages(ns);

            client.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return 0;
    }

    private static void SendMessage(NetworkStream ns, IMessage message)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(message.Compose() + '\n'); ;
        ns.Write(bytes);
        ns.Flush();
        Console.WriteLine($"Send: {message.ToString()}");
    }

    private static void ReciveMessages(NetworkStream ns)
    {
        byte[] bytes = new byte[1024];
        int bytesRead = ns.Read(bytes, 0, bytes.Length);

        string text = Encoding.ASCII.GetString(bytes, 0, bytesRead);

        // Console.WriteLine("###" + text + "###");

        foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            IMessage message = null;
            // Console.WriteLine($"Recived Line: {line}");
            switch (line)
            {
                case String s when s.StartsWith("motd"):
                    message = new MotdMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("goal"):
                    message = new GoalMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("game"):
                    message = new GameMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("pos"):
                    message = new PosMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("error"):
                    message = new ErrorMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("win"):
                    message = new WinMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("lose"):
                    message = new LoseMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("tick"):
                    message = new TickMessage();
                    message.Parse(line);
                    break;

                case String s when s.StartsWith("die"):
                    message = new DieMessage();
                    message.Parse(line);
                    break;

                default:
                    Console.WriteLine($"Unknown Line: {line}");
                    break;
            }


            //    Console.WriteLine(message);

            ProcessMessage(ns, message);
        }
    }

    private static void ProcessMessage(NetworkStream ns, IMessage message)
    {
        if (message == null)
            return;

        if (message.Type == MessageType.tick)
        {
            if (!Alive)
                return;

            // if (!WalkedPoints.Any(x => x.X == pos.X && x.Y == pos.Y))
            //     WalkedPoints.Add(pos.GetPositionPoint());

            // PathPoints.Push(pos.GetPositionPoint());

            MoveMessage move = null;


            move = GetZickZackMove(move);

            move = GetNextNewMove(move);

            //   move = GetBacktrackMove(pos, move);

            move = GetNextFreeRngMove(CurrentPosition, move);
            move = GetDefaultMove(CurrentPosition, move);

            SendMessage(ns, move);
        }

        if (message.Type == MessageType.game)
        {
            GameMessage game = (message as GameMessage);
            Alive = true;
            CurrentId = game.Id;

            MaxX = game.Width;
            MaxY = game.Height;
            Board = new int[MaxX, MaxY];
            StartZick = 15;
            Console.WriteLine($"ID >{CurrentId}<");

            SendMessage(ns, new MoveMessage() { Direction = MoveDirection.up });
        }

        if (message.Type == MessageType.pos)
        {
            var pos = (message as PosMessage);

            Board[pos.X, pos.Y] = pos.Id;

            if (CurrentId == pos.Id)
            {
                CurrentPosition = new Point(pos.X, pos.Y);
                Console.WriteLine(pos);
            }


        }

        if (message.Type == MessageType.die)
        {
            if ((message as DieMessage).Ids.Contains(CurrentId))
            {
                Alive = false;
                Console.WriteLine("DEATH");
            }

            foreach (var id in (message as DieMessage).Ids)
            {
                for (int x = 0; x < MaxX; x++)
                {
                    for (int y = 0; y < MaxY; y++)
                    {
                        if (Board[x, y] == id)
                            Board[x, y] = 0;
                    }
                }
            }

            Console.WriteLine(message);
        }

        if (message.Type == MessageType.lose)
        {

            Console.WriteLine("Im death");
        }

    }

    private static MoveMessage GetDefaultMove(Point pos, MoveMessage move)
    {
        if (move == null)
        {
            // Console.WriteLine("GetDefaultMove");

            if (CheckBoardEmptyAt(MoveDirection.down))
                move = new MoveMessage() { Direction = MoveDirection.down };

            else if (CheckBoardEmptyAt(MoveDirection.left))
                move = new MoveMessage() { Direction = MoveDirection.left };

            else if (CheckBoardEmptyAt(MoveDirection.up))
                move = new MoveMessage() { Direction = MoveDirection.up };

            else if (CheckBoardEmptyAt(MoveDirection.right))
                move = new MoveMessage() { Direction = MoveDirection.right };
            else
            {
                move = new MoveMessage() { Direction = MoveDirection.right };
                Console.WriteLine("!NO free move");
            }

        }

        return move;
    }

    private static Point GetMoveResult(Point pos, MoveDirection direction)
    {
        // Links - rechts X
        // hoch - runter y
        // links oben 0,0
        Point result;
        switch (direction)
        {
            case MoveDirection.up:
                result = new Point(pos.X, pos.Y - 1);
                break;
            case MoveDirection.down:
                result = new Point(pos.X, pos.Y + 1);
                break;
            case MoveDirection.left:
                result = new Point(pos.X - 1, pos.Y);
                break;
            case MoveDirection.right:
                result = new Point(pos.X + 1, pos.Y);
                break;
            default:
                Console.WriteLine("Move error");
                result = pos;
                break;
        }


        if (result.X < 0)
            result = new Point(result.X + MaxX, result.Y);
        if (result.Y < 0)
            result = new Point(result.X, result.Y + MaxY);

        result = new Point(result.X % MaxX, result.Y % MaxY);



        return result;

    }
    private static bool CheckBoardEmptyAt(Point pos)
    {
        // Links - rechts X
        // hoch - runter y
        // links oben 0,0
        return Board[pos.X, pos.Y] == 0;
    }
    private static bool CheckBoardEmptyAt(MoveDirection dir)
    {
        // Links - rechts X
        // hoch - runter y
        // links oben 0,0
        // Console.WriteLine($"+ x:{CurrentPosition.X} y:{CurrentPosition.Y}");
        var pos = GetMoveResult(CurrentPosition, dir);
        //Console.WriteLine($"- x:{pos.X} y:{pos.Y}");
        return Board[pos.X, pos.Y] == 0;
    }

    private static MoveMessage GetNextFreeRngMove(Point pos, MoveMessage move)
    {
        if (move == null)
        {
            // Console.WriteLine("GetNextFreeRngMove");
            switch (random.Next(0, 4))
            {
                case 0:
                    if (CheckBoardEmptyAt(MoveDirection.down))
                        move = new MoveMessage() { Direction = MoveDirection.down };
                    break;
                case 1:
                    if (CheckBoardEmptyAt(MoveDirection.left))
                        move = new MoveMessage() { Direction = MoveDirection.left };
                    break;
                case 2:
                    if (CheckBoardEmptyAt(MoveDirection.up))
                        move = new MoveMessage() { Direction = MoveDirection.up };
                    break;
                case 3:
                    if (CheckBoardEmptyAt(MoveDirection.right))
                        move = new MoveMessage() { Direction = MoveDirection.right };
                    break;
            }
        }
        return move;
    }

    private static MoveMessage GetNextNewMove(MoveMessage move)
    {
        if (move == null)
        {
            Dictionary<MoveDirection, int> frees = new Dictionary<MoveDirection, int>();
            frees.Add(MoveDirection.up, RunDepthFirstSearch(MoveDirection.up));
            frees.Add(MoveDirection.down, RunDepthFirstSearch(MoveDirection.down));
            frees.Add(MoveDirection.left, RunDepthFirstSearch(MoveDirection.left));
            frees.Add(MoveDirection.right, RunDepthFirstSearch(MoveDirection.right));

            var best = frees.First(y => y.Value == frees.Max(x => x.Value));

            move = new MoveMessage() { Direction = best.Key };

            Console.WriteLine($"up:{frees[MoveDirection.up]} down:{frees[MoveDirection.down]} left:{frees[MoveDirection.left]} right:{frees[MoveDirection.right]} = {best.Key}");
        }
        return move;
    }


    private static MoveMessage GetZickZackMove(MoveMessage move)
    {
        if (move == null && StartZick > 0)
        {
            if (StartZick % 2 == 0)
                move = new MoveMessage() { Direction = MoveDirection.down };
            else
                move = new MoveMessage() { Direction = MoveDirection.left };
            StartZick--;
        }
        return move;
    }

    public static int DepthFirstSearch(Point position, bool[,] visited)
    {
        if (position.X < 0 || position.X >= Board.GetLength(0) ||
            position.Y < 0 || position.Y >= Board.GetLength(1))
        {
            return 0;
        }

        if (visited[position.X, position.Y] || Board[position.X, position.Y] != 0)
        {
            return 0;
        }

        visited[position.X, position.Y] = true;

        int count = 1; // Count this cell

        // Links - rechts X
        // hoch - runter y
        // links oben 0,0
        // Count the cells reachable from this cell (up, down, left, right)
        count += DepthFirstSearch(new Point(position.X, position.Y - 1), visited); // Up
        count += DepthFirstSearch(new Point(position.X, position.Y + 1), visited); // Down
        count += DepthFirstSearch(new Point(position.X - 1, position.Y), visited); // Left
        count += DepthFirstSearch(new Point(position.X + 1, position.Y), visited); // Right

        return count;
    }

    public static int RunDepthFirstSearch(MoveDirection dir)
    {
        bool[,] visited = new bool[MaxX, MaxY];
        visited[CurrentPosition.X, CurrentPosition.Y] = true;
        return DepthFirstSearch(GetMoveResult(CurrentPosition, dir), visited);
    }



}