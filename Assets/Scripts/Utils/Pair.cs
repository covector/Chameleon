public struct Pair<T1, T2>
{
    public T1 left { get; set; }
    public T2 right { get; set; }
    public Pair(T1 left, T2 right)
    {
        this.left = left;
        this.right = right;
    }
}