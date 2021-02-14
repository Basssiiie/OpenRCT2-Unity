using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

#nullable enable

namespace Sprites
{
    /// <summary>
    /// Abstract base class for shared code in moving sprites around.
    /// </summary>
    public abstract class SpriteController<TSprite> : MonoBehaviour where TSprite : ISprite
    {
        [SerializeField, Required] GameObject _spritePrefab = null!;

        const int MaxSprites = 8000;


        protected TSprite[] _spriteBuffer = Array.Empty<TSprite>();
        protected Dictionary<ushort, SpriteObject> _spriteObjects = null!;
        int _currentUpdateTick;


        /// <summary>
        /// Bind the method to fill the sprite buffer.
        /// </summary>
        protected abstract int FillSpriteBuffer(TSprite[] buffer);


        /// <summary>
        /// Initializes the sprite controller.
        /// </summary>
        void Start()
        {
            _spriteBuffer = new TSprite[MaxSprites];
            int amount = FillSpriteBuffer(_spriteBuffer);

            _spriteObjects = new Dictionary<ushort, SpriteObject>(amount);

            for (int i = 0; i < amount; i++)
            {
                AddSprite(i, _spriteBuffer[i]);
            }
        }


        /// <summary>
        /// Update ~40 per second to get the sprite information from OpenRCT2.
        /// </summary>
        void FixedUpdate()
        {
            _currentUpdateTick++;

            int amount = FillSpriteBuffer(_spriteBuffer);

            for (int i = 0; i < amount; i++)
            {
                UpdateSprite(i, _spriteBuffer[i]);
            }
        }


        /// <summary>
        /// Update and lerp the sprites every frame for smoother transitions.
        /// </summary>
        void LateUpdate()
        {
            foreach (var sprite in _spriteObjects.Values)
            {
                MoveSprite(sprite);
            }
        }


        /// <summary>
        /// Adds a new peep object to the dictionary.
        /// </summary>
        protected virtual SpriteObject AddSprite(int index, in TSprite sprite)
        {
            Vector3 position = sprite.Position;
            GameObject peepObj = Instantiate(_spritePrefab, position, Quaternion.identity, transform);

            SpriteObject instance = new SpriteObject(peepObj)
            {
                bufferIndex = index,
                from = position,
                towards = position
            };

            _spriteObjects.Add(sprite.Id, instance);
            return instance;
        }


        /// <summary>
        /// Sets the new start and end positions for this game tick.
        /// </summary>
        protected virtual SpriteObject UpdateSprite(int index, in TSprite sprite)
        {
            ushort id = sprite.Id;

            if (!_spriteObjects.TryGetValue(id, out SpriteObject obj))
            {
                obj = AddSprite(index, sprite);
            }
            else
            {
                obj.bufferIndex = index;
            }

            obj.lastUpdate = _currentUpdateTick;

            Vector3 target = sprite.Position;

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
            if (spriteObject.lastUpdate < _currentUpdateTick)
            {
                DisableSprite(spriteObject);
                return;
            }
            else
            {
                EnableSprite(spriteObject);
            }

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
            public readonly GameObject gameObject;
            public int bufferIndex;
            public Vector3 from;
            public Vector3 towards;
            public float timeSinceStart;
            public int lastUpdate;


            public SpriteObject(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }
        }
    }
}
