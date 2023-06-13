using System.Drawing;

interface IMessage
{

    public string Raw { get; set; }
    public MessageType Type { get; set; }

    public string Keyword { get; set; }

    string Compose();
    void Parse(string message);

}

class MotdMessage : IMessage
{
    public string Message { get; set; } = "";
    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.motd;
    public string Keyword { get; set; } = "motd";

    public string Compose()
    {
        return $"{Keyword}|{Message}";
    }

    public void Parse(string message)
    {
        Raw = message;
        Message = message.Split('|')[1];
    }

    public override string ToString()
    {
        return $"Message of the Day; {Message}";
    }
}

class JoinMessage : IMessage
{
    public JoinMessage(string username, string password)
    {
        Username = username;
        Password = password;


    }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.join;
    public string Keyword { get; set; } = "join";


    public string Compose()
    {
        return $"{Keyword}|{Username}|{Password}";
    }

    public void Parse(string message)
    {
        Raw = message;
        Username = message.Split('|')[1];
        Password = message.Split('|')[2];
    }

    public override string ToString()
    {
        return Compose();
    }
}

class DieMessage : IMessage
{
    public DieMessage()
    {
        Ids = new List<int>();
    }


    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.die;
    public List<int> Ids { get; set; }
    public string Keyword { get; set; } = "die";
    public string Compose()
    {
        return $"{Ids[0]}";
    }

    public void Parse(string message)
    {
        Raw = message;
        var splits = message.Split('|');
        bool first = true;
        foreach (string id in splits)
        {
            if (first)
            {
                first = false;
                continue;
            }
            Ids.Add(int.Parse(id));
        }

    }

    public override string ToString()
    {
        return Compose();
    }
}

class GoalMessage : IMessage

{
    public int X { get; set; }
    public int Y { get; set; }

    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.goal;
    public string Keyword { get; set; } = "goal";

    public string Compose()
    {
        return $"{Keyword}|{X}|{Y}";
    }

    public void Parse(string message)
    {
        Raw = message;
        X = int.Parse(message.Split('|')[1]);
        Y = int.Parse(message.Split('|')[2]);
    }

    public override string ToString()
    {
        return $"Goal at x:{X} y:{Y}";
    }
}


class GameMessage : IMessage

{

    public int Width { get; set; }
    public int Height { get; set; }

    public int Id { get; set; }

    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.game;
    public string Keyword { get; set; } = "game";

    public string Compose()
    {
        return $"{Keyword}|{Width}|{Height}|{Id}";
    }

    public void Parse(string message)
    {
        Raw = message;
        Width = int.Parse(message.Split('|')[1]);
        Height = int.Parse(message.Split('|')[2]);
        Id = int.Parse(message.Split('|')[3]);

    }

    public override string ToString()
    {
        return $"Board Size: width:{Width} height:{Height} ID: {Id}";
    }
}

class PosMessage : IMessage

{
    public int X { get; set; }
    public int Y { get; set; }
    public int Id { get; set; }



    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.pos;
    public string Keyword { get; set; } = "pos";

    public string Compose()
    {
        return $"{Keyword}|{X}|{Y}|{Id}";
    }
    public Point GetPositionPoint()
    {
        return new Point(X, Y);
    }

    public void Parse(string message)
    {
        try
        {
              Raw = message;
        Id = int.Parse(message.Split('|')[1]);
        X = int.Parse(message.Split('|')[2]);
        Y = int.Parse(message.Split('|')[3]); 
        }
        catch (System.Exception ex)
        {
            Console.Write("### Error ### ");
            Console.WriteLine(ex);
        }
     
            }

    public override string ToString()
    {
        return $"x|y {X}|{Y} ID {Id}";
    }
}


class ErrorMessage : IMessage

{
    public string Error { get; set; }

    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.error;
    public string Keyword { get; set; } = "error";

    public string Compose()
    {
        return $"{Keyword}|{Error}";
    }

    public void Parse(string message)
    {
        Raw = message;
        Error = message.Split('|')[1];

    }

    public override string ToString()
    {
        return $"Error:{Error}";
    }
}

class TickMessage : IMessage

{

    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.tick;
    public string Keyword { get; set; } = "tick";

    public string Compose()
    {
        return $"{Keyword}";
    }

    public void Parse(string message)
    {
        Raw = message;
    }

    public override string ToString()
    {
        return $"tick";
    }
}

class MoveMessage : IMessage

{
    public MoveDirection Direction { get; set; } = MoveDirection.up;

    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.move;
    public string Keyword { get; set; } = "move";

    public string Compose()
    {
        return $"{Keyword}|{Direction}";
    }

    public void Parse(string message)
    {
    }

    public override string ToString()
    {
        return $"move:{Direction}";
    }
}


class WinMessage : IMessage

{
    public int Wins { get; set; }
    public int Losses { get; set; }

    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.win;
    public string Keyword { get; set; } = "win";

    public string Compose()
    {
        return $"{Keyword}|{Wins}|{Losses}";
    }

    public void Parse(string message)
    {
        Raw = message;
        Wins = int.Parse(message.Split('|')[1]);
        Losses = int.Parse(message.Split('|')[1]);
    }

    public override string ToString()
    {
        return $"Wins:{Wins} Losses: {Losses}";
    }
}

class LoseMessage : IMessage

{
    public int Wins { get; set; }
    public int Losses { get; set; }

    public string Raw { get; set; } = "";
    public MessageType Type { get; set; } = MessageType.lose;
    public string Keyword { get; set; } = "lose";

    public string Compose()
    {
        return $"{Keyword}|{Wins}|{Losses}";
    }

    public void Parse(string message)
    {
        Raw = message;
        Wins = int.Parse(message.Split('|')[1]);
        Losses = int.Parse(message.Split('|')[1]);
    }

    public override string ToString()
    {
        return $"Wins:{Wins} Losses: {Losses}";
    }
}


