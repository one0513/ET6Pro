
[System.Serializable]
public class BMSymbol
{
    public string key;
    public string spriteName;

    private int _length;

    public int Length
    {
        get
        {
            if (_length == 0) _length = key.Length;
            return _length;
        }
    }
}

