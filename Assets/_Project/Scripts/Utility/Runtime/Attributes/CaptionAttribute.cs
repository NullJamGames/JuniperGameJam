using System;

namespace NJG.Utilities
{
    public class CaptionAttribute : Attribute
    {
        public string Text;

        public CaptionAttribute(string text)
        {
            Text = text;
        }
    }
}