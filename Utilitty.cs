using System.Drawing;

public enum MessageType
{
    motd, // Motto of the day
    join,
    error,
    goal,
    pos,
    move,
    chat,
    win,
    lose,
    game,
    tick,
    die,
}

public enum MoveDirection
{
    up,
    right,
    down,
    left
}

public static class MoveHelper
{
    public static Point GetMovePosition(this Point point, MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.up:
                return new Point(point.X, point.Y - 1);
                break;
            case MoveDirection.right:
                return new Point(point.X + 1, point.Y);
                break;
            case MoveDirection.down:
                return new Point(point.X, point.Y + 1);
                break;
            case MoveDirection.left:
                return new Point(point.X - 1, point.Y);
                break;
            default:
                return new Point(point.X, point.Y);
                break;
        }

    }

    public static MoveDirection GetMoveDirectionFromPosition(this Point current, Point target)
    {

        if (current.X == target.X && current.Y < target.Y)
            return MoveDirection.down;
        if (current.X == target.X && current.Y > target.Y)
            return MoveDirection.up;
        if (current.X < target.X && current.Y == target.Y)
            return MoveDirection.right;
        if (current.X > target.X && current.Y == target.Y)
            return MoveDirection.left;

        throw new InvalidOperationException($"Direction not found. From: {current.X}|{current.Y} To: {target.X}|{target.Y}");
    }
}
