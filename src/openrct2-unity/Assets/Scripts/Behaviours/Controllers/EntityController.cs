using System;
using OpenRCT2.Bindings.Entities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Abstract base class for shared code in moving entities around.
    /// </summary>
    public abstract class EntityController<TEntity> where TEntity : struct
    {
        const int _bucketSize = 256;
        const int _capacitySafeZone = 8;

        readonly Transform _parent;

        TEntity[] _entities = Array.Empty<TEntity>();
        GameObject[] _objects = Array.Empty<GameObject>();

        protected abstract int UpdateEntities(TEntity[] entities);
        protected abstract bool IsActive(in TEntity entity);
        protected abstract GameObject CreateEntity(int index);
        protected abstract void UpdateEntity(int index, in TEntity entity, GameObject gameObject);
        protected virtual void SetCapacity(int capacity)
        {
            // By default do nothing.
        }


        protected EntityController(EntityType type, Transform parent)
        {
            _parent = parent;

            var count = EntityRegistry.GetCount(type);
            EnsureCapacity(count);
        }

        /// <summary>
        /// Updates all controlled entities for the current tick.
        /// </summary>
        public void Update()
        {
            var count = UpdateEntities(_entities);
            EnsureCapacity(count); // new size will be used next update

            for (var idx = 0; idx < count; idx++)
            {
                ref var entity = ref _entities[idx];
                var obj = _objects[idx];

                if (IsActive(entity))
                {
                    if (obj == null)
                    {
                        _objects[idx] = obj = CreateEntity(idx);
                        obj.transform.parent = _parent;
                    }
                    else if (!obj.activeSelf)
                    {
                        obj.SetActive(true);
                    }

                    UpdateEntity(idx, entity, obj);
                }
                else if (obj != null && obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Ensures there is more than enough capacity in the buffer arrays.
        /// </summary>
        void EnsureCapacity(int count)
        {
            var suggestedCapacity = ((count + _capacitySafeZone) / _bucketSize + 1) * _bucketSize;

            if (suggestedCapacity > _entities.Length)
            {
                Debug.Log($"{GetType().Name} resized buffer to {suggestedCapacity}");
                Array.Resize(ref _entities, suggestedCapacity);
                Array.Resize(ref _objects, suggestedCapacity);
                SetCapacity(suggestedCapacity);
            }
        }
    }
}
