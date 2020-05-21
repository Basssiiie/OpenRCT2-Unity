using Lib;
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

        PeepController peepController;
        ushort peepId;


        public void LoadPeepController(PeepController peepController, ushort peepId)
        {
            this.peepId = peepId;
            this.peepController = peepController;

            title.text = $"Guest {this.peepId}";
            InvokeRepeating(nameof(UpdateData), 0f, 5f);
        }


        void UpdateData()
        {
            Peep? peep = peepController.GetPeepById(peepId);
            if (peep == null)
            {
                Destroy(gameObject);
                return;
            }

            Peep value = peep.Value;

            happinessBar.value = value.happiness;
            energyBar.value = value.energy;
            hungerBar.value = value.hunger;
            thirstBar.value = value.thirst;
            nauseaBar.value = value.nausea;
            toiletBar.value = value.toilet;

            /*
            var intensityBinary = System.Convert.ToString(peep.intensity, 2);
            var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
            var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);
            */
        }
    }
}
