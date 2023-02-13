namespace NetGrad
{
    public class Neuron
    {
        private Value[] _weights;
        private Value _bias;

        public Neuron(long numberOfInputs)
        {
            _weights = new Value[numberOfInputs];
            _bias = new Random().NextDouble() * 2 - 1;
            InitializeWeights();
        }

        public Value[] Parameters => GetParameters();

        public Value Invoke(Value[] inputs)
        {
            return _weights
                .Select((w, i) => w * inputs[i])
                .Aggregate(_bias, (prev, next) => prev + next)
                .Tanh();
        }

        private void InitializeWeights()
        {
            for (var i = 0; i < _weights.Length; i++)
            {
                _weights[i] = new Random().NextDouble() * 2 - 1;
            }
        }

        private Value[] GetParameters()
        {
            var par = _weights
                .ToList();

            par.Add(_bias);

            return par.ToArray();
        }
    }
}
