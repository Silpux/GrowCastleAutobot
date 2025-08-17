using gca.Enums;

namespace gca.Classes
{
    public class Loop30Detector
    {

        private const int MIN_LOWER_VALUES_FOR_DETECTION = 5;

        private bool seen30 = false;
        private int smallerCount = 0;

        private int processCount = 0;

        public bool Process(int value)
        {

            if (value == -1)
            {
                return false;
            }

            processCount = Math.Min(processCount + 1, MIN_LOWER_VALUES_FOR_DETECTION + 1);

            if (value < (int)CrystalsAmount._30)
            {
                smallerCount++;
                seen30 = false;
            }
            else if (value == (int)CrystalsAmount._30)
            {
                bool enoughSmallerCount = smallerCount >= MIN_LOWER_VALUES_FOR_DETECTION;
                smallerCount = 0;

                if (!seen30 && (enoughSmallerCount || processCount <= MIN_LOWER_VALUES_FOR_DETECTION))
                {
                    seen30 = true;
                    return true;
                }
            }
            return false;
        }
    }
}
