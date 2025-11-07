using Microsoft.ML;
using Microsoft.ML.Data;

namespace MottuApi.ML;

public class SentimentModel
{
    private readonly PredictionEngine<Input, Output> _pred;
    public SentimentModel()
    {
        var ml = new MLContext();
        var samples = new[]
        {
            new Input { Text = "produto excelente, recomendo" , Label = true },
            new Input { Text = "péssimo, veio quebrado"       , Label = false },
            new Input { Text = "maravilhoso atendimento"      , Label = true },
            new Input { Text = "horrível experiência"         , Label = false }
        };
        var data = ml.Data.LoadFromEnumerable(samples);
        var pipeline = ml.Transforms.Text.FeaturizeText("Features", nameof(Input.Text))
            .Append(ml.BinaryClassification.Trainers.SdcaLogisticRegression());
        var model = pipeline.Fit(data);
        _pred = ml.Model.CreatePredictionEngine<Input, Output>(model);
    }

    public bool Predict(string text) => _pred.Predict(new Input { Text = text }).PredictedLabel;

    public class Input { public string Text { get; set; } = ""; public bool Label { get; set; } }
    public class Output { [ColumnName("PredictedLabel")] public bool PredictedLabel { get; set; } public float Score { get; set; } }
}
