using OpenRCT2.Bindings.Entities;
using OpenRCT2.Utilities;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace UI
{
    public class PeepWindow : MonoBehaviour
    {
        [SerializeField, Required] Text _title = null!;
        [SerializeField, Required] Slider _happinessBar = null!;
        [SerializeField, Required] Slider _energyBar = null!;
        [SerializeField, Required] Slider _hungerBar = null!;
        [SerializeField, Required] Slider _thirstBar = null!;
        [SerializeField, Required] Slider _nauseaBar = null!;
        [SerializeField, Required] Slider _toiletBar = null!;

        ushort _peepId;


        public void SetPeep(ushort peepId)
        {
            _peepId = peepId;

            _title.text = $"Guest {_peepId}";
            InvokeRepeating(nameof(UpdateData), 0f, 5f);
        }


        void UpdateData()
        {
            if (!EntityRegistry.GetGuestStats(_peepId, out GuestStats stats))
            {
                Destroy(gameObject);
                return;
            }

            _happinessBar.value = stats.happiness;
            _energyBar.value = stats.energy;
            _hungerBar.value = stats.hunger;
            _thirstBar.value = stats.thirst;
            _nauseaBar.value = stats.nausea;
            _toiletBar.value = stats.toilet;
            /*
            var intensityBinary = System.Convert.ToString(peep.intensity, 2);
            var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
            var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);
            */
        }
    }
}
