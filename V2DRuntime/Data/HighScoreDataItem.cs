using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace V2DRuntime.Data
{
    [XmlRootAttribute(ElementName = "Score", IsNullable = false)]
    public struct HighScoreDataItem : IComparable
    {
        public string name;
        public int score;
        public int level;
        public string data;
        //[XmlIgnore]
        public bool isCurrent;

        public HighScoreDataItem(string name, int score, bool isCurrent)  : this(name, score, isCurrent, 0, "")
        {
        }
        public HighScoreDataItem(string name, int score, bool isCurrent, int level) : this(name, score, isCurrent, level, "")
        {
        }
        public HighScoreDataItem(string name, int score, bool isCurrent, int level, string data)
        {
            this.name = name;
            this.score = score;
            this.isCurrent = isCurrent;
            this.level = level;
            this.data = data;
        }
        public int CompareTo(object o)
        {
            int result = -1;
            if (o is HighScoreDataItem)
            {
                HighScoreDataItem hs = ((HighScoreDataItem)o);
                result = (hs.score < this.score) ? -1 : (hs.score > this.score) ? 1 : 0;

                if (result == 0)
                {
                    result = (hs.level < this.level) ? -1 : (hs.level > this.level) ? 1 : 0;
                }
            }
            return result;
        }
    }
}
