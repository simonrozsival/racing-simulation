namespace Racing.IO.Model
{
    public interface ISerializableEvent
    {
        string Type { get; }
        double Time { get; }
    }
}
