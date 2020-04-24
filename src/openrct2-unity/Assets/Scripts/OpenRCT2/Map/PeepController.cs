using System.Collections.Generic;
using UnityEngine;

namespace OpenRCT2.Unity
{
    /// <summary>
    /// Controller which moves and updates all the peeps in the park.
    /// </summary>
    [RequireComponent(typeof(Map))]
    public class PeepController : MonoBehaviour
    {
        [SerializeField] GameObject peepPrefab;


        const int MaxPeeps = 8000;


        Peep[] peepBuffer;
        Dictionary<ushort, PeepObject> peepObjects;
        int currentUpdateTick;


        class PeepObject
        {
            public GameObject gameObject;
            public float timeSinceStart;
            public int lastUpdate;
            public Vector3 from;
            public Vector3 towards;
        }


        void Start()
        {
            peepBuffer = new Peep[MaxPeeps];
            int amount = OpenRCT2.GetAllPeeps(peepBuffer);
            peepObjects = new Dictionary<ushort, PeepObject>(amount);
            for (int i = 0; i < amount; i++)
            {
                AddPeep(ref peepBuffer[i]);
            }
        }


        void FixedUpdate()
        {
            currentUpdateTick++;

            int amount = OpenRCT2.GetAllPeeps(peepBuffer);

            for (int i = 0; i < amount; i++)
            {
                SetPeepPositions(ref peepBuffer[i]);
            }
        }


        void LateUpdate()
        {
            foreach (var peep in peepObjects.Values)
            {
                UpdatePeepPosition(peep);
            }
        }

        Color decodeColour(byte colour)
        {
            var colourRGB = new Color32(0, 0, 0, 1);
            switch (colour)
            {
                case 0:
                    colourRGB = new Color32(0, 0, 0, 1);
                    break;
                case 1:
                    colourRGB = new Color32(128, 128, 128, 1);
                    break;
                case 2:
                    colourRGB = new Color32(255, 255, 255, 1);
                    break;
                case 3:
                    colourRGB = new Color32(85, 26, 139, 1);
                    break;
                case 4:
                    colourRGB = new Color32(171, 130, 255, 1);
                    break;
                case 5:
                    colourRGB = new Color32(160, 32, 240, 1);
                    break;
                case 6:
                    colourRGB = new Color32(0, 0, 139, 1);
                    break;
                case 7:
                    colourRGB = new Color32(102, 102, 255, 1);
                    break;
                case 8:
                    colourRGB = new Color32(135, 206, 235, 1);
                    break;
                case 9:
                    colourRGB = new Color32(0, 128, 128, 1);
                    break;
                case 10:
                    colourRGB = new Color32(127, 255, 212, 1);
                    break;
                case 11:
                    colourRGB = new Color32(124, 205, 124, 1);
                    break;
                case 12:
                    colourRGB = new Color32(0, 100, 0, 1);
                    break;
                case 13:
                    colourRGB = new Color32(110, 139, 61, 1);
                    break;
                case 14:
                    colourRGB = new Color32(0, 255, 0, 1);
                    break;
                case 15:
                    colourRGB = new Color32(192, 255, 62, 1);
                    break;
                case 16:
                    colourRGB = new Color32(85, 107, 47, 1);
                    break;
                case 17:
                    colourRGB = new Color32(255, 255, 0, 1);
                    break;
                case 18:
                    colourRGB = new Color32(139, 139, 0, 1);
                    break;
                case 19:
                    colourRGB = new Color32(255, 165, 0, 1);
                    break;
                case 20:
                    colourRGB = new Color32(255, 127, 0, 1);
                    break;
                case 21:
                    colourRGB = new Color32(244, 164, 96, 1);
                    break;
                case 22:
                    colourRGB = new Color32(165, 42, 42, 1);
                    break;
                case 23:
                    colourRGB = new Color32(205, 170, 125, 1);
                    break;
                case 24:
                    colourRGB = new Color32(61, 51, 37, 1);
                    break;
                case 25:
                    colourRGB = new Color32(255, 160, 122, 1);
                    break;
                case 26:
                    colourRGB = new Color32(205, 55, 0, 1);
                    break;
                case 27:
                    colourRGB = new Color32(200, 0, 0, 1);
                    break;
                case 28:
                    colourRGB = new Color32(255, 0, 0, 1);
                    break;
                case 29:
                    colourRGB = new Color32(205, 16, 118, 1);
                    break;
                case 30:
                    colourRGB = new Color32(255, 105, 180, 1);
                    break;
                case 31:
                    colourRGB = new Color32(255, 174, 185, 1);
                    break;
                default:
                    break;
            }
            return colourRGB;
        }

        public void UpdateColours(GameObject peepObj, Peep peep)
        {

            GameObject tshirt = peepObj.transform.GetChild(0).gameObject;
            GameObject trousers = peepObj.transform.GetChild(1).gameObject;

            var tshirtRenderer = tshirt.GetComponent<Renderer>();
            var trousersRenderer = trousers.GetComponent<Renderer>();

            tshirtRenderer.material.color = decodeColour(peep.tshirtColour);
            trousersRenderer.material.color = decodeColour(peep.trousersColour);

        }

        /// <summary>
        /// Adds a new peep object to the dictionary.
        /// </summary>
        PeepObject AddPeep(ref Peep peep)
        {
            ushort id = peep.Id;
            var type = peep.type;
            GameObject peepObj = Instantiate(peepPrefab, Vector3.zero, Quaternion.identity, transform);

            peepObj.name = $"{type} {id}";

            UpdateColours(peepObj, peep);

            Vector3 position = Map.CoordsToVector3(peep.Position);

            PeepObject instance = new PeepObject
            {
                gameObject = peepObj,
                from = position,
                towards = position
            };

            peepObjects.Add(peep.Id, instance);
            return instance;
        }


        /// <summary>
        /// Sets the new start and end positions for this game tick.
        /// </summary>
        void SetPeepPositions(ref Peep peep)
        {
            ushort id = peep.Id;

            if (!peepObjects.TryGetValue(id, out PeepObject obj))
                obj = AddPeep(ref peep);

            obj.lastUpdate = currentUpdateTick;

            Vector3 target = Map.CoordsToVector3(peep.Position);

            if (obj.towards == target)
                return;

            obj.from = obj.towards;
            obj.towards = target;
            obj.timeSinceStart = Time.timeSinceLevelLoad;
        }


        /// <summary>
        /// Updates the actual object position by lerping between the information
        /// from last game tick.
        /// </summary>
        void UpdatePeepPosition(PeepObject obj)
        {
            if (obj.lastUpdate < currentUpdateTick)
            {
                DisablePeep(obj);
                return;
            }
            else
                EnablePeep(obj);

            Transform transf = obj.gameObject.transform;

            if (transf.position == obj.towards)
                return;

            // Update position
            float time = (Time.timeSinceLevelLoad - obj.timeSinceStart) / Time.fixedDeltaTime;
            Vector3 lerped = Vector3.Lerp(transf.position, obj.towards, time);

            transf.position = lerped;

            // Update rotation
            Vector3 forward = new Vector3(obj.from.x - obj.towards.x, 0, obj.from.z - obj.towards.z);

            if (forward != Vector3.zero)
                transf.rotation = Quaternion.LookRotation(forward);

        }


        void EnablePeep(PeepObject obj)
        {
            if (!obj.gameObject.activeSelf)
                obj.gameObject.SetActive(true);
        }


        void DisablePeep(PeepObject obj)
        {
            if (obj.gameObject.activeSelf)
                obj.gameObject.SetActive(false);
        }
    }
}
