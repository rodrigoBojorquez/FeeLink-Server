namespace FeeLink.Infrastructure.Services.Esp;

public class EspJsonReconstructor
{
    private readonly SortedDictionary<int, string> _fragments = new();
    private int _lastFragmentIndex = 0;

    public void AddFragment(string fragment)
    {
        var sepIdx = fragment.IndexOf('|');
        if (sepIdx == -1) return;
        if (!int.TryParse(fragment.Substring(1, sepIdx - 1), out int idx)) return;
        var payload = fragment.Substring(sepIdx + 1);

        _fragments[idx] = payload;
        if (payload.EndsWith("}")) // Marca de posible último fragmento
            _lastFragmentIndex = idx;
    }

    public bool IsComplete()
    {
        return _lastFragmentIndex > 0 && _fragments.Count == _lastFragmentIndex;
    }

    public string GetJson()
    {
        return string.Concat(_fragments.OrderBy(f => f.Key).Select(f => f.Value));
    }

    public void Clear()
    {
        _fragments.Clear();
        _lastFragmentIndex = 0;
    }
}
