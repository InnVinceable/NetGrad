namespace NetGrad
{
    public class MultiLayerPerceptron
    {
        private Layer[] _layers;
        
        public MultiLayerPerceptron(long numberOfInputs, params long[] numberOfOutputs)
        {
            var sz = (new[] { numberOfInputs })
                .Concat(numberOfOutputs)
                .ToArray();

            _layers = new Layer[numberOfInputs];
            for (var i = 0; i < numberOfOutputs.Length; i++)
            {
                _layers[i] = new Layer(sz[i], sz[i + 1]);
            }
        }

        public Value[] Parameters => _layers.SelectMany(layer => layer.Parameters).ToArray();

        public Value[] Invoke(params Value[] inputs)
        {
            foreach (var layer in _layers)
            {
                inputs = layer.Invoke(inputs);
            }

            return inputs;
        }
    }
}
