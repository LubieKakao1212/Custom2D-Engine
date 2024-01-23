using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledLib.Objects;

namespace Custom2d_Engine.TMX.LayerProcessor
{
    public static class ObjectLayerProcessors
    {
        public static Func<LoadedMap<Color>, ObjectType, bool> FirstFrom<ObjectType>(params Func<LoadedMap<Color>, ObjectType, bool>[] handlers) where ObjectType : BaseObject
        {
            return (map, obj) =>
            {
                for (int i = 0; i < handlers.Length; i++)
                {
                    if (handlers[i](map, obj))
                    {
                        return true;
                    }
                }
                return false;
            };
        }

        public static Func<LoadedMap<Color>, BaseObject, bool> OfType<ObjectType>(Func<LoadedMap<Color>, ObjectType, bool> handler) where ObjectType : BaseObject
        {
            return (map, obj) => obj is ObjectType cast ? handler(map, cast) : false;
        }

        public static Func<LoadedMap<Color>, ObjectType, bool> WithProperty<ObjectType>(Func<LoadedMap<Color>, ObjectType, bool> handler, string property, string value) where ObjectType : BaseObject
        {
            return (map, obj) => obj.Properties[property].Equals(value) ? handler(map, obj) : false;
        }

        public static LoadedMap<Color>.ObjectLayerHandler Build<ObjectType>(this Func<LoadedMap<Color>, BaseObject, bool> handler) where ObjectType : BaseObject
        {
            return (map, obj) => handler(map, obj);
        }
        
        public static class Builder
        {
            //TODO
        }
    }
}
