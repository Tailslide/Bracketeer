using Bracketeer.Helpers;
using System.Text;

namespace Bracketeer.Pages
{
    public partial class Index
    {
        /// <summary>
        /// Input text shown on the left
        /// </summary>
        private string inText = "IF(({{Run Pulses Per M3 Run 1 A/F}} + {{Run Pulses Per M3 Run 2 A/F}}) > 0,1,(IF(({{Run Meter Factor Run 1 A/F}} + {{Run Meter Factor Run 2 A/F}})=0,\"\",(IF(J36=\"LITRES\",(AVERAGE({{Run Meter Factor Run 1 A/F}}:{{Run Meter Factor Run 2 A/F}})),\"\")))))";
        public string InText
        {
            get
            {
                return inText;
            }
            set
            {
                if (value != inText)
                {
                    inText = value;
                    OnInTextChanged();
                }
            }
        }

        /// <summary>
        /// Colorize output toggle switch position
        /// </summary>
        private bool doColor = true;
        protected bool DoColor
        {
            get
            {
                return doColor;
            }
            set
            {
                if (value != doColor)
                {
                    doColor = value;
                    OnDoColorChanged();
                }
            }
        }


        /// <summary>
        /// Indent toggle switch position
        /// </summary>
        private bool doIndent = true;
        protected bool DoIndent
        {
            get
            {
                return doIndent;
            }
            set
            {
                if (doIndent != value)
                {
                    doIndent = value;
                    OnDoIndentChanged();
                }
            }
        }

        private void OnDoColorChanged()
        {
            RefreshOutput();
        }

        private void OnDoIndentChanged()
        {
            RefreshOutput();
        }

        private void OnInTextChanged()
        {
            RefreshOutput();
        }

        private void RefreshOutput()
        {
            if (doIndent)
                BuildTreeItems();
            else
                BuildColoredText(); 
        }


        /// <summary>
        /// List of (text, class) tuples for displaying colored text as ColoredText components
        /// blazor doesn't seem to let you use named tuples :/
        /// </summary>
        public List<(string, string)> spans = new List<(string, string)>();

        private HashSet<TreeItemData> TreeItems { get; set; } = new HashSet<TreeItemData>();

        /// <summary>
        /// Given an indent level, return css class name
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string getClass(int level)
        {
            if (doColor)
                return $"type{level + 1}";
            else
                return "";
        }

        /// <summary>
        /// Build list of text and css classes used to display colored text pieces
        /// </summary>
        private void BuildColoredText()
        {
            spans.Clear();

            string s = inText.deindent();
            var parent = TreeItems;
            int parenlevel = 0;
            while (s.Trim().Length > 0)
            {
                int? nextopen = s.IndexOf('(');
                if (nextopen == -1) nextopen = null;
                int? nextclose = s.IndexOf(')');
                if (nextclose == -1) nextclose = null;


                if (nextopen.HasValue && (!nextclose.HasValue || nextclose.Value > nextopen.Value))
                {
                    //next bracket is open.. add node for remaining text
                    spans.Add(new(s.Substring(0, nextopen.Value + 1), getClass(parenlevel)));
                    parenlevel++;
                    s = s.Substring(nextopen.Value + 1);
                }
                else
                if (nextclose.HasValue && (!nextopen.HasValue || nextopen.Value > nextclose.Value))
                {
                    // next bracket is close.. add node for remaining text
                    spans.Add(new(s.Substring(0, nextclose.Value + 1), getClass(parenlevel)));
                    s = s.Substring(nextclose.Value + 1);
                    parenlevel--;
                }
            }
        }

        /// <summary>
        /// Build data structures tree control binds to display heirarchy
        /// </summary>
        private void BuildTreeItems()
        {
            TreeItems.Clear();

            Stack<HashSet<TreeItemData>> parents = new Stack<HashSet<TreeItemData>>();

            string s = inText.deindent();
            var parent = TreeItems;
            int parenlevel = 0;
            while (s.Trim().Length > 0)
            {
                int? nextopen = s.IndexOf('(');
                if (nextopen == -1) nextopen = null;
                int? nextclose = s.IndexOf(')');
                if (nextclose == -1) nextclose = null;


                if (nextopen.HasValue && (!nextclose.HasValue || nextclose.Value > nextopen.Value))
                {
                    TreeItemData newchild = new TreeItemData(s.Substring(0, nextopen.Value + 1), parenlevel);
                    TreeItemData newchild2 = new TreeItemData(")", parenlevel);
                    parent.Add(newchild);
                    parent.Add(newchild2);
                    parents.Push(parent);
                    parenlevel++;
                    parent = newchild.TreeItems;
                    s = s.Substring(nextopen.Value + 1);
                }
                else
                if (nextclose.HasValue && (!nextopen.HasValue || nextopen.Value > nextclose.Value))
                {
                    // next bracket is close.. add node for remaining text
                    parent.Add(new TreeItemData(s.Substring(0, nextclose.Value), parenlevel));
                    s = s.Substring(nextclose.Value + 1);
                    if (parents.Count == 0)
                    {
                        // mismatched paren!!
                    }
                    else
                        parent = parents.Pop();

                    parenlevel--;
                }
            }
        }

        protected override void OnInitialized()
        {
            BuildTreeItems();
        }

    }
}
