using NetGrad;
using NetGrad.Example.Console;

var d = 3;

var xys = new List<(double[] xs, double ys)>();

for (var x = 0; x < d; x++)
{
    for (var y = 0; y < d; y++)
    {
        xys.Add((new double[] { x, y }, x * y));
    }
}

var mlp = new MultiLayerPerceptron(2, d * d, 1);

var predictions = xys
    .Select(xy => (prediction: mlp.Invoke(xy.xs.Select(xs => new Value(xs)).ToArray())[0], expected: xy.ys/(d * d)))
    .ToArray();

var loss = Sum(predictions, p => (p.prediction - p.expected).ToThePowerOf(2));

foreach (var prediction in predictions)
{
    Console.WriteLine(prediction.ToString());
}

while (loss.Data > 0.0001)
{
    // Forward pass
    predictions = xys
        .Select(xy => (prediction: mlp.Invoke(xy.xs.Select(xs => new Value(xs)).ToArray())[0], expected: xy.ys/(d * d)))
        .ToArray();

    loss = Sum(predictions, p => (p.expected - p.prediction).ToThePowerOf(2));

    // Back propagate
    foreach (var parameter in mlp.Parameters)
    {
        parameter.ZeroGrad();
    }
    loss.BackPropagate();

    // Nudge
    foreach (var parameter in mlp.Parameters)
    {
        parameter.Data += (-0.01 * parameter.Gradient);
    }

    Console.WriteLine($"loss: {loss.Data}");
}

foreach (var prediction in predictions)
{
    Console.WriteLine((prediction.prediction.Data*10).ToString());
}

GraphHelper.GenerateGraph(loss);


while (true)
{
    var inp = Console.ReadLine();
    var vals = inp.Split(',');


    var result = mlp.Invoke(double.Parse(vals[0]), double.Parse(vals[1]));

    Console.WriteLine((Math.Round(result[0].Data * (d * d))));
}



Value Sum((Value prediction, double expected)[] values, Func<(Value prediction, double expected), Value> func)
{
    var sum = func(values[0]);
    for (var i = 1; i < values.Count(); i++)
    {
        sum = sum + func(values[i]);
    }

    return sum;
}
