namespace OpenRCT2.Unity
{
    /// <summary>
    /// Controller which moves and updates all ride vehicles in the park.
    /// </summary>
    public class VehicleController : SpriteController<Vehicle>
    {
        /// <summary>
        /// Reads all vehicles into the buffer from the dll hook.
        /// </summary>
        protected override int FillSpriteBuffer(Vehicle[] buffer)
            => OpenRCT2.GetAllVehicles(buffer);


        /// <summary>
        /// Updates the vehicle with the correct rotation.
        /// </summary>
        protected override SpriteObject UpdateSprite(int index, ref Vehicle sprite)
        {
            var obj = base.UpdateSprite(index, ref sprite);

            obj.gameObject.transform.rotation = sprite.Rotation;
            return obj;
        }
    }
}
