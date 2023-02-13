namespace NetGrad
{
    public class Layer
    {
        private Neuron[] _neurons;

        public Layer(long numberOfInputs, long numberOfOutputs)
        {
            _neurons = new Neuron[numberOfOutputs];
            for (var i = 0; i < numberOfOutputs; i++)
            {
                _neurons[i] = new Neuron(numberOfInputs);
            }
        }

        public Value[] Parameters => _neurons.SelectMany(x => x.Parameters).ToArray();

        public Value[] Invoke(Value[] inputs)
        {
            var values = new Value[_neurons.Length];
            for (var i = 0; i < _neurons.Length; i++)
            {
                values[i] = _neurons[i].Invoke(inputs);
            }

            return values;
        }
    }
}
