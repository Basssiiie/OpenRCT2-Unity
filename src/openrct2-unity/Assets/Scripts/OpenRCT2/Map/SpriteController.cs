using System.Collections.Generic;
using UnityEngine;

namespace OpenRCT
{
    /// <summary>
    /// Abstract base class for shared code in moving sprites around.
    /// </summary>
    public abstract class SpriteController<TSprite> : MonoBehaviour where TSprite : ISprite
    {
        [SerializeField] GameObject spritePrefab;

        const int MaxSprites = 8000;


        protected TSprite[] spriteBuffer;
        protected Dictionary<ushort, SpriteObject> spriteObjects;
        int currentUpdateTick;


        /// <summary>
        /// Bind the method to fill the sprite buffer.
        /// </summary>
        protected abstract int FillSpriteBuffer(TSprite[] buffer);


        /// <summary>
        /// Initializes the sprite controller.
        /// </summary>
        void Start()
        {
            spriteBuffer = new TSprite[MaxSprites];
            int amount = FillSpriteBuffer(spriteBuffer);

            spriteObjects = new Dictionary<ushort, SpriteObject>(amount);

            for (int i = 0; i < amount; i++)
            {
                AddSprite(i, ref spriteBuffer[i]);
            }
        }


        /// <summary>
        /// Update ~40 per second to get the sprite information from OpenRCT2.
        /// </summary>
        void FixedUpdate()
        {
            currentUpdateTick++;

            int amount = FillSpriteBuffer(spriteBuffer);

            for (int i = 0; i < amount; i++)
            {
                UpdateSprite(i, ref spriteBuffer[i]);
            }
        }


        /// <summary>
        /// Update and lerp the sprites every frame for smoother transitions.
        /// </summary>
        void LateUpdate()
        {
            foreach (var sprite in spriteObjects.Values)
            {
                MoveSprite(sprite);
            }
        }


        /// <summary>
        /// Adds a new peep object to the dictionary.
        /// </summary>
        protected virtual SpriteObject AddSprite(int index, ref TSprite sprite)
        {
            Vector3 position = Map.CoordsToVector3(sprite.Position);
            GameObject peepObj = Instantiate(spritePrefab, position, Quaternion.identity, transform);

            SpriteObject instance = new SpriteObject
            {
                bufferIndex = index,
                gameObject = peepObj,
                from = position,
                towards = position
            };

            spriteObjects.Add(sprite.Id, instance);
            return instance;
        }


        /// <summary>
        /// Sets the new start and end positions for this game tick.
        /// </summary>
        protected virtual SpriteObject UpdateSprite(int index, ref TSprite sprite)
        {
            ushort id = sprite.Id;

            if (!spriteObjects.TryGetValue(id, out SpriteObject obj))
            {
                obj = AddSprite(index, ref sprite);
            }
            else
            {
                obj.bufferIndex = index;
            }

            obj.lastUpdate = currentUpdateTick;

            Vector3 target = Map.CoordsToVector3(sprite.Position);

            if (obj.towards != target)
            {
                obj.from = obj.towards;
                obj.towards = target;
                obj.timeSinceStart = Time.timeSinceLevelLoad;
            }
            return obj;
        }


        /// <summary>
        /// Updates the actual object position by lerping between the information
        /// from last game tick.
        /// </summary>
        protected virtual void MoveSprite(SpriteObject spriteObject)
        {
            if (spriteObject.lastUpdate < currentUpdateTick)
            {
                DisableSprite(spriteObject);
                return;
            }
            else
                EnableSprite(spriteObject);

            Transform transf = spriteObject.gameObject.transform;

            if (transf.position == spriteObject.towards)
                return;

            // Update position
            float time = (Time.timeSinceLevelLoad - spriteObject.timeSinceStart) / Time.fixedDeltaTime;
            Vector3 lerped = Vector3.Lerp(transf.position, spriteObject.towards, time);

            transf.position = lerped;
        }


        /// <summary>
        /// Activates the gameobject of the sprite.
        /// </summary>
        void EnableSprite(SpriteObject spriteObject)
        {
            if (!spriteObject.gameObject.activeSelf)
                spriteObject.gameObject.SetActive(true);
        }


        /// <summary>
        /// Deactivates the gameobject of the sprite.
        /// </summary>
        void DisableSprite(SpriteObject spriteObject)
        {
            if (spriteObject.gameObject.activeSelf)
                spriteObject.gameObject.SetActive(false);
        }


        /// <summary>
        /// Internal sprite object.
        /// </summary>
        protected class SpriteObject
        {
            public int bufferIndex;
            public GameObject gameObject;
            public float timeSinceStart;
            public int lastUpdate;
            public Vector3 from;
            public Vector3 towards;
        }
    }
}
