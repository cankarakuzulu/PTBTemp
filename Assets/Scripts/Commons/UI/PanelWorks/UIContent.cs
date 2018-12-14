using UnityEngine;
using System.Collections;

namespace nopact.Commons.UI.PanelWorks
{
    public class UIContent
    {
        private Sprite spriteContent;
        private string textContent;
        private bool hasSpriteContent, hasTextContent;

        public UIContent( string text )
        {
            HasTextContent = true;
            TextContent = text;
        }

        public UIContent ( Sprite sprite )
        {
            HasSpriteContent = true;
            SpriteContent = sprite;
        }

        public UIContent ( string text, Sprite sprite )
        {

            HasSpriteContent = HasTextContent = true;
            SpriteContent = sprite;
            TextContent = text;

        }

        public bool HasSpriteContent
        {
            get
            {
                return hasSpriteContent;
            }

            private set
            {
                hasSpriteContent = value;
            }
        }

        public bool HasTextContent
        {
            get
            {
                return hasTextContent;
            }
            private set
            {
                hasTextContent = value;
            }
        }

        public string TextContent
        {
            get
            {
                return textContent;
            }
            private set
            {
                textContent = value;
            }
        }

        public Sprite SpriteContent
        {
            get
            {
                return spriteContent;
            }
            private set
            {
                spriteContent = value;
            }
        }
    }
}
