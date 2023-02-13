using System.Text.Json;

namespace NetGrad;

public class Value
{
    private readonly string? _operation;
    private readonly string? _label;
    private Action _backward;

    public Value(double value, string label = "")
    {
        _label = label;
        _backward = () => { };
        Guid = Guid.NewGuid();
        Data = value;
        Children = null;
        Gradient = 0;
    }

    private Value(double value, Value[] prev, string op)
    {
        _operation = op;
        _backward = () => { };
        Guid = Guid.NewGuid();
        Data = value;
        Children = prev;
        Gradient = 0;
    }

    public Guid Guid { get; }

    public double Data { get; set; }

    public Value[]? Children { get; private set; }

    public double Gradient { get; private set; }

    public static Value operator +(Value lhs, Value rhs)
    {
        var val = new Value(
            lhs.Data + rhs.Data,
            new Value[] { lhs, rhs },
            "+");

        val._backward = () =>
        {
            lhs.Gradient += 1 * val.Gradient;
            rhs.Gradient += 1 * val.Gradient;
        };

        return val;
    }

    public static Value operator *(Value lhs, Value rhs)
    {
        var val = new Value(
            lhs.Data * rhs.Data,
            new Value[] { lhs, rhs },
            "*");

        val._backward = () =>
        {
            lhs.Gradient += rhs.Data * val.Gradient;
            rhs.Gradient += lhs.Data * val.Gradient;
        };

        return val;
    }

    public Value Exp()
    {
        var val = new Value(Math.Exp(Data), new Value[] { this }, "exp");

        val._backward = () =>
        {
            Gradient = val.Data * val.Gradient;
        };

        return val;
    }

    public Value Tanh()
    {
        var n = Data;
        var t = (Math.Exp(2*n) - 1) / (Math.Exp(2*n) + 1);
        var val = new Value(t, new Value[] { this }, "tanh");

        val._backward = () =>
        {
            Gradient += (1 - Math.Pow(val.Data, 2)) * val.Gradient;
        };

        return val;
    }

    public Value Relu()
    {
        var val = new Value(
            Data < 0 ? 0 : Data,
            new Value[] { this },
            "Relu");

        val._backward = () =>
        {
            Gradient += val.Data > 0 ? val.Gradient : 0;
        };

        return val;
    }

    public Value ToThePowerOf(double power)
    {
        var val = new Value(
            Math.Pow(Data, power),
            new Value[] { this },
            $"^{power}");

        val._backward = () =>
        {
            Gradient += (power * Math.Pow(Data, power - 1)) * val.Gradient;
        };

        return val;
    }

    public static Value operator -(Value v)
    {
        var val = v * -1;
        return val;
    }

    public static Value operator -(Value lhs, Value rhs)
    {
        var val = lhs + (-rhs);
        return val;
    }

    public static Value operator /(Value lhs, Value rhs)
    {
        var val = lhs * rhs.ToThePowerOf(-1);
        return val;
    }

    public static implicit operator Value(double value)
    {
        return new Value(value);
    }

    public void BackPropagate()
    {
        var topo = new List<Value>();
        var visited = new List<Value>();

        Build(this);

        Gradient = 1;
        topo.Reverse();

        foreach (var v in topo)
        {
            v._backward();
        }

        void Build(Value v)
        {
            if (!visited.Contains(v))
            {
                visited.Add(v);
                if (v.Children != null)
                    v.Children.ToList().ForEach(Build);
                topo.Add(v);
            }
        }
    }

    public void ZeroGrad()
    {
        Gradient = 0;
    }

    // Formatted for DotDocuments
    public override string ToString()
    {
        return $"{{{(_operation != null ? _operation + "|" : "")}{(!string.IsNullOrEmpty(_label) ? _label : "")}|" +
            $"{{d: {Math.Round(Data, 4)}|g: {Math.Round(Gradient, 4)}}}}}";

    }
}
