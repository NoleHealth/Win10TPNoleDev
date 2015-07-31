
namespace ByDesign.Excelerator.Classes
{
    public class TranslatableText
    {
        private string _key;
        private string _defaultText;

        public string Key { get { return _key; } set { _key = (value == null) ? "" : value; } }
        public string DefaultText { get { return _defaultText; } set { _defaultText = (value == null) ? "" : value; } }

        public TranslatableText() : this("", "") { }

        public TranslatableText(string key, string defaultText)
        {
            _key = key;
            _defaultText = defaultText;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DefaultText))
                return base.ToString();
            return DefaultText;
        }
    }
}
