namespace Domain.ValuableObjects
{
    public class SeedTrayPermutation
    {
        private int _firstSeedTrayID;
        private int _firstAmount;
        private int _secondSeedTrayID;
        private int _secondAmount;
        private int _thirdSeedTrayID;
        private int _thirdAmount;

        public SeedTrayPermutation(int pFirstSeedTrayID, int pFirstAmount)
        {
            _firstSeedTrayID = pFirstSeedTrayID;
            _firstAmount = pFirstAmount;
            _secondSeedTrayID = 0;
            _secondAmount = 0;
            _thirdSeedTrayID = 0;
            _thirdAmount = 0;
        }

        public SeedTrayPermutation(int pFirstSeedTrayID, int pFirstAmount, int pSecondSeedTrayID2, int pSecondAmount)
        {
            _firstSeedTrayID = pFirstSeedTrayID;
            _firstAmount = pFirstAmount;
            _secondSeedTrayID = pSecondSeedTrayID2;
            _secondAmount = pSecondAmount;
            _thirdSeedTrayID = 0;
            _thirdAmount = 0;
        }

        public SeedTrayPermutation(int pFirstSeedTrayID, int pFirstAmount, int pSecondSeedTrayID2, int pSecondAmount, int pThirdSeedTrayID3, int pThirdAmount)
        {
            _firstSeedTrayID = pFirstSeedTrayID;
            _firstAmount = pFirstAmount;
            _secondSeedTrayID = pSecondSeedTrayID2;
            _secondAmount = pSecondAmount;
            _thirdSeedTrayID = pThirdSeedTrayID3;
            _thirdAmount = pThirdAmount;
        }

        public SeedTrayPermutation(SeedTrayPermutation pSeedTrayPermutationOriginal)
        {
            this._firstSeedTrayID = pSeedTrayPermutationOriginal.FirstSeedTrayID;
            this._firstAmount = pSeedTrayPermutationOriginal.FirstAmount;
            this._secondSeedTrayID = pSeedTrayPermutationOriginal.SecondSeedTrayID;
            this._secondAmount = pSeedTrayPermutationOriginal.SecondAmount;
            this._thirdSeedTrayID = pSeedTrayPermutationOriginal.ThirdSeedTrayID;
            this._thirdAmount = pSeedTrayPermutationOriginal.ThirdAmount;
        }
        public int FirstSeedTrayID { get => _firstSeedTrayID; set => _firstSeedTrayID = value; }

        public int FirstAmount { get => _firstAmount; set => _firstAmount = value; }

        public int SecondSeedTrayID { get => _secondSeedTrayID; set => _secondSeedTrayID = value; }

        public int SecondAmount { get => _secondAmount; set => _secondAmount = value; }

        public int ThirdSeedTrayID { get => _thirdSeedTrayID; set => _thirdSeedTrayID = value; }

        public int ThirdAmount { get => _thirdAmount; set => _thirdAmount = value; }
    }
}
