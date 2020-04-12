using System.Linq;
using System.Text.RegularExpressions;
using Terminal.Gui;

namespace Game.UI
{
    internal class PickOneDialog<T> : Dialog
    {
        T _result;
        bool _optionChosen;

        public PickOneDialog(string title, string message, in int dlgWidth, in int dlgHeight, in int dlgBoundary, T[] options) : base(title,dlgWidth,dlgHeight)
        {
            
            var line = dlgHeight - (dlgBoundary)*2 - options.Length;

            if (!string.IsNullOrWhiteSpace(message))
            {
                int width = dlgWidth - (dlgBoundary * 2);

                var msg = Wrap(message, width-1).TrimEnd();

                var text = new Label(0, 0, msg)
                {
                    Height = line - 1, Width = width
                };

                //if it is too long a message
                int newlines = msg.Count(c => c == '\n');
                if (newlines > line - 1)
                {
                    var view = new ScrollView(new Rect(0, 0, width, line - 1))
                    {
                        ContentSize = new Size(width, newlines + 1),
                        ContentOffset = new Point(0, 0),
                        ShowVerticalScrollIndicator = true,
                        ShowHorizontalScrollIndicator = false
                    };
                    view.Add(text);
                    Add(view);
                }
                else
                    Add(text);
            }
            
            foreach (var value in options)
            {
                T v1 = value;

                string name = value.ToString();

                var btn = new Button(0, line++, name)
                {
                    Clicked = () =>
                    {
                        _result = v1;
                        Running = false;
                        _optionChosen = true;
                    }
                };


                Add(btn);

                if(options.Length == 1)
                    FocusFirst();
            }
        }

        public bool Show(out T chosen)
        {
            Application.Run(this);

            chosen = _result;
            return _optionChosen;
        }
        public string Wrap(string s, int width)
        {
            var r = new Regex(@"(?:((?>.{1," + width + @"}(?:(?<=[^\S\r\n])[^\S\r\n]?|(?=\r?\n)|$|[^\S\r\n]))|.{1,16})(?:\r?\n)?|(?:\r?\n|$))");
            return r.Replace(s, "$1\n");
        }
    }
}