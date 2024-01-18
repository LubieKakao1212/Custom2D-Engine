using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering.Sprites.Atlas
{
    public class SpriteAtlasLoader<T> where T : struct
    {
        private ContentManager Content;
        private string[] textureNames;
        private SpriteAtlas<T> atlas; 

        public SpriteAtlasLoader(ContentManager content, SpriteAtlas<T> atlas, params string[] textureNames)
        {
            this.Content = content;
            this.textureNames = textureNames;
            this.atlas = atlas;
        }

        public Sprite[] Load(string path, params Rectangle[] rects)
        {
            Texture2D[] textures = new Texture2D[textureNames.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                var file = $"{path}/{textureNames[i]}";
                if (File.Exists($"{Content.RootDirectory}/{file}.xnb"))
                {
                    textures[i] = Content.Load<Texture2D>(file);
                }
            }
            return atlas.AddTextureRects(textures, rects);
        }
    }
}
