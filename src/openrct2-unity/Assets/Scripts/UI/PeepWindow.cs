using Lib;
using Sprites;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PeepWindow : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Slider happinessBar;
        [SerializeField] Slider energyBar;
        [SerializeField] Slider hungerBar;
        [SerializeField] Slider thirstBar;
        [SerializeField] Slider nauseaBar;
        [SerializeField] Slider toiletBar;

        ushort peepId;


        public void SetPeep(ushort peepId)
        {
            this.peepId = peepId;

            title.text = $"Guest {this.peepId}";
            InvokeRepeating(nameof(UpdateData), 0f, 5f);
        }


        void UpdateData()
        {
            PeepStats stats = new PeepStats();

            if (!OpenRCT2.GetPeepStats(peepId, ref stats))
            {
                Destroy(gameObject);
                return;
            }

            happinessBar.value = stats.happiness;
            energyBar.value = stats.energy;
            hungerBar.value = stats.hunger;
            thirstBar.value = stats.thirst;
            nauseaBar.value = stats.nausea;
            toiletBar.value = stats.toilet;
            /*
            var intensityBinary = System.Convert.ToString(peep.intensity, 2);
            var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
            var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);
            */
        }
    }
}
