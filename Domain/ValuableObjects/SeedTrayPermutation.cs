namespace Domain.ValuableObjects;

//LATER - Document this class
public class SeedTrayPermutation
{
    private int _firstSeedTrayID;
    private string _firstSeedTrayName;
    private int _firstAmount;
    private int _secondSeedTrayID;
    private string _secondSeedTrayName;
    private int _secondAmount;
    private int _thirdSeedTrayID;
    private string _thirdSeedTrayName;
    private int _thirdAmount;

    public SeedTrayPermutation(int pFirstSeedTrayID, string pfirstSeedTrayName, int pFirstAmount
        , int pSecondSeedTrayID2 = 0, string pSecondSeedTrayName = "", int pSecondAmount = 0
        , int pThirdSeedTrayID3 = 0, string pThirdSeedTrayName = "", int pThirdAmount = 0)
    {
        _firstSeedTrayID = pFirstSeedTrayID;
        _firstSeedTrayName = pfirstSeedTrayName;
        _firstAmount = pFirstAmount;
        _secondSeedTrayID = pSecondSeedTrayID2;
        _secondSeedTrayName = pSecondSeedTrayName;
        _secondAmount = pSecondAmount;
        _thirdSeedTrayID = pThirdSeedTrayID3;
        _thirdSeedTrayName = pThirdSeedTrayName;
        _thirdAmount = pThirdAmount;
    }

    public SeedTrayPermutation(SeedTrayPermutation pSeedTrayPermutationOriginal)
    {
        this._firstSeedTrayID = pSeedTrayPermutationOriginal.FirstSeedTrayID;
        this._firstSeedTrayName = pSeedTrayPermutationOriginal._firstSeedTrayName;
        this._firstAmount = pSeedTrayPermutationOriginal.FirstAmount;
        this._secondSeedTrayID = pSeedTrayPermutationOriginal.SecondSeedTrayID;
        this._secondSeedTrayName = pSeedTrayPermutationOriginal.SecondSeedTrayName;
        this._secondAmount = pSeedTrayPermutationOriginal.SecondAmount;
        this._thirdSeedTrayID = pSeedTrayPermutationOriginal.ThirdSeedTrayID;
        this._thirdSeedTrayName = pSeedTrayPermutationOriginal.ThirdSeedTrayName;
        this._thirdAmount = pSeedTrayPermutationOriginal.ThirdAmount;
    }

    public int FirstSeedTrayID { get => _firstSeedTrayID; set => _firstSeedTrayID = value; }

    public string FirstSeedTrayName { get => _firstSeedTrayName; set => _firstSeedTrayName = value; }

    public int FirstAmount { get => _firstAmount; set => _firstAmount = value; }

    public int SecondSeedTrayID { get => _secondSeedTrayID; set => _secondSeedTrayID = value; }

    public string SecondSeedTrayName { get => _secondSeedTrayName; set => _secondSeedTrayName = value; }

    public int SecondAmount { get => _secondAmount; set => _secondAmount = value; }

    public int ThirdSeedTrayID { get => _thirdSeedTrayID; set => _thirdSeedTrayID = value; }

    public string ThirdSeedTrayName { get => _thirdSeedTrayName; set => _thirdSeedTrayName = value; }

    public int ThirdAmount { get => _thirdAmount; set => _thirdAmount = value; }
}
