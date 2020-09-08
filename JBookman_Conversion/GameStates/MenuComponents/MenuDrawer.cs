using JBookman_Conversion.EngineBits.Rendering;
using System.Collections.Generic;

namespace JBookman_Conversion.GameStates.MenuComponents
{
    public class MenuDrawer
    {
        internal List<TextPrimitive> GetPrimitivesToRender(List<MenuItem> itemsToRender)
        {
            var primitives = new List<TextPrimitive>();

            var charWidth = 0.5f;

            var startingX = 0.0f; // todo

            // Work out location
            foreach (var item in itemsToRender)
            {
                var charArray = item.Text.ToCharArray();

                var numberOfChars = charArray.Length;

                for(var i = 0; i < numberOfChars; i++)
                {
                    var charOffset = i * charWidth;

                    var prim = new TextPrimitive
                    {
                        Character = charArray[i],
                        X = startingX + charOffset,
                        Y = 0f, // tood
                        Z = 1f
                    };

                    primitives.Add(prim);
                }

                //var menuItem = SomeMenuItem(item.Text);

                ////primitives.Add(menuItem);
            }

            return primitives;
        }

        private TextPrimitive SomeMenuItem(string text)
        {
            // Y = order from item? count / items to get Y info? -- Fixed per string?

            // X = starting location and render sideways per character
            // or if centre, find mid point text and render L-R using a -offset for the characters before mid and +offset for after?
            var item = new TextPrimitive
            {
                Character = text[0],
                X = 0,
                Y = 0,
                Z = 1f,
            };

            return item;
        }
    }
}
