using System;

namespace Rodonaves.EDI.Enums
{
    public class KeyDescriptionValue<T>
    {
        protected KeyDescriptionValue()
        {
        }
        public KeyDescriptionValue(T enumm, string description, string value)
        {
            Enum = enumm;
            Description = description;
            Value = value;
        }

        public T Enum { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public class KeyDescriptionValue : KeyDescriptionValue<Enum>
    {
    }
}
