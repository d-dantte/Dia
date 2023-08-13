using Axis.Pulsar.Grammar.Recognizers;

namespace Axis.Dia.Convert.Text
{
    /// <summary>
    /// 
    /// </summary>
    public class ParseException : Exception
    {
        public IRecognitionResult RecognitionResult { get; }

        public ParseException(IRecognitionResult recognitionResult)
        : base("An error occured while tokenizing the string")
        {
            RecognitionResult = recognitionResult;
        }
    }
}
