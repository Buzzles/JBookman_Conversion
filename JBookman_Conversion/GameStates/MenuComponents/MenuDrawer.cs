using JBookman_Conversion.EngineBits.Rendering;
using System.Collections.Generic;

namespace JBookman_Conversion.GameStates.MenuComponents
{
    public class MenuDrawer
    {
        internal List<TextPrimitive> GetPrimitivesToRender(List<MenuItem> itemsToRender)
        {
            var primitives = new List<TextPrimitive>();

            // Temp.
            // Todo: Change text primitive to be starting location and text string,
            // have renderer create 1 tile high but as wide as needed for the text string, rather than a 1x1 square tile
            var charWidth = 1.0f; //0.5f;

            var startingX = 0.0f; // todo

            // Work out location
            foreach (var item in itemsToRender)
            {
                // V1 - Tile per character - temp

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

                // V2 - Proper way, set location and text, not per tile
                var stringPrim = new TextPrimitive
                {
                    Text = item.Text,
                    X = 10f, // temp, mid way down screen
                    Y = 0f, // temp, left side
                    Z = 1f
                };

                primitives.Add(stringPrim);

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
