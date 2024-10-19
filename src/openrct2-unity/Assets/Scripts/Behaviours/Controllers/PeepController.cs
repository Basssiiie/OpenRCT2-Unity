using System;
using OpenRCT2.Bindings.Entities;
using OpenRCT2.Generators;
using OpenRCT2.Generators.Sprites;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Abstract controller which moves and updates all the peeps in the park.
    /// </summary>
    public abstract class PeepController : EntityController<PeepEntity>
    {
        struct PeepState
        {
            public MeshRenderer renderer;
            public int animation;
        }

        static readonly int _paletteKey = Shader.PropertyToID("_Palette");
        static readonly int _animationKey = Shader.PropertyToID("_Animation");
        static readonly int _lengthKey = Shader.PropertyToID("_Length");
        static readonly int _sizeKey = Shader.PropertyToID("_Size");
        static readonly int _rotationsCountKey = Shader.PropertyToID("_RotationsCount");
        static readonly int _rotationKey = Shader.PropertyToID("_Rotation");
        static readonly int _offsetKey = Shader.PropertyToID("_Offset");

        readonly EntityType _type;
        readonly GameObject _prefab;

        PeepState[] _state = Array.Empty<PeepState>();


        /// <inheritdoc/>
        protected PeepController(EntityType type, Transform parent, GameObject prefab)
            : base(type, parent)
        {
            _type = type;
            _prefab = prefab;
        }

        /// <inheritdoc/>
        protected override bool IsActive(in PeepEntity entity)
        {
            return entity.x > 0;
        }

        /// <inheritdoc/>
        protected override GameObject CreateEntity(int index)
        {
            var obj = GameObject.Instantiate(_prefab);
            obj.name = $"{_type} {index}";

            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.material.SetTexture(_paletteKey, PaletteFactory.GetPalette());

            _state[index].renderer = renderer;
            return obj;
        }

        /// <inheritdoc/>
        protected override void UpdateEntity(int index, in PeepEntity entity, GameObject gameObject)
        {
            gameObject.transform.localPosition = World.CoordsToVector3(entity.x, entity.z, entity.y);

            ref var state = ref _state[index];
            var material = state.renderer.material;
            var animationKey = HashCode.Combine(entity.animationGroup, entity.animationType, entity.tshirtColour, entity.trousersColour, entity.accessoryColour);

            if (animationKey != state.animation)
            {
                var animation = PeepAnimationsFactory.GetOrCreate(entity.animationGroup, entity.animationType, entity.tshirtColour, entity.trousersColour, entity.accessoryColour);
                var frames = animation.frames;

                material.SetTexture(_animationKey, frames);
                material.SetFloat(_lengthKey, animation.data.length);
                material.SetVector(_sizeKey, new Vector2(frames.width, frames.height));
                material.SetFloat(_rotationsCountKey, animation.data.rotations);

                state.animation = animationKey;
            }

            material.SetFloat(_rotationKey, entity.direction);
            material.SetFloat(_offsetKey, entity.animationOffset);
        }

        /// <inheritdoc/>
        protected override void SetCapacity(int capacity)
        {
            Array.Resize(ref _state, capacity);
        }
    }
}
