namespace Majaja.Utilities {

    public static class MathTools {

        public static int Clamp (int value, int min, int max) {
            if (min > max)
                return value;

            if (value < min)
                value = min;
            else if (value > max)
                value = max;

            return value;
        }

    }
}
