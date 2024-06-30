namespace Domain.ValuableObjects;

/// <summary>
/// This class represent the different permutations the seedtrays could make until 3 level items.
/// </summary>
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

    /// <summary>
    /// Creates a new instance with only the items passed as arguments.
    /// </summary>
    /// <param name="pFirstSeedTrayID">Id of the first <c>SeedTray</c> type.</param>
    /// <param name="pfirstSeedTrayName">Name of the first <c>SeedTray</c> type.</param>
    /// <param name="pFirstAmount">Amount of the first <c>SeedTray</c> type.</param>
    /// <param name="pSecondSeedTrayID2">Id of the second <c>SeedTray</c> type.</param>
    /// <param name="pSecondSeedTrayName">Name of the second <c>SeedTray</c> type.</param>
    /// <param name="pSecondAmount">Amount of the second <c>SeedTray</c> type.</param>
    /// <param name="pThirdSeedTrayID3">Id of the third <c>SeedTray</c> type.</param>
    /// <param name="pThirdSeedTrayName">Name of the third <c>SeedTray</c> type.</param>
    /// <param name="pThirdAmount">Amount of the third <c>SeedTray</c> type.</param>
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

    /// <summary>
    /// Creates a new copy instance of another <c>SeedTray</c> object.
    /// </summary>
    /// <param name="pSeedTrayPermutationOriginal">The original <c>SeedTray</c> object.</param>
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

    /// <summary>
    /// Gets or sets the Id of the first <c>SeedTray</c> type.
    /// </summary>
    public int FirstSeedTrayID { get => _firstSeedTrayID; set => _firstSeedTrayID = value; }

    /// <summary>
    /// Gets or sets the Name of the first <c>SeedTray</c> type.
    /// </summary>
    public string FirstSeedTrayName { get => _firstSeedTrayName; set => _firstSeedTrayName = value; }

    /// <summary>
    /// Gets or sets the Amount of the first <c>SeedTray</c> type.
    /// </summary>
    public int FirstAmount { get => _firstAmount; set => _firstAmount = value; }

    /// <summary>
    /// Gets or sets the Id of the second <c>SeedTray</c> type.
    /// </summary>
    public int SecondSeedTrayID { get => _secondSeedTrayID; set => _secondSeedTrayID = value; }

    /// <summary>
    /// Gets or sets the Name of the second <c>SeedTray</c> type.
    /// </summary>
    public string SecondSeedTrayName { get => _secondSeedTrayName; set => _secondSeedTrayName = value; }

    /// <summary>
    /// Gets or sets the Amount of the second <c>SeedTray</c> type.
    /// </summary>
    public int SecondAmount { get => _secondAmount; set => _secondAmount = value; }

    /// <summary>
    /// Gets or sets the Id of the third <c>SeedTray</c> type.
    /// </summary>
    public int ThirdSeedTrayID { get => _thirdSeedTrayID; set => _thirdSeedTrayID = value; }

    /// <summary>
    /// Gets or sets the Name of the third <c>SeedTray</c> type.
    /// </summary>
    public string ThirdSeedTrayName { get => _thirdSeedTrayName; set => _thirdSeedTrayName = value; }

    /// <summary>
    /// Gets or sets the Amount of the third <c>SeedTray</c> type.
    /// </summary>
    public int ThirdAmount { get => _thirdAmount; set => _thirdAmount = value; }
}
