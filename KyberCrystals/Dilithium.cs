namespace KyberCrystals;


public class Dilithium
{
    private readonly DilithiumParams _param;

    public Dilithium(DilithiumParams param)
    {
        _param = param;
    }
    
    public void KeyGen()
    {
        throw new NotImplementedException();
    }
    
    public void Sign()
    {
        throw new NotImplementedException();
    }
    
    public void Verify()
    {
        throw new NotImplementedException();
    }
    
    # region Support Algorithms from figure 3
    
    private void Power2Round(int r, int d)
    {
        throw new NotImplementedException();
    }
    
    private void MakeHint(int z, int r, int a)
    {
        throw new NotImplementedException();
    }
    
    private void UseHint(int h, int r, int a)
    {
        throw new NotImplementedException();
    }
    
    private void Decompose(int r, int a)
    {
        throw new NotImplementedException();
    }
    
    private void HighBits(int r, int a)
    {
        throw new NotImplementedException();
    }
    
    private void LowBits(int r, int a)
    {
        throw new NotImplementedException();
    }
    
    # endregion
}