using UnityEngine;

[CreateAssetMenu]
public class MaterialData : ItemData
{
    public int dateToExp;
    public bool isExp;
    
    public void OnExpDate(){
        isExp = true;
    }

    public void OnDateChange(){
        if (dateToExp > 0)
            dateToExp -= 1;
        
        if (dateToExp == 0)
            OnExpDate();
    }

    public new int GetDateToExp(){
        return dateToExp;
    }
}
